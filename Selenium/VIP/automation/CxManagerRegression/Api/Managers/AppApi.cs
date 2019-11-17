using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Api.Controllers;
using Api.Resources;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Microsoft.WindowsAzure.Storage.Blob;
using Models.Apps;
using Models.Apps.ActualAppVersions;
using Models.Users;
using Newtonsoft.Json;
using RestSharp;

namespace Api.Managers
{
    /// <summary>
    /// Requests to CX Manager API, endpoint group App
    /// </summary>
    public static class AppApi
    {
        /// <summary>
        /// Deletes apps (or all of specified type(s)) within all or specified tenant
        /// </summary>
        /// <param name="deleteCompletely">Optional. If true, delete app(s) of the type(s) specified. 
        /// If false, mark the app(s) version(s) as deleted.</param>
        /// <param name="appTypes">App types array (optional, use null to delete all app types)
        /// </param>
        /// <param name="tenantTitle">Tenant title (optional)</param>
        public static void DeleteApps(bool deleteCompletely = true,
            string[] appTypes = null, 
            TenantTitle tenantTitle = TenantTitle.All)
        {
            var tenantList = tenantTitle == TenantTitle.All
                ? ActionManager.Tenants.ToArray()
                : ActionManager.Tenants
                    .Where(x => x.Title == tenantTitle.ToString())
                    .ToArray();
            if (appTypes == null || appTypes.Length == 0)
            {
                appTypes = new [] { AppTitle.Any };
            }

            BackgroundTaskApi.DeleteAllTasksAsync(TestConfig.AdminUser)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            foreach (var tenant in tenantList)
            {
                var response = RestController.HttpRequestJson(
                    UriCxm.Apps, Method.GET, tenantCode: tenant.Code, user: TestConfig.AdminUser);
                var apps = JsonConvert.DeserializeObject<AppResponse[]>(response.Content);
                foreach (var app in apps)
                {
                    if (appTypes.All(x => x.ToString() != AppTitle.Any) 
                        && appTypes.All(x => x.ToString() != app.ActualAppVersion.Title) 
                        && appTypes.All(x => x.ToString() != app.Key)
                        && !app.ActualAppVersion.Title.Contains("Auto"))
                    {
                        continue;
                    }

                    var app1 = GetById(app.AppId, tenant.Code, TestConfig.AdminUser);

                    if (app1.Places != null && app1.Places.Count > 0)
                    {                         
                        foreach (var place in app1.Places)
                        {
                            try
                            {
                                var p = PlaceApi.GetById(place.Id, tenant.Code);
                                p.Schedule.ScheduleApps.ForEach(x => x.DoDelete = true);
                                PlaceApi.SavePlace(p);
                            }
                            catch
                            {
                                // ignored
                            }
                        }

                        app1 = GetById(app.AppId, tenant.Code, TestConfig.AdminUser);
                    }

                    foreach (var version in app1.Versions)
                    {
                        RestController.HttpRequestJson(
                            new Uri(
                                    string.Format(
                                        UriCxm.AppsDelete,
                                        version.Id,
                                        DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss"),
                                        deleteCompletely.ToString().ToLower()),
                                    UriKind.Relative)
                                .ToString(),
                            Method.DELETE,
                            tenantCode: tenant.Code,
                            user: TestConfig.AdminUser);
                    }
                }
            }
        }

        /// <summary>
        /// Imports app from file by app version in file name
        /// </summary>
        /// <param name="path">Full path to file</param>
        /// <param name="mask">File name mask (like name*.zip) or file name without path</param>
        /// <param name="version">App version</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        public static AppResponse ImportApp(string path, string mask, string version)
        {
            var appFilePath = string.Empty;
            var token = BackgroundTaskApi.GetPackageSas();

            try
            {
                var blob = new CloudAppendBlob(new Uri(token));
                appFilePath = FileManager.GetFileByVersion(path, mask, version);
                blob.UploadFromFile(appFilePath);
                blob.SetProperties();
            }
            catch (Exception e)
            {
                throw new FileLoadException(
                    $"App API: Error uploading file {appFilePath} to cloud: {e.Message}");
            }

            var fileName = Path.GetFileName(appFilePath);
            var task = BackgroundTaskApi.NewBackgroundTask(TaskActionType.UploadApp, fileName,
                new FileInfo(appFilePath).Length,
                token);
            if (task == null || string.IsNullOrEmpty(task.TaskId))
            {
                throw new Exception($"Could not start new background task for app '{fileName}' import");
            }
            var body = new AppImport
            {
                PackageSas = token,
                TaskId = task.TaskId
            };

            BackgroundTaskApi.ProcessedObjects.AddOrUpdate(task.TaskId,
                (null, null, new ManualResetEvent(false)),
                (k, v) => (null, null, new ManualResetEvent(false)));
            Trace.TraceInformation($"App API: Import task {task.TaskId} started");

            RestController.HttpRequestJson(UriCxm.BackgroundTaskExecution, Method.POST, body);

            if (BackgroundTaskApi.ProcessedObjects.TryGetValue(task.TaskId, out var appObj) &&
                appObj.Item3.WaitOne(TimeSpan.FromSeconds(TestConfig.AppImportTimeout)))
            {
                Trace.TraceInformation($"App API: Import task {task.TaskId} complete");
            }
            else
            {
                Trace.TraceError($"App API: {task.TaskId} timed out!");
                throw new TimeoutException(
                    $"App API: Import on tenant {ActionManager.CurrentTenantCode} timed out after " +
                    $"{TestConfig.AppImportTimeout} s. File {fileName}.");
            }

            var result = BackgroundTaskApi.ProcessedObjects.TryGetValue(task.TaskId, out appObj) ?
                appObj.Item1 as AppResponse :
                null;
            BackgroundTaskApi.ProcessedObjects.TryRemove(task.TaskId, out _);

            return result;
        }

        /// <summary>
        /// Deletes specified app version. If no more uploaded app versions are left after removal, 
        /// the app will be fully removed. If some versions are left, the latest one will be set as
        /// actual app version.
        /// </summary>
        /// <param name="app">App object</param>
        /// <param name="version">App version (e.g. "0.0.1")</param>
        /// <param name="isDeleteCompletely">Optional. If true, removes the app version completely.
        /// If false, marks the version as deleted.</param>
        public static void DeleteAppVersion(AppResponse app, string version, bool isDeleteCompletely = true)
        {
            var ver = app?.Versions?
                .FirstOrDefault(x => string.Join(".", x.Major, x.Minor, x.Revision) == version 
                                      && x.Status != (int) AppStatus.Deleted);
            if (ver != null)
            {
                if (ver.Status == (int) AppStatus.Published)
                {
                    foreach (var place in app.Places)
                    {
                        try
                        {
                            var p = PlaceApi.GetById(place.Id);
                            p.Schedule.ScheduleApps.ForEach(x => x.DoDelete = true);
                            PlaceApi.SavePlace(p);
                        }
                        catch
                        {
                            // ignored
                        }
                    }   
                }

                app = GetById(app.AppId, user: TestConfig.AdminUser);

                if (ver.Status == (int) AppStatus.New && !isDeleteCompletely)
                {
                    app.Versions.Single(x => string.Join(".", x.Major, x.Minor, x.Revision) == version).Status = 
                        (int) AppStatus.Available;
                    if (string.Join(".", app.ActualAppVersion.Major, app.ActualAppVersion.Minor, app.ActualAppVersion.Revision) == version 
                        && app.ActualAppVersion.Status == (int) AppStatus.New)
                    {
                        app.ActualAppVersion.Status = (int) AppStatus.Available;
                    }
                    app = SaveApp(AppResponseToRequest(app));
                }

                try
                {
                    RestController.HttpRequestJson(
                        new Uri(string.Format(UriCxm.AppsDelete, ver.Id, app.Updated, isDeleteCompletely),
                                UriKind.Relative)
                            .ToString(),
                        Method.DELETE,
                        user: TestConfig.AdminUser);
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Uploads image file to storage
        /// </summary>
        /// <param name="pathFile">Full path to picture file</param>
        /// <returns>(<see cref="AppImage"/>) App image object</returns>
        public static AppImage UploadImage(string pathFile)
        {
            string hash, token;
            using (var stream = File.Open(pathFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                hash = RestController.GetMd5Hash(stream);

                var resp1 = RestController.HttpRequestJson(
                    UriCxm.AppsGetImageSas,
                    Method.GET);
                token = JsonConvert.DeserializeObject<string>(resp1.Content);

                var blob = new CloudBlockBlob(new Uri(token));
                stream.Position = 0;
                blob.UploadFromStream(stream);
                blob.SetProperties();
            }

            var response = RestController.HttpRequestJson(
                UriCxm.AppsCreateThumbnails,
                Method.POST,
                new
                {
                    hash = hash,
                    sas = token
                }
            );
            var tnUrl = JsonConvert.DeserializeObject<string>(response.Content);
            var result = new AppImage
            {
                ImageName = hash,
                FullImageUrl = string.Empty,
                ShowImageUrl = tnUrl,
                ThumbnailUrl = tnUrl
            };

            return result;
        }

        /// <summary>
        /// Returns app object by app ID
        /// </summary>
        /// <param name="appId">Property "AppId" of app object</param>
        /// <param name="tenantCode">Tenant code (optional)</param>
        /// <param name="user">User that makes the API request</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        public static AppResponse GetById(long appId, string tenantCode = null, 
            User user = null)
        {
            user = user ?? ActionManager.CurrentUser;
            var response = RestController.HttpRequestJson(string.Format(UriCxm.AppById, appId),
                Method.GET, tenantCode: tenantCode, user: user);
            var app = JsonConvert.DeserializeObject<AppResponse>(response.Content);
            return app;
        }

        /// <summary>
        /// Get app object if it exists
        /// </summary>
        /// <param name="type">App type</param>
        /// <param name="version">App version (optional)</param>
        /// <returns>(<see cref="AppResponse"/>) App object or null if not found</returns>
        public static AppResponse GetApp(
            string type,
            string version = null)
        {
            var response = RestController.HttpRequestJson(UriCxm.Apps, Method.GET);
            var apps = JsonConvert.DeserializeObject<AppResponse[]>(response.Content);
            if (type != AppTitle.Any)
            {
                apps = apps
                    .AsParallel()
                    .Where(x => x.ActualAppVersion.Title == type)
                    .ToArray();
            }

            if (!string.IsNullOrEmpty(version) && apps.Length > 0)
            {
                foreach (var app in apps)
                {
                    foreach (var version1 in app.Versions)
                    {
                        var av = GetAppVersion(version1.Id);
                        if (string.Join(".", av.Major, av.Minor, av.Revision) == version)
                        {
                            return GetById(app.AppId);
                        }
                    }
                }

                return null;
            }

            var app1 = apps?.AsParallel().AsOrdered().OrderBy(x => x.AppId).LastOrDefault();
            return app1 == null ? null : GetById(app1.AppId);
        }

        /// <summary>
        /// Get app version data by app version ID
        /// </summary>
        /// <param name="appVersionId">App version ID</param>
        /// <returns>(<see cref="ActualAppVersionResponse"/>) Actual app version object</returns>
        public static ActualAppVersionResponse GetAppVersion(long appVersionId)
        {
            var response = RestController.HttpRequestJson(
                string.Format(UriCxm.AppsGetVersion, appVersionId),
                Method.GET);
            return JsonConvert.DeserializeObject<ActualAppVersionResponse>(response.Content);
        }

        /// <summary>
        /// Saves app data
        /// </summary>
        /// <param name="app">App object</param>
        /// <param name="isOverwriteItems">Should items be overwritten or not (optional)</param>
        /// <param name="tenantCode">Tenant code (optional)</param>
        /// <param name="user">User that makes the request to API</param>
        /// <returns>(<see cref="AppResponse"/>) Actual app version object</returns>
        public static AppResponse SaveApp(AppRequest app, bool isOverwriteItems = false, string tenantCode = null,
            User user = null)
        {
            user = user ?? ActionManager.CurrentUser;
            app.ActualAppVersion.Updated = app.Updated = DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss");
            RestController.HttpRequestJson(
                string.Format(UriCxm.AppsSave, isOverwriteItems), Method.POST, app, tenantCode, user);
            ActionManager.CloseAlert();
            ActionManager.TurnOffInfoPopups();
            return GetById(app.AppId, tenantCode, user);
        }

        /// <summary>
        /// Converts <see cref="AppResponse"/> object to <see cref="AppRequest"/> object
        /// </summary>
        /// <param name="app"><see cref="AppResponse"/> object</param>
        /// <returns>(<see cref="AppRequest"/>) App object for API request</returns>
        public static AppRequest AppResponseToRequest(AppResponse app)
        {
            var appReq = new AppRequest
            {
                AppId = app.AppId,
                Created = app.Created,
                CreatedBy = app.CreatedBy,
                CreatedName = app.CreatedName,
                Description = app.Description,
                FullImageUrl = app.FullImageUrl,
                ImageName = app.ImageName,
                Key = app.Key,
                Places = app.Places,
                PushableUsersCount = app.PushableUsersCount,
                ShowFullImageUrl = app.ShowFullImageUrl,
                ShowImageUrl = app.ShowImageUrl,
                StatusName = app.StatusName,
                ThumbnailUrl = app.ThumbnailUrl,
                Title = app.Title,
                Type = app.Type,
                Updated = DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss"),
                UpdatedBy = app.UpdatedBy,
                Tags = app.Tags,
                Versions = app.Versions,
                ActualAppVersion = new ActualAppVersionRequest
                {
                    AllowedApis = app.ActualAppVersion.AllowedApis,
                    ApiKey = app.ActualAppVersion.ApiKey,
                    AppType = app.ActualAppVersion.AppType,
                    AppUserEmail = app.ActualAppVersion.AppUserEmail,
                    Created = app.ActualAppVersion.Created,
                    CreatedBy = app.ActualAppVersion.CreatedBy,
                    DeviceTypes = app.ActualAppVersion.DeviceTypes,
                    FullImageUrl = app.ActualAppVersion.FullImageUrl,
                    I18N = new List<I18n>(),
                    Id = app.ActualAppVersion.Id,
                    ImageName = app.ActualAppVersion.ImageName,
                    Major = app.ActualAppVersion.Major,
                    MasterData = app.ActualAppVersion.MasterData,
                    MasterDataValues = app.ActualAppVersion.MasterDataValues,
                    Minor = app.ActualAppVersion.Minor,
                    OverwritableItems = app.ActualAppVersion.OverwritableItems,
                    PackageItems = app.ActualAppVersion.PackageItems,
                    ParametersSchema = app.ActualAppVersion.ParametersSchema,
                    PushableItemTypes = app.ActualAppVersion.PushableItemTypes,
                    Revision = app.ActualAppVersion.Revision,
                    Status = app.ActualAppVersion.Status,
                    ThumbnailUrl = app.ActualAppVersion.ThumbnailUrl,
                    Title = app.ActualAppVersion.Title,
                    Updated = DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss"),
                    UpdatedBy = app.ActualAppVersion.UpdatedBy
                }
            };

            foreach (var i18N in app.ActualAppVersion.I18N)
            {
                appReq.ActualAppVersion.I18N.Add(JsonConvert.DeserializeObject<I18n>(i18N));
            }

            return appReq;
        }
    }
}
