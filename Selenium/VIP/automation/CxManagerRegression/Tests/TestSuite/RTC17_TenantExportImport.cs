using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.Apps;
using Models.Items;
using Models.UserDirectory;
using NUnit.Framework;
using OpenQA.Selenium;
using Tests.Helpers;
using Tests.Resources;
using Place = Models.Places.Place;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC17_TenantExportImport : ParentTest
    {
        private Role _role;
        private string[] _allPermissions;

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            FileManager.Delete(
                Path.Combine(TestConfig.BrowserDownloadFolder + "export*.zip"));
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

        private void OpenExportImport()
        {
            MouseOver(PageHeader.UserMenuButton);
            Click(PageHeader.UserMenuExportImportButton);
        }

        [Test, Regression]
        public void RT17010_ExportImportMenuItemPermission()
        {
            _allPermissions = UserDirectoryApi.GetSupportedPermissions()
                .Where(x => x != "TenantExportImport")
                .ToArray();
            _role = UserDirectoryApi.SetRolePermissions(_role, _allPermissions);
            UserDirectoryApi.AssignRolesToUser(TestConfig.PermissionsUser, null);
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.manylang);
            CurrentUser = TestConfig.PermissionsUser;
            TestStart();
            MouseOver(PageHeader.UserMenuButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.UserMenuExportImportButton),
                @"There should be no Export/Import button in user menu without permission");

            _allPermissions = _allPermissions.Concat(new [] { "TenantExportImport" }).ToArray();
            var role2 = UserDirectoryApi.SetRolePermissions(null, _allPermissions);
            UserDirectoryApi.AssignRolesToUser(TestConfig.PermissionsUser, null);
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, role2, TenantTitle.manylang);
            RefreshPage();
            MouseOver(PageHeader.UserMenuButton);
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.UserMenuExportImportButton),
                @"There should be Export/Import button in user menu with permission restored");

            UserDirectoryApi.AssignRolesToUser(TestConfig.PermissionsUser, null);
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, role2, 
                TenantTitle.manylang, TenantTitle.onelang);
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, 
                TenantTitle.manylang, TenantTitle.onelang);
            RefreshPage();
            MouseOver(PageHeader.UserMenuButton);
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.UserMenuExportImportButton),
                @"There should be Export/Import button in user menu with 2 roles assigned");
            ChangeTenant(TenantTitle.onelang);
            MouseOver(PageHeader.UserMenuButton);
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.UserMenuExportImportButton),
                @"There should be Export/Import button in user menu with 2 roles assigned");

            NavigateTo(TestConfig.TenantsUrl);
            MouseOver(PageHeader.UserMenuButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.UserMenuExportImportButton),
                @"There should be no Export/Import button in user menu on Tenants page");
        }

        [Test, Regression]
        public void RT17020_ExportImportScreen()
        {
            CurrentTenant = TenantTitle.manylang;
            TestStart();

            MouseOver(PageHeader.UserMenuButton);
            Click(PageHeader.UserMenuExportImportButton);
            Assert.IsTrue(IsElementFound(ExportImport.Dialog),
                "Export/Import dialog window should be opened");
            Assert.IsTrue(IsElementFound(ExportImport.AddSubMenu),
                "+Add submenu button is absent");
            MouseOver(ExportImport.AddSubMenu);
            Assert.IsTrue(IsElementFound(ExportImport.PlacesButton),
                "Places button should be displayed in +Add submenu");
            Assert.IsTrue(IsElementFound(ExportImport.AppsButton),
                "Apps button should be displayed in +Add submenu");
            Assert.IsTrue(IsElementFound(ExportImport.ItemsButton),
                "Items button should be displayed in +Add submenu");
            Assert.IsTrue(IsElementFound(ExportImport.ExportButtonDisabled), 
                "Export button should be displayed and disabled");
            Assert.IsTrue(IsElementFound(ExportImport.CancelButton), "Cancel button is absent");
            Assert.IsTrue(IsElementFound(ExportImport.ImportButton), "Import button is absent");
            Assert.IsTrue(IsElementFound(ExportImport.ClearAllButtonDisabled), 
                "Clear All button should be displayed and disabled");

            // places
            Click(ExportImport.PlacesButton);
            Assert.IsTrue(IsElementFound(PlacesPage.SelectPlaceDialog),
                "Select places dialog should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.TableRowByTitleCollapseExpandButtonModal, string.Empty)),
                "Places should be displayed in hierarchical view");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PictureColumn),
                "Places dialog table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.TitleColumn),
                "Places dialog table should have Title column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ItemsColumn),
                "Places dialog table should have Items column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DeviceColumn),
                "Places dialog table should have Device column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.StatusColumn),
                "Places dialog table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ModifiedColumn),
                "Places dialog table should have Modified column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.SelectAllButton),
                "Places dialog table should have Select All button");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ClearSelectionButton),
                "Places dialog table should have Clear Selection button");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.CancelButton),
                "Places dialog table should have Cancel button");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AddButtonDisabled),
                "Places dialog table should have +Add button disabled");
            Assert.IsFalse(
                IsAnyElementEquals(string.Format(PlacesPage.TableRowStatusByTitle, string.Empty), StatusDeleted),
                "Places dialog should contain no Deleted places");

            Click(PlacesPage.SelectAllButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.TableRowUnselected),
                "All rows should be selected on Select All button press");
            Click(PlacesPage.ClearSelectionButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.TableRowSelected),
                "All rows selection should be cleared on Clear Selection button press");
            Click(PlacesPage.TableRowUnselected);
            Click(PlacesPage.TableRowUnselected);
            Assert.IsTrue(CountElements(PlacesPage.TableRowSelected) == 2,
                "Clicked 2 places should be marked as selected");
            Click(PlacesPage.TableRowSelected);
            Assert.IsTrue(CountElements(PlacesPage.TableRowSelected) == 1,
                "Clicked 1 selected place should be not marked as selected any more");

            Click(PlacesPage.StatusColumnModal);
            Click(PlacesPage.ModifiedColumnModal);
            Assert.IsTrue(CountElements(PlacesPage.TableRowSelected) == 1,
                "After sorting changes 1 selected place should still be marked as selected");
            Click(PlacesPage.TitleColumn);

            Click(PlacesPage.CancelButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.SelectPlaceDialog),
                "Select places dialog should be closed on Cancel press");
            Assert.IsTrue(IsElementFound(ExportImport.Dialog),
                "Export/Import dialog window should be opened");

            MouseOver(ExportImport.AddSubMenu);
            Click(ExportImport.PlacesButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.TableRowSelected),
                "No previously selected places on Places dialog re-open");
            Click(PlacesPage.SelectAllButton);
            var rowsSelected = GetCounterModal();
            Click(PlacesPage.AddButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.SelectPlaceDialog),
                "Select places dialog should be closed on +Add press");
            Assert.IsTrue(rowsSelected == CountElements(ExportImport.TableRow), 
                "All tab: Number of selected places is not equal to number of displayed places");
            
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.PlacesTab),
                "Places tab button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.AppsTab),
                "Apps tab button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.ItemsTab),
                "Items tab button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.AllTab),
                "All tab button should be displayed");

            Click(ExportImport.PlacesTab);
            Assert.IsTrue(rowsSelected == CountElements(ExportImport.TableRow),
                "Places tab: Number of selected places is not equal to number of displayed places");
            var places = GetValuesAsList(ExportImport.TableRowTitle);
            Click(ExportImport.AppsTab);
            Assert.IsTrue(IsElementNotFoundQuickly(ExportImport.TableRow),
                "Apps tab: No table rows should be displayed");
            Click(ExportImport.ItemsTab);
            Assert.IsTrue(IsElementNotFoundQuickly(ExportImport.TableRow),
                "Items tab: No table rows should be displayed");

            // apps
            MouseOver(ExportImport.AddSubMenu);
            Click(ExportImport.AppsButton);
            Assert.IsTrue(IsElementFound(PlacesPage.SelectAppDialog),
                "Select apps dialog should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ScreenshotColumn),
                "Apps dialog table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.TitleColumn),
                "Apps dialog table should have Title column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.DeviceTypeColumn),
                "Apps dialog table should have Device column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.StatusColumn),
                "Apps dialog table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.CreatedByColumn),
                "Apps dialog table should have Modified column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ModifiedColumn),
                "Apps dialog table should have Modified column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.SelectAllButton),
                "Apps dialog table should have Select All button");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ClearSelectionButton),
                "Apps dialog table should have Clear Selection button");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.CancelButton),
                "Apps dialog table should have Cancel button");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.AddButtonDisabled),
                "Apps dialog table should have +Add button disabled");
            Assert.IsFalse(
                IsAnyElementEquals(string.Format(AppsPage.TableRowStatusByTitle, string.Empty), StatusDeleted),
                "Apps dialog should contain no Deleted apps");

            Click(AppsPage.SelectAllButton);
            var rowsSelectedApps = GetCounterModal();
            Assert.IsTrue(IsElementNotFound(AppsPage.TableRowUnselected),
                "All app rows should be selected on Select All button press");
            Click(AppsPage.ClearSelectionButton);
            Assert.IsTrue(IsElementNotFound(AppsPage.TableRowSelected),
                "All app rows selection should be cleared on Clear Selection button press");
            Click(AppsPage.TableRowUnselected);
            Click(AppsPage.TableRowUnselected);
            Assert.IsTrue(CountElements(AppsPage.TableRowSelected) == 2,
                "Clicked 2 apps should be marked as selected");
            Click(AppsPage.TableRowSelected);
            Assert.IsTrue(CountElements(AppsPage.TableRowSelected) == 1,
                "Clicked 1 selected app should be not marked as selected any more");

            Click(AppsPage.StatusColumnModal);
            Click(AppsPage.ModifiedColumnModal);
            Assert.IsTrue(CountElements(AppsPage.TableRowSelected) == 1,
                "After sorting changes 1 selected app should still be marked as selected");
            Click(AppsPage.TitleColumnModal);

            Click(AppsPage.CancelButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.SelectAppDialog),
                "Select apps dialog should be closed on Cancel press");
            Assert.IsTrue(IsElementFound(ExportImport.Dialog),
                "Export/Import dialog window should be opened");

            MouseOver(ExportImport.AddSubMenu);
            Click(ExportImport.AppsButton);
            Assert.IsTrue(IsElementNotFound(AppsPage.TableRowSelected),
                "No previously selected apps on Apps dialog re-open");
            Click(AppsPage.SelectAllButton);
            Click(AppsPage.AddButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.SelectAppDialog),
                "Select apps dialog should be closed on +Add press");
            Assert.IsTrue(rowsSelected + rowsSelectedApps == CountElements(ExportImport.TableRow),
                "All tab: Number of selected apps and places is not equal to number of displayed ones");

            Click(ExportImport.PlacesTab);
            Assert.IsTrue(rowsSelected == CountElements(ExportImport.TableRow),
                "Places tab: Number of selected places is not equal to number of displayed places");
            Click(ExportImport.AppsTab);
            Assert.IsTrue(rowsSelectedApps == CountElements(ExportImport.TableRow),
                "Apps tab: Number of selected apps is not equal to number of displayed apps");
            var apps = GetValuesAsList(ExportImport.TableRowTitle);
            Click(ExportImport.ItemsTab);
            Assert.IsTrue(IsElementNotFoundQuickly(ExportImport.TableRow),
                "Items tab: No table rows should be displayed");

            // items
            MouseOver(ExportImport.AddSubMenu);
            Click(ExportImport.ItemsButton);
            Assert.IsTrue(IsElementFound(PlacesPage.SelectItemDialog),
                "Select item dialog should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureColumn),
                "Items dialog table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TitleColumn),
                "Items dialog table should have Title column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TypeColumn),
                "Items dialog table should have Device column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.StatusColumn),
                "Items dialog table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.CreatedColumn),
                "Items dialog table should have Modified column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ModifiedColumn),
                "Items dialog table should have Modified column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SelectAllButton),
                "Items dialog table should have Select All button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ClearSelectionButton),
                "Items dialog table should have Clear Selection button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.CancelButton),
                "Items dialog table should have Cancel button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AddButtonDisabled),
                "Items dialog table should have +Add button disabled");
            Assert.IsFalse(
                IsAnyElementEquals(string.Format(ItemsPage.TableRowStatusByTitle, string.Empty), StatusDeleted),
                "Items dialog should contain no Deleted items");

            Click(ItemsPage.SelectAllButton);
            var rowsSelectedItems = GetCounterModal();
            Assert.IsTrue(IsElementNotFound(ItemsPage.TableRowUnselected),
                "All item rows should be selected on Select All button press");
            Click(ItemsPage.ClearSelectionButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.TableRowSelected),
                "All item rows selection should be cleared on Clear Selection button press");
            Click(ItemsPage.TableRowUnselected);
            Click(ItemsPage.TableRowUnselected);
            Assert.IsTrue(CountElements(ItemsPage.TableRowSelected) == 2,
                "Clicked 2 items should be marked as selected");
            Click(ItemsPage.TableRowSelected);
            Assert.IsTrue(CountElements(ItemsPage.TableRowSelected) == 1,
                "Clicked 1 selected item should be not marked as selected any more");

            Click(ItemsPage.StatusColumnModal);
            Click(ItemsPage.ModifiedColumnModal);
            Assert.IsTrue(CountElements(ItemsPage.TableRowSelected) == 1,
                "After sorting changes 1 selected item should still be marked as selected");
            Click(ItemsPage.TitleColumnModal);

            Click(ItemsPage.CancelButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.SelectItemDialog),
                "Select Items dialog should be closed on Cancel press");
            Assert.IsTrue(IsElementFound(ExportImport.Dialog),
                "Export/Import dialog window should be opened");

            MouseOver(ExportImport.AddSubMenu);
            Click(ExportImport.ItemsButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.TableRowSelected),
                "No previously selected Items on Items dialog re-open");
            Click(ItemsPage.SelectAllButton);
            Click(ItemsPage.AddButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.SelectItemDialog),
                "Select Items dialog should be closed on +Add press");
            Assert.IsTrue(rowsSelected + rowsSelectedApps + rowsSelectedItems 
                          == CountElements(ExportImport.TableRow),
                "All tab: Number of selected items, apps, and places is not equal to number of displayed ones");

            Click(ExportImport.PlacesTab);
            Assert.IsTrue(rowsSelected == CountElements(ExportImport.TableRow),
                "Places tab: Number of selected places is not equal to number of displayed places");
            Click(ExportImport.AppsTab);
            Assert.IsTrue(rowsSelectedApps == CountElements(ExportImport.TableRow),
                "Apps tab: Number of selected apps is not equal to number of displayed apps");
            Click(ExportImport.ItemsTab);
            Assert.IsTrue(rowsSelectedItems == CountElements(ExportImport.TableRow),
                "Items tab: Number of selected items is not equal to number of displayed items");
            var items = GetValuesAsList(ExportImport.TableRowTitle);
            
            Click(ExportImport.ImportButton);
            FileManager.Upload(TestConfig.Zip);
            Assert.IsTrue(IsElementFound(ExportImport.ErrorArchiveDoesNotContainTenantData),
                @"Error 'archive does not contain tenant data' should be shown for wrong archive");
            Click(ExportImport.OkButton);

            ChangeTenant(TenantTitle.onelang);
            MouseOver(PageHeader.UserMenuButton);
            Click(PageHeader.UserMenuExportImportButton);
            MouseOver(ExportImport.AddSubMenu);
            Click(ExportImport.PlacesButton);
            var places1 = GetValuesAsList(PlacesPage.TableRowTitle);
            Assert.IsFalse(IsCollectionContainsCollection(places, places1),
                "Select place dialog should have native place list for every tenant");
            Click(PlacesPage.CancelButton);
            MouseOver(ExportImport.AddSubMenu);
            Click(ExportImport.AppsButton);
            var apps1 = GetValuesAsList(PlacesPage.TableRowTitle);
            Assert.IsFalse(IsCollectionContainsCollection(apps, apps1),
                "Select app dialog should have native app list for every tenant");
            Click(AppsPage.CancelButton);
            MouseOver(ExportImport.AddSubMenu);
            Click(ExportImport.ItemsButton);
            var items1 = GetValuesAsList(PlacesPage.TableRowTitle);
            Assert.IsFalse(IsCollectionContainsCollection(items, items1),
                "Select item dialog should have native item list for every tenant");
            Click(ItemsPage.CancelButton);
            Click(ExportImport.CancelButton);
        }

        [Test, Regression]
        public void RT17030_ExportToNewTenant()
        {
            CurrentTenant = TenantTitle.export1;
            Place place1 = null, place2 = null, place3 = null, place4 = null, 
                place5 = null;
            AppResponse app1 = null, app2 = null, app3 = null;
            Item item1 = null, item2 = null, item3 = null;
            var root = AddPlaceNoType(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true);
            app1 = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            Parallel.Invoke(
                () => app3 = AddAppPlayer(version: TestConfig.PlayerAppVersions[1]),
                () => AddAppIpadPlayer(),
                () => AddAppLegoBoost(TestConfig.LegoBoostAppVersions[0]),
                () => AddAppEventKit(TestConfig.EventKitAppVersions[0]),
                () => place1 = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: true, 
                    pageToBeOpened: 0, isCreateNewPlace: true),
                () => place2 = AddPlaceWw(PlaceStatus.Any, isAssignDevice: true, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => place3 = AddPlaceNoType(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => place4 = AddPlaceIbeacon(PlaceStatus.Active, pageToBeOpened: 0, isCreateNewPlace: true),
                () => place5 = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => AddPlaceIos(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => app2 = AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion),
                () => item1 = AddItem(ItemType.PorscheCar, isAddNew: true),
                () => item2 = AddItem(ItemType.PorscheCar, isAssignImage: true, isAddNew: true),
                () => item3 = ItemApi.SearchItem(ItemTypeWelcomeEmailTemplate)
            );
            AddItemToIbeaconApp(app1, "$.texts.emails.welcomeEmailTemplate", item3);
            (item2, place1) = AssignPlaceToItem(item2, place1, isAddSilently: true);
            Parallel.Invoke(
                () => root = AssignChildrenToParentPlace(root, isAddSilently: true, place1, place2),
                () => place3 = AssignChildrenToParentPlace(place3, isAddSilently: true, place4, place5),
                () => place1 = AssignAppToPlace(place1, app1, item1, ItemTypePorscheCar, isAddSilently: true),
                () => place2 = AssignAppToPlace(place2, app2, null, null, isAddSilently: true)
            );           
            TestStart();

            OpenExportImport();
            MouseOver(ExportImport.AddSubMenu);
            Click(ExportImport.PlacesButton);
            if (IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitleCollapsed, root.Title)))
            {
                Click(string.Format(PlacesPage.TableRowByTitleCollapseExpandButtonModal, root.Title));
            }
            Click(string.Format(PlacesPage.TableRowByTitle, root.Title));
            Click(string.Format(PlacesPage.TableRowByTitle, place1.Title));
            Click(string.Format(PlacesPage.TableRowByTitle, place2.Title));
            if (IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitleCollapsed, place3.Title)))
            {
                Click(string.Format(PlacesPage.TableRowByTitleCollapseExpandButtonModal, place3.Title));
            }
            Click(string.Format(PlacesPage.TableRowByTitle, place5.Title));
            Click(PlacesPage.AddButton);
            Click(ExportImport.ExportButton);
            IsUseAllPageReadyChecks = false;
            Assert.IsTrue(IsElementFound(CommonElement.UploadProgress),
                "Upload/export progress gauge should be shown on page header");
            Assert.IsTrue(IsElementFound(CommonElement.UploadSpinner),
                "Upload spinner should be shown on page header during export");
            IsUseAllPageReadyChecks = true;
            var mask = $"{CurrentTenantCode}_{DateTime.UtcNow:yyyyMMdd}_??????.zip";
            var exportFileName = FileManager.GetRecentDownloadedFileName(mask);
            CloseDownloadPanel();
            Assert.IsNotNull(exportFileName,
                $"Tenant export error. File {mask} should be downloaded automatically.");

            ChangeTenant(TenantTitle.import1);
            OpenExportImport();
            Click(ExportImport.ImportButton);
            FileManager.Upload(exportFileName);
            IsUseAllPageReadyChecks = false;
            Assert.IsTrue(IsElementFound(CommonElement.UploadProgress),
                "Upload/import progress gauge should be shown on page header");
            Assert.IsTrue(IsElementFound(CommonElement.UploadSpinner),
                "Upload spinner should be shown on page header during import");
            IsUseAllPageReadyChecks = true;
            Assert.IsTrue(IsElementFound(ExportImport.ImportCompletedDialog),
                @"Dialog 'Import completed' should be shown on import end");
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.CloseButton),
                @"Close button should be present in footer of 'Import completed' dialog");

            Assert.IsTrue(IsElementFoundQuickly(ExportImport.PlacesTabCompleted),
                "Places tab button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.AppsTabCompleted),
                "Apps tab button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.ItemsTabCompleted),
                "Items tab button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.AllTabCompleted),
                "All tab button should be displayed");

            var placesExported = new[]
            {
                root.Title, place1.Title, place2.Title, place5.Title
            };
            var appsExported = new[]
            {
                app1.ActualAppVersion.Title, app2.ActualAppVersion.Title, app3.ActualAppVersion.Title
            };
            var itemsExported = new[]
            {
                $"{item1.JsonDataTitle} {item1.SerialNumber}", 
                $"{item2.JsonDataTitle} {item2.SerialNumber}", 
                item3.JsonDataTitle
            };
            Assert.IsTrue(AreCollectionsEqual(
                    GetValuesAsList(ExportImport.TableRowTitleCompleted),
                    placesExported.Concat(appsExported).Concat(itemsExported)),
                "All tab: should contain list of places, apps, and items were exported." +
                "\nPlaces: " + string.Join(", ", placesExported) +
                "\nApps: " + string.Join(", ", appsExported) +
                "\nItems: " + string.Join(", ", itemsExported));
            Click(ExportImport.PlacesTabCompleted);
            Assert.IsTrue(AreCollectionsEqual(
                    GetValuesAsList(ExportImport.TableRowTitleCompleted),
                    placesExported),
                "Places tab: should contain list of places were exported: " + 
                string.Join(", ", placesExported));
            Click(ExportImport.AppsTabCompleted);
            Assert.IsTrue(AreCollectionsEqual(
                    GetValuesAsList(ExportImport.TableRowTitleCompleted),
                    appsExported),
                "Apps tab: should contain list of apps were exported: " +
                string.Join(", ", appsExported));
            Click(ExportImport.ItemsTabCompleted);
            Assert.IsTrue(AreCollectionsEqual(
                    GetValuesAsList(ExportImport.TableRowTitleCompleted),
                    itemsExported),
                "Items tab: should contain list of items were exported: " +
                string.Join(", ", itemsExported));
            Click(ExportImport.CloseButton);
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.RefreshPageDialog),
                "Dialog that asks for page refresh should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.YesButton),
                "Dialog that asks for page refresh should have Yes button");
            Assert.IsTrue(IsElementFoundQuickly(ExportImport.NoButton),
                "Dialog that asks for page refresh should have No button");
            Click(ExportImport.YesButton);

            Assert.IsTrue(GetCounter() == placesExported.Length,
                "On place list page counter should be equal to number of places: " +
                string.Join(", ", placesExported));
            Click(string.Format(PlacesPage.TableRowByTitleCollapseExpandButton, root.Title),
                true, 1);
            Assert.IsTrue(IsCollectionContainsCollection(
                    GetValuesAsList(PlacesPage.TableRowTitle),
                    placesExported),
                "Place list should contain list of places: " + string.Join(", ", placesExported));

            Click(string.Format(PlacesPage.TableRowByTitle, place2.Title));
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, string.Empty),
                "WW place should be imported without device assigned");
            
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow1),
                "WW place should have Apps section with app assigned");

            OpenPlacesPage();
            Click(string.Format(PlacesPage.TableRowByTitle, place1.Title));
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "iBeacon place should be imported without device assigned");

            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow1),
                "iBeacon place should have Apps section with app assigned");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(PlacesPage.ItemsSectionItemByTitle, item2.JsonDataTitle)),
                $@"Place {place1.Title} should keep assignation to item {item2.JsonDataTitle}");

            OpenAppsPage();
            Assert.IsTrue(GetCounter() == appsExported.Length,
                "On app list page counter should be equal to number of apps: " +
                string.Join(", ", appsExported));
            Assert.IsTrue(IsCollectionContainsCollection(
                    GetValuesAsList(AppsPage.TableRowTitle),
                    appsExported),
                "App list should contain list of apps: " + string.Join(", ", appsExported));

            Click(string.Format(AppsPage.TableRowByText, app1.ActualAppVersion.Title));
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Assert.IsTrue(IsElementEquals(AppsPage.WelcomeNewCustomerReadOnly, item3.JsonDataTitle),
                $@"Texts > Emails > Welcome New Customer should contain item '{item3.JsonDataTitle}'");

            OpenItemsPage();
            Assert.IsTrue(GetCounter() == itemsExported.Length,
                "On item list page counter should be equal to number of items: " +
                string.Join(", ", itemsExported));
            Assert.IsTrue(IsCollectionContainsCollection(
                    GetValuesAsList(AppsPage.TableRowTitle),
                    itemsExported),
                "Item list should contain list of items: " + string.Join(", ", itemsExported));

            Click(string.Format(ItemsPage.TableRowByTitle, item2.JsonDataTitle));
            Assert.IsTrue(
                IsElementFound(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place1.Title)),
                $@"References: item should have reference to place '{place1.Title}'");

            var linkItem = GetElementAttribute(ItemsPage.Image, "src");
            OpenMediaPage();
            Assert.IsTrue(GetElementsAttribute(MediaLibrary.TableRowPicture, "style").Any(x => x == linkItem),
                $@"Media library: cannot find asset (main image) of item '{item2.JsonDataTitle}'");
        }

        public void RT10040_PlacesAppsSection()
        {
            Place place1 = null, place2 = null;
            AppResponse app1 = null, app2 = null;
            CurrentTenant = TenantTitle.manylang;
            Parallel.Invoke(
                () => place1 = AddPlaceWw(PlaceStatus.Any, isAssignDevice: true, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => place2 = AddPlaceIbeacon(PlaceStatus.Any, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => _role = UserDirectoryApi.SetRolePermissions(
                    _role,
                    "Login",
                    "CreateLocation",
                    "GetAnyLocation",
                    "ModifyOwnLocation",
                    "DeleteOwnLocation",
                    "UndeleteOwnLocation",
                    "ModifyAnyLocation",
                    "DeleteAnyLocation",
                    "UndeleteAnyLocation"),
                () => app1 = AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion),
                () => app2 = AddAppIbeacon(TestConfig.IbeaconAppVersions[1])
            );
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.manylang);
            TestStart(TestConfig.LoginUrl, false);
            Parallel.Invoke(
                () => Login(TestConfig.PermissionsUser, false),
                () => place2 = AssignAppToPlace(place2, app2, null, null, isAddSilently: true),
                () => place1 = AssignAppToPlace(place1, app1, null, null, isAddSilently: true)
            );

            OpenEntityPage(place1);
            Assert.IsTrue(IsElementNotFound(PlacesPage.AppsSectionTableHeader),
                "Apps section should not be displayed");
            Assert.IsTrue(IsElementNotFound(PlacesPage.ScheduleSectionRow1),
                "Schedule section should not be displayed");
            Assert.IsTrue(IsElementFound(PlacesPage.FaceApiKeyReadOnly),
                "Face API Key field should be shown");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "GetAnyScheduleEntry");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableHeader),
                "Apps section should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow1),
                "Apps section: one app row should be displayed");
            Assert.IsTrue(IsElementNotFound(PlacesPage.ScheduleSectionRow1),
                "Schedule section should not be displayed");

            MouseOver(PageFooter.AddPlaceSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddExistingPlaceAsChildButton),
                "Add Existing Place As Child button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CreateNewChildPlaceButton),
                "Create New Child Place button should be present in page footer");

            OpenEntityPage(place2);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableHeader),
                "Apps section should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow1),
                "Apps section: one app row should be displayed");

            EditForm();
            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsTitleReadOnly),
                "Apps section: Title should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppTitleReadOnly),
                "Apps section: App Title should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsVersionReadOnly),
                "Apps section: Version should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsCarReadOnly),
                "Apps section: Car should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsPoiReadOnly),
                "Apps section: POI should be read-only");

            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton),
                "Apps section: Car '...' should not be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRowDetailsPoiDetailsButton),
                "Apps section: POI '...' should not be shown");

            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow1DeleteButton),
                "Apps section: Row 1 delete button should not be shown");

            MouseOver(PageFooter.AddPlaceSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddExistingPlaceAsChildButton),
                "Add Existing Place As Child button should be present in page footer");         
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.AddAppInPlacesButton),
                "Add App button should not be present in page footer"); 

            OpenEntityPage(place1);
            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsTitleReadOnly),
                "Apps section: Title should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppPackageReadOnly),
                "Apps section: App Title should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsVersionReadOnly),
                "Apps section: Version should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsPlayerReadOnly),
                "Apps section: Player Title should be read-only");
            Assert.IsTrue(CountElements(PlacesPage.AppsSectionTableRowDetailsSinglePropFields) == 6,
                "Apps section: Title, App title, Version, Player title, Auto player update, and Auto " + 
                "package update fields should be shown");

            EditForm();
            //Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsTitleReadOnly),
                "Apps section: Title should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppPackageReadOnly),
                "Apps section: App Package should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsVersionReadOnly),
                "Apps section: Version should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsPlayerReadOnly),
                "Apps section: Player Title should be read-only");

            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow1DeleteButton),
                "Apps section: Row 1 delete button should not be shown");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be present in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.UploadDayButton),
                "Upload Day button should not be shown in footer");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "GetAnyScheduleEntry",
                "ModifyAnyScheduleEntry", 
                "CreateScheduleEntry");
            RefreshPage();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddExistingPlaceAsChildButton),
                "Add Existing Place As Child button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CreateNewChildPlaceButton),
                "Create New Child Place button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddAppInPlacesButton),
                "Add App button should be present in page footer");

            EditForm();
            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsTitle),
                "Apps section: Title should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppPackageDropDown),
                "Apps section: App Title should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsVersionDropDown),
                "Apps section: Version should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsPlayerReadOnly),
                "Apps section: Player Title should be read-only");

            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow1DeleteButton),
                "Apps section: Row 1 delete button should not be shown");

            MouseOver(PageFooter.AddPlaceSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddExistingPlaceAsChildButton),
                "Add Existing Place As Child button should be present in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.CreateNewChildPlaceButton),
                "Create New Child Place button should not be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddAppInPlacesButton),
                "Add App button should be present in page footer");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "GetAnyScheduleEntry",
                "ModifyAnyScheduleEntry",
                "CreateScheduleEntry",
                "DeleteAnyScheduleEntry");
            RefreshPage();
            EditForm();
            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow1DeleteButton),
                "Apps section: Row 1 delete button should be shown");

            Click(PlacesPage.AppsSectionTableRow1DeleteButton);
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(PlacesPage.AppsSectionTableHeader),
                "Apps section should not be displayed after app delete");
        }

        public void RT10050_PlacesAppSectionSpecial()
        {
            Place place1 = null;
            AppResponse app1 = null;
            CurrentTenant = TenantTitle.nolang;
            Parallel.Invoke(
                () => place1 = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => _role = UserDirectoryApi.SetRolePermissions(
                    _role,
                    "Login",
                    "CreateLocation",
                    "GetAnyLocation",
                    "ModifyOwnLocation",
                    "DeleteOwnLocation",
                    "UndeleteOwnLocation",
                    "ModifyAnyLocation",
                    "DeleteAnyLocation",
                    "UndeleteAnyLocation",
                    "GetAnyScheduleEntry",
                    "ModifyAnyScheduleEntry",
                    "CreateScheduleEntry",
                    "DeleteAnyScheduleEntry",
                    "ViewVolume", 
                    "ViewInteraction"),
                () => app1 = AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion)
            );
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.nolang);
            TestStart(TestConfig.LoginUrl, false);
            Parallel.Invoke(
                () => Login(TestConfig.PermissionsUser, false),
                () => place1 = AssignAppToPlace(place1, app1, null, null, isAddSilently: true)
            );

            OpenEntityPage(place1);
            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsTitleReadOnly),
                "Apps section: Title should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppPackageReadOnly),
                "Apps section: App Title should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsVersionReadOnly),
                "Apps section: Version should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsPlayerReadOnly),
                "Apps section: Player Title should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsMasterVolumeReadOnly),
                "Apps section: Master Volume should be read-only");
            Assert.IsTrue(CountElements(PlacesPage.AppsSectionTableRowDetailsSinglePropFields) == 7,
                "Apps section: only Title, App title, Version, Player title, Master Volume, Auto player " + 
                "update, and Auto package update fields should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsInteractionButton),
                "Apps section: Interaction button should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionRow1DetailsDiagnosticsButton),
                "Apps section: Diagnostics button should not be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionRow1DetailsDownloadButton),
                "Apps section: Download button should not be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionRow1DetailsScreenButton),
                "Apps section: Screen button should not be shown");

            EditForm();
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsMasterVolumeReadOnly),
                "Apps section: Master Volume should be read-only");

            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsInteractionButton, 
                PlacesPage.FocusAreaButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.IdleTimeoutMinutesReadOnly),
                "Apps > App > Interaction > Idle Timeout should be read-only");

            Click(PlacesPage.FocusAreaButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsFocusAreaDepthReadOnly),
                "Apps > App > Interaction > Focus Area > Focus Area Depth should be read-only");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "GetAnyScheduleEntry",
                "ModifyAnyScheduleEntry",
                "CreateScheduleEntry",
                "DeleteAnyScheduleEntry",
                "ViewVolume",
                "ViewInteraction",
                "ViewLivePreview", 
                "ViewDiagnostics", 
                "ModifyVolume", 
                "ViewRecordingQuality", 
                "ViewRecordingConditions", 
                "ViewDownload", 
                "ViewComposerUrls", 
                "ModifyInteraction", 
                "ViewLogo", 
                "ViewBiometrics");
            RefreshPage();
            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsTitleReadOnly),
                "Apps section: Title should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppPackageReadOnly),
                "Apps section: App Title should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsVersionReadOnly),
                "Apps section: Version should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsPlayerReadOnly),
                "Apps section: Player Title should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsMasterVolumeReadOnly),
                "Apps section: Master Volume should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsLivePreviewCheckBox),
                "Apps section: Live Preview should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsRecordingQualityReadOnly),
                "Apps section: Recording Quality should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsComposerFileUrlReadOnly),
                "Apps section: Composer File Url should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsComposerServiceUrlReadOnly),
                "Apps section: Composer Service Url should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsGenderAgeTrackingReadOnly),
                "Apps section: Gender/Age Tracking should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAgeGenderBasedContentCheckBox),
                "Apps section: Age-/Gender-based Content should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsInteractionButton),
                "Apps section: Interaction button should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsDiagnosticsButton),
                "Apps section: Diagnostics button should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsDownloadButton),
                "Apps section: Download button should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsScreenButton),
                "Apps section: Screen button should be shown");

            EditForm();
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsLivePreviewCheckBox),
                "Apps section: Live Preview should be shown");
            Click(PlacesPage.AppsSectionTableRowDetailsLivePreviewCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.AppsSectionTableRowDetailsLivePreviewCheckBox),
                "Apps section: Live Preview should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsRecordingQualityReadOnly),
                "Apps section: Recording Quality should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsComposerFileUrlReadOnly),
                "Apps section: Composer File Url should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsComposerServiceUrlReadOnly),
                "Apps section: Composer Service Url should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsGenderAgeTrackingReadOnly),
                "Apps section: Gender/Age Tracking should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAgeGenderBasedContentCheckBox),
                "Apps section: Age-/Gender-based Content should be shown");
            Click(PlacesPage.AppsSectionTableRowDetailsAgeGenderBasedContentCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.AppsSectionTableRowDetailsAgeGenderBasedContentCheckBox),
                "Apps section: Age-/Gender-based Content should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsTitle),
                "Apps section: Title should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppPackageDropDown),
                "Apps section: App Title should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsVersionDropDown),
                "Apps section: Version should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsPlayerReadOnly),
                "Apps section: Player Title should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsMasterVolume),
                "Apps section: Master Volume should be available for edit");

            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsDiagnosticsButton, PlacesPage.UserRecordingButton);
            Assert.IsTrue(IsElementFound(PlacesPage.TopMostCheckBox),
                "Apps section: Top Most should be shown");
            Click(PlacesPage.TopMostCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.TopMostCheckBox), "Apps section: Top Most should be read-only");

            Click(PlacesPage.UserRecordingButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppWidthReadOnly),
                "Apps > App > Diagnostics > User Recording > App Width should be read-only");

            Click(PlacesPage.AppsSectionRow1DetailsDownloadButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsTempMediaDeletionReadOnly),
                "Apps > App > Download > Temp Media Deletion should be read-only");

            Click(PlacesPage.AppsSectionRow1DetailsInteractionButton);
            Assert.IsTrue(IsElementFound(PlacesPage.IdleTimeoutMinutesDropDown),
                "Apps > App > Interaction > Idle Timeout should be available for edit");

            Click(PlacesPage.FocusAreaButton);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsFocusAreaDepth),
                "Apps > App > Interaction > Focus Area > Focus Area Depth should be available for edit");

            Click(PlacesPage.AppsSectionRow1DetailsScreenButton);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsLogoLocationReadOnly),
                "Apps > App > Screen > Logo Location should be read-only");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "GetAnyScheduleEntry",
                "ModifyAnyScheduleEntry",
                "CreateScheduleEntry",
                "DeleteAnyScheduleEntry",
                "ViewVolume",
                "ViewInteraction",
                "ViewLivePreview",
                "ViewDiagnostics",
                "ModifyVolume",
                "ViewRecordingQuality",
                "ViewRecordingConditions",
                "ViewDownload",
                "ViewComposerUrls",
                "ModifyInteraction",
                "ViewLogo",
                "ViewBiometrics",
                "ModifyLivePreview", 
                "ModifyDiagnostics", 
                "ModifyRecordingQuality", 
                "ModifyRecordingConditions", 
                "ModifyDownload", 
                "ModifyComposerUrls", 
                "ModifyLogo", 
                "ModifyBiometrics");
            RefreshPage();
            EditForm();
            Click(PlacesPage.AppsSectionTableRow1);
            Click(PlacesPage.AppsSectionTableRowDetailsLivePreviewCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.AppsSectionTableRowDetailsLivePreviewCheckBox),
                "Apps section: Live Preview should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsRecordingQualityDropDown),
                "Apps section: Recording Quality should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsComposerFileUrlDropDown),
                "Apps section: Composer File Url should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsComposerServiceUrlDropDown),
                "Apps section: Composer Service Url should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsGenderAgeTrackingDropDown),
                "Apps section: Gender/Age Tracking should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAgeGenderBasedContentCheckBox),
                "Apps section: Age-/Gender-based Content should be shown");
            Click(PlacesPage.AppsSectionTableRowDetailsAgeGenderBasedContentCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.AppsSectionTableRowDetailsAgeGenderBasedContentCheckBox),
                "Apps section: Age-/Gender-based Content should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsTitle),
                "Apps section: Title should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppPackageDropDown),
                "Apps section: App Title should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsVersionDropDown),
                "Apps section: Version should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsPlayerReadOnly),
                "Apps section: Player Title should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsMasterVolume),
                "Apps section: Master Volume should be available for edit");

            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsDiagnosticsButton, PlacesPage.UserRecordingButton);
            Assert.IsTrue(IsElementFound(PlacesPage.TopMostCheckBox),
                "Apps section: Top Most should be shown");
            Click(PlacesPage.TopMostCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.TopMostCheckBox), 
                "Apps section: Top Most should be available for edit");

            Click(PlacesPage.UserRecordingButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsAppWidth),
                "Apps > App > Diagnostics > User Recording > App Width should be available for edit");

            Click(PlacesPage.AppsSectionRow1DetailsDownloadButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsTempMediaDeletionDropDown),
                "Apps > App > Download > Temp Media Deletion should be available for edit");

            Click(PlacesPage.AppsSectionRow1DetailsScreenButton);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetailsLogoLocationDropDown),
                "Apps > App > Screen > Logo Location should be available for edit");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place should be saved successfully");
        }

        public void RT10060_PlacesFooterButtons()
        {
            Place place1 = null;
            AppResponse app1 = null;
            CurrentTenant = TenantTitle.nolang;
            Parallel.Invoke(
                () => place1 = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => _role = UserDirectoryApi.SetRolePermissions(
                    _role,
                    "Login",
                    "CreateLocation",
                    "GetAnyLocation",
                    "ModifyOwnLocation",
                    "DeleteOwnLocation",
                    "UndeleteOwnLocation",
                    "ModifyAnyLocation",
                    "DeleteAnyLocation",
                    "UndeleteAnyLocation",
                    "GetAnyScheduleEntry",
                    "ModifyAnyScheduleEntry",
                    "CreateScheduleEntry",
                    "DeleteAnyScheduleEntry",
                    "ViewVolume",
                    "ViewInteraction",
                    "ViewLivePreview",
                    "ViewDiagnostics",
                    "ModifyVolume",
                    "ViewRecordingQuality",
                    "ViewRecordingConditions",
                    "ViewDownload",
                    "ViewComposerUrls",
                    "ModifyInteraction",
                    "ViewLogo",
                    "ViewBiometrics",
                    "ModifyLivePreview",
                    "ModifyDiagnostics",
                    "ModifyRecordingQuality",
                    "ModifyRecordingConditions",
                    "ModifyDownload",
                    "ModifyComposerUrls",
                    "ModifyLogo",
                    "ModifyBiometrics"),
                () => app1 = AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion)
            );
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.nolang);
            TestStart(TestConfig.LoginUrl, false);
            Parallel.Invoke(
                () => Login(TestConfig.PermissionsUser, false),
                () => place1 = AssignAppToPlace(place1, app1, null, null, isAddSilently: true)
            );

            OpenEntityPage(place1);
            EditForm();
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be present in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.UploadDayButton),
                "Upload Day button should not be shown in footer");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "GetAnyScheduleEntry",
                "ModifyAnyScheduleEntry",
                "CreateScheduleEntry",
                "DeleteAnyScheduleEntry",
                "ViewVolume",
                "ViewInteraction",
                "ViewLivePreview",
                "ViewDiagnostics",
                "ModifyVolume",
                "ViewRecordingQuality",
                "ViewRecordingConditions",
                "ViewDownload",
                "ViewComposerUrls",
                "ModifyInteraction",
                "ViewLogo",
                "ViewBiometrics",
                "ModifyLivePreview",
                "ModifyDiagnostics",
                "ModifyRecordingQuality",
                "ModifyRecordingConditions",
                "ModifyDownload",
                "ModifyComposerUrls",
                "ModifyLogo",
                "ModifyBiometrics",
                "ManageScheduleDays", 
                "ViewSchedule");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableHeader),
                "Apps section should be displayed");
            Assert.IsTrue(IsElementFound(PlacesPage.ScheduleSectionRow1),
                "Schedule section should be displayed");
            Assert.IsTrue(IsElementFound(PlacesPage.FaceApiKeyReadOnly),
                "Face API Key field should be shown");

            EditForm();
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.UploadDayButton),
                "Upload Day button should be present in footer");

            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.EnabledCheckBox),
                "Schedule section: Enabled checkbox is shown");
            Click(PlacesPage.EnabledCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.EnabledCheckBox),
                "Schedule section: Enabled checkbox should be read-only");

            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScheduleSectionDetailsAddDayButton),
                @"Schedule section: '+ Add Day' button should not be shown");

            Click(PlacesPage.ScheduleSectionRow1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScheduleSectionDetailsDayReadOnly),
                "Schedule section: row 1, Day drop-down should be read-only");

            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScheduleSectionDetailsAddUpButton)
                    && IsElementNotFoundQuickly(PlacesPage.ScheduleSectionDetailsAddDownButton),
                @"Schedule section: row 1, '+^' and 'v+' buttons should not be shown");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "GetAnyScheduleEntry",
                "ModifyAnyScheduleEntry",
                "CreateScheduleEntry",
                "DeleteAnyScheduleEntry",
                "ViewVolume",
                "ViewInteraction",
                "ViewLivePreview",
                "ViewDiagnostics",
                "ModifyVolume",
                "ViewRecordingQuality",
                "ViewRecordingConditions",
                "ViewDownload",
                "ViewComposerUrls",
                "ModifyInteraction",
                "ViewLogo",
                "ViewBiometrics",
                "ModifyLivePreview",
                "ModifyDiagnostics",
                "ModifyRecordingQuality",
                "ModifyRecordingConditions",
                "ModifyDownload",
                "ModifyComposerUrls",
                "ModifyLogo",
                "ModifyBiometrics",
                "ManageScheduleDays",
                "ViewSchedule",
                "ChangeSchedule");
            RefreshPage();
            EditForm();
            Click(PlacesPage.EnabledCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.EnabledCheckBox),
                "Schedule section: Enabled checkbox should be available for edit");

            Click(PlacesPage.ScheduleSectionRow1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScheduleSectionDetailsDayDropDown),
                "Schedule section: row 1, Day drop-down should be available for edit");

            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScheduleSectionDetailsAddUpButton)
                          && IsElementFoundQuickly(PlacesPage.ScheduleSectionDetailsAddDownButton),
                @"Schedule section: row 1, both '+^' and 'v+' buttons should be shown");

            Click(PlacesPage.ScheduleSectionDetailsAddUpButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScheduleSectionDetailsAppDropDown)
                    && IsElementFoundQuickly(PlacesPage.ScheduleSectionDetailsTimeStart1HoursDropDown),
                @"Schedule section: row 1, 2 Time and 1 App drop-downs should appear on '+^' button press");
            
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place should be saved successfully");
        }

        public void RT10070_PlacesDeviceDropDown()
        {
            Place place1 = null;
            AppResponse app1 = null;
            CurrentTenant = TenantTitle.nolang;
            Parallel.Invoke(
                () => place1 = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0, isAssignDevice: true, 
                    isCreateNewPlace: true),
                () => _role = UserDirectoryApi.SetRolePermissions(
                    _role,
                    "Login",
                    "CreateLocation",
                    "GetAnyLocation",
                    "ModifyOwnLocation",
                    "DeleteOwnLocation",
                    "UndeleteOwnLocation",
                    "ModifyAnyLocation",
                    "DeleteAnyLocation",
                    "UndeleteAnyLocation",
                    "DevicesManagement"),
                () => app1 = AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion)
            );
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.nolang);
            TestStart(TestConfig.LoginUrl, false);
            Parallel.Invoke(
                () => Login(TestConfig.PermissionsUser, false),
                () => place1 = AssignAppToPlace(place1, app1, null, null, isAddSilently: true)
            );

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.WindowsWorkstationDetailsButtonDisabled),
                "Devices details button should be disabled");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New place should be saved");
            var newPlaceId = GetEntityIdFromUrl();

            OpenEntityPage(place1);
            EditForm();
            Assert.IsTrue(IsElementFound(PlacesPage.WindowsWorkstationDetailsButtonDisabled),
                @"WW field should contain disabled '...' button");
            Assert.IsTrue(AreElementsContainText(PlacesPage.WindowsWorkstation, place1.Device.Name),
                "WW field should contain 1 device assigned previously by admin");
            OpenDevicesFromPlace();
            Assert.IsTrue(CountElements(Devices.TableRow) == 1,
                "Device dialog should contain 1 device assigned previously by admin");          

            Click(Devices.TableRow);
            Assert.IsTrue(IsElementFound(Devices.RackNumberReadOnly),
                "Modal dialog with device properties should be displayed and be read-only");

            Assert.IsTrue(IsElementFoundQuickly(Devices.PcNumberReadOnly), 
                "PC Number field should be read-only");

            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice),
                "Cancel button should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.SubmitButton),
                "Submit button should not be shown");

            Click(Devices.CancelButtonDevice);
            Click(Devices.CancelButton);

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "AssignNewStation",
                "DevicesManagement");
            RefreshPage();
            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(CountElements(Devices.TableRow) > 1,
                "Devices dialog should show all devices");
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.ClearSelectionButton),
                "Devices dialog should hide Clear Selection button");

            Click(string.Format(Devices.TableRowByText, "DQZJF065"));  
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New place should be saved");
            
            OpenEntityPage<Place>(newPlaceId);
            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(CountElements(Devices.TableRow) > 1,
                "Devices dialog should show all devices");
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.ClearSelectionButton),
                "Devices dialog should hide Clear Selection button");

            Click(string.Format(Devices.TableRowByText, "DQZJF065"));
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New place should be saved");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "AssignNewStation",
                "UnassignStation",
                "DevicesManagement");
            RefreshPage();
            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(CountElements(Devices.TableRow) > 1,
                "Devices dialog should show all devices");
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.ClearSelectionButton),
                "Devices dialog should hide Clear Selection button");

            OpenDevicesFromMenu();
            Click(string.Format(Devices.TableRowByText, "DQZJF065"));
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice),
                "Cancel button should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.SubmitButtonDevice),
                "Submit button should not be shown");

            Click(Devices.CancelButtonDevice);
            Click(Devices.CancelButton);

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "AssignNewStation",
                "UnassignStation",
                "CreateDevice", 
                "ModifyDevice",
                "DevicesManagement");
            RefreshPage();
            EditForm();
            OpenDevicesFromMenu();
            Click(string.Format(Devices.TableRowByText, "DQZJF065"));
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice),
                "Cancel button should be shown");
            Assert.IsTrue(IsElementFoundQuickly(Devices.SubmitButtonDevice),
                "Submit button should be shown");

            var rackNo = $"{RandomNumberWord}";
            WaitTime(1);
            SendText(Devices.RackNumber, rackNo);
            WaitTime(1);
            Click(Devices.SubmitButtonDevice);
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.RackNumber) 
                    && IsElementNotFoundQuickly(Devices.RackNumberReadOnly),
                "Device properties modal dialog should be closed on Submit button");

            Assert.IsTrue(CountElements(Devices.TableRow) > 1
                          && IsElementFoundQuickly(string.Format(Devices.TableRowByText, rackNo))
                          && IsElementFoundQuickly(Devices.ClearSelectionButton)
                          && IsElementNotFoundQuickly(Devices.AddButton),
                "Devices dialog should show device list including <empty>, but no <New> option");

            Click(Devices.ClearSelectionButton);
            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, string.Empty),
                "WW field should be empty");
        }

        public void RT10080_Apps()
        {
            AppResponse app2 = null;
            _role = UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyApp");
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.nolang, TenantTitle.manylang);
            CurrentTenant = TenantTitle.manylang;
            AddAppPlayer();
            AppApi.DeleteApps(true, new [] { AppTitle.ComposerHq1, AppTitle.ComposerHq2 }, CurrentTenant);
            var app = AddAppComposerHq1();
            Parallel.Invoke(
                () => app = AddAppComposerHq1(AppStatus.Available, true),
                () => app2 = AddAppIbeacon(TestConfig.IbeaconAppVersions[1])
            );
            TestStart(TestConfig.LoginUrl, false);
            Login(TestConfig.PermissionsUser, CurrentTenant);
            AppApi.DeleteAppVersion(app, TestConfig.ComposerHqApp1Version2, false);

            Assert.IsTrue(IsElementFound(PageHeader.PageAppsButton), 
                "Tab Apps should be displayed in page header");
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.PagePlacesButton),
                "Tab Places should not be displayed in page header");
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.PageItemsButton),
                "Tab Items should not be displayed in page header");

            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.AddAppInAppsButton),
                "+Add New button should not be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Show Deleted button should be displayed in page footer");

            PressKeys(Keys.Insert);
            Assert.IsFalse(FileManager.IsWindowOpen(FileManager.WindowTitle),
                "Browser open file dialog should not be opened on Insert key press");

            OpenEntityPage(app);
            Assert.IsTrue(IsElementNotFound(PageFooter.AddVersionButton),
                "+Add Version button should not be displayed in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DistributeSubMenu),
                "Distribute submenu should not be displayed in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.EditButton),
                "Edit button should not be displayed in page footer");

            Click(string.Format(AppsPage.Version, TestConfig.ComposerHqApp1Version));
            Assert.IsTrue(IsElementNotFound(PageFooter.AddVersionButton),
                "+Add Version button should not be displayed in page footer");

            Click(string.Format(AppsPage.TableRowByText, TestConfig.ComposerHqApp1Version));
            Assert.IsTrue(IsElementNotFound(PageFooter.AddVersionButton),
                "+Add Version button should not be displayed in page footer");
            Assert.IsTrue(IsElementNotFound(PageFooter.DeleteButton),
                "Delete button should not be displayed in page footer");

            PressKeys(Keys.Delete);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.DeleteConfirmationDialog),
                "Delete confirmation dialog should not be opened on Delete key press");

            Click(string.Format(AppsPage.TableRowByText, TestConfig.ComposerHqApp1Version2));
            Assert.IsTrue(IsElementNotFound(PageFooter.AddVersionButton),
                "+Add Version button should not be displayed in page footer");
            Assert.IsTrue(IsElementNotFound(PageFooter.RestoreButton),
                "Restore button should not be displayed in page footer");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyApp",
                "ImportApp", 
                "ModifyOwnApp");
            OpenAppsPage();
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageFooter.AddAppInAppsButton),
                "+Add New button should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Show Deleted button should be displayed in page footer");

            OpenEntityPage(app);
            Assert.IsTrue(IsElementNotFound(PageFooter.AddVersionButton),
                "+Add Version button should not be displayed in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DistributeSubMenu),
                "Distribute submenu should not be displayed in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.EditButton),
                "Edit button should not be displayed in page footer");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var fileName = Path.Combine(TestConfig.ComposerHqApp1Folder, TestConfig.ComposerHqApp1File);
            FileManager.Upload(fileName);
            Assert.IsTrue(IsElementFound(AppsPage.YouDontHaveCorrespondingPermissionDialog),
                @"Error dialog that you don't have permission should be displayed");

            Click(AppsPage.OkButton);

            Click(PageFooter.AddAppInAppsButton);
            var fileName2 = Path.Combine(TestConfig.ComposerHqApp2Folder, TestConfig.ComposerHqApp2File);
            FileManager.Upload(fileName2);
            Assert.IsTrue(IsEditMode(), "Uploaded app should be in edit mode");

            SendText(AppsPage.Description, $"Auto test {RandomNumber}");
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Submitted app should be in view mode");

            RefreshPage();
            Assert.IsTrue(IsElementNotFound(PageFooter.AddVersionButton),
                "+Add Version button should not be displayed in page footer");
            Assert.IsTrue(IsElementNotFound(PageFooter.DistributeSubMenu),
                "Distribute submenu should not be displayed in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.EditButton),
                "Edit button should be displayed in page footer");

            Click(string.Format(AppsPage.Version, TestConfig.ComposerHqApp2Version));
            Click(string.Format(AppsPage.TableRowByText, TestConfig.ComposerHqApp2Version));
            Assert.IsTrue(IsElementNotFound(PageFooter.AddVersionButton),
                "+Add Version button should not be displayed in page footer");
            Assert.IsTrue(IsElementNotFound(PageFooter.DeleteButton),
                "Delete button should not be displayed in page footer");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(fileName2);
            Assert.IsTrue(IsElementFound(AppsPage.YouDontHaveCorrespondingPermissionDialog),
                @"Error dialog that you don't have permission should be displayed");

            Click(AppsPage.OkButton);
            
            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyApp",
                "ImportApp",
                "ModifyOwnApp",
                "ImportAppVersion", 
                "DeleteOwnAppVersion", 
                "UndeleteOwnAppVersion");
            OpenAppsPage();
            RefreshPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(fileName2);
            Assert.IsTrue(IsEditMode(), "Composer HQ 2 app should be uploaded and in edit mode");

            OpenEntityPage(app);
            Assert.IsTrue(IsElementNotFound(PageFooter.AddVersionButton),
                "+Add Version button should be not displayed in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DistributeSubMenu),
                "Distribute submenu should be not displayed in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.EditButton),
                "Edit button should be not displayed in page footer");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(fileName);
            Assert.IsTrue(IsElementNotFound(AppsPage.YouDontHaveCorrespondingPermissionDialog),
                @"Error dialog that you don't have permission should be not displayed");
            Assert.IsTrue(IsEditMode(), 
                "Composer HQ 1 app version should be uploaded and in edit mode");
            SubmitForm();

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyApp",
                "ImportApp",
                "ModifyOwnApp",
                "ImportAppVersion",
                "DeleteOwnAppVersion",
                "UndeleteOwnAppVersion",
                "ModifyAnyApp", 
                "DeleteAnyAppVersion", 
                "UndeleteAnyAppVersion");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageFooter.AddVersionButton),
                "+Add Version button should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be displayed in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DistributeSubMenu),
                "Distribute submenu should not be displayed in page footer");

            EditForm();
            SendText(AppsPage.Description, $"Auto test {RandomNumber}");
            SubmitForm();
            Assert.IsTrue(IsViewMode(),
                "Composer HQ 1 app should be modified and saved");

            Click(string.Format(AppsPage.Version, TestConfig.ComposerHqApp1Version));
            Click(string.Format(AppsPage.TableRowByText, TestConfig.ComposerHqApp1Version));
            Assert.IsTrue(IsElementFound(PageFooter.AddVersionButton),
                "+Add Version button should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be displayed in page footer");

            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Assert.IsTrue(IsElementNotFound(string.Format(
                    AppsPage.TableRowByText, TestConfig.ComposerHqApp1Version)),
                $"App version {TestConfig.ComposerHqApp1Version} should be deleted completely");

            Click(string.Format(AppsPage.TableRowByText, TestConfig.ComposerHqApp1Version2));
            Assert.IsTrue(IsElementFound(PageFooter.AddVersionButton),
                "+Add Version button should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.RestoreButton),
                "Restore button should be displayed in page footer");

            Click(PageFooter.RestoreButton);
            Assert.IsTrue(IsElementFound(string.Format(
                    AppsPage.TableRowVersionStatus, TestConfig.ComposerHqApp1Version2, StatusAvailable)),
                $"App version {TestConfig.ComposerHqApp1Version2} should have status {StatusAvailable}");
            //SubmitForm();

            OpenEntityPage(app2);
            Assert.IsTrue(IsElementFound(PageFooter.AddVersionButton),
                "+Add Version button should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be displayed in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DistributeSubMenu),
                "Distribute submenu should not be displayed in page footer");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyApp",
                "ImportApp",
                "ModifyOwnApp",
                "ImportAppVersion",
                "DeleteOwnAppVersion",
                "UndeleteOwnAppVersion",
                "ModifyAnyApp",
                "DeleteAnyAppVersion",
                "UndeleteAnyAppVersion",
                "DistributeApp");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageFooter.AddVersionButton),
                "+Add Version button should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DistributeSubMenu),
                "Distribute submenu should be displayed in page footer");

            MouseOver(PageFooter.DistributeSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DistributeAppButton),
                "App button in Distribute menu should be displayed in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DistributeMasterDataButton),
                "Master Data button in Distribute menu should not be displayed in page footer");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyApp",
                "ImportApp",
                "ModifyOwnApp",
                "ImportAppVersion",
                "DeleteOwnAppVersion",
                "UndeleteOwnAppVersion",
                "ModifyAnyApp",
                "DeleteAnyAppVersion",
                "UndeleteAnyAppVersion",
                "DistributeApp",
                "DistributeAppMasterData");
            RefreshPage();

            Assert.IsTrue(IsElementFound(PageFooter.AddVersionButton),
                "+Add Version button should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DistributeSubMenu),
                "Distribute submenu should be displayed in page footer");

            MouseOver(PageFooter.DistributeSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DistributeAppButton),
                "App button in Distribute menu should be displayed in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DistributeMasterDataButton),
                "Master Data button in Distribute menu should be displayed in page footer");
        }

        public void RT10090_Items()
        {
            Item itemCar = null, itemPorscheCar = null, itemUsedCar = null, itemEmailTemplate = null;
            _role = UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion");
            UserDirectoryApi.AssignRolesToUser(
                TestConfig.PermissionsUser, new [] { UserRole.CxmAdmin }, TenantTitle.onelang);
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.manylang);
            CurrentTenant = TenantTitle.onelang;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            CurrentTenant = TenantTitle.manylang;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            Parallel.Invoke(
                () => itemCar = AddItem(ItemType.Car),
                () => itemPorscheCar = AddItem(ItemType.PorscheCar),
                () => itemUsedCar = AddItem(ItemType.UsedCar),
                () => itemEmailTemplate = AddItem(ItemType.EmailTemplate)
            );
            TestStart(TestConfig.LoginUrl, false);
            Login(TestConfig.PermissionsUser, CurrentTenant);

            Assert.IsTrue(IsElementFound(PageHeader.PageAppsButton),
                "Tab Apps should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PagePlacesButton),
                "Tab Places should be displayed in page header");
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.PageItemsButton)
                          && IsElementNotFoundQuickly(PageHeader.PageCarsButton),
                "Tab Items (or Cars) should not be displayed in page header");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion",
                "ChangePorscheCar",
                "ViewPorscheCar");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageHeader.PageAppsButton),
                "Tab Apps should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PagePlacesButton),
                "Tab Places should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PagePorscheCarButton),
                "Tab Porsche Car should be displayed in page header");

            Click(PageHeader.PagePorscheCarButton);
            Assert.IsTrue(IsElementFound(
                    string.Format(ItemsPage.TableRowByText, itemPorscheCar.LangJsonData.EnJson.Title)),
                "Porsche car type item(s) should be shown. Now only Porsche Car type allowed.");
            Assert.IsTrue(IsElementNotFound(
                    string.Format(ItemsPage.TableRowByText, itemEmailTemplate.JsonDataTitle)),
                "Email Template items should not be shown. Only Porsche Car type allowed.");
            Assert.IsTrue(IsElementNotFound(
                    string.Format(ItemsPage.TableRowByText, itemUsedCar.LangJsonData.EnJson.Title)),
                "Used Car items should not be shown. Only Porsche Car type allowed.");
            Assert.IsTrue(IsElementNotFound(
                    string.Format(ItemsPage.TableRowByText, itemCar.LangJsonData.EnJson.Title)),
                "Car items should not be shown. Only Porsche Car type allowed.");

            Click(PageFooter.AddItemButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.TypeDropDown), "Type drop-down should not be shown");

            SendText(ItemsPage.Title, $"Auto test {RandomNumber}");
            SendText(ItemsPage.Vin, $"{RandomNumber}22");
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New Porsche Car item should be saved");

            Click(PageHeader.PagePorscheCarButton);
            Assert.IsTrue(IsElementNotFound(PageHeader.FilterDropDown),
                "Filter drop-down should not be present");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddItemButton),
                "+Add New button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ImportSubMenu),
                "Import submenu should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Show Deleted button should be shown in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.FollowButton),
                "Follow button should not be shown in page footer");

            OpenEntityPage(itemPorscheCar);
            Assert.IsTrue(IsElementFound(PageFooter.DeleteButton),
                "Delete button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be shown in page footer");

            EditForm();
            SendText(ItemsPage.Vin, $"{RandomNumber}33");
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Porsche Car item should be saved");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion",
                "ChangePorscheCar",
                "ViewPorscheCar",
                "ViewPorscheUsedCar");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageHeader.PageAppsButton),
                "Tab Apps should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PagePlacesButton),
                "Tab Places should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PageItemsButton),
                "Tab Items should be displayed in page header");

            OpenItemsPage();
            Assert.IsTrue(IsElementFound(
                    string.Format(ItemsPage.TableRowByText, itemPorscheCar.LangJsonData.EnJson.Title)),
                "Porsche car type item(s) should be shown");
            Assert.IsTrue(IsElementFound(
                    string.Format(ItemsPage.TableRowByText, itemUsedCar.LangJsonData.EnJson.Title)),
                "Used Car type item(s) should be shown");
            Assert.IsTrue(IsElementNotFound(
                    string.Format(ItemsPage.TableRowByText, itemCar.LangJsonData.EnJson.Title)),
                "Car type item(s) should not be shown. Only Porsche Car and Used Car types allowed.");
            Assert.IsTrue(IsElementNotFound(
                    string.Format(ItemsPage.TableRowByText, itemEmailTemplate.JsonDataTitle)),
                "Email Template items should not be shown. Only Porsche Car and Used Car types allowed.");

            Assert.IsTrue(IsElementFound(PageHeader.FilterDropDown),
                "Filter drop-down should be present from now");

            ClickUntilShown(PageHeader.FilterDropDown, CommonElement.DropDownOptionList);
            var allowedTypes = new []
            {
                ItemTypeAllTypes,
                ItemTypeCars,
                ItemTypePorscheCar,
                ItemTypeUsedCar
            };
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), allowedTypes),
                "Type drop-down should contain: " + string.Join(", ", allowedTypes));

            Click(PageFooter.AddItemButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.TypeDropDown), "Type drop-down should not be shown");

            OpenEntityPage(itemUsedCar);
            Assert.IsTrue(IsElementNotFound(PageFooter.DeleteButton),
                "Delete button should not be shown in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should not be shown in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.EditButton),
                "Edit button should not be shown in page footer");

            PressKeys(Keys.Delete);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.DeleteConfirmationDialog),
                "Delete confirmation dialog should not be opened on Delete key press");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion",
                "ChangePorscheCar",
                "ViewPorscheCar",
                "ViewPorscheUsedCar",
                "ModifyPorscheUsedCar");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageFooter.DeleteButton),
                "Delete button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be shown in page footer");

            EditForm();
            Assert.IsTrue(IsElementFound(PageFooter.ImportButton),
                "Import button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be shown in page footer");

            if (IsElementNotFoundQuickly(ItemsPage.PictureSectionImage))
            {
                Click(ItemsPage.PicturesSectionAddButton);
                Click(ItemsPage.PicturesSectionUploadButton);
                FileManager.Upload(TestConfig.ImageCar);
            }
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Used Car item should be saved");

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            Assert.IsTrue(IsElementFound(ItemsPage.TypeDropDown), "Type drop-down should be shown");

            ClickUntilShown(ItemsPage.TypeDropDown, CommonElement.DropDownOptionList);
            allowedTypes = new []
            {
                ItemTypePorscheCar,
                ItemTypeUsedCar
            };
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), allowedTypes),
                "Type drop-down should contain: " + string.Join(", ", allowedTypes));

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion",
                "ChangePorscheCar",
                "ViewPorscheCar",
                "ViewPorscheUsedCar",
                "ModifyPorscheUsedCar",
                "ViewEmailTemplate", 
                "FollowItems");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageHeader.PageAppsButton),
                "Tab Apps should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PagePlacesButton),
                "Tab Places should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PageItemsButton),
                "Tab Items should be displayed in page header");

            OpenItemsPage();
            allowedTypes = new []
            {
                ItemTypeAllTypes,
                ItemTypeCars,
                ItemTypePorscheCar,
                ItemTypeUsedCar,
                ItemTypeEmailTemplate
            };
            ClickUntilShown(PageHeader.FilterDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), allowedTypes),
                "Filter drop-down should contain: " + string.Join(", ", allowedTypes));

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddItemButton),
                "+Add New button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ImportSubMenu),
                "Import submenu should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Show Deleted button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FollowButtonInactive),
                "Follow button should be shown and inactive in page footer");

            DropDownSelect(PageHeader.FilterDropDown, ItemTypeEmailTemplate, false);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddItemButton),
                "+Add New button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ImportSubMenu),
                "Import submenu should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Show Deleted button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FollowButton),
                "Follow button should be shown and active in page footer");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion",
                "ViewEmailTemplate",
                "FollowItems");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageHeader.PageAppsButton),
                "Tab Apps should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PagePlacesButton),
                "Tab Places should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PageEmailTemplateButton),
                "Tab Email Template should be displayed in page header");

            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.AddItemButton),
                "+Add New button should not be shown in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.ImportSubMenu)
                    && IsElementNotFoundQuickly(PageFooter.ImportButton),
                "Import should not be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Show Deleted button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FollowButton),
                "Follow button should be shown and active in page footer");

            PressKeys(Keys.Insert);
            Assert.IsTrue(IsPageContainsUri(TestConfig.ItemsUri),
                "New item form should not open on Insert key press");

            Assert.IsTrue(IsElementNotFound(PageHeader.FilterDropDown),
                "Filter drop-down should not be present");

            ChangeTenant(TenantTitle.onelang);
            Assert.IsTrue(IsElementFound(PageHeader.PageAppsButton),
                "Tab Apps should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PagePlacesButton),
                "Tab Places should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PageItemsButton),
                "Tab Items should be displayed in page header");

            OpenItemsPage();
            allowedTypes = new []
            {
                ItemTypeAllTypes,
                ItemTypeCars,
                ItemTypePorscheCar,
                ItemTypeUsedCar,
                ItemTypePdfCar,
                ItemTypeCustomerProfile,
                ItemTypeEmailTemplate,
                ItemTypeEmployee,
                ItemTypeEventOrPromotion,
                ItemTypePoi,
                ItemTypeServiceBooking,
                ItemTypeSalesAppointment,
                ItemTypeTestDrive
            };
            ClickUntilShown(PageHeader.FilterDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), allowedTypes),
                "Filter drop-down should contain: " + string.Join(", ", allowedTypes) +
                ". But contains: " +
                string.Join(", ", GetElementsText(CommonElement.DropDownOptionList)));

            Click(PageFooter.AddItemButton);
            allowedTypes = new []
            {
                ItemTypeCars,
                ItemTypePorscheCar,
                ItemTypeUsedCar,
                ItemTypePdfCar,
                ItemTypeCustomerProfile,
                ItemTypeEmailTemplate,
                ItemTypeEmployee,
                ItemTypeEventOrPromotion,
                ItemTypePoi,
                ItemTypeServiceBooking,
                ItemTypeSalesAppointment,
                ItemTypeTestDrive
            };
            ClickUntilShown(ItemsPage.TypeDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), allowedTypes),
                "Type drop-down should contain: " + string.Join(", ", allowedTypes) +
                ". But contains: " +
                string.Join(", ", GetElementsText(CommonElement.DropDownOptionList)));
        }

        public void RT10100_ItemsAnnouncement()
        {
            Item itemCar = null, itemPorscheCar = null, itemEvent = null;
            CurrentTenant = TenantTitle.manylang;
            var app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            _role = UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion",
                "ViewCar",
                "ViewAnnouncement",
                "ModifyAnnouncement");
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.manylang);
            Parallel.Invoke(
                () => itemCar = AddItem(ItemType.Car),
                () => itemPorscheCar = AddItem(ItemType.PorscheCar),
                () => itemEvent = AddItem(ItemType.EventOrPromotion),
                () => AddItem(ItemType.EmailTemplate)
            );
            TestStart(TestConfig.LoginUrl, false);
            Login(TestConfig.PermissionsUser, false);

            Assert.IsTrue(IsElementFound(PageHeader.PageAppsButton),
                "Tab Apps should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PagePlacesButton),
                "Tab Places should be displayed in page header");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PageItemsButton),
                "Tab Items should be displayed in page header");

            OpenItemsPage();
            Assert.IsTrue(IsElementNotFound(
                    string.Format(ItemsPage.TableRowByText, itemPorscheCar.LangJsonData.EnJson.Title)),
                "Porsche car type item(s) should not be shown");
            Assert.IsTrue(IsElementFound(
                    string.Format(ItemsPage.TableRowByText, itemEvent.LangJsonData.EnJson.Title)),
                "Event Or Promotion type item(s) should be shown");
            Assert.IsTrue(IsElementFound(
                    string.Format(ItemsPage.TableRowByText, itemCar.LangJsonData.EnJson.Title)),
                "Car type item(s) should not be shown. Only Porsche Car and Used Car types allowed.");

            var allowedTypes = new []
            {
                ItemTypeAllTypes,
                ItemTypeCars,
                ItemTypeEventOrPromotion
            };
            ClickUntilShown(PageHeader.FilterDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), allowedTypes),
                "Filter drop-down should contain: " + string.Join(", ", allowedTypes));

            Click(PageFooter.AddItemButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.TypeDropDown),
                "Type drop-down should not be shown now");

            SendText(ItemsPage.Title, $"Auto test {RandomNumber}");
            SendText(ItemsPage.TitleForPush, $"Auto test {RandomNumber}");
            SendText(ItemsPage.MessageForPush, $"Auto test {RandomNumber}");
            SendText(ItemsPage.Description, $"Auto test {RandomNumber}");
            Click(ItemsPage.PictureUploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New Event Or Promotion item should be saved");

            Assert.IsTrue(IsElementFound(PageFooter.DeleteButton),
                "Delete button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be shown in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.PushButton),
                "Push button should not be shown in page footer");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion",
                "ViewCar",
                "ViewAnnouncement",
                "ModifyAnnouncement",
                "PushItems");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageFooter.DeleteButton),
                "Delete button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.PushButton),
                "Push button should be shown in page footer");

            EditForm();
            Click(ItemsPage.PictureImage);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog), "Media library should be opened");
            Assert.IsTrue(CountElements(MediaLibrary.TableRows) == 1, 
                "Media library should contain 1 picture that have been uploaded earlier");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.UploadButton),
                "Upload button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ClearSelectionButton),
                "Clear Selection button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShareSubMenu),
                "Share submenu should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ModifySubMenu),
                "Modify submenu should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be shown in page footer");

            MouseOver(PageFooter.ShareSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CopyUrlButton),
                "Copy URL submenu item should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DownloadButton),
                "Download submenu item should be shown in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DistributeButton),
                "Distribute submenu item button should not be shown in page footer");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion",
                "ViewCar",
                "ViewAnnouncement",
                "ModifyAnnouncement",
                "PushItems",
                "DistributeAsset", 
                "ViewAllUploadedAssets", 
                "ModifyAnyApp");
            RefreshPage();
            EditForm();
            Click(ItemsPage.PictureImage);
            Assert.IsTrue(CountElements(MediaLibrary.TableRows) >= 1,
                "Media library should contain many pictures that have been uploaded by all users");

            MouseOver(PageFooter.ShareSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CopyUrlButton),
                "Copy URL submenu item should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DownloadButton),
                "Download submenu item should be shown in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DistributeButton),
                "Distribute submenu item button should be hidden in page footer");

            Click(PageFooter.CancelButton); 
            OpenEntityPage(app);
            EditForm();
            Click(AppsPage.TextsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.WelcomeNewCustomerDetailsButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorTheAreNoItemsDialog),
                @"Dialog window 'There are no items to be added' should be shown");

            Click(AppsPage.OkButton);

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "GetAnyLocation",
                "GetAnyApp",
                "ImportApp",
                "ImportAppVersion",
                "ViewCar",
                "ViewAnnouncement",
                "ModifyAnnouncement",
                "PushItems",
                "DistributeAsset",
                "ViewAllUploadedAssets",
                "ModifyAnyApp",
                "ViewEmailTemplate");
            RefreshPage();
            OpenEntityPage(app);
            EditForm();
            Click(AppsPage.TextsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.WelcomeNewCustomerDetailsButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.SelectItemNewDialog),
                @"Dialog window 'Items' should be shown");
        }

        public void RT10200_HideSchemaElements()
        {
            CurrentTenant = TenantTitle.onelang;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            UserDirectoryApi.AssignRolesToUser(
                TestConfig.PermissionsUser, new [] { UserRole.CxmAdmin }, TenantTitle.onelang);
            _role = UserDirectoryApi.SetRolePermissions(
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
                "UndeleteOwnAppVersion",
                "UndeleteAnyAppVersion",
                "CreateModel",
                "ModifyModel",
                "ChangePorscheCar",
                "ModifyCar",
                "ViewCar",
                "ViewPorscheCar",
                "ViewPorscheCarLeasing");
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.permissions);
            CurrentTenant = TenantTitle.permissions;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            CurrentUser = TestConfig.PermissionsUser;
            TestStart();

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            var title = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Title, title);
            SendText(ItemsPage.Vin, $"{RandomNumber}11");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LeasingButton),
                "Leasing group button should be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.FinanceOfferButton),
                "Finance Offer group button should be not displayed");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be in view mode on submit");
            var itemId = GetEntityIdFromUrl();

            ChangeTenant(TenantTitle.onelang);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            SendText(ItemsPage.Title, $"Auto test {RandomNumber}");
            SendText(ItemsPage.Vin, $"{RandomNumber}11");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LeasingButton),
                "Leasing group button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.FinanceOfferButton),
                "Finance Offer group button should be displayed");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be in view mode on submit");
            var itemId2 = GetEntityIdFromUrl();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(
                TestConfig.IbeaconAppFolder,
                TestConfig.IbeaconAppFile,
                TestConfig.IbeaconAppVersions[4]); // special iBeacon test app
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsEditMode(),
                $"Uploaded test app v.{TestConfig.IbeaconAppVersions[4]} should be in edit mode");
            var appId = GetEntityIdFromUrl();
            RefreshPage();
            OpenEntityPage<Item>(itemId2);
            Assert.IsTrue(IsElementFound(ItemsPage.LeasingButton),
                "Leasing group button should be displayed");
            Assert.IsTrue(IsElementFound(ItemsPage.FinanceOfferButton),
                "Finance Offer group button should be displayed");
            Assert.IsTrue(IsElementNotFound(ItemsPage.TeaserCheckBox),
                "Teaser check box should be not displayed");

            OpenEntityPage<AppResponse>(appId);
            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowByText, TestConfig.IbeaconAppVersions[4]));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            RefreshPage();
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            Assert.IsTrue(IsElementFound(ItemsPage.LeasingButton),
                "Leasing group button should be displayed");
            Assert.IsTrue(IsElementFound(ItemsPage.FinanceOfferButton),
                "Finance Offer group button should be displayed");
            Assert.IsTrue(IsElementNotFound(ItemsPage.TeaserCheckBox),
                "Teaser check box should be not displayed");

            Click(PageFooter.CancelButton);
            ChangeTenant(TenantTitle.permissions);
            OpenEntityPage<Item>(itemId);
            Assert.IsTrue(IsElementFound(ItemsPage.LeasingButton),
                "Leasing group button should be displayed");
            Assert.IsTrue(IsElementNotFound(ItemsPage.FinanceOfferButton),
                "Finance Offer group button should be not displayed");
            Assert.IsTrue(IsElementNotFound(ItemsPage.TeaserCheckBox),
                "Teaser check box should be not displayed");

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
                "UndeleteOwnAppVersion",
                "UndeleteAnyAppVersion",
                "CreateModel",
                "ModifyModel",
                "ChangePorscheCar",
                "ModifyCar",
                "ViewCar",
                "ViewPorscheCar",
                "ViewPorscheCarLeasing", 
                "TestViewTeaser");
            RefreshPage();
            Assert.IsTrue(IsElementFound(ItemsPage.TeaserCheckBox), "Teaser check box should be displayed");

            EditForm();
            Click(ItemsPage.TeaserCheckBox);
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.TeaserCheckBox), "Teaser check box should be On");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be in view mode on submit");

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
                "UndeleteOwnAppVersion",
                "UndeleteAnyAppVersion",
                "CreateModel",
                "ModifyModel",
                "ModifyCar",
                "ViewCar",
                "ViewPorscheCarLeasing",
                "TestViewTeaser");
            OpenItemsPage();
            RefreshPage();
            Assert.IsTrue(IsElementNotFound(string.Format(ItemsPage.TableRowByText, title)),
                $@"Item with title '{title}' should be not displayed on Items list page");

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
                "UndeleteOwnAppVersion",
                "UndeleteAnyAppVersion",
                "CreateModel",
                "ModifyModel",
                "ChangePorscheCar",
                "ModifyCar",
                "ViewCar",
                "ViewPorscheCar",
                "ViewPorscheCarLeasing");
            RefreshPage();
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            pathFile = FileManager.GetFileByVersion(
                TestConfig.IbeaconAppFolder,
                TestConfig.IbeaconAppFile,
                TestConfig.IbeaconAppVersions[3]); 
            FileManager.Upload(pathFile);
            Click(PageFooter.CancelButton);
            OpenEntityPage<Item>(itemId);
            Assert.IsTrue(IsElementFound(ItemsPage.LeasingButton),
                "Leasing group button should be displayed");
            Assert.IsTrue(IsElementNotFound(ItemsPage.FinanceOfferButton),
                "Finance Offer group button should be not displayed");
            Assert.IsTrue(IsElementNotFound(ItemsPage.TeaserCheckBox),
                "Teaser check box should be not displayed");
        }

        public void RT10210_AppMasterData()
        {
            CurrentTenant = TenantTitle.onelang;
            AddAppIpadPlayer();
            UserDirectoryApi.AssignRolesToUser(
                TestConfig.PermissionsUser, new [] { UserRole.CxmAdmin }, TenantTitle.onelang);
            _role = UserDirectoryApi.SetRolePermissions(
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
                "UndeleteOwnAppVersion",
                "UndeleteAnyAppVersion",
                "CreateModel",
                "ModifyModel");
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.permissions);
            CurrentTenant = TenantTitle.permissions;
            AddAppIpadPlayer();
            CurrentUser = TestConfig.PermissionsUser;
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.TestElementPermissionDptFolder,
                TestConfig.TestElementPermissionDptFile, TestConfig.ElementPermissionDptVersions[0]);
            FileManager.Upload(pathFile);
            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsEditMode(), 
                $"App v.{TestConfig.ElementPermissionDptVersions[0]} should be not saved");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.Market),
                $"App v.{TestConfig.ElementPermissionDptVersions[0]} should have not Market field");

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
                "UndeleteOwnAppVersion",
                "UndeleteAnyAppVersion",
                "CreateModel",
                "ModifyModel",
                "PorscheDPTviewMarket");
            RefreshPage();
            Assert.IsTrue(IsElementFound(AppsPage.Market),
                $"App v.{TestConfig.ElementPermissionDptVersions[0]} should have Market field");

            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsEditMode(),
                $"App v.{TestConfig.ElementPermissionDptVersions[0]} should be not saved");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMarket),
                "Validation error for Market field should be shown");

            SendText(AppsPage.Market, "AA");
            SubmitForm();
            Assert.IsTrue(IsViewMode(),
                $"App v.{TestConfig.ElementPermissionDptVersions[0]} should be saved");

            EditForm();
            Click(PageFooter.AddVersionButton);
            pathFile = FileManager.GetFileByVersion(TestConfig.TestElementPermissionDptFolder,
                TestConfig.TestElementPermissionDptFile, TestConfig.ElementPermissionDptVersions[1]);
            FileManager.UploadAsBackgroundTask(pathFile);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.InteriorColorDisclaimer),
                $"App v.{TestConfig.ElementPermissionDptVersions[1]} " +
                "should have not Interior Color Disclaimer field");

            SubmitForm();
            RefreshPage();
            Assert.IsTrue(IsElementNotFound(AppsPage.InteriorColorDisclaimer),
                "App v." +
                $"{TestConfig.ElementPermissionDptVersions[1]} " +
                "should have not Interior Color Disclaimer field");
            Assert.IsTrue(IsElementFound(AppsPage.Market),
                $"App v.{TestConfig.ElementPermissionDptVersions[1]} should have Market field");

            ClickUntilShown(AppsPage.LanguageDeButton, AppsPage.LanguageDeButtonActive);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.InteriorColorDisclaimer),
                "App v." +
                $"{TestConfig.ElementPermissionDptVersions[1]} " +
                "should have not Interior Color Disclaimer field in language DE");

            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowByText, TestConfig.ElementPermissionDptVersions[1]));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsElementFound(AppsPage.InteriorColorDisclaimer),
                "App v." +
                $"{TestConfig.ElementPermissionDptVersions[0]} " +
                "should have Interior Color Disclaimer field in language DE");
        }

        public void RT10220_AppMasterData2()
        {
            AddAppIpadPlayer();
            AddAppElementPermission(version: TestConfig.ElementPermissionDptVersions[0]);
            UserDirectoryApi.AssignRolesToUser(
                TestConfig.PermissionsUser, new [] { UserRole.CxmAdmin }, TenantTitle.onelang);
            _role = UserDirectoryApi.SetRolePermissions(
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
                "UndeleteOwnAppVersion",
                "UndeleteAnyAppVersion",
                "CreateModel",
                "ModifyModel",
                "PorscheDPTviewMarket");
            CurrentTenant = TenantTitle.onelang;
            AddAppIpadPlayer();
            var app = AddAppElementPermission(version: TestConfig.ElementPermissionDptVersions[0]);
            UserDirectoryApi.AddRoleToUser(
                TestConfig.PermissionsUser, _role, TenantTitle.permissions, TenantTitle.onelang);
            CurrentUser = TestConfig.PermissionsUser;
            TestStart();

            OpenEntityPage(app);
            Assert.IsTrue(IsElementFound(AppsPage.MarketReadOnly),
                $"App v.{TestConfig.ElementPermissionDptVersions[0]} should have Market field");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ImprintUrlVal),
                $"App v.{TestConfig.ElementPermissionDptVersions[0]} should have Imprint Url field");

            ChangeTenant(TenantTitle.permissions);
            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.TestElementPermissionDptFolder,
                TestConfig.TestElementPermissionDptFile, TestConfig.ElementPermissionDptVersions[2]);
            FileManager.UploadAsBackgroundTask(pathFile);
            Assert.IsTrue(IsElementFound(AppsPage.Market),
                $"App v.{TestConfig.ElementPermissionDptVersions[2]} should have Market field");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ImprintUrl),
                $"App v.{TestConfig.ElementPermissionDptVersions[2]} should have not Imprint Url field");

            SendText(AppsPage.Market, "QQ");
            SubmitForm();
            var appId = GetEntityIdFromUrl();
            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowByText, TestConfig.ElementPermissionDptVersions[2]));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.MarkAsDeletedButton);
            Click(PageHeader.NavigateBackButton);
            RefreshPage();
            Assert.IsTrue(IsElementFound(AppsPage.Market),
                $"App v.{TestConfig.ElementPermissionDptVersions[0]} should have Market field");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ImprintUrl),
                $"App v.{TestConfig.ElementPermissionDptVersions[0]} should have Imprint Url field");

            Click(PageFooter.AddVersionButton);
            pathFile = FileManager.GetFileByVersion(TestConfig.TestElementPermissionDptFolder,
                TestConfig.TestElementPermissionDptFile, TestConfig.ElementPermissionDptVersions[3]);
            FileManager.UploadAsBackgroundTask(pathFile);
            Assert.IsTrue(IsElementFound(AppsPage.Market),
                $"App v.{TestConfig.ElementPermissionDptVersions[3]} should have Market field");
            Assert.IsTrue(IsElementNotFound(AppsPage.ImprintUrl),
                $"App v.{TestConfig.ElementPermissionDptVersions[3]} should have not Imprint Url field");

            SubmitForm();
            Click(PageFooter.AddVersionButton);
            pathFile = FileManager.GetFileByVersion(TestConfig.TestElementPermissionDptFolder,
                TestConfig.TestElementPermissionDptFile, TestConfig.ElementPermissionDptVersions[4]);
            FileManager.UploadAsBackgroundTask(pathFile);
            Assert.IsTrue(IsElementFound(AppsPage.Market),
                $"App v.{TestConfig.ElementPermissionDptVersions[4]} should have Market field");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ImprintUrl),
                $"App v.{TestConfig.ElementPermissionDptVersions[4]} should have Imprint Url field");

            Click(PageFooter.CancelButton);
            OpenEntityPage<AppResponse>(appId);
            RefreshPage();
            Assert.IsTrue(IsElementFound(AppsPage.Market),
                $"App v.{TestConfig.ElementPermissionDptVersions[3]} should have Market field");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ImprintUrl),
                $"App v.{TestConfig.ElementPermissionDptVersions[3]} should have not Imprint Url field");
        }

        [TearDown]
        public async Task TearDown()
        {
            IsUseAllPageReadyChecks = true;
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
