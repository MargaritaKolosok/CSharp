using System.Linq;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC04A_PlacesNoTypeTests : ParentTest
    {
        private string _title = $"Auto place {RandomNumber}";
        private string _timezone = "Kiev";
        private string _position = "52.316646, 13.174032";
        private string _toleranceRadius = "1000";
        private readonly string _placeWidth = "0";
        private string _titleChild;

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
        public void RT04010_PlaceWithoutSave()
        {
            TestStart();
            Click(PageFooter.AddPlaceButton);

            Assert.IsTrue(IsElementFound(PlacesPage.ErrorTitleIsRequired),
                @"Title error 'This field is required' is not displayed" );
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorTimezoneIsRequired),
                @"Timezone error 'This field is required' is not displayed");

            _title = $"Auto place {RandomNumber}";
            SendText(PlacesPage.Title, _title);
            ClickUntilShown(PlacesPage.TimezoneDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(IsAlphabeticallySorted(CommonElement.DropDownOptionList),
                "Timezone dropdown item list is not alphabetically ordered");

            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorTimezoneIsRequired),
                @"Timezone error 'This field is required' is not displayed");
            Assert.IsTrue(IsEditMode(),
                @"Place has been saved with empty Timezone field");

            ClickUntilShown(PlacesPage.TimezoneDropDown, string.Format(CommonElement.DropDownOption, TimezoneKiev));
            SendText(CommonElement.DropDownInput, _timezone);
            Assert.IsTrue(IsElementFound(string.Format(CommonElement.DropDownOption, TimezoneKiev)),
                "Europe/Kiev timezone dropdown list item is not displayed");

            Click(string.Format(CommonElement.DropDownOption, TimezoneKiev));
            Assert.IsTrue(AreElementsContainText(PlacesPage.TimezoneDropDown, _timezone),
                "Timezone dropdown is not set to Europe/Kiev list item");

            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.IbeaconDetailsButton),
                "Device dropdown is not disabled");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementNotFound(string.Format(PlacesPage.TableRowByTitle, _title)),
                "Place has been saved despite that has been canceled");
        }

        [Test, Regression]
        public void RT04020_AddPlaceSaved()
        {
            TestStart();
            Click(PageFooter.AddPlaceButton);

            _title = $"Auto place {RandomNumber}";
            SendText(PlacesPage.Title, _title);
            ClickUntilShown(PlacesPage.TimezoneDropDown, string.Format(CommonElement.DropDownOption, TimezoneKiev));
            PressKeys(Keys.ArrowDown + Keys.ArrowDown);
            _timezone = "Addis";
            Assert.IsTrue(AreElementsContainText(PlacesPage.TimezoneListItemActive, _timezone),
                @"Timezone dropdown list item 'Africa/Addis_Ababa' should be highlighted");
            PressKeys(Keys.Enter);
            Assert.IsTrue(AreElementsContainText(PlacesPage.TimezoneDropDown, _timezone),
                "Timezone dropdown is not set to Africa/Addis_Ababa list item");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place is not saved");
            Assert.IsTrue(IsElementEquals(PlacesPage.TitleReadOnly, _title),
                $@"Title is not set to '{_title}'");
            Assert.IsTrue(AreElementsContainText(PlacesPage.TimezoneReadOnly, _timezone),
                "Timezone is not set to Africa/Addis_Ababa after save");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $@"Status is not set to '{StatusNoDevice}' after save");
            Assert.IsTrue(IsElementEquals(PlacesPage.DeviceTypeReadOnly, string.Empty),
                @"Device Type is not empty after save");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                @"Device is not empty after save");
            Assert.IsTrue(IsElementEquals(PlacesPage.ToleranceRadiusReadOnly, "5 m"),
                @"Tolerance Radius is not set to '5 m' after save");
        }

        [Test, Regression]
        public void RT04030_SetPosition()
        {
            TestStart();
            AddPlaceNoType(PlaceStatus.NoDevice);

            Click(PlacesPage.GlobeButtonReadOnly);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.Map),
                "Map is displayed in view mode");
            EditForm();
            Assert.IsTrue(GetValue(PlacesPage.Position) != string.Empty,
                "Position does not contain current coordinates");
            SendText(PlacesPage.Position, "99.92432424,90.47475");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorPositionLatLongNotValid),
                @"Error 'Latitude and/or longitude values are not valid' is not displayed");
            SendText(PlacesPage.Position, _position);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorPositionLatLongNotValid),
                @"Error 'Latitude and/or longitude values are not valid' should not be displayed");
            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.PositionReadOnly, _position),
                @"Position field did not save correct value");
        }

        [Test, Regression]
        public void RT04040_MapActions()
        {
            TestStart();
            AddPlaceNoType(PlaceStatus.NoDevice);

            EditForm();
            ClearTextInElement(PlacesPage.Position);
            SubmitForm();
            Assert.IsTrue(GetValue(PlacesPage.PositionReadOnly) == string.Empty,
                "Position field has not been cleared");

            Click(PageFooter.EditButton);
            Click(PlacesPage.GlobeButton);
            Assert.IsTrue(IsElementFound(PlacesPage.Map), "Map is not displayed in edit mode");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.MapMarker), "Map position marker is displayed");

            ClickAtPoint(PlacesPage.Map, 10, 10, MoveToElementOffsetOrigin.Center);
            Assert.IsTrue(IsElementFound(PlacesPage.MapMarker), "Map position marker is not displayed");
            Click(PageFooter.SelectButton);
            _position = GetValue(PlacesPage.Position);
            Assert.IsTrue(_position != string.Empty, "Position does not contain selected coordinates");
            SubmitForm();
            Assert.IsTrue(GetValue(PlacesPage.PositionReadOnly) == _position, 
                "Position does not contain selected coordinates");
        }

        [Test, Regression]
        public void RT04050_MapChangesCoordinates()
        {
            TestStart();
            AddPlaceNoType(PlaceStatus.NoDevice);
            EditForm();

            Click(PlacesPage.GlobeButton);

            Click(PageFooter.ClearSelectionButton);
            Assert.IsTrue(IsEditMode(),
                "Place is not in edit mode after clear position selection");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.Map),
                "Map is still displayed on Clear Selection button press");
            Assert.IsTrue(GetValue(PlacesPage.Position) != string.Empty,
                "Position field is empty, but should have a placeholder value");

            _position = "52.521006, 13.326885";
            SendText(PlacesPage.Position, _position);
            Click(PlacesPage.GlobeButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.MapMarker),
                "Map marker is not displayed on coordinates inputted");

            SendText(PlacesPage.MapSearch, "7 Gutenbergstraße Heidelberg");
            PressKeys(Keys.Enter);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.MapMarker),
                "Map marker is still displayed on coordinates inputted");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.Map),
                "Map marker is still displayed on coordinates inputted");
            Assert.IsTrue(IsEditMode(),
                "Place is not in edit mode after position selection cancellation");
            Assert.IsFalse(IsElementEquals(PlacesPage.Position, _position),
                "Position value has changed despite Cancel button has been pressed after search");

            Click(PageFooter.CancelButton);
            Assert.IsFalse(IsElementEquals(PlacesPage.PositionReadOnly, _position),
                "Position value has changed despite all place changes were canceled");
        }

        [Test, Regression]
        public void RT04060_NumericFieldValidation()
        {
            TestStart();
            AddPlaceNoType(PlaceStatus.NoDevice);
            EditForm();

            SendText(PlacesPage.ToleranceRadius, "-50");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorToleranceRadiusMustBeZeroOrGreater),
                @"Error for Tolerance Radius 'This must be 0 or greater' should be displayed");

            Click(PlacesPage.ToleranceRadiusIncreaseButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorToleranceRadiusMustBeZeroOrGreater),
                @"Error for Tolerance Radius 'This must be 0 or greater' is still displayed");
            Assert.IsTrue(IsElementEquals(PlacesPage.ToleranceRadius, "0"),
                @"Tolerance Radius should be 0 on '+' button press");

            _toleranceRadius = "2000";
            SendText(PlacesPage.ToleranceRadius, _toleranceRadius);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorToleranceRadiusPlaceholder),
                $"Some error for Tolerance Radius is displayed on value {_toleranceRadius}");

            Click(PlacesPage.ToleranceRadiusIncreaseButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.ToleranceRadius, _toleranceRadius),
                $@"Tolerance Radius should be {_toleranceRadius} on '+' button press");

            Click(PlacesPage.ToleranceRadiusDecreaseButton);
            _toleranceRadius = "1999";
            Assert.IsTrue(IsElementEquals(PlacesPage.ToleranceRadius, _toleranceRadius),
                $@"Tolerance Radius should be {_toleranceRadius} on '-' button press");

            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChangesWillBeDiscardedDialog),
                @"Dialog 'Changes will be discarded' not displayed on back button press");

            Click(PlacesPage.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChangesWillBeDiscardedDialog),
                @"Dialog 'Changes will be discarded' is still displayed on Cancel button press");
            Assert.IsTrue(IsEditMode(), "Page is not in edit mode");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.ToleranceRadiusReadOnly, _toleranceRadius + " m"),
                $@"Tolerance radius should be '{_toleranceRadius} m' after save");
        }

        [Test, Regression]
        public void RT04070_AddChildPlace()
        {
            TestStart();
            AddPlaceNoType(PlaceStatus.NoDevice);

            _title = GetValue(PlacesPage.TitleReadOnly, waitForValue: true);
            // place timezone in view mode contains region and current time 
            _timezone = GetValue(PlacesPage.TimezoneReadOnly, waitForValue: true).Substring(0, 12);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CreateNewChildPlaceButton),
                @"Button Create New Child Place is not found on Add button mouse-over");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddExistingPlaceAsChildButton),
                @"Button Add Existing Place As Child is not found on Add button mouse-over");

            Click(PageFooter.CreateNewChildPlaceButton);
            var currTimezone = GetValue(PlacesPage.TimezoneDropDown, waitForValue: true);
            Assert.IsTrue(currTimezone?.Contains(_timezone),
                $@"Timezone dropdown does not contain '{_timezone}'. It equals '{currTimezone}'");

            _titleChild = $"Auto place {RandomNumber}";
            SendText(PlacesPage.Title, _titleChild);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Page is in edit mode");

            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.PlaceMapWithMap),
                "Place Map field in not empty");
            Assert.IsTrue(IsElementEquals(PlacesPage.PlaceWidthReadOnly, _placeWidth + " m"),
                $@"Place Width should be '{_placeWidth} m'");

            var breadCrumbs = $"Tenants {CurrentTenantCode} Places {_title}";
            Assert.IsTrue(
                CleanUpString(GetValuesAsString(PageHeader.BreadCrumbsAllPath)) == breadCrumbs,
                "Bread crumbs in header have wrong content");
            Assert.IsTrue(IsElementEquals(PlacesPage.PlaceName, _titleChild),
                $@"Place name in top panel not equal to child place title '{_titleChild}'");
        }

        [Test, Regression]
        public void RT04080_ChildPlacesSection()
        {
            TestStart();
            var place = AddPlaceNoType(PlaceStatus.NoDevice, isAddChild: true, pageToBeOpened: 2);
            _title = place.Title;
            _titleChild = GetValue(PlacesPage.TitleReadOnly);

            EditForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.CreateNewChildPlaceButton),
                @"Button Create New Child Place is found on Add button mouse-over for child place");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddExistingPlaceAsChildButton),
                @"Button Add Existing Place As Child is not found on Add button mouse-over for child place");

            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.TitleReadOnly, _title),
                "Not switched to parent place by <Back button press");

            Assert.IsTrue(IsElementFound(PlacesPage.ChildPlacesSectionTitle),
                "Child Places section is absent on parent place page");
            var tableHeader = CleanUpString(GetValuesAsString(PlacesPage.ChildPlacesSectionTableHeader));
            var tableHeaderOriginal = "#TitleDevice TypeDeviceItems";
            Assert.IsTrue(tableHeader == tableHeaderOriginal,
                "Child Places section table should have columns: #, Title, Device Type, Device, Items");

            var tableRow = CleanUpString(GetValuesAsString(PlacesPage.ChildPlacesSectionTableRow1));
            var tableRowOriginal = $"1{_titleChild}";
            Assert.IsTrue(tableRow == tableRowOriginal,
                $"Child Places section table row have content: # = 1, Title = {_titleChild}");

            var breadCrumbs = $"Tenants {CurrentTenantCode} Places";
            Assert.IsTrue(CleanUpString(
                    GetValuesAsString(PageHeader.BreadCrumbsAllPath)) == breadCrumbs,
                "Bread crumbs in header have wrong content");
            Assert.IsTrue(IsElementEquals(PlacesPage.PlaceName, _title),
                $@"Place name in top panel not equal to parent place title '{_title}'");
        }

        [Test, Regression]
        public void RT04090_OpenChildPlaceFromSection()
        {
            TestStart();
            var place = AddPlaceNoType(PlaceStatus.NoDevice, isAddChild: true);
            _title = place.Title;
            _titleChild = place.ChildPlaces.LastOrDefault()?.Title;
            if (IsElementEquals(PlacesPage.PlaceName, _titleChild))
            {
                Click(PageHeader.NavigateBackButton);
            }

            Click(PlacesPage.ChildPlacesSectionTableRow1);
            Assert.IsTrue(IsElementEquals(PlacesPage.TitleReadOnly, _titleChild),
                "Not switched to child place page by click on place in Child Places section");

            Click(string.Format(PageHeader.BreadCrumb, "Places"));
            Assert.IsTrue(IsElementFound(string.Format(PlacesPage.TableRowByTitle, _title)),
                @"Not switched to Places page by click on 'Places' in breadcrumbs");
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
                PlaceApi.DeletePlaces(PlaceType.NoType);
            }
        }
    }
}
