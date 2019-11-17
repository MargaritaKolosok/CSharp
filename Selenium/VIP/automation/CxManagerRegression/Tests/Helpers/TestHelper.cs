using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using MailKit.Net.Imap;
using Models.Accounts.Models;
using Models.Apps;
using Models.Apps.MasterDataValues;
using Models.Interfaces;
using Models.Items.References.ReferenceInfo;
using Models.Places;
using Models.Places.Devices;
using Models.Places.Items;
using Models.Places.ParametersInstances;
using Models.Places.Schedules;
using Models.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using Tests.Resources;
using Item = Models.Items.Item;
using Place = Models.Places.Place;

namespace Tests.Helpers
{
    public class TestHelper : ActionManager
    {
        protected const string DeviceTypeIbeacon = "iBeacon";
        protected const string DeviceTypeWw = "Windows Workstation";
        protected const string DeviceTypeNoType = "Device";
        protected const string DeviceTypeIosDevice = "iOS Device";
        protected const string DeviceTypeNone = "None";
        protected const string DeviceTypeAndroidWorkstation = "Composer iOS/Android Workstation";

        protected const string ItemTypeAllTypes = "All types";
        protected const string ItemTypePorscheCar = "Porsche Car";
        protected const string ItemTypePoi = "Point of Interest";
        protected const string ItemTypeCars = "Cars";
        protected const string ItemTypeUsedCar = "Used car";
        protected const string ItemTypePdfCar = "PDF car";
        protected const string ItemTypeDailyItemsReport = "Daily items report";
        protected const string ItemTypeWelcomeEmailTemplate = "Welcome email template";
        protected const string ItemTypeActionButtonsTemplate = "Action buttons template";
        protected const string ItemTypeServiceBooking = "Service booking";
        protected const string ItemTypeCustomerProfile = "Customer profile";
        protected const string ItemTypeEmployee = "Employee";
        protected const string ItemTypeEventOrPromotion = "Event or Promotion";
        protected const string ItemTypeEmailTemplate = "Email template";
        protected const string ItemTypeTestDrive = "Test drive";
        protected const string ItemTypeSalesAppointment = "Sales appointment";

        protected const string AssetTypeAll = "All";
        protected const string AssetTypeImage = "Image";
        protected const string AssetTypeVideo = "Video";
        protected const string AssetTypeAudio = "Audio";
        protected const string AssetTypePdf = "Pdf";
        protected const string AssetTypeFont = "Font";
        protected const string AssetTypeCar = "Car";
        protected const string AssetTypeZip = "Zip";

        protected const string StatusActive = "Active";
        protected const string StatusPoweredOff = "Powered off";
        protected const string StatusPlaying = "Playing";
        protected const string StatusSleeping = "Sleeping";
        protected const string StatusRebooting = "Rebooting";
        protected const string StatusAvailable = "Available";
        protected const string StatusNoDevice = "No device";
        protected const string StatusDeleted = "Deleted";
        protected const string StatusUnknown = "Unknown";
        protected const string StatusError = "Error";
        protected const string StatusIdle = "Idle";
        protected const string StatusNew = "New";
        protected const string StatusSyncing = "Syncing";
        protected const string StatusPublished = "Published";
        protected const string StatusPendingApproval = "Pending approval";
        protected const string StatusRejected = "Rejected";

        protected const string TimezoneKiev = "Kiev";
        protected const string TimezoneLondon = "London";

        private readonly object _dptApp = new object();
        private readonly object _ibApp = new object();
        private readonly object _ib2App = new object();
        private readonly object _elPermApp = new object();
        private readonly object _evKitApp = new object();
        private readonly object _ipadApp = new object();
        protected readonly object PlayerApp = new object();
        private readonly object _legoApp = new object();
        private readonly object _vipBApp = new object();

        /// <summary>
        /// Login to CX Manager web application and choose first available tenant (if allowed)
        /// </summary>
        /// <param name="login">User credentials</param>
        /// <param name="isPressTenantButton">Press tenant button or not (optional, press by default)</param>
        protected void Login(User login, bool isPressTenantButton = true)
        {
            SendText(LoginPage.LoginEmail, login.Email);
            SendText(LoginPage.LoginPassword, login.Password);
            Click(LoginPage.LoginButton);
            if (!isPressTenantButton)
            {
                IsUserLoggedIn = false;
                return;
            }
            
            if (IsElementFoundQuickly(TenantsPage.TenantButton))
            {
                var timeout = TestConfig.ImplicitWaitTimeout;
                Click(string.Format(TenantsPage.TableRowByTenantCode, CurrentTenantCode), timeout: timeout);
            }

            IsUserLoggedIn = IsPageContainsUri(TestConfig.PlacesUri);
        }

        /// <summary>
        /// Login to CX Manager web application and choose specific tenant
        /// </summary>
        /// <param name="login">User credentials</param>
        /// <param name="tenant">Tenant title</param>
        protected void Login(User login, TenantTitle tenant)
        {
            if (!IsPageContainsUri(TestConfig.LoginUrl))
            {
                NavigateTo(TestConfig.LoginUrl);
            }
            SendText(LoginPage.LoginEmail, login.Email);
            if (!IsElementEquals(LoginPage.LoginEmail, login.Email))
            {
                SendText(LoginPage.LoginEmail, login.Email);
            }
            SendText(LoginPage.LoginPassword, login.Password);
            Click(LoginPage.LoginButton);
            CurrentTenant = tenant;
            NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlacesUri}");
            IsUserLoggedIn = IsElementFoundQuickly(PageHeader.UserMenuButton);
        }

        /// <summary>
        /// Login to CX Manager web application with specific credentials
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">User password</param>
        protected void Login(string email, string password)
        {
            Click(LoginPage.LoginEmail);
            SendText(LoginPage.LoginEmail, email);
            SendText(LoginPage.LoginPassword, password);
            Click(LoginPage.LoginButton);
            IsUserLoggedIn = IsElementNotFoundQuickly(LoginPage.ErrorMessagePlaceholder);
        }

        /// <summary>
        /// Login to CX Manager web application as current user (see
        /// <see cref="ActionManager.CurrentUser"/>) to current tenant (see <see
        /// cref="ActionManager.CurrentTenant"/>)
        /// </summary>
        protected void Login()
        {
            Login(CurrentUser);
        }

        /// <summary>
        /// Logout from CX Manager web application
        /// </summary>
        protected void Logout()
        {
            MouseOver(PageHeader.UserMenuButton);
            Click(PageHeader.UserMenuLogoutButton);
            CurrentUser.Sid = null;
            IsUserLoggedIn = IsPageRedirectedTo(TestConfig.LoginUrl);
        }

        /// <summary>
        /// Changes current tenant in CX Manager by changing current URL. Also changes 
        /// <see cref="ActionManager.CurrentTenant"/> and <see cref="ActionManager.CurrentTenantCode"/>.
        /// </summary>
        /// <param name="tenant">Tenant title to switch to</param>
        protected void ChangeTenant(TenantTitle tenant)
        {
            CurrentTenant = tenant;
            try
            {
                CloseAlert();
                NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlacesUri}");
                RefreshPage();
                IsUserLoggedIn = true;
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Changes current tenant in CX Manager UI (if browser instance is running). Also changes 
        /// <see cref="ActionManager.CurrentTenant"/> and <see cref="ActionManager.CurrentTenantCode"/>.
        /// </summary>
        /// <param name="tenant">Tenant title to switch to</param>
        protected void ChangeTenantUi(TenantTitle tenant)
        {
            try
            {
                CloseAlert();
                Click(PageHeader.BreadCrumbTenants);
                Click(string.Format(TenantsPage.TableRowByTitle, tenant));
                CurrentTenant = tenant;
            }
            catch
            {
                throw new Exception("Cannot change tenant");
            }
        }

        /// <summary>
        /// Determines whether specified tenant multi-language or not
        /// </summary>
        /// <param name="tenantCode">Tenant code</param>
        /// <returns>(<see cref="bool"/>) True if multi-language tenant</returns>
        private bool IsTenantMultiLanguage(string tenantCode)
        {
            return Tenants.SingleOrDefault(x => x.Code == tenantCode)?.Langs?.Count > 1;
        }

        /// <summary>
        /// Determines whether approval flow for apps and items enabled
        /// </summary>
        /// <returns>(<see cref="bool"/>) True if approval flow enabled</returns>
        private bool IsApprovalEnabled()
        {
            var approvalEnabled = 
                Tenants.SingleOrDefault(x => x.Code == CurrentTenantCode)?.ApprovalEnabled;
            return approvalEnabled != null && (bool) approvalEnabled;
        }

        /// <summary>
        /// Awaits a new mail for the specified mail client in specific time period
        /// </summary>
        /// <param name="client">Mail client</param>
        /// <param name="timeoutSeconds">Timeout in seconds (optional, 300 seconds by default)</param>
        /// <returns>(<see cref="bool"/>) True if got a new mail</returns>
        protected bool WaitForNewMail(ImapClient client, int timeoutSeconds = 300)
        {
            bool gotNewMail;
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                gotNewMail = MailManager.GotNewMail(client);
                if (!gotNewMail)
                {
                    // to avoid mail server DoS, use request delay
                    WaitTime(1);
                }
            }
            while (sw.Elapsed < TimeSpan.FromSeconds(timeoutSeconds) && !gotNewMail);

            return gotNewMail;
        }

        /// <summary>
        /// Postpones test run and waits until specific time of day comes. This method must be
        /// called some time before the target time. When called later, it does nothing.
        /// </summary>
        /// <param name="targetTimeSpan">Target time of day</param>
        protected void WaitForTimeOfDay(TimeSpan targetTimeSpan)
        {
            if (targetTimeSpan >= DateTime.Now.TimeOfDay)
            {
                return;
            }
            using (new Timer(x => { }, null, targetTimeSpan, TimeSpan.FromDays(1)))
            { }
        }

        /// <summary>
        /// Navigates to the specified page and logs in as specific user to CX Manager web application
        /// </summary>
        /// <param name="login">User credentials</param>
        /// <param name="url">Page URL</param>
        protected void NavigateAndLogin(string url, User login)
        {
            NavigateTo(url);
            Login(login);
        }
        
        /// <summary>
        /// Uploads DPT app version (if it does not exist) from file to current tenant
        /// </summary>
        /// <param name="status">App status to be set</param>
        /// <param name="version">App version</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppDpt(AppStatus status, string version)
        {
            lock (_dptApp)
            {
                var app = AppApi.GetApp(AppTitle.Dpt, version: version);
                if (app == null)
                {
                    ShowAlert("Uploading DPT app...");
                    app = AppApi.ImportApp(TestConfig.DptMobileAppFolder, TestConfig.DptMobileAppFile,
                        version);
                    CloseAlert();
                    if (app == null)
                    {
                        return null;
                    }
                }

                if (status == AppStatus.New)
                {
                    if (app.ActualAppVersion.Status != (int)AppStatus.New)
                    {
                        AppApi.DeleteAppVersion(app, version);
                        ShowAlert("Uploading DPT app...");
                        app = AppApi.ImportApp(TestConfig.DptMobileAppFolder, TestConfig.DptMobileAppFile,
                            version);
                        CloseAlert();
                    }
                    return app;
                }

                if (status == AppStatus.Deleted)
                {
                    AppApi.DeleteAppVersion(app, version, isDeleteCompletely: false);
                    app = AppApi.GetById(app.AppId);
                    return app;
                }

                if (app.ActualAppVersion.Status != (int)AppStatus.Available)
                {
                    var appReq = AppApi.AppResponseToRequest(app);
                    try
                    {
                        appReq.ActualAppVersion.MasterDataValues.en = TestData.DptMasterDataValuesEn;
                    }
                    catch
                    {
                        appReq.ActualAppVersion.MasterDataValues = new MasterDataValues
                        {
                            en = TestData.DptMasterDataValuesEn
                        };
                    }
                    if (IsTenantMultiLanguage(CurrentTenantCode))
                    {
                        appReq.ActualAppVersion.MasterDataValues.de = TestData.DptMasterDataValuesDe;
                    }

                    app = AppApi.SaveApp(appReq);
                    if (IsApprovalEnabled())
                    {
                        if (app.ActualAppVersion.Status == (int)AppStatus.New
                                    || app.ActualAppVersion.Status == (int)AppStatus.Rejected)
                        {
                            app = ApprovalApi.RequestApproval(app, CurrentTenant);
                        }

                        if (app.ActualAppVersion.Status == (int)AppStatus.PendingApproval)
                        {
                            app = ApprovalApi.Approve(app);
                        }
                    }
                }

                return app;  
            }
        }

        /// <summary>
        /// Uploads iBeacon app version (if it does not exist) from file
        /// </summary>
        /// <param name="version">App version</param>
        /// <param name="isOverwriteItems">Should items from the app be overwritten (optional)</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppIbeacon(string version, bool isOverwriteItems = false)
        {
            lock (_ibApp)
            {
                var app = AppApi.GetApp(AppTitle.Ibeacon, version);
                if (app == null)
                {
                    ShowAlert("Uploading iBeacon app...");
                    app = AppApi.ImportApp(
                        TestConfig.IbeaconAppFolder, TestConfig.IbeaconAppFile, version);
                    CloseAlert();
                }

                if (app != null &&
                    (app.ActualAppVersion.Status != (int)AppStatus.Available
                     || app.Versions.Any(x => x.Status == (int)AppStatus.New)))
                {
                    var appReq = AppApi.AppResponseToRequest(app);
                    appReq.ActualAppVersion.Version = version;
                    var v = version.Split('.');
                    if (appReq.Versions.Count > 1)
                    {
                        var ver = app.Versions.SingleOrDefault(x => x.Major == int.Parse(v[0])
                                                                    && x.Minor == int.Parse(v[1])
                                                                    && x.Revision == int.Parse(v[2]));
                        if (ver != null)
                        {
                            appReq.ActualAppVersion.Id = ver.Id;
                        }
                    }

                    appReq.ActualAppVersion.Major = int.Parse(v[0]);
                    appReq.ActualAppVersion.Minor = int.Parse(v[1]);
                    appReq.ActualAppVersion.Revision = int.Parse(v[2]);
                    appReq.ActualAppVersion.MasterData = TestData.IbeaconMasterData;

                    try
                    {
                        appReq.ActualAppVersion.MasterDataValues.en = TestData.IbeaconMasterDataValuesEn;
                    }
                    catch
                    {
                        appReq.ActualAppVersion.MasterDataValues = new MasterDataValues
                        {
                            en = TestData.IbeaconMasterDataValuesEn
                        };
                    }
                    if (IsTenantMultiLanguage(CurrentTenantCode))
                    {
                        appReq.ActualAppVersion.MasterDataValues.de = TestData.IbeaconMasterDataValuesDe;
                    }

                    appReq.Versions = null;

                    app = AppApi.SaveApp(appReq, isOverwriteItems);
                    if (IsApprovalEnabled())
                    {
                        if (app.ActualAppVersion.Status == (int)AppStatus.New
                            || app.ActualAppVersion.Status == (int)AppStatus.Rejected)
                        {
                            app = ApprovalApi.RequestApproval(app, CurrentTenant);
                        }

                        if (app.ActualAppVersion.Status == (int)AppStatus.PendingApproval)
                        {
                            app = ApprovalApi.Approve(app);
                        }
                    }
                }

                return app; 
            }  
        }

        /// <summary>
        /// Uploads specially created iBeacon app version (if it does not exist) from file. 
        /// Used only in tests RT09020 - RT09040.
        /// </summary>
        /// <param name="status">App resulting status</param>
        /// <param name="version">App version</param>
        /// <param name="isOverwriteItems">Defines whether embedded items be overwritten or not</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppIbeacon2(AppStatus status, string version, bool isOverwriteItems = false)
        {
            lock (_ib2App)
            {
                var app = AppApi.GetApp(AppTitle.IbeaconRt09020, version: version);
                if (app == null)
                {
                    ShowAlert("Uploading iBeacon2 app...");
                    app = AppApi.ImportApp(
                        TestConfig.IbeaconAppFolderRt09020, TestConfig.IbeaconAppFileRt09020, version);
                    CloseAlert();
                }

                if (status == AppStatus.New)
                {
                    if (app.ActualAppVersion.Status != (int)AppStatus.New)
                    {
                        AppApi.DeleteAppVersion(app, version);
                        ShowAlert("Uploading iBeacon2 app...");
                        app = AppApi.ImportApp(
                            TestConfig.IbeaconAppFolderRt09020, TestConfig.IbeaconAppFileRt09020, version);
                        CloseAlert();
                    }

                    return app;
                }

                if (status == AppStatus.Deleted)
                {
                    AppApi.DeleteAppVersion(app, version, isDeleteCompletely: false);
                    app = AppApi.GetById(app.AppId);
                    return app;
                }

                if (app.ActualAppVersion.Status != (int)AppStatus.Available)
                {
                    var appReq = AppApi.AppResponseToRequest(app);
                    appReq.ActualAppVersion.Version = version;
                    var v = version.Split('.');
                    if (appReq.Versions.Count > 1)
                    {
                        var ver = app.Versions.SingleOrDefault(x => x.Major == int.Parse(v[0])
                                                                    && x.Minor == int.Parse(v[1])
                                                                    && x.Revision == int.Parse(v[2]));
                        if (ver != null)
                            appReq.ActualAppVersion.Id = ver.Id;
                    }

                    appReq.ActualAppVersion.Major = int.Parse(v[0]);
                    appReq.ActualAppVersion.Minor = int.Parse(v[1]);
                    appReq.ActualAppVersion.Revision = int.Parse(v[2]);
                    appReq.ActualAppVersion.MasterData = TestData.IbeaconMasterData;

                    try
                    {
                        appReq.ActualAppVersion.MasterDataValues.en = TestData.IbeaconMasterDataValuesEn;
                        appReq.ActualAppVersion.MasterDataValues.de = TestData.IbeaconMasterDataValuesDe;
                    }
                    catch
                    {
                        appReq.ActualAppVersion.MasterDataValues = new MasterDataValues
                        {
                            en = TestData.IbeaconMasterDataValuesEn,
                            de = TestData.IbeaconMasterDataValuesEn
                        };
                    }

                    appReq.Versions = null;

                    app = AppApi.SaveApp(appReq, isOverwriteItems);
                    if (IsApprovalEnabled())
                    {
                        if (app.ActualAppVersion.Status == (int) AppStatus.New
                            || app.ActualAppVersion.Status == (int) AppStatus.Rejected)
                        {
                            app = ApprovalApi.RequestApproval(app, CurrentTenant);
                        }

                        if (app.ActualAppVersion.Status == (int) AppStatus.PendingApproval)
                        {
                            app = ApprovalApi.Approve(app);
                        }
                    } 
                }      

                if (status == AppStatus.Published)
                {
                    var place = PlaceApi.GetPlace(PlaceType.Ww, PlaceStatus.Any, hasDeviceAssigned: true);
                    AssignAppToPlace(place, app, null, null, isAddSilently: true);
                }

                return app;
            }
        }

        /// <summary>
        /// Uploads Player app version (if it does not exist) from file
        /// </summary>
        /// <param name="status">App status to be set (optional, sets AppStatus.Available by default)</param>
        /// <param name="version">App version number (optional, uploads earliest by default)</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppPlayer(
            AppStatus status = AppStatus.Available, 
            string version = "")
        {
            lock (PlayerApp)
            {
                version = string.IsNullOrEmpty(version) ? TestConfig.PlayerAppVersions[0] : version;
                var app = AppApi.GetApp(AppTitle.Player, version);
                if (app == null)
                {
                    ShowAlert("Uploading Player app...");
                    app = AppApi.ImportApp(TestConfig.PlayerAppFolder, TestConfig.PlayerAppFile, version);
                    CloseAlert();
                    if (app == null)
                    {
                        return null;
                    }
                }

                if (status == AppStatus.New)
                {
                    if (app.ActualAppVersion.Status != (int)AppStatus.New)
                    {
                        AppApi.DeleteAppVersion(app, version);
                        ShowAlert("Uploading Player app...");
                        app = AppApi.ImportApp(TestConfig.PlayerAppFolder, TestConfig.PlayerAppFile, version);
                        CloseAlert();
                    }
                    return app;
                }

                if (status == AppStatus.Deleted)
                {
                    AppApi.DeleteAppVersion(app, version, isDeleteCompletely: false);
                    app = AppApi.GetById(app.AppId);
                    return app;
                }

                if (app.ActualAppVersion.Status != (int)AppStatus.Available
                    && !IsApprovalEnabled())
                {
                    app = AppApi.SaveApp(AppApi.AppResponseToRequest(app));
                }
                if (app.ActualAppVersion.Status != (int)AppStatus.Available
                    && IsApprovalEnabled())
                {
                    if (app.ActualAppVersion.Status == (int)AppStatus.New
                        || app.ActualAppVersion.Status == (int)AppStatus.Rejected)
                    {
                        app = ApprovalApi.RequestApproval(app, CurrentTenant);
                    }

                    if (app.ActualAppVersion.Status == (int)AppStatus.PendingApproval)
                    {
                        app = ApprovalApi.Approve(app);
                    }
                }

                if (status == AppStatus.Published)
                {
                    var place = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0);
                    AssignAppToPlace(place, app, null, null, isAddSilently: true);
                }

                return app;  
            } 
        }

        /// <summary>
        /// Uploads Composer HQ app 1 version (if it does not exist) from file
        /// </summary>
        /// <param name="status">App status to be set</param>
        /// <param name="forcedUpload">Upload app version when it already exists (optional)</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppComposerHq1(AppStatus status = AppStatus.Available, bool forcedUpload = false)
        {
            var app = AppApi.GetApp(AppTitle.ComposerHq1);
            
            if (app == null || forcedUpload)
            {
                ShowAlert("Uploading Composer HQ app 1...");
                app = AppApi.ImportApp(TestConfig.ComposerHqApp1Folder, TestConfig.ComposerHqApp1File, null);
                CloseAlert();
                if (app == null)
                {
                    return null;
                }
            }

            if (status == AppStatus.New)
            {
                if (app.ActualAppVersion.Status != (int) AppStatus.New)
                {
                    AppApi.DeleteAppVersion(app, TestConfig.ComposerHqApp1Version);
                    ShowAlert("Uploading Composer HQ app 1...");
                    app = AppApi.ImportApp(TestConfig.ComposerHqApp1Folder, TestConfig.ComposerHqApp1File, null);
                    CloseAlert();
                }
                return app;
            }

            if (status == AppStatus.Deleted)
            {
                AppApi.DeleteAppVersion(app, TestConfig.ComposerHqApp1Version, isDeleteCompletely: false);
                app = AppApi.GetById(app.AppId);
                return app;
            }

            if (app.ActualAppVersion.Status != (int) AppStatus.Available
                && !IsApprovalEnabled())
            {
                app = AppApi.SaveApp(AppApi.AppResponseToRequest(app));
            }
            if (app.ActualAppVersion.Status != (int) AppStatus.Available
                && IsApprovalEnabled())
            {
                if (app.ActualAppVersion.Status == (int) AppStatus.New
                    || app.ActualAppVersion.Status == (int) AppStatus.Rejected)
                {
                    app = ApprovalApi.RequestApproval(app, CurrentTenant);
                }

                if (app.ActualAppVersion.Status == (int) AppStatus.PendingApproval)
                {
                    app = ApprovalApi.Approve(app);
                }
            }

            if (status == AppStatus.Published)
            {
                AssignAppToPlace(PlaceApi.GetPlace(PlaceType.Ww, PlaceStatus.Any, hasDeviceAssigned: true), app,
                    null, null, isAddSilently: true);
            }

            return app;
        }

        /// <summary>
        /// Uploads Composer HQ app 2 version (if it does not exist) from file
        /// </summary>
        /// <param name="status">App status to be set</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppComposerHq2(AppStatus status = AppStatus.Available)
        {
            var app = AppApi.GetApp(AppTitle.ComposerHq2);

            if (app == null)
            {
                ShowAlert("Uploading Composer HQ app 2...");
                app = AppApi.ImportApp(
                    TestConfig.ComposerHqApp2Folder, TestConfig.ComposerHqApp2File, null);
                CloseAlert();
                if (app == null)
                {
                    return null;
                }
            }

            if (status == AppStatus.New)
            {
                if (app.ActualAppVersion.Status != (int) AppStatus.New)
                {
                    AppApi.DeleteAppVersion(app, TestConfig.ComposerHqApp1Version);
                    ShowAlert("Uploading Composer HQ app 2...");
                    app = AppApi.ImportApp(TestConfig.ComposerHqApp2Folder, TestConfig.ComposerHqApp2File, null);
                    CloseAlert();
                }
                return app;
            }

            if (status == AppStatus.Deleted)
            {
                AppApi.DeleteAppVersion(app, TestConfig.ComposerHqApp2Version, isDeleteCompletely: false);
                app = AppApi.GetById(app.AppId);
                return app;
            }

            if (app.ActualAppVersion.Status != (int) AppStatus.Available
                && !IsApprovalEnabled())
            {
                app = AppApi.SaveApp(AppApi.AppResponseToRequest(app));
            }
            if (app.ActualAppVersion.Status != (int) AppStatus.Available
                && IsApprovalEnabled())
            {
                if (app.ActualAppVersion.Status == (int) AppStatus.New
                    || app.ActualAppVersion.Status == (int) AppStatus.Rejected)
                {
                    app = ApprovalApi.RequestApproval(app, CurrentTenant);
                }

                if (app.ActualAppVersion.Status == (int) AppStatus.PendingApproval)
                {
                    app = ApprovalApi.Approve(app);
                }
            }

            if (status == AppStatus.Published)
            {
                AssignAppToPlace(PlaceApi.GetPlace(PlaceType.Ww, PlaceStatus.Any, hasDeviceAssigned: true), app,
                    null, null, isAddSilently: true);
            }

            return app;
        }

        /// <summary>
        /// Uploads Composer VIPB app version (if it does not exist) from file
        /// </summary>
        /// <param name="version">App version number</param>
        /// <param name="status">App status (optional)s</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppComposerVipB(string version, AppStatus status = AppStatus.Available)
        {
            lock (_vipBApp)
            {
                var app = AppApi.GetApp(AppTitle.ComposerVipB, version);
                if (app == null)
                {
                    ShowAlert("Uploading Composer VIPB app...");
                    app = AppApi.ImportApp(TestConfig.ComposerVipbAppFolder, TestConfig.ComposerVipbAppFile, version);
                    CloseAlert();
                    if (app == null)
                    {
                        return null;
                    }
                }

                if (status == AppStatus.New)
                {
                    if (app.ActualAppVersion.Status != (int)AppStatus.New)
                    {
                        AppApi.DeleteAppVersion(app, version);
                        ShowAlert("Uploading VIPB app...");
                        app = AppApi.ImportApp(TestConfig.ComposerVipbAppFolder, TestConfig.ComposerVipbAppFile, version);
                        CloseAlert();
                    }
                    return app;
                }

                if (status == AppStatus.Deleted)
                {
                    AppApi.DeleteAppVersion(app, version, isDeleteCompletely: false);
                    app = AppApi.GetById(app.AppId);
                    return app;
                }

                if (app.ActualAppVersion.Status != (int)AppStatus.Available)
                {
                    var appReq = AppApi.AppResponseToRequest(app);
                    var ver = app.Versions.Single(x => string.Join(".", x.Major, x.Minor, x.Revision) == version);
                    appReq.ActualAppVersion.Id = ver.Id;
                    app = AppApi.SaveApp(appReq);
                    if (IsApprovalEnabled())
                    {
                        if (app.ActualAppVersion.Status == (int)AppStatus.New
                                    || app.ActualAppVersion.Status == (int)AppStatus.Rejected)
                        {
                            app = ApprovalApi.RequestApproval(app, CurrentTenant);
                        }

                        if (app.ActualAppVersion.Status == (int)AppStatus.PendingApproval)
                        {
                            app = ApprovalApi.Approve(app);
                        }
                    }
                }

                if (status == AppStatus.Published)
                {
                    AssignAppToPlace(PlaceApi.GetPlace(PlaceType.Ww, PlaceStatus.Any, hasDeviceAssigned: true), app,
                        null, null, isAddSilently: true);
                }

                return app; 
            }  
        }

        /// <summary>
        /// Uploads Event Kit app version (if does not exist on current tenant) from file
        /// </summary>
        /// <param name="version">App version number</param>
        /// <param name="status">App status (optional)s</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppEventKit(string version, AppStatus status = AppStatus.Available)
        {
            lock (_evKitApp)
            {
                var app = AppApi.GetApp(AppTitle.EventKit, version);
                if (app == null)
                {
                    ShowAlert("Uploading Event Kit app...");
                    app = AppApi.ImportApp(TestConfig.EventKitAppFolder, TestConfig.EventKitAppFile, version);
                    CloseAlert();
                    if (app == null)
                    {
                        return null;
                    }
                }

                if (status == AppStatus.New)
                {
                    if (app.ActualAppVersion.Status != (int)AppStatus.New)
                    {
                        AppApi.DeleteAppVersion(app, version);
                        ShowAlert("Uploading Event Kit app...");
                        app = AppApi.ImportApp(TestConfig.EventKitAppFolder, TestConfig.EventKitAppFile, version);
                        CloseAlert();
                    }
                    return app;
                }

                if (status == AppStatus.Deleted)
                {
                    AppApi.DeleteAppVersion(app, version, isDeleteCompletely: false);
                    app = AppApi.GetById(app.AppId);
                    return app;
                }

                if (app.ActualAppVersion.Status != (int)AppStatus.Available)
                {
                    var appReq = AppApi.AppResponseToRequest(app);
                    var ver = app.Versions.Single(x => string.Join(".", x.Major, x.Minor, x.Revision) == version);
                    appReq.ActualAppVersion.Id = ver.Id;
                    app = AppApi.SaveApp(appReq);
                    if (IsApprovalEnabled())
                    {
                        if (app.ActualAppVersion.Status == (int)AppStatus.New
                                    || app.ActualAppVersion.Status == (int)AppStatus.Rejected)
                        {
                            app = ApprovalApi.RequestApproval(app, CurrentTenant);
                        }

                        if (app.ActualAppVersion.Status == (int)AppStatus.PendingApproval)
                        {
                            app = ApprovalApi.Approve(app);
                        }
                    }
                }

                if (status == AppStatus.Published)
                {
                    var place = PlaceApi.GetPlace(PlaceType.Ww, PlaceStatus.Any, hasDeviceAssigned: true);
                    AssignAppToPlace(place, app, null, null, isAddSilently: true);
                }

                return app;   
            }
        }

        /// <summary>
        /// Uploads Lego Boost Unity app version (if does not exist on current tenant)
        /// from file
        /// </summary>
        /// <param name="version">App version number</param>
        /// <param name="status">App status (optional)s</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppLegoBoost(string version, AppStatus status = AppStatus.Available)
        {
            lock (_legoApp)
            {
                var app = AppApi.GetApp(AppTitle.LegoBoostUnity, version);
                if (app == null)
                {
                    ShowAlert("Uploading Lego Boost Unity app...");
                    app = AppApi.ImportApp(TestConfig.LegoBoostAppFolder,
                        TestConfig.LegoBoostAppFile, version);
                    CloseAlert();
                    if (app == null)
                    {
                        return null;
                    }
                }

                if (status == AppStatus.New)
                {
                    if (app.ActualAppVersion.Status != (int)AppStatus.New)
                    {
                        AppApi.DeleteAppVersion(app, version);
                        ShowAlert("Uploading Lego Boost Unity app...");
                        app = AppApi.ImportApp(TestConfig.LegoBoostAppFolder,
                            TestConfig.LegoBoostAppFile, version);
                        CloseAlert();
                    }
                    return app;
                }

                if (status == AppStatus.Deleted)
                {
                    AppApi.DeleteAppVersion(app, version, isDeleteCompletely: false);
                    app = AppApi.GetById(app.AppId);
                    return app;
                }

                if (app.ActualAppVersion.Status != (int)AppStatus.Available)
                {
                    var appReq = AppApi.AppResponseToRequest(app);
                    var ver = app.Versions.Single(x => string.Join(".", x.Major, x.Minor, x.Revision) == version);
                    appReq.ActualAppVersion.Id = ver.Id;
                    app = AppApi.SaveApp(appReq);
                    if (IsApprovalEnabled())
                    {
                        if (app.ActualAppVersion.Status == (int)AppStatus.New
                                    || app.ActualAppVersion.Status == (int)AppStatus.Rejected)
                        {
                            app = ApprovalApi.RequestApproval(app, CurrentTenant);
                        }

                        if (app.ActualAppVersion.Status == (int)AppStatus.PendingApproval)
                        {
                            app = ApprovalApi.Approve(app);
                        }
                    }
                }

                if (status == AppStatus.Published)
                {
                    var place = PlaceApi.GetPlace(PlaceType.Ww, PlaceStatus.Any, hasDeviceAssigned: true);
                    AssignAppToPlace(place, app, null, null, isAddSilently: true);
                }

                return app; 
            } 
        }

        /// <summary>
        /// Uploads iPad Player app version (if it does not exist) from file
        /// </summary>
        /// <param name="status">App status to be set (optional)</param>
        /// <param name="version">App version number (optional)</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppIpadPlayer(
            AppStatus status = AppStatus.Available,
            string version = null)
        {
            lock (_ipadApp)
            {
                version = string.IsNullOrEmpty(version) ? TestConfig.IpadPlayerAppVersions[0] : version;
                var app = AppApi.GetApp(AppTitle.IpadPlayer, version);

                if (app == null)
                {
                    ShowAlert("Uploading iPad Player app...");
                    app = AppApi.ImportApp(TestConfig.IpadPlayerAppFolder, TestConfig.IpadPlayerAppFile, version);
                    CloseAlert();
                    if (app == null)
                    {
                        return null;
                    }
                }

                if (status == AppStatus.New)
                {
                    if (app.ActualAppVersion.Status != (int)AppStatus.New)
                    {
                        AppApi.DeleteAppVersion(app, version);
                        ShowAlert("Uploading iPad Player app...");
                        app = AppApi.ImportApp(TestConfig.IpadPlayerAppFolder,
                            TestConfig.IpadPlayerAppFile, version);
                        CloseAlert();
                    }

                    return app;
                }

                if (status == AppStatus.Deleted)
                {
                    AppApi.DeleteAppVersion(app, version, isDeleteCompletely: false);
                    app = AppApi.GetById(app.AppId);
                    return app;
                }

                if (app.ActualAppVersion.Status != (int)AppStatus.Available &&
                    !IsApprovalEnabled())
                {
                    app = AppApi.SaveApp(AppApi.AppResponseToRequest(app));
                }
                if (app.ActualAppVersion.Status != (int)AppStatus.Available
                    && IsApprovalEnabled())
                {
                    if (app.ActualAppVersion.Status == (int)AppStatus.New
                        || app.ActualAppVersion.Status == (int)AppStatus.Rejected)
                    {
                        app = ApprovalApi.RequestApproval(app, CurrentTenant);
                    }

                    if (app.ActualAppVersion.Status == (int)AppStatus.PendingApproval)
                    {
                        app = ApprovalApi.Approve(app);
                    }
                }

                if (status == AppStatus.Published)
                {
                    var place = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0);
                    AssignAppToPlace(place, app, null, null, isAddSilently: true);
                }

                return app;  
            }
        }

        /// <summary>
        /// Uploads Test Element Permission app version (if it does not exist) from file
        /// </summary>
        /// <param name="status">App status to be set (optional)</param>
        /// <param name="version">App version number (optional)</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AddAppElementPermission(
            AppStatus status = AppStatus.Available,
            string version = null)
        {
            lock (_elPermApp)
            {
                version = string.IsNullOrEmpty(version) ? TestConfig.ElementPermissionDptVersions[0] : version;
                var app = AppApi.GetApp(AppTitle.TestElementPermissionDpt, version);

                if (app == null)
                {
                    ShowAlert("Uploading Element Permission DPT app...");
                    app = AppApi.ImportApp(TestConfig.TestElementPermissionDptFolder,
                        TestConfig.TestElementPermissionDptFile, version);
                    CloseAlert();
                    if (app == null)
                    {
                        return null;
                    }
                }

                if (status == AppStatus.New)
                {
                    if (app.ActualAppVersion.Status != (int)AppStatus.New)
                    {
                        AppApi.DeleteAppVersion(app, version);
                        ShowAlert("Uploading Element Permission DPT app...");
                        app = AppApi.ImportApp(TestConfig.TestElementPermissionDptFolder,
                            TestConfig.TestElementPermissionDptFile, version);
                        CloseAlert();
                    }

                    return app;
                }

                if (status == AppStatus.Deleted)
                {
                    AppApi.DeleteAppVersion(app, version, isDeleteCompletely: false);
                    app = AppApi.GetById(app.AppId);
                    return app;
                }

                if (app.ActualAppVersion.Status != (int)AppStatus.Available &&
                    !IsApprovalEnabled())
                {
                    app = AppApi.SaveApp(AppApi.AppResponseToRequest(app));
                }

                if (app.ActualAppVersion.Status != (int)AppStatus.Available
                    && IsApprovalEnabled())
                {
                    if (app.ActualAppVersion.Status == (int)AppStatus.New
                        || app.ActualAppVersion.Status == (int)AppStatus.Rejected)
                    {
                        app = ApprovalApi.RequestApproval(app, CurrentTenant);
                    }

                    if (app.ActualAppVersion.Status == (int)AppStatus.PendingApproval)
                    {
                        app = ApprovalApi.Approve(app);
                    }
                }

                if (status == AppStatus.Published)
                {
                    var place = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0);
                    AssignAppToPlace(place, app, null, null, isAddSilently: true);
                }

                return app; 
            }
        }

        /// <summary>
        /// Deletes specified version of specified app
        /// </summary>
        /// <param name="appType">App type</param>
        /// <param name="version">App version</param>
        /// <param name="appLockObject">Object to lock</param>
        protected void DeleteAppVersion(string appType, string version, object appLockObject)
        {
            lock (appLockObject)
            {
                var app = AppApi.GetApp(appType, version);
                if (app != null)
                {
                    AppApi.DeleteAppVersion(app, version);
                } 
            }
        }

        /// <summary>
        /// Sets property in json
        /// </summary>
        /// <param name="obj">Json object</param>
        /// <param name="path">Path to property in json</param>
        /// <param name="value">Value for json property</param>
        private static void SetPropertyByPath(JToken obj, string path, JToken value)
        {
            var p = obj.SelectToken(path);
            if (p != null)
            {
                p.Replace(value);
                return;
            }

            var segments = path.Split('.').Where(x => x != "$").ToArray();
            foreach (var segment in segments.Take(segments.Length - 1))
            {
                var current = obj[segment];
                if (current == null)
                {
                    obj[segment] = new JObject();
                }
                obj = obj[segment];
            }

            var jVal = value as JValue;
            obj[segments.Last()] = jVal ?? JToken.FromObject(value);
        }

        /// <summary>
        /// Adds specified item's reference to iBeacon app (MasterDataValues json section) by specified
        /// json schema path
        /// </summary>
        /// <param name="app">iBeacon app object</param>
        /// <param name="jsonSchemaPath">Json schema path</param>
        /// <param name="item">Item object</param>
        protected void AddItemToIbeaconApp(AppResponse app, string jsonSchemaPath, Item item)
        {
            var obj = app.ActualAppVersion.MasterDataValues;
            foreach (var lang in obj?.GetType().GetProperties())
            {
                string value;
                try
                {
                    value = lang.GetValue(obj).ToString();
                }
                catch
                {
                    continue;
                }

                var targetJson = JObject.Parse(value);
                var data = new JObject
                {
                    ["id"] = $"/#/{CurrentTenantCode}{TestConfig.ItemUri}/{item.Id}",
                    ["title"] = item.JsonDataTitle
                };
                SetPropertyByPath(targetJson, jsonSchemaPath, data);
                lang.SetValue(obj, JsonConvert.SerializeObject(targetJson));
            }

            var app1 = AppApi.AppResponseToRequest(app);
            AppApi.SaveApp(app1);
        }

        /// <summary>
        /// Starts edit mode for currently opened form (clicks Edit button on page footer)
        /// </summary>
        protected void EditForm()
        {
            if (IsElementFoundQuickly(CommonElement.ModalDialog1, 0.5)
                || IsElementFoundQuickly(CommonElement.ModalDialog2, 0.5))
            {
                throw new Exception("Modal dialog prevents Edit button click");
            }
            Click(PageFooter.EditButton);
            TurnOffInfoPopups();
            IsUserLoggedIn = true;
        }

        /// <summary>
        /// Submits currently opened form (clicks Submit button on page footer)
        /// </summary>
        protected void SubmitForm()
        {
            if (IsElementNotFound(CommonElement.ValidationError))
            {
                if (IsElementFoundQuickly(CommonElement.ModalDialog1, 0.5)
                    || IsElementFoundQuickly(CommonElement.ModalDialog2, 0.5))
                {
                    throw new Exception("Modal dialog prevents Edit button click");
                }
                try
                {
                    MouseOver(PageFooter.SubmitButton);
                    ClickAction();
                    WaitForPageReady(TestConfig.ImplicitWaitTimeout);
                    Thread.Sleep(500); // looks weird, but browser needs time to draw page elements
                }
                catch (Exception ex)
                {
                    throw new WebException($"{ex.Message} Cannot click Submit button");
                }

                IsUserLoggedIn = true;
                return;
            }
            Click(CommonElement.ValidationError);
            throw new ElementNotInteractableException(
                "Submit button is inactive due to validation error(s) displayed on page");
        }

        /// <summary>
        /// Determines whether edit mode is active for current web page
        /// </summary>
        /// <returns>(<see cref="bool"/>) True if edit mode active</returns>
        protected bool IsEditMode()
        {
            return IsElementFound(PageFooter.SubmitButton, TestConfig.ImplicitWaitTimeout * 1.5);
        }

        /// <summary>
        /// Determines whether current web page is in view mode
        /// </summary>
        /// <returns>(<see cref="bool"/>) True if view mode active</returns>
        protected bool IsViewMode()
        {
            return IsElementFound(PageFooter.EditButton, TestConfig.ImplicitWaitTimeout * 1.5);
        }

        /// <summary>
        /// Returns existing place or adds new one without Device Type in specified tenant and status
        /// </summary>
        /// <param name="status">Place status</param>
        /// <param name="isAddChild">Should have children or not (optional)</param>
        /// <param name="pageToBeOpened">Optional. 0 - do not open any page, 1 - open parent place page (default value),
        /// 2 - open last child place's page (if applicable).</param>
        /// <param name="isCreateNewPlace">Should a new place be created or used an existing one (optional)</param>
        /// <returns>(<see cref="Place"/>) Place object</returns>
        protected Place AddPlaceNoType(
            PlaceStatus status, 
            bool isAddChild = false,
            byte pageToBeOpened = 1,
            bool isCreateNewPlace = false)
        {
            Place place;
            if (!isCreateNewPlace)
            {
                place = PlaceApi.GetPlace(PlaceType.NoType, status, hasDeviceAssigned: false,
                            hasChildren: isAddChild, hasParent: false) ??
                        PlaceApi.CreateNewPlaceNoType(isAddChild);
            }
            else
            {
                place = PlaceApi.CreateNewPlaceNoType(isAddChild);
            }

            if (status == PlaceStatus.Deleted && place.Status != (int) PlaceStatus.Deleted)
            {
                place = PlaceApi.DeletePlace(place);
            }

            if (pageToBeOpened == 2)
            {
                var childId = place.ChildPlaces.Select(x => x.Id).LastOrDefault();
                if (childId != null)
                {
                    NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlaceUri}/{childId}");
                }
            }
            
            if (pageToBeOpened == 1)
            {
                OpenEntityPage(place);
            }

            return place;
        }

        /// <summary>
        /// Returns existing place or adds new one with device type "iBeacon" in specified tenant and status
        /// </summary>
        /// <param name="status">Place status</param>
        /// <param name="isAssignIbeacon">Should an iBeacon device be assigned or not (optional)</param>
        /// <param name="isAddChild">Should have children or not (optional)</param>
        /// <param name="pageToBeOpened">0 - do not open any page, 1 - open parent place page, 2 - open child place 
        /// page (optional)</param>
        /// <param name="isCreateNewPlace">Should a new place be created or used an existing one (optional)</param>
        /// <returns>(<see cref="Place"/>) Parent place object</returns>
        protected Place AddPlaceIbeacon(
            PlaceStatus status,
            bool isAssignIbeacon = false,
            bool isAddChild = false,
            byte pageToBeOpened = 1,
            bool isCreateNewPlace = false)
        {
            Place place;

            if (!isCreateNewPlace)
            {
                place = PlaceApi.GetPlace(PlaceType.Ibeacon, status, isAssignIbeacon,
                            hasChildren: isAddChild, hasParent: false) ??
                        PlaceApi.CreateNewPlaceIbeacon(status, isAssignIbeacon: isAssignIbeacon, isAddChild: isAddChild);
            }
            else
            {
                place = PlaceApi.CreateNewPlaceIbeacon(status, isAssignIbeacon: isAssignIbeacon, isAddChild: isAddChild);
            }
            
            if (pageToBeOpened == 2)
            {
                var childId = place.ChildPlaces.Select(x => x.Id).LastOrDefault();
                if (childId != null)
                    NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlaceUri}/{childId}");
            }
            
            if (pageToBeOpened == 1)
            {
                OpenEntityPage(place);
            }
            return place;
        }

        /// <summary>
        /// Returns existing place or adds new one with device type "Windows Workstation" in specified tenant and status
        /// </summary>
        /// <param name="status">Place status</param>
        /// <param name="isAssignDevice">Should an Windows Workstation device be assigned or not (optional)</param>
        /// <param name="isAddChild">Should have children or not (optional)</param>
        /// <param name="pageToBeOpened">0 - do not open any page, 1 - open parent place page, 2 - open child place 
        /// page (optional)</param>
        /// <param name="isCreateNewPlace">Should a new place be created or used an existing one (optional)</param>
        /// <returns>(<see cref="Place"/>) Place object</returns>
        protected Place AddPlaceWw(
            PlaceStatus status,
            bool isAssignDevice = false,
            bool isAddChild = false,
            byte pageToBeOpened = 1,
            bool isCreateNewPlace = false)
        {
            Place place;
            if (!isCreateNewPlace)
            {
                place = PlaceApi.GetPlace(PlaceType.Ww, status, isAssignDevice,
                            hasChildren: isAddChild, hasParent: false) ?? 
                        PlaceApi.CreateNewPlaceWw(isAssignDevice: isAssignDevice, isAddChild: isAddChild);
            }
            else
            {
                place = PlaceApi.CreateNewPlaceWw(isAssignDevice: isAssignDevice, isAddChild: isAddChild);
            }

            if (pageToBeOpened == 2)
            {
                var childId = place.ChildPlaces.Select(x => x.Id).LastOrDefault();
                if (childId != null)
                    NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlaceUri}/{childId}");
            }
            
            if (pageToBeOpened == 1)
            {
                OpenEntityPage(place);
            }

            return place;
        }

        /// <summary>
        /// Returns existing place or adds new one with device type "iOS Device" in specified tenant and status
        /// </summary>
        /// <param name="status">Place status</param>
        /// <param name="isAssignDevice">Should an iOS device be assigned or not (optional)</param>
        /// <param name="isAddChild">Should have children or not (optional)</param>
        /// <param name="pageToBeOpened">0 - do not open any page, 1 - open parent place page, 2 - open child place 
        /// page (optional)</param>
        /// <param name="isCreateNewPlace">Should a new place be created or used an existing one (optional)</param>
        /// <returns>(<see cref="Place"/>) Place object</returns>
        protected Place AddPlaceIos(
            PlaceStatus status,
            bool isAssignDevice = false,
            bool isAddChild = false,
            byte pageToBeOpened = 1,
            bool isCreateNewPlace = false)
        {
            Place place;
            if (!isCreateNewPlace)
            {
                place = PlaceApi.GetPlace(PlaceType.IosDevice, status, isAssignDevice,
                            hasChildren: isAddChild, hasParent: false) ??
                        PlaceApi.CreateNewPlaceIos(isAssignDevice: isAssignDevice, isAddChild: isAddChild);
            }
            else
            {
                place = PlaceApi.CreateNewPlaceIos(isAssignDevice: isAssignDevice, isAddChild: isAddChild);
            }

            if (pageToBeOpened == 2)
            {
                var childId = place.ChildPlaces.Select(x => x.Id).LastOrDefault();
                if (childId != null)
                    NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlaceUri}/{childId}");
            }

            if (pageToBeOpened == 1)
            {
                OpenEntityPage(place);
            }

            return place;
        }

        /// <summary>
        /// Returns existing item or adds new one with specified status, type, and tenant
        /// </summary>
        /// <param name="type">Item type</param>
        /// <param name="isAssignImage">Should image be assigned to the item or not (optional)</param>
        /// <param name="isAddNew">Create a new item or try to find an existing one</param>
        /// <returns>(<see cref="Item"/>) Item object</returns>
        protected Item AddItem(ItemType type, bool isAssignImage = false, bool isAddNew = false)
        {
            var path = TestConfig.ImportItemFolder;
            string mask, hash;
            var settings = new List<string>();

            var item = isAddNew ? null : ItemApi.GetItem(type, ItemStatus.Active, isAssignImage);
            
            if (item != null)
            {
                return item;
            }

            switch (type)
            {
                case ItemType.PorscheCar:
                    settings.AddRange(new []
                    { 
                        $"Auto item {RandomNumber}",
                        $"{RandomNumber}11"
                    });
                    if (isAssignImage)
                    {
                        mask = "PorscheCar.json";
                        hash = ItemApi.UploadFile(TestConfig.ImageCar);
                        settings.AddRange(new []
                        {
                            hash,
                            Path.GetFileName(TestConfig.ImageCar)
                        });
                    }
                    else
                    {
                        mask = "PorscheCar_NoImages.json";
                    }
                    break;
                case ItemType.UsedCar:
                    mask = "UsedCar.json";
                    hash = ItemApi.UploadFile(TestConfig.ImageCar);
                    settings.AddRange(new []
                    {
                        $"Auto item {RandomNumber}",
                        $"{RandomNumber}11",
                        hash,
                        Path.GetFileName(TestConfig.ImageCar)
                    });
                    break;
                case ItemType.CustomerProfile:
                    mask = "CustomerProfile.json";
                    settings.AddRange(new []
                    {
                        $"Auto item {RandomNumber}",
                        $"Auto item {RandomNumber}"
                    });
                    break;
                case ItemType.Employee:
                    mask = "Employee.json";
                    hash = ItemApi.UploadFile(TestConfig.Image08);
                    settings.AddRange(new []
                    {
                        $"Auto item {RandomNumber}",
                        $"Auto item {RandomNumber}",
                        hash,
                        Path.GetFileName(TestConfig.Image08)
                    });
                    break;
                case ItemType.Poi:
                    if (isAssignImage)
                    {
                        mask = "Poi.json";
                        hash = ItemApi.UploadFile(TestConfig.ImageCar);
                        settings.AddRange(new []
                        {
                            $"Auto item {RandomNumber}",
                            hash,
                            Path.GetFileName(TestConfig.ImageCar)
                        });
                    }
                    else
                    {
                        mask = "Poi_NoImages.json";
                        settings.AddRange(new []
                        {
                            $"Auto item {RandomNumber}"
                        });
                    }
                    break;
                case ItemType.EventOrPromotion:
                    mask = "EventOrPromotion.json";
                    hash = ItemApi.UploadFile(TestConfig.ImageCar);
                    settings.AddRange(new []
                    {
                        $"Auto item {RandomNumber}",
                        hash,
                        Path.GetFileName(TestConfig.ImageCar)
                    });
                    break;
                case ItemType.EmailTemplate:
                    mask = "EmailTemplate.json";
                    settings.AddRange(new []
                    {
                        $"Auto item {RandomNumber}",
                        RandomNumber
                    });
                    break;
                case ItemType.ServiceBooking:
                    mask = "ServiceBooking.json";
                    settings.AddRange(new []
                    {
                        $"Auto item {RandomNumber}",
                        $"Auto item {RandomNumber}"
                    });
                    break;
                case ItemType.PdfCar:
                    mask = "PdfCar.json";
                    hash = ItemApi.UploadFile(TestConfig.Pdf1);
                    settings.AddRange(new []
                    {
                        $"Auto item {RandomNumber}",
                        $"{RandomNumber}11",
                        hash,
                        Path.GetFileName(TestConfig.Pdf1)
                    });
                    break;
                case ItemType.SalesAppointment:
                    mask = "SalesAppointment.json";
                    settings.AddRange(new []
                    {
                        $"Auto item {RandomNumber}",
                        $"Auto item {RandomNumber}"
                    });
                    break;
                case ItemType.TestDrive:
                    mask = "TestDrive.json";
                    settings.AddRange(new []
                    {
                        $"Auto item {RandomNumber}",
                        $"Auto item {RandomNumber}"
                    });
                    break;
                default:
                    mask = "Car.json";
                    settings.AddRange(new[]
                    {
                        $"Auto item {RandomNumber}",
                        $"{RandomNumber}11"
                    });
                    break;
            }

            item = ItemApi.ImportItem(path, mask, settings);
            return item;
        }

        /// <summary>
        /// Assigns specified app to the specified place
        /// </summary>
        /// <param name="place">Place object</param>
        /// <param name="app">App object</param>
        /// <param name="item">Item object</param>
        /// <param name="itemType">Item type string</param>
        /// <param name="isAddSilently">Should the place web page be opened or not (optional)</param>
        /// <param name="isAddItemToExistingApp">Should item be assigned to an existing <paramref name="app"/>
        /// or not (optional)</param>
        /// <returns>(<see cref="Place"/>) Place object</returns>
        protected Place AssignAppToPlace(
            Place place,
            AppResponse app,
            Item item,
            string itemType,
            bool isAddSilently = false,
            bool isAddItemToExistingApp = false)
        {
            if (app != null)
            {
                ScheduleApp scheduleApp;
                if (!isAddItemToExistingApp)
                {
                    scheduleApp = ScheduleApi.GetScheduleEntry(app.ActualAppVersion.Id);
                    var appVersion = AppApi.GetAppVersion(app.ActualAppVersion.Id);
                    scheduleApp.Id = -1001 - (place.Schedule?.ScheduleApps?.Count ?? 0);
                    scheduleApp.AppVersions.Last().ParametersSchema = appVersion.ParametersSchema;
                    scheduleApp.AppVersions.Last().Title = appVersion.Title;

                    if (place.Schedule == null)
                    {
                        place.Schedule = new Schedule
                        {
                            PlaceId = place.Id,
                            ScheduleApps = new List<ScheduleApp>(),
                            ScheduleId = place.Id
                        };
                    }

                    place.Schedule.ScheduleApps?.Add(scheduleApp); 
                }
                else
                {
                    scheduleApp = place.Schedule.ScheduleApps.Single(x => x.AppId == app.AppId);
                }

                if (item == null)
                {
                    if (scheduleApp.ParametersInstance == null)
                    {
                        scheduleApp.ParametersInstanceJson = new ParametersInstance();
                    }    
                }
                else
                {
                    if (scheduleApp.ParametersInstanceJson == null)
                        scheduleApp.ParametersInstanceJson = new ParametersInstance();

                    switch (item.SchemaModel.Id)
                    {
                        case 2:
                            scheduleApp.ParametersInstanceJson.item = new JsonItem
                            {
                                id = $"/#/{CurrentTenantCode}{TestConfig.ItemUri}/{item.Id}",
                                title = $"{item.LangJsonData.EnJson.Title} ({itemType})"
                            };
                            break;
                        case 7:
                            scheduleApp.ParametersInstanceJson.poi = new JsonItem
                            {
                                id = $"/#/{CurrentTenantCode}{TestConfig.ItemUri}/{item.Id}",
                                title = $"{item.LangJsonData.EnJson.Title} ({itemType})"
                            };
                            break;
                    }
                    
                    scheduleApp.ParametersInstance =
                        JsonConvert.SerializeObject(scheduleApp.ParametersInstanceJson);
                }

                place = PlaceApi.SavePlace(place);
            }
            
            if (!isAddSilently)
            {
                OpenEntityPage(place);
            }
            
            return place;
        }

        /// <summary>
        /// Opens specified place, item, or app page
        /// </summary>
        /// <typeparam name="T"><see cref="IEntity"/> derived type (optional)</typeparam>
        /// <param name="entity">Entity object</param>
        protected void OpenEntityPage<T>(T entity) where T : class, IEntity
        {
            if (entity == null)
            {
                throw new Exception("Cannot open page of an entity whose is null or type unknown ");
            }

            CloseAlert();
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);

            switch (entity)
            {
                case Item item:
                    NavigateTo(
                        $@"{TestConfig.BaseUrl}#{CurrentTenantCode}{TestConfig.ItemUri}/{item.Id}");
                    break;

                case Place place:
                    NavigateTo(
                        $@"{TestConfig.BaseUrl}#{CurrentTenantCode}{TestConfig.PlaceUri}/{place.Id}");
                    break;

                case AppResponse appResp:
                    NavigateTo(
                        $@"{TestConfig.BaseUrl}#{CurrentTenantCode}{TestConfig.AppUri}/{appResp.AppId}");
                    break;

                case AppRequest appReq:
                    NavigateTo(
                        $@"{TestConfig.BaseUrl}#{CurrentTenantCode}{TestConfig.AppUri}/{appReq.AppId}");
                    break;

                default:
                    throw new Exception("Cannot open page of an entity with unsupported type");
            }

            IsUserLoggedIn = true;
        }

        /// <summary>
        /// Opens place, item, or app page by type and ID
        /// </summary>
        /// <typeparam name="T"><see cref="IEntity"/> derived type</typeparam>
        /// <param name="objId">Entity ID (use positive numeric values)</param>
        protected void OpenEntityPage<T>(long? objId) where T : class, IEntity
        {
            ulong id;
            try
            {
                id = Convert.ToUInt64(objId);
                if (id == 0)
                {
                    throw new Exception("Cannot open page of an entity with ID = 0");
                }
            }
            catch
            {
                throw new Exception($@"Cannot open page of an entity with wrong ID = '{objId}'");
            }

            CloseAlert();
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);

            if (typeof(T) == typeof(Item))
            {
                NavigateTo(
                    $@"{TestConfig.BaseUrl}#{CurrentTenantCode}{TestConfig.ItemUri}/{id}");
                return;
            }

            if (typeof(T) == typeof(Place))
            {
                NavigateTo(
                    $@"{TestConfig.BaseUrl}#{CurrentTenantCode}{TestConfig.PlaceUri}/{id}");
                return;
            }

            if (typeof(T) != typeof(AppResponse) && typeof(T) != typeof(AppRequest))
            {
                throw new Exception("Cannot open page of an entity with unsupported type");
            }

            NavigateTo(
                $@"{TestConfig.BaseUrl}#{CurrentTenantCode}{TestConfig.AppUri}/{id}");
            IsUserLoggedIn = true;
        }

        /// <summary>
        /// Assigns specified place to the specified item
        /// </summary>
        /// <param name="item">Item object</param>
        /// <param name="place">Place object</param>
        /// <param name="isAddSilently">Should item page be opened or not (optional)</param>
        /// <returns>(<see cref="ValueTuple{Item,Place}"/>) Object that contains updated item
        /// and place objects</returns>
        protected (Item, Place) AssignPlaceToItem(
            Item item, 
            Place place,
            bool isAddSilently = false)
        {
            if (item == null || place == null)
            {
                throw new Exception("Item or/and place is null");
            }

            item.References.Places.Insert(0,
                new ReferenceInfo
                {
                    DirectAssignments = true,
                    Id = place.Id,
                    Title = place.Title,
                    Url = $@"/{CurrentTenantCode}{TestConfig.PlaceUri}/{place.Id}",
                    Status = place.Status
                }
            );
            
            item = ItemApi.SaveItem(item);
            place = PlaceApi.GetById(place.Id);
            
            if (!isAddSilently)
            {
                OpenEntityPage(item);
            }

            return (item, place);
        }

        /// <summary>
        /// Assigns collection of child places to specified parent place. If some child place is null, a new one
        /// will be created and then assigned to parent place.
        /// </summary>
        /// <param name="parentPlace">Parent place object</param>
        /// <param name="isAddSilently">Should parent place page be opened or not (optional)</param>
        /// <param name="childPlaces">Child place objects array (optional)</param>
        /// <returns>(<see cref="Place"/>) Parent place object</returns>
        protected Place AssignChildrenToParentPlace(
            Place parentPlace, bool isAddSilently = false, 
            params Place[] childPlaces
            )
        {
            for (var i = 0; i < childPlaces.Length; i++)
            {
                if (parentPlace.Id == childPlaces[i].Id)
                {
                    switch (parentPlace.DeviceTypeId)
                    {
                        case null:
                            childPlaces[i] = PlaceApi.CreateNewPlaceNoType(isAddChild: false);
                            break;
                        case (int?)PlaceType.Ibeacon:
                            childPlaces[i] = PlaceApi.CreateNewPlaceIbeacon(PlaceStatus.NoDevice, 
                                isAssignIbeacon: false, isAddChild: false);
                            break;
                        case (int?)PlaceType.Ww:
                            childPlaces[i] = PlaceApi.CreateNewPlaceWw(isAssignDevice: false, isAddChild: false);
                            break;
                        default:
                            throw new ArgumentException("Unknown device type of parent place. Cannot create a new child.");
                    }
                }

                childPlaces[i].ParentId = parentPlace.Id;
                childPlaces[i].Parents.Add(new ParentPlace
                {
                    Id = parentPlace.Id,
                    Title = parentPlace.Title
                });
                childPlaces[i] = PlaceApi.SavePlace(childPlaces[i]);

                parentPlace.ChildPlaces.Add(new ChildPlace
                {
                    Id = childPlaces[i].Id,
                    Device = childPlaces[i].Device,
                    DeviceTypeName = string.Empty,
                    MapX = childPlaces[i].MapX,
                    MapY = childPlaces[i].MapY,
                    Radius = childPlaces[i].Radius,
                    Status = childPlaces[i].Status,
                    ThumbnailUrl = childPlaces[i].ThumbnailUrl,
                    Title = childPlaces[i].Title
                });
            }

            parentPlace = PlaceApi.SavePlace(parentPlace);

            if (!isAddSilently)
            {
                OpenEntityPage(parentPlace);
            }

            return parentPlace;
        }

        /// <summary>
        /// Assigns image to the specified app
        /// </summary>
        /// <param name="app">App object</param>
        /// <returns>(<see cref="AppResponse"/>) App object</returns>
        protected AppResponse AssignImageToApp(AppResponse app)
        {
            var appPicture = AppApi.UploadImage(TestConfig.ImageJpeg);
            var appReq = AppApi.AppResponseToRequest(app);

            appReq.FullImageUrl = appPicture.FullImageUrl;
            appReq.ImageName = appPicture.ImageName;
            appReq.ThumbnailUrl = appPicture.ThumbnailUrl;
            appReq.ShowImageUrl = appPicture.ShowImageUrl;

            app = AppApi.SaveApp(appReq);
            
            return app;
        }

        /// <summary>
        /// Assigns image to the specified place
        /// </summary>
        /// <param name="place">Place object</param>
        /// <returns>(<see cref="Place"/>) Place object</returns>
        protected Place AssignImageToPlace(Place place)
        {
            var placeImage = PlaceApi.UploadImage(TestConfig.Image138);

            place.FullImageUrl = placeImage.FullImageUrl;
            place.ImageName = placeImage.ImageName;
            place.ThumbnailUrl = placeImage.ThumbnailUrl;

            place = PlaceApi.SavePlace(place);

            return place;
        }

        /// <summary>
        /// Assigns device to specified place. If <paramref name="device"/> is null, a new
        /// device will be created. 
        /// </summary>
        /// <param name="place">Place object</param>
        /// <param name="device">Device object</param>
        /// <returns>(<see cref="Place"/>) Place object</returns>
        protected Place AssignDeviceToPlace(Place place, Device device = null)
        {
            if (device == null)
            {
                switch (place.DeviceTypeId)
                {
                    case (int)PlaceType.Ibeacon:
                        device = PlaceApi.GetIbeacon();
                        break;
                    case (int)PlaceType.Ww:
                        device = PlaceApi.GetWw();
                        break;
                } 
            }

            if (device != null && device.Id == null)
            {
                switch (place.DeviceTypeId)
                {
                    case (int)PlaceType.Ibeacon:
                        device = PlaceApi.GetIbeacon(device.Data.Major.ToString(), device.Data.Minor.ToString());
                        break;
                    case (int)PlaceType.Ww:
                        device = PlaceApi.GetWw(device.Name);
                        break;
                }
            }
            
            place.Device = device;
            if (place.Device != null)
            {
                place.Device.AttachmentPlaceTitle = place.Title;
            }
            place = PlaceApi.SavePlace(place);

            return place;
        }

        /// <summary>
        /// Find entity in list by inputting text in "Filter" field on page header
        /// </summary>
        /// <param name="expression">Text expression</param>
        protected void SetFilter(string expression)
        {
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);
            ClearTextInElement(PageHeader.Filter);
            if (!string.IsNullOrEmpty(expression))
            {
                PressKeys(expression.Contains(' ') ? $"\"{expression}\"" : expression);
            }
        }

        /// <summary>
        /// Find entity in list by inputting text in modal dialog "Filter" field
        /// </summary>
        /// <param name="expression">Text expression</param>
        protected void SetFilterModal(string expression)
        {
            Click(CommonElement.FilterModal);
            ClearTextInFocusedElement();
            if (!string.IsNullOrEmpty(expression))
            {
                PressKeys(expression);
            }
        }

        /// <summary>
        /// Opens Devices dialog from page header menu
        /// </summary>
        protected void OpenDevicesFromMenu()
        {
            if (IsElementFoundQuickly(CommonElement.Backdrop, 0.5))
            {
                ClickAtPoint(CommonElement.Backdrop, 10, 10);
            }
            if (IsElementFoundQuickly(Devices.DevicesDialog, 0.5))
            {
                return;
            }

            MouseOver(PageHeader.UserMenuButton);
            Click(PageHeader.UserMenuDevicesButton);
        }

        /// <summary>
        /// Opens Devices dialog by click on device details button on place form
        /// </summary>
        protected void OpenDevicesFromPlace()
        {
            if (IsElementFoundQuickly(Devices.CancelButtonDevice, 0.5))
            {
                Click(Devices.CancelButtonDevice);
            }
            if (IsElementFoundQuickly(Devices.CancelButton, 0.5))
            {
                Click(Devices.CancelButton);
            }

            Click(PlacesPage.IbeaconDetailsButton);
        }

        /// <summary>
        /// Opens Devices dialog from page header menu and then opens specified
        /// device's properties
        /// </summary>
        /// <param name="devTitle">Title of device</param>
        protected void OpenDevice(string devTitle)
        {
            OpenDevicesFromMenu();
            if (IsElementNotFoundQuickly(Devices.HideDeletedButton))
            {
                Click(Devices.ShowDeletedButton);
            }
            Click(string.Format(Devices.TableRowByText, devTitle));
        }

        /// <summary>
        /// Opens Devices dialog by click on device details button on place form
        /// and presses Clear Selection button to clear Device field on place form
        /// </summary>
        protected void ClearDevice()
        {
            OpenDevicesFromPlace();
            Click(Devices.ClearSelectionButton);
        }

        /// <summary>
        /// Opens Devices dialog and creates a new device of iBeacon device type
        /// </summary>
        /// <param name="uuid">UUID parameter</param>
        /// <param name="major">Major parameter</param>
        /// <param name="minor">Minor parameter</param>
        protected void CreateDeviceIbeacon(string uuid = null, string major= null, string minor = null)
        {
            OpenDevicesFromMenu();
            Click(Devices.AddButton);
            if (string.IsNullOrEmpty(uuid))
            {
                return;
            }
            SendText(Devices.Uuid, uuid);
            if (string.IsNullOrEmpty(major))
            {
                return;
            }
            SendText(Devices.Major, major);
            if (string.IsNullOrEmpty(minor))
            {
                return;
            }
            SendText(Devices.Minor, minor);
            Click(Devices.SubmitButtonDevice);
        }

        /// <summary>
        /// Replaces in string two and more white spaces with 1 white space. Also trims leading and trailing 
        /// white spaces. Removes \r, \n, \b, \f, and \t.
        /// </summary>
        /// <param name="str">String to be cleaned up</param>
        /// <returns>(<see cref="string"/>) Cleaned up string</returns>
        protected string CleanUpString(string str)
        {
            return string.Join(" ", 
                str.Trim().Split(
                    new [] { " ", "\r", "\n", "\t", "\b", "\f" }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Postpones test execution for a time specified in seconds. If wait time is greater than 5
        /// seconds, browser alert will be shown automatically.
        /// </summary>
        /// <param name="seconds">Time in seconds. Fractional numbers allowed.</param>
        protected void WaitTime(double seconds)
        {
            if (seconds > 5)
            {
                ShowAlert($"Waiting for {seconds} seconds ...");
            }

            Task.Delay(TimeSpan.FromMilliseconds(seconds * 1000)).Wait();

            if (seconds > 5)
            {
                CloseAlert();
            }
        }

        /// <summary>
        /// Get followed by current user item models as a collection from account info
        /// </summary>
        /// <returns>(<see cref="IEnumerable{Model}"/>) List of followed models</returns>
        protected IEnumerable<Model> GetFollowedItemTypes()
        {
            var accountInfo = AccountApi.GetAccountData();
            return accountInfo.Models.Where(x => x.Followed);
        }

        /// <summary>
        /// Unfollows specified item type(s)
        /// </summary>
        /// <param name="types">Item types array</param>
        protected void UnfollowItemTypes(params ItemType[] types)
        {
            var followedTypes = GetFollowedItemTypes()?
                .Select(x => x.Id)
                .ToArray();
            if (followedTypes == null || followedTypes.Length == 0)
            {
                return;
            }
            followedTypes = followedTypes
                .Intersect(types
                    .Select(x => (int) x)
                    .ToArray())
                .ToArray();
            foreach (var type in followedTypes)
            {
                ItemApi.UnfollowItems((ItemType)type);
            }
        }

        /// <summary>
        /// Awaits presence of file in SFTP directory in specific time period
        /// </summary>
        /// <param name="ftpM"></param>
        /// <param name="path">SFTP directory path</param>
        /// <param name="fileName">File name with extension</param>
        /// <param name="timeoutSeconds">Timeout in seconds (optional)</param>
        /// <returns>(<see cref="bool"/>) True file appears during timeout</returns>
        protected bool WaitForCarImportComplete(FtpManager ftpM, string path, string fileName, double timeoutSeconds = 60)
        {
            bool isExists;
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                isExists = ftpM.IsFileExists(path, fileName);
                if (!isExists)
                {
                    // to avoid SFTP server DoS, use request delay
                    WaitTime(1);
                }
            }
            while (sw.Elapsed < TimeSpan.FromSeconds(timeoutSeconds) && !isExists);
            return isExists;
        }

        /// <summary>
        /// Selects date in specified date picker field
        /// </summary>
        /// <param name="datePickerSelector">Date picker field element selector</param>
        /// <param name="date">Date to be selected</param>
        protected void SelectDate(string datePickerSelector, DateTime date)
        {
            Click(datePickerSelector);
            SendText(datePickerSelector, date.ToString("dd.MM.yyyy"));
        }

        /// <summary>
        /// Get timestamp and user name from field by web element selector
        /// </summary>
        /// <param name="selector">Timestamp element selector</param>
        /// <param name="isWaitForValue">Wait a bit before element value pick</param>
        /// <returns>(<see cref="ValueTuple{DateTime, String}"/>) DateTime value with string timestamp</returns>
        protected (DateTime, string) GetTimestamp(string selector, bool isWaitForValue = false)
        {
            var stamp = CleanUpString(GetValue(selector, isWaitForValue));
            if (string.IsNullOrEmpty(stamp))
            {
                throw new Exception($@"Timestamp by selector '{selector}' looks empty");
            }
            var startPos = stamp.IndexOf(' ');
            var temp = stamp.IndexOf('(') - startPos;
            var length = temp > 0 ? temp : stamp.Length - startPos;
            return (Convert.ToDateTime(stamp.Substring(startPos, length)), stamp);
        }
        
        /// <summary>
        /// Gets two timestamps by their element selectors and compares first element to
        /// second one
        /// </summary>
        /// <param name="firstStampSelector">First timestamp element selector</param>
        /// <param name="secondStampSelector">Second timestamp element selector</param>
        /// <returns>(<see cref="Equality"/>) Comparision result</returns>
        protected Equality CompareTimestamps(string firstStampSelector, string secondStampSelector)
        {
            var stamp1 = GetTimestamp(firstStampSelector, true).Item1;
            var stamp2 = GetTimestamp(secondStampSelector).Item1;

            if (Math.Abs((stamp1 - stamp2).Seconds) <= 3)
            {
                return Equality.Equal;
            }

            return stamp1 > stamp2 ? Equality.Greater : Equality.Less;
        }

        /// <summary>
        /// Determines whether timestamp contains user name
        /// </summary>
        /// <param name="timestampSelector">Timestamp element selector</param>
        /// <param name="userName">User name</param>
        /// <returns>(<see cref="bool"/>) True if contains</returns>
        protected bool IsUserInTimestamp(string timestampSelector, string userName)
        {
            var stamp = GetTimestamp(timestampSelector).Item2;
            return stamp.Contains(userName);
        }

        /// <summary>
        /// Is somebody currently logged in as <see cref="ActionManager.CurrentUser"/>
        /// </summary>
        /// <returns>(<see cref="bool"/>) True if logged in as<see cref="ActionManager.CurrentUser"/>
        /// </returns>
        protected bool IsLoggedInAsCurrentUser()
        {
            if (!IsElementFoundQuickly(PageHeader.UserMenuButton))
            {
                return false;
            }
            var mailbox = CurrentUser.Email.Substring(0, CurrentUser.Email.IndexOf('@'));
            return GetValue(PageHeader.UserMenuButton).Contains(mailbox);
        }

        /// <summary>
        /// Returns counter value from current page header or null if no value
        /// </summary>
        /// <returns>(<see cref="Nullable{Int32}"/>) Counter value</returns>
        protected int? GetCounter()
        {
            var text = GetValue(PageHeader.PageName);
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            var left = text.IndexOf('(');
            var right = text.IndexOf(')');
            if (left < 0 || right < 0)
            {
                return null;
            }
            return Convert.ToInt32(text.Substring(left + 1, right - left - 1));
        }

        /// <summary>
        /// Returns counter value from current modal dialog header or null if no value
        /// </summary>
        /// <returns>(<see cref="Nullable{Int32}"/>) Counter value</returns>
        protected int? GetCounterModal()
        {
            var text = GetValue(PlacesPage.DialogHeader);
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            var left = text.IndexOf('(');
            var right = text.IndexOf(')');
            if (left < 0 || right < 0)
            {
                return null;
            }
            return Convert.ToInt32(text.Substring(left + 1, right - left - 1));
        }

        /// <summary>
        /// Opens Apps list web page
        /// </summary>
        protected void OpenAppsPage()
        {
            CloseAlert();
            CloseModalDialogs();
            if (GetCurrentUrl().Contains(TestConfig.AppsUri))
            {
                RefreshPage();
                return;
            }

            NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.AppsUri}");
            IsUserLoggedIn = true;
        }

        /// <summary>
        /// Opens Items list web page
        /// </summary>
        protected void OpenItemsPage()
        {
            CloseAlert();
            CloseModalDialogs();
            if (IsElementFoundQuickly(PageHeader.PageCarsButton)
                || GetCurrentUrl().Contains(TestConfig.ItemsUri))
            {
                RefreshPage();
            } 

            NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemsUri}");
            IsUserLoggedIn = true;
        }

        /// <summary>
        /// Opens Places list web page
        /// </summary>
        protected void OpenPlacesPage()
        {
            CloseAlert();
            CloseModalDialogs();
            if (GetCurrentUrl().Contains(TestConfig.PlacesUri))
            {
                RefreshPage();
                return;
            }

            NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlacesUri}");
            IsUserLoggedIn = true;
        }

        /// <summary>
        /// Opens Cars list web page
        /// </summary>
        protected void OpenCarsPage()
        {
            CloseAlert();
            CloseModalDialogs();
            Click(PageHeader.PageCarsButton);
            IsUserLoggedIn = true;
        }

        protected void OpenMediaPage()
        {
            CloseAlert();
            CloseModalDialogs();
            Click(PageHeader.PageMediaButton);
            IsUserLoggedIn = true;
        }
    }
}
