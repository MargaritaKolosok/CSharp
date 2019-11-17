using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Api.Controllers;
using Api.Resources;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Microsoft.WindowsAzure.Storage.Blob;
using Models.Apps;
using Models.BackgroundTasks;
using Models.ExportTenant;
using Newtonsoft.Json;
using RestSharp;

namespace Api.Managers
{
    /// <summary>
    /// Requests to CX Manager API, endpoint group Tenants
    /// </summary>
    public static class TenantApi
    {
        /// <summary>
        /// Imports tenant ZIP archive to current tenant (see <see
        /// cref="ActionManager.CurrentTenant"/>). The ZIP file will be taken by path in
        /// <see cref="TestConfig.BrowserDownloadFolder"/>.
        /// </summary>
        /// <param name="fileName">Exported tenant archive file name without path (like
        /// tenantCode_[timestamp].zip)</param>
        public static void ImportTenant(string fileName)
        {
            var tenantFilePath = string.Empty;
            var token = BackgroundTaskApi.GetPackageSas();

            try
            {
                var blob = new CloudAppendBlob(new Uri(token));
                tenantFilePath = Path.Combine(TestConfig.BrowserDownloadFolder, fileName);
                blob.UploadFromFile(tenantFilePath);
                blob.SetProperties();
            }
            catch (Exception e)
            {
                throw new FileLoadException(
                    $"Tenant API: Error uploading file {tenantFilePath} to cloud: {e.Message}");
            }

            var task = BackgroundTaskApi.NewBackgroundTask(TaskActionType.ImportTenant, fileName,
                new FileInfo(tenantFilePath).Length,
                token);
            if (task == null || string.IsNullOrEmpty(task.TaskId))
            {
                throw new Exception(
                    $"Could not start new background task for tenant archive '{fileName}' import");
            }
            var body = new AppImport
            {
                PackageSas = token,
                TaskId = task.TaskId
            };

            BackgroundTaskApi.ProcessedObjects.AddOrUpdate(task.TaskId,
                (null, null, new ManualResetEvent(false)),
                (k, v) => (null, null, new ManualResetEvent(false)));
            Trace.TraceInformation($"Tenant API: Import task {task.TaskId} started");

            RestController.HttpRequestJson(UriCxm.BackgroundTaskExecution, Method.POST, body);

            if (BackgroundTaskApi.ProcessedObjects.TryGetValue(task.TaskId, out var tenantObj) &&
                tenantObj.Item3.WaitOne(TimeSpan.FromSeconds(TestConfig.TenantImportTimeout)))
            {
                Trace.TraceInformation($"Tenant API: Import task {task.TaskId} complete");
            }
            else
            {
                Trace.TraceError($"Tenant API: {task.TaskId} import timed out!");
                BackgroundTaskApi.DeleteTaskId(task.TaskId);
                throw new TimeoutException(
                    $"Tenant API: Import to tenant {ActionManager.CurrentTenantCode} timed out after " +
                    $"{TestConfig.TenantImportTimeout} s. File {fileName}.");
            }

            BackgroundTaskApi.ProcessedObjects.TryRemove(task.TaskId, out _);
            BackgroundTaskApi.DeleteTaskId(task.TaskId);
        }

        /// <summary>
        /// Exports all entities from current tenant (see <see cref="ActionManager.CurrentTenant"/>)
        /// </summary>
        /// <returns>(<see cref="string"/>) Exported ZIP file name that is saved in <see cref=
        /// "TestConfig.BrowserDownloadFolder"/> folder</returns>
        public static string ExportTenant()
        {
            var response = RestController.HttpRequestJson(UriCxm.TenantsExport, Method.GET);
            var objList = JsonConvert.DeserializeObject<EntitiesListInfo>(response.Content);
 
            var entitiesIdList = new EntitiesListId();
            entitiesIdList.Apps.AddRange(objList.Apps.Select(x => x.Id));
            entitiesIdList.Items.AddRange(objList.Items.Select(x => x.Id));
            entitiesIdList.Places.AddRange(objList.Places.Select(x => x.Id));
            //entitiesIdList.SourceId = "xxx";
            
            response = RestController.HttpRequestJson(UriCxm.TenantsExport, Method.POST, entitiesIdList);
            var task = JsonConvert.DeserializeObject<BackgroundTask>(response.Content);

            if (task == null || string.IsNullOrEmpty(task.TaskId))
            {
                throw new Exception(
                    $"Could not start new background task for tenant '{ActionManager.CurrentTenant}' export");
            }

            BackgroundTaskApi.ProcessedObjects.AddOrUpdate(task.TaskId,
                (null, null, new ManualResetEvent(false)),
                (k, v) => (null, null, new ManualResetEvent(false)));
            Trace.TraceInformation($"Tenant API: Export task {task.TaskId} started");

            string sas, fileName;
            if (BackgroundTaskApi.ProcessedObjects.TryGetValue(task.TaskId, out var tenantObj) &&
                tenantObj.Item3.WaitOne(TimeSpan.FromSeconds(TestConfig.TenantExportTimeout)))
            {
                var obj = tenantObj.Item1 as BackgroundTask;
                sas = obj?.FileSas;
                fileName = obj?.FileName;
                Trace.TraceInformation($"Tenant API: Export task {task.TaskId} complete");
            }
            else
            {
                Trace.TraceError($"Tenant API: {task.TaskId} export timed out!");
                BackgroundTaskApi.DeleteTaskId(task.TaskId);
                throw new TimeoutException(
                    $"Tenant API: Export from tenant {ActionManager.CurrentTenantCode} timed out after " +
                    $"{TestConfig.TenantExportTimeout} s.");
            }

            FileManager.Download(sas, fileName);
            BackgroundTaskApi.ProcessedObjects.TryRemove(task.TaskId, out _);
            BackgroundTaskApi.DeleteTaskId(task.TaskId);
            return fileName;
        }
    }
}
