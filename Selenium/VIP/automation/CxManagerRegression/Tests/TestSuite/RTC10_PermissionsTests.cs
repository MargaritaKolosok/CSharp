using System.IO;
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
    public sealed class RTC10_PermissionsTests : ParentTest
    {
        private Role _role;

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            CurrentUser = TestConfig.AdminUser;
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }

            CurrentTenant = TenantTitle.permissions;
            CurrentUser = TestConfig.AdminUser;
            UserDirectoryApi.AssignRolesToUser(TestConfig.PermissionsUser, null);
        }

        [Test, Regression]
        public void RT10010_LoginAndPlaces()
        {
            Place place = null, place2 = null;
            Parallel.Invoke(
                () => place = AddPlaceNoType(PlaceStatus.NoDevice, pageToBeOpened: 0, isCreateNewPlace: true),
                () => place2 = AddPlaceIbeacon(PlaceStatus.Deleted, pageToBeOpened: 0, isCreateNewPlace: true)
            );
            TestStart(TestConfig.LoginUrl, false);

            Login(TestConfig.PermissionsUser, false);
            Assert.IsTrue(IsElementFound(LoginPage.ErrorNoRolesAndPermissionsAssigned),
                @"Error 'You have no roles and permissions assigned to you...' not displayed");

            _role = UserDirectoryApi.SetRolePermissions(_role, "Login");
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.permissions);
            RefreshPage();
            Login(TestConfig.PermissionsUser, false);
            Assert.IsTrue(IsElementFoundQuickly(LoginPage.ErrorNotAuthorizedToAccessSystem),
                @"Dialog 'You are not authorized to access the system, please contact administrator' should be displayed");

            Click(LoginPage.OkButton);
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.manylang);
            RefreshPage();
            Login(TestConfig.PermissionsUser, false);
            Assert.IsTrue(IsPageContainsUri(TestConfig.TenantsUrl),
                "User should be redirected to the Tenants page");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, TenantTitle.permissions.ToString()))
                && IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, TenantTitle.manylang.ToString())),
                $"On Tenants page should be {TenantTitle.permissions.ToString()} and {TenantTitle.manylang.ToString()} available");

            Click(string.Format(TenantsPage.TableRowByTitle, TenantTitle.permissions.ToString()));
            Assert.IsTrue(IsElementFoundQuickly(LoginPage.ErrorNotAuthorizedToAccessSystem),
                @"Dialog 'You are not authorized to access the system, please contact administrator' should be displayed");

            Click(LoginPage.OkButton);
            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation");
            Click(string.Format(TenantsPage.TableRowByTitle, TenantTitle.permissions.ToString()));
            Assert.IsTrue(IsElementFound(PageHeader.PagePlacesButton), "Places tab should be available");
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.PageAppsButton), "Apps tab should be unavailable");
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.PageItemsButton)
                          && IsElementNotFoundQuickly(PageHeader.PageCarsButton),
                "Items & Cars tabs should be unavailable");

            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.TableRowTitle),
                "There should be some place(s) created before by other user");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceButton),
                "Add New button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Show Deleted button should be present in page footer");

            OpenEntityPage(place);
            Assert.IsTrue(IsElementFound(PageFooter.DuplicateButton),
                "Add New button should be present in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should not be present in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.EditButton),
                "Edit button should not be present in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.DeleteButton),
                "Delete button should not be present in page footer");

            PressKeys(Keys.F2);
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.SubmitButton), 
                "Place should stay in view mode on F2 press");

            PressKeys(Keys.Delete);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DeleteDialog), 
                "Delete place dialog should not be shown on Delete key press");

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New place should be created successfully");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Add New button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be present in page footer");

            EditForm();
            var title = $"Auto test {RandomNumber}";
            SendText(PlacesPage.Title, title);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New place should be created successfully");

            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteButton);
            Assert.IsTrue(IsElementNotFound(string.Format(PlacesPage.TableRowByTitle, title)),
                $@"Newly created place '{title}' should be deleted");

            Click(PageFooter.ShowDeletedButton);
            Click(string.Format(PlacesPage.TableRowByTitle, title));
            Click(PageFooter.RestoreButton);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New place should be restored successfully");

            OpenEntityPage(place2);
            Assert.IsTrue(IsElementNotFound(PageFooter.RestoreButton),
                "Restore button should be present in page footer for place delete by other user");

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
                "UndeleteAnyLocation");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PageFooter.RestoreButton, timeout: 60),
                "Restore button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenuInactive),
                "Inactive +Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButtonInactive),
                "Inactive Duplicate button should be present in page footer");

            Click(PageFooter.RestoreButton);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Other user's place should be restored successfully");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be present in page footer");

            OpenEntityPage(place);
            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteButton);
            Assert.IsTrue(IsElementEquals(string.Format(PlacesPage.TableRowStatusByTitle, place.Title), StatusDeleted),
                $"Newly created place {place.Title} should be shown and Status is {StatusDeleted}");
        }

        [Test, Regression]
        public void RT10020_PlacesAndDevices()
        {
            Place place1 = null;
            CurrentTenant = TenantTitle.manylang;
            var device1 = PlaceApi.CreateIbeacon(RandomNumberWord, RandomNumberWord);
            var device2 = PlaceApi.CreateIbeacon(RandomNumberWord, RandomNumberWord);
            Parallel.Invoke(
                () => AddPlaceWw(PlaceStatus.Unknown, isAssignDevice: true, pageToBeOpened: 0, 
                    isCreateNewPlace: true),
                () => place1 = AddPlaceIbeacon(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => PlaceApi.DeleteIbeacon(device2),
                () => _role = UserDirectoryApi.SetRolePermissions(
                    _role,
                    "Login",
                    "DevicesManagement",
                    "CreateLocation",
                    "GetAnyLocation",
                    "ModifyOwnLocation",
                    "DeleteOwnLocation",
                    "UndeleteOwnLocation",
                    "ModifyAnyLocation",
                    "DeleteAnyLocation",
                    "UndeleteAnyLocation",
                    "DevicesManagement")
            );
            TestStart(TestConfig.LoginUrl, false);
            Parallel.Invoke(
                () => AssignDeviceToPlace(place1, device1),
                () => UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.manylang)
            );
            Login(TestConfig.PermissionsUser, CurrentTenant);

            var deviceName = $"{device1.Data.Major}/{device1.Data.Minor}";
            var deviceName2 = $"{device2.Data.Major}/{device2.Data.Minor}";
            OpenDevicesFromMenu();
            Click(string.Format(Devices.TableRowByText, deviceName));
            Assert.IsTrue(IsElementFound(Devices.CancelButtonDevice),
                "Cancel button should be available in device dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.SubmitButtonDevice),
                "Submit button should be not available in device dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.DeleteButtonDevice),
                "Delete button should be not available in device dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.AddButton),
                "Add button should be not shown in Devices dialog");

            Assert.IsTrue(IsElementFoundQuickly(Devices.MajorReadOnly),
                "Major field should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(Devices.MinorReadOnly),
                "Minor field should be read-only");

            ClickUntilConditionMet(Devices.CancelButtonDevice, 
                () => IsElementNotFoundQuickly(Devices.UuidReadOnly));
            Click(Devices.CancelButton);
            OpenEntityPage(place1);
            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementFound(
                    string.Format(Devices.TableRowByText, deviceName)), 
                $"iBeacon device {deviceName} should be shown in Devices dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(Devices.TableRowByText, deviceName2)),
                $"iBeacon deleted device {deviceName2} should be not shown in Devices dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.ClearSelectionButton),
                "Clear Selection button should be shown in Devices dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButton),
                "Cancel button should be shown in Devices dialog");

            OpenDevicesFromMenu();
            Click(Devices.ShowDeletedButton, true, 1);
            Click(string.Format(Devices.TableRowByText, deviceName2));
            Assert.IsTrue(IsElementFound(Devices.UuidReadOnly),
                "iBeacon device properties modal should be displayed on deleted device selection");
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice),
                "Cancel button should be available in device dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.RestoreButtonDevice),
                "Restore button should not be available in device dialog");

            ClickUntilConditionMet(Devices.CancelButtonDevice,
                () => IsElementNotFoundQuickly(Devices.UuidReadOnly));
            Click(Devices.CancelButton);
            ClearDevice();
            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty), "iBeacon field should be empty");

            EditForm();
            OpenDevicesFromPlace();
            Click(string.Format(Devices.TableRowByText, deviceName));
            SubmitForm();
            Assert.IsTrue(
                AreElementsContainText(PlacesPage.Ibeacon, deviceName),
                $"iBeacon field should contain {deviceName}");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "DevicesManagement",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "CreateDevice", 
                "ModifyDevice", 
                "RestoreDevice",
                "DevicesManagement");
            RefreshPage();
            EditForm();
            OpenDevicesFromMenu();
            Click(string.Format(Devices.TableRowByText, deviceName));
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice),
                "Cancel button should be available in device dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.SubmitButtonDevice),
                "Submit button should be available in device dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.DeleteButtonDevice),
                "Delete button should not be available in device dialog");

            Assert.IsTrue(IsElementFoundQuickly(Devices.Uuid),
                "UUID field should not be read-only");
            Assert.IsTrue(IsElementFoundQuickly(Devices.Major),
                "Major field should not be read-only");
            Assert.IsTrue(IsElementFoundQuickly(Devices.Minor),
                "Minor field should not be read-only");

            Click(Devices.SubmitButtonDevice);
            Click(Devices.ShowDeletedButton, true, 1);
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(Devices.TableRowByText, deviceName2)),
                $"iBeacon deleted device {deviceName2} should be shown in Devices dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.AddButton),
                "Add button should be shown in Devices dialog");

            Click(string.Format(Devices.TableRowByText, deviceName2));
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice),
                "Cancel button should be available in device dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.RestoreButtonDevice),
                "Restore button should be available in device dialog");

            Click(Devices.RestoreButtonDevice);
            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementFoundQuickly(Devices.ClearSelectionButton),
                "Clear Selection button should be shown in Devices dialog");
            Click(string.Format(Devices.TableRowByText, deviceName2));
            Assert.IsTrue(IsEditMode(), "Place should stay in edit mode");
            Assert.IsTrue(
                AreElementsContainText(PlacesPage.Ibeacon, deviceName2),
                $"Devices dialog should contain {deviceName2}");
            SubmitForm();

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "DevicesManagement",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "CreateDevice",
                "ModifyDevice",
                "RestoreDevice",
                "DeleteDevice",
                "DevicesManagement");
            RefreshPage();
            EditForm();
            OpenDevicesFromMenu();
            Click(string.Format(Devices.TableRowByText, deviceName2));
            Assert.IsTrue(IsElementFoundQuickly(Devices.CancelButtonDevice),
                "Cancel button should be available in device dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.SubmitButtonDevice),
                "Submit button should be available in device dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.DeleteButtonDevice),
                "Delete button should be available in device dialog");

            Click(Devices.DeleteButtonDevice);
            Click(PlacesPage.DeleteButton);
            Click(Devices.CancelButton);
            Assert.IsTrue(IsEditMode(), 
                "Place should stay in edit mode after assigned device delete");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "iBeacon field should be empty after device delete");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsViewMode(), "Place should switch to view mode");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "Devices dialog should be empty");

            OpenEntityPage(place1);
            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementFoundQuickly(Devices.ClearSelectionButton),
                "Clear Selection button should be shown in device dialog");
            Assert.IsTrue(CountElements(Devices.TableRow) > 1
                    && IsElementFoundQuickly(string.Format(Devices.TableRowByText, place1.Device.Name)),
                $@"Devices including '{place1.Device.Name}' should be shown in Devices dialog");

            UserDirectoryApi.SetRolePermissions(
                _role,
                "Login",
                "DevicesManagement",
                "CreateLocation",
                "GetAnyLocation",
                "ModifyOwnLocation",
                "DeleteOwnLocation",
                "UndeleteOwnLocation",
                "ModifyAnyLocation",
                "DeleteAnyLocation",
                "UndeleteAnyLocation",
                "CreateDevice",
                "ModifyDevice",
                "RestoreDevice",
                "DeleteDevice");
            RefreshPage();
            MouseOver(PageHeader.UserMenuButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.UserMenuDevicesButton),
                "Devices menu item should be not shown in user menu");
        }

        [Test, Regression]
        public void RT10030_PlacesSectionsAndFields()
        {
            Place place1 = null, place2 = null;
            AppResponse app1 = null, app2 = null;
            CurrentTenant = TenantTitle.manylang; // manylang
            Parallel.Invoke(
                () => place2 = AddPlaceWw(PlaceStatus.Any, isAssignDevice: true, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => AddAppPlayer(),
                () => UserDirectoryApi.AssignRolesToUser(
                    TestConfig.PermissionsUser, new[] { UserRole.CxmAdmin }, TenantTitle.manylang),
                () => app2 = AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion)
            );
            place2 = AssignAppToPlace(place2, app2, null, null, isAddSilently: true);
            CurrentTenant = TenantTitle.nolang; // nolang
            Parallel.Invoke(
                () => AddAppPlayer(),
                () => place1 = AddPlaceWw(PlaceStatus.Any, isAssignDevice: true, pageToBeOpened: 0,
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
                    "CreateDevice", 
                    "ModifyDevice", 
                    "RestoreDevice", 
                    "DeleteDevice"),
                () => app1 = AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion)
            );
            Parallel.Invoke(
                () => place1 = AssignAppToPlace(place1, app1, null, null, isAddSilently: true),
                () => UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, TenantTitle.nolang)
            );
            TestStart(TestConfig.LoginUrl, false);
            Login(TestConfig.PermissionsUser, CurrentTenant);

            OpenEntityPage(place1);
            Assert.IsTrue(IsElementNotFound(PlacesPage.AppsSectionTableHeader),
                "Apps section should not be displayed");
            Assert.IsTrue(IsElementFound(PlacesPage.FaceApiKeyReadOnly),
                "Face API Key field should be shown");
            
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.LiveImagesButton)
                          && IsElementNotFoundQuickly(PageFooter.LiveImagesButtonInactive),
                "Live Images button should not be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be present in page footer");

            MouseOver(PageFooter.AddPlaceSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddExistingPlaceAsChildButton),
                "Add Existing Place As Child button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CreateNewChildPlaceButton),
                "Create New Child Place button should be present in page footer");

            EditForm();
            Assert.IsTrue(IsElementNotFound(PlacesPage.AppsSectionTableHeader), 
                "Apps section should not be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.FaceApiKey),
                "Face API Key field should be shown");

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            Assert.IsTrue(IsElementNotFound(PlacesPage.AppsSectionTableHeader),
                "Apps section should not be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.FaceApiKey),
                "Face API Key field should be shown");

            Click(PageFooter.CancelButton);
            ChangeTenant(TenantTitle.manylang);
            CurrentTenant = TenantTitle.manylang;
            OpenEntityPage(place2);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableHeader),
                "Apps section should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScheduleSectionRow1),
                "Schedule section should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ConfigurationSectionTitle),
                "Configuration section should be displayed");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.LiveImagesButtonInactive),
                "Live Images button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be present in page footer");

            EditForm();
            Click(PlacesPage.ExternalPowerOffWithUpsCheckBox);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place should be saved successfully");

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
                "CreateDevice",
                "ModifyDevice",
                "RestoreDevice",
                "DeleteDevice",
                "ViewAppSwitcher",
                "ViewEventLogMonitoring",
                "ViewLiveMonitoring",
                "ViewKinectMonitoring",
                "ViewOfflineMonitoring",
                "ViewHardwareMonitoring",
                "ViewAgentUpdate",
                "ViewAppExecution",
                "ViewFileTransfer");
            ChangeTenant(TenantTitle.nolang);
            OpenEntityPage(place1);
            Assert.IsTrue(IsElementFound(PlacesPage.AppSwitcherButton),
                "Configuration section: App Switcher button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.MonitoringButton),
                "Configuration section: Monitoring button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenButton),
                "Configuration section: Screen button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.UpdateButton),
                "Configuration section: Update button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.UploadGroupButton),
                "Configuration section: Monitoring button should be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableHeader),
                "Apps section should not be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScheduleSectionRow1),
                "Schedule section should not be displayed");

            EditForm();
            Assert.IsTrue(IsElementFound(PlacesPage.PingIntervalMinutesReadOnly),
                "Configuration section: Ping Interval should be read-only");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.FaceApiKey),
                "Configuration section: Face API Key field should be shown and can be modified");
            
            ClickUntilShown(PlacesPage.AppSwitcherButton, PlacesPage.ShowAppTitlesCheckBox);
            Assert.IsTrue(IsElementNotFound(PlacesPage.MaxValidPixels),
                "Configuration > App switcher > Max Valid Pixels should be read-only");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppSwitcherGestureDropDown),
                "Configuration > App switcher > App Switcher Gesture should be read-only");
            Click(PlacesPage.ShowAppTitlesCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.ShowAppTitlesCheckBox),
                "Configuration > App switcher > Show App Titles should be read-only");

            ClickUntilShown(PlacesPage.MonitoringButton, PlacesPage.AvailabilityButton);
            ClickUntilShown(PlacesPage.AvailabilityButton, PlacesPage.MonitoringCheckBox);
            Click(PlacesPage.MonitoringCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.MonitoringCheckBox),
                "Configuration > Monitoring > Availability > Monitoring should be read-only");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.MaxOfflineTimeHoursDropDown),
                "Configuration > Monitoring > Availability > Max Offline Time should be read-only");

            ClickUntilShown(PlacesPage.DepthImageButton, PlacesPage.WarningEnabledCheckBox);
            Click(PlacesPage.WarningEnabledCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.WarningEnabledCheckBox),
                "Configuration > Monitoring > Depth Image > Warning Enabled should be read-only");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AllowedBlackWarningMinutesDropDown),
                "Configuration > Monitoring > Depth Image > Allowed Black Warning should be read-only");
            Click(PlacesPage.ErrorEnabledCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.ErrorEnabledCheckBox),
                "Configuration > Monitoring > Depth Image > Error Enabled should be read-only");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AllowedBlackErrorMinutesDropDown),
                "Configuration > Monitoring > Depth Image > Allowed Black Error should be read-only");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.MinimumBrightness),
                "Configuration > Monitoring > Depth Image > Minimum Brightness should be read-only");

            Click(PlacesPage.EventLogButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.IntervalInSecondsMinutesReadOnly),
                "Configuration > Monitoring > Event Log > Interval In Seconds should be read-only");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.EventLogConfigurationsAddButton),
                "Configuration > Monitoring > Event Log > +Add button should not be shown");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementFound(PageFooter.DuplicateButton),
                "Duplicate button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be present in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.LiveImagesButton)
                          && IsElementNotFoundQuickly(PageFooter.LiveImagesButtonInactive),
                "Live Images button should not be shown in footer");

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
                "CreateDevice",
                "ModifyDevice",
                "RestoreDevice",
                "DeleteDevice",
                "ViewAppSwitcher",
                "ViewEventLogMonitoring",
                "ViewLiveMonitoring",
                "ViewKinectMonitoring",
                "ViewOfflineMonitoring",
                "ViewHardwareMonitoring",
                "ViewAgentUpdate",
                "ViewAppExecution",
                "ViewFileTransfer",
                "ChangeAppSwitcher", 
                "ChangeEventLogMonitoring", 
                "ChangeLiveMonitoring", 
                "ChangeKinectMonitoring", 
                "ChangeOfflineMonitoring", 
                "ChangeHardwareMonitoring", 
                "ChangeAgentUpdate", 
                "ChangeAppExecution", 
                "ChangeFileTransfer", 
                "ViewLiveImagesButton");
            RefreshPage();
            Assert.IsTrue(IsElementFound(PlacesPage.AppSwitcherButton),
                "Configuration section: App Switcher button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.MonitoringButton),
                "Configuration section: Monitoring button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenButton),
                "Configuration section: Screen button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.UpdateButton),
                "Configuration section: Update button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.UploadGroupButton),
                "Configuration section: Monitoring button should be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableHeader),
                "Apps section should not be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScheduleSectionRow1),
                "Schedule section should not be displayed");

            EditForm();
            Assert.IsTrue(IsElementFound(PlacesPage.FaceApiKey),
                "Configuration: all shown fields should be available for edit");

            ClickUntilShown(PlacesPage.AppSwitcherButton, PlacesPage.ShowAppTitlesCheckBox);
            Assert.IsTrue(IsElementFound(PlacesPage.MaxValidPixels),
                "Configuration > App switcher > Max Valid Pixels should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppSwitcherGestureDropDown),
                "Configuration > App switcher > App Switcher Gesture should be available for edit");
            Click(PlacesPage.ShowAppTitlesCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.ShowAppTitlesCheckBox),
                "Configuration > App switcher > Show App Titles should be available for edit");

            ClickUntilShown(PlacesPage.MonitoringButton, PlacesPage.AvailabilityButton);
            ClickUntilShown(PlacesPage.AvailabilityButton, PlacesPage.MonitoringCheckBox);
            Click(PlacesPage.MonitoringCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.MonitoringCheckBox),
                "Configuration > Monitoring > Availability > Monitoring should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.MaxOfflineTimeHoursDropDown),
                "Configuration > Monitoring > Availability > Max Offline Time should be available for edit");

            ClickUntilShown(PlacesPage.DepthImageButton, PlacesPage.WarningEnabledCheckBox);
            Click(PlacesPage.WarningEnabledCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.WarningEnabledCheckBox),
                "Configuration > Monitoring > Depth Image > Warning Enabled should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AllowedBlackWarningMinutesDropDown),
                "Configuration > Monitoring > Depth Image > Allowed Black Warning should be available for edit");
            Click(PlacesPage.ErrorEnabledCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.ErrorEnabledCheckBox),
                "Configuration > Monitoring > Depth Image > Error Enabled should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AllowedBlackErrorMinutesDropDown),
                "Configuration > Monitoring > Depth Image > Allowed Black Error should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.MinimumBrightness),
                "Configuration > Monitoring > Depth Image > Minimum Brightness should be available for edit");

            Click(PlacesPage.EventLogButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.IntervalInSecondsMinutesDropDown),
                "Configuration > Monitoring > Event Log > Interval In Seconds should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.EventLogConfigurationsAddButton),
                "Configuration > Monitoring > Event Log > +Add button should be shown");

            Click(PlacesPage.EventLogConfigurationsRow1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.EventLogConfigurationsRow1DetailsSource),
                "Configuration > Monitoring > Event Log > Event Log Configurations > Row 1 > " + 
                "Source should be available for edit");

            ClickUntilShown(PlacesPage.HardwareButton, PlacesPage.HardwareMonitorConfigurationRow1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.HardwareMonitorConfigurationAddButton),
                "Configuration > Monitoring > Hardware > +Add button should be shown");

            Click(PlacesPage.HardwareMonitorConfigurationRow1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.HardwareRow1DetailsAlertErrorCode),
                "Configuration > Monitoring > Hardware > Row 1 > Alert Error Code should be available for edit");

            ClickUntilShown(PlacesPage.LiveImagesButton, PlacesPage.LiveImagesEnableCheckBox);
            Click(PlacesPage.LiveImagesEnableCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.LiveImagesEnableCheckBox),
                "Configuration > Monitoring > Live Images > Enable checkbox should be available for edit");

            ClickUntilShown(PlacesPage.ScreenButton, PlacesPage.OverlappingButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenWidth),
                "Configuration > Screen > Screen Width should be available for edit");

            ClickUntilShown(PlacesPage.OverlappingButton, PlacesPage.OverlappingForProjectorsCheckBox);
            Click(PlacesPage.OverlappingForProjectorsCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.OverlappingForProjectorsCheckBox),
                "Configuration > Screen > Overlapping > Overlapping For Projectors checkbox " +
                "should be available for edit");

            Click(PlacesPage.UpdateButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AgentUpdateDropDown),
                "Configuration > Update > Enable checkbox should be available for edit");

            ClickUntilShown(PlacesPage.UploadGroupButton, PlacesPage.EnabledCheckBox);
            Click(PlacesPage.EnabledCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.EnabledCheckBox),
                "Configuration > Upload > Enabled checkbox should be available for edit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.FileUploadConfigurationsAddButton),
                "Configuration > Upload > +Add button should be shown");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place should be saved successfully");

            Assert.IsTrue(IsElementFound(PageFooter.DuplicateButton),
                "Duplicate button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "+Add button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be present in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.LiveImagesButtonInactive),
                "Live Images button should not be shown in footer");
        }

        [Test, Regression]
        public void RT10040_PlacesAppsSection()
        {
            Place place1 = null, place2 = null;
            AppResponse app1 = null, app2 = null;
            CurrentTenant = TenantTitle.manylang;
            Parallel.Invoke(
                () => AddAppPlayer(),
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

        [Test, Regression]
        public void RT10050_PlacesAppSectionSpecial()
        {
            Place place1 = null;
            AppResponse app1 = null;
            CurrentTenant = TenantTitle.nolang;
            Parallel.Invoke(
                () => AddAppPlayer(),
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
            place1 = AssignAppToPlace(place1, app1, null, null, isAddSilently: true);
            CurrentUser = TestConfig.PermissionsUser;
            TestStart();

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

        [Test, Regression]
        public void RT10060_PlacesFooterButtons()
        {
            Place place1 = null;
            AppResponse app1 = null;
            CurrentTenant = TenantTitle.nolang;
            Parallel.Invoke(
                () => AddAppPlayer(),
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
            place1 = AssignAppToPlace(place1, app1, null, null, isAddSilently: true);
            CurrentUser = TestConfig.PermissionsUser;
            TestStart();

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

        [Test, Regression]
        public void RT10070_PlacesDeviceDropDown()
        {
            Place place1 = null;
            AppResponse app1 = null;
            CurrentTenant = TenantTitle.nolang;
            Parallel.Invoke(
                () => AddAppPlayer(),
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
            place1 = AssignAppToPlace(place1, app1, null, null, isAddSilently: true);
            UserDirectoryApi.AddRoleToUser(TestConfig.PermissionsUser, _role, CurrentTenant);
            CurrentUser = TestConfig.PermissionsUser;
            TestStart();

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
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, place1.Device.Name),
                "WW field should contain 1 device assigned previously by admin");           
            Click(PageFooter.CancelButton);

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

        [Test, Regression]
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

        [Test, Regression]
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

        [Test, Regression]
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

        [Test, Regression]
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

            RefreshPage();
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

        [Test, Regression]
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

        [Test, Regression]
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
