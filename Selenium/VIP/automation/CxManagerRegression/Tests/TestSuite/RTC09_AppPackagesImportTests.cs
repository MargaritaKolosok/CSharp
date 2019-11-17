using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.UserDirectory;
using Models.Users;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC09_AppPackagesImportTests : ParentTest
    {
        private const string AppName = "testmodel1";
        private const string ItemTypeTestModel1 = "Test Model 1";
        private Role _role;

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }

            CurrentTenant = TenantTitle.import;
            CurrentUser = TestConfig.AdminUser;
        }

        [Test, Regression]
        public void RT09010_Models()
        {
            var newUser = new User
            {
                Email = $"Auto{RandomNumber}@ameria.de",
                Password = CurrentUser.Password
            };
            Parallel.Invoke(
                () => AccountApi.CreateNewAccount(newUser),
                () => _role = UserDirectoryApi.SetRolePermissions(
                    _role,
                    "Login",
                    "GetAnyLocation",
                    "ImportApp",
                    "GetAnyApp",
                    "ModifyOwnApp",
                    "ModifyAnyApp",
                    "ImportAppVersion",
                    "DeleteOwnAppVersion",
                    "DeleteAnyAppVersion",
                    "ChangePorscheCar",
                    "ModifyPorschePdfCar",
                    "ModifyPorscheUsedCar",
                    "ViewPorscheCar",
                    "ViewPorschePdfCar",
                    "ViewPorscheUsedCar"),
                () => AddAppDpt(AppStatus.Any, TestConfig.DptAppVersions[0])
            );
            UserDirectoryApi.SetUserStatus(newUser, UserStatus.Active);
            UserDirectoryApi.AddRoleToUser(newUser, _role, TenantTitle.import);
            CurrentUser = newUser;
            TestStart(isSelectTenant: false);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            ClickUntilShown(ItemsPage.TypeDropDown, CommonElement.DropDownOptionList);
            var availableTypes = new List<string>
            {
                ItemTypeCars, ItemTypePorscheCar, ItemTypeUsedCar, ItemTypePdfCar
            };
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), availableTypes),
                "Only following item types should be available: " + string.Join(", ", availableTypes));

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var name = FileManager.GetFileByVersion(TestConfig.DptMobileAppFolderRt09010,
                TestConfig.DptMobileAppFileRt09010, TestConfig.DptAppEarliestVersionRt09010);
            FileManager.Upload(name);
            Assert.IsTrue(IsElementFound(AppsPage.YouDontHavePermissionsDialog),
                @"Error dialog 'You don't have permissions to modify models' should be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.YouDontHavePermissionsDialog),
                @"Error dialog 'You don't have permissions to modify models' should be closed");
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppsUri), $@"App '{AppName}' should not be uploaded");

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            ClickUntilShown(ItemsPage.TypeDropDown, CommonElement.DropDownOptionList);
            Assert.IsFalse(AreElementsContainText(CommonElement.DropDownOptionList, AppName),
                $@"Dropdown Type should not contain '{AppName}'");
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), availableTypes),
                "Only following item types should be available: " + string.Join(", ", availableTypes));

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "ImportApp",
                "GetAnyApp",
                "ModifyOwnApp",
                "ModifyAnyApp",
                "ImportAppVersion",
                "DeleteOwnAppVersion",
                "DeleteAnyAppVersion",
                "ChangePorscheCar",
                "ModifyPorschePdfCar",
                "ModifyPorscheUsedCar",
                "ViewPorscheCar",
                "ViewPorschePdfCar",
                "ViewPorscheUsedCar",
                "ModifyModel");
            OpenAppsPage();
            RefreshPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(name);
            Assert.IsTrue(IsElementFound(AppsPage.YouDontHavePermissionsDialog),
                @"Error dialog 'You don't have permissions to create models' should be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppsUri), $@"App '{AppName}' should not be uploaded");

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            ClickUntilShown(ItemsPage.TypeDropDown, CommonElement.DropDownOptionList);
            Assert.IsFalse(AreElementsContainText(CommonElement.DropDownOptionList, AppName),
                $@"Dropdown Type should not contain '{AppName}'");
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), availableTypes),
                "Only following item types should be available: " + string.Join(", ", availableTypes));

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "ImportApp",
                "GetAnyApp",
                "ModifyOwnApp",
                "ModifyAnyApp",
                "ImportAppVersion",
                "DeleteOwnAppVersion",
                "DeleteAnyAppVersion",
                "ChangePorscheCar",
                "ModifyPorschePdfCar",
                "ModifyPorscheUsedCar",
                "ViewPorscheCar",
                "ViewPorschePdfCar",
                "ViewPorscheUsedCar",
                "CreateModel");
            OpenAppsPage();
            RefreshPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(name);
            Assert.IsTrue(IsElementFound(AppsPage.AppTitle) && AreElementsContainText(AppsPage.AppTitle, AppName),
                $@"App '{AppName}' should be uploaded");

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            ClickUntilShown(ItemsPage.TypeDropDown, CommonElement.DropDownOptionList);
            Assert.IsFalse(AreElementsContainText(CommonElement.DropDownOptionList, AppName),
                $@"Dropdown Type should not contain '{AppName}'");
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), availableTypes),
                "Only following item types should be available: " + string.Join(", ", availableTypes));

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "ImportApp",
                "GetAnyApp",
                "ModifyOwnApp",
                "ModifyAnyApp",
                "ImportAppVersion",
                "DeleteOwnAppVersion",
                "DeleteAnyAppVersion",
                "ChangePorscheCar",
                "ModifyPorschePdfCar",
                "ModifyPorscheUsedCar",
                "ViewPorscheCar",
                "ViewPorschePdfCar",
                "ViewPorscheUsedCar",
                "CreateModel",
                "ViewTestmodel1",
                "ModifyTestmodel1");
            OpenItemsPage();
            RefreshPage();
            Click(PageFooter.AddItemButton);
            ClickUntilShown(ItemsPage.TypeDropDown, CommonElement.DropDownOptionList);
            availableTypes.Add(ItemTypeTestModel1);
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), availableTypes),
                "Only following item types should be available: " + string.Join(", ", availableTypes));

            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeTestModel1);
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 2,
                "Title and Picture fields must have validation errors");

            SendText(ItemsPage.Title, "12345678901");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorNotMore10Chars),
                @"Error 'Not more than 10 characters are allowed' should be shown");

            Click(PageFooter.CancelButton);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            name = FileManager.GetFileByVersion(TestConfig.DptMobileAppFolderRt09010,
                TestConfig.DptMobileAppFileRt09010, TestConfig.DptAppLatestVersionRt09010);
            FileManager.Upload(name);
            Assert.IsTrue(IsElementFound(AppsPage.YouDontHavePermissionsDialog),
                @"Error dialog 'You don't have permissions to create/modify models' should be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.YouDontHavePermissionsDialog),
                @"Error dialog 'You don't have permissions to create/modify models' should be closed");
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppsUri), $@"App '{AppName}' should not be uploaded");

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeTestModel1);
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 2,
                "Title and Picture fields must have validation errors");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "ImportApp",
                "GetAnyApp",
                "ModifyOwnApp",
                "ModifyAnyApp",
                "ImportAppVersion",
                "DeleteOwnAppVersion",
                "DeleteAnyAppVersion",
                "ChangePorscheCar",
                "ModifyPorschePdfCar",
                "ModifyPorscheUsedCar",
                "ViewPorscheCar",
                "ViewPorschePdfCar",
                "ViewPorscheUsedCar",
                "CreateModel",
                "ViewTestmodel1",
                "ModifyTestmodel1",
                "ModifyModel");
            Click(PageHeader.PageAppsButton);
            Click(ItemsPage.OkButton);
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(name);
            Assert.IsTrue(IsElementFound(AppsPage.AppTitle) && AreElementsContainText(AppsPage.AppTitle, AppName),
                $@"App '{AppName}' should be uploaded");

            OpenItemsPage();
            RefreshPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeTestModel1);
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 1,
                "AppTitle field only must have validation error");

            SendText(ItemsPage.Title, "12345678901");
            Assert.IsTrue(IsElementNotFoundQuickly(CommonElement.ValidationError), "No validation errors should be shown");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be saved successfully");

            OpenItemsPage();
            ClickUntilShown(PageHeader.FilterDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(AreElementsContainText(CommonElement.DropDownOptionList, ItemTypeTestModel1),
                $@"Filter dropdown should contain '{ItemTypeTestModel1}'");
        }

        [Test, Regression]
        // some app uploads implemented via API thus many steps look different comparing to a test case
        public void RT09020_Items()
        {
            TestStart();

            var itemName = "Test Welcome email template";
            var app = AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[0]); // 0.0.1
            OpenItemsPage();
            RefreshPage();
            Assert.IsTrue(IsElementFound(ItemsPage.TableEmpty),
                $"There should be no items on TestItem iBeacon app v.{TestConfig.IbeaconAppVersionsRt09020[0]} upload and submit");

            AddAppIbeacon2(AppStatus.New, TestConfig.IbeaconAppVersionsRt09020[1]); // 0.0.2
            RefreshPage();
            Assert.IsTrue(IsElementFound(ItemsPage.TableEmpty),
                $"There should be no items on TestItem iBeacon app v.{TestConfig.IbeaconAppVersionsRt09020[1]} upload, no submit");

            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[1]); // 0.0.2
            RefreshPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByText, itemName))
                && CountElements(ItemsPage.TableRowTitle) == 1,
                $"There should be one item on TestItem iBeacon app v.{TestConfig.IbeaconAppVersionsRt09020[1]} upload and submit");

            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName), ItemsPage.Status);
            var newTitle = $"Auto test {RandomNumber}";
            EditForm();
            SendText(ItemsPage.Title, newTitle);
            SubmitForm();
            var modifiedTime = GetValue(AppsPage.Modified);
            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[2]); // 0.0.3
            OpenItemsPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByText, newTitle))
                && CountElements(ItemsPage.TableRowTitle) == 1,
                $@"There should be one item '{newTitle}'on TestItem iBeacon app v.{
                    TestConfig.IbeaconAppVersionsRt09020[2]} upload and submit");

            ClickUntilShown(string.Format(ItemsPage.TableRowByText, newTitle), ItemsPage.Status);
            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, newTitle),
                $@"Item '{newTitle}' should not be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[2]} upload and submit");
            Assert.IsTrue(IsElementEquals(AppsPage.Modified, modifiedTime),
                $@"Item '{newTitle}' should not be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[2]} upload and submit");

            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[3], isOverwriteItems: true); // 0.0.4
            OpenItemsPage();
            itemName = "Json Test Welcome email template";
            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName), ItemsPage.Status);
            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, itemName),
                $@"Item '{itemName}' should be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[3]} upload and submit");
            Assert.IsFalse(IsElementEquals(AppsPage.Modified, modifiedTime),
                $@"Item '{itemName}' should be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[3]} upload and submit");

            EditForm();
            var newSubject = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Subject, newSubject);
            SubmitForm();
            modifiedTime = GetValue(AppsPage.Modified);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var fileName = FileManager.GetFileByVersion(
                TestConfig.IbeaconAppFolderRt09020,
                TestConfig.IbeaconAppFileRt09020,
                TestConfig.IbeaconAppVersionsRt09020[4]);
            FileManager.Upload(fileName);  // 0.0.5
            SubmitForm();
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorItemAlreadyExistsDialog),
                @"Dialog 'Item(s) with title(s) Json Test Welcome email template already exist. " +
                @"Do you want to overwrite?' should be displayed");

            Click(ItemsPage.NoButton);

            OpenItemsPage();
            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName), ItemsPage.Status);
            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, itemName),
                $@"Item '{itemName}' should not be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[4]} upload and submit");
            Assert.IsTrue(IsElementEquals(ItemsPage.SubjectReadOnly, newSubject) 
                          && IsElementEquals(ItemsPage.Modified, modifiedTime),
                $@"Item '{itemName}' should not be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[4]} upload and submit");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            fileName = FileManager.GetFileByVersion(
                TestConfig.IbeaconAppFolderRt09020, 
                TestConfig.IbeaconAppFileRt09020,
                TestConfig.IbeaconAppVersionsRt09020[5]);
            FileManager.Upload(fileName); // 0.0.6
            SubmitForm();
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorItemAlreadyExistsDialog),
                @"Dialog 'Item(s) with title(s) Json Test Welcome email template already exist. " + 
                @"Do you want to overwrite?' should be displayed");

            Click(AppsPage.CancelButton);

            OpenEntityPage(app);
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.IbeaconAppVersionsRt09020[5]),
                $"App v.{TestConfig.IbeaconAppVersionsRt09020[5]} should be uploaded on Cancel button press " + 
                "in overwrite items dialog");
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusNew), 
                $@"App v.{TestConfig.IbeaconAppVersionsRt09020[5]} should be in status '{StatusNew}' " +
                "on Cancel button press in overwrite items dialog");

            OpenItemsPage();
            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName), ItemsPage.Status);
            Assert.IsTrue(IsElementEquals(ItemsPage.SubjectReadOnly, newSubject) 
                          && IsElementEquals(AppsPage.Modified, modifiedTime),
                $@"Item '{itemName}' should not be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[5]} upload try (Cancel button pressed " +
                "in overwrite items dialog)");

            OpenEntityPage(app);
            EditForm();
            SubmitForm();
            Click(ItemsPage.YesButton);
            OpenItemsPage();
            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName), ItemsPage.Status);
            Assert.IsTrue(!IsElementEquals(ItemsPage.SubjectReadOnly, newSubject) 
                          && !IsElementEquals(AppsPage.Modified, modifiedTime),
                $@"Item '{itemName}' should be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[5]} on Yes button press " +
                "in overwrite items dialog");
            modifiedTime = GetValue(AppsPage.Modified);
            newSubject = GetValue(ItemsPage.SubjectReadOnly);

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            fileName = FileManager.GetFileByVersion(
                TestConfig.IbeaconAppFolderRt09020,
                TestConfig.IbeaconAppFileRt09020,
                TestConfig.IbeaconAppVersionsRt09020[6]);
            FileManager.Upload(fileName); // 0.0.7
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(ItemsPage.ErrorItemAlreadyExistsDialog),
                @"Dialog 'Item(s) already exist. Do you want to overwrite?' should not be displayed on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[6]} upload");
            
            OpenItemsPage();
            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName), ItemsPage.SubjectReadOnly);
            Assert.IsTrue(IsElementEquals(ItemsPage.SubjectReadOnly, newSubject) 
                          && !IsElementEquals(AppsPage.Modified, modifiedTime),
                $@"Item '{itemName}' should be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[6]} upload");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN should be present and active in language panel");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageDeButton),
                "Language DE should be absent in language panel");
        }

        [Test, Regression]
        public void RT09030_ItemLanguages()
        {
            const string itemName = "Json Test Welcome email template";
            const string itemName2 = "Test2 Welcome email template";
            const string itemName3 = "Test3 Welcome email template";
            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[6], 
                    isOverwriteItems: true); // 0.0.7
            TestStart();

            OpenItemsPage();
            Click(string.Format(ItemsPage.TableRowByText, itemName));
            var subject = GetValue(ItemsPage.SubjectReadOnly);

            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[7]); // 0.0.8
            OpenItemsPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByText, itemName)),
                "There should be one item on TestItem iBeacon app " + 
                $"v.{TestConfig.IbeaconAppVersionsRt09020[7]} upload and submit");

            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName), ItemsPage.Status);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive), 
                "EN language button should be present in panel and be active");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButton),
                "DE language button should be present in panel and be inactive");

            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, itemName),
                $@"Item '{itemName}' should not be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[7]} upload and submit");
            Assert.IsTrue(IsElementEquals(ItemsPage.SubjectReadOnly, subject),
                $@"Item '{itemName}' (lang EN) should not be overwritten on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[7]} upload and submit");

            Click(ItemsPage.LanguageDeButton);
            Assert.IsFalse(IsElementEquals(ItemsPage.SubjectReadOnly, subject),
                $@"Item '{itemName}' (lang DE) should have a different Subject on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[7]} upload and submit");

            subject = $"Autotest {RandomNumber}";
            EditForm();
            SendText(ItemsPage.Subject, subject);
            SubmitForm();
            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[8], isOverwriteItems: true); // 0.0.9
            
            RefreshPage();
            Click(ItemsPage.LanguageEnButton);
            Assert.IsFalse(IsElementEquals(ItemsPage.SubjectReadOnly, subject),
                $@"Item '{itemName}' (lang EN) should have a different Subject on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[8]} upload and submit");
            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, itemName),
                $@"Item '{itemName}' (lang EN) should have a normal Title on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[8]} upload and submit");
            Click(ItemsPage.LanguageDeButton);
            Assert.IsFalse(IsElementEquals(ItemsPage.SubjectReadOnly, subject),
                $@"Item '{itemName}' (lang DE) should have a different Subject on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[8]} upload and submit");

            EditForm();
            SendText(ItemsPage.Subject, subject);
            SubmitForm();
            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[9], isOverwriteItems: true); // 0.1.10

            RefreshPage();
            Click(ItemsPage.LanguageEnButton);
            Assert.IsFalse(IsElementEquals(ItemsPage.SubjectReadOnly, subject),
                $@"Item '{itemName}' (lang EN) should have a different Subject on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[9]} upload and submit");
            Click(ItemsPage.LanguageDeButton);
            Assert.IsFalse(IsElementEquals(ItemsPage.SubjectReadOnly, subject),
                $@"Item '{itemName}' (lang DE) should have a different Subject on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[9]} upload and submit");

            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[10], isOverwriteItems: true); // 0.1.11

            OpenItemsPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByText, itemName)),
                $@"There should be an item '{itemName}' on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[10]} upload and submit");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TableRowByText, itemName2)),
                $@"There should be an item '{itemName2}' on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[10]} upload and submit");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TableRowByText, itemName3)),
                $@"There should be an item '{itemName3}' on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[10]} upload and submit");

            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName2), ItemsPage.Status);
            Assert.IsTrue(IsElementFound(ItemsPage.LanguageEnButtonActive),
                $@"Item '{itemName2}': EN language button should be present in panel and be active");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageDeButton),
                $@"Item '{itemName2}': DE language button should not be present");
            OpenItemsPage();
            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName3), ItemsPage.Status);
            Assert.IsTrue(IsElementNotFound(ItemsPage.LanguageEnButton),
                $@"Item '{itemName3}': EN language button should not be present");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonActive),
                $@"Item '{itemName3}': DE language button should not be present in panel and be active");

            Click(PageFooter.DeleteButton);
            Click(ItemsPage.DeleteButton);

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var fileName = FileManager.GetFileByVersion(
                TestConfig.IbeaconAppFolderRt09020,
                TestConfig.IbeaconAppFileRt09020,
                TestConfig.IbeaconAppVersionsRt09020[11]);
            FileManager.Upload(fileName); // 0.1.12
            Assert.IsTrue(IsElementNotFound(ItemsPage.ErrorItemAlreadyExistsDialog),
                @"Dialog 'Item(s) with title(s) Json Test Welcome email template already exist. " +
                @"Do you want to overwrite?' should be displayed");
            SubmitForm();

            OpenItemsPage();
            Assert.IsTrue(IsElementNotFound(string.Format(ItemsPage.TableRowByText, itemName3)),
                $@"There should not be an item '{itemName3}' on delete");
            Click(PageFooter.ShowDeletedButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TableRowByText, itemName3)),
                $@"There should be an item '{itemName3}' on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[11]} upload and Show Deleted press");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            fileName = FileManager.GetFileByVersion(
                TestConfig.IbeaconAppFolderRt09020,
                TestConfig.IbeaconAppFileRt09020,
                TestConfig.IbeaconAppVersionsRt09020[12]);
            FileManager.Upload(fileName); // 0.1.13
            SubmitForm();
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorItemAlreadyExistsDialog),
                @"Dialog 'Item(s) with title(s) Json Test Welcome email template already exist. " +
                @"Do you want to overwrite?' should be displayed");

            Click(ItemsPage.YesButton);
            OpenItemsPage();
            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName3), ItemsPage.Status);
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive),
                $@"Item '{itemName3}' should have Status '{StatusActive}' on app " +
                $"v.{TestConfig.IbeaconAppVersionsRt09020[12]} upload and submit");
            Assert.IsTrue(AreElementsContainText(ItemsPage.TitleReadOnly, itemName3),
                $@"Item '{itemName3}' should contain text '{itemName3}' in Title on app " +
                $"v.{TestConfig.IbeaconAppVersionsRt09020[12]} upload and submit");
        }

        [Test, Regression]
        public void RT09040_ItemImages()
        {
            const string itemName = "Test Welcome email template";
            const string subjectEn = "EN Welcome to Porsche Centre";
            CurrentTenant = TenantTitle.onelang;
            TestStart();

            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[13], isOverwriteItems: true); // 0.1.14
            OpenItemsPage();
            RefreshPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByText, itemName))
                          && CountElements(string.Format(ItemsPage.TableRowByText, itemName)) == 1,
                "There should be one item on TestItem iBeacon app " +
                $"v.{TestConfig.IbeaconAppVersionsRt09020[13]} upload and submit");

            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName), ItemsPage.Status);
            Assert.IsTrue(IsElementNotFound(ItemsPage.LanguageBar), "Language panel should be absent");
            var imgSrcPrev = GetElementAttribute(ItemsPage.Image, "src");

            Assert.IsTrue(IsElementEquals(ItemsPage.SubjectReadOnly, subjectEn), 
                $"Subject field should contain {subjectEn}");

            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[14], isOverwriteItems: true); // 0.1.15
            OpenItemsPage();
            ClickUntilShown(string.Format(ItemsPage.TableRowByText, itemName), ItemsPage.Status);
            var imgSrcCurr = GetElementAttribute(ItemsPage.Image, "src");
            Assert.IsTrue(imgSrcCurr != imgSrcPrev,
                "Item should have a different image on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[14]} upload and submit");
            Assert.IsTrue(imgSrcCurr == GetElementAttribute(ItemsPage.AssetsSectionImage, "src"),
                "Item should have first Assets image as the main item image on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[14]} upload and submit");

            EditForm();
            Click(ItemsPage.AssetsSectionImageDeleteButton);
            SubmitForm();
            imgSrcPrev = GetElementAttribute(ItemsPage.Image, "src");
            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[15], isOverwriteItems: true); // 0.1.16
            
            RefreshPage();
            imgSrcCurr = GetElementAttribute(ItemsPage.Image, "src");
            Assert.IsTrue(imgSrcCurr == imgSrcPrev,
                "Item should not change Assets and main image on " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[15]} upload and submit");

            EditForm();
            Click(ItemsPage.AssetsSectionImageDeleteButton);
            SubmitForm();
            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[16], isOverwriteItems: true); // 0.1.17

            RefreshPage();
            Assert.IsTrue(IsElementFound(ItemsPage.AssetsSectionNoImages),
                $"Assets section should be empty on app v.{TestConfig.IbeaconAppVersionsRt09020[16]} upload and submit");

            EditForm();
            var key = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Key, key);
            SubmitForm();
            AddAppIbeacon2(AppStatus.Available, TestConfig.IbeaconAppVersionsRt09020[17], isOverwriteItems: true); // 0.1.18

            OpenItemsPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByText, itemName))
                && CountElements(string.Format(ItemsPage.TableRowByText, itemName)) >= 2 ,
                $@"There should be 2 items '{itemName}' on existing item Key field change and " +
                $"app v.{TestConfig.IbeaconAppVersionsRt09020[17]} upload and submit");
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
                AppApi.DeleteApps();
            }
        }
    }
}
