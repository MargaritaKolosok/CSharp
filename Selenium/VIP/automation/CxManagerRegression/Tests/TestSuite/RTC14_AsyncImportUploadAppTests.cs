using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC14_AsyncImportUploadAppTests : ParentTest
    {
        private string _hariboAppFileName, _compHq1Path, _compHq2Path, _playerPath, _playerFile;
        private readonly ConnectionManager _cm = new ConnectionManager();

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false; 
            _hariboAppFileName = Path.GetFileName(TestConfig.HariboApp);
            _compHq1Path = Path.Combine(TestConfig.ComposerHqApp1Folder, TestConfig.ComposerHqApp1File);
            _compHq2Path = Path.Combine(TestConfig.ComposerHqApp2Folder, TestConfig.ComposerHqApp2File);
            _playerPath = FileManager.GetFileByVersion(TestConfig.PlayerAppFolder, TestConfig.PlayerAppFile,
                TestConfig.PlayerAppVersions[1]);
            _playerFile = Path.GetFileName(_playerPath); 
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }

            CurrentTenant = TenantTitle.upload1;
            CurrentUser = TestConfig.AdminUser;
            await BackgroundTaskApi.DeleteAllTasksAsync(TestConfig.AdminUser).ConfigureAwait(false);
            AppApi.DeleteApps(true,
                new[]
                {
                    AppTitle.Haribo, 
                    AppTitle.ComposerHq1, 
                    AppTitle.ComposerHq2, 
                    AppTitle.SapPorsche, 
                    AppTitle.Player
                }, 
                TenantTitle.upload1);
            AppApi.DeleteApps(true,
                new[] { AppTitle.Haribo, AppTitle.ComposerHq1, AppTitle.ComposerHq2 }, TenantTitle.upload2);
            // page ready without "upload spinner" element check
            IsUseAllPageReadyChecks = false;
        }

        private void ReLogin()
        {
            OpenPrimaryBrowser();
            Navigate(TestConfig.LoginUrl);
            ClearBrowserCache();
            Login(CurrentUser, isPressTenantButton: false);
        }

        [Test, Regression]
        public void RT14010_UploadAppWebElements()
        {
            TestStart();

            Assert.IsTrue(IsElementNotFound(CommonElement.UploadWarning) 
                          && IsElementNotFound(CommonElement.UploadSpinner),
                "No upload spinner and upload warning icons should be present on page header");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.PlayerAppFolder, TestConfig.PlayerAppFile,
                TestConfig.PlayerAppVersions[0]);
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsElementFound(CommonElement.UploadProgress),
                "Upload progress gauge on page header should be shown on app import activation");

            Assert.IsTrue(IsElementFound(AppsPage.AppTitle, 90), 
                $"App Player v.{TestConfig.PlayerAppVersions[0]} is not imported");
            Assert.IsTrue(IsElementNotFound(CommonElement.UploadWarning)
                    && IsElementNotFound(CommonElement.UploadSpinner)
                    && IsElementNotFound(CommonElement.UploadProgress),
                "No upload progress gauge, spinner, and upload warning should be present on page header");

            SubmitForm();
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Click(CommonElement.UploadSpinner);
            Assert.IsTrue(IsElementFound(PageHeader.TasksDialog),
                "Tasks dialog should be shown on upload spinner click");
            Assert.IsTrue(IsElementFound(PageHeader.CancelAllTasksButton),
                "Tasks dialog should contain Cancel All Tasks button");
            Assert.IsTrue(IsElementFound(PageHeader.OkButton),
                "Tasks dialog should contain OK button");
            
            Assert.IsTrue(IsElementFound(string.Format(PageHeader.TasksTableRowByText, _hariboAppFileName)),
                "Tasks dialog should contain current task row");
            Assert.IsTrue(IsElementFound(
                    string.Format(PageHeader.TasksTableRowByText, "Uploading"), 1)
                || IsElementFound(
                    string.Format(PageHeader.TasksTableRowByText, "Import"), 1),
                @"Tasks dialog should contain current task row with status 'Uploading' or 'Importing'");
            Assert.IsTrue(
                IsElementFound(
                    string.Format(PageHeader.TasksTableRowByText, CurrentTenantCode)),
                $@"Tasks dialog should contain current task row with tenant code '{CurrentTenantCode}'");

            Click(PageHeader.OkButton);
            Assert.IsTrue(IsElementNotFound(PageHeader.TasksDialog),
                "Tasks dialog should be closed on OK button click");
            Assert.IsTrue(IsElementFound(CommonElement.UploadProgress),
                "Upload progress gauge on page header should be shown on Tasks dialog closure");
            Assert.IsTrue(IsElementFound(CommonElement.UploadSpinner),
                "Upload spinner on page header should be shown on Tasks dialog closure");

            Assert.IsTrue(IsElementFound(AppsPage.AppTitle, 90), "App Haribo is not imported");

            Click(PageFooter.CancelButton);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Click(CommonElement.UploadSpinner);
            if (IsElementFound(string.Format(PageHeader.TasksTableRowByText, "Upload"), 1))
            {
                Click(string.Format(PageHeader.TasksTableRowByTextDeleteButton, "Upload"));
                Assert.IsTrue(IsElementNotFound(PageHeader.TasksDialog),
                    "Tasks dialog should be closed on Delete task button click (if no tasks left)");
                Assert.IsTrue(IsElementNotFound(CommonElement.UploadProgress),
                    "Upload progress gauge on page header should be hidden on the last task delete");
                Assert.IsTrue(IsElementNotFound(CommonElement.UploadSpinner),
                    "Upload spinner on page header should be hidden on the last task delete");

                WaitTime(30);
                RefreshPage();
                Assert.IsTrue(IsElementNotFound(string.Format(AppsPage.TableRowByText, AppTitle.Haribo)),
                    $@"App '{AppTitle.Haribo}' should not be shown as its import task was deleted");
            }
            else
            {
                OpenAppsPage();
            }

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_compHq1Path);
            Click(CommonElement.UploadSpinner);
            Assert.IsTrue(IsElementFound(PageHeader.TasksDialog),
                "Tasks dialog should be shown on upload spinner click");
            Assert.IsTrue(IsElementNotFound(PageHeader.TasksDialog, TestConfig.AppImportTimeout),
                "Tasks dialog should be closed automatically on the last task successfully completed");
            Assert.IsTrue(IsElementNotFound(CommonElement.UploadProgress),
                "Upload progress gauge on page header should be hidden when no active tasks");
            Assert.IsTrue(IsElementNotFound(CommonElement.UploadSpinner),
                "Upload spinner on page header should be hidden when no active tasks");
            Assert.IsTrue(IsEditMode(), "Composer HQ v1 app should be in edit mode");

            Click(PageFooter.CancelButton);
        }

        [Test, Regression]
        public void RT14020_UploadAppWarning()
        {
            AddAppPlayer();
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_compHq2Path);
            Click(CommonElement.UploadSpinner);
            Assert.IsTrue(IsElementFound(PageHeader.TasksDialog),
                "Tasks dialog should be shown on upload spinner click");
            Assert.IsTrue(IsElementFound(
                    string.Format(PageHeader.TasksTableRowByText, _hariboAppFileName)),
                $@"App '{AppTitle.Haribo}' should be shown in Tasks dialog");
            Assert.IsTrue(
                IsElementFound(
                    string.Format(PageHeader.TasksTableRowByText, TestConfig.ComposerHqApp2File)),
                $@"App '{AppTitle.ComposerHq2}' should be shown in Tasks dialog");

            var result = 0;
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                if (IsElementNotFound(string.Format(PageHeader.TasksTableRowByText, _hariboAppFileName), 1))
                {
                    result = 1;
                }
                if (IsElementNotFound(
                    string.Format(PageHeader.TasksTableRowByText, TestConfig.ComposerHqApp2File), 1))
                {
                    result = 2;
                }
            } while (sw.Elapsed < TimeSpan.FromSeconds(TestConfig.AppImportTimeout) && result == 0);
            sw.Stop();
            Assert.NotZero(result, 
                $@"Import of apps '{AppTitle.Haribo}' and '{AppTitle.ComposerHq2}' is timed out");
            var appTitle = result == 2 ? AppTitle.Haribo : AppTitle.ComposerHq2;
            IsElementFound(AppsPage.AppTitle, TestConfig.AppImportTimeout);
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitle, appTitle) && IsEditMode(),
                $@"The latest imported app '{appTitle}' should be shown in edit mode");

            Assert.IsTrue(IsElementNotFound(PageHeader.UploadSpinner)
                    && IsElementNotFound(PageHeader.UploadWarning),
                "There should be no spinner or warning on page header");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $@"After submit app should have Status '{StatusAvailable}'");

            var appTitle2 = result == 2 ? AppTitle.ComposerHq2 : AppTitle.Haribo;
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(string.Format(AppsPage.TableRowByTwoParams, appTitle2, StatusNew)),
                $@"First imported app '{appTitle2}' should be shown in status '{StatusNew}'");

            Click(string.Format(AppsPage.TableRowByText, appTitle2));
            EditForm();
            SubmitForm();
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $@"After submit app should have Status '{StatusAvailable}'");

            Click(PageFooter.AddVersionButton);
            FileManager.Upload(result == 1 ? _compHq2Path : TestConfig.HariboApp);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorIsNotAVersionDialog),
                @"Exception dialog 'uploaded app is not a version' is displayed");
            Assert.IsTrue(IsElementFound(PageHeader.UploadWarning),
                "There should be upload warning on page header");
            Click(AppsPage.OkButton);

            Click(PageHeader.UploadWarning);
            Assert.IsTrue(IsElementFound(PageHeader.TasksDialog),
                "Tasks dialog should be shown on upload warning click");
            var fileName = result == 1 ? TestConfig.ComposerHqApp2File : _hariboAppFileName;
            Assert.True(IsElementFoundQuickly(string.Format(PageHeader.TasksTableRowByThreeParams,
                    fileName, "Import has failed", CurrentTenant)),
                $@"There should be app with App/Package '{fileName}', Status 'Import has failed', " +
                $"and tenant '{CurrentTenant}' displayed");

            Click(PageHeader.OkButton);
            if (result == 1)
            {
                OpenAppsPage();
                Click(string.Format(AppsPage.TableRowByText, appTitle));
            }
            Click(PageFooter.AddVersionButton);
            FileManager.Upload(_compHq2Path);
            Assert.IsTrue(IsElementFound(PageHeader.UploadSpinner)
                          && IsElementFound(PageHeader.UploadWarning),
                "There should be both spinner and warning displayed on page header");

            Click(PageHeader.UploadSpinner);
            Assert.IsTrue(IsElementFound(PageHeader.TasksDialog),
                "Tasks dialog should be shown on upload spinner click");
            Assert.True(IsElementFound(string.Format(PageHeader.TasksTableRowByThreeParams,
                    fileName, "Import has failed", CurrentTenant)),
                $@"There should be app with App/Package '{fileName}', Status 'Import has failed', " +
                $"and tenant '{CurrentTenant}' displayed");
            fileName = result == 1 ? _hariboAppFileName : TestConfig.ComposerHqApp2File;
            Assert.True(IsElementFound(string.Format(PageHeader.TasksTableRowByThreeParams,
                    fileName, "Uploading", CurrentTenant)),
                $@"There should be app with App/Package '{fileName}', Status 'Uploading', " +
                $"and tenant '{CurrentTenant}' displayed");

            Click(PageHeader.OkButton);
            Assert.IsTrue(IsEditMode(), "Imported app should be in edit mode");

            Assert.IsTrue(IsElementNotFound(PageHeader.UploadSpinner),
                "There should be no spinner on page header after import");
            Assert.IsTrue(IsElementFound(PageHeader.UploadWarning),
                "There should be upload warning on page header after import");

            Click(PageHeader.UploadWarning);
            fileName = result == 1 ? TestConfig.ComposerHqApp2File : _hariboAppFileName;
            Assert.True(IsElementFoundQuickly(string.Format(PageHeader.TasksTableRowByThreeParams,
                    fileName, "Import has failed", CurrentTenant)),
                $@"There should be app with App/Package '{fileName}', Status 'Import has failed', " +
                $"and tenant '{CurrentTenant}' displayed");
            fileName = result == 1 ? _hariboAppFileName : TestConfig.ComposerHqApp2File;
            Assert.True(IsElementNotFoundQuickly(string.Format(PageHeader.TasksTableRowByText,
                    fileName)),
                $@"There should be no app with App/Package '{fileName}' displayed");

            Click(string.Format(PageHeader.TasksTableRowByTextDeleteButton, "Import has failed"));
            Assert.IsTrue(IsElementNotFound(PageHeader.TasksDialog),
                "Tasks dialog should be closed on the last task removal");

            Assert.IsTrue(IsElementNotFound(PageHeader.UploadSpinner),
                "There should be no spinner after the last task removal");
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadWarning),
                "There should be no upload warning after the last task removal");

            RefreshPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadSpinner),
                "There should be no spinner after page refresh");
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadWarning),
                "There should be no upload warning after page refresh");
        }

        [Test, Regression]
        public void RT14030_UploadInterruptions()
        {
            AddAppPlayer();
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            IsElementFound(CommonElement.UploadProgress);
            Assert.IsFalse(IsFormSaved(), 
                "Javascript alert dialog should be shown on page refresh attempt");

            AcceptAlert();
            Assert.IsTrue(IsElementFound(PageHeader.UploadWarning),
                "There should be upload warning after page refresh");
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadSpinner),
                "There should be no spinner after page refresh");

            Click(PageHeader.UploadWarning);
            Assert.True(IsElementFoundQuickly(string.Format(PageHeader.TasksTableRowByThreeParams,
                    _hariboAppFileName, "Upload has failed", CurrentTenant)),
                $@"There should be app with App/Package '{_hariboAppFileName}', Status 'Upload has failed', " +
                $"and tenant '{CurrentTenant}' displayed");
            var percentage =
                Convert.ToInt16(GetValue(
                        string.Format(PageHeader.TasksTableRowByTextPercentage, _hariboAppFileName))
                .TrimEnd('%'));
            Click(string.Format(PageHeader.TasksTableRowByTextReuploadButton, _hariboAppFileName));
            FileManager.Upload(TestConfig.HariboApp);
            Assert.True(IsElementFound(string.Format(PageHeader.TasksTableRowByThreeParams,
                    _hariboAppFileName, "Uploading...", CurrentTenant)),
                $@"There should be app with App/Package '{_hariboAppFileName}', Status 'Uploading...', " +
                $"and tenant '{CurrentTenant}' displayed");
            var percentageNow =
                Convert.ToInt16(GetValue(
                        string.Format(PageHeader.TasksTableRowByTextPercentage, _hariboAppFileName), true)
                .TrimEnd('%'));
            Assert.IsTrue(percentage <= percentageNow, "Import percentage should not start from beginning");

            // closing tab was replaced with refresh page
            Assert.IsFalse(IsFormSaved(),
                "Javascript alert dialog should be shown on current page refresh");

            AcceptAlert();
            
            Click(PageHeader.UploadWarning);
            percentage = Convert.ToInt16(GetValue(
                string.Format(PageHeader.TasksTableRowByTextPercentage, _hariboAppFileName))
                .TrimEnd('%'));
            Assert.True(IsElementFoundQuickly(string.Format(PageHeader.TasksTableRowByThreeParams,
                    _hariboAppFileName, "Upload has failed", CurrentTenant)),
                $@"There should be app with App/Package '{_hariboAppFileName}', Status 'Upload has failed', " +
                $"and tenant '{CurrentTenant}' displayed");

            Click(PageHeader.OkButton);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Assert.IsTrue(IsElementFound(PageHeader.UploadSpinner),
                "There should be shown spinner on repeated app upload");
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadWarning),
                "There should be no upload warning on repeated app upload");

            Click(PageHeader.UploadSpinner);
            Assert.IsTrue(
                CountElements(string.Format(PageHeader.TasksTableRowByText, "")) == 1,
                "Only 1 task should be present in Tasks dialog");
            Assert.True(IsElementFound(string.Format(PageHeader.TasksTableRowByThreeParams,
                    _hariboAppFileName, "Uploading", CurrentTenant)),
                $@"There should be app with App/Package '{_hariboAppFileName}', Status 'Uploading', " +
                $"and tenant '{CurrentTenant}' displayed");
            percentageNow = Convert.ToInt16(GetValue(
                string.Format(PageHeader.TasksTableRowByTextPercentage, _hariboAppFileName))
                .TrimEnd('%'));
            Assert.IsTrue(percentage <= percentageNow, "Import percentage should not start from beginning");

            Assert.IsTrue(
                IsElementNotFound(string.Format(PageHeader.TasksTableRowByTextReuploadButton, "")),
                "Re-upload button should not be shown for task");

            Click(PageHeader.OkButton);
            IsElementFound(AppsPage.AppTitle, 90);
            Assert.IsTrue(IsEditMode(),
                "Application should be in edit mode after upload continue");

            Click(PageFooter.CancelButton);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Click(PageHeader.UploadSpinner);
            Assert.True(IsElementFound(string.Format(PageHeader.TasksTableRowByThreeParams,
                    _hariboAppFileName, "Importing", CurrentTenant)),
                $@"There should be app with App/Package '{_hariboAppFileName}', Status 'Importing', " +
                $"and tenant '{CurrentTenant}' displayed");

            Assert.IsTrue(IsFormSaved(),
                "Javascript alert dialog should be not shown on current page refresh");

            WaitTime(60);
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadSpinner),
                "After browser close and re-login there should be no upload spinner");
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadWarning),
                "After browser close and re-login there should be no upload warning");

            OpenAppsPage();
            Assert.IsTrue(
                IsElementFound(string.Format(AppsPage.TableRowByTwoParams, AppTitle.Haribo, StatusNew)),
                $@"App '{AppTitle.Haribo}' should be in status {StatusNew} on Apps list page");
        }

        [Test, Regression]
        public void RT14040_UploadStateOnTenants()
        {
            CurrentTenant = TenantTitle.upload2;
            AddAppPlayer();
            CurrentTenant = TenantTitle.upload1;
            AddAppPlayer();
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_compHq1Path);
            Click(PageHeader.BreadCrumbTenants);
            Assert.IsTrue(IsPageContainsUri(TestConfig.TenantsUrl),
                $"User should stay on Tenants page after app {AppTitle.ComposerHq1} import complete");
            Click(PageHeader.UploadSpinner);
            Assert.IsTrue(IsElementNotFound(PageHeader.TasksDialog),
                $"Tasks dialog should be closed on app {AppTitle.ComposerHq1} import complete");

            Click(string.Format(TenantsPage.TableRowByTitle, TenantTitle.upload1));
            OpenAppsPage();
            Assert.IsTrue(
                IsElementFound(string.Format(AppsPage.TableRowByTwoParams, AppTitle.ComposerHq1, StatusNew)),
                $"App {AppTitle.ComposerHq1} should be shown on App list page in {StatusNew} status");
            
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_compHq2Path);
            ChangeTenantUi(TenantTitle.upload2);
            Click(PageHeader.PageAppsButton);
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Click(PageHeader.UploadSpinner);
            Assert.True(IsElementFound(
                    string.Format(PageHeader.TasksTableRowByTwoParams, TestConfig.ComposerHqApp2File, TenantTitle.upload1)),
                $@"There should be app with App/Package '{TestConfig.ComposerHqApp2File}' and tenant " +
                $@"'{TenantTitle.upload1}' displayed");
            Assert.True(IsElementFound(
                    string.Format(PageHeader.TasksTableRowByTwoParams, _hariboAppFileName, CurrentTenant)),
                $@"There should be app with App/Package '{_hariboAppFileName}' and tenant " + 
                $@"'{CurrentTenant}' displayed");

            Assert.IsTrue(IsElementNotFound(PageHeader.TasksDialog, 120),
                $"Tasks dialog should be closed on app {AppTitle.ComposerHq2} import complete");
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitle, AppTitle.Haribo) && IsEditMode(),
                $"App {AppTitle.Haribo} page should be opened in edit mode after two apps import");

            Click(PageFooter.CancelButton);
            ChangeTenant(TenantTitle.upload1);
            Click(PageHeader.PageAppsButton);
            Assert.IsTrue(
                IsElementFound(string.Format(AppsPage.TableRowByTwoParams, AppTitle.ComposerHq2, StatusNew)),
                $"App {AppTitle.ComposerHq2} should be shown on App list page in {StatusNew} status");

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            IsElementFound(CommonElement.UploadProgress);
            ChangeTenantUi(TenantTitle.upload2);
            Click(PageHeader.PageAppsButton);
            Assert.IsFalse(IsFormSaved(),
                "Javascript alert dialog should be shown on current page refresh");
            AcceptAlert();
            Click(PageHeader.UploadWarning);
            Click(string.Format(PageHeader.TasksTableRowByTextReuploadButton, _hariboAppFileName));
            FileManager.Upload(_compHq2Path);
            Assert.IsTrue(IsElementFound(PageHeader.ErrorYouHaveSelectedWrongPackageDialog),
                @"Error 'You have selected wrong package...' should be shown on wrong package selection " +
                "to re-upload");

            Click(PageHeader.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.ErrorYouHaveSelectedWrongPackageDialog),
                @"Error dialog 'You have selected wrong package...' should be on OK button press");
            Assert.IsTrue(IsElementFound(PageHeader.TasksDialog),
                "Tasks dialog should stay opened error dialog close");

            Click(string.Format(PageHeader.TasksTableRowByTextReuploadButton, _hariboAppFileName));
            FileManager.Upload(TestConfig.HariboApp);
            Assert.True(IsElementFound(string.Format(PageHeader.TasksTableRowByThreeParams,
                    _hariboAppFileName, "Importing", TenantTitle.upload1)),
                $@"There should be app with App/Package '{_hariboAppFileName}', Status 'Importing', " +
                $"and tenant '{TenantTitle.upload1}' displayed");

            Assert.IsTrue(IsElementNotFound(PageHeader.TasksDialog),
                $"Tasks dialog should be closed on app {AppTitle.Haribo} re-upload and import complete " +
                "on different tenant");
            Assert.IsTrue(IsElementNotFound(AppsPage.AppTitle),
                $"App {AppTitle.Haribo} page should not open after re-upload and import complete " +
                "on different tenant");

            ChangeTenant(TenantTitle.upload1);
            Click(PageHeader.PageAppsButton);
            Assert.IsTrue(
                IsElementFound(string.Format(AppsPage.TableRowByTwoParams, AppTitle.Haribo, StatusNew)),
                $"App {AppTitle.Haribo} should be shown on App list page in {StatusNew} status");
        }

        [Test, Regression]
        public void RT14050_UploadNetworkConditions()
        {
            AddAppPlayer();
            DeleteAppVersion(AppTitle.Player, TestConfig.PlayerAppVersions[1], PlayerApp);
            var place = AddPlaceNoType(PlaceStatus.Any, pageToBeOpened: 0);
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Click(PageHeader.PagePlacesButton);
            Click(string.Format(PlacesPage.TableRowByTitle, place.Title));
            IsUseAllPageReadyChecks = true;
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitle, AppTitle.Haribo) && IsEditMode(),
                $"App {AppTitle.Haribo} page should be opened in edit mode after import");
            IsUseAllPageReadyChecks = false;

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppsUri),
                "User should be redirected to Apps list page on Cancel press on just imported app form");

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Click(PageHeader.PagePlacesButton);
            Click(string.Format(PlacesPage.TableRowByTitle, place.Title));
            EditForm();
            var newDescription = RandomNumber;
            SendText(PlacesPage.Description, newDescription);
            Assert.IsTrue(IsElementFound(PlacesPage.ErrorAppNotSavedDialog, 120),
                $@"Dialog '{AppTitle.Haribo} has been successfully uploaded. If you leave to save it, " + 
                "current changes will be discarded...' should be displayed after app import");

            Click(PlacesPage.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorAppNotSavedDialog),
                $@"Dialog '{AppTitle.Haribo} has been successfully uploaded. If you leave to save it, " +
                "current changes will be discarded...' should be closed on Cancel press");
            Assert.IsTrue(IsElementEquals(PlacesPage.Title, place.Title) && IsEditMode(),
                $@"Place '{place.Title}' page should be opened in edit mode after Cancel press");
            Assert.IsTrue(IsElementEquals(PlacesPage.Description, newDescription),
                $@"Place Description should be unchanged '{newDescription}' after Cancel press");

            Click(PageFooter.CancelButton);
            OpenAppsPage();
            Assert.IsTrue(
                IsElementFound(string.Format(AppsPage.TableRowByTwoParams, AppTitle.Haribo, StatusNew)),
                $"App {AppTitle.Haribo} should be shown on App list page in {StatusNew} status");

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_playerPath);
            Click(PageHeader.PagePlacesButton);
            Click(string.Format(PlacesPage.TableRowByTitle, place.Title));
            EditForm();
            newDescription = RandomNumber;
            SendText(PlacesPage.Description, newDescription);
            Assert.IsTrue(IsElementFound(PlacesPage.ErrorAppNotSavedDialog, 120),
                $@"Dialog '{AppTitle.Player} has been successfully uploaded. If you leave to save it, " +
                "current changes will be discarded...' should be displayed after app import");
            Click(PlacesPage.OkButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppUri) 
                          && IsElementEquals(AppsPage.AppTitle, AppTitle.Player) && IsEditMode(),
                $"App {AppTitle.Player} page should be shown and be in edit mode on OK button press");

            Click(PageFooter.CancelButton);
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_playerPath);
            WaitTime(5);
            _cm.DisconnectNetwork();
            Assert.IsTrue(IsElementFound(AppsPage.ErrorInternetConnectionLostDialog),
                @"Error 'Upload has been interrupted due to the lost internet connection...' dialog should " 
                + " be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorInternetConnectionLostDialog),
                @"Error 'Upload has been interrupted due to the lost internet connection...' dialog should "
                + " be closed on OK button press");
            Assert.IsTrue(IsElementFound(PageHeader.UploadWarning),
                "After network disconnect during app upload there should be upload warning");

            _cm.ConnectNetwork();
            Click(PageHeader.UploadWarning);
            Assert.True(IsElementFound(string.Format(PageHeader.TasksTableRowByThreeParams,
                    _playerFile, "Upload has failed", CurrentTenant)),
                $@"There should be app with App/Package '{_playerFile}', " + 
                $@"Status 'Upload has failed', and tenant '{CurrentTenant}' displayed");
            Assert.True(
                IsElementFound(string.Format(PageHeader.TasksTableRowByTextProgressFailed,
                    _playerFile)),
                $@"There should be app with App/Package '{_playerFile}' " +
                @"and red progress bar displayed");
            var progress = Convert.ToInt16(
                GetValue(string.Format(PageHeader.TasksTableRowByTextPercentage, _playerFile))
                .TrimEnd('%'));

            Click(string.Format(PageHeader.TasksTableRowByTextReuploadButton, _playerFile));
            FileManager.Upload(_playerPath);
            var progressNow = Convert.ToInt16(
                GetValue(string.Format(PageHeader.TasksTableRowByTextPercentage, _playerFile), true)
                .TrimEnd('%'));
            Assert.IsTrue(progress <= progressNow,
                "Import percentage should not start from beginning on re-upload app");

            IsElementFound(AppsPage.AppTitle, TestConfig.AppImportTimeout);
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitle, AppTitle.Player) && IsEditMode(),
                $"App {AppTitle.Player} page should be shown and be in edit mode on re-upload end");
        }

        [Test, Regression]
        public void RT14060_UploadAppVersions()
        {
            AppApi.DeleteApps(
                true, 
                new []
                {
                    AppTitle.Player, AppTitle.Haribo, AppTitle.ComposerHq1, AppTitle.ComposerHq2
                }, 
                CurrentTenant);
            AddAppPlayer();
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_compHq1Path);
            CloseTab(GetCurrentTabHandle());
            ReLogin();
            Assert.IsTrue(IsElementFound(PageHeader.UploadWarning),
                "Red warning button should be in page header because of closed tab during app upload");

            Click(PageHeader.UploadWarning);
            var progress = Convert.ToInt16(
                GetValue(
                    string.Format(PageHeader.TasksTableRowByTextPercentage, TestConfig.ComposerHqApp1File))
                .TrimEnd('%'));
            Click(string.Format(PageHeader.TasksTableRowByTextReuploadButton, TestConfig.ComposerHqApp1File));
            FileManager.Upload(_compHq1Path);
            var progressNow = Convert.ToInt16(
                GetValue(
                    string.Format(
                        PageHeader.TasksTableRowByTextPercentage, TestConfig.ComposerHqApp1File), true)
                .TrimEnd('%'));
            Assert.IsTrue(progress <= progressNow,
                "Import percentage should not start from beginning on re-upload app");

            Assert.IsTrue(IsElementNotFound(PageHeader.TasksDialog),
                $"Tasks dialog should be closed on app {AppTitle.ComposerHq1} re-upload and import " +
                "complete");

            Click(string.Format(TenantsPage.TableRowByTitle, TenantTitle.upload1));
            OpenAppsPage();
            Assert.IsTrue(
                IsElementFound(string.Format(AppsPage.TableRowByTwoParams, AppTitle.ComposerHq1, StatusNew)),
                $"App {AppTitle.ComposerHq1} should be shown on App list page in {StatusNew} status");

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_compHq2Path);
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_playerPath);
            Click(PageHeader.UploadSpinner);
            Click(string.Format(PageHeader.TasksTableRowByTextDeleteButton, TestConfig.ComposerHqApp2File));
            Assert.IsTrue(IsElementNotFound(
                    string.Format(PageHeader.TasksTableRowByText, TestConfig.ComposerHqApp2File)),
                $@"App {TestConfig.ComposerHqApp2File} should be deleted from task list on Delete press");
            Assert.IsTrue(IsElementFound(
                    string.Format(PageHeader.TasksTableRowByText, _hariboAppFileName)),
                $@"App {_hariboAppFileName} should continue on other task removal from the tasks list");
            Assert.IsTrue(IsElementFound(
                    string.Format(PageHeader.TasksTableRowByText, _playerFile)),
                $@"App {_playerFile} should continue on other task removal from the tasks list");

            bool isImport;
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                isImport = 
                    IsElementFound(string.Format(
                       PageHeader.TasksTableRowByTwoParams, _hariboAppFileName, "Importing..."), 0.5)
                    || IsElementFound(string.Format(
                       PageHeader.TasksTableRowByTwoParams, _playerFile, "Importing..."), 0.5);
            } 
            while (sw.Elapsed < TimeSpan.FromSeconds(TestConfig.AppImportTimeout) && !isImport);
            sw.Stop();
            Assert.IsTrue(isImport,
                $@"Upload of apps '{AppTitle.Haribo}' and '{AppTitle.Player}' is timed out");
            Click(PageHeader.CancelAllTasksButton);
            Assert.IsTrue(IsElementNotFound(PageHeader.TasksDialog),
                @"Tasks dialog should be closed on 'Cancel All Tasks' button press");

            WaitTime(30);
            RefreshPage();
            Assert.IsTrue(IsElementNotFound(
                    string.Format(AppsPage.TableRowByText, AppTitle.ComposerHq2)),
                $@"App {AppTitle.ComposerHq2} should be not shown on Apps list page (task canceled)");
            Assert.IsTrue(IsElementNotFound(
                    string.Format(AppsPage.TableRowByText, AppTitle.Haribo)),
                $@"App {AppTitle.Haribo} should be not shown on Apps list page (task canceled)");
            Click(string.Format(AppsPage.TableRowByText, AppTitle.Player));
            Assert.IsTrue(IsElementNotFound(string.Format(AppsPage.Version, TestConfig.PlayerAppVersions[1])),
                $@"App {AppTitle.Player} v.{TestConfig.PlayerAppVersions[1]} should be not shown " + 
                "(task canceled)");
        }

        [Test, Regression]
        public void RT14070_UploadAppDuplicates()
        {
            DeleteAppVersion(AppTitle.Player, TestConfig.PlayerAppVersions[1], PlayerApp);
            AppApi.DeleteApps(true, new [] { AppTitle.Haribo }, CurrentTenant);
            AddAppPlayer();
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_playerPath);
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(_playerPath);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorVersionAlreadyExistsDialog, TestConfig.AppImportTimeout),
                $@"Error 'version already exists' should be shown for app {_playerFile} because of " +
                "attempt to upload the same app version twice");

            Click(AppsPage.OkButton);
            Click(string.Format(AppsPage.TableRowByText, AppTitle.Player));
            Assert.IsTrue(
                CountElements(string.Format(AppsPage.Version, TestConfig.PlayerAppVersions[0])) == 1,
                $@"App '{AppTitle.Player}' v.{TestConfig.PlayerAppVersions[0]} should be shown 1 time");
            Assert.IsTrue(
                CountElements(string.Format(AppsPage.Version, TestConfig.PlayerAppVersions[1])) == 1,
                $@"App '{AppTitle.Player}' v.{TestConfig.PlayerAppVersions[1]} should be shown 1 time");

            OpenAppsPage();
            Assert.IsTrue(
                CountElements(string.Format(AppsPage.TableRowByText, AppTitle.Player)) == 1,
                $@"App '{AppTitle.Player}' should be shown 1 time on Apps list page");

            Click(PageHeader.UploadWarning);
            Click(PageHeader.CancelAllTasksButton);
            Click(PageFooter.AddAppInAppsButton);
            var pathFile1 = FileManager.GetFileByVersion(TestConfig.SapPorscheAppFolder, TestConfig.SapPorscheAppFile,
                TestConfig.SapPorscheAppVersions[0]);
            FileManager.UploadAsBackgroundTask(pathFile1);
            Click(PageFooter.AddAppInAppsButton);
            var pathFile2 = FileManager.GetFileByVersion(TestConfig.SapPorscheAppFolder, TestConfig.SapPorscheAppFile,
                TestConfig.SapPorscheAppVersions[1]);
            FileManager.Upload(pathFile2);
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.HariboApp);
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadSpinner, TestConfig.AppImportTimeout), 
                @"3 apps upload & import is timed out");
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadWarning),
                "After import of 3 apps there should be no upload warning on page header");
            Assert.IsTrue(IsEditMode(), "After 3 apps import, the last imported app should be in edit mode");
            Click(PageFooter.SubmitButton);

            Click(PageHeader.PageAppsButton);
            Assert.IsTrue(
                CountElements(string.Format(AppsPage.TableRowByText, AppTitle.Haribo)) == 1,
                $@"App '{AppTitle.Haribo}' should be shown 1 time on Apps list page");
            Assert.IsTrue(
                CountElements(string.Format(AppsPage.TableRowByText, AppTitle.SapPorsche)) == 1,
                $@"App '{AppTitle.SapPorsche}' should be shown 1 time on Apps list page");

            Click(string.Format(AppsPage.TableRowByText, AppTitle.SapPorsche));
            Assert.IsTrue(
                CountElements(string.Format(AppsPage.Version, TestConfig.SapPorscheAppVersions[0])) == 1,
                $@"App version {TestConfig.SapPorscheAppVersions[0]} should be shown 1 time");
            Assert.IsTrue(
                CountElements(string.Format(AppsPage.Version, TestConfig.SapPorscheAppVersions[1])) == 1,
                $@"App version {TestConfig.SapPorscheAppVersions[1]} should be shown 1 time");
        }

        [TearDown]
        public async Task TearDown()
        {
            if (!_cm.IsConnected)
            {
                _cm.ConnectNetwork();
            }
            await TestEnd().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void EndFixtureTests()
        {
            IsUseAllPageReadyChecks = true; // use normal set of page readiness checks
            if (IsEachFixtureInNewBrowser)
            {
                ClosePrimaryBrowser();
            }
            if (TestContext.Parameters.Count == 0)
            {
                PlaceApi.DeletePlaces();
                AppApi.DeleteApps();
                ItemApi.DeleteItems();
            }
        }
    }
}
