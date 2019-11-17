using System;
using System.Drawing;
using System.Globalization;
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
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Tests.Helpers;
using Tests.Resources;
using Place = Models.Places.Place;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC04D_PlacesOperationsTests : ParentTest
    {
        private const int PlaceWidth = 14;

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            CurrentTenant = TenantTitle.manylang;
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }
        }

        private void PreparePlace(string pathFile, bool isSelectPosition = false)
        {
            EditForm();
            if (!string.IsNullOrEmpty(pathFile))
            {
                Click(PlacesPage.PlaceMapUploadButton);
                FileManager.Upload(pathFile);
            }
            SendText(PlacesPage.PlaceWidth, PlaceWidth.ToString(CultureInfo.InvariantCulture));
            SubmitForm();
            EditForm();
            if (!isSelectPosition)
            {
                return;
            }
            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            MakeMapMarkersAccessible(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable1);
            Click(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable1);
            DragAndDrop(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap1, PlacesPage.ChildPlacesSectionMap);
            SubmitForm();
        }

        [Test, Regression]
        public void RT04320_AddExistingPlaceAsChild()
        {
            Place placeNoTypeChild = null, placeIbeacon = null, placeWw = null, placeNoType = null;
            Item item = null;
            AppResponse app2 = null;
            Parallel.Invoke(
                () => app2 = AddAppIbeacon(TestConfig.IbeaconAppVersions[0]),
                () => placeWw = AddPlaceWw(PlaceStatus.Active, isAssignDevice: true, isAddChild: false,
                    pageToBeOpened: 0, isCreateNewPlace: true),
                () => AddAppPlayer(),
                () => placeNoTypeChild = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0, 
                    isCreateNewPlace: true),
                () => placeIbeacon = AddPlaceIbeacon(PlaceStatus.Any, isAssignIbeacon: true,
                    isAddChild: false, pageToBeOpened: 0, isCreateNewPlace: true),
                () => item = AddItem(ItemType.PorscheCar)
            );
            var app1 = AddAppComposerHq2();
            Parallel.Invoke(
                () => placeNoType = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeWw = AssignAppToPlace(placeWw, app1, null, null, isAddSilently: true),
                () => placeIbeacon = AssignAppToPlace(placeIbeacon, app2, item, ItemTypePorscheCar,
                    isAddSilently: true)
            );
            TestStart();
            placeNoType = AssignChildrenToParentPlace(placeNoType, false, placeNoTypeChild);

            EditForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);
            Assert.IsTrue(IsElementFound(PlacesPage.SelectPlaceDialog),
                @"Dialog 'select a place from the list' is not shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.CancelButton),
                "Cancel button should be shown in dialog");

            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, placeWw.Title)),
                $@"Place '{placeWw.Title}' should be shown in select place dialog");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, placeIbeacon.Title)),
                $@"Place '{placeIbeacon.Title}' should be shown in select place dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(PlacesPage.TableRowByTitle, placeNoTypeChild.Title)),
                $@"Child place '{placeNoTypeChild.Title}' should not be shown in select place dialog");

            Click(string.Format(PlacesPage.TableRowByTitle, placeIbeacon.Title));
            Assert.IsTrue(IsElementNotFound(PlacesPage.SelectPlaceDialog),
                @"Dialog 'select a place from the list' is still shown");
            var rowValues = 
                $"{placeIbeacon.Title}{DeviceTypeIbeacon}{placeIbeacon.Device.Name}{item.LangJsonData.EnJson.Title}";
            Assert.IsTrue(CleanUpString(GetValuesAsString(PlacesPage.ChildPlacesSectionTableRow1)).Contains(rowValues)
                    || CleanUpString(GetValuesAsString(PlacesPage.ChildPlacesSectionTableRow2)).Contains(rowValues),
                $@"Child places section - iBeacon place row should contain: '{rowValues}'");
            Assert.IsTrue(
                IsAlphabeticallySorted(PlacesPage.ChildPlacesSectionTableRowsColumnTitle, isElementDropDown: false),
                "Child places are not ordered by Title");

            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, placeWw.Title)),
                $@"Place '{placeWw.Title}' should be shown in select place dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(PlacesPage.TableRowByTitle, placeIbeacon.Title)),
                $@"Place '{placeIbeacon.Title}' should not be shown in select place dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(PlacesPage.TableRowByTitle, placeNoTypeChild.Title)),
                $@"Child place '{placeNoTypeChild.Title}' should not be shown in select place dialog");

            Click(string.Format(PlacesPage.TableRowByTitle, placeWw.Title));
            Assert.IsTrue(IsElementNotFound(PlacesPage.SelectPlaceDialog),
                @"Dialog 'select a place from the list' is still shown");
            rowValues = $"{placeWw.Title}{DeviceTypeWw}{placeWw.Device.Name}";
            Assert.IsTrue(CleanUpString(GetValuesAsString(PlacesPage.ChildPlacesSectionTableRow2)).Contains(rowValues)
                    || CleanUpString(GetValuesAsString(PlacesPage.ChildPlacesSectionTableRow3)).Contains(rowValues)
                    || CleanUpString(GetValuesAsString(PlacesPage.ChildPlacesSectionTableRow1)).Contains(rowValues),
                $@"Child places section - WW place row should contain: '{rowValues}'");
            Assert.IsTrue(
                IsAlphabeticallySorted(PlacesPage.ChildPlacesSectionTableRowsColumnTitle, isElementDropDown: false),
                "Child places are not ordered by Title");

            Click(PlacesPage.ChildPlacesSectionTableRow1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChangesWillBeDiscardedDialog),
                @"Dialog 'your changes will be discarded' should be displayed");

            Click(PlacesPage.CancelButton);
            SubmitForm();
            Assert.IsTrue(CountElements(PlacesPage.ChildPlacesSectionTableRowsColumnTitle) == 4,
                "Three child places should be saved");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionTableRowsDeleteButton),
                "There should be no delete row buttons in Child Places section in view mode");
            Assert.IsTrue(
                IsAlphabeticallySorted(PlacesPage.ChildPlacesSectionTableRowsColumnTitle, isElementDropDown: false),
                "Child places are not ordered by Title");
        }

        [Test, Regression]
        public void RT04330_DetachPlaces()
        {
            Place placeNoTypeChild = null, placeIbeacon = null, placeWw = null, placeNoType = null;
            Item item = null;
            AppResponse app1 = null, app2 = null;
            Parallel.Invoke(
                () => app1 = AddAppIbeacon(TestConfig.IbeaconAppVersions[0]),
                () => placeIbeacon = AddPlaceIbeacon(PlaceStatus.Any, isAssignIbeacon: true,
                    isAddChild: false, pageToBeOpened: 0, isCreateNewPlace: true),
                () => item = AddItem(ItemType.PorscheCar),
                () => AddAppPlayer()
                
            );
            Parallel.Invoke(
                () => placeNoType = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                        isCreateNewPlace: true),
                () => placeWw = AddPlaceWw(PlaceStatus.Active, isAssignDevice: true, isAddChild: false,
                    pageToBeOpened: 0, isCreateNewPlace: true),
                () => placeNoTypeChild = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeIbeacon = AssignAppToPlace(placeIbeacon, app1, item, ItemTypePorscheCar,
                    isAddSilently: true),
                () => app2 = AddAppComposerHq2()
            );
            placeWw = AssignAppToPlace(placeWw, app2, null, null, isAddSilently: true);
            TestStart();
            placeNoType = AssignChildrenToParentPlace(placeNoType, false, placeNoTypeChild, placeIbeacon, placeWw);

            EditForm();
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneLondon);
            Click(string.Format(PlacesPage.ChildPlacesSectionTableRowDeleteByTitleButton, placeIbeacon.Title));
            Assert.IsTrue(
                AreElementsContainText(PlacesPage.ChildPlacesSectionTableRowsColumnTitle, placeNoTypeChild.Title),
                $"Child places section should contain place {placeNoTypeChild.Title}");

            SubmitForm();
            Click(string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeNoTypeChild.Title));
            Assert.IsTrue(!IsElementEquals(PlacesPage.TimezoneReadOnly, TimezoneLondon),
                "Child places should not change their timezones when parent place changes it");

            EditForm();
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneLondon);
            SubmitForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);
            Assert.IsTrue(IsElementFound(string.Format(PlacesPage.TableRowByTitle, placeIbeacon.Title)),
                $@"Place '{placeIbeacon.Title}' should be shown in select place dialog");

            Click(string.Format(PlacesPage.TableRowByTitle, placeIbeacon.Title));
            Assert.IsTrue(AreElementsContainText(PlacesPage.ChildPlacesSectionTableRowsColumnTitle, placeIbeacon.Title),
                $"Child places section should contain iBeacon place {placeIbeacon.Title}");

            SubmitForm();
            Click(string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeIbeacon.Title));
            Assert.IsTrue(!IsElementEquals(PlacesPage.TimezoneReadOnly, TimezoneLondon),
                "Child places should not change their timezones when parent place changes it");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PageHeader.BreadCrumb, placeNoTypeChild.Title)),
                $"Bread crumbs should contain child place without type title {placeNoTypeChild.Title}");

            Click(PageHeader.NavigateBackButton);
            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.TitleReadOnly, placeNoType.Title),
                $"User should be on parent place no type page - title {placeNoType.Title}");
            Assert.IsTrue(IsElementFoundQuickly(
                string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeWw.Title)),
                $@"Place '{placeWw.Title}' should be shown in select place dialog");
            Assert.IsTrue(IsElementFoundQuickly(
                string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeNoTypeChild.Title)),
                $@"Child place '{placeNoTypeChild.Title}' should be shown in select place dialog");
            Assert.IsTrue(IsElementNotFoundQuickly(
                string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeIbeacon.Title)),
                $@"Child place '{placeIbeacon.Title}' should not be shown in select place dialog");
        }

        [Test, Regression]
        public void RT04340_DeleteRestorePlaces()
        {
            Place placeIbeacon = null, placeWw = null, placeNoType = null;
            Parallel.Invoke(
                () => placeNoType = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeIbeacon = AddPlaceIbeacon(PlaceStatus.Any, isAssignIbeacon: true,
                    isAddChild: false, pageToBeOpened: 0, isCreateNewPlace: true),
                () => placeWw = AddPlaceWw(PlaceStatus.Active, isAssignDevice: true, isAddChild: false,
                    pageToBeOpened: 0, isCreateNewPlace: true)
            );
            TestStart();
            placeNoType = AssignChildrenToParentPlace(placeNoType, false, placeIbeacon, placeWw);

            Click(PageFooter.DeleteButton);
            Assert.IsTrue(IsElementFound(PlacesPage.DeleteDialog), 
                @"'This place has X children' dialog should be displayed");
            Assert.IsTrue(IsElementFound(PlacesPage.DeleteChildPlacesButton),
                @"'Delete child places' button should be shown" );
            Assert.IsTrue(IsElementFound(PlacesPage.KeepChildPlacesButton),
                @"'Keep child places' button should be shown");
            Assert.IsTrue(IsElementFound(PlacesPage.CancelButton), @"'Cancel' button should be shown");

            Click(PlacesPage.DeleteChildPlacesButton);
            Assert.IsTrue(IsElementFound(PlacesPage.TableHeader), "Places page should be shown");
            OpenEntityPage(placeIbeacon);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusDeleted),
                $@"Child iBeacon place '{placeIbeacon.Title}' should have Status '{StatusDeleted}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "iBeacon field should be empty");

            OpenEntityPage(placeNoType);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusDeleted),
                $@"Child iBeacon place '{placeIbeacon.Title}' should have Status '{StatusDeleted}'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionTableHeader),
                "Child places section should be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionTableRow1),
                "Child places section displayed, but its table is not empty");

            Click(PlacesPage.ChildPlacesSectionShowDeletedButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionHideDeletedButton),
                "Hide Deleted button should be shown in Child Places section");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionShowDeletedButton),
                "Child places section displayed, but its table is not empty");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionTableRow1),
                "Child place row 1 should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionTableRow2),
                "Child place row 2 should be displayed");

            Click(PageFooter.RestoreButton);
            SubmitForm();
            Click(PlacesPage.ChildPlacesSectionHideDeletedButton);
            Assert.IsTrue(IsElementFound(PlacesPage.ChildPlacesSectionTableRow1),
                "Child place row 1 should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionTableRow2),
                "Child place row 2 should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionShowDeletedButton),
                "Show Deleted button should be shown in Child Places section");

            Click(PlacesPage.ChildPlacesSectionTableRow1);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $@"Child place should have Status '{StatusNoDevice}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "iBeacon field should be empty");
        }

        [Test, Regression]
        public void RT04350_DeleteRestorePlacesKeepChildren()
        {
            Place placeChild1 = null, placeChild12 = null, placeChild2 = null, placeNoType = null;
            Parallel.Invoke(
                () => placeNoType = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeChild1 = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeChild12 = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeChild2 = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true)
            );
            placeChild1 = AssignChildrenToParentPlace(placeChild1, true, placeChild12);
            TestStart();
            placeNoType = AssignChildrenToParentPlace(placeNoType, false, placeChild1, placeChild2);

            Click(PageFooter.DeleteButton);
            Click(PlacesPage.KeepChildPlacesButton);
            Assert.IsTrue(IsElementFound(PlacesPage.TableHeader), "Places page should be shown");

            OpenEntityPage(placeNoType);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusDeleted),
                $@"Parent place should have Status '{StatusDeleted}'");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionTableHeader),
                "Child places section should not be displayed");

            OpenEntityPage(placeChild1);
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice),
                $@"Child place should have Status '{StatusNoDevice}'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionTableRow1),
                "Child place row 1 should be displayed");

            Assert.IsFalse(AreElementsContainText(PageHeader.BreadCrumbsAllPath, placeNoType.Title),
                $@"Bread crumbs should not contain parent place '{placeNoType.Title}'");

            Click(PageFooter.DeleteButton);
            Assert.IsTrue(IsElementFound(PlacesPage.DeleteDialog),
                @"'This place has X children' dialog should be displayed");
            Assert.IsTrue(IsElementFound(PlacesPage.DeleteChildPlacesButton),
                @"'Delete child places' button should be shown");
            Assert.IsTrue(IsElementFound(PlacesPage.KeepChildPlacesButton),
                @"'Keep child places' button should be shown");
            Assert.IsTrue(IsElementFound(PlacesPage.CancelButton), @"'Cancel' button should be shown");

            Click(PlacesPage.CancelButton);
            Assert.IsTrue(IsElementNotFound(PlacesPage.DeleteDialog),
                @"'This place has X children' dialog should be closed");
            Assert.IsFalse(IsEditMode(), "Place should be in view mode");

            OpenEntityPage(placeNoType);
            Click(PageFooter.RestoreButton);
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(PlacesPage.ChildPlacesSectionTableHeader),
                "Child places section should not be displayed");
        }

        [Test, Regression]
        public void RT04360_ChildPlaceDeleteDetach()
        {
            Place placeChild1 = null, placeChild2 = null, placeChild3 = null, placeNoType = null;
            Parallel.Invoke(
                () => placeNoType = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeChild1 = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeChild2 = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeChild3 = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                isCreateNewPlace: true)
            );
            TestStart();
            placeNoType = AssignChildrenToParentPlace(placeNoType, false, placeChild1, placeChild2, placeChild3);

            Click(string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeChild2.Title));          
            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteButton);
            Assert.IsTrue(IsElementFound(PlacesPage.TableHeader), "Places page should be opened");
            
            Click(string.Format(PlacesPage.TableRowByTitle, placeNoType.Title));
            Assert.IsTrue(IsElementNotFound(
                    string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeChild2.Title)),
                $@"Child place 2 '{placeChild2.Title}' should not be shown in Child places");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeChild1.Title)),
                $@"Child place 1 '{placeChild1.Title}' should be shown in Child places");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeChild3.Title)),
                $@"Child place 3 '{placeChild3.Title}' should be shown in Child places");

            Click(PlacesPage.ChildPlacesSectionShowDeletedButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.ChildPlacesSectionTableRow3TitleCell, placeChild2.Title),
                $@"Child place 2 '{placeChild2.Title}' should be shown in Child places in row 3");

            EditForm();
            Click(PlacesPage.ChildPlacesSectionTableRow3DeleteButton);
            Assert.IsTrue(IsElementNotFound(
                    string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeChild2.Title)),
                $@"Child place 2 '{placeChild2.Title}' should not be shown in Child places");

            SubmitForm();
            Assert.IsTrue(IsElementNotFound(
                    string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeChild2.Title)),
                $@"Child place 2 '{placeChild2.Title}' should not be shown in Child places");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeChild1.Title)),
                $@"Child place 1 '{placeChild1.Title}' should be shown in Child places");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeChild3.Title)),
                $@"Child place 3 '{placeChild3.Title}' should be shown in Child places");


            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);
            Assert.IsTrue(IsElementNotFound(string.Format(PlacesPage.TableRowByTitle, placeChild2.Title)),
                $@"Child place 2 '{placeChild2.Title}' should not be shown in 'select places' modal dialog");

            Click(PlacesPage.CancelButton);
            OpenEntityPage(placeChild2);
            Assert.IsTrue(IsElementNotFound(string.Format(PageHeader.BreadCrumb, placeNoType.Title)),
                $"Bread crumbs should not contain ex-parent place title {placeNoType.Title}");

            Click(PageFooter.RestoreButton);
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(string.Format(PageHeader.BreadCrumb, placeNoType.Title)),
                $"Bread crumbs should not contain ex-parent place title {placeNoType.Title}");
        }

        [Test, Regression]
        public void RT04361_ChangePlaceType()
        {
            Place placeIbeacon = null, placeWw = null, placeNoType = null;
            AppResponse app1 = null, app2 = null;
            Item item = null;
            Parallel.Invoke(
                () => app1 = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]),
                () => AddAppPlayer(),
                () => item = AddItem(ItemType.PorscheCar),
                () => placeIbeacon = AddPlaceIbeacon(PlaceStatus.Any, isAssignIbeacon: true, isAddChild: false,
                    pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => placeWw = AddPlaceWw(PlaceStatus.Active, isAssignDevice: true, isAddChild: false,
                    pageToBeOpened: 0,
                    isCreateNewPlace: true)
            );
            AssignPlaceToItem(item, placeIbeacon, isAddSilently: true);
            Parallel.Invoke(
                () => app2 = AddAppComposerHq1(),
                () => AssignPlaceToItem(item, placeIbeacon, isAddSilently: true),
                () => placeIbeacon = AssignAppToPlace(placeIbeacon, app1, null, null, isAddSilently: true)
            );
            TestStart();
            Parallel.Invoke(
                () => AssignAppToPlace(placeWw, app2, null, null, isAddSilently: true),
                () => placeNoType = AddPlaceNoType(PlaceStatus.Any, isAddChild: true, isCreateNewPlace: true)
            );
            PreparePlace(TestConfig.Image138, isSelectPosition: true);
            
            OpenEntityPage(placeWw);
            EditForm();
            var title = GetValue(PlacesPage.Title);
            var timezone = GetValue(PlacesPage.TimezoneDropDown, waitForValue: true);
            var device = GetValue(PlacesPage.WindowsWorkstation);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            Assert.IsTrue(IsElementEquals(PlacesPage.Title, title), "Title should not change on Device Type new selection");
            Assert.IsTrue(IsElementEquals(PlacesPage.TimezoneDropDown, timezone), 
                "Timezone should not change on Device Type new selection");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstationFieldName, DeviceTypeIbeacon), 
                $@"Device field label should change to '{DeviceTypeIbeacon}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty), "iBeacon field should be empty");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScheduleSectionRow1), "Schedule section should be absent");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow1), "Apps section should be absent");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.FaceApiKey), "Configuration section should be absent");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.DeviceTypeReadOnly, DeviceTypeWw), 
                $@"Device Type dropdown should display '{DeviceTypeWw}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, device),
                $@"iBeacon field should contain device '{device}'");

            EditForm();
            title = GetValue(PlacesPage.Title);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.TitleReadOnly, title), 
                "Title should not change on Device Type new selection");
            Assert.IsTrue(AreElementsContainText(PlacesPage.TimezoneReadOnly, TimezoneKiev), 
                "Timezone should not change on Device Type new selection");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstationFieldName, DeviceTypeIbeacon),
                $@"Device field label should change to '{DeviceTypeIbeacon}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty), "iBeacon field should be empty");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScheduleSectionRow1), "Schedule section should be absent");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow1), "Apps section should be absent");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.FaceApiKeyReadOnly), "Configuration section should be absent");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice), 
                $@"Place Status should be '{StatusNoDevice}'");

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            Click(PlacesPage.WindowsWorkstationDetailsButton);
            SetFilterModal(device);
            Assert.IsTrue(IsElementEquals(string.Format(Devices.TableRowPlaceByTitle, device), string.Empty),
                $@"WW device '{device}' should have no place assigned");
            Click(Devices.CancelButton);
            Click(PageFooter.CancelButton);

            OpenEntityPage(placeIbeacon);
            EditForm();
            title = GetValue(PlacesPage.Title);
            timezone = GetValue(PlacesPage.TimezoneDropDown, waitForValue: true).Substring(0, 12);
            device = GetValue(PlacesPage.Ibeacon);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, string.Empty);
            Assert.IsTrue(IsElementEquals(PlacesPage.Title, title), "Title should not change on Device Type new selection");
            Assert.IsTrue(AreElementsContainText(PlacesPage.TimezoneDropDown, timezone), 
                "Timezone should not change on Device Type new selection");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstationFieldName, DeviceTypeNoType),
                $@"Device field label should change to '{DeviceTypeNoType}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty), 
                "Device dropdown should be empty and read-only");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScheduleSectionRow1), "Schedule section should be absent");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow1), "Apps section should be absent");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.FaceApiKey), "Configuration section should be absent");

            Click(PageFooter.CancelButton);
            EditForm();
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Assert.IsTrue(IsElementNotFound(string.Format(PlacesPage.TableSelectAppTitle, DeviceTypeIbeacon)),
                "Select Apps modal dialog should not contain iBeacon apps");

            Click(string.Format(PlacesPage.TableSelectAppTitle, AppTitle.ComposerHq1));
            SubmitForm();
            ScrollTop();
            Assert.IsTrue(IsElementEquals(PlacesPage.TitleReadOnly, title), 
                "Title should not change on Device Type new selection");
            Assert.IsTrue(AreElementsContainText(PlacesPage.TimezoneReadOnly, timezone), 
                "Timezone should not change on Device Type new selection");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstationFieldName, DeviceTypeWw),
                $@"Device field label should change to '{DeviceTypeWw}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstation, string.Empty), 
                "Devices dialog should be empty");
            ScrollBottom();
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ScheduleSectionRow1), "Schedule section should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionTableRow1), "Apps section should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.FaceApiKeyReadOnly), "Configuration section should be displayed");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusNoDevice), $@"Status should be '{StatusNoDevice}'");

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            Click(PlacesPage.IbeaconDetailsButton);
            Assert.IsTrue(IsElementEquals(string.Format(Devices.TableRowPlaceByTitle, device), string.Empty),
                $@"iBeacon device '{device}' should have no place assigned");
            Click(Devices.CancelButton);
            Click(PageFooter.CancelButton);

            OpenEntityPage(placeNoType);
            EditForm();
            title = GetValue(PlacesPage.Title);
            timezone = GetValue(PlacesPage.TimezoneDropDown, waitForValue: true).Substring(0, 12);
            var position = GetValue(PlacesPage.Position);
            var childTitle = GetValue(PlacesPage.ChildPlacesSectionTableRow1TitleCell);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            Assert.IsTrue(IsElementEquals(PlacesPage.Title, title), 
                "Title should not change on Device Type new selection");
            Assert.IsTrue(AreElementsContainText(PlacesPage.TimezoneDropDown, timezone), 
                $@"Timezone '{timezone}' should not change on Device Type new selection");
            Assert.IsTrue(IsElementEquals(PlacesPage.WindowsWorkstationFieldName, DeviceTypeIbeacon),
                $@"Device field label should change to '{DeviceTypeIbeacon}'");
            Assert.IsTrue(IsElementEquals(PlacesPage.Ibeacon, string.Empty),
                "Devices dialog should be empty");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PlaceMapImageInline), 
                "There should be a pre-loaded image in Place Map");
            Assert.IsTrue(IsElementEquals(PlacesPage.Position, position), 
                "Position should not change on Device Type new selection");
            Assert.IsTrue(IsElementEquals(PlacesPage.ChildPlacesSectionTableRow1TitleCell, childTitle),
                "Child Places section should not change on Device Type new selection");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ScheduleSectionRow1), 
                "Schedule section should not be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.AppsSectionTableRow1), 
                "Apps section should not be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.FaceApiKey), 
                "Configuration section should not be displayed");

            OpenDevicesFromPlace();
            Click(string.Format(Devices.TableRowByText, device));
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Changes are not saved");
            Assert.IsTrue(IsElementEquals(PlacesPage.Status, StatusActive), 
                $@"Status should be '{StatusActive}'");
        }

        [Test, Regression]
        public void RT04370_PlaceMapUploadRatio138()
        {
            TestStart();
            AddPlaceNoType(PlaceStatus.Any, isAddChild: true, isCreateNewPlace: true);
            
            Assert.IsTrue(IsElementFound(PlacesPage.PlaceMapReadOnly), "Place Map field not found");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.PlaceMapUploadButton),
                "Place map field upload image button should be absent");
            Click(PlacesPage.PlaceMapReadOnly);
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should not be displayed in view mode on Place map field click");

            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionMapWithItemsButton),
                @"'Map with items' button should not be displayed in Child places section");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionMapWithPlacesButton),
                @"'Map with places' button should not be displayed in Child places section");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionShowDeletedButton),
                @"'Show deleted places' button should be displayed in Child places section");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionTableViewButton),
                @"'Table view' button should be displayed in Child places section");

            EditForm();
            Click(PlacesPage.PlaceMapImageEmpty);
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should be opened");
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.TableRowSelected),
                "There should not be selected medias in dialog");

            Click(PlacesPage.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should not be closed");
            Assert.IsTrue(IsEditMode(), "Place is in view mode");

            Click(PlacesPage.PlaceMapUploadButton);
            FileManager.Upload(TestConfig.Image138);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PlaceMapImageInline), 
                "Place map field image is not shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorPlaceWidthIsNotValid),
                @"Error 'Place width is not valid' should be displayed");
            
            SendText(PlacesPage.PlaceWidth, PlaceWidth.ToString(CultureInfo.InvariantCulture));
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorPlaceWidthIsNotValid),
                @"Error 'Place width is not valid' should not be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionMapWithItemsButton),
                @"'Map with items' button should be displayed in Child places section");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionMapWithPlacesButton),
                @"'Map with places' button should be displayed in Child places section");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionShowDeletedButton),
                @"'Show deleted places' button should be displayed in Child places section");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionTableViewButton),
                @"'Table view' button should be displayed in Child places section");

            SubmitForm();
            Assert.IsFalse(IsEditMode(), "Place is in edit mode");
            Click(PlacesPage.PlaceMapImageInline);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.PlaceMapUploadButton),
                "Place map field upload image button should be absent");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PlaceMapImagePreview),
                "Place map preview is not opened");    
        }

        [Test, Regression]
        public void RT04380_PlaceMapUploadRatio250()
        {
            TestStart();
            AddPlaceNoType(PlaceStatus.Any, isAddChild: true, isCreateNewPlace: true);
            PreparePlace(TestConfig.Image138);

            Click(PlacesPage.PlaceMapImageInline);
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should be displayed in view mode on Place map field click");
            var fileName = Path.GetFileName(TestConfig.Image138);
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(MediaLibrary.TableRowSelected, fileName)),
                $"There is no selected {fileName} in media library or another image is selected");

            Click(PlacesPage.UploadButton);
            FileManager.Upload(TestConfig.Image25);
            fileName = Path.GetFileName(TestConfig.Image25);
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(MediaLibrary.TableRowSelected, fileName)),
                $"There is no selected {fileName} in media library or another image is selected");

            Click(PlacesPage.SubmitButton);
            Assert.IsTrue(IsEditMode(), "Place is in view mode");
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should be closed on image submit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PlaceMapImageInline), "Place map field image is not shown");

            Click(PlacesPage.PlaceMapImageInline);
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should be displayed on Place map field click");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(MediaLibrary.TableRowSelected, fileName)),
                $"There is no selected {fileName} in media library or another image is selected");

            Click(PlacesPage.ClearSelectionButton);
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should stay opened");
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.TableRowSelected),
                "Media library should not contain selected image");

            Click(PlacesPage.SubmitButton);
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should be closed on submit");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PlaceMapImageEmpty),
                "No image should be uploaded to Place map field");
            Assert.IsTrue(IsEditMode(), "Place is in view mode");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionMapWithItemsButton),
                @"'Map with items' button should not be displayed in Child places section");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionMapWithPlacesButton),
                @"'Map with places' button should not be displayed in Child places section");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionShowDeletedButton),
                @"'Show deleted places' button should be displayed in Child places section");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionTableViewButton),
                @"'Table view' button should be displayed in Child places section");

            Click(PlacesPage.PlaceMapUploadButton);
            FileManager.Upload(TestConfig.Image25);
            SubmitForm();
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PlaceMapImageInline),
                "Image 2.5 is not uploaded into Place map field");
        }

        [Test, Regression]
        public void RT04390_PlaceMapMarkerPosition()
        {
            Place placeNoType = null, placeChild = null;
            Item item = null;
            AppResponse app = null;
            Parallel.Invoke(
                () => placeNoType = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => item = AddItem(ItemType.PorscheCar),
                () => app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]),
                () => placeChild = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: true, pageToBeOpened: 0,
                    isCreateNewPlace: true)
            );
            placeChild = AssignAppToPlace(placeChild, app, item, ItemTypePorscheCar, isAddSilently: true);
            TestStart();
            placeNoType = AssignChildrenToParentPlace(placeNoType, false, placeChild);
            PreparePlace(TestConfig.Image138);

            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionTableHeader),
                "Child Places section table should be absent");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionMap),
                "Child Places section should display place map");

            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild.Title)),
                "Child places side bar should have map with non-draggable marker");
            MakeMapMarkersAccessible(
                string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild.Title));
            Click(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild.Title));
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceTitle, placeChild.Title)),
                $@"Child places side bar should have child place '{placeChild.Title}'");
            Assert.IsTrue(IsElementFoundQuickly(
                string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, StatusActive)),
                $@"Child places side bar should have child place in status '{StatusActive}'");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, placeChild.Device.Name)),
                $@"Child places side bar should have child place with device '{placeChild.Device.Name}'");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, DeviceTypeIbeacon)),
                $@"Child places side bar should have child place with device type '{DeviceTypeIbeacon}'");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, placeChild.Device.Name)),
                $@"Child places side bar should have child place with device '{placeChild.Device.Name}'");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, placeNoType.ChildPlaces.Last().Item)),
                $@"Child places side bar should have item '{placeNoType.ChildPlaces.Last().Item}'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionSidebarChildPlaceViewButton),
                "Child places side bar should have child place View button");

            var location = GetElementLocation(
                string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, placeChild.Title));
            DragAndDrop(string.Format(
                PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, placeChild.Title), PlacesPage.ChildPlacesSectionMap);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionSidebarChildPlaceCloseButton),
                "Child places side bar should have X button for place");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionSidebarChildPlaceViewButton),
                "Child places side bar should have child place View button");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionSidebarChildPlaceDetachButton),
                "Child places side bar should have child place Detach button");

            Click(PlacesPage.ChildPlacesSectionSidebarChildPlaceCloseButton);
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceTitle, placeChild.Title)),
                "Child places side bar should be closed on X button press");

            SubmitForm();
            RefreshPage();
            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            var newLocation = GetElementLocation(
                string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild.Title));
            Assert.IsTrue(newLocation != location, "Place map marker new location is not saved");
            
            EditForm();
            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            Click(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild.Title));
            Click(PlacesPage.ChildPlacesSectionSidebarChildPlaceDetachButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionTableHeader),
                "Child Places section should be removed");

            Click(PageFooter.CancelButton);
            EditForm();
            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            Click(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild.Title));
            Click(PlacesPage.ChildPlacesSectionSidebarChildPlaceViewButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.TitleReadOnly, placeChild.Title),
                $@"Child place '{placeChild.Title}' web page should be opened");
        }

        [Test, Regression]
        public void RT04400_ChildPlacesMapMarkerPositions()
        {
            Place placeNoType = null, placeChild1 = null, placeChild2 = null;
            Item item1 = null, item2 = null;
            AppResponse app = null;
            item1 = AddItem(ItemType.PorscheCar, isAddNew: true);
            Parallel.Invoke(
                () => placeNoType = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]),
                () => item1 = AddItem(ItemType.PorscheCar, isAddNew: true),
                () => item2 = AddItem(ItemType.Poi, isAssignImage: true),
                () => placeChild2 = AddPlaceWw(PlaceStatus.Active, pageToBeOpened: 0, isCreateNewPlace: true),
                () => placeChild1 = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: true, pageToBeOpened: 0,
                    isCreateNewPlace: true)
            );
            TestStart();
            placeChild1 = AssignAppToPlace(placeChild1, app, item1, ItemTypePorscheCar,
                isAddSilently: true);
            placeChild1 = AssignAppToPlace(placeChild1, app, item2, ItemTypePoi,
                isAddSilently: true, isAddItemToExistingApp: true);
            placeNoType = AssignChildrenToParentPlace(placeNoType, false, placeChild2, placeChild1);
            PreparePlace(TestConfig.Image138);

            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            MakeMapMarkersAccessible(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable1);
            Click(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild1.Title));
            var itemTitle = item1.LangJsonData.EnJson.Title;
            if (IsElementNotFoundQuickly(string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, itemTitle)))
            {
                DragAndDropByOffset(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, 
                    placeChild2.Title), new Point(0, 0), new Point(160, 0));
                Click(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild1.Title));
            }
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, itemTitle)),
                $@"Child places side bar should have a child place with Porsche Car item '{itemTitle}'");
            var poiTitle = item2.LangJsonData.EnJson.Title;
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, poiTitle)),
                $@"Child places side bar should have a child place with POI item '{poiTitle}'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionSidebarChildPlaceNormalImage),
                "Child places side bar should have a child place with Porsche Car item without image");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionSidebarChildPlaceNormalImage),
                "Child places side bar should have a child place with POI item with image");

            var location = GetElementLocation(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, placeChild1.Title));
            DragAndDrop(
                string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, placeChild1.Title), PlacesPage.ChildPlacesSectionMap);
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild2.Title)),
                "Child places side bar should have map with non-draggable marker");
            var place1Color = GetElementCssValue(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, placeChild1.Title), "background-color");

            Click(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild2.Title));
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceTitle, placeChild2.Title)),
                $@"Child places side bar should have child place '{placeChild2.Title}'");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, StatusNoDevice)),
                $@"Child places side bar should have child place in status '{StatusNoDevice}'");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, DeviceTypeWw)),
                $@"Child places side bar should have child place with device type '{DeviceTypeWw}'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionSidebarChildPlaceViewButton),
                "Child places side bar should have child place View button");

            DragAndDropByOffset(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, placeChild2.Title),
                new Point(0, 0), new Point(100, 0)); 
            var location2 = GetElementLocation(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, placeChild2.Title));
            Assert.IsTrue(location != location2, "Both place map markers should be on different places on the map");
            
            Assert.IsTrue(
                GetElementCssValue(
                    string.Format(
                        PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, placeChild2.Title), "background-color") != place1Color, 
                "Web element colors of place markers should be different");

            var place2Size = GetElementSize(
                string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapOuter, placeChild2.Title));
            var place1Size = GetElementSize(
                string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapOuter, placeChild1.Title));
            SendText(PlacesPage.PlaceWidth, (PlaceWidth * 1.5).ToString(CultureInfo.InvariantCulture));
            WaitTime(0.5);
            var place1NewSize = GetElementSize(
                string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapOuter, placeChild1.Title));
            var place2NewSize = GetElementSize(
                string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapOuter, placeChild2.Title));
            Assert.IsTrue(Math.Abs(place1NewSize.Width * 1.5 - place1Size.Width) < 1,
                "Child place WW map marker outer size should be increased 1.5 times after Place Width field value increase");
            Assert.IsTrue(Math.Abs(place2NewSize.Width * 1.5 - place2Size.Width) < 1,
                "Child place iBeacon map marker outer size should be increased 1.5 times after Place Width field value increase");

            SubmitForm();
            ClickUntilShown(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild1.Title),
                string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlacePlaceholder, itemTitle));
            Click(string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlacePlaceholder, itemTitle));
            Assert.IsTrue(IsPageContainsUri($"{TestConfig.ItemUri}/{item1.Id}"), 
                "User is not redirected to Porsche Car item page");
        }

        [Test, Regression]
        public void RT04410_ChildPlacesMapMarkerSizeChange()
        {
            Place placeNoType = null, placeChild1 = null, placeChild2 = null, placeChild3 = null;
            Item item1 = null, item2 = null;
            AppResponse app = null;
            Parallel.Invoke(
                () => placeNoType = AddPlaceNoType(PlaceStatus.Any, isAddChild: false, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => item1 = AddItem(ItemType.PorscheCar),
                () => item2 = AddItem(ItemType.Poi),
                () => app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]),
                () => placeChild3 = AddPlaceIbeacon(PlaceStatus.Active, pageToBeOpened: 0),
                () => placeChild2 = AddPlaceIbeacon(PlaceStatus.Active, pageToBeOpened: 0),
                () => placeChild1 = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: true, pageToBeOpened: 0,
                    isCreateNewPlace: true)
            );
            placeChild1 = AssignAppToPlace(placeChild1, app, item1, ItemTypePorscheCar,
                isAddSilently: true);
            Parallel.Invoke(
                () => placeChild1 = AssignAppToPlace(placeChild1, app, item2, ItemTypePoi,
                    isAddSilently: true, isAddItemToExistingApp: true),
                () => placeNoType = AssignChildrenToParentPlace(placeNoType, true, placeChild3)
            );
            TestStart();
            Parallel.Invoke(
                () => PlaceApi.DeletePlace(placeChild3),
                () => placeNoType = AssignChildrenToParentPlace(placeNoType, false, placeChild2, placeChild1)
            );
            PreparePlace(TestConfig.Image138);

            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            Assert.IsTrue(IsElementFound(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild1.Title)),
                $@"Child place 1 '{placeChild1.Title}' should be displayed on map with place");
            Assert.IsTrue(IsElementFound(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild2.Title)),
                $@"Child place 2 '{placeChild2.Title}' should be displayed on map with place");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild3.Title)),
                $@"Child place 3 '{placeChild3.Title}' (deleted) should not be displayed on map with place");

            Click(PlacesPage.ChildPlacesSectionTableViewButton);
            Click(PlacesPage.ChildPlacesSectionShowDeletedButton);
            Assert.IsTrue(IsElementFound(
                    string.Format(PlacesPage.ChildPlacesSectionTableRowsColumnTitleValue, placeChild3.Title)),
                $@"Child place 3 '{placeChild3.Title}' (deleted) should be displayed on child places table");

            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            Assert.IsTrue(IsElementFound(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild1.Title)),
                $@"Child place 1 '{placeChild1.Title}' should be displayed on map with place");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild2.Title)),
                $@"Child place 2 '{placeChild2.Title}' should be displayed on map with place");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild3.Title)),
                $@"Child place 3 '{placeChild3.Title}' (deleted) should not be displayed on map with place");

            Click(PlacesPage.ChildPlacesSectionMapWithItemsButton);
            var itemTitle = item1.LangJsonData.EnJson.Title;
            var poiTitle = item2.LangJsonData.EnJson.Title;
            Assert.IsTrue(IsElementFound(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, itemTitle)),
                $@"Place 1 '{placeChild1.Title}' should show item Car Porsche '{itemTitle}' on map marker");
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, poiTitle)),
                $@"Place 1 '{placeChild1.Title}' should show item POI '{poiTitle}' on map marker");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapEmpty),
                $@"Child place 2 '{placeChild2.Title}' should display empty map marker");
            
            var place1Opacity = GetElementCssValue(
                string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapOuter, itemTitle), "opacity");
            var place2Opacity = GetElementCssValue(
                PlacesPage.ChildPlacesSectionPlaceMarkerOnMapEmptyOuter, "opacity");
            Assert.IsTrue(place1Opacity == place2Opacity, 
                "Child 1 and child 2 places map markers should have the same opacity on map");
            
            MakeMapMarkersAccessible(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable1);
            ClickAtPoint(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable1, 
                10, 10, MoveToElementOffsetOrigin.Center);
            if (IsElementNotFoundQuickly(
                string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, itemTitle)))
            {
                var size = GetElementSize(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap1);
                var pointToDrag = new Point
                {
                    X = Convert.ToInt32(size.Width / 2) + 10,
                    Y = Convert.ToInt32(size.Height / 2) + 10
                };
                DragAndDropByOffset(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap1, pointToDrag, new Point(100, 0));
                ClickAtPoint(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, itemTitle),
                    10, 10, MoveToElementOffsetOrigin.Center);
            }
            DragAndDrop(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, itemTitle), new Point(10, 10),
                PlacesPage.ChildPlacesSectionMap);
            Click(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMap, itemTitle));
            //TODO

            //var place1Size =
            //    GetElementSize(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapOuter, itemTitle));
            //// calculate map marker's top point offset (outer circle top edge point)
            //var pointDragFrom = new Point
            //{
            //    X = Convert.ToInt32(place1Size.Width),
            //    Y = Convert.ToInt32(place1Size.Width / 2)
            //};
            //// drag outer circle top 10 pixels higher
            //DragAndDropByOffset(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapOuter, itemTitle),
            //    pointDragFrom, new Point(10, 0));
            //var place1NewSize =
            //    GetElementSize(string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapOuter, itemTitle));
            //Assert.IsTrue(place1NewSize.Height > place1Size.Height && place1NewSize.Width > place1Size.Width,
            //    "Child place map marker outer size should be increased");

            SubmitForm();
            Assert.IsTrue(IsElementFound(
                    string.Format(PlacesPage.ChildPlacesSectionSidebarChildPlaceContent, itemTitle)),
                $@"Child places side bar should be opened and contain items of child place 1 '{placeChild1.Title}'");

            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            Assert.IsTrue(IsElementFound(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild1.Title)),
                $@"Child place 1 '{placeChild1.Title}' should be displayed on map with place");
            Assert.IsTrue(IsElementFound(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild2.Title)),
                $@"Child place 2 '{placeChild2.Title}' should be displayed on map with place");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(PlacesPage.ChildPlacesSectionPlaceMarkerOnMapNonDraggable, placeChild3.Title)),
                $@"Child place 3 '{placeChild3.Title}' (deleted) should not be displayed on map with place");
        }

        [Test, Regression]
        public void RT04420_PlacePicturesProcessing()
        {
            TestStart();
            AddPlaceNoType(PlaceStatus.Any, isAddChild: true, isCreateNewPlace: true);
            PreparePlace(string.Empty);
            
            //StartEdit();
            Click(PlacesPage.PlaceMapUploadButton);
            FileManager.Upload(TestConfig.Image025);
            Assert.IsTrue(IsElementFound(PlacesPage.AssetConstraintDialog),
                "Asset aspect ratio dialog should be displayed (MinRatio = 0.4)");

            Click(PlacesPage.OkButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PlaceMapImageEmpty),
                "Place Map field contains image, but it should not");

            Click(PlacesPage.PlaceMapImageEmpty);
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.TableRowSelected),
                "In media library no image should be selected");

            Click(PlacesPage.UploadButton);
            FileManager.Upload(TestConfig.Image285);
            Assert.IsTrue(IsElementFound(PlacesPage.AssetConstraintDialog),
                "Asset aspect ratio dialog should be displayed (MaxRatio = 2.5)");

            Click(PlacesPage.OkButton);
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.MediaLibraryDialog), 
                "Media library dialog should stay open");

            Click(PlacesPage.UploadButton);
            FileManager.Upload(TestConfig.ImageGif);
            Assert.IsTrue(IsElementFound(PlacesPage.AssetFormatDialog),
                "Asset image formats supported dialog should be displayed");

            Click(PlacesPage.OkButton);
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.MediaLibraryDialog), 
                "Media library dialog should stay opened");

            Click(PlacesPage.UploadButton);
            FileManager.Upload(TestConfig.Image04);
            Assert.IsTrue(IsElementFound(MediaLibrary.TableRowSelected),
                "There is no selected image 0.4 in media library");

            Click(PlacesPage.SubmitButton);
            Assert.IsTrue(IsElementFound(PlacesPage.PlaceMapImageInline), 
                "There is no image 0.4 in Place Map field");
            Click(PlacesPage.PlaceMapUploadButton);
            FileManager.Upload(TestConfig.ImageSvg);
            Assert.IsTrue(IsElementFound(PlacesPage.PlaceMapImageInline), 
                "There is no SVG image in Place Map field");

            Click(PlacesPage.ChildPlacesSectionMapWithPlacesButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ChildPlacesSectionMap), 
                "Child places section should display map image");
        }

        [Test, Regression]
        public void RT04430_PlaceKeyboardShortcuts()
        {
            AddAppPlayer();
            AddAppComposerHq2();
            TestStart();
            var place = AddPlaceWw(PlaceStatus.Any, isCreateNewPlace: true);

            IsElementFound(PlacesPage.TitleReadOnly);
            PressKeys(Keys.F2);
            Assert.IsTrue(IsEditMode(), "'F2' should switch place to edit mode");

            PressKeys(Keys.Escape);
            Assert.IsTrue(IsViewMode(), "'Esc' should switch place to view mode");

            WaitTime(3);
            PressKeys(Keys.Insert);
            Assert.IsTrue(IsElementFound(PlacesPage.SelectAppDialog) 
                          || IsElementFoundQuickly(PlacesPage.NoAppsToBeAddedDialog), 
                @"'Insert' should open 'Apps' or 'No apps to be added' dialog");

            WaitTime(2);
            PressKeys(Keys.Escape);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.SelectAppDialog), 
                @"'Esc' should close 'Apps' dialog");
            Assert.IsTrue(IsViewMode(), "Place should be in view mode");

            EditForm();
            var newTitle = $"Auto test {RandomNumber}";
            SendText(PlacesPage.Title, newTitle);
            Click(PageHeader.PageAppsButton);
            Assert.IsTrue(IsElementFound(PlacesPage.ChangesWillBeDiscardedDialog),
                @"'Place changes will be discarded' dialog should open");
            PressKeys(Keys.Escape);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChangesWillBeDiscardedDialog),
                @"'Esc' should close 'Your changes will be discarded' dialog");
            Assert.IsTrue(IsEditMode(), "Place should be in edit mode");

            ClickUntilShown(PlacesPage.TimezoneDropDown, CommonElement.DropDownOptionList);
            PressKeys(Keys.Escape);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.TimezoneListItemsWindow), 
                "'Esc' 1st time - Timezone drop-down is still open");
            PressKeys(Keys.Escape);
            Assert.IsTrue(IsViewMode(), 
                "'Esc' 2nd time - Place should be in view mode");
            PressKeys(Keys.Escape);
            var placesPageUrl = $"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlacesUri}";
            Assert.IsTrue(IsPageRedirectedTo(placesPageUrl), 
                "'Esc' 3rd time - User should be redirected to Places main page");

            Click(PageHeader.PagePlacesButton);
            PressKeys(Keys.Insert);
            Assert.IsTrue(IsElementFound(PlacesPage.Title), "'Insert' should open new place page");

            PressKeys(Keys.Escape);
            Assert.IsTrue(IsPageRedirectedTo(placesPageUrl),
                "'Esc' pressed - User should be redirected to Places main page");

            PressKeys("t");
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.TenantsUrl}"),
                "'T' pressed - User should be redirected to Tenants page");

            PressKeys(Keys.Escape);
            Assert.IsTrue(IsPageRedirectedTo(placesPageUrl),
                "'Esc' pressed - User should be redirected to Places main page");

            OpenEntityPage(place);
            EditForm();
            PressKeys(Keys.Control + Keys.Enter);
            Assert.IsTrue(IsViewMode(), "'Ctrl + Enter' should submit place");

            PressKeys(Keys.Delete);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DeleteDialog), 
                "'Delete' should display place delete dialog");

            PressKeys(Keys.Escape);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DeleteDialog), 
                "'Esc' should close delete dialog");
            Assert.IsTrue(IsViewMode(), "'Esc' pressed - Place should be in view mode");

            PressKeys("q");
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.LoginUrl}"),
                "'Q' pressed - User should be logged out and login page should be opened.");
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
