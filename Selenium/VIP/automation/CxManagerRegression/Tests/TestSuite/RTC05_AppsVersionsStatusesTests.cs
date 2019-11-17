using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.Apps;
using Models.Places;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC05_AppsVersionsStatusesTests : ParentTest
    {
        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            //if (TestContext.Parameters.Count != 0)
            //{
            //    AppApi.DeleteApps(true, new [] {AppTitle.ComposerVipB});
            //}
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }
            CurrentTenant = TenantTitle.manylang;
            CurrentUser = TestConfig.AdminUser;
        }

        private void PreparePlace(string pathFile)
        {
            EditForm();
            if (!string.IsNullOrEmpty(pathFile))
            {
                Click(PlacesPage.ImageUploadButton);
                FileManager.Upload(pathFile);
            }
            SubmitForm();
        }

        [Test, Regression]
        public void RT05010_AppTwoVersionsUpload()
        {
            TestStart();
            AppApi.DeleteApps(true, new[] { AppTitle.ComposerVipB }, CurrentTenant);

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var earliestComposerVipbAppFileVersion = FileManager.GetFileByVersion(
                TestConfig.ComposerVipbAppFolder,
                TestConfig.ComposerVipbAppFile,
                TestConfig.ComposerVipbAppEarliestVersion);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNew), $@"App Composer VIPB should have Status '{StatusNew}'");
            Assert.IsTrue(IsEditMode(), "App Composer VIPB should be in edit mode");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppEarliestVersion)),
                $@"Versions field should contain app version '{TestConfig.ComposerVipbAppEarliestVersion}'");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppsUri), "User is not redirected to Apps page");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.TableRowComposerVipbAppNew),
                "Canceled app Composer VIPB is visible in Apps table");

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            OpenAppsPage();
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppsUri), "User is not redirected to Apps page");
            
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.TableRowComposerVipbAppNew),
                $"App Composer VIPB should have status {StatusNew} in app table");

            Click(AppsPage.TableRowComposerVipbAppNew);
            Assert.IsTrue(IsViewMode(), "App Composer VIPB should be in view mode");

            EditForm();
            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusAvailable), 
                $@"App Composer VIPB should have Status '{StatusAvailable}'");

            Click(PageFooter.AddVersionButton);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorVersionAlreadyExistsDialog), 
                "Dialog window saying that the app version is already exists should be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorVersionAlreadyExistsDialog),
                "Dialog window saying that the app version is already exists should be closed");
            Assert.IsTrue(IsViewMode(), "App Composer VIPB should be in view mode");

            Click(PageFooter.AddVersionButton);
            var middleComposerVipbAppFileVersion = FileManager.GetFileByVersion(
                TestConfig.ComposerVipbAppFolder,
                TestConfig.ComposerVipbAppFile,
                TestConfig.ComposerVipbAppMiddleVersion);
            FileManager.Upload(middleComposerVipbAppFileVersion);
            Assert.IsTrue(IsEditMode(), "App Composer VIPB should be in edit mode");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppMiddleVersion)),
                $@"Composer VIPB app version '{TestConfig.ComposerVipbAppMiddleVersion}' should be shown in Versions field");
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusNew), $"App Composer VIPB should have status {StatusNew}");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $"App Composer VIPB should have status {StatusAvailable}");
        }

        [Test, Regression]
        public void RT05020_AppVersionsAddDelete()
        {
            AppResponse app = null;
            Place place = null;
            AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion);
            TestStart();
            Parallel.Invoke(
                () => app = AddAppComposerVipB(TestConfig.ComposerVipbAppMiddleVersion),
                () => place = AddPlaceWw(PlaceStatus.NoDevice, pageToBeOpened: 0)
            );
            AppApi.DeleteAppVersion(app, TestConfig.ComposerVipbAppEarliestVersion);

            OpenEntityPage(app);
            Click(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppMiddleVersion));
            Click(PageFooter.AddVersionButton);
            var earliestComposerVipbAppFileVersion = FileManager.GetFileByVersion(
                TestConfig.ComposerVipbAppFolder,
                TestConfig.ComposerVipbAppFile,
                TestConfig.ComposerVipbAppEarliestVersion);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusNew), $"App Composer VIPB should have status {StatusNew}");
            Assert.IsTrue(IsEditMode(), "App Composer VIPB should be in edit mode");

            Click(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppEarliestVersion));
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusNew)),
                $@"Version {TestConfig.ComposerVipbAppEarliestVersion} should have status '{StatusNew}'");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusAvailable)),
                $@"Version {TestConfig.ComposerVipbAppEarliestVersion} should have status '{StatusAvailable}'");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddVersionButton),
                "Add Version button should be present on app versions page");

            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusNew));
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton), 
                "Delete button should be present in page footer");

            Click(PageFooter.DeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.DeleteConfirmationDialog),
                "Version delete confirmation dialog should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.DeleteButton),
                "Delete button should be present in version delete confirmation dialog");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.CancelButton),
                "Cancel button should be present in version delete confirmation dialog");

            Click(AppsPage.DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.DeleteConfirmationDialog),
                "Version delete confirmation dialog should be closed");
            Assert.IsTrue(IsElementNotFound(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusNew)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be absent in list after its removal");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in versions list");

            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsViewMode(), "App Composer VIPB should be in view mode");
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable), 
                $"App Composer VIPB should have status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppMiddleVersion)),
                $"App Composer VIPB version {TestConfig.ComposerVipbAppMiddleVersion} should be shown in Versions field");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppEarliestVersion)),
                $"App Composer VIPB version {TestConfig.ComposerVipbAppEarliestVersion} should be absent in Versions field");

            Click(PageFooter.AddVersionButton);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            Assert.IsTrue(IsElementFound(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppEarliestVersion)),
                $"App Composer VIPB version {TestConfig.ComposerVipbAppEarliestVersion} should be present in Versions field");
            SubmitForm();
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $"App Composer VIPB should have status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppMiddleVersion)),
                $"App Composer VIPB version {TestConfig.ComposerVipbAppMiddleVersion} should be shown in Versions field");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppEarliestVersion)),
                $"App Composer VIPB version {TestConfig.ComposerVipbAppEarliestVersion} should be shown in Versions field");

            OpenEntityPage(place);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(string.Format(PlacesPage.TableSelectAppTitle, AppTitle.ComposerVipB));
            Assert.IsTrue(AreElementsContainText(PlacesPage.AppsSectionTableRow1AppPackageCell, TestConfig.ComposerVipbAppMiddleVersion),
                $"App Composer VIPB version {TestConfig.ComposerVipbAppMiddleVersion} should be added to WW place");

            SubmitForm();
            OpenEntityPage(app);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusPublished),
                $"App Composer VIPB should have status {StatusPublished}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.PlacesSectionPlaceByTitle, place.Title)),
                $"Places section should have place {place.Title}");

            Click(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppEarliestVersion));
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable)),
                $@"Version {TestConfig.ComposerVipbAppEarliestVersion} should have status '{StatusAvailable}'");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusPublished)),
                $@"Version {TestConfig.ComposerVipbAppEarliestVersion} should have status '{StatusPublished}'");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddVersionButton),
                "Add Version should be present on app versions page");

            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusPublished));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.MarkAsDeletedButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorCantBeDeletedDialog),
                "Error message saying that the version cannot be deleted should be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorCantBeDeletedDialog),
                "Error message saying that the version cannot be deleted should be closed");

            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.MarkAsDeletedButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.DeleteConfirmationDialog),
                "Delete app version confirmation dialog should be closed");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusDeleted)),
                $@"Version {TestConfig.ComposerVipbAppEarliestVersion} should have status '{StatusDeleted}'");

            Click(PageHeader.NavigateBackButton);
            TurnOffInfoPopups();
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.Version, TestConfig.ComposerVipbAppMiddleVersion)),
                $"App Composer VIPB version {TestConfig.ComposerVipbAppMiddleVersion} should be shown in Versions field");
            Assert.IsTrue(IsElementEquals(AppsPage.VersionInactive, "1 inactive"),
                @"App Composer VIPB version '1 inactive' should be shown in Versions field");

            Click(string.Format(AppsPage.PlacesSectionPlaceByTitle, place.Title));
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlaceUri}/{place.Id}"),
                "User is not redirected to place page");
            
            EditForm();
            Click(PlacesPage.AppsSectionTableRow1);
            Click(PlacesPage.AppsSectionTableRow1DeleteButton);
            SubmitForm();
            OpenEntityPage(app);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $"App Composer VIPB should have status {StatusAvailable}");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.PlacesSection), 
                "Places section should now be absent in the app");

            Click(AppsPage.VersionInactive);
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusDeleted)),
                $@"Version {TestConfig.ComposerVipbAppEarliestVersion} should have status '{StatusDeleted}'");

            SetFilter("");
            Assert.IsTrue(IsElementFound(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppMiddleVersion} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusDeleted)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in list with status {StatusDeleted}");

            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Assert.IsTrue(IsElementNotFound(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppMiddleVersion} should be absent in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusDeleted)),
                $"Version {TestConfig.ComposerVipbAppMiddleVersion} should be absent in list with status {StatusDeleted}");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusDeleted)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in list with status {StatusDeleted}");

            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusDeleted),
                $"App Composer VIPB should have status {StatusDeleted}");
            Assert.IsTrue(IsElementEquals(AppsPage.Versions, string.Empty),
                @"Versions field should not contain active versions within");
            Assert.IsTrue(IsElementEquals(AppsPage.VersionsInactive, "1 inactive"),
                @"Versions field should contain only '1 inactive' text within");

            Click(PageFooter.AddVersionButton);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorVersionAlreadyExistsDialog),
                "Dialog saying that the app version already exists should be shown");

            Click(AppsPage.OkButton);
            Click(AppsPage.VersionInactive);
            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusDeleted));
            Assert.IsTrue(IsElementFound(PageFooter.AddVersionButton),
                "Add Version button should be present on app versions page");
            Assert.IsTrue(IsElementFound(PageFooter.RestoreButton),
                "Restore button should be present on app versions page");

            Click(PageFooter.RestoreButton);
            SetFilter("");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in list with status {StatusAvailable}");

            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Assert.IsTrue(IsElementNotFound(AppsPage.TableRowComposerVipbApp),
                "App Composer VIPB should be completely deleted");

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            SubmitForm();
            Assert.IsTrue(IsElementEquals(AppsPage.Versions, TestConfig.ComposerVipbAppEarliestVersion),
                $"Versions field should contain only app version {TestConfig.ComposerVipbAppEarliestVersion}");
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $"App Composer VIPB should have status {StatusAvailable}");
        }

        [Test, Regression]
        public void RT05030_AppThreeVersionsStatusChange()
        {
            Place place1 = null;
            AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion, AppStatus.Deleted);
            Parallel.Invoke(
                () => AddAppComposerVipB(TestConfig.ComposerVipbAppMiddleVersion),
                () => place1 = AddPlaceWw(PlaceStatus.NoDevice, pageToBeOpened: 0)
            );
            TestStart();
            var place2 = AddPlaceWw(PlaceStatus.NoDevice, isCreateNewPlace: true);
            var app = AddAppComposerVipB(TestConfig.ComposerVipbAppLatestVersion, AppStatus.New);
            PreparePlace(TestConfig.Image138);

            OpenEntityPage(place1);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(string.Format(PlacesPage.TableSelectAppTitle, AppTitle.ComposerVipB));
            DropDownSelect(PlacesPage.AppsSectionTableRowDetailsAutoPackageUpdateDropDown, "None");
            DropDownSelect(PlacesPage.AppsSectionTableRowDetailsAppPackageDropDown, TestConfig.ComposerVipbAppMiddleVersion);
            Assert.IsTrue(AreElementsContainText(
                    PlacesPage.AppsSectionTableRow1AppPackageCell, TestConfig.ComposerVipbAppMiddleVersion),
                $"App version assigned to WW place should be {TestConfig.ComposerVipbAppMiddleVersion}");

            SubmitForm();
            OpenEntityPage(app);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNew), $"App Status should be {StatusNew}");

            EditForm();
            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusPublished), $"App Status should be {StatusPublished}");

            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusDeleted));
            Click(PageFooter.RestoreButton);
            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusPublished), $"App Status should be {StatusPublished}");

            OpenEntityPage(place2);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(string.Format(PlacesPage.TableSelectAppTitle, AppTitle.ComposerVipB));
            DropDownSelect(PlacesPage.AppsSectionTableRowDetailsAutoPackageUpdateDropDown, "None");
            Assert.IsTrue(AreElementsContainText(PlacesPage.AppsSectionTableRow1AppPackageCell, TestConfig.ComposerVipbAppLatestVersion),
                $"WW place should contain app version {TestConfig.ComposerVipbAppLatestVersion} in Apps section");

            ClickUntilShown(PlacesPage.AppsSectionTableRowDetailsAppPackageDropDown,
                CommonElement.DropDownOptionList);
            Assert.IsTrue(AreElementsContainText(CommonElement.DropDownOptionList, TestConfig.ComposerVipbAppEarliestVersion),
                $"App Package dropdown should contain app version {TestConfig.ComposerVipbAppEarliestVersion}");
            Assert.IsTrue(AreElementsContainText(CommonElement.DropDownOptionList, TestConfig.ComposerVipbAppMiddleVersion),
                $"App Package dropdown should contain app version {TestConfig.ComposerVipbAppMiddleVersion}");
            Assert.IsTrue(AreElementsContainText(CommonElement.DropDownOptionList, TestConfig.ComposerVipbAppLatestVersion),
                $"App Package dropdown should contain app version {TestConfig.ComposerVipbAppLatestVersion}");

            SubmitForm();
            OpenEntityPage(app);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusPublished), $"App Status should be {StatusPublished}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.PlacesSectionPlaceByTitle, place1.Title)),
                $"Places section should contain place1 '{place1.Title}'");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.PlacesSectionPlaceByTitleAndPicture, place2.Title)),
                $"Places section should contain place2 '{place2.Title}' with pre-loaded picture");

            Click(string.Format(AppsPage.PlacesSectionPlaceByTitleAndPicture, place2.Title));
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlaceUri}/{place2.Id}"),
                "User is not redirected to place2 page");

            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteButton);
            OpenEntityPage(app);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusPublished), $"App Status should be {StatusPublished}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.PlacesSectionPlaceByTitle, place1.Title)),
                $"Places section should contain place1 '{place1.Title}'");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(AppsPage.PlacesSectionPlaceByTitle, place2.Title)),
                $"Places section should not contain place2 '{place2.Title}'");

            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusPublished)),
                $"Version {TestConfig.ComposerVipbAppMiddleVersion} should be present in list with status {StatusPublished}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppLatestVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppLatestVersion} should be present in list with status {StatusAvailable}");

            OpenEntityPage(place2);
            Assert.IsTrue(AreElementsContainText(PlacesPage.AppsSectionTableRow1TitleCell, AppTitle.ComposerVipB),
                "Deleted place2 should contain Composer VIPB app in Apps section");

            Click(PageFooter.RestoreButton);
            SubmitForm();
            OpenEntityPage(app);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusPublished), $"App Status should be {StatusPublished}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.PlacesSectionPlaceByTitle, place1.Title)),
                $"Places section should contain place1 '{place1.Title}'");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.PlacesSectionPlaceByTitleAndPicture, place2.Title)),
                $"Places section should contain place2 '{place2.Title}' with pre-loaded picture");

            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusPublished)),
                $"Version {TestConfig.ComposerVipbAppMiddleVersion} should be present in list with status {StatusPublished}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppLatestVersion, StatusPublished)),
                $"Version {TestConfig.ComposerVipbAppLatestVersion} should be present in list with status {StatusPublished}");

            OpenEntityPage(place1);
            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteButton);
            OpenEntityPage(place2);
            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteButton);
            OpenEntityPage(app);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusAvailable), $"App Status should be {StatusAvailable}");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.PlacesSection), "Places section should be absent");

            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppMiddleVersion} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppLatestVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppLatestVersion} should be present in list with status {StatusAvailable}");

            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppLatestVersion, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.TableRowComposerVipbApp), 
                "Composer VIPB app should not be present as all its version have been deleted");

            OpenEntityPage(place2);
            Assert.IsTrue(IsElementNotFound(PlacesPage.AppsSectionTableHeader), 
                "Place2 should not contain Apps section anymore");
        }

        [Test, Regression]
        public void RT05040_TwoUsersAndAppTimestamps()
        {
            CurrentTenant = TenantTitle.manylang;
            TestStart();
            AppApi.DeleteApps(true, new [] { AppTitle.ComposerVipB, AppTitle.ComposerVipB2 }, CurrentTenant);
            CurrentTenant = TenantTitle.nolang;
            AppApi.DeleteApps(true, new [] { AppTitle.ComposerVipB, AppTitle.ComposerVipB2 }, CurrentTenant);
            OpenSecondaryBrowser(); // 2
            CurrentTenant = TenantTitle.manylang;
            NavigateAndLogin(TestConfig.BaseUrl, TestConfig.AdminUser2);

            // 1
            SwitchToAnotherBrowser();
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var earliestComposerVipbAppFileVersion = FileManager.GetFileByVersion(
                TestConfig.ComposerVipbAppFolder,
                TestConfig.ComposerVipbAppFile,
                TestConfig.ComposerVipbAppEarliestVersion);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            SubmitForm();
            var created1 = GetTimestamp(AppsPage.Created);
            var modified1 = GetTimestamp(AppsPage.Modified);
            Assert.IsTrue(Math.Abs((created1.Item1 - modified1.Item1).Seconds) <= 10, 
                "Created timestamp data should be equal to Modified field data");
            var userProperties = UserDirectoryApi.GetUserData(TestConfig.AdminUser);
            var userName = $"{userProperties.GivenName} {userProperties.FamilyName}";
            Assert.IsTrue(created1.Item2.Contains(userName), 
                "Created should contain current user firstname and lastname");
            Assert.IsTrue(modified1.Item2.Contains(userName), 
                "Modified should contain current user firstname and lastname");

            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFound(
                    string.Format(AppsPage.TableRowByThreeParams, 
                        AppTitle.ComposerVipB, 
                        userName, 
                        modified1.Item1.ToString("H:mm"))),
                "App Composer VIPB version should contain correct title, current user name and modified time: " +
                $@"'{AppTitle.ComposerVipB}', '{userName}', '{modified1.Item1:H:mm}'");

            // 2
            SwitchToAnotherBrowser();
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorVersionAlreadyExistsDialog),
                "Error dialog that the version is already exists should be shown");

            Click(AppsPage.OkButton);
            Click(AppsPage.TableRowComposerVipbApp);
            Click(PageFooter.AddVersionButton);
            FileManager.Upload(TestConfig.ComposerVipbApp2File);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorIsNotAVersionDialog),
                "Error dialog that the app is not a version of the app should be shown");

            Click(AppsPage.OkButton);
            WaitTime(2);
            Click(PageFooter.AddVersionButton);
            var middleComposerVipbAppFileVersion = FileManager.GetFileByVersion(
                TestConfig.ComposerVipbAppFolder,
                TestConfig.ComposerVipbAppFile,
                TestConfig.ComposerVipbAppMiddleVersion);
            FileManager.Upload(middleComposerVipbAppFileVersion);
            SubmitForm();
            var created2 = GetTimestamp(AppsPage.Created);
            var modified2 = GetTimestamp(AppsPage.Modified);
            var user2Properties = UserDirectoryApi.GetUserData(TestConfig.AdminUser2);
            var user2Name = $"{user2Properties.GivenName} {user2Properties.FamilyName}";
            Assert.IsTrue(created2.Equals(created1), "Created timestamp and user data should never change");
            Assert.IsTrue(modified2.Item1 > modified1.Item1, "Modified timestamp data should increase");
            Assert.IsTrue(modified2.Item2.Contains(user2Name), 
                "Modified should contain current user firstname and lastname");
        
            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFound(string.Format(
                        AppsPage.TableRowByThreeParams, 
                        TestConfig.ComposerVipbAppEarliestVersion, 
                        userName, 
                        modified1.Item1.ToString("H:mm"))),
                $"App version {TestConfig.ComposerVipbAppEarliestVersion} should contain previous user " +
                "name and correct modified time");
            Assert.IsTrue(IsElementFound(string.Format(
                        AppsPage.TableRowByThreeParams, 
                        TestConfig.ComposerVipbAppMiddleVersion, 
                        user2Name, 
                        modified2.Item1.ToString("H:mm"))),
                $"App version {TestConfig.ComposerVipbAppMiddleVersion} should contain current user name " +
                "and correct modified time");
            
            WaitTime(60); //
            OpenAppsPage();
            Click(AppsPage.TableRowComposerVipbApp);
            EditForm();
            var newAppTitle = $"Auto test {RandomNumber}";
            SendText(AppsPage.AppTitle, newAppTitle, isCheckInput: true);
            SubmitForm();
            var modified3 = GetTimestamp(AppsPage.Modified).Item1;
            Assert.IsTrue(modified3 > modified1.Item1, "Modified timestamp data should increase");

            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFound(string.Format(
                    AppsPage.TableRowByThreeParams, 
                    TestConfig.ComposerVipbAppEarliestVersion, 
                    userName, 
                    modified1.Item1.ToString("H:mm"))),
                $"App version {TestConfig.ComposerVipbAppEarliestVersion} should contain previous user name " +
                "and correct modified time");
            Assert.IsTrue(IsElementFound(string.Format(
                    AppsPage.TableRowByThreeParams, 
                    TestConfig.ComposerVipbAppMiddleVersion, 
                    user2Name, 
                    modified3.ToString("H:mm"))),
                $"App version {TestConfig.ComposerVipbAppMiddleVersion} should contain current user name " + 
                "and correct modified time");

            WaitTime(2);
            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);

            Click(PageHeader.NavigateBackButton);
            var created4 = GetTimestamp(AppsPage.Created);
            var modified4 = GetTimestamp(AppsPage.Modified);
            Assert.IsTrue(created4.Equals(created1), "Created timestamp and user data should never change");
            Assert.IsTrue(modified4.Item1 > modified3, "Modified timestamp data should increase");

            RefreshPage();
            Click(PageFooter.AddVersionButton);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            Assert.IsTrue(IsElementNotFound(PageHeader.UploadSpinner, 45),
                $"App version {TestConfig.ComposerVipbAppEarliestVersion} upload timeout");
            //Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusNew),
            //    $"App version {TestConfig.ComposerVipbAppEarliestVersion} should have status {StatusNew}" );         
            // 1
            SwitchToAnotherBrowser();
            OpenAppsPage();
            ClickUntilShown(string.Format(AppsPage.TableRowByText, newAppTitle), AppsPage.Status);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusAvailable), $"App Status should be {StatusAvailable}");

            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusNew)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in list with status {StatusNew}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppMiddleVersion} should be present in list with status {StatusAvailable}");

            Click(PageFooter.AddVersionButton);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorVersionAlreadyExistsDialog),
                "Error dialog that the app version already exists should be shown");

            Click(AppsPage.OkButton);
            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppMiddleVersion, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusNew)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in list with status {StatusNew}");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(
                    AppsPage.TableRowByText, TestConfig.ComposerVipbAppMiddleVersion)),
                $"Version {TestConfig.ComposerVipbAppMiddleVersion} should be absent");
            OpenAppsPage();

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(earliestComposerVipbAppFileVersion);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorVersionAlreadyExistsDialog),
                "Error dialog that the app version already exists should be shown");
            Click(AppsPage.OkButton);

            // 2
            SwitchToAnotherBrowser();
            OpenAppsPage();
            ClickUntilShown(string.Format(AppsPage.TableRowByText, newAppTitle), AppsPage.Status);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNew), $"App Status should be {StatusNew}");
            EditForm();
            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusAvailable), $"App Status should be {StatusAvailable}");

            // 1
            SwitchToAnotherBrowser();
            OpenAppsPage();
            ClickUntilShown(string.Format(AppsPage.TableRowByText, newAppTitle), AppsPage.Status);
            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable)),
                $"Version {TestConfig.ComposerVipbAppEarliestVersion} should be present in list with status {StatusAvailable}");
            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerVipbAppEarliestVersion, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.MarkAsDeletedButton);
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.AppsUri}"));
            
            Click(PageFooter.ShowDeletedButton);
            ClickUntilShown(string.Format(AppsPage.TableRowByText, newAppTitle), AppsPage.Status);
            var modified5 = GetTimestamp(AppsPage.Modified).Item1;
            Assert.IsTrue(modified5 > modified4.Item1, "Modified timestamp data should increase");

            Click(AppsPage.VersionInactive);
            Assert.IsTrue(IsElementFound(string.Format(
                    AppsPage.TableRowByThreeParams, 
                    TestConfig.ComposerVipbAppEarliestVersion, 
                    user2Name, 
                    modified5.ToString("H:mm"))),
                $"App version {TestConfig.ComposerVipbAppEarliestVersion} should contain previous user name and " + 
                "correct modified time");

            // 2
            SwitchToAnotherBrowser();
            ChangeTenant(TenantTitle.nolang);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.ComposerVipbApp2File);
            Assert.IsTrue(IsEditMode(), "App Composer VIPB 2 should be uploaded successfully");
            SubmitForm();
        }

        [Test, Regression]
        public void RT05050_TwoUsersAndAppVersionList()
        {
            CurrentTenant = TenantTitle.onelang;
            AddAppPlayer();
            TestStart();
            AppApi.DeleteApps(true, new [] { AppTitle.ComposerHq1, AppTitle.ComposerHq2 }, CurrentTenant);
            
            // 1
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.ComposerHqApp1Folder + "\\" + TestConfig.ComposerHqApp1File);
            SubmitForm();
            Assert.IsTrue(IsElementEquals(AppsPage.Versions, TestConfig.ComposerHqApp1Version), 
                $"Uploaded Composer HQ 1 app should have version {TestConfig.ComposerHqApp1Version}");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.ComposerHqApp1Folder + "\\" + TestConfig.ComposerHqApp1File);
            var appVersions = GetValuesAsString(AppsPage.Versions);
            Assert.IsTrue(appVersions.Contains(TestConfig.ComposerHqApp1Version) && appVersions.Contains(TestConfig.ComposerHqApp1Version2),
                $"Uploaded Composer HQ 1 app should have versions {TestConfig.ComposerHqApp1Version2}, {TestConfig.ComposerHqApp1Version}");

            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version, StatusAvailable)),
                $"Version {TestConfig.ComposerHqApp1Version} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version2, StatusNew)),
                $"Version {TestConfig.ComposerHqApp1Version2} should be present in list with status {StatusNew}");

            // 2
            OpenSecondaryBrowser();
            NavigateAndLogin(TestConfig.BaseUrl, TestConfig.AdminUser2);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.ComposerHqApp1Folder + "\\" + TestConfig.ComposerHqApp1File);
            SubmitForm();
            appVersions = GetValuesAsString(AppsPage.Versions);
            Assert.IsTrue(appVersions.Contains(TestConfig.ComposerHqApp1Version) 
                          && appVersions.Contains(TestConfig.ComposerHqApp1Version2)
                          && appVersions.Contains(TestConfig.ComposerHqApp1Version3),
                $"Uploaded Composer HQ 1 app should have versions {TestConfig.ComposerHqApp1Version3}, " + 
                $"{TestConfig.ComposerHqApp1Version2}, {TestConfig.ComposerHqApp1Version}");

            // 1
            SwitchToAnotherBrowser();
            Click(PageHeader.NavigateBackButton);
            appVersions = GetValuesAsString(AppsPage.Versions);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable), 
                $"App should be in status {StatusAvailable}");
            Assert.IsTrue(appVersions.Contains(TestConfig.ComposerHqApp1Version) 
                          && appVersions.Contains(TestConfig.ComposerHqApp1Version2)
                          && appVersions.Contains(TestConfig.ComposerHqApp1Version3),
                $"Uploaded Composer HQ 1 app should have versions {TestConfig.ComposerHqApp1Version3}, " +
                $"{TestConfig.ComposerHqApp1Version2}, {TestConfig.ComposerHqApp1Version}");
            
            EditForm();
            SubmitForm();
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable), 
                $"App should be in status {StatusAvailable}");

            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version2, StatusNew));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version2, StatusNew)),
                $"Version {TestConfig.ComposerHqApp1Version2} should be deleted");

            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable), 
                $"App should be in status {StatusAvailable}");

            Click(PageFooter.AddVersionButton);
            FileManager.Upload(TestConfig.ComposerHqApp2Folder + "\\" + TestConfig.ComposerHqApp2File);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorIsNotAVersionDialog),
                "Error that this is not a version of uploaded app should be displayed");

            Click(AppsPage.OkButton);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.ComposerHqApp2Folder + "\\" + TestConfig.ComposerHqApp2File);
            Assert.IsTrue(IsEditMode(), "App Composer HQ 2 should be uploaded successfully");

            // 2
            SwitchToAnotherBrowser();
            RefreshPage();
            EditForm();
            SendText(AppsPage.AppTitle, AppTitle.ComposerHq2, isCheckInput: true);
            SubmitForm();
            Assert.IsTrue(IsElementFound(AppsPage.ErrorAppTitleIsUsedDialog), 
                "Error dialog that the name is already used by another app should be displayed");

            Click(AppsPage.OkButton);
            Click(PageFooter.CancelButton);
            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version3, StatusAvailable));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.MarkAsDeletedButton);
            SetFilter("Composer");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version, StatusAvailable)),
                $"Version {TestConfig.ComposerHqApp1Version} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version3, StatusDeleted)),
                $"Version {TestConfig.ComposerHqApp1Version3} should be present in list with status {StatusDeleted}");

            SetFilter(TestConfig.ComposerHqApp1Version);
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version, StatusAvailable)),
                $"Version {TestConfig.ComposerHqApp1Version} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version3, StatusDeleted)),
                $"Version {TestConfig.ComposerHqApp1Version3} should not be present in list with status {StatusDeleted}");

            var userProperties = UserDirectoryApi.GetUserData(TestConfig.AdminUser2);
            SetFilter(userProperties.GivenName);
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version, StatusAvailable)),
                $"Version {TestConfig.ComposerHqApp1Version} should not be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version3, StatusDeleted)),
                $"Version {TestConfig.ComposerHqApp1Version3} should be present in list with status {StatusDeleted}");

            SetFilter("today");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version, StatusAvailable)),
                $"Version {TestConfig.ComposerHqApp1Version} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version3, StatusDeleted)),
                $"Version {TestConfig.ComposerHqApp1Version3} should be present in list with status {StatusDeleted}");

            SetFilter("1..");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version, StatusAvailable)),
                $"Version {TestConfig.ComposerHqApp1Version} should not be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version3, StatusDeleted)),
                $"Version {TestConfig.ComposerHqApp1Version3} should not be present in list with status {StatusDeleted}");

            Click(PageHeader.NavigateBackButton);
            Click(AppsPage.Versions);
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version, StatusAvailable)),
                $"Version {TestConfig.ComposerHqApp1Version} should be present in list with status {StatusAvailable}");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version3, StatusDeleted)),
                $"Version {TestConfig.ComposerHqApp1Version3} should be present in list with status {StatusDeleted}");
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, string.Empty) 
                          || IsElementEquals(PageHeader.Filter, "Filter"), 
                "Filter input field should be empty on versions page open");
        }

        [TearDown]
        public async Task TearDown()
        {
            var task = TestEnd();
            var browserTabs = GetTabHandles();
            if (browserTabs != null && browserTabs.Count > 1)
            {
                foreach (var browserTab in browserTabs.Skip(1))
                {
                    CloseTab(browserTab);
                }
            }
            await task;   
        }

        [OneTimeTearDown]
        public void EndFixtureTests()
        {
            if (IsEachFixtureInNewBrowser)
            {
                ClosePrimaryBrowser();
            }
            if (TestContext.Parameters.Count == 0)
            {
                PlaceApi.DeletePlaces();
                AppApi.DeleteApps();
            }
        }
    }
}
