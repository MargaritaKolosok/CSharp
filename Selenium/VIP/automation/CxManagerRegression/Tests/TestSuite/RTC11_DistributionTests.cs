using System;
using System.IO;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.Apps;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC11_DistributionTests : ParentTest
    {
        private string _rootTitle,
            _distribution1Title,
            _distributionEn1Title,
            _distributionEnDeTitle,
            _distributionDeEnTitle,
            _distributionArTitle;
        private const string TitleColumnName = "Tenant";
        private const string TenantCodeColumnName = "Tenant code";
        private const string CreatedByColumnName = "Created by";
        private const string ModifiedColumnName = "Modified";
        private const string AscOrderAttrValue = "ascending";
        private const string DescOrderAttrValue = "descending";

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            CurrentUser = TestConfig.AdminUser;
            _distribution1Title = TenantTitle.Distribution1.ToString();
            _distributionEn1Title = TenantTitle.DistributionEN1.ToString();
            _rootTitle = TenantTitle.Root.ToString();
            _distributionEnDeTitle = TenantTitle.DistributionEN_DE.ToString();
            _distributionDeEnTitle = TenantTitle.DistributionDE_EN.ToString();
            _distributionArTitle = TenantTitle.DistributionAR.ToString();
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }

            CurrentTenant = TenantTitle.Distribution1;
            CurrentUser = TestConfig.AdminUser;
        }

        [Test, Regression]
        public void RT11010_DistributeApp()
        {
            var app = AddAppPlayer();
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin }, CurrentTenant);
            TestStart(isSelectTenant: false);

            OpenEntityPage(app);
            Assert.IsTrue(IsElementNotFound(PageFooter.DistributeSubMenu),
                @"Distribute sub-menu should be unavailable in footer");

            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin }, 
                TenantTitle.Distribution1, 
                TenantTitle.DistributionEN1);
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageFooter.DistributeSubMenu),
                "Distribute sub-menu should be available in footer");

            MouseOver(PageFooter.DistributeSubMenu);
            Assert.IsTrue(IsElementFound(PageFooter.DistributeAppButton),
                "Distribute App button should be available in footer");
            Assert.IsTrue(IsElementNotFound(PageFooter.DistributeMasterDataButton),
                "Distribute Master Data button should be unavailable in footer");

            Click(PageFooter.DistributeAppButton);
            Assert.IsTrue(IsElementFound(TenantsPage.TenantsDialog),
                "Click on Distribute App button should open Tenants dialog");

            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title)),
                $"Tenant {_distribution1Title} should be collapsed");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $"Tenant {_rootTitle} should not be shown as user does not have access to it");

            Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowDisabledByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be shown disabled");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title)),
                $"Tenant {_distribution1Title} should be shown");

            Assert.IsTrue(IsElementFoundQuickly(TenantsPage.SelectAllButton),
                "Select All button should be available in modal footer");
            Assert.IsTrue(IsElementFoundQuickly(TenantsPage.DeselectAllButton),
                "Deselect All button should be available in modal footer");
            Assert.IsTrue(IsElementFoundQuickly(TenantsPage.CancelButton),
                "Cancel button should be available in modal footer");
            Assert.IsTrue(IsElementFoundQuickly(TenantsPage.UploadDisabledButton),
                "Upload button should be disabled in modal footer");

            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, "")),
                "There should be no tenants selected by default");

            Click(string.Format(TenantsPage.TableRowByTitle, _distribution1Title));
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, "")),
                $"There should be no tenants selected on {_distribution1Title} tenant click");

            Click(TenantsPage.UploadDisabledButton);
            Assert.IsTrue(IsElementFoundQuickly(TenantsPage.TenantsDialog),
                "There should be tenants modal still displayed on disabled Upload button click");

            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title));
            Assert.IsTrue(
                IsElementFound(string.Format(TenantsPage.TableRowSelectedByTitle, _distributionEn1Title)),
                $"Tenant {_distributionEn1Title} should be shown selected on click");
            Assert.IsTrue(IsElementFound(TenantsPage.UploadButton),
                "Upload button should be shown and active in modal footer");

            Click(TenantsPage.UploadButton);
            Assert.IsTrue(IsElementNotFound(TenantsPage.TenantsDialog, 60),
                "There should be no modal dialog displayed on Upload button click");
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog, 60),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be closed on OK button click");
            Assert.IsTrue(IsViewMode(), "App page should be in view mode after upload to another tenant");

            ChangeTenant(TenantTitle.DistributionEN1);
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableRowPlayerApp), "Player app should be in apps list");

            Click(AppsPage.TableRowPlayerApp);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $@"App Status should be equal to '{StatusAvailable}'");
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitleReadOnly, AppTitle.Player),
                $@"App AppTitle should be equal to '{AppTitle.Player}'");
            Assert.IsTrue(IsElementEquals(AppsPage.Versions, TestConfig.PlayerAppVersions[0]),
                $@"App Versions should be equal to '{TestConfig.PlayerAppVersions[0]}'");
            
            Assert.IsTrue(CompareTimestamps(AppsPage.Created, AppsPage.Modified) == Equality.Equal, 
                "Created timestamp should be equal to Modified field");
            var userProperties = UserDirectoryApi.GetUserData(TestConfig.DistributionUser);
            var userName = $"{userProperties.GivenName} {userProperties.FamilyName}";
            Assert.IsTrue(IsUserInTimestamp(AppsPage.Created, userName),
                "Created should contain firstname and lastname");
            Assert.IsTrue(IsUserInTimestamp(AppsPage.Modified, userName),
                "Modified should contain firstname and lastname");

            EditForm();
            Assert.IsTrue(IsElementNotFound(PageFooter.DistributeSubMenu),
                "Distribute button should not be available in edit mode");

            Click(PageFooter.CancelButton);
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Assert.IsTrue(IsElementFound(AppsPage.NoAppropriateTenantToDistributeDialog),
                @"Dialog 'no appropriate tenant to distribute to' should be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.NoAppropriateTenantToDistributeDialog),
                @"Dialog 'no appropriate tenant to distribute to' should be closed on OK button click");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetEarliestFileVersion(TestConfig.PlayerAppFolder, TestConfig.PlayerAppFile);
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorVersionAlreadyExistsDialog, 90),
                $"Error the app version {TestConfig.PlayerAppVersions[0]} already exists should be shown");
            Click(AppsPage.OkButton);
        }

        [Test, Regression]
        public void RT11020_DistributeAppVersion()
        {
            AddAppPlayer();
            CurrentTenant = TenantTitle.DistributionEN1;
            var app = AddAppPlayer();
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE);
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.PlayerAppFolder, TestConfig.PlayerAppFile,
                TestConfig.PlayerAppVersions[1]);
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsElementFound(AppsPage.Status, 120), 
                $"Player app v.{TestConfig.PlayerAppVersions[1]} import timeout");
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusNew),
                $@"Uploaded app v.{TestConfig.PlayerAppVersions[1]} status should be '{StatusNew}'");
            OpenEntityPage(app);
            Assert.IsTrue(IsElementNotFound(PageFooter.DistributeSubMenu),
                $@"Distribute sub-menu should be not shown in page footer when status '{StatusNew}'");
            var imgSource = GetElementAttribute(AppsPage.Image, "src");

            EditForm();
            SubmitForm();
            Assert.IsTrue(IsElementFound(PageFooter.DistributeSubMenu),
                "Distribute sub-menu should be shown in page footer after app submit");

            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Assert.IsTrue(IsElementFound(TenantsPage.TenantsDialog), 
                "Tenants modal dialog should be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title)),
                $"Tenant {_distribution1Title} should be shown collapsed");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle)),
                $"Tenant {_distributionArTitle} should be shown");

            Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle)),
                $"Tenant {_distributionEnDeTitle} should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title)),
                $"Tenant {_distributionEn1Title} should not be shown");

            Click(string.Format(TenantsPage.TableRowByTitle, _distribution1Title));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be selected in table");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowUnselectedByTitle, _distributionEnDeTitle)),
                $"Tenant {_distributionEnDeTitle} should be not selected in table");

            Click(TenantsPage.SelectAllButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be selected in table");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distributionEnDeTitle)),
                $"Tenant {_distributionEnDeTitle} should be selected in table");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distributionArTitle)),
                $"Tenant {_distributionArTitle} should be selected in table");

            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be selected in table");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowUnselectedByTitle, _distributionEnDeTitle)),
                $"Tenant {_distributionEnDeTitle} should be not selected in table");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distributionArTitle)),
                $"Tenant {_distributionArTitle} should be selected in table");

            Click(TenantsPage.DeselectAllButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowUnselectedByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be not selected in table");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowUnselectedByTitle, _distributionEnDeTitle)),
                $"Tenant {_distributionEnDeTitle} should be not selected in table");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowUnselectedByTitle, _distributionArTitle)),
                $"Tenant {_distributionArTitle} should be not selected in table");

            Click(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle));
            Click(TenantsPage.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(TenantsPage.TenantsDialog),
                "Tenants modal dialog should be closed on Cancel click");
            
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Assert.IsTrue(IsElementNotFound(string.Format(TenantsPage.TableRowSelectedByTitle, string.Empty)),
                "All tenants should be not selected in table");

            Click(string.Format(TenantsPage.TableRowByTitle, _distribution1Title));
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle));
            Click(TenantsPage.UploadButton);
            Click(AppsPage.OkButton, ignoreIfNoElement: false, 120);
            ChangeTenant(TenantTitle.Distribution1);
            OpenAppsPage();
            Click(AppsPage.TableRowPlayerApp);
            Assert.IsTrue(IsElementFound(string.Format(AppsPage.Version, TestConfig.PlayerAppVersions[0]))
                          && IsElementFound(string.Format(AppsPage.Version, TestConfig.PlayerAppVersions[1])),
                $"App in tenant {_distribution1Title} should have 2 app versions");
            ChangeTenant(TenantTitle.DistributionAR);
            OpenAppsPage();
            Click(AppsPage.TableRowPlayerApp);
            Assert.IsTrue(IsElementNotFound(string.Format(AppsPage.Version, TestConfig.PlayerAppVersions[0]))
                          && IsElementFound(string.Format(AppsPage.Version, TestConfig.PlayerAppVersions[1])),
                $"App in tenant {_distributionArTitle} should have 1 app version: {TestConfig.PlayerAppVersions[1]}");
            ChangeTenant(TenantTitle.DistributionEN_DE);
            OpenAppsPage();
            Assert.IsTrue(IsElementNotFound(AppsPage.TableRowPlayerApp),
                $"Player app should be absent on tenant {_distributionEnDeTitle}");

            ChangeTenant(TenantTitle.DistributionEN1);
            OpenEntityPage(app);
            Click2(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowByText, TestConfig.PlayerAppVersions[1]));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.MarkAsDeletedButton);
            Click(PageHeader.NavigateBackButton);
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowDisabledByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be disabled in table");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowUnselectedByTitle, _distributionArTitle)),
                $"Tenant {_distributionArTitle} should be present and not selected in table");

            Click(TenantsPage.SelectAllButton);
            Click(TenantsPage.UploadButton);
            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionEN_DE);
            OpenAppsPage();
            Click(AppsPage.TableRowPlayerApp);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable), $@"App Status should be '{StatusAvailable}'");
            Assert.IsTrue(GetElementAttribute(AppsPage.Image, "src") == imgSource, 
                $"App should have is the same image as a source app at tenant {_distributionEn1Title} has");

            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.Version, TestConfig.PlayerAppVersions[0]))
                          && IsElementNotFoundQuickly(string.Format(AppsPage.Version, TestConfig.PlayerAppVersions[1])),
                $"App in tenant {_distributionEnDeTitle} should have 1 app version: {TestConfig.PlayerAppVersions[0]}");
        }

        [Test, Regression]
        public void RT11030_DistributeAppSortingAndVersion()
        {
            CurrentTenant = TenantTitle.DistributionEN_DE;
            AddAppPlayer();
            CurrentTenant = TenantTitle.Distribution1;
            AddAppPlayer();
            var app = AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion);
            var place = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true);
            AssignAppToPlace(place, app, null, null, isAddSilently: true);
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE);
            TestStart();

            OpenEntityPage(app);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusPublished),
                $@"App Status on tenant {_distribution1Title} should be '{StatusPublished}'");
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            Click(string.Format(TenantsPage.TableHeaderByName, TitleColumnName));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TenantRowExpanded, _distribution1Title)),
                $"Tenant {_distribution1Title} should look expanded after sort order change");
            var sample = new []
            {
                _distributionArTitle,
                _distribution1Title,
                _distributionEn1Title,
                _distributionEnDeTitle
            };
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowTitle, sample),
                $"Tenants should be sorted in descending order: " + string.Join(", ", sample));

            Click(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle));
            Click(TenantsPage.UploadButton);
            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionAR);
            OpenAppsPage();
            Click(AppsPage.TableRowComposerVipbApp);
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $@"App Status on tenant {_distributionArTitle} should be '{StatusAvailable}'");

            EditForm();
            Click(AppsPage.ImageUploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            var imgLink = GetElementAttribute(AppsPage.Image, "src");
            SubmitForm();
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            SetFilterModal("EN_DE");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowDisabledByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be shown and disabled on filter active");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle)),
                $"Tenant {_distributionEnDeTitle} should be shown on filter active");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title)),
                $"Tenant {_distributionEn1Title} should be not shown on filter active");

            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle));
            Click(TenantsPage.UploadButton);
            Click(AppsPage.OkButton);
            EditForm();
            SendText(AppsPage.AppTitle, $"Auto test {RandomNumber}", isCheckInput: true);
            SubmitForm();
            ChangeTenant(TenantTitle.DistributionEN_DE);
            OpenAppsPage();
            Click(AppsPage.TableRowComposerVipbApp);
            Assert.IsTrue(GetElementAttribute(AppsPage.Image, "src") == imgLink,
                $"App on tenant {_distributionEnDeTitle} has wrong image");

            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle)),
                $"Tenant {_distributionArTitle} (where app was renamed) should be not shown");

            ClickAtPoint(CommonElement.Backdrop, 10, 300); // click outside dialog
            Assert.IsTrue(IsElementNotFoundQuickly(TenantsPage.TenantsDialog),
                "Tenants dialog should be closed on click outside it");

            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowByText, TestConfig.ComposerVipbAppEarliestVersion));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.MarkAsDeletedButton);
            Click(PageFooter.ShowDeletedButton);
            Click(AppsPage.TableRowComposerVipbApp);
            Assert.IsTrue(IsElementNotFound(PageFooter.DistributeSubMenu),
                $@"Distribute button should be shown in page footer (app Status '{StatusDeleted}')");

            ChangeTenant(TenantTitle.Distribution1);
            // #100177 regression check
            OpenEntityPage(place);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableHeader),
                "No Apps section. Place has no apps assigned anymore.");
            Assert.IsTrue(
                AreElementsContainText(PlacesPage.AppsSectionTableRow1TitleCell, AppTitle.ComposerVipB),
                $"Apps section: app {AppTitle.ComposerVipB} should be assigned to the place");
            //
            OpenEntityPage(app);
            Click(PageFooter.AddVersionButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.ComposerVipbAppFolder,
                TestConfig.ComposerVipbAppFile,
                TestConfig.ComposerVipbAppMiddleVersion);
            FileManager.Upload(pathFile);
            var newTitle = $"Auto test {RandomNumber}";
            SendText(AppsPage.AppTitle, newTitle, isCheckInput: true);
            Click(AppsPage.ImageUploadButton);
            FileManager.Upload(TestConfig.ImageFhdFile);
            SubmitForm();
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            if (IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle), 3))
            {
                ClickUntilShown(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title),
                    string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle));
            }
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle));
            Click(TenantsPage.UploadButton);
            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionEN_DE);
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(string.Format(AppsPage.TableRowByText, newTitle)),
                $@"App Composer_VipB should be shown with new name '{newTitle}' after distribution");
            Click(string.Format(AppsPage.TableRowByText, newTitle));
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $@"App Status on tenant {_distributionEnDeTitle} should be '{StatusAvailable}'");
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitleReadOnly, newTitle),
                $@"App 'App title' on tenant {_distributionEnDeTitle} should be '{newTitle}'");
            Assert.IsTrue(GetElementAttribute(AppsPage.Image, "src") == imgLink,
                $"App on tenant {_distributionEnDeTitle} has wrong image");
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.ComposerVipbAppMiddleVersion),
                $"App Versions on tenant {_distributionEnDeTitle} should contain version " + 
                $"{TestConfig.ComposerVipbAppMiddleVersion}");
            Assert.IsTrue(AreElementsContainText(AppsPage.VersionsInactive, "1 inactive"),
                $@"App Versions on tenant {_distributionEnDeTitle} should contain '1 inactive'");

            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionEN_DE);
            RefreshPage();
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            pathFile = FileManager.GetEarliestFileVersion(TestConfig.ComposerHqApp1Folder,
                TestConfig.ComposerHqApp1File);
            FileManager.Upload(pathFile);
            SubmitForm();
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            Click(TenantsPage.SelectAllButton);
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be not selected (changed access)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distributionEn1Title)),
                $"Tenant {_distributionEn1Title} should be selected (changed access)");

            Click(TenantsPage.UploadButton);
            Click(AppsPage.OkButton);
            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowByText, TestConfig.ComposerHqApp1Version));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            ChangeTenant(TenantTitle.DistributionEN1);
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableRowComposerHqApp1Available),
                $"Tenant {_distributionEn1Title} Apps list does not contain HQ1 app");

            Click(AppsPage.TableRowComposerHqApp1Available);
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            if (IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle)))
            {
                Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            }
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle)),
                $"Tenant {_distributionEnDeTitle} should be shown");

            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionEN_DE,
                TenantTitle.DistributionAR);
            ChangeTenant(TenantTitle.Distribution1);
            OpenAppsPage();
            Assert.IsTrue(IsElementNotFound(AppsPage.TableRowComposerHqApp1Available),
                $"Tenant {_distribution1Title} Apps list should have no app Composer HQ1");
        }

        [Test, Regression]
        public void RT11040_DistributeMdDialogMain()
        {
            CurrentTenant = TenantTitle.DistributionEN1;
            var app = AddAppIbeacon(TestConfig.IbeaconAppVersions[0]);
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE,
                TenantTitle.DistributionDE_EN);
            TestStart();

            OpenEntityPage(app);
            MouseOver(PageFooter.DistributeSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DistributeAppButton),
                "App button in Distribute sub-menu in page footer should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DistributeMasterDataButton),
                "Master Data button in Distribute sub-menu in page footer should be shown");

            Click(PageFooter.DistributeMasterDataButton);
            Assert.IsTrue(IsElementFound(AppsPage.DistributeMdDialog), "Distribute MD dialog should be displayed");

            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.DistributeMdSelectedPartialRows) 
                          && IsElementNotFoundQuickly(AppsPage.DistributeMdUnselectedRows),
                "Distribute MD: there should be no unselected or partially selected rows");
            
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(AppsPage.DistributeMdRowByParentText, "beacon", "Accuracy")),
                "Distribute MD: Accuracy should be a child of Beacon");
            Assert.IsTrue(
                IsElementFoundQuickly(
                    string.Format(AppsPage.DistributeMdRowByParentText, "featureaccessibility", "Service booking")),
                "Distribute MD: Service booking should be a child of Service booking");
            Assert.IsTrue(
                GetElementLocation(string.Format(AppsPage.DistributeMdRowByText, "Beacon")).X
                == GetElementLocation(string.Format(AppsPage.DistributeMdRowByText, "Feature Accessibility")).X,
                "Distribute MD: Beacon and Feature Accessibility should be shown at the same hierarchical level");
            Assert.IsTrue(
                IsElementFoundQuickly(
                    string.Format(AppsPage.DistributeMdRowByParentText, "poi", "Show Video Special")),
                "Distribute MD: Show Video Special should be a child of POI Details");
            Assert.IsTrue(
                GetElementLocation(string.Format(AppsPage.DistributeMdRowByText, "Enable")).X
                == GetElementLocation(string.Format(AppsPage.DistributeMdRowByText, "Show Video Special")).X,
                "Distribute MD: Enable and Show Video Special should be shown at the same hierarchical level");

            Click(AppsPage.NextButton);
            Assert.IsTrue(IsElementFound(AppsPage.NoAppropriateTenantToDistributeDialog),
                @"Error dialog 'no appropriate tenant to distribute to' should be displayed");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.NoAppropriateTenantToDistributeDialog),
                @"Error dialog 'no appropriate tenant to distribute to' should be closed on OK button click");
            Assert.IsTrue(IsElementFound(AppsPage.DistributeMdDialog), 
                "Distribute MD dialog should be opened again");

            Click(AppsPage.DeselectAllButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.DistributeMdSelectedPartialRows)
                          && IsElementNotFoundQuickly(AppsPage.DistributeMdSelectedRows),
                "Distribute MD: there should be no selected or partially selected rows");

            Assert.IsTrue(IsElementFoundQuickly(AppsPage.NextDisabledButton),
                "Distribute MD: Next button should be shown disable (all unselected)");

            Click(string.Format(AppsPage.DistributeMdRowByText, "Hiding delay"));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdSelectedRowByText, "Hiding delay")),
                "Distribute MD: Hiding delay should be selected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdSelectedPartialRowByText, "Beacon")),
                "Distribute MD: Beacon should be partially selected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRowByText, "Showing delay")),
                "Distribute MD: Showing delay should be unselected");

            Assert.IsTrue(IsElementFoundQuickly(AppsPage.NextButton),
                "Distribute MD: Next button should be shown enabled (Hiding delay selected)");

            Click(string.Format(AppsPage.DistributeMdRowByText, "Hiding delay"));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRows, "Beacon")),
                "Distribute MD: Hiding delay should be unselected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRowByText, "Showing delay")),
                "Distribute MD: Showing delay should be unselected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRowByText, "Accuracy")),
                "Distribute MD: Accuracy should be unselected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRowByText, "Beacon")),
                "Distribute MD: Beacon should be unselected");

            Click(string.Format(AppsPage.DistributeMdRowByText, "POI Details"));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdSelectedRows, "POI Details")),
                "Distribute MD: POI Details should be selected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdSelectedRowByText, "Show Video Special")),
                "Distribute MD: Show Video Special should be selected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdSelectedRowByText, "Read More")),
                "Distribute MD: Read More should be selected");

            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(AppsPage.DistributeMdSelectedPartialRowByText, "Feature Accessibility")),
                "Distribute MD: Feature Accessibility should be partially selected");

            Click(string.Format(AppsPage.DistributeMdRowByText, "Feature Accessibility"));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdSelectedRows, "Feature Accessibility")),
                "Distribute MD: Feature Accessibility should be selected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdSelectedRowByText, "POI Details")),
                "Distribute MD: POI Details should be selected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdSelectedRowByText, "Read More")),
                "Distribute MD: Read More should be selected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRowByText, "Market")),
                "Distribute MD: Market should be unselected");

            Click(string.Format(AppsPage.DistributeMdRowByText, "Feature Accessibility"));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRows, "Feature Accessibility")),
                "Distribute MD: Feature Accessibility should be unselected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRowByText, "POI Details")),
                "Distribute MD: POI Details should be unselected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRowByText, "Read More")),
                "Distribute MD: Read More should be unselected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdUnselectedRowByText, "Market")),
                "Distribute MD: Market should be unselected");

            Click(AppsPage.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.DistributeMdDialog), 
                "Distribute MD dialog should be closed");
        }

        [Test, Regression]
        public void RT11050_DistributeMdDialogItems()
        {
            CurrentTenant = TenantTitle.DistributionEN1;
            var app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            AppApi.DeleteAppVersion(app, TestConfig.IbeaconAppVersions[0]);
            var item = ItemApi.SearchItem(ItemTypeWelcomeEmailTemplate);
            Assert.IsNotNull(item,
                $@"Item {ItemTypeWelcomeEmailTemplate} not found. Is iBeacon app imported and available on tenant " +
                $"{CurrentTenantCode}?");
            AddItemToIbeaconApp(app, "$.texts.emails.welcomeEmailTemplate", item);
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE,
                TenantTitle.DistributionDE_EN);
            TestStart();

            OpenEntityPage(app);
            EditForm();
            Click(AppsPage.BeaconButton);
            var newAccuracy = "20";
            var newRecipient = $"a{RandomNumber}@bbb.ccc";
            SendText(AppsPage.Accuracy, newAccuracy);
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            if (IsElementNotFoundQuickly(AppsPage.ServiceBookingRecipientEmail))
            {
                ClickUntilConditionMet(AppsPage.ServiceBookingRecipientAddButton, 
                    () => CountElements(AppsPage.ServiceBookingRecipientEmail) == 1);
            }            
            SendText(AppsPage.ServiceBookingRecipientEmail, newRecipient);
            ClickUntilShown(AppsPage.FeatureAccessibilityButton, AppsPage.CarDetailsButton);
            Click(AppsPage.CarDetailsButton);
            Click(AppsPage.ShareWithFriendsCheckBox);
            SubmitForm();
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            if (IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionDeEnTitle)))
            {
                Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            }
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionDeEnTitle));
            Click(TenantsPage.UploadButton);
            Assert.IsTrue(IsElementFound(AppsPage.CrossLanguageMdValuesOverwrittenDialog),
                @"Dialog 'cross-language master data values to be overwritten' should be shown");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.YesButton),
                @"'Yes' button should be shown in the dialog");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.NoButton),
                @"'No' button should be shown in the dialog");

            Click(AppsPage.YesButton);
            Assert.IsTrue(IsElementFound(AppsPage.DistributeOverwriteFollowingItemsDialog),
                @"Dialog 'Do you want to distribute/overwrite the following items:' should be shown");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.YesButton),
                @"'Yes' button should be shown in the dialog");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.NoButton),
                @"'No' button should be shown in the dialog");

            Click(AppsPage.NoButton);
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionDE_EN);
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableRowIbeaconApp),
                $"iBeacon app should be in Apps list on tenant {_distributionDeEnTitle}");

            Click(AppsPage.TableRowIbeaconApp);
            var appId = GetEntityIdFromUrl();
            Click(AppsPage.BeaconButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{newAccuracy} m"),
                $@"Accuracy field should equal to '{newAccuracy} m'");
            Assert.IsTrue(IsElementNotFound(AppsPage.MarketCodeReadOnly),
                "Market Code field should be not shown unless Market group is opened");
            Click(AppsPage.MarketButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.MarketCodeReadOnly),
                "Market Code field should be shown when Market group is opened");
            var marketCode = GetValue(AppsPage.MarketCodeReadOnly);
            Click(AppsPage.FeatureAccessibilityButton);
            Click(AppsPage.CarDetailsButton);
            Assert.IsTrue(IsCheckBoxOff(AppsPage.ShareWithFriendsCheckBox),
                "Feature Accessibility > Car Details > Share With Friends check box should be Off");

            Click(AppsPage.InformationButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.AddressLine1),
                "Information > Addresses table should contain at least 1 row");

            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Assert.IsTrue(IsElementNotFound(AppsPage.ServiceBookingRecipientEmailReadOnly),
                "Texts > Emails > Service Booking recipient email should be absent");
            Assert.IsTrue(IsElementEquals(AppsPage.WelcomeNewCustomerReadOnly, string.Empty),
                "Texts > Emails > Welcome New Customer should be empty");
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            Assert.IsTrue(IsElementEquals(AppsPage.MessengerInNewCarsReadOnly, string.Empty),
                @"Texts > Emails > Sharing templates for new cars > Messenger field should be empty");

            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageEnButtonActive),
                "Lang bar: EN should be active language");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.LanguageDeButtonActive)
                          && IsElementNotFoundQuickly(AppsPage.LanguageDeButton),
                "Lang bar: DE should be absent");

            OpenItemsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableEmpty),
                "There should be no items in Items list");

            OpenEntityPage<AppResponse>(appId);
            EditForm();
            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsEditMode(), "App should be in edit mode due to validation errors on page");

            Click(AppsPage.TextsButton);
            Click(AppsPage.GoodByeMessageButton);
            const string text = "x";
            SendText(AppsPage.TextInGoodByeMessage, text);
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            SendText(AppsPage.EmailSubjectInNewCars, text);
            SendText(AppsPage.EmailBodyInNewCars, text);
            SendText(AppsPage.TwitterInNewCars, text);
            SendText(AppsPage.MessengerInNewCars, text);
            Click(AppsPage.SharingTemplatesForUsedCarsButton);
            SendText(AppsPage.EmailSubjectInUsedCars, text);
            SendText(AppsPage.EmailBodyInUsedCars, text);
            SendText(AppsPage.TwitterInUsedCars, text);
            SendText(AppsPage.MessengerInUsedCars, text);
            Click(AppsPage.LanguageAddButton);
            Click(AppsPage.LanguageDeButtonActiveInMenu);
            Click(AppsPage.TextsButton);
            Click(AppsPage.GoodByeMessageButton);
            SendText(AppsPage.TextInGoodByeMessage, text);
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            SendText(AppsPage.EmailSubjectInNewCars, text);
            SendText(AppsPage.EmailBodyInNewCars, text);
            SendText(AppsPage.TwitterInNewCars, text);
            SendText(AppsPage.MessengerInNewCars, text);
            Click(AppsPage.SharingTemplatesForUsedCarsButton);
            SendText(AppsPage.EmailSubjectInUsedCars, text);
            SendText(AppsPage.EmailBodyInUsedCars, text);
            SendText(AppsPage.TwitterInUsedCars, text);
            SendText(AppsPage.MessengerInUsedCars, text);
            Click(AppsPage.BeaconButton);
            newAccuracy = "15";
            SendText(AppsPage.Accuracy, newAccuracy);
            Click(AppsPage.InformationButton);
            Click(AppsPage.InformationPictureUploadButton);
            FileManager.Upload(TestConfig.Image285);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), 
                "App should be in view mode on added DE language and Submit press");

            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            if (IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle)))
            {
                Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            }
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle));
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle));
            Click(TenantsPage.UploadButton);
            Click(AppsPage.YesButton); // Do you want cross-language master data values to be overwritten? 
            Click(AppsPage.YesButton); // Do you want to distribute/overwrite the following items: ... ?
            Click(AppsPage.OkButton);  // Upload has been successfully completed
            ChangeTenant(TenantTitle.DistributionEN_DE);
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableRowIbeaconApp),
                $"iBeacon app should be present in Apps list on tenant {CurrentTenantCode}");

            Click(AppsPage.TableRowIbeaconApp);
            Click(AppsPage.BeaconButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{newAccuracy} m"),
                $@"Beacon > Accuracy field should equal to '{newAccuracy} m'");

            Click(AppsPage.InformationButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.InformationPictureEmpty),
                "Information > Picture field should be empty");

            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            Assert.IsTrue(IsElementEquals(AppsPage.MessengerInNewCarsReadOnly, string.Empty),
                @"Lang EN: Texts > Emails > Sharing Templates For New Cars > Messenger should be empty");
            Click(AppsPage.LanguageDeButton);
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            Assert.IsTrue(IsElementEquals(AppsPage.MessengerInNewCarsReadOnly, string.Empty),
                @"Lang DE: Texts > Emails > Sharing Templates For New Cars > Messenger should be empty");

            OpenItemsPage();
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 6,
                $"All 6 items included to iBeacon app should be distributed to tenant {_distributionEnDeTitle}");

            Click(string.Format(ItemsPage.TableRowByText, ItemTypeDailyItemsReport));
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Lang bar: EN should be active language");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButton), "Lang bar: DE should be shown");

            ChangeTenant(TenantTitle.DistributionAR);
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableRowIbeaconApp),
                $"iBeacon app should be in Apps list on tenant {_distributionArTitle}");

            Click(AppsPage.TableRowIbeaconApp);
            Click(AppsPage.MarketButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.MarketCodeReadOnly),
                "Market Code field should be shown when Market group is opened");
            Assert.IsTrue(IsElementEquals(AppsPage.MarketCodeReadOnly, marketCode),
                $@"Market Code field should be equal to '{marketCode}'");
            Click(AppsPage.BeaconButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{newAccuracy} m"),
                $@"Accuracy field should be equal to '{newAccuracy} m'");

            Click(AppsPage.InformationButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.AddressLine1),
                "Information > Addresses table should contain no rows");

            Click(AppsPage.TextsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            Assert.IsTrue(IsElementEquals(AppsPage.MessengerInNewCarsReadOnly, string.Empty),
                "Messenger field should be empty");

            OpenItemsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableEmpty),
                "There should be no items in Items list");

            ChangeTenant(TenantTitle.DistributionEN1);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetEarliestFileVersion(TestConfig.IbeaconAppFolder, TestConfig.IbeaconAppFile);
            FileManager.Upload(pathFile);
            ClickUntilShown(AppsPage.MarketButton, AppsPage.MarketCode);
            SendText(AppsPage.MarketCode, "QQ");
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "App is still in edit mode on Submit press");

            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowByText, TestConfig.IbeaconAppVersions[1]));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Click(PageHeader.NavigateBackButton);
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle));
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionDeEnTitle));
            Click(TenantsPage.UploadButton);
            Click(AppsPage.NoButton); // Do you want cross-language master data values to be overwritten?
            Click(AppsPage.NoButton); // Do you want to distribute/overwrite the following items: ... ?
            Click(AppsPage.OkButton); // Upload has been successfully completed
            ChangeTenant(TenantTitle.DistributionDE_EN);
            OpenAppsPage();
            Click(AppsPage.TableRowIbeaconApp);
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.IbeaconAppVersions[0]),
                $"On tenant {_distributionEnDeTitle} app should contain version " + 
                $"{TestConfig.IbeaconAppVersions[0]}");
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.IbeaconAppVersions[1]),
                $"On tenant {_distributionEnDeTitle} app should contain version " +
                $"{TestConfig.IbeaconAppVersions[1]}");

            Click(AppsPage.MarketButton);
            Assert.IsTrue(IsElementEquals(AppsPage.MarketCodeReadOnly, marketCode),
                $@"Market Code should be equal to '{marketCode}'");
            Click(AppsPage.TextsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            Assert.IsTrue(IsElementEquals(AppsPage.MessengerInNewCarsReadOnly, text),
                $@"Messenger should be equal to '{text}'");

            ChangeTenant(TenantTitle.DistributionEN_DE);
            OpenItemsPage();
            Click(ItemsPage.TableRow);
            Assert.IsTrue(CompareTimestamps(AppsPage.Created, AppsPage.Modified) == Equality.Equal, 
                "Created timestamp should equal to Modified field");
        }

        [Test, Regression]
        public void RT11060_DistributeMdProperties()
        {
            AppApi.DeleteApps(true, new [] { AppTitle.Ibeacon }, TenantTitle.DistributionDE_EN);
            AppApi.DeleteApps(true, new [] { AppTitle.Ibeacon }, TenantTitle.DistributionAR);
            AppApi.DeleteApps(true, new [] { AppTitle.Ibeacon }, TenantTitle.DistributionEN1);
            CurrentTenant = TenantTitle.DistributionDE_EN;
            var appDeEn = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentTenant = TenantTitle.DistributionAR;
            var appAr = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentTenant = TenantTitle.DistributionEN1;
            var appEn1 = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE,
                TenantTitle.DistributionDE_EN);
            TestStart();

            OpenEntityPage(appEn1);
            EditForm();
            var accuracy = "7";
            var currencyFormatting = "Currency First";
            var salutationProfile = "some new salutation";
            Click(AppsPage.BeaconButton);
            SendText(AppsPage.Accuracy, accuracy);
            Click(AppsPage.MarketButton);
            DropDownSelect(AppsPage.CurrencyFormattingDropDown, currencyFormatting);
            Click(AppsPage.TextsButton);
            Click(AppsPage.GoodByeMessageButton);
            Click(AppsPage.EnableInGoodByeMessageCheckBox);
            Click(AppsPage.InformationButton);
            SendText(AppsPage.SalutationProfile, salutationProfile);
            SubmitForm();
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeMasterDataButton);
            Click(AppsPage.NextButton);
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TenantRowCollapsed, _distribution1Title))
                    && IsElementFoundQuickly(string.Format(TenantsPage.TableRowDisabledByTitle, _distribution1Title)),
                $@"Tenant '{_distribution1Title}' should look collapsed and disabled");
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle))
                    && IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowDisabledByTitle, _distributionArTitle)),
                $@"Tenant '{_distributionArTitle}' should be shown and enabled");

            Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionDeEnTitle)),
                $@"Tenant '{_distributionDeEnTitle}' should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle)),
                $@"Tenant '{_distributionEnDeTitle}' should be not shown");

            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, string.Empty)),
                "No tenants should currently be selected");
            Assert.IsTrue(IsElementFoundQuickly(TenantsPage.ApplyDisabledButton),
                "Disabled Apply button should be shown");

            Click(TenantsPage.SelectAllButton);
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distribution1Title)),
                $@"Tenant '{_distribution1Title}' should be not selected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distributionDeEnTitle)),
                $@"Tenant '{_distributionDeEnTitle}' should be selected");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, _distributionArTitle)),
                $@"Tenant '{_distributionArTitle}' should be selected");

            Click(TenantsPage.ApplyButton);
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be closed");

            ChangeTenant(TenantTitle.DistributionAR);
            OpenEntityPage(appAr);
            Click(AppsPage.BeaconButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{accuracy} m"),
                $@"(Tenant {CurrentTenant.ToString()}) Beacon > Accuracy should be '{accuracy} m'");
            Click(AppsPage.MarketButton);
            Assert.IsTrue(IsElementEquals(AppsPage.CurrencyFormattingReadOnly, currencyFormatting),
                $@"(Tenant {CurrentTenant.ToString()}) Market > Currency Formatting should be '{currencyFormatting}'");
            Click(AppsPage.TextsButton);
            Click(AppsPage.GoodByeMessageButton);
            Assert.IsTrue(IsCheckBoxOn(AppsPage.EnableInGoodByeMessageCheckBox),
                $@"(Tenant {CurrentTenant.ToString()}) Text > Goodbye Message > Enable check box should be 'On'");
            Click(AppsPage.InformationButton);
            Assert.IsFalse(IsElementEquals(AppsPage.SalutationNoProfileReadOnly, salutationProfile),
                $@"(Tenant {CurrentTenant.ToString()}) Information > Salutation Profile should be not '{salutationProfile}'");

            ChangeTenant(TenantTitle.DistributionDE_EN);
            OpenEntityPage(appDeEn);
            Click(AppsPage.BeaconButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{accuracy} m"),
                $@"(Tenant {CurrentTenant.ToString()}) Beacon > Accuracy should be '{accuracy} m'");
            Click(AppsPage.MarketButton);
            Assert.IsTrue(IsElementEquals(AppsPage.CurrencyFormattingReadOnly, currencyFormatting),
                $@"(Tenant {CurrentTenant.ToString()}) Market > Currency Formatting should be '{currencyFormatting}'");
            Click(AppsPage.TextsButton);
            Click(AppsPage.GoodByeMessageButton);
            Assert.IsTrue(IsCheckBoxOn(AppsPage.EnableInGoodByeMessageCheckBox),
                $@"(Tenant {CurrentTenant.ToString()}) Text > Goodbye Message > Enable check box should be 'On'");
            Click(AppsPage.InformationButton);
            Assert.IsFalse(IsElementEquals(AppsPage.SalutationNoProfileReadOnly, salutationProfile),
                $@"(Tenant {CurrentTenant.ToString()}) Information > Salutation Profile should be not '{salutationProfile}'");

            EditForm();
            accuracy = "8";
            var showingDelay = "4";
            var marketCode = "XX";
            var defaultCurrency = "UAH";
            SendText(AppsPage.Accuracy, accuracy);
            SendText(AppsPage.ShowingDelay, showingDelay);
            SendText(AppsPage.MarketCode, marketCode);
            SendText(AppsPage.DefaultCurrency, defaultCurrency);
            SendText(AppsPage.TextInGoodByeMessage, "a");
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            SendText(AppsPage.EmailSubjectInNewCars, "a");
            SendText(AppsPage.EmailBodyInNewCars, "a");
            SendText(AppsPage.TwitterInNewCars, "a");
            SendText(AppsPage.MessengerInNewCars, "a");
            Click(AppsPage.SharingTemplatesForUsedCarsButton);
            SendText(AppsPage.EmailSubjectInUsedCars, "a");
            SendText(AppsPage.EmailBodyInUsedCars, "a");
            SendText(AppsPage.TwitterInUsedCars, "a");
            SendText(AppsPage.MessengerInUsedCars, "a");
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "App should be saved");

            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeMasterDataButton);
            Click(string.Format(AppsPage.DistributeMdRowByText, "Accuracy")); // deselect
            Click(string.Format(AppsPage.DistributeMdRowByText, "Market")); // deselect group
            Click(string.Format(AppsPage.DistributeMdRowByText, "Default currency")); // select in Market group
            Click(AppsPage.NextButton);
            if (IsElementNotFound(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title), 2))
            {
                Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            }
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title));
            Click(TenantsPage.ApplyButton);

            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be closed");
            ChangeTenant(TenantTitle.DistributionEN1);
            OpenEntityPage(appEn1);
            Click(AppsPage.BeaconButton);
            Assert.IsTrue(IsElementEquals(AppsPage.ShowingDelayReadOnly, $"{showingDelay} s"),
                $@"(Tenant {CurrentTenant.ToString()}) Beacon > Showing Delay should be '{showingDelay} s'");
            Assert.IsFalse(IsElementEquals(AppsPage.AccuracyReadOnly, $"{accuracy} m"),
                $@"(Tenant {CurrentTenant.ToString()}) Beacon > Accuracy should not be '{accuracy} m'");
            Click(AppsPage.MarketButton);
            Assert.IsTrue(IsElementEquals(AppsPage.DefaultCurrencyReadOnly, defaultCurrency),
                $@"(Tenant {CurrentTenant.ToString()}) Market > Default Currency should be '{defaultCurrency}'");
            Assert.IsFalse(IsElementEquals(AppsPage.MarketCodeReadOnly, marketCode),
                $@"(Tenant {CurrentTenant.ToString()}) Market > Market Code should not be '{marketCode}'");
        }

        [Test, Regression]
        public void RT11070_DistributeAppEventKit()
        {
            CurrentTenant = TenantTitle.DistributionEN1;
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE,
                TenantTitle.DistributionDE_EN);
            TestStart();
            
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(
                TestConfig.EventKitAppFolder, 
                TestConfig.EventKitAppFile,
                TestConfig.EventKitAppVersions[0]);
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsEditMode(), $@"App '{AppTitle.EventKit}' should be in edit mode");
            
            SendText(AppsPage.Pin, "0000");
            ClickUntilShown(AppsPage.TouchpointsButton, AppsPage.Touchpoint01Button);
            Click(AppsPage.Touchpoint01Button);
            var touchPointTitle = $"Auto test {RandomNumber}";
            SendText(AppsPage.Touchpoint01TouchpointTitle, touchPointTitle);
            Click(AppsPage.Touchpoint01TouchpointPreviewImageUploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            ClickUntilShown(AppsPage.Touchpoint01TouchpointContentAddButton, 
                AppsPage.Touchpoint01TouchpointContentRow1ContentDescPositionDropDown);
            var touchpointPreviewImageUrl = GetElementAttribute(AppsPage.Touchpoint01TouchpointPreviewImage, "src");
            Click(AppsPage.Touchpoint01TouchpointContentRow1PreviewImageUploadButton);
            FileManager.Upload(TestConfig.Image25);
            DropDownSelect(AppsPage.Touchpoint01TouchpointContentRow1ContentDescPositionDropDown, "Right");
            var row1PreviewImageUrl = 
                GetElementAttribute(AppsPage.Touchpoint01TouchpointContentRow1PreviewImage, "src");
            Click(AppsPage.Touchpoint01TouchpointContentRow1ContentMediaUploadButton);
            FileManager.Upload(TestConfig.Video1Mp4);
            IsElementFound(AppsPage.Touchpoint01TouchpointContentRow1ContentMedia, 60);
            var row1ContentMediaUrl =
                GetElementAttribute(AppsPage.Touchpoint01TouchpointContentRow1ContentMedia, "src");
            SubmitForm();
            Assert.IsTrue(IsViewMode(), $@"App '{AppTitle.EventKit}' should be saved");

            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle));
            Click(TenantsPage.UploadButton);
            Click(AppsPage.YesButton);
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionAR);
            OpenAppsPage();
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.TableRowByText, AppTitle.EventKit)),
                $@"App '{AppTitle.EventKit}' should be distributed to tenant {_distributionArTitle}");

            Click(string.Format(AppsPage.TableRowByText, AppTitle.EventKit));
            EditForm();
            ClickUntilShown(AppsPage.TouchpointsButton, AppsPage.Touchpoint01Button);
            Click(AppsPage.Touchpoint01Button);
            Assert.IsTrue(IsElementEquals(AppsPage.Touchpoint01TouchpointTitle, touchPointTitle),
                $@"Touchpoints > Touchpoint 01 > Touchpoint Title should be '{touchPointTitle}' " +
                $"after distribution to tenant {_distributionArTitle}");

            Assert.IsTrue(GetElementAttribute(AppsPage.Touchpoint01TouchpointPreviewImage, "src") 
                          == touchpointPreviewImageUrl,
                "Touchpoints > Touchpoint 01 > Touchpoint Preview Image should be distributed " + 
                $"to tenant {_distributionArTitle}");

            Click(AppsPage.Touchpoint01TouchpointPreviewImage);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library should be opened on Touchpoint Preview Image field click");
            var imageCarFileName = Path.GetFileName(TestConfig.ImageCar);
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowSelectedByText, imageCarFileName)),
                $"Media library should have image {imageCarFileName} selected");

            Click(AppsPage.CancelButton);
            ClickUntilShown(AppsPage.Touchpoint01TouchpointContentRow1, 
                AppsPage.Touchpoint01TouchpointContentRow1PreviewImage);
            Assert.IsTrue(GetElementAttribute(AppsPage.Touchpoint01TouchpointContentRow1PreviewImage, "src")
                          == row1PreviewImageUrl,
                "Touchpoints > Touchpoint 01 > Touchpoint Content > Row 1, Content Preview Image should be " + 
                $"distributed to tenant {_distributionArTitle}");
            
            Click(AppsPage.Touchpoint01TouchpointContentRow1PreviewImage);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library should be opened on row 1, Content Preview Image field click");
            var image25FileName = Path.GetFileName(TestConfig.Image25);
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowSelectedByText, image25FileName)),
                $"Media library should have image {image25FileName} selected");

            Click(AppsPage.CancelButton);
            Assert.IsTrue(
                IsElementEquals(AppsPage.Touchpoint01TouchpointContentRow1ContentDescPositionDropDown, "Right"),
                "Touchpoints > Touchpoint 01 > Touchpoint Content > Row 1, Content Desc Position should be " +
                $"distributed to tenant {_distributionArTitle}");

            Assert.IsTrue(GetElementAttribute(AppsPage.Touchpoint01TouchpointContentRow1ContentMedia, "src")
                          == row1ContentMediaUrl,
                "Touchpoints > Touchpoint 01 > Touchpoint Content > Row 1, Content Media should be " +
                $"distributed to tenant {_distributionArTitle}");

            Click(AppsPage.Touchpoint01TouchpointContentRow1ContentMedia);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library should be opened on row 1, Content Media field click");
            var video1Mp4FileName = Path.GetFileName(TestConfig.Video1Mp4);
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowSelectedByText, video1Mp4FileName)),
                $"Media library should have image {video1Mp4FileName} selected");
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowByText, image25FileName)),
                $"Media library should have image {image25FileName} for field Content Media");
            Click(AppsPage.CancelButton);
        }

        [Test, Regression]
        public void RT11080_DistributeMdEventKit()
        {
            AppApi.DeleteApps(true, new [] {AppTitle.EventKit});
            CurrentTenant = TenantTitle.DistributionAR;
            var appAr = AddAppEventKit(TestConfig.EventKitAppVersions[0]);
            TestStart();
            OpenEntityPage(appAr);
            EditForm();
            ClickUntilShown(AppsPage.TouchpointsButton, AppsPage.Touchpoint01Button);
            SendText(AppsPage.Pin, "0000");
            Click(AppsPage.Touchpoint01Button);
            Click(AppsPage.Touchpoint01TouchpointPreviewImageUploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            ClickUntilShown(AppsPage.Touchpoint01TouchpointContentAddButton,
                AppsPage.Touchpoint01TouchpointContentRow1ContentDescPositionDropDown);
            var touchpointPreviewImageUrlAr = GetElementAttribute(AppsPage.Touchpoint01TouchpointPreviewImage, "src");
            Click(AppsPage.Touchpoint01TouchpointContentRow1PreviewImageUploadButton);
            FileManager.Upload(TestConfig.Image25);
            Click(AppsPage.Touchpoint01TouchpointContentRow1ContentMediaUploadButton);
            FileManager.Upload(TestConfig.Video1Mp4);
            SubmitForm();
            ChangeTenant(TenantTitle.DistributionEN1);
            var appEn = AddAppEventKit(TestConfig.EventKitAppVersions[0]);
            OpenEntityPage(appEn);
            EditForm();
            ClickUntilShown(AppsPage.TouchpointsButton, AppsPage.Touchpoint01Button);
            SendText(AppsPage.Pin, "0000");
            Click(AppsPage.Touchpoint01Button);
            Click(AppsPage.Touchpoint01TouchpointPreviewImageUploadButton);
            FileManager.Upload(TestConfig.Image025);
            ClickUntilShown(AppsPage.Touchpoint01TouchpointContentAddButton,
                AppsPage.Touchpoint01TouchpointContentRow1ContentDescPositionDropDown);
            var touchpointPreviewImageUrlEn = GetElementAttribute(AppsPage.Touchpoint01TouchpointPreviewImage, "src");
            Click(AppsPage.Touchpoint01TouchpointContentRow1PreviewImageUploadButton);
            FileManager.Upload(TestConfig.Image138);
            var row1PreviewImageUrlEn =
                GetElementAttribute(AppsPage.Touchpoint01TouchpointContentRow1PreviewImage, "src");
            Click(AppsPage.Touchpoint01TouchpointContentRow1ContentMediaUploadButton);
            FileManager.Upload(TestConfig.Video2Mp4);
            IsElementFound(AppsPage.Touchpoint01TouchpointContentRow1ContentMedia, 60);
            var row1ContentMediaUrlEn =
                GetElementAttribute(AppsPage.Touchpoint01TouchpointContentRow1ContentMedia, "src");
            SubmitForm();
            CurrentUser = TestConfig.DistributionUser;
            CurrentTenant = TenantTitle.DistributionEN1;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE,
                TenantTitle.DistributionDE_EN);

            TestStart();
            OpenEntityPage(appEn);
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeMasterDataButton);
            Click(string.Format(AppsPage.DistributeMdRowByParentText, "tp1.2", "Touchpoint preview image"));
            Click(AppsPage.NextButton);
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle));
            Click(TenantsPage.ApplyButton);
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionAR);
            OpenEntityPage(appAr);
            ClickUntilShown(AppsPage.TouchpointsButton, AppsPage.Touchpoint01Button);
            Click(AppsPage.Touchpoint01Button);
            Assert.IsTrue(GetElementAttribute(AppsPage.Touchpoint01TouchpointPreviewImage, "src")
                          == touchpointPreviewImageUrlAr,
                "Touchpoints > Touchpoint 01 > Touchpoint Preview Image should be not distributed " +
                $"to tenant {_distributionArTitle}");

            Click(AppsPage.Touchpoint01TouchpointContentRow1);
            Assert.IsTrue(GetElementAttribute(AppsPage.Touchpoint01TouchpointContentRow1PreviewImage, "src")
                          == row1PreviewImageUrlEn,
                "Touchpoints > Touchpoint 01 > Touchpoint Content > Row 1, Content Preview Image should be " +
                $"distributed to tenant {_distributionArTitle}");

            Assert.IsTrue(GetElementAttribute(AppsPage.Touchpoint01TouchpointContentRow1ContentMedia, "src")
                          == row1ContentMediaUrlEn,
                "Touchpoints > Touchpoint 01 > Touchpoint Content > Row 1, Content Media should be " +
                $"distributed to tenant {_distributionArTitle}");

            EditForm();
            Click(AppsPage.Touchpoint02Button);
            Click(AppsPage.Touchpoint02TouchpointPreviewImageUploadButton);
            FileManager.Upload(TestConfig.Image025);
            var image025FileName = Path.GetFileName(TestConfig.Image025);
            var touchpoint2PreviewImageUrlEn = GetElementAttribute(AppsPage.Touchpoint02TouchpointPreviewImage, "src");
            SubmitForm();
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeMasterDataButton);
            Click(string.Format(AppsPage.DistributeMdRowByText, "Touchpoint 01"));
            Click(AppsPage.NextButton);
            if (IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title)))
            {
                Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            }
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title));
            Click(TenantsPage.ApplyButton);
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionEN1);
            OpenEntityPage(appEn);
            ClickUntilShown(AppsPage.TouchpointsButton, AppsPage.Touchpoint01Button);
            Click(AppsPage.Touchpoint01Button);
            Assert.IsTrue(GetElementAttribute(AppsPage.Touchpoint01TouchpointPreviewImage, "src")
                          == touchpointPreviewImageUrlEn,
                "Touchpoints > Touchpoint 01 > Touchpoint Preview Image should be not distributed " +
                $"to tenant {_distributionEn1Title}");

            Click(AppsPage.Touchpoint02Button);
            Assert.IsTrue(GetElementAttribute(AppsPage.Touchpoint02TouchpointPreviewImage, "src")
                          == touchpoint2PreviewImageUrlEn,
                "Touchpoints > Touchpoint 02 > Touchpoint Preview Image should be distributed " +
                $"to tenant {_distributionEn1Title}");

            EditForm();
            Click(AppsPage.Touchpoint02TouchpointPreviewImage);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library should be opened on row 1, Content Preview Image field click");
            Assert.IsTrue(CountElements(string.Format(MediaLibrary.TableRowByText, image025FileName)) == 1,
                $"Only 1 instance of asset {image025FileName} should be shown in Media Library");

            Click(PageFooter.CancelButton);
            Click(PageFooter.CancelButton);
        }

        [Test, Regression]
        public void RT11090_DistributeAppIpad()
        {
            AppApi.DeleteApps(true, new [] {AppTitle.IpadPlayer});
            CurrentTenant = TenantTitle.DistributionAR;
            AddAppIpadPlayer();
            CurrentTenant = TenantTitle.DistributionEN1;
            AddAppIpadPlayer();
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE,
                TenantTitle.DistributionDE_EN);
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.DptAppFolderRt11090,
                TestConfig.DptAppFileRt11090, TestConfig.DptAppVersionsRt11090[0]);
            FileManager.UploadAsBackgroundTask(pathFile);
            Assert.IsTrue(IsElementFound(AppsPage.AppTitle),
                $@"App '{AppTitle.DptRt11090}' has not been imported");
            var allPermissions = UserDirectoryApi.GetSupportedPermissions();
            var role = UserDirectoryApi.GetRole((long) UserRole.CxmAdmin);
            UserDirectoryApi.SetRolePermissions(role, allPermissions);
            RefreshPage();
            const string market = "AAA";
            SendText(AppsPage.Market, market);
            var pin = GetValue(AppsPage.Pin);
            var stateResetTimer = GetValue(AppsPage.StateResetTimer);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), 
                $"App {AppTitle.DptRt11090} v.{TestConfig.DptAppVersionsRt11090[0]} should be saved");

            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeMasterDataButton);
            Assert.IsTrue(IsElementFound(AppsPage.NoDistributablePropertiesDialog),
                @"Dialog 'There are no distributable properties' should be displayed");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.NoDistributablePropertiesDialog),
                @"Dialog 'There are no distributable properties' should be closed");
            Assert.IsTrue(IsViewMode(),
                $"App {AppTitle.DptRt11090} page should be shown after failed MD distribution");

            Click(PageFooter.AddVersionButton);
            pathFile = FileManager.GetFileByVersion(TestConfig.DptAppFolderRt11090,
                TestConfig.DptAppFileRt11090, TestConfig.DptAppVersionsRt11090[1]);
            FileManager.UploadAsBackgroundTask(pathFile);
            SubmitForm();
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeMasterDataButton);
            Assert.IsTrue(IsElementFound(AppsPage.DistributeMdDialog),
                $"Distribute MD dialog should be displayed for app v.{TestConfig.DptAppVersionsRt11090[1]}");
            Assert.IsTrue(CountElements(string.Format(AppsPage.DistributeMdRowByText, string.Empty)) == 2,
                "In MD dialog should be 2 properties: PIN, State Reset Timer");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdRowByText, "PIN")),
                "In MD dialog should be shown PIN property");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdRowByText, "State reset timer")),
                @"In MD dialog should be shown 'State reset timer' property");

            Click(AppsPage.CancelButton);
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle));
            Click(TenantsPage.UploadButton);
            Assert.IsTrue(IsElementFound(AppsPage.CrossLanguageMdValuesOverwrittenDialog),
                @"Dialog 'Do you want cross-language master data values to be overwritten?' should be displayed");
            Click(AppsPage.YesButton);
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed on app " + 
                $"v.{TestConfig.DptAppVersionsRt11090[1]} distribution");

            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionAR);
            OpenAppsPage();
            Click(string.Format(AppsPage.TableRowByText, AppTitle.DptRt11090));
            ScrollBottom();
            Assert.IsTrue(IsElementEquals(AppsPage.PinReadOnly, pin),
                $@"Field PIN should be '{pin}'");
            Assert.IsTrue(IsElementEquals(AppsPage.StateResetTimerReadOnly, stateResetTimer),
                $@"Field State Reset Timer should be '{stateResetTimer}'");
            Assert.IsTrue(IsElementEquals(AppsPage.MarketReadOnly, string.Empty),
                "Field Market should be empty (not distributed)");

            Click(PageFooter.AddVersionButton);
            pathFile = FileManager.GetFileByVersion(TestConfig.DptAppFolderRt11090,
                TestConfig.DptAppFileRt11090, TestConfig.DptAppVersionsRt11090[2]);
            FileManager.UploadAsBackgroundTask(pathFile);
            const string pin2 = "7777";
            SendText(AppsPage.Market, "BBB");
            SendText(AppsPage.Pin, pin2);
            SendText(AppsPage.StateResetTimer, "777");
            SubmitForm();
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeMasterDataButton);
            Assert.IsTrue(IsElementFound(AppsPage.DistributeMdDialog),
                $"Distribute MD dialog should be displayed for app v.{TestConfig.DptAppVersionsRt11090[2]}");
            Assert.IsTrue(CountElements(string.Format(AppsPage.DistributeMdRowByText, string.Empty)) == 1,
                "In MD dialog should be 2 properties: Pin, State Reset Timer");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.DistributeMdRowByText, "PIN")),
                "In MD dialog should be shown PIN property");

            Click(AppsPage.NextButton);
            Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title));
            Click(TenantsPage.ApplyButton);
            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionEN1);
            OpenAppsPage();
            Click(string.Format(AppsPage.TableRowByText, AppTitle.DptRt11090));
            ScrollBottom();
            Assert.IsTrue(IsElementEquals(AppsPage.PinReadOnly, pin2),
                $@"Field PIN should be '{pin2}'");
            Assert.IsTrue(IsElementEquals(AppsPage.StateResetTimerReadOnly, stateResetTimer),
                $@"Field State Reset Timer should be '{stateResetTimer}' (not distributed)");
            Assert.IsTrue(IsElementEquals(AppsPage.MarketReadOnly, market),
                $@"Field Market should be '{market}' (not distributed)");

            Click(string.Format(AppsPage.Version, "."));
            Click(string.Format(AppsPage.TableRowByText, TestConfig.DptAppVersionsRt11090[1]));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.MarkAsDeletedButton);
            Click(PageHeader.NavigateBackButton);
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeMasterDataButton);
            Assert.IsTrue(IsElementFound(AppsPage.NoDistributablePropertiesDialog),
                @"Dialog 'There are no distributable properties in this app' should be displayed");

            Click(AppsPage.OkButton);
        }

        [Test, Regression]
        public void RT11100_DistributeAssetsImage()
        {
            CurrentTenant = TenantTitle.DistributionAR;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentTenant = TenantTitle.DistributionEN_DE;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentTenant = TenantTitle.DistributionEN1;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE);
            TestStart();

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            ClickUntilShown(ItemsPage.PicturesButton, ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionPictureEmpty);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library dialog should be opened");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.ShareSubMenu),
                "Share sub-menu should not be shown on page footer");

            Click(PageFooter.UploadButton);
            FileManager.Upload(TestConfig.ImageUnique);
            Assert.IsTrue(IsElementFound(PageFooter.ShareSubMenu),
                "Share sub-menu should be shown on page footer");

            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.DistributeButton);
            Assert.IsTrue(IsElementFound(TenantsPage.TenantsDialog),
                "Tenants dialog should appear on press Share > Distribute in Media library");

            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowSelectedByTitle, string.Empty)),
                "No tenants should be shown and selected in Tenants dialog");
            Assert.IsTrue(IsElementFoundQuickly(TenantsPage.UploadDisabledButton),
                "Upload button on Tenants dialog should be shown and disabled");

            Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            Assert.IsTrue(CountElements(TenantsPage.TableRowTitle) == 3,
                $"Tenants which should be in dialog: {_distribution1Title}, {_distributionEnDeTitle}, {_distributionArTitle}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowDisabledByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be present and disabled in Tenants dialog");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle)),
                $"Tenant {_distributionEnDeTitle} should be present in Tenants dialog");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle)),
                $"Tenant {_distributionArTitle} should be present in Tenants dialog");

            Click(string.Format(TenantsPage.TableRowByTitle, _distributionEnDeTitle));
            Click(TenantsPage.UploadButton);
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(ItemsPage.OkButton);
            var fileName = Path.GetFileName(TestConfig.ImageUnique);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName)),
                $"Image {fileName} should be shown and selected in Media Library");

            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.DistributeButton);
            Assert.IsTrue(CountElements(TenantsPage.TableRowTitle) == 1,
                $"Only 1 tenant should be in dialog: {_distributionArTitle}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle)),
                $"Tenant {_distributionArTitle} should be present in Tenants dialog");

            Click(TenantsPage.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(TenantsPage.TenantsDialog),
                "Tenants list dialog should be closed");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName)),
                $"Image {fileName} should be shown and selected in Media Library");
            Click(PageFooter.CancelButton);
            Click(PageFooter.CancelButton);

            //ChangeTenant(TenantTitle.DistributionEN1);
            //OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            ClickUntilShown(ItemsPage.PicturesButton, ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionPictureEmpty);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library dialog should be opened");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, fileName)),
                $"Image {fileName} should be shown in Media Library");

            Click(string.Format(MediaLibrary.TableRowByText, fileName));
            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.DistributeButton);
            Assert.IsTrue(CountElements(TenantsPage.TableRowTitle) == 1,
                $"Only 1 tenant should be in dialog: {_distributionArTitle}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle)),
                $"Tenant {_distributionArTitle} should be present in Tenants dialog");

            Click(string.Format(TenantsPage.TableRowByTitle, _distributionArTitle));
            Click(TenantsPage.UploadButton);
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(ItemsPage.OkButton);
            Click(PageFooter.CancelButton);
            Click(PageFooter.CancelButton);
            ChangeTenant(TenantTitle.DistributionAR);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            ClickUntilShown(ItemsPage.PicturesButton, ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionPictureEmpty);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library dialog should be opened");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, fileName)),
                $"Image {fileName} should be shown in Media Library");

            Click(string.Format(MediaLibrary.TableRowByText, fileName));
            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.DistributeButton);
            Assert.IsTrue(IsElementFound(ItemsPage.NoAppropriateTenantToDistributeDialog),
                "Dialog 'There is no appropriate tenant to distribute to' should be opened");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.OkButton),
                "Dialog 'There is no appropriate tenant to distribute to' should contain OK button");

            Click(ItemsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.NoAppropriateTenantToDistributeDialog),
                "Dialog 'There is no appropriate tenant to distribute to' should be opened");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName)),
                $"Image {fileName} should be shown and selected in Media Library");

            var downloadedImage = Path.Combine(TestConfig.BrowserDownloadFolder, fileName);
            FileManager.Delete(downloadedImage);
            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.DownloadButton);
            Assert.IsTrue(FileManager.IsFileExist(downloadedImage),
                $"Image file {downloadedImage} has not been downloaded to {TestConfig.BrowserDownloadFolder}");
            CloseDownloadPanel();

            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.CopyUrlButton);
            var imageUrl = GetClipboardContent();
            Assert.IsFalse(string.IsNullOrEmpty(imageUrl),
                "Image URL has not been copied to Windows clipboard on Share > Copy URL button click");
            var newTabHandle = OpenNewTab();
            NavigateTo(imageUrl);
            Assert.IsTrue(IsPageRedirectedTo(imageUrl), $"User is not redirected to {imageUrl}");
            CloseTab(newTabHandle);
        }

        [Test, Regression]
        public void RT11110_DistributeAssetsPdf()
        {
            CurrentTenant = TenantTitle.DistributionDE_EN;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentTenant = TenantTitle.DistributionEN_DE;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentTenant = TenantTitle.DistributionEN1;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionDE_EN,
                TenantTitle.DistributionEN_DE);
            TestStart(); 

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePdfCar);
            Click(ItemsPage.PdfEmpty);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library dialog should be opened");
            Click(PageFooter.UploadButton);
            FileManager.Upload(TestConfig.Pdf1Distribute);
            var filePdf1 = Path.GetFileName(TestConfig.Pdf1Distribute);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, filePdf1)),
                $"Image {filePdf1} should be shown in Media Library");
            Click(PageFooter.CancelButton);
            Click(PageFooter.CancelButton);

            ChangeTenant(TenantTitle.DistributionEN_DE);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePdfCar);
            Click(ItemsPage.PdfEmpty);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library dialog should be opened");
            Click(PageFooter.UploadButton);
            FileManager.Upload(TestConfig.Pdf1Distribute);
            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.DistributeButton);
            Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            Assert.IsTrue(CountElements(TenantsPage.TableRowTitle) == 2,
                $"Tenants which should be in dialog: {_distribution1Title}, {_distributionDeEnTitle}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowDisabledByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be present and disabled in Tenants dialog");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionDeEnTitle)),
                $"Tenant {_distributionDeEnTitle} should be present in Tenants dialog");

            Click(TenantsPage.CancelButton);
            Click(PageFooter.UploadButton);
            FileManager.Upload(TestConfig.Pdf2Distribute);
            var filePdf2 = Path.GetFileName(TestConfig.Pdf2Distribute);
            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.DistributeButton);
            if (IsElementFoundQuickly(string.Format(TenantsPage.TenantRowCollapsed, _distribution1Title)))
            {
                Click(string.Format(TenantsPage.CollapseExpandButton, _distribution1Title));
            }
            Assert.IsTrue(CountElements(TenantsPage.TableRowTitle) == 3,
                $"Tenants which should be in dialog: {_distribution1Title}, {_distributionDeEnTitle}, {_distributionEn1Title}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowDisabledByTitle, _distribution1Title)),
                $"Tenant {_distribution1Title} should be present and disabled in Tenants dialog");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionDeEnTitle)),
                $"Tenant {_distributionDeEnTitle} should be present in Tenants dialog");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _distributionEn1Title)),
                $"Tenant {_distributionEn1Title} should be present in Tenants dialog");

            Click(TenantsPage.SelectAllButton);
            Click(TenantsPage.UploadButton);
            Assert.IsTrue(IsElementFound(AppsPage.UploadHasBeenSuccessfullyCompletedDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");
            Click(ItemsPage.OkButton);
            Click(PageFooter.CancelButton);
            Click(PageFooter.CancelButton);

            ChangeTenant(TenantTitle.DistributionEN1);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePdfCar);
            Click(ItemsPage.PdfEmpty);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, filePdf2)),
                $"Image {filePdf2} should be shown in Media Library (tenant {CurrentTenant.ToString()})");
            Click(PageFooter.CancelButton);
            Click(PageFooter.CancelButton);

            ChangeTenant(TenantTitle.DistributionDE_EN);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePdfCar);
            Click(ItemsPage.PdfEmpty);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, filePdf2)),
                $"Image {filePdf2} should be shown in Media Library (tenant {CurrentTenant.ToString()})");

            var filePdf2Size = new FileInfo(TestConfig.Pdf2Distribute).Length.ToString();
            var filePdf2SizeInTable = GetValue(string.Format(MediaLibrary.TableRowSizeByTitle, filePdf2));
            // remove delimiters in number ternaries
            filePdf2SizeInTable = string.Join(string.Empty,
                filePdf2SizeInTable.Trim().Split(new [] { " ", ".", "," }, StringSplitOptions.RemoveEmptyEntries));
            Assert.IsTrue(filePdf2SizeInTable == filePdf2Size, 
                $"File {filePdf2} should have Size = {filePdf2Size}");

            Click(PageFooter.CancelButton);
            Click(PageFooter.CancelButton);
            Click(string.Format(ItemsPage.TableRowByTitle, ItemTypeDailyItemsReport));
            EditForm();
            Click(ItemsPage.AssetsSectionImage);
            Assert.IsTrue(IsElementNotFound(string.Format(MediaLibrary.TableRowSelectedByText, filePdf1)),
                $"Media Library should have no {filePdf1} selected");
            Assert.IsTrue(IsElementNotFound(string.Format(MediaLibrary.TableRowSelectedByText, filePdf2)),
                $"Media Library should have no {filePdf2} selected");

            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.DistributeButton);
            Assert.IsTrue(IsElementFound(ItemsPage.NoAppropriateTenantToDistributeDialog),
                @"Error 'There is no appropriate tenant to distribute to' should be displayed");
            Click(ItemsPage.OkButton);
            Click(PageFooter.CancelButton);
            Click(PageFooter.CancelButton);
        }

        [Test, Regression]
        public void RT11120_DistributeMultipleAssets()
        {
            CurrentUser = TestConfig.DistributionUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Distribution1,
                TenantTitle.DistributionEN1,
                TenantTitle.DistributionAR,
                TenantTitle.DistributionEN_DE,
                TenantTitle.DistributionDE_EN);
            CurrentTenant = TenantTitle.Distribution1;
            TestStart();
            OpenMediaPage();
            
            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.Asset1, TestConfig.Asset2);
            var asset1 = Path.GetFileName(TestConfig.Asset1);
            var asset2 = Path.GetFileName(TestConfig.Asset2);
            ClickUntilConditionMet(string.Format(MediaLibrary.TableRowByText, asset1),
                () => IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, asset1)));
            ClickUntilConditionMet(string.Format(MediaLibrary.TableRowByText, asset2),
                () => IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, asset2)));

            MouseOver(MediaLibrary.ShareSubMenu);
            Click(MediaLibrary.DistributeButton);
            Assert.IsTrue(IsElementFound(
                        string.Format(TenantsPage.TableRowDisabledByTitle, TenantTitle.Distribution1))
                    && IsElementFound(
                        string.Format(TenantsPage.TenantRowCollapsed, TenantTitle.Distribution1)),
                $"Tenant row {TenantTitle.Distribution1} should be shown, collapsed, and disabled");
            Assert.IsTrue(IsElementFound(
                    string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionAR)),
                $"Tenant row {TenantTitle.DistributionAR} should be shown");

            Click(string.Format(TenantsPage.CollapseExpandButton, TenantTitle.Distribution1));
            Click(string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionDE_EN));
            Click(string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionAR));
            Click(TenantsPage.UploadButton);
            Assert.IsTrue(IsElementFoundQuickly(TenantsPage.UploadSuccessfulDialog),
                @"Dialog 'Upload has been successfully completed' should be displayed");

            Click(TenantsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(TenantsPage.UploadSuccessfulDialog),
                @"Dialog 'Upload has been successfully completed' should be closed");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, asset1))
                && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, asset2)),
                "Both uploaded assets should stay selected");

            MouseOver(MediaLibrary.ShareSubMenu);
            Click(MediaLibrary.DistributeButton);
            Assert.IsTrue(IsElementFound(
                    string.Format(TenantsPage.TableRowDisabledByTitle, TenantTitle.Distribution1)),
                $"Tenant row {TenantTitle.Distribution1} should be shown, collapsed, and disabled");
            Assert.IsTrue(IsElementFound(
                    string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionEN1)),
                $"Tenant row {TenantTitle.DistributionEN1} should be shown");
            Assert.IsTrue(IsElementFound(
                    string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionEN_DE)),
                $"Tenant row {TenantTitle.DistributionEN_DE} should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionDE_EN)),
                $"Tenant row {TenantTitle.DistributionDE_EN} should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionAR)),
                $"Tenant row {TenantTitle.DistributionAR} should be shown");

            Click(TenantsPage.CancelButton);
            ChangeTenant(TenantTitle.DistributionDE_EN);
            OpenMediaPage();
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, asset1))
                    && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, asset2)),
                $"Both uploaded assets should be present on tenant {TenantTitle.DistributionDE_EN}");

            ChangeTenant(TenantTitle.DistributionAR);
            OpenMediaPage();
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, asset1))
                    && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, asset2)),
                $"Both uploaded assets should be present on tenant {TenantTitle.DistributionAR}");

            Click(string.Format(MediaLibrary.TableRowByText, asset1));
            Click(string.Format(MediaLibrary.TableRowByText, asset2));
            MouseOver(MediaLibrary.ShareSubMenu);
            Click(MediaLibrary.DistributeButton);
            Assert.IsTrue(IsElementFound(
                        string.Format(TenantsPage.TableRowDisabledByTitle, TenantTitle.Distribution1))
                    && IsElementFound(
                        string.Format(TenantsPage.TenantRowCollapsed, TenantTitle.Distribution1)),
                $"Tenant row {TenantTitle.Distribution1} should be shown, collapsed, and disabled");
            
            Click(string.Format(TenantsPage.CollapseExpandButton, TenantTitle.Distribution1));
            Assert.IsTrue(IsElementFound(
                    string.Format(TenantsPage.TableRowDisabledByTitle, TenantTitle.Distribution1)),
                $"Tenant row {TenantTitle.Distribution1} should be shown, expanded, and disabled");
            Assert.IsTrue(IsElementFound(
                    string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionEN1)),
                $"Tenant row {TenantTitle.DistributionEN1} should be shown");
            Assert.IsTrue(IsElementFound(
                    string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionEN_DE)),
                $"Tenant row {TenantTitle.DistributionEN_DE} should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionDE_EN)),
                $"Tenant row {TenantTitle.DistributionDE_EN} should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionAR)),
                $"Tenant row {TenantTitle.DistributionAR} should be shown");

            Click(string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionEN1));
            Click(string.Format(TenantsPage.TableRowByTitle, TenantTitle.DistributionEN_DE));
            Click(TenantsPage.UploadButton);
            Click(TenantsPage.OkButton);
            MouseOver(MediaLibrary.ShareSubMenu);
            Click(MediaLibrary.DistributeButton);
            Assert.IsTrue(IsElementNotFound(AppsPage.NoAppropriateTenantToDistributeDialog),
                @"Error 'There is no appropriate tenant to distribute' should be displayed");

            Click(TenantsPage.OkButton);
            ChangeTenant(TenantTitle.DistributionEN1);
            OpenMediaPage();
            Assert.IsTrue(
                IsElementFound(string.Format(MediaLibrary.TableRowByText, asset1))
                    && IsElementFound(string.Format(MediaLibrary.TableRowByText, asset2)),
                $"Both uploaded assets should be present on tenant {TenantTitle.DistributionEN1}");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestEnd().ConfigureAwait(false);
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
                ItemApi.DeleteItems();
            }
        }
    }
}
