using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.Apps;
using Models.Items;
using Models.Places;
using NUnit.Framework;
using OpenQA.Selenium;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC04B_PlacesIbeaconTests : ParentTest
    {
        private string _titlePlaceNoBeacon = "";
        private string _titlePlaceWithBeacon = "";
        private string _major = RandomNumberWord;
        private readonly string _majorNew = RandomNumberWord;
        private string _minor = RandomNumberWord;
        private readonly string _uuid = Guid.NewGuid().ToString();

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
            CurrentTenant = TenantTitle.manylang;
            CurrentUser = TestConfig.AdminUser;
        }

        [Test, Regression]
        public void RT04100_CreateIbeaconPlace()
        {
            TestStart();

            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            Assert.IsTrue(IsElementFound(PlacesPage.ErrorTitleIsRequired),
                "Error 'Title field is required' should be displayed");
            Assert.IsTrue(IsElementFound(PlacesPage.ErrorTimezoneIsRequired),
                "Error 'Timezone field is required' should be displayed");

            _titlePlaceNoBeacon = $"Auto place {RandomNumber}";
            SendText(PlacesPage.Title, _titlePlaceNoBeacon);
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorTitleIsRequired),
                "Error 'Title field is required' is still displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorTimezoneIsRequired),
                "Error 'Timezone field is required' is still displayed");

            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.Ibeacon),
                "Devices dialog not found");

            SubmitForm();

            Assert.IsTrue(AreElementsContainText(PlacesPage.Status, StatusNoDevice),
                $@"Place Status should be '{StatusNoDevice}'");
            Assert.IsTrue(AreElementsContainText(PlacesPage.DeviceTypeReadOnly, DeviceTypeIbeacon),
                @"Place Device Type should be 'iBeacon'");
            Assert.IsTrue(AreElementsContainText(PlacesPage.Ibeacon, string.Empty),
                @"Place iBeacon field should be empty");
        }

        [Test, Regression]
        public void RT04110_AssignIbeaconToPlace()
        {
            TestStart();
            AddPlaceIbeacon(PlaceStatus.NoDevice, isCreateNewPlace: true);

            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementNotFound(Devices.TableRow),
                "Devices dialog should have no items");

            CreateDeviceIbeacon();
            Assert.IsTrue(IsElementFoundQuickly(Devices.Uuid),
                "Modal dialog for new iBeacon is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(Devices.Major),
                "Field Major is not displayed in new iBeacon dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.Minor),
                "Field Minor is not displayed in new iBeacon dialog");
            Assert.IsTrue(IsElementEquals(Devices.Uuid, string.Empty),
                "Field UUID is not empty");
            Assert.IsTrue(IsElementEquals(Devices.Major, string.Empty),
                "Field Major is not empty");
            Assert.IsTrue(IsElementEquals(Devices.Minor, string.Empty),
                "Field Minor is not empty");

            Click(Devices.SubmitButtonDevice);
            SendText(Devices.Uuid, "5443534534");
            Assert.IsTrue(IsElementFoundQuickly(Devices.ErrorUuidDoesNotMatchPattern),
                @"Error 'This does not match the expected pattern' should be displayed for UUID field");
            Assert.IsTrue(IsElementFoundQuickly(Devices.ErrorMajorRequired),
                @"Error 'Major required' should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(Devices.ErrorMinorRequired),
                @"Error 'Minor required' should be displayed");

            SendText(Devices.Uuid, _uuid);
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.ErrorUuidDoesNotMatchPattern),
                @"Error 'This does not match the expected pattern' should not be displayed for UUID field");
            Assert.IsTrue(IsElementFoundQuickly(Devices.ErrorMajorRequired),
                @"Error 'Major required' should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(Devices.ErrorMinorRequired),
                @"Error 'Minor required' should be displayed");

            Click(Devices.MajorIncreaseButton);
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.ErrorMajorRequired),
                @"Error 'Major required' should not be displayed");
            Assert.IsTrue(IsElementFoundQuickly(Devices.ErrorMinorRequired),
                @"Error 'Minor required' should be displayed");

            _minor = RandomNumberWord;
            SendText(Devices.Minor, _minor);
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.ErrorMinorRequired),
                @"Error 'Minor required' should be not displayed");

            ClickUntilConditionMet(Devices.SubmitButtonDevice, 
                () => IsElementNotFoundQuickly(Devices.Uuid));
            Click(Devices.CancelButton);
            OpenDevicesFromPlace();
            Click(string.Format(Devices.TableRowByText, $"1/{_minor}"));
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, $"1/{_minor}"),
                $@"iBeacon field should be equal to '1/{_minor}'");
            Assert.IsTrue(IsEditMode(),
                "Place should stay in edit mode");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusActive),
                $"Place should be in {StatusActive} status");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, $@"1/{_minor}"),
                $@"iBeacon field should be equal to '1/{_minor}'");
        }

        [Test, Regression]
        public void RT04120_ChangePlaceAndCancel()
        {
            TestStart();
            AddPlaceIbeacon(PlaceStatus.NoDevice, isAssignIbeacon: true);

            _titlePlaceNoBeacon = GetValue(PlacesPage.TitleReadOnly);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.IbeaconDetailsButton),
                "iBeacon details button should not be shown when place in view mode");

            EditForm();
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.IbeaconDetailsButton),
                "iBeacon details button should be shown when place in edit mode");
            var ibeaconName = GetValue(PlacesPage.Ibeacon);
            OpenDevicesFromMenu();
            Click(string.Format(Devices.TableRowByText, ibeaconName));
            Assert.IsTrue(IsElementFoundQuickly(Devices.SubmitButtonDevice),
                "Submit button should be shown in iBeacon details dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice),
                "Cancel button should be shown in iBeacon details dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.DeleteButtonDevice),
                "Delete button should be shown in iBeacon details dialog");

            var newValue = Convert.ToInt32(GetValue(Devices.Major, true)) + 1;
            var minor1 = GetValue(Devices.Minor);
            Click(Devices.MajorIncreaseButton);
            Assert.IsTrue(IsElementEquals(Devices.Major, newValue.ToString()),
                $@"Major field in iBeacon details dialog should become '{newValue}' on '+' press");

            Click(Devices.DeleteButtonDevice);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DeleteDialog),
                "Delete iBeacon confirmation is not shown");

            Click(PlacesPage.CancelButton);
            Click(Devices.CancelButtonDevice);
            Click(Devices.CancelButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, $@"{newValue - 1}/{minor1}"),
                $@"iBeacon field should be equal to '{newValue - 1}/{minor1}'");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, $@"{newValue - 1}/{minor1}"),
                $@"iBeacon field should be equal to '{newValue - 1}/{minor1}'");
            Assert.IsTrue(IsViewMode(), "Place is still in edit mode");
        }

        [Test, Regression]
        public void RT04140_DuplicatePlace()
        {
            TestStart();
            var place = AddPlaceIbeacon(PlaceStatus.NoDevice, isAssignIbeacon: true);
            _titlePlaceWithBeacon = place.Title;

            Click(PageFooter.DuplicateButton);
            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton),
                "Copy of place is not in edit mode");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $@"Copy of place should have Status '{StatusNoDevice}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Title, _titlePlaceWithBeacon),
                $@"Title should be equal to original one: '{_titlePlaceWithBeacon}'");
            Assert.IsTrue(AreElementsContainText(PlacesPage.TimezoneDropDown, TimezoneKiev),
                $@"Timezone should be equal to original one: '{TimezoneKiev}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon),
                $@"Device Type should be equal to original one: '{DeviceTypeIbeacon}'");
            Assert.IsTrue(AreElementsContainText(PlacesPage.Ibeacon, string.Empty),
                "iBeacon field should be empty");

            SubmitForm();
            Assert.IsTrue(IsElementFound(PlacesPage.PlaceAlreadyExistsDialog),
                $@"Modal dialog 'Place with title {_titlePlaceWithBeacon} already exists' should be displayed");

            Click(PlacesPage.OkButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.PlaceAlreadyExistsDialog),
                $@"Modal dialog 'Place with title {_titlePlaceWithBeacon} already exists' should be closed");
            Assert.IsTrue(IsEditMode(), "Copy of place is not in edit mode");

            _titlePlaceNoBeacon = $"Auto place {RandomNumber}";
            SendText(PlacesPage.Title, _titlePlaceNoBeacon);
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(PlacesPage.DialogPlaceholder),
                @"No errors/dialogs should be displayed on place copy Submit button press");
        }

        [Test, Regression]
        public void RT04150_IbeaconFieldChecks()
        {
            var place1 = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: true, pageToBeOpened: 0);
            TestStart();
            var place2 = AddPlaceIbeacon(PlaceStatus.Active);
            
            EditForm();
            OpenDevicesFromPlace();
            var deviceName = $"{place1.Device.Data.Major}/{place1.Device.Data.Minor}";
            Assert.IsTrue(IsAnyElementEquals(Devices.TableRowTitle, deviceName),
                "Devices dialog should have device " + deviceName);

            Assert.IsTrue(IsElementEquals(string.Format(Devices.TableRowPlaceByTitle, deviceName), place1.Title),
                $@"Devices dialog should contain device {deviceName} with place {place1.Title} selected");

            Click(string.Format(Devices.TableRowByText, deviceName));
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.DevicesDialog),
                "Devices dialog should be closed");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, deviceName),
                $"Device field should be set to {deviceName}");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.TitleReadOnly, place2.Title),
                $@"Place should be '{place2.Title}' after submit");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusActive),
                $"Place should be in {StatusActive} status after submit");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, deviceName),
                $@"iBeacon field should have value '{deviceName}' after submit");
            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementEquals(string.Format(Devices.TableRowPlaceByTitle, deviceName), place2.Title),
                $@"Devices dialog should contain device {deviceName} with place {place2.Title} selected");
            Click(Devices.CancelButton);

            OpenEntityPage(place1);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $"Place should be in {StatusNoDevice} status device reassignment");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                @"iBeacon field should be empty after device reassignment");
        }

        [Test, Regression]
        public void RT04160_IbeaconDeviceDeleteRestore()
        {
            var place1 = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: false, pageToBeOpened: 0);
            TestStart();
            var place2 = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: true);

            EditForm();
            var devTitle = GetValue(PlacesPage.Ibeacon);
            OpenDevice(devTitle);
            Click(Devices.DeleteButtonDevice);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DeleteDialog),
                "Delete iBeacon confirmation is not shown");

            Click(PlacesPage.DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DeleteDialog),
                "Delete iBeacon confirmation is still shown");
            Click(Devices.CancelButton);
            Assert.IsTrue(IsEditMode(), "Place is not in edit mode");

            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusActive),
                $"Place should be in {StatusActive} status after submit");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "iBeacon field should be empty after iBeacon deletion");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $"Place should be in {StatusNoDevice} status after submit");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "iBeacon field should be empty after iBeacon deletion");

            OpenPlacesPage();
            Click(string.Format(PlacesPage.TableRowByTitle, place1.Title));
            EditForm();
            OpenDevicesFromPlace();
            Assert.IsFalse(
                IsAnyElementEquals(
                    string.Format(Devices.TableRowStatusByTitle, string.Empty), 
                    StatusDeleted),
                $"Devices dialog opened from place Device field should hide {StatusDeleted} devices");
            var deviceName = $"{place2.Device.Data.Major}/{place2.Device.Data.Minor}";
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(Devices.TableRowByText, deviceName)),
                @"Devices dialog should have no device " + deviceName);

            OpenDevicesFromMenu();
            Click(Devices.ShowDeletedButton);
            Click(string.Format(Devices.TableRowByText, deviceName));
            Assert.IsTrue(IsElementFound(Devices.UuidReadOnly),
                $"iBeacon {deviceName} restore dialog is not displayed or field UUID is not read-only");
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice),
                "iBeacon restore dialog does not have Cancel button");
            Assert.IsTrue(IsElementFoundQuickly(Devices.RestoreButtonDevice),
                "iBeacon restore dialog does not have Restore button");

            Assert.IsTrue(IsElementFoundQuickly(Devices.MajorReadOnly),
                "iBeacon restore dialog Major field is not read-only");
            Assert.IsTrue(IsElementFoundQuickly(Devices.MinorReadOnly),
                "iBeacon restore dialog Major field is not read-only");

            Click(Devices.RestoreButtonDevice);
            Assert.IsTrue(IsElementNotFound(Devices.UuidReadOnly),
                "iBeacon restore dialog still displayed after Restore button press");

            Assert.IsTrue(
                IsElementEquals(string.Format(Devices.TableRowStatusByTitle, deviceName), StatusActive),
                $"Restored device {deviceName} should be in status {StatusActive}");
            Click(Devices.CancelButton);
            OpenDevicesFromPlace();
            Click(string.Format(Devices.TableRowByText, deviceName));
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, deviceName),
                $@"iBeacon field should have device '{deviceName}'");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusActive),
                $"Place should be in {StatusActive} status after submit");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, deviceName),
                $@"iBeacon field should have value '{deviceName}' after submit");
        }

        [Test, Regression]
        public void RT04170_NewIbeaconDevice()
        {
            TestStart();
            var place1 = AddPlaceIbeacon(PlaceStatus.Any, isAssignIbeacon: true, isCreateNewPlace: true);

            EditForm();
            var oldDeviceName = $"{place1.Device.Data.Major}/{place1.Device.Data.Minor}";
            var newDeviceName = $"{_majorNew}/{_minor}";
            CreateDeviceIbeacon(_uuid, _majorNew, _minor);
            OpenDevicesFromPlace();
            Click(string.Format(Devices.TableRowByText, newDeviceName));
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, newDeviceName),
                $@"iBeacon field should have value '{newDeviceName}'");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, oldDeviceName),
                $@"iBeacon field should have value '{oldDeviceName}'");

            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementFound(string.Format(Devices.TableRowByText, oldDeviceName))
                    && IsElementFound(string.Format(Devices.TableRowByText, newDeviceName)),
                $@"Devices dialog should have devices: {oldDeviceName}, {newDeviceName}");
            
            Assert.IsTrue(
                IsElementEquals(string.Format(Devices.TableRowStatusByTitle, newDeviceName), StatusActive),
                $@"Device '{newDeviceName}' should be present in Devices dialog in status {StatusActive}");

            Click(Devices.CancelButton);
        }

        [Test, Regression]
        public void RT04171_AnotherTenantIbeacons()
        {
            CurrentTenant = TenantTitle.onelang;
            TestStart();
            AddPlaceIbeacon(PlaceStatus.Active);

            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementNotFound(Devices.TableRow),
                $@"Tenant {TenantTitle.onelang}: Devices dialog should have no devices");
        }

        [Test, Regression]
        public void RT04172_IbeaconPropertiesCheck()
        {
            var guid = Guid.NewGuid().ToString();
            var guid2 = Guid.NewGuid().ToString();
            CurrentTenant = TenantTitle.nolang;
            TestStart();
            var d1 = PlaceApi.CreateIbeacon(null, null, guid);
            var d2 = PlaceApi.CreateIbeacon(null, null, guid);
            var d3 = PlaceApi.CreateIbeacon(null, null, guid);
            var place = AddPlaceIbeacon(PlaceStatus.Active);

            EditForm();
            OpenDevicesFromPlace();
            var originalCollection = new List<string>
            {
                $"{d1.Data.Major}/{d1.Data.Minor}",
                $"{d2.Data.Major}/{d2.Data.Minor}",
                $"{d3.Data.Major}/{d3.Data.Minor}"
            };
            var currentCollection = GetElementsText(Devices.TableRowTitle);
            Assert.IsTrue(IsCollectionContainsCollection(currentCollection, originalCollection),
                @"Devices dialog should have list items: " + string.Join(", ", originalCollection));

            var d4Major = RandomNumberWord;
            var d4Minor = RandomNumberWord;
            CreateDeviceIbeacon(guid, d4Major, d4Minor);
            OpenDevicesFromPlace();
            originalCollection = new List<string>
            {
                $"{d4Major}/{d4Minor}",
                $"{d1.Data.Major}/{d1.Data.Minor}",
                $"{d2.Data.Major}/{d2.Data.Minor}",
                $"{d3.Data.Major}/{d3.Data.Minor}"
            };
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(Devices.TableRowTitle), originalCollection),
                @"Devices dialog should have devices: " + string.Join(", ", originalCollection));
            
            SetFilterModal(guid);
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(Devices.TableRowTitle), originalCollection),
                "Devices dialog should have devices: " + string.Join(", ", originalCollection));

            SetFilterModal($@"{d4Major}/");
            originalCollection = new List<string>
            {
                $"{d4Major}/{d4Minor}"
            };
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(Devices.TableRowTitle), originalCollection),
                @"Devices dialog should have only device " + string.Join(", ", originalCollection));

            Click(string.Format(Devices.TableRowByText, $"{d4Major}/{d4Minor}"));
            SubmitForm();
            EditForm();
            OpenDevicesFromPlace();
            SetFilterModal(place.Title);
            Assert.IsTrue(IsElementEquals(
                    string.Format(Devices.TableRowPlaceByTitle, $"{d4Major}/{d4Minor}"), place.Title),
                $"iBeacon {d4Major}/{d4Minor} assigned to place should have its title {place.Title}");

            PressKeys(Keys.Escape);
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.DevicesDialog),
                @"Devices dialog should be closed on 'Esc' press");

            ClearDevice();
            SubmitForm();
            EditForm();
            OpenDevicesFromPlace();
            SetFilterModal(place.Title);
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.TableRow), 
                $@"Devices dialog should not show any device by filter '{place.Title}'");
            SetFilterModal(string.Empty);

            OpenDevice($"{d1.Data.Major}/{d1.Data.Minor}");
            SendText(Devices.Uuid, guid2);
            var d1Minor = RandomNumberWord;
            SendText(Devices.Minor, d1Minor);
            Click(Devices.SubmitButtonDevice);
            OpenDevicesFromPlace();
            SetFilterModal(guid);
            originalCollection = new List<string>
            {
                $"{d2.Data.Major}/{d2.Data.Minor}",
                $"{d3.Data.Major}/{d3.Data.Minor}",
                $"{d4Major}/{d4Minor}"
            };
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(Devices.TableRowTitle), originalCollection),
                @"Devices dialog should have devices with old UUID: " + 
                    string.Join(", ", originalCollection));

            SetFilterModal(guid2);
            originalCollection = new List<string>
            {
                $"{d1.Data.Major}/{d1Minor}"
            };
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(Devices.TableRowTitle), originalCollection),
                "Devices dialog should have device: " + string.Join(", ", originalCollection));

            SetFilterModal($"{d1.Data.Major}/{d1Minor}");
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(Devices.TableRowTitle), originalCollection),
                "Devices dialog should have device: " + string.Join(", ", originalCollection));

            OpenDevice($"{d1.Data.Major}/{d1Minor}");
            Click(Devices.DeleteButtonDevice);
            Click(PlacesPage.DeleteButton);
            Click(Devices.ShowDeletedButton, true, 5);
            SetFilterModal(guid2);
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(Devices.TableRowTitle), originalCollection),
                @"Devices dialog should have one deleted device: " + 
                    string.Join(", ", originalCollection));

            OpenDevice($"{d1.Data.Major}/{d1Minor}");
            Assert.IsTrue(IsElementFoundQuickly(Devices.UuidReadOnly), 
                "Restore device dialog should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice), 
                "Restore device dialog should have Cancel button");
            Assert.IsTrue(IsElementFoundQuickly(Devices.RestoreButtonDevice), 
                "Restore device dialog should have Restore button");

            Click(PlacesPage.RestoreButton);
            Assert.IsTrue(
                IsElementEquals(
                    string.Format(Devices.TableRowStatusByTitle, $"{d1.Data.Major}/{d1Minor}"), StatusActive),
                $"iBeacon {d1.Data.Major}/{d1Minor} should be restored and {StatusActive}");
            
            Click(Devices.CancelButton);
            Click(PageFooter.SubmitButton);
        }

        [Test, Regression]
        public void RT04180_PlaceAndDptApp()
        {
            CurrentTenant = TenantTitle.onelang;
            AddAppDpt(AppStatus.Any, TestConfig.DptAppVersions[0]);
            TestStart();
            AddPlaceIbeacon(PlaceStatus.Active);

            MouseOver(PageFooter.AddPlaceSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddExistingPlaceAsChildButton),
                @"Button 'Add Existing Place As Child' not found");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CreateNewChildPlaceButton),
                @"Button 'Create New Child Place' not found");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddAppInPlacesButton),
                @"Button 'Add app' not found");

            Click(PageFooter.AddAppInPlacesButton);
            Assert.IsTrue(IsElementFound(PlacesPage.NoAppsToBeAddedDialog),
                "Error 'There is no apps to be added' should be displayed");
            
            Click(PlacesPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.NoAppsToBeAddedDialog),
                "Error 'There is no apps to be added' is still displayed");
            Assert.IsTrue(IsViewMode(), "Place is in edit mode");
        }

        [Test, Regression]
        public void RT04190_AddAppToPlace()
        {
            AddAppPlayer();
            Parallel.Invoke(
                () => AddAppIbeacon(TestConfig.IbeaconAppVersions[1]),
                () => AddAppComposerHq1()
            );
            TestStart();
            AddPlaceIbeacon(PlaceStatus.Active);

            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Assert.IsTrue(IsElementFound(PlacesPage.SelectAppDialog),
                @"'Select an app from the list' dialog should be displayed");
            Assert.IsTrue(CountElements(PlacesPage.TableSelectAppTitleColumnValues) == 1
                    && AreAllElementsContainText(PlacesPage.TableSelectAppTitleColumnValues, AppTitle.Ibeacon),
                @"One iBeacon app should be displayed in 'select app' dialog");

            Click(string.Format(PlacesPage.TableSelectAppTitle, AppTitle.Ibeacon));
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.SelectAppDialog),
                @"Dialog 'select app' should be closed after app selection");
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetails),
                "App should be shown expanded in Apps section");
            Assert.IsTrue(IsEditMode(), "Place is in view mode after app addition");

            SubmitForm();
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRow1),
                "App assigned to the place should be shown in Apps section");
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetails),
                "App details should be expanded");
            Assert.IsTrue(AreElementsContainText(PlacesPage.AppsSectionTableRowDetailsTitleReadOnly, AppTitle.Ibeacon),
                $@"App field 'Title' should contain '{AppTitle.Ibeacon}' text");
            Assert.IsTrue(AreElementsContainText(PlacesPage.AppsSectionTableRowDetailsAppTitleReadOnly, AppTitle.Ibeacon),
                $@"App field 'App Title' should contain '{AppTitle.Ibeacon}' text");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsVersionReadOnly, TestConfig.IbeaconAppVersions[1]),
                $@"App field 'Version' should contain '{TestConfig.IbeaconAppVersions[1]}' text");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsCarReadOnly, string.Empty),
                @"App field 'Car' should be empty");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsPoiReadOnly, string.Empty),
                @"App field 'POI' should be empty");
        }

        [Test, Regression]
        public void RT04200_AddItemToApp()
        {
            Place place = null;
            Item item = null;
            AppResponse app = null;
            Parallel.Invoke(
                () => place = AddPlaceIbeacon(PlaceStatus.Active, pageToBeOpened: 0),
                () => app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]),
                () => item = AddItem(ItemType.PorscheCar)
            );
            TestStart();
            AssignAppToPlace(place, app, item: null, itemType: null);

            EditForm();
            Click(PlacesPage.AppsSectionTableRow1);
            Click(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton);
            Assert.IsTrue(IsElementFound(PlacesPage.SelectItemDialog),
                @"'Select item' dialog is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ClearSelectionButton), 
                "Clear Selection button is absent in modal window");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.CancelButton),
                "Cancel button is absent in modal window");
            
            Click(string.Format(PlacesPage.TableSelectItemTitle, item.LangJsonData.EnJson.Title));
            var carName = $"{item.LangJsonData.EnJson.Title} ({ItemTypePorscheCar})";
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsCar, carName),
                $@"Car field should have value '{carName}'");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsCarReadOnly, carName),
                $@"Car field after save should have value '{carName}'");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton),
                @"Button '...' should be absent in Car field after save");

            RefreshPage();
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRowDetails),
                "Section Apps should be collapsed after page refresh");
            var tableHeader = GetValuesAsString(PlacesPage.AppsSectionTableHeader);
            Assert.IsTrue(tableHeader.Contains("#") && tableHeader.Contains("Title")
                    && tableHeader.Contains("Entity") && tableHeader.Contains("Version")
                    && tableHeader.Contains("Status"),
                "Apps section table should have columns: #, Status, Title, Entity, Version");
            var tableRow = CleanUpString(GetValuesAsString(PlacesPage.AppsSectionTableRow1));
            var tableRowOriginal = $"1 Queued{app.ActualAppVersion.Title}{TestConfig.IbeaconAppVersions[1]}";
            Assert.IsTrue(tableRow == tableRowOriginal,
                @"Apps section table row have content: # = '1', Status = 'Queued', " +
                $@"Title = '{app.ActualAppVersion.Title}', Entity = '', " + 
                $@"Version = '{TestConfig.IbeaconAppVersions[1]}'");

            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetails),
                "Section Apps, row 1 should be expanded");
        }

        [Test, Regression]
        public void RT04210_DeleteApp()
        {
            Place place = null;
            AppResponse app = null;
            Item item = null;
            Parallel.Invoke(
                () => place = AddPlaceIbeacon(PlaceStatus.Active, pageToBeOpened: 0),
                () => app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]),
                () => item = AddItem(ItemType.PorscheCar)
            );
            TestStart();
            AssignAppToPlace(place, app, item, ItemTypePorscheCar);
            if (IsElementNotFound(PlacesPage.AppsSectionTableRowDetails))
            {
                Click(PlacesPage.AppsSectionTableRow1);
            }

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add submenu button not found in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button not found in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button not found in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button not found in footer");

            var carName = GetValue(PlacesPage.AppsSectionTableRowDetailsCarReadOnly);
            Click(PageFooter.DuplicateButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsCar, carName),
                $"In place copy should be the same car '{carName}'");

            var newTitle = $"Auto test {RandomNumber}";
            SendText(PlacesPage.Title, newTitle);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place currently in edit mode");
            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsCarReadOnly, carName),
                $"In place copy should be the same car '{carName}'");

            EditForm();
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add submenu button not found in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button not found in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button not found in footer");

            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow1DeleteButton),
                "In Apps section, row 1 has Delete button even in collapsed state");

            Click(PlacesPage.AppsSectionTableRow1);
            Click(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton);
            Click(PlacesPage.ClearSelectionButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DialogPlaceholder),
                "Item selection dialog is still opened");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsCar, string.Empty),
                "Car field should be empty after clear selection");

            Click(PlacesPage.AppsSectionTableRowDetailsPoiDetailsButton);
            // To run only in TeamCity, otherwise these actions will fail
            //if (TestContext.Parameters.Count != 0)
            //{
            //Assert.IsTrue(IsElementFound(PlacesPage.NoItemsToBeAddedDialog),
            //    @"'No items to be added' dialog should be displayed");

            if (IsElementFoundQuickly(PlacesPage.NoItemsToBeAddedDialog))
                Click(PlacesPage.OkButton);
            else
                Click(PlacesPage.CancelButton);
                //Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.NoItemsToBeAddedDialog),
                //    @"'No items to be added' dialog should be closed");
            //}

            Click(PlacesPage.AppsSectionTableRow1DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DialogPlaceholder),
                "Some dialog window is opened");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow1),
                "App has not been deleted after click on trash bin button");

            SubmitForm();
            Assert.IsTrue(IsElementNotFound(PlacesPage.AppsSectionTableRow1),
                "App has not been deleted after click on trash bin button and submit");
            Assert.IsTrue(IsViewMode(), "Place currently in edit mode");
        }

        [Test, Regression]
        public void RT04220_AddItemToApp()
        {
            Place place = null;
            AppResponse app2 = null;
            Item item1 = null, item2 = null;
            Parallel.Invoke(
                () => place = AddPlaceIbeacon(PlaceStatus.Active, pageToBeOpened: 0, isCreateNewPlace: true),
                () => AddAppIbeacon(TestConfig.IbeaconAppVersions[0]),
                () => app2 = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]),
                () => item1 = AddItem(ItemType.Poi),
                () => item2 = AddItem(ItemType.PorscheCar)
            );
            TestStart();
            place = AssignAppToPlace(place, app2, item2, ItemTypePorscheCar);

            EditForm();
            if (IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRowDetails))
            {
                Click(PlacesPage.AppsSectionTableRow1);
            }
            ClickUntilShown(PlacesPage.AppsSectionTableRowDetailsPoiDetailsButton,
                PlacesPage.SelectItemDialog);
            Assert.IsTrue(IsElementFound(PlacesPage.SelectItemDialog),
                @"'Select Item' dialog not displayed");
            // To run only in TeamCity, otherwise this action will fail
            //if (TestContext.Parameters.Count != 0)
            //{
                Assert.IsTrue(CountElements(PlacesPage.TableSelectItemTitleColumnValues) >= 1,
                    "Only one POI item should be available for selection");
            //}
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ClearSelectionButton),
                "Clear Selection button is absent in modal window");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.CancelButton),
                "Cancel button is absent in modal window");

            Click(string.Format(PlacesPage.TableSelectItemTitle, item1.LangJsonData.EnJson.Title));
            var poiName = $"{item1.LangJsonData.EnJson.Title} ({ItemTypePoi})";
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsPoiDropDown, poiName),
                $@"POI field should have value '{poiName}'");

            var newTitle = $"Auto test {RandomNumber}";
            SendText(PlacesPage.AppsSectionTableRowDetailsTitle, newTitle);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow1TitleCell, newTitle),
                "Title in table row 1 should be equal to Title in expanded row details section");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow1AppPackageCell, string.Empty),
                "App/Package in table row 1 should be empty");

            DropDownSelect(PlacesPage.AppsSectionTableRowDetailsVersionDropDown, TestConfig.IbeaconAppVersions[0]);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsTitle, newTitle),
                $@"Title in row 1 details section should be equal to '{newTitle}'");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow1TitleCell, newTitle),
                $@"Title in table row 1 should be equal to '{newTitle}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow1AppPackageCell, string.Empty),
                "App/Package in table row 1 should be empty");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow1VersionCell, TestConfig.IbeaconAppVersions[0]),
                $@"Version in table row 1 should be equal to '{TestConfig.IbeaconAppVersions[0]}'");
        }

        [Test, Regression]
        public void RT04230_PlaceRestore()
        {
            Place place = null;
            AppResponse app = null;
            Item item = null;
            Parallel.Invoke(
                () => place = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: true, pageToBeOpened: 0),
                () => app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]),
                () => item = AddItem(ItemType.PorscheCar)
            );
            TestStart();
            place = AssignAppToPlace(place, app, item, ItemTypePorscheCar);
            if (IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRowDetails))
            {
                Click(PlacesPage.AppsSectionTableRow1);
            }

            // get iBeacon's "major/minor" value without place name
            var iBeaconValue = GetValue(PlacesPage.Ibeacon);
            Click(PageFooter.DeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DeleteDialog),
                "Delete place confirmation dialog not displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.CancelButton),
                "Delete place confirmation dialog has no Cancel button");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DeleteButton),
                "Delete place confirmation dialog has no Delete button");

            Click(PlacesPage.DeleteButton);
            Assert.IsTrue(IsElementFound(PlacesPage.TableHeader),
                "Places page should be displayed");

            Click(PageFooter.ShowDeletedButton, true, 5);
            Click(string.Format(PlacesPage.TableRowByTitle, place.Title));
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusDeleted),
                $"Place should have Status '{StatusDeleted}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "iBeacon field should be empty");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow1),
                "App iBeacon should be still assigned to the place");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should present in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButtonInactive),
                "Inactive Duplicate button should present in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.RestoreButton),
                "Restore button should present in footer");

            Click(PageFooter.RestoreButton);
            Assert.IsTrue(IsEditMode(), "Place in not in edit mode after restore");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $"Place should be in status '{StatusNoDevice}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "iBeacon field should be empty");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should present in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should present in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should present in footer");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place in not in view mode after submit");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $"Place should be in status '{StatusNoDevice}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "iBeacon field should be empty");

            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementFoundQuickly(string.Format(Devices.TableRowByText, iBeaconValue)),
                $"iBeacon device {iBeaconValue} is available in list");
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
                PlaceApi.DeletePlaces(PlaceType.NoType);
                PlaceApi.DeletePlaces(PlaceType.Ibeacon);
            }
        }
    }
}
