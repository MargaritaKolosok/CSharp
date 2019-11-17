using System.Collections.Generic;
using System.Drawing;
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
    public sealed class RTC04C_PlacesWwTests : ParentTest
    {
        private const string WwName1 = "1PSVJ0D4";
        private const string WwName2 = "DQZJF062";
        private const string RackNumber = "1P";
        private const string ScheduleDayAnyDay = "Any Day";
        private const string ScheduleAppNonePowerOff = "None (Power off)";
        private const string FaceApiKey = "343dc163891c4b8a85c37cbbfae89be6";

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
        }

        [Test, Regression]
        public void RT04240_CreateWwPlace()
        {
            CurrentTenant = TenantTitle.nolang;
            TestStart();

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.WindowsWorkstationFieldName),
                "Field Windows Workstation should appear");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.FaceApiKey),
                "Field Face Api Key should appear");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PingIntervalMinutesDropDown),
                "Field Ping Interval should appear");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.KillTimeoutMinutesDropDown),
                "Field Kill Timeout should appear");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.RebootIntervalDropDown),
                "Field Reboot Interval should appear");

            Assert.IsTrue(IsElementEquals(PlacesPage.PingIntervalMinutesDropDown, "0 m"),
                @"Ping Interval - Minutes should be '0 m' by default");
            Assert.IsTrue(IsElementEquals(PlacesPage.PingIntervalSecondsDropDown, "15 s"),
                @"Ping Interval - Seconds should be '15 s' by default");
            Click(PlacesPage.ScreenButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.ScreenWidth, "1080"),
                @"Screen width should be '1080 px' by default");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                @"'+ Add' button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.UploadDayButton),
                "Upload Day button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be shown in footer");

            var title = $"Auto test {RandomNumber}";
            SendText(PlacesPage.Title, title);
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place in edit mode after submit");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $@"Place Status should be '{StatusNoDevice}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, string.Empty),
                "Windows Workstation field should be empty");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "'+ Add' button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FileAccessButton),
                "File Access button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.LiveImagesButtonInactive),
                "Inactive Live Images button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be shown in footer");
        }

        [Test, Regression]
        public void RT04250_AddWwDeviceToPlace()
        {
            CurrentTenant = TenantTitle.nolang;
            TestStart();
            var place = AddPlaceWw(PlaceStatus.Active);

            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, string.Empty),
                "Windows Workstation field should be empty");

            EditForm();
            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementFound(Devices.TableRow),
                "Devices dialog: available devices should be listed");

            Click(string.Format(Devices.TableRowByText, RackNumber), true, 1);
            Click(string.Format(Devices.TableRowByText, WwName1), true, 1);
            Assert.IsTrue(AreElementsContainText(PlacesPage.WindowsWorkstation, WwName1) ||
                          AreElementsContainText(PlacesPage.WindowsWorkstation, RackNumber),
                $@"Windows Workstation should contain value '{WwName1}' or '{RackNumber}'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.WindowsWorkstationDetailsButton),
                @"Devices dialog should have button '...'");

            Click(PlacesPage.WindowsWorkstationDetailsButton);
            Assert.IsTrue(IsElementFound(Devices.CancelButton),
                "Devices modal dialog not opened");
            Assert.IsTrue(IsElementFoundQuickly(Devices.ClearSelectionButton),
                "Not found Clear Selection button in Devices dialog");
            Assert.IsTrue(IsElementFoundQuickly(Devices.TableRow),
                "Devices dialog table empty");

            Click(Devices.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(Devices.TableRow),
                @"Devices dialog is still opened");
            Assert.IsTrue(IsEditMode(), "Place is not in edit mode");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place is still in edit mode");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusUnknown), 
                $"Place Status should be {StatusUnknown}");
            Assert.IsTrue(AreElementsContainText(PlacesPage.WindowsWorkstation, WwName1) ||
                          AreElementsContainText(PlacesPage.WindowsWorkstation, RackNumber),
                $@"Windows Workstation field should contain '{WwName1}' or '{RackNumber}'");

            EditForm();
            OpenDevicesFromMenu();
            Click(string.Format(Devices.TableRowByText, RackNumber), true, 1);
            Click(string.Format(Devices.TableRowByText, WwName1), true, 1);
            SendText(Devices.RackNumber, RackNumber);
            Click(Devices.SubmitButtonDevice);
            Assert.IsTrue(IsElementFound(string.Format(Devices.TableRowByText, RackNumber)),
                $"Devices dialog should contain '{RackNumber}'");

            OpenDevicesFromPlace();
            Click(string.Format(Devices.TableRowByText, RackNumber));
            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, RackNumber),
                $@"Windows Workstation field should be '{RackNumber}'");
        }

        [Test, Regression]
        public void RT04270_AddApp()
        {
            Parallel.Invoke(
                () => AddAppDpt(AppStatus.Any, TestConfig.DptAppVersions[0]),
                () => AddAppIbeacon(TestConfig.IbeaconAppVersions[0]),
                () => AddAppPlayer()
            );
            AddAppComposerHq1();
            TestStart();
            AddPlaceWw(PlaceStatus.Active);

            MouseOver(PageFooter.AddPlaceSubMenu);
            Assert.IsTrue(IsElementFound(PageFooter.AddExistingPlaceAsChildButton),
                @"'Add existing place as child' button not found");
            Assert.IsTrue(IsElementFound(PageFooter.CreateNewChildPlaceButton),
                @"'Create new child place' button not found");
            Assert.IsTrue(IsElementFound(PageFooter.AddAppInPlacesButton),
                @"'Add app' button not found");

            Click(PageFooter.AddAppInPlacesButton);
            Assert.IsTrue(!AreElementsContainText(
                              PlacesPage.TableSelectAppTitleColumnValues, AppTitle.Ibeacon) 
                          && !AreElementsContainText(PlacesPage.TableSelectAppTitleColumnValues, AppTitle.Dpt),
                "Only Composer (and Player) apps should be available in modal dialog");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.TableSelectAppTitle, AppTitle.ComposerHq1)),
                "Composer HQ app 1 should be available in modal dialog");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.TableSelectAppTitle, AppTitle.Player)),
                "Composer Player app should be available in modal dialog");

            Click(string.Format(PlacesPage.TableSelectAppTitle, AppTitle.ComposerHq1));
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRow1),
                "Composer HQ app 1 not added to place");
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetails),
                "Apps section - app table row is collapsed");
            Assert.IsTrue(IsEditMode(), "Place is in view mode");

            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsImage),
                "App image is not displayed");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsTitle, 
                    $"{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})" ),
                $@"Apps details Title value should be '{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsAppPackageDropDown,
                    $"{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})"),
                $@"Apps details App Package value should be '{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})'");
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.LivePreviewCheckBox),
                @"Apps details Live Preview value should be 'Off'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsDiagnosticsButton),
                "Diagnostics button is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsDownloadButton),
                "Download button is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsInteractionButton),
                "Interaction button button is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsScreenButton),
                "Screen button is not displayed");

            Assert.IsTrue(IsCheckBoxOff(PlacesPage.EnabledCheckBox),
                @"Schedule details Enabled value should be 'Off'");

            Click(PlacesPage.AppsSectionRow1DetailsInteractionButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.IdleTimeoutMinutesDropDown, "0 m"),
                @"Interaction - Idle Timeout value should be '0m 10s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.IdleTimeoutSecondsDropDown, "10 s"),
                @"Interaction - Idle Timeout value should be '0m 10s'");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                "'+ Add' button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.UploadDayButton),
                "Upload Day button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be shown in footer");

            SubmitForm();
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRowDetails),
                "Apps section details should be expanded after submit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRowDetailsImage),
                "App image is not displayed");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsTitleReadOnly,
                    $"{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})"),
                $@"Apps details Title value should be '{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsAppPackageReadOnly,
                    $"{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})"),
                $@"Apps details App Package value should be '{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})'");
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.LivePreviewCheckBox),
                @"Apps details Live Preview value should be 'Off'");
            Assert.IsTrue(IsElementEquals(PlacesPage.IdleTimeoutMinutesReadOnly, "0 m"),
                @"Interaction - Idle Timeout value should be '0m 10s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.IdleTimeoutSecondsReadOnly, "10 s"),
                @"Interaction - Idle Timeout value should be '0m 10s'");
        }

        [Test, Regression]
        public void RT04280_AddSecondApp()
        {
            Place place = null;
            AppResponse app1 = null, app2 = null;
            Parallel.Invoke(
                () => AddAppPlayer(),
                () => place = AddPlaceWw(PlaceStatus.Active, pageToBeOpened: 0)
            );
            Parallel.Invoke(
                () => app1 = AddAppComposerHq1(),
                () => app2 = AddAppComposerHq2()
            );
            app2 = AssignImageToApp(app2);
            TestStart();
            place = AssignAppToPlace(place, app1, null, null);

            EditForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(string.Format(PlacesPage.TableSelectAppTitle, AppTitle.Player));
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRow2),
                "Assigned Player app should be shown at table row 2");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRowDetails),
                "Preloaded app details section should not be shown in row 1");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorAppPackageRow2Required),
                @"Error 'App package is required' should be displayed");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                @"'+ Add' button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.UploadDayButton),
                "Upload Day button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AppsOrderButton),
                "Apps Order button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be shown in footer");

            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsEditMode(), "Submit button should be inactive");

            ClickUntilShown(PlacesPage.AppsSectionTableRow2DetailsAppPackageDropDown, 
                CommonElement.DropDownOptionList);
            var app1FullName = $"{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})";
            var app2FullName = $"{AppTitle.ComposerHq2} ({TestConfig.ComposerHqApp2Version})";
            Assert.IsTrue(
                IsCollectionContainsCollection(
                    GetElementsText(CommonElement.DropDownOptionList),
                    new[] { app1FullName, app2FullName }),
                "App Package drop-down should contain two Composer HQ apps");

            DropDownSelect(PlacesPage.AppsSectionTableRow2DetailsAppPackageDropDown, app2FullName);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow2DetailsAppPackageDropDown, app2FullName),
                $@"App Package drop-down should be set to '{app2FullName}'");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorAppPackageRow2Required),
                @"Error 'App package is required' should be hidden");

            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow2DetailsImage),
                @"Image should be displayed");

            SubmitForm();
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRow2),
                "Assigned Player app should be shown at table row 2");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRowDetails),
                "Preloaded app details section should not be shown in row 1");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow2DetailsAppPackageReadOnly, app2FullName),
                $@"App Package drop-down should be set to '{app2FullName}'");

            RefreshPage();
            Assert.IsTrue(IsElementNotFound(PlacesPage.AppsSectionTableRowDetails),
                "App row 1 details section should be hidden");
            Assert.IsTrue(IsElementNotFound(PlacesPage.AppsSectionTableRow2Details),
                "App row 2 details section should be hidden");

            Click(PlacesPage.AppsSectionTableRow2);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRow2Details),
                "App row 2 details section should be shown on row 2 click");

            Click(PlacesPage.AppsSectionTableRow2DetailsImage);
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}/app/{app2.AppId}"),
                "App page is not opened on app image click");
        }

        [Test, Regression]
        public void RT04290_AddAnotherTheSameApp()
        {
            Place place = null;
            AppResponse app1 = null, app2 = null;
            AddAppPlayer();
            Parallel.Invoke(
                () => app1 = AddAppComposerHq1(),
                () => app2 = AddAppComposerHq2()
            );
            Parallel.Invoke(
                () => app2 = AssignImageToApp(app2),
                () => place = AddPlaceWw(PlaceStatus.Active, isCreateNewPlace: true, pageToBeOpened: 0)
            );
            TestStart();
            place = AssignAppToPlace(place, app1, null, null, isAddSilently: true);
            place = AssignAppToPlace(place, app2, null, null);

            EditForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(string.Format(PlacesPage.TableSelectAppTitle, AppTitle.ComposerHq2));
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRow3),
                "Assigned Composer HQ 2 app should be shown at table row 3");

            var title1 = $"{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})";
            var title2 = $"{AppTitle.ComposerHq2} ({TestConfig.ComposerHqApp2Version})";
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow3TitleCell, title2),
                $@"Apps section: row 3 Title should be '{title2}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow3AppPackageCell, title2),
                $@"Apps section: row 3 App/Package should be like Title: '{title2}'");

            var newTitle1 = $"Auto test {RandomNumber}";
            SendText(PlacesPage.AppsSectionTableRow3DetailsTitle, newTitle1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow3DetailsPlayerReadOnly),
                "Apps section: row 3 Player field can be edited");

            DropDownSelect(PlacesPage.AppsSectionTableRow3DetailsAppPackageDropDown, AppTitle.ComposerHq1);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow3DetailsTitle, newTitle1),
                "Apps section, row 3 Title in details should not change after App package change");
            Assert.IsTrue(IsElementEquals(
                    PlacesPage.AppsSectionTableRow3DetailsVersionDropDown, TestConfig.PlayerAppVersions[0]),
                "Apps section, row 3 Title in details should not change after App package change");

            var placeAppTitles = GetElementsText(PlacesPage.AppsSectionTableRowsColumnTitle).Skip(1);
            Click(PageFooter.AppsOrderButton);
            Assert.IsTrue(IsElementFound(PlacesPage.ChangeOrderOfAppsDialog),
                "Dialog 'change order of apps' is not shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.CancelButton),
                "Dialog 'change order of apps' does not have Cancel button");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DoneButton),
                "Dialog 'change order of apps' does not have Done button");
            var modalAppsTitles = GetElementsText(PlacesPage.ChangeOrderOfAppsDialogTableColumnTitle);
            Assert.IsTrue(IsCollectionContainsCollection(placeAppTitles, modalAppsTitles),
                "Apps in modal dialog should be the same as in Apps section and be in the same order");

            DragAndDrop(PlacesPage.ChangeOrderOfAppsDialogTableRowLast, 
                PlacesPage.ChangeOrderOfAppsDialogTableRowFirst);
            Assert.IsTrue(IsElementEquals(
                    PlacesPage.ChangeOrderOfAppsDialogTableColumnTitle, newTitle1),
                $"Last row '{newTitle1}' has not been dragged to first row's placeholder in modal dialog");

            Click(PlacesPage.DoneButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.ChangeOrderOfAppsDialog),
                "Dialog 'change order of apps' is still shown");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRow1TitleCell, newTitle1),
                "Last row has not been moved to first row's placeholder in Apps section");

            Click(PlacesPage.AppsSectionTableRow2);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow2Details),
                "Apps section: row 2 details should be shown");

            Click(PlacesPage.AppsSectionTableRow2DeleteButton);
            Assert.IsFalse(IsElementEquals(PlacesPage.AppsSectionTableRow2TitleCell, title1),
                "Apps section row 2 is not deleted");

            Click(PageFooter.AppsOrderButton);
            Assert.IsFalse(AreElementsContainText(
                    PlacesPage.ChangeOrderOfAppsDialogTableColumnTitle, title1),
                "Dialog 'change order of apps' should not contain deleted app");

            DragAndDrop(PlacesPage.ChangeOrderOfAppsDialogTableRowLast, PlacesPage.ChangeOrderOfAppsDialogTableRowFirst);
            Click(PlacesPage.CancelButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.ChangeOrderOfAppsDialog),
                "Dialog 'change order of apps' is still shown");

            SubmitForm();
            Assert.IsTrue(CountElements(PlacesPage.AppsSectionTableRowsColumnTitle) == 3,
                "Apps section should contain 2 apps");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                @"'+ Add' button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FileAccessButton),
                "File Access button should be shown in footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.AppsOrderButton),
                "Apps Order button should be absent in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.LiveImagesButtonInactive),
                "Inactive Live Images button should be shown in footer");

            EditForm();
            ClickUntilShown(PlacesPage.AppsSectionTableRow1, PlacesPage.AppsSectionTableRow1DeleteButton);
            Click(PlacesPage.AppsSectionTableRow1DeleteButton);
            ClickUntilShown(PlacesPage.AppsSectionTableRow1, PlacesPage.AppsSectionTableRow1DeleteButton);
            Click(PlacesPage.AppsSectionTableRow1DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.EnabledCheckBox),
                "Schedule section is still displayed after all apps were deleted");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableHeader),
                "Apps section is still displayed after all apps were deleted");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.AppsOrderButton),
                "Apps Order button should be absent in footer");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableHeader),
                "Apps section is not displayed after all changes cancel");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.EnabledCheckBox),
                "Schedule section is not displayed after all changes cancel");
        }

        [Test, Regression]
        public void RT04300_ScheduleSettings()
        {
            AppResponse app1 = null, app2 = null;
            Place place = null;
            AddAppPlayer();
            Parallel.Invoke(
                () => app1 = AddAppComposerHq1(),
                () => app2 = AddAppComposerHq2(),
                () => place = AddPlaceWw(PlaceStatus.Active, isAssignDevice: true, pageToBeOpened: 0)
            );
            app2 = AssignImageToApp(app2);
            TestStart();
            place = AssignAppToPlace(place, app1, null, null, isAddSilently: true);
            place = AssignAppToPlace(place, app2, null, null);

            Click(PlacesPage.ScheduleSectionRow1);
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsDayReadOnly, ScheduleDayAnyDay),
                $@"Schedule section - row 1 field Day should be '{ScheduleDayAnyDay}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsAppReadOnly, ScheduleAppNonePowerOff),
                $@"Schedule section - row 1 field App should be '{ScheduleAppNonePowerOff}'");

            EditForm();
            Click(PlacesPage.EnabledCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.EnabledCheckBox),
                "Schedule section - Enabled checkbox should be on");
            Click(PlacesPage.ScheduleSectionDetailsAddUpButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart1HoursDropDown, "8 h")
                    && IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart1MinutesDropDown, "0 m"),
                @"Schedule section - First Time should be '8 h 0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart2HoursDropDown, "20 h")
                          && IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart2MinutesDropDown, "0 m"),
                @"Schedule section - Second Time should be '20 h 0 m'");
            var compApp1FullName = $"{AppTitle.ComposerHq1} ({TestConfig.ComposerHqApp1Version})";
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsAppDropDown, compApp1FullName),
                $@"Schedule section - App drop-down should be '{compApp1FullName}'");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart1HoursReadOnly, "8 h")
                    && IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart1MinutesReadOnly, "0 m"),
                @"Schedule section - First Time should be '8 h 0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart2HoursReadOnly, "20 h")
                    && IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart2MinutesReadOnly, "0 m"),
                @"Schedule section - Second Time should be '20 h 0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsAppReadOnly, compApp1FullName),
                $@"Schedule section - App drop-down should be '{compApp1FullName}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusUnknown)
                    || IsElementEquals(PlacesPage.Status, StatusIdle),
                $@"Place should have Status = {StatusUnknown} or {StatusIdle}");

            Click(PageFooter.DuplicateButton);
            Assert.IsTrue(IsEditMode(), "Place duplicate should be in edit mode");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $@"Place duplicate should have Status = '{StatusNoDevice}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.DeviceTypeDropDown, DeviceTypeWw),
                $@"Place duplicate should have Device Type = '{DeviceTypeWw}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, string.Empty),
                "Place duplicate should have empty Devices dialog");

            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            SubmitForm();
            Click(PlacesPage.ScheduleSectionRow1);
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart1HoursReadOnly, "8 h")
                          && IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart1MinutesReadOnly, "0 m"),
                @"Schedule section - First Time should be '8 h 0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart2HoursReadOnly, "20 h")
                          && IsElementEquals(PlacesPage.ScheduleSectionDetailsTimeStart2MinutesReadOnly, "0 m"),
                @"Schedule section - Second Time should be '20 h 0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ScheduleSectionDetailsAppReadOnly, compApp1FullName),
                $@"Schedule section - App drop-down should be '{compApp1FullName}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $@"Place should have Status = '{StatusNoDevice}'");
        }

        [Test, Regression]
        public void RT04310_DeleteRestoreWhenSchedulerIsSet()
        {
            AppResponse app1 = null, app2 = null;
            Place place = null;
            AddAppPlayer();
            Parallel.Invoke(
                () => app1 = AddAppComposerHq1(),
                () => app2 = AddAppComposerHq2(),
                () => place = AddPlaceWw(PlaceStatus.Active, isAssignDevice: true, pageToBeOpened: 0, 
                    isCreateNewPlace: true)
            );
            TestStart();
            place = AssignAppToPlace(place, app1, null, null, isAddSilently: true);
            place = AssignAppToPlace(place, app2, null, null);
            if (IsCheckBoxOff(PlacesPage.EnabledCheckBox))
            {
                EditForm();
                Click(PlacesPage.EnabledCheckBox);
                SubmitForm();
            }

            var device = place.Device.Name;
            Click(PageFooter.DeleteButton);
            Assert.IsTrue(IsElementFound(PlacesPage.DeleteDialog), "Delete place dialog is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.CancelButton),
                "Cancel button should be shown in place delete confirmation dialog");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DeleteButton),
                "Delete button should be shown in place delete confirmation dialog");

            Click(PlacesPage.DeleteButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.PlacesUri),
                "Place main page should be displayed");

            Click(PageFooter.ShowDeletedButton, ignoreIfNoElement: true, 3);
            Click(string.Format(PlacesPage.TableRowByTitle, place.Title));
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusDeleted),
                $@"Place should have Status '{StatusDeleted}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, string.Empty),
                "Windows Workstation field should be empty");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow1),
                "Apps section should be present");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScheduleSectionRow1),
                "Schedule section should be present");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenuInactive),
                @"'+ Add' button should be present in footer and inactive");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FileAccessButtonInactive),
                @"'File access' button should be present in footer and inactive");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.LiveImagesButtonInactive),
                @"'Live Images' button should be present in footer and inactive");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButtonInactive),
                @"'Duplicate' button should be present in footer and inactive");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.RestoreButton),
                @"'Restore' button should be present in footer and active");

            Click(PageFooter.RestoreButton);
            Assert.IsTrue(IsEditMode(), "Place should be in edit mode after restoration");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $@"Place should have Status '{StatusNoDevice}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, string.Empty),
                "Windows Workstation field should be empty");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceSubMenu),
                @"'+ Add' button should be present in footer and active");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.UploadDayButton),
                @"'Upload day' button should be present in footer and active");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AppsOrderButton),
                @"'Apps order' button should be present in footer and active");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                @"'Cancel' button should be present in footer and active");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                @"'Submit' button should be present in footer and active");

            OpenDevicesFromPlace();
            Assert.IsTrue(IsElementEquals(string.Format(Devices.TableRowPlaceByTitle, device), string.Empty),
                $"Device {device} should be shown without place assigned");
          
            Click(string.Format(Devices.TableRowByText, device));
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place should be in view mode after restoration and submit");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusUnknown) || IsElementEquals(PlacesPage.Status, StatusIdle),
                $@"Place should have Status '{StatusUnknown}' or '{StatusIdle}'");
        }

        [Test, Regression]
        public void RT04311_PropertiesDefaultValues()
        {
            TestStart();

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            Assert.IsTrue(IsElementEquals(PlacesPage.FaceApiKey, FaceApiKey),
                $@"Configuration > Face Api Key should be '{FaceApiKey}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.PingIntervalMinutesDropDown, "0 m"),
                @"Configuration > Ping Interval (minutes) should be '0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.PingIntervalSecondsDropDown, "15 s"),
                @"Configuration > Ping Interval (seconds) should be '15 s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.KillTimeoutMinutesDropDown, "0 m"),
                @"Configuration > Kill Timeout (minutes) should be '0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.KillTimeoutSecondsDropDown, "15 s"),
                @"Configuration > Kill Timeout (seconds) should be '15 s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.RebootIntervalDropDown, "2 days"),
                @"Configuration > Reboot Interval should be '2 days'");

            ClickUntilShown(PlacesPage.AppSwitcherButton, PlacesPage.MaxValidPixels);
            Assert.IsTrue(IsElementEquals(PlacesPage.MaxValidPixels, "5"),
                @"App Switcher > Max Valid Pixels should be '5 %'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppSwitcherGestureDropDown, "No Depth"),
                @"App Switcher > App Switcher Gesture should be 'No Depth'");
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.ShowAppTitlesCheckBox),
                @"App Switcher > Show App Titles should be 'On'");
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.AllowSwitchingScheduledAppCheckBox),
                @"App Switcher > Allow Switching Scheduled App should be 'Off'");

            ClickUntilShown(PlacesPage.DistributionButton, PlacesPage.DistributionInternetGateway);
            Assert.IsTrue(IsElementEquals(PlacesPage.DistributionLocalDistributionPort, "8444"),
                "Distribution > Local Distribution Port should be '8444'");
            Assert.IsTrue(IsElementEquals(PlacesPage.DistributionLocalDistributionThresholdDropDown, "> 100 KB"),
                @"Distribution > Local Distribution Threshold should be '> 100 KB'");
            Assert.IsTrue(IsElementEquals(PlacesPage.DistributionInternetGateway, string.Empty),
                "Distribution > Internet Gateway should be empty");

            ClickUntilShown(PlacesPage.MonitoringButton, PlacesPage.AvailabilityButton);
            ClickUntilShown(PlacesPage.AvailabilityButton, PlacesPage.MonitoringCheckBox);
            Assert.IsTrue(IsElementEquals(PlacesPage.MaxOfflineTimeHoursDropDown, "0 h"),
                @"Monitoring > Availability > Max Offline Time (hours) should be '0 h'");
            Assert.IsTrue(IsElementEquals(PlacesPage.MaxOfflineTimeMinutesDropDown, "0 m"),
                @"Monitoring > Availability > Max Offline Time (minutes) should be '0 m'");
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.MonitoringCheckBox),
                @"Monitoring > Availability > Monitoring should be 'Off'");

            ClickUntilShown(PlacesPage.DepthImageButton, PlacesPage.WarningEnabledCheckBox);
            Assert.IsTrue(IsElementEquals(PlacesPage.AllowedBlackErrorMinutesDropDown, "1 m"),
                @"Monitoring > Depth image > Allowed Black Error (minutes) should be '1 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AllowedBlackErrorSecondsDropDown, "30 s"),
                @"Monitoring > Depth image > Allowed Black Error (seconds) should be '30 s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AllowedBlackWarningMinutesDropDown, "0 m"),
                @"Monitoring > Depth image > Allowed Black Warning (minutes) should be '0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AllowedBlackWarningSecondsDropDown, "20 s"),
                @"Monitoring > Depth image > Allowed Black Warning (seconds) should be '20 s'");
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.WarningEnabledCheckBox),
                @"Monitoring > Depth image > Warning Enabled should be 'On'");
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.ErrorEnabledCheckBox),
                @"Monitoring > Depth image > Error Enabled should be 'On'");
            Assert.IsTrue(IsElementEquals(PlacesPage.MinimumBrightness, "1"),
                @"Monitoring > Depth image > MinimumBrightness should be '1'");

            ClickUntilShown(PlacesPage.EventLogButton, PlacesPage.IntervalInSecondsMinutesDropDown);
            Assert.IsTrue(IsElementEquals(PlacesPage.IntervalInSecondsMinutesDropDown, "0 m"),
                @"Monitoring > Depth image > Interval In Seconds (minutes) should be '0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.IntervalInSecondsSecondsDropDown, "30 s"),
                @"Monitoring > Depth image > Interval In Seconds (seconds) should be '30 s'");
            var rows = new List<string>();
            for (var i = 1; i <= 4; i++)
            {
                rows.Add(GetValuesAsString(string.Format(PlacesPage.EventLogConfigurationsRow, i)));
            }
            Assert.IsTrue(rows[0].Contains("System.Error"),
                @"Event Log > Event Log Configurations > row 1, Alert Error Code should be 'System.Error'");
            Assert.IsTrue(rows[1].Contains("Application.Error"),
                @"Event Log > Event Log Configurations > row 2, Alert Error Code should be 'Application.Error'");
            Assert.IsTrue(rows[2].Contains("DeviceService.LowFrameRate"),
                @"Event Log > Event Log Configurations > row 3, Alert Error Code should be 'DeviceService.LowFrameRate'");
            Assert.IsTrue(rows[3].Contains("Display.Error"),
                @"Event Log > Event Log Configurations > row 4, Alert Error Code should be 'Display.Error'");

            ClickUntilShown(PlacesPage.EventLogConfigurationsRow1, 
                PlacesPage.EventLogConfigurationsRow1DetailsAlertErrorCode);
            Assert.IsTrue(IsElementEquals(PlacesPage.EventLogConfigurationsRow1DetailsAlertErrorCode, "System.Error"),
                @"Event Log > Event Log Configurations > row 1 details > Alert Error Code should be 'System.Error'");
            Assert.IsTrue(IsElementEquals(PlacesPage.EventLogConfigurationsRow1DetailsAlertErrorMessage, 
                    "Error entry in system event log"),
                "Event Log > Event Log Configurations > row 1 details > Alert Error Message should be " + 
                @"'Error entry in system event log'");
            Assert.IsTrue(IsElementEquals(PlacesPage.EventLogConfigurationsRow1DetailsAlertLevelDropDown, "Warning"),
                @"Event Log > Event Log Configurations > row 1 details > Alert Level should be 'Warning'");
            Assert.IsTrue(IsElementEquals(PlacesPage.EventLogConfigurationsRow1DetailsMinEventLogLevelDropDown, "Error"),
                @"Event Log > Event Log Configurations > row 1 details > Min Event Log Level should be 'Error'");
            Assert.IsTrue(IsElementEquals(PlacesPage.EventLogConfigurationsRow1DetailsEventLogDropDown, "System"),
                @"Event Log > Event Log Configurations > row 1 details > Event Log should be 'System'");
            Assert.IsTrue(IsElementEquals(PlacesPage.EventLogConfigurationsRow1DetailsSource, string.Empty),
                "Event Log > Event Log Configurations > row 1 details > Source should be empty");
            Assert.IsTrue(IsElementEquals(PlacesPage.EventLogConfigurationsRow1DetailsErrorMessagePattern, string.Empty),
                "Event Log > Event Log Configurations > row 1 details > Error Message Pattern should be empty");

            ClickUntilShown(PlacesPage.HardwareButton, PlacesPage.HardwareMonitorConfigurationAddButton);
            rows = new List<string>();
            for (var i = 1; i <= 4; i++)
            {
                rows.Add(GetValuesAsString(string.Format(PlacesPage.HardwareMonitorConfigurationRow, i)));
            }
            Assert.IsTrue(rows[0].Contains("CPU.Load.High"),
                @"Hardware > row 1, Alert Error Code should be 'CPU.Load.High'");
            Assert.IsTrue(rows[1].Contains("Disk.Space.Low"),
                @"Hardware > row 2, Alert Error Code should be 'Disk.Space.Low'");
            Assert.IsTrue(rows[2].Contains("Disk.Space.Low"),
                @"Hardware > row 3, Alert Error Code should be 'Disk.Space.Low'");
            Assert.IsTrue(rows[3].Contains("Screen.SizeTooWide"),
                @"Hardware > row 4, Alert Error Code should be 'Screen.SizeTooWide'");

            ClickUntilShown(string.Format(PlacesPage.HardwareMonitorConfigurationRow, 3), 
                PlacesPage.HardwareRow3DetailsAlertErrorCode);
            Assert.IsTrue(IsElementEquals(PlacesPage.HardwareRow3DetailsAlertErrorCode, "Disk.Space.Low"),
                @"Hardware > row 3 details > Alert Error Code should be 'Disk.Space.Low'");
            Assert.IsTrue(IsElementEquals(PlacesPage.HardwareRow3DetailsAlertErrorMessage,
                    "Less than 2% of space available on the hard disk"),
                "Hardware > row 3 details > Alert Error Message should be " +
                @"'Less than 2% of space available on the hard disk'");
            Assert.IsTrue(IsElementEquals(PlacesPage.HardwareRow3DetailsAlertLevelDropDown, "Error"),
                @"Hardware > row 3 details > Alert Level should be 'Warning'");
            Assert.IsTrue(IsElementEquals(PlacesPage.HardwareRow3MinAllowedValue, "0"),
                @"Hardware > row 3 details > Min Allowed Value should be '0'");
            Assert.IsTrue(IsElementEquals(PlacesPage.HardwareRow3MaxAllowedValue, "98"),
                @"Hardware > row 3 details > Max Allowed Value should be '98'");
            Assert.IsTrue(IsElementEquals(PlacesPage.HardwareRow3DetailsSensorDropDown, "Disk space used %"),
                @"Hardware > row 3 details > Sensor should be 'Disk space used %'");
            Assert.IsTrue(IsElementEquals(PlacesPage.HardwareRow3AllowedExceedingSecondsMinutesDropDown, "0 m"),
                @"Hardware > row 3 details > Allowed Exceeding Seconds (minutes) should be '0 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.HardwareRow3AllowedExceedingSecondsSecondsDropDown, "15 s"),
                @"Hardware > row 3 details > Allowed Exceeding Seconds (seconds) should be '15 s'");

            ClickUntilShown(PlacesPage.LiveImagesButton, PlacesPage.LiveImagesEnableCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.LiveImagesEnableCheckBox), @"Live Images > Enable should be 'On'");
            Assert.IsTrue(IsElementEquals(PlacesPage.LiveImagesCompressionQuality, "0.3"),
                @"Live Images > Compression Quality should be '0.3'");
            Assert.IsTrue(IsElementEquals(PlacesPage.LiveImagesFrameIntervalMinutesDropDown, "1 m"),
                @"Live Images > Frame Interval (minutes) should be '1 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.LiveImagesFrameIntervalSecondsDropDown, "0 s"),
                @"Live Images > Frame Interval (seconds) should be '0 s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.LiveImagesScale, "0.5"),
                @"Live Images > Scale should be '0.5'");
            Assert.IsTrue(IsElementEquals(PlacesPage.LiveImagesStartOfDeletionDropDown, "after 14 days"),
                @"Live Images > Start Of Deletion should be 'after 14 days'");

            ClickUntilShown(PlacesPage.ScreenButton, PlacesPage.ScreenHeight);
            Assert.IsTrue(IsElementEquals(PlacesPage.ScreenWidth, "1080"),
                @"Screen > Screen Width should be '1080 px'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ScreenHeight, "1920"),
                @"Screen > Screen Height should be '1920 px'");

            ClickUntilShown(PlacesPage.OverlappingButton, PlacesPage.OverlappingForProjectorsCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.OverlappingForProjectorsCheckBox),
                @"Screen > Overlapping > Overlapping For Projectors should be 'Off'");
            Assert.IsTrue(IsElementEquals(PlacesPage.OverlappingHeight, "108"),
                @"Screen > Overlapping > Overlapping Height should be '108 px'");
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.OverlappingGridCheckBox),
                @"Screen > Overlapping > Overlapping Grid should be 'Off'");

            ClickUntilShown(PlacesPage.UpdateButton, PlacesPage.AgentUpdateDropDown);
            Assert.IsTrue(IsElementEquals(PlacesPage.AgentUpdateDropDown, "No Skeletons"),
                @"Update > Agent Update should be 'No Skeletons'");

            ClickUntilShown(PlacesPage.UploadGroupButton, PlacesPage.EnabledCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.EnabledCheckBox), @"Upload > Enabled should be 'On'");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.UploadFileUploadConfigurationRow1),
                "Upload > File Upload Configurations table should have no rows");

            Click(PageFooter.CancelButton);
        }

        [Test, Regression]
        public void RT04312_PropertiesValidationAndSavedValues()
        {
            TestStart();

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            DropDownSelect(PlacesPage.RebootIntervalDropDown, "Daily");

            ClickUntilShown(PlacesPage.AppSwitcherButton, PlacesPage.MaxValidPixels);
            SendText(PlacesPage.MaxValidPixels, "6");
            Assert.IsTrue(IsElementFound(PlacesPage.ErrorMaxValidPixelsLower),
                @"App Switcher > Error 'This must be 5% or lower' should be shown under " +
                "Max Valid Pixels");

            SendText(PlacesPage.MaxValidPixels, "0");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorMaxValidPixelsLower),
                @"App Switcher > Error 'This must be 5% or lower' should be not shown " + 
                "under Max Valid Pixels");

            ClickUntilShown(PlacesPage.DistributionButton, PlacesPage.DistributionInternetGateway);
            SendText(PlacesPage.DistributionLocalDistributionPort, "66000");
            Assert.IsTrue(IsElementFound(PlacesPage.ErrorLocalDistributionPortLower),
                @"Distribution > Error 'This must be 65,535 or lower' should be shown " + 
                "under Local Distribution Port");

            SendText(PlacesPage.DistributionLocalDistributionPort, "0");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorLocalDistributionPortLower),
                @"Distribution > Error 'This must be 65,535 or lower' should be not shown " + 
                "under Local Distribution Port");

            ClickUntilShown(PlacesPage.MonitoringButton, PlacesPage.AvailabilityButton);
            ClickUntilShown(PlacesPage.AvailabilityButton, PlacesPage.MonitoringCheckBox);
            Click(PlacesPage.MonitoringCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.MonitoringCheckBox),
                @"Monitoring > Availability > Monitoring should be 'On'");

            ClickUntilShown(PlacesPage.DepthImageButton, PlacesPage.AllowedBlackErrorMinutesDropDown);
            DropDownSelect(PlacesPage.AllowedBlackErrorMinutesDropDown, "10 m");
            Assert.IsTrue(IsElementEquals(PlacesPage.AllowedBlackErrorSecondsDropDown, "0 s"),
                @"Monitoring > Depth Image > Allow Black Error second should be '0 s' after minutes change");

            ClickUntilShown(PlacesPage.EventLogButton, PlacesPage.EventLogConfigurationsRow1);
            ClickUntilShown(PlacesPage.EventLogConfigurationsRow1, 
                PlacesPage.EventLogConfigurationsRow1DetailsAlertErrorCode);
            DropDownSelect(PlacesPage.EventLogConfigurationsRow1DetailsEventLogDropDown, "Application");
            ClickUntilShown(PlacesPage.EventLogConfigurationsRow2,
                PlacesPage.EventLogConfigurationsRow2DetailsAlertErrorCode);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.EventLogConfigurationsRow2DeleteButton),
                @"Monitoring > Event Log > row 2 should have Delete button (expanded row)");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.EventLogConfigurationsRow1DeleteButton),
                @"Monitoring > Event Log > row 1 should have no Delete button (collapsed row)");

            Click(PlacesPage.EventLogConfigurationsRow2DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.EventLogConfigurationsRow4),
                @"Monitoring > Event Log > there should be 3 rows (row 2 was deleted before)");
            Assert.IsTrue(!AreElementsContainText(PlacesPage.EventLogConfigurationsRow2, "Application.Error"),
                @"Monitoring > Event Log > there should be no row with text 'Application.Error'");

            ClickUntilShown(PlacesPage.EventLogConfigurationsAddButton, 
                PlacesPage.EventLogConfigurationsRow4);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorAlertErrorCodeOneChar),
                @"Monitoring > Event Log > added row 4 should have validation error under Alert Error Code");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorAlertErrorMessageOneChar),
                @"Monitoring > Event Log > added row 4 should have validation error under Alert Error Message");

            SendText(PlacesPage.EventLogConfigurationsRow4DetailsAlertErrorCode, "Test.test");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorAlertErrorCodeOneChar),
                @"Monitoring > Event Log > added row 4 should have no validation error under Alert Error Code");
            SendText(PlacesPage.EventLogConfigurationsRow4DetailsAlertErrorMessage, "Some message");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorAlertErrorMessageOneChar),
                @"Monitoring > Event Log > added row 4 should have no validation error under Alert Error Message");

            ClickUntilShown(PlacesPage.LiveImagesButton, PlacesPage.LiveImagesScale);
            SendText(PlacesPage.LiveImagesScale, "1.1");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorScaleLower),
                @"Monitoring > Live Images > Scale validation error 'This must be 1 or lower' should be shown");

            ClearTextInElement(PlacesPage.LiveImagesScale);
            DragAndDropByOffset(PlacesPage.LiveImagesScaleSlider, new Point(0, 0), new Point(200, 0));
            Assert.IsTrue(IsElementEquals(PlacesPage.LiveImagesScale, "1.00"),
                @"Monitoring > Live Images > Scale should show '1.00' after slider drag to the right");

            ClickUntilShown(PlacesPage.UpdateButton, PlacesPage.AgentUpdateDropDown);
            DropDownSelect(PlacesPage.AgentUpdateDropDown, "Off");
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place should be saved successfully");

            Click(PageFooter.DuplicateButton);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            SubmitForm();
            EditForm();
            ClickUntilShown(PlacesPage.AppSwitcherButton, PlacesPage.MaxValidPixels);
            Assert.IsTrue(IsElementEquals(PlacesPage.MaxValidPixels, "0"),
                @"App Switcher > Max Valid Pixels should be '0'");
            ClickUntilShown(PlacesPage.DistributionButton, PlacesPage.DistributionInternetGateway);
            Assert.IsTrue(IsElementEquals(PlacesPage.DistributionLocalDistributionPort, "0"),
                @"Distribution > Local Distribution Port should be '0'");
            ClickUntilShown(PlacesPage.MonitoringButton, PlacesPage.AvailabilityButton);
            ClickUntilShown(PlacesPage.AvailabilityButton, PlacesPage.MonitoringCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.MonitoringCheckBox),
                @"Monitoring > Availability > Monitoring should be 'On'");
            ClickUntilShown(PlacesPage.DepthImageButton, PlacesPage.AllowedBlackErrorMinutesDropDown);
            Assert.IsTrue(IsElementEquals(PlacesPage.AllowedBlackErrorMinutesDropDown, "10 m"),
                @"Monitoring > Depth Image > Allow Black Error minutes should be '10 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AllowedBlackErrorSecondsDropDown, "0 s"),
                @"Monitoring > Depth Image > Allow Black Error second should be '0 s'");
            ClickUntilShown(PlacesPage.EventLogButton, PlacesPage.EventLogConfigurationsRow1);
            Assert.IsTrue(!AreElementsContainText(PlacesPage.EventLogConfigurationsRow2, "Application.Error"),
                @"Monitoring > Event Log > there should be no row with text 'Application.Error'");
            Assert.IsTrue(AreElementsContainText(PlacesPage.EventLogConfigurationsRow4, "Test.test"),
                @"Monitoring > Event Log > there should be a row with text 'Test.test'");
            ClickUntilShown(PlacesPage.LiveImagesButton, PlacesPage.LiveImagesScale);
            Assert.IsTrue(IsElementEquals(PlacesPage.LiveImagesScale, "1.00"),
                @"Monitoring > Live Images > Scale should be '1.00'");
            ClickUntilShown(PlacesPage.UpdateButton, PlacesPage.AgentUpdateDropDown);
            Assert.IsTrue(IsElementEquals(PlacesPage.AgentUpdateDropDown, "Off"),
                "Update > Agent Update should be 'Off'");

            Click(PageFooter.CancelButton);
        }

        [Test, Regression]
        public void RT04313_AppsPropertiesChangeSynchronization()
        {
            AddAppPlayer(version: TestConfig.PlayerAppVersions[1]);
            Parallel.Invoke(
                () => AddAppComposerHq1(),
                () => AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion)
            );
            TestStart();

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(string.Format(AppsPage.TableRowByText, AppTitle.ComposerHq1));
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(AppsPage.TableRowComposerVipbApp);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.ConfigurationSectionLivePreviewCheckBox),
                @"Configuration > Live Preview should be 'Off'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionMasterVolume, "30"),
                @"Configuration > Master Volume should be '30 %'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionRecordingQualityDropDown, "Low"),
                @"Configuration > Recording Quality should be 'Low'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionComposerServiceUrlDropDown, "PRD"),
                @"Configuration > Composer Service URL should be 'PRD'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionComposerFileUrlDropDown, "PRD"),
                @"Configuration > Composer File URL should be 'PRD'");
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionGenderAgeTrackingDropDown, "Disabled"),
                @"Configuration > Gender/Age Tracking should be 'PRD'");
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.ConfigurationSectionAgeGenderBasedContentDropDown),
                @"Configuration > Age-/Gender-based Content should be 'On'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DiagnosticsButton),
                "Diagnostics group button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DownloadButton),
                "Download group button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.InteractionButton),
                "Interaction group button should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenButton),
                "Screen group button should be displayed");

            ClickUntilShown(PlacesPage.DiagnosticsButton, PlacesPage.DiagnosticsSectionDebugModeCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.DiagnosticsSectionDebugModeCheckBox),
                @"Diagnostics > Debug Mode should be 'Off'");
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.DiagnosticsSectionTopMostCheckBox),
                @"Diagnostics > Top Most should be 'On'");
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.DiagnosticsSectionShowSensorStreamsCheckBox),
                @"Diagnostics > Show Sensor Streams should be 'Off'");
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.DiagnosticsSectionShowBlackScreenCheckBox),
                @"Diagnostics > Show Black Screen should be 'Off'");
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.DiagnosticsSectionCaptureMovieCheckBox),
                @"Diagnostics > Capture Movie should be 'Off'");
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.DiagnosticsSectionShowFrameworkCursorCheckBox),
                @"Diagnostics > Show Framework Cursor should be 'Off'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.UserRecordingButton),
                "Diagnostics > User Recording group button should be displayed");

            Assert.IsTrue(CountElements(PlacesPage.ConfigurationSectionScreenButton) == 1,
                @"Configuration > only 1 'Screen' group button should be shown");

            ClickUntilShown(PlacesPage.ScreenButton, PlacesPage.ScreenWidth);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenWidth), 
                @"Screen > Screen Width should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenHeight),
                @"Screen > Screen Height should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenSectionLogoLocationDropDown),
                @"Screen > Logo Location should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenSectionLogoHorizontalOffset),
                @"Screen > Logo Horizontal Offset should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenSectionLogoVerticalOffset),
                @"Screen > Logo Vertical Offset should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenSectionLogoOpacity),
                @"Screen > Logo Opacity should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenSectionLogoScale),
                @"Screen > Logo Scale should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.OverlappingButton),
                "Screen > Overlapping group button should be displayed");

            ClickUntilShown(PlacesPage.InteractionButton, 
                PlacesPage.InteractionSectionIdleTimeoutMinutesDropDown);
            DropDownSelect(PlacesPage.InteractionSectionIdleTimeoutMinutesDropDown, "1 m");
            DropDownSelect(PlacesPage.InteractionSectionIdleTimeoutSecondsDropDown, "59 s");
            ClickUntilShown(PlacesPage.FocusAreaButton, PlacesPage.FocusAreaSectionFocusAreaDistance);
            SendText(PlacesPage.FocusAreaSectionFocusAreaDistance, "2");
            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsInteractionButton,
                PlacesPage.AppsSectionRow1DetailsFocusAreaButton);
            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsFocusAreaButton, 
                PlacesPage.AppsSectionRow1DetailsFocusAreaDistance);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow1DetailsIdleTimeoutMinutesDropDown, "1 m"),
                @"Apps section > Row 1 > Interaction > Idle Timeout (minutes) should be '1 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow1DetailsIdleTimeoutSecondsDropDown, "59 s"),
                @"Apps section > Row 1 > Interaction > Idle Timeout (seconds) should be '59 s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow1DetailsFocusAreaDistance, "2"),
                @"Apps section > Row 1 > Interaction > Focus Area > Focus Area Distance should be '2 m'");
            ClickUntilShown(PlacesPage.AppsSectionRow2DetailsInteractionButton,
                PlacesPage.AppsSectionRow2DetailsFocusAreaButton);
            ClickUntilShown(PlacesPage.AppsSectionRow2DetailsFocusAreaButton,
                PlacesPage.AppsSectionRow2DetailsFocusAreaDistance);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow2DetailsIdleTimeoutMinutesDropDown, "1 m"),
                @"Apps section > Row 2 > Interaction > Idle Timeout (minutes) should be '1 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow2DetailsIdleTimeoutSecondsDropDown, "59 s"),
                @"Apps section > Row 2 > Interaction > Idle Timeout (seconds) should be '59 s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow2DetailsFocusAreaDistance, "2"),
                @"Apps section > Row 2 > Interaction > Focus Area > Focus Area Distance should be '2 m'");

            SendText(PlacesPage.AppsSectionRow2DetailsFocusAreaDistance, "0.5");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow1DetailsFocusAreaDistance, "2"),
                @"Apps section > Row 1 > Interaction > Focus Area > Focus Area Distance should be '2 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.FocusAreaSectionFocusAreaDistance, string.Empty),
                "Configuration > Interaction > Focus Area > Focus Area Distance should be empty");

            SubmitForm();
            RefreshPage();
            EditForm();
            ClickUntilShown(PlacesPage.InteractionButton,
                PlacesPage.InteractionSectionIdleTimeoutMinutesDropDown);
            ClickUntilShown(PlacesPage.FocusAreaButton, PlacesPage.FocusAreaSectionFocusAreaDistance);
            Assert.IsTrue(IsElementEquals(PlacesPage.InteractionSectionIdleTimeoutMinutesDropDown, "1 m"),
                @"Configuration > Interaction > Idle Timeout (minutes) should be '1 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.InteractionSectionIdleTimeoutSecondsDropDown, "59 s"),
                @"Configuration > Interaction > Idle Timeout (seconds) should be '59 s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.FocusAreaSectionFocusAreaDistance, string.Empty),
                "Configuration > Interaction > Focus Area > Focus Area Distance should be empty");
            ClickUntilShown(PlacesPage.AppsSectionTableRow1,
                PlacesPage.AppsSectionRow1DetailsInteractionButton);
            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsInteractionButton,
                PlacesPage.AppsSectionRow1DetailsFocusAreaButton);
            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsFocusAreaButton,
                PlacesPage.AppsSectionRow1DetailsFocusAreaDistance);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow1DetailsIdleTimeoutMinutesDropDown, "1 m"),
                @"Apps section > Row 1 > Interaction > Idle Timeout (minutes) should be '1 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow1DetailsIdleTimeoutSecondsDropDown, "59 s"),
                @"Apps section > Row 1 > Interaction > Idle Timeout (seconds) should be '59 s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow1DetailsFocusAreaDistance, "2"),
                @"Apps section > Row 1 > Interaction > Focus Area > Focus Area Distance should be '2 m'");
            ClickUntilShown(PlacesPage.AppsSectionTableRow2,
                PlacesPage.AppsSectionRow2DetailsInteractionButton);
            ClickUntilShown(PlacesPage.AppsSectionRow2DetailsInteractionButton,
                PlacesPage.AppsSectionRow2DetailsFocusAreaButton);
            ClickUntilShown(PlacesPage.AppsSectionRow2DetailsFocusAreaButton,
                PlacesPage.AppsSectionRow2DetailsFocusAreaDistance);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow2DetailsIdleTimeoutMinutesDropDown, "1 m"),
                @"Apps section > Row 2 > Interaction > Idle Timeout (minutes) should be '1 m'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow2DetailsIdleTimeoutSecondsDropDown, "59 s"),
                @"Apps section > Row 2 > Interaction > Idle Timeout (seconds) should be '59 s'");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow2DetailsFocusAreaDistance, "0.5"),
                @"Apps section > Row 2 > Interaction > Focus Area > Focus Area Distance should be '0.5 m'");

            ClickUntilConditionMet(PlacesPage.AppsSectionTableRow2DeleteButton,
                () => IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow2));
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ConfigurationSectionLivePreviewCheckBox),
                "Configuration > Live Preview should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ConfigurationSectionMasterVolume),
                "Configuration > Master Volume should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ConfigurationSectionRecordingQualityDropDown),
                @"Configuration > Recording Quality should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ConfigurationSectionComposerServiceUrlDropDown),
                @"Configuration > Composer Service URL should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ConfigurationSectionComposerFileUrlDropDown),
                @"Configuration > Composer File URL should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ConfigurationSectionGenderAgeTrackingDropDown),
                @"Configuration > Gender/Age Tracking should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ConfigurationSectionAgeGenderBasedContentDropDown),
                @"Configuration > Age-/Gender-based Content should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DiagnosticsButton),
                "Diagnostics group button should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DownloadButton),
                "Download group button should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.InteractionButton),
                "Interaction group button should be hidden on VIPB app delete");

            ClickUntilShown(PlacesPage.ScreenButton, PlacesPage.ScreenWidth);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScreenSectionLogoLocationDropDown),
                @"Screen > Logo Location should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScreenSectionLogoHorizontalOffset),
                @"Screen > Logo Horizontal Offset should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScreenSectionLogoVerticalOffset),
                @"Screen > Logo Vertical Offset should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScreenSectionLogoOpacity),
                @"Screen > Logo Opacity should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScreenSectionLogoScale),
                @"Screen > Logo Scale should be hidden on VIPB app delete");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenWidth),
                @"Screen > Screen Width should be displayed on VIPB app delete");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenHeight),
                @"Screen > Screen Height should be displayed on VIPB app delete");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.OverlappingButton),
                "Screen > Overlapping group button should be displayed on VIPB app delete");

            Click(PlacesPage.AppsSectionTableRowDetailsLivePreviewCheckBox);
            SubmitForm();
            ClickUntilShown(PlacesPage.AppsSectionTableRow1, 
                PlacesPage.AppsSectionTableRowDetailsLivePreviewCheckBox);
            Assert.IsTrue(IsCheckBoxOn(PlacesPage.AppsSectionTableRowDetailsLivePreviewCheckBox),
                @"Apps section > Row 1 > Live Preview should be 'On'");
        }

        [Test, Regression]
        public void RT04314_AppsPropertiesChangeSynchronization2()
        {
            AddAppPlayer(version: TestConfig.PlayerAppVersions[1]);
            Parallel.Invoke(
                () => AddAppComposerHq1(),
                () => AddAppComposerVipB(TestConfig.ComposerVipbAppMiddleVersion),
                () => AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion),
                () => AddAppPlayer(version: TestConfig.PlayerAppVersions[0]),
                () => AddAppLegoBoost(TestConfig.LegoBoostAppVersions[0])
            );
            TestStart();

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(string.Format(AppsPage.TableRowByText, AppTitle.ComposerHq1));
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(string.Format(AppsPage.TableRowByText, AppTitle.LegoBoostUnity));
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionMasterVolume, "30"),
                @"Configuration > Master Volume should be '30 %'");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ConfigurationSectionRecordingQualityDropDown),
                "Configuration > Recording Quality should be not displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ConfigurationSectionAgeGenderBasedContentDropDown),
                "Configuration > Age-/Gender-Based Content should be not displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DiagnosticsButton),
                "Diagnostics group button should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScreenButton),
                "Screen group button should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.InteractionButton),
                "Interaction group button should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DownloadButton),
                "Download group button should be hidden");

            ClickUntilShown(PlacesPage.DiagnosticsButton, PlacesPage.DiagnosticsSectionCaptureMovieCheckBox);
            Assert.IsTrue(CountElements(PlacesPage.DiagnosticsSectionControls) == 1,
                "Diagnostics > section should contain only Capture Movie check box");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DiagnosticsSectionCaptureMovieCheckBox),
                "Diagnostics > Capture Movie should be displayed");

            SendText(PlacesPage.ConfigurationSectionMasterVolume, "50");
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(AppsPage.TableRowComposerVipbApp);
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionMasterVolume, string.Empty),
                "Configuration > Master Volume should be empty on VIPB app adding");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsMasterVolume, "50"),
                @"Apps section > row 1 > Master Volume should be '50 %' on VIPB app adding");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow2DetailsMasterVolume, "50"),
                @"Apps section > row 2 > Master Volume should be '50 %' on VIPB app adding");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow3DetailsMasterVolume, "30"),
                @"Apps section > row 3 > Master Volume should be '30 %' on VIPB app adding");

            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ConfigurationSectionLivePreviewCheckBox),
                "Configuration > Live Preview should appear on VIPB app adding");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DownloadButton),
                "Download group button should appear on VIPB app adding");

            ClickUntilShown(PlacesPage.DiagnosticsButton, PlacesPage.DiagnosticsSectionCaptureMovieCheckBox);
            Click(PlacesPage.DiagnosticsSectionTopMostCheckBox);
            ClickUntilConditionMet(PlacesPage.AppsSectionTableRow1DeleteButton, 
                () => IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow3));
            ClickUntilShown(PlacesPage.DiagnosticsButton, PlacesPage.DiagnosticsSectionCaptureMovieCheckBox);
            Assert.IsTrue(CountElements(PlacesPage.DiagnosticsSectionControls) == 1,
                "Diagnostics > section should contain only Capture Movie check box on HQ1 deletion");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DiagnosticsSectionCaptureMovieCheckBox),
                "Diagnostics > Capture Movie should be displayed on HQ1 deletion");

            ClickUntilShown(PlacesPage.AppsSectionRow2DetailsDiagnosticsButton,
                PlacesPage.AppsSectionRow2DetailsTopMostCheckBox);
            Assert.IsTrue(IsCheckBoxOff(PlacesPage.AppsSectionRow2DetailsTopMostCheckBox),
                @"Apps section > row 2 > Top Most should be 'On' on HQ1 deletion");

            SendText(PlacesPage.AppsSectionRow1DetailsMasterVolume, "70");
            SendText(PlacesPage.AppsSectionRow2DetailsMasterVolume, "70");
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionMasterVolume, "70"),
                @"Configuration > Master Volume should be '70 %' on the same setting in all apps");

            DropDownSelect(PlacesPage.AppsSectionTableRow2DetailsVersionDropDown, 
                TestConfig.PlayerAppVersions[0]);
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionMasterVolume, "70"),
                @"Configuration > Master Volume should be '70 %' on VIPB field Version modification");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow2DetailsMasterVolume, "70"),
                @"Apps section > row 2 > Master Volume should be '70 %' on VIPB field Version modification");

            DropDownSelect(PlacesPage.AppsSectionTableRow2DetailsAppPackageDropDown,
                $"{AppTitle.ComposerVipB} ({TestConfig.ComposerVipbAppEarliestVersion})");
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionMasterVolume, "70"),
                @"Configuration > Master Volume should be '70 %' on VIPB field App Package modification");
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionRow2DetailsMasterVolume, "70"),
                @"Apps section > row 2 > Master Volume should be '70 %' on VIPB field App Package modification");

            SubmitForm();
            EditForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.ConfigurationSectionMasterVolume, "70"),
                @"Configuration > Master Volume should be '70 %' on place submit");

            Click(PageFooter.CancelButton);
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
            }
        }
    }
}
