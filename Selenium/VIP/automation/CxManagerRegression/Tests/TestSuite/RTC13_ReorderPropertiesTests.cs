using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Models.Items;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC13_ReorderPropertiesTests : ParentTest
    {
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

            CurrentTenant = TenantTitle.reordering1;
            CurrentUser = TestConfig.AdminUser;
        }

        private void PrepareItem(Item item)
        {
            OpenEntityPage(item);
            EditForm();
            SendText(ItemsPage.Title, $"Auto test {RandomNumber}");
            SendText(ItemsPage.Vin, $"{RandomNumber}88");
            for (var i = 1; i <= 3; i++)
            {
                ClickUntilShown(ItemsPage.SerialDataSectionAddButton, ItemsPage.SerialDataSectionRowHeadline);
                SendText(ItemsPage.SerialDataSectionRowHeadline, $"Serial {i}");
                ClickUntilShown(ItemsPage.SerialDataSectionEntriesSectionAddButton, 
                    ItemsPage.SerialDataSectionEntriesSectionEntry);
                SendText(ItemsPage.SerialDataSectionEntriesSectionEntry, $"E1S{i}");
            }
            ClickUntilShown(ItemsPage.TechnicalDataSectionAddButton, ItemsPage.TechnicalDataSectionRowHeadline);
            SendText(ItemsPage.TechnicalDataSectionRowHeadline, "Tech 1");
            ClickUntilShown(ItemsPage.TechnicalDataSectionAddButton,
                ItemsPage.TechnicalDataSectionRowHeadline);
            SendText(ItemsPage.TechnicalDataSectionRowHeadline, "Tech 2");
            SubmitForm();
        }

        private void PreparePictures()
        {
            EditForm();
            ClickUntilShown(ItemsPage.PicturesButton,
                ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            ClickUntilShown(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton,
                ItemsPage.PicturesSectionExteriorPicturesSectionUploadButton);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionUploadButton);
            FileManager.Upload(TestConfig.ImageJpeg);
            ClickUntilConditionMet(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton,
                () => CountElements(ItemsPage.PicturesSectionExteriorPicturesSectionUploadButton) == 2);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionUploadLastButton);
            FileManager.Upload(TestConfig.ImageCar);
            ClickUntilShown(ItemsPage.PicturesSectionInteriorPicturesSectionAddButton,
                ItemsPage.PicturesSectionInteriorPicturesSectionUploadButton);
            Click(ItemsPage.PicturesSectionInteriorPicturesSectionUploadButton);
            FileManager.Upload(TestConfig.Image08);
            ClickUntilConditionMet(ItemsPage.PicturesSectionInteriorPicturesSectionAddButton,
                () => CountElements(ItemsPage.PicturesSectionInteriorPicturesSectionUploadButton) == 2);
            Click(ItemsPage.PicturesSectionInteriorPicturesSectionUploadLastButton);
            FileManager.Upload(TestConfig.ImagePoi);
        }

        [Test, Regression]
        public void RT13010_PropertiesOrderingBasics()
        {
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            var item = AddItem(ItemType.PorscheCar);
            TestStart();

            OpenEntityPage(item);
            Assert.IsTrue(IsElementNotFound(PageFooter.PropertiesOrderButton), 
                "Properties Order button should be not shown in page footer in view mode");

            EditForm();
            var errorsDuringImportLocation = GetElementLocation(ItemsPage.ErrorsDuringImport);
            var vinLocation = GetElementLocation(ItemsPage.Vin);
            Click(PageFooter.PropertiesOrderButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ChangeOrderDialog),
                "Change order dialog should be shown");
            Assert.IsTrue(
                GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Errors during import"), "data-idx") == "0",
                @"'Errors during import' property should be first item in change order of properties list");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Errors during import"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Engine"), "data-idx")) < 0,
                @"'Errors during import' property should be shown higher than 'Engine' item in change order of properties list");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Engine"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Finance Offer"), "data-idx")) < 0,
                @"'Engine' property should be shown higher than 'Finance Offer' item in change order of properties list");
            
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TablePropertyByText, "Capacity")),
                "There should be no collapsed item list nodes in change order list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TablePropertyByText, "Option")),
                "There should be no collapsed item list nodes in change order list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TablePropertyByText, "Mileage")),
                "There should be no collapsed item list nodes in change order list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TablePropertyByText, "Exterior pictures")),
                "There should be no collapsed item list nodes in change order list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TablePropertyByText, "Currency")),
                "There should be no collapsed item list nodes in change order list");

            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "VIN"),
                string.Format(ItemsPage.TablePropertyByText, "Errors during import"), "data-lev");
            Assert.IsTrue(
                GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "VIN"), "data-idx") == "0",
                @"After drag and drop 'VIN' property should be first item in change order of properties list");

            Click(ItemsPage.CancelButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.ChangeOrderDialog),
                "Change properties dialog should be closed on Cancel press");
            
            Assert.IsTrue(errorsDuringImportLocation == GetElementLocation(ItemsPage.ErrorsDuringImport),
                "'Errors during import' field location should not change on item page");
            Assert.IsTrue(vinLocation == GetElementLocation(ItemsPage.Vin),
                "'VIN' field location should not change on item page");

            Click(PageFooter.PropertiesOrderButton);
            Assert.IsTrue(
                GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Errors during import"), "data-idx") == "0",
                @"'Errors during import' property should be first item in change order of properties list");

            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "Errors during import"), 
                string.Format(ItemsPage.TablePropertyByText, "Story"), "data-lev");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Errors during import"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Story"), "data-idx")) > 0,
                @"'Errors during import' property should be shown under 'Story' item in change order of properties list");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Errors during import"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Engine"), "data-idx")) < 0,
                @"'Errors during import' property should be shown higher than 'Engine' item in change order of properties list");

            Click(ItemsPage.DoneButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.ChangeOrderDialog),
                "Change properties dialog should be closed on Done press");
            Assert.IsTrue(IsEditMode(),
                "Item page should be in edit mode on change order dialog Done button press");

            Assert.IsTrue(GetElementLocation(ItemsPage.Story).Y < GetElementLocation(ItemsPage.ErrorsDuringImport).Y,
                "Field 'Story' should be displayed above 'Errors during import' field");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(GetElementLocation(ItemsPage.ErrorsDuringImportReadOnly).X == errorsDuringImportLocation.X,
                @"After page footer button Cancel press 'Errors during import' property should be on its original position");

            EditForm();
            Click(PageFooter.PropertiesOrderButton);
            Assert.IsTrue(
                GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Errors during import"), "data-idx") == "0",
                @"'Errors during import' property should be first item in change order of properties list");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Errors during import"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "VIN"), "data-idx")) < 0,
                @"'Errors during import' property should be shown above 'VIN' item in change order of properties list");

            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "Pictures"), 
                string.Format(ItemsPage.TablePropertyByText, "Errors during import"), "data-lev");
            Assert.IsTrue(
                GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Pictures"), "data-idx") == "0",
                @"After drag and drop 'Pictures' property group should be first item in change order of properties list");

            Click(ItemsPage.DoneButton);
            Assert.IsTrue(GetElementLocation(ItemsPage.PicturesButton).X < errorsDuringImportLocation.X 
                          || GetElementLocation(ItemsPage.PicturesButton).Y < errorsDuringImportLocation.Y,
                @"'Pictures' button should be displayed before 'Errors during import' field in edit mode");

            SubmitForm();
            Assert.IsTrue(GetElementLocation(ItemsPage.PicturesButton).X < errorsDuringImportLocation.X
                          || GetElementLocation(ItemsPage.PicturesButton).Y < errorsDuringImportLocation.Y,
                @"'Pictures' button should be displayed before 'Errors during import' field after submit");

            RefreshPage();
            Assert.IsTrue(GetElementLocation(ItemsPage.PicturesButton).X < errorsDuringImportLocation.X
                          || GetElementLocation(ItemsPage.PicturesButton).Y < errorsDuringImportLocation.Y,
                @"'Pictures' button should be displayed before 'Errors during import' field after page refresh");
        }

        [Test, Regression]
        public void RT13020_OrderingOfPropertyGroups()
        {
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            var item = AddItem(ItemType.UsedCar);
            TestStart();

            OpenEntityPage(item);
            EditForm();
            // technical insertion
            ClickUntilShown(ItemsPage.FuelConsumptionDataButton, ItemsPage.EnergyEfficiencyClass);
            var extraUrbanInL100KmLocation = GetElementLocation(ItemsPage.ExtraUrbanInL100Km);
            //
            Click(PageFooter.PropertiesOrderButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ChangeOrderDialog), "Change order dialog should be shown");

            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "Teaser"), 
                string.Format(ItemsPage.TablePropertyByText, "Transmission"), "data-lev");
            Assert.IsTrue(
                GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Teaser"), "data-lev").Contains("root"),
                @"Standalone 'Teaser' property cannot be put into property group");

            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "Teaser"), 
                string.Format(ItemsPage.TablePropertyByText, "Vehicle Data"), "data-lev");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Vehicle Data"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Teaser"), "data-idx")) < 0,
                @"'Teaser' property should be shown under 'Vehicle Data' group in change order of properties list");

            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TablePropertyCollapsedByText, "Vehicle Data")),
                @"'Vehicle Data' property group should be shown collapsed in change order of properties list");

            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "Extra urban in l/100 km"), 
                string.Format(ItemsPage.TablePropertyByText, "Energy Efficiency Class"), "data-lev");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Extra urban in l/100 km"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Energy Efficiency Class"), "data-idx")) > 0,
                @"'Extra urban in l/100 km' property should be shown under 'Energy Efficiency Class' item in change order of properties list");

            var eecOrderNo =
                GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Energy Efficiency Class"), "data-idx");
            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "Energy Efficiency Class"),
                string.Format(ItemsPage.TablePropertyByText, "Equipment"), "data-lev");
            Assert.IsTrue(
                eecOrderNo == 
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Energy Efficiency Class"), "data-idx"),
                @"Drag and drop of property group member 'Energy Efficiency Class' outside its group should be impossible");

            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "Fuel Consumption Data"),
                string.Format(ItemsPage.TablePropertyByText, "Vehicle Data"), "data-lev");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Fuel Consumption Data"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Vehicle Data"), "data-idx")) > 0,
                @"'Fuel Consumption Data' group should be shown under 'Vehicle Data' group in change order of properties list");

            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TablePropertyCollapsedByText, "Fuel Consumption Data")),
                @"'Fuel Consumption Data' property group should be shown collapsed in change order of properties list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TablePropertyCollapsedByText, "Vehicle Data")),
                @"'Vehicle Data' property group should be shown collapsed in change order of properties list");

            Click(ItemsPage.DoneButton);
            var fuelConsumptionDataLocation = GetElementLocation(ItemsPage.FuelConsumptionDataButton);
            Assert.IsTrue(GetElementLocation(ItemsPage.VehicleDataButton).X < fuelConsumptionDataLocation.X
                          || GetElementLocation(ItemsPage.VehicleDataButton).Y < fuelConsumptionDataLocation.Y,
                @"'Vehicle Data' button should be displayed before 'Fuel Consumption Data' button in edit mode");
            var teaserLocation = GetElementLocation(ItemsPage.TeaserCheckBox);
            Assert.IsTrue(GetElementLocation(ItemsPage.FuelConsumptionDataButton).X < teaserLocation.X
                          || GetElementLocation(ItemsPage.FuelConsumptionDataButton).Y < teaserLocation.Y,
                @"'Fuel Consumption Data' button should be displayed before 'Teaser' check box in edit mode");

            ClickUntilShown(ItemsPage.FuelConsumptionDataButton, ItemsPage.EnergyEfficiencyClass);
            Assert.IsTrue(GetElementLocation(ItemsPage.ExtraUrbanInL100Km).Y > extraUrbanInL100KmLocation.Y
                          || GetElementLocation(ItemsPage.ExtraUrbanInL100Km).X > extraUrbanInL100KmLocation.X,
                @"Fuel Consumption Data > 'Extra urban in l/100 km' should be displayed as the last field in the section");

            ClickUntilShown(ItemsPage.PicturesSectionAddButton, ItemsPage.PicturesSectionUploadButton);
            Click(ItemsPage.PicturesSectionUploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be saved successfully");
        }

        [Test, Regression]
        public void RT13030_PropertyOrderingTransitions()
        {
            CurrentTenant = TenantTitle.reordering2;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            var item4 = AddItem(ItemType.PorscheCar);
            CurrentTenant = TenantTitle.reordering1;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            var item2 = AddItem(ItemType.PorscheCar);
            var item3 = AddItem(ItemType.PorscheCar, isAddNew: true);
            TestStart();

            OpenEntityPage(item2);
            EditForm();
            Click(PageFooter.PropertiesOrderButton);
            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "btyp"),
                string.Format(ItemsPage.TablePropertyByText, "Individual Options"), "data-lev");
            Click(ItemsPage.DoneButton);
            Assert.IsTrue(
                GetElementLocation(ItemsPage.Btyp).Y > GetElementLocation(ItemsPage.IndOptionsSectionAddButton).Y,
                @"Lang EN: 'btyp' field should be displayed below 'Individual Options' section");

            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(
                GetElementLocation(ItemsPage.BtypReadOnly).Y > GetElementLocation(ItemsPage.IndOptionsSectionAddButton).Y,
                @"Lang DE: 'btyp' field should be displayed below 'Individual Options' section");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be saved successfully");

            OpenEntityPage(item3);
            var btypYCoord = GetElementLocation(ItemsPage.BtypReadOnly).Y;
            Assert.IsTrue(btypYCoord > GetElementLocation(ItemsPage.IndOptionsSection).Y
                    && btypYCoord < GetElementLocation(ItemsPage.TEquipmentSection).Y,
                @"Another item: 'btyp' field should be displayed between 'Individual Options' and 'TEquipment' sections");

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            Assert.IsTrue(
                GetElementLocation(ItemsPage.Btyp).Y > GetElementLocation(ItemsPage.IndOptionsSectionAddButton).Y,
                @"New item: 'btyp' field should be displayed below 'Individual Options' section");

            Click(PageFooter.CancelButton);
            ChangeTenant(TenantTitle.reordering2);
            OpenEntityPage(item4);
            var btypCoord = GetElementLocation(ItemsPage.BtypReadOnly);
            var teaserCoord = GetElementLocation(ItemsPage.TeaserCheckBox);
            Assert.IsTrue(btypCoord.X < teaserCoord.X || btypCoord.Y < teaserCoord.Y,
                @"Reordering2 tenant: 'btyp' field should be displayed above 'Individual Options' section");

            ChangeTenant(TenantTitle.reordering1);
            AddAppIbeacon(TestConfig.IbeaconAppVersions[3]);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            Assert.IsTrue(
                GetElementLocation(ItemsPage.Btyp).Y > GetElementLocation(ItemsPage.IndOptionsSectionAddButton).Y,
                @"New item: 'btyp' field should be displayed below 'Individual Options' section");

            Click(PageFooter.PropertiesOrderButton);
            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "Teaser"),
                string.Format(ItemsPage.TablePropertyByText, "Exterior Color"), "data-lev");
            Click(ItemsPage.DoneButton);
            SendText(ItemsPage.Title, $"Auto test {RandomNumber}");
            SendText(ItemsPage.Vin, $"{RandomNumber}99");
            SubmitForm();
            Assert.IsTrue(
                GetElementLocation(ItemsPage.TeaserCheckBox).Y < GetElementLocation(ItemsPage.ExteriorColorReadOnly).Y 
                    || GetElementLocation(ItemsPage.TeaserCheckBox).X < GetElementLocation(ItemsPage.ExteriorColorReadOnly).X,
                @"New item: 'Teaser' field should be displayed before 'Exterior Color' field");
        }

        [Test, Regression]
        public void RT13040_SectionPropertyOrdering()
        {
            CurrentTenant = TenantTitle.reordering2;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            var item1 = AddItem(ItemType.PorscheCar, isAddNew: true);
            var item2 = AddItem(ItemType.PorscheCar, isAddNew: true);
            TestStart();
            PrepareItem(item2);
            PrepareItem(item1);

            EditForm();
            Click(ItemsPage.SerialDataSectionChangeOrderButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionAddButton), 
                "Serial Data > +Add button should be hidden on Change Order button click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionCancelButton),
                "Serial Data > Cancel button should be shown on Change Order button click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionDoneButton),
                "Serial Data > Done button should be shown on Change Order button click");
            Assert.IsTrue(CountElements(ItemsPage.SerialDataSectionHighlightedRow) == 3,
                "Serial Data > all 3 rows should be highlighted on Change Order button click");

            DragAndDropHtml5(ItemsPage.SerialDataSectionRow1, ItemsPage.SerialDataSectionRow3);
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 2",
                @"Serial Data > row 1 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 3",
                @"Serial Data > row 2 should contain text 'Serial 3'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow3) == "3Serial 1",
                @"Serial Data > row 3 should contain text 'Serial 1'");

            Click(ItemsPage.TechnicalDataSectionChangeOrderButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.TechnicalDataSectionAddButton),
                "Technical Data > +Add button should be hidden on Change Order button click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TechnicalDataSectionCancelButton),
                "Technical Data > Cancel button should be shown on Change Order button click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TechnicalDataSectionDoneButton),
                "Technical Data > Done button should be shown on Change Order button click");
            Assert.IsTrue(CountElements(ItemsPage.TechnicalDataSectionHighlightedRow) == 2,
                "Technical Data > both rows should be highlighted on Change Order button click");

            Assert.IsTrue(CountElements(ItemsPage.SerialDataSectionHighlightedRow) == 3,
                "Serial Data > all 3 rows should remain highlighted on Change Order button click " +
                "in Technical Data section");

            DragAndDropHtml5(ItemsPage.TechnicalDataSectionRow1, ItemsPage.TechnicalDataSectionRow2);
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow1) == "1Tech 2",
                @"Technical Data > row 1 should contain text 'Tech 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow2) == "2Tech 1",
                @"Technical Data > row 2 should contain text 'Tech 1'");

            Click(ItemsPage.TechnicalDataSectionDoneButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TechnicalDataSectionAddButton),
                "Technical Data > +Add button should be shown on Done button click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TechnicalDataSectionChangeOrderButton),
                "Technical Data > Change Order button should be shown on Done button click");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.TechnicalDataSectionCancelButton),
                "Technical Data > Cancel button should be hidden on Done button click");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.TechnicalDataSectionDoneButton),
                "Technical Data > Done button should be hidden on Done button click");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.TechnicalDataSectionHighlightedRow),
                "Technical Data > no rows should be highlighted on Done button click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionCancelButton),
                "Serial Data > Cancel button should be shown on Done button click " +
                "in Technical Data section");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionDoneButton),
                "Serial Data > Done button should be shown on Done button click " +
                "in Technical Data section");

            Click(ItemsPage.SerialDataSectionDoneButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionAddButton),
                "Serial Data > +Add button should be shown on Done button click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionChangeOrderButton),
                "Serial Data > Change Order button should be shown on Done button click");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionCancelButton),
                "Serial Data > Cancel button should be hidden on Done button click");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionDoneButton),
                "Serial Data > Done button should be hidden on Done button click");

            SubmitForm();
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 2",
                @"Serial Data > row 1 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 3",
                @"Serial Data > row 2 should contain text 'Serial 3'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow3) == "3Serial 1",
                @"Serial Data > row 3 should contain text 'Serial 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow1) == "1Tech 2",
                @"Technical Data > row 1 should contain text 'Tech 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow2) == "2Tech 1",
                @"Technical Data > row 2 should contain text 'Tech 1'");

            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.TechnicalDataSectionChangeOrderButton),
                "Technical Data > Change Order button should be hidden on Submit button click");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionChangeOrderButton),
                "Serial Data > Change Order button should be hidden on Submit button click");

            RefreshPage();
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 2",
                @"Serial Data > row 1 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 3",
                @"Serial Data > row 2 should contain text 'Serial 3'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow3) == "3Serial 1",
                @"Serial Data > row 3 should contain text 'Serial 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow1) == "1Tech 2",
                @"Technical Data > row 1 should contain text 'Tech 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow2) == "2Tech 1",
                @"Technical Data > row 2 should contain text 'Tech 1'");

            OpenEntityPage(item2);
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 1",
                @"Other item2 > Serial Data > row 1 should contain text 'Serial 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 2",
                @"Other item2 > Serial Data > row 2 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow3) == "3Serial 3",
                @"Other item2 > Serial Data > row 3 should contain text 'Serial 3'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow1) == "1Tech 1",
                @"Other item2 > Technical Data > row 1 should contain text 'Tech 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow2) == "2Tech 2",
                @"Other item2 > Technical Data > row 2 should contain text 'Tech 2'");
        }

        [Test, Regression]
        public void RT13050_SectionPropertyOrderingTransitions1()
        {
            CurrentTenant = TenantTitle.reordering2;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            var item = AddItem(ItemType.PorscheCar, isAddNew: true);
            TestStart();
            PrepareItem(item);

            EditForm();
            Click(ItemsPage.SerialDataSectionChangeOrderButton);
            Click(ItemsPage.TechnicalDataSectionChangeOrderButton);
            // exception flow is commented out due to exception prevention in DragAndDropHtml5()
            //
            //DragAndDropHtml5(ItemsPage.TechnicalDataSectionRow2, ItemsPage.SerialDataSectionRow1);
            //Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 1",
            //    @"Drag-and-drop between sections > Serial Data > row 1 should contain text 'Serial 1'");
            //Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 2",
            //    @"Drag-and-drop between sections > Serial Data > row 2 should contain text 'Serial 2'");
            //Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow3) == "3Serial 3",
            //    @"Drag-and-drop between sections > Serial Data > row 3 should contain text 'Serial 3'");
            //Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow1) == "1Tech 1",
            //    @"Drag-and-drop between sections > Technical Data > row 1 should contain text 'Tech 1'");
            //Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow2) == "2Tech 2",
            //    @"Drag-and-drop between sections > Technical Data > row 2 should contain text 'Tech 2'");

            DragAndDropHtml5(ItemsPage.SerialDataSectionRow1, ItemsPage.SerialDataSectionRow3);
            Click(ItemsPage.SerialDataSectionCancelButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionAddButton),
                "Serial Data > +Add button should be shown on Cancel button click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionChangeOrderButton),
                "Serial Data > Change Order button should be shown on Cancel button click");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionCancelButton),
                "Serial Data > Cancel button should be hidden on Cancel button click");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionDoneButton),
                "Serial Data > Done button should be hidden on Cancel button click");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionHighlightedRow),
                "Serial Data > no rows should be highlighted on Cancel button click");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 1",
                @"Serial Data > row 1 should contain text 'Serial 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 2",
                @"Serial Data > row 2 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow3) == "3Serial 3",
                @"Serial Data > row 3 should contain text 'Serial 3'");

            DragAndDropHtml5(ItemsPage.TechnicalDataSectionRow1, ItemsPage.TechnicalDataSectionRow2);
            Click(ItemsPage.TechnicalDataSectionDoneButton);
            SubmitForm();
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow1) == "1Tech 2",
                @"Drag-and-drop between sections > Technical Data > row 1 should contain text 'Tech 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.TechnicalDataSectionRow2) == "2Tech 1",
                @"Drag-and-drop between sections > Technical Data > row 2 should contain text 'Tech 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 1",
                @"Serial Data > row 1 should contain text 'Serial 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 2",
                @"Serial Data > row 2 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow3) == "3Serial 3",
                @"Serial Data > row 3 should contain text 'Serial 3'");

            EditForm();
            Click(ItemsPage.SerialDataSectionChangeOrderButton);
            DragAndDropHtml5(ItemsPage.SerialDataSectionRow2, ItemsPage.SerialDataSectionRow1);
            Click(ItemsPage.SerialDataSectionDoneButton);
            Click(PageFooter.CancelButton);
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 1",
                @"Cancel pressed > Serial Data > row 1 should contain text 'Serial 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 2",
                @"Cancel pressed > Serial Data > row 2 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow3) == "3Serial 3",
                @"Cancel pressed > Serial Data > row 3 should contain text 'Serial 3'");

            EditForm();
            Click(ItemsPage.SerialDataSectionChangeOrderButton);
            DragAndDropHtml5(ItemsPage.SerialDataSectionRow2, ItemsPage.SerialDataSectionRow1);
            SubmitForm();
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 1",
                @"Submit without Done pressed > Serial Data > row 1 should contain text 'Serial 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 2",
                @"Submit without Done pressed > Serial Data > row 2 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow3) == "3Serial 3",
                @"Submit without Done pressed > Serial Data > row 3 should contain text 'Serial 3'");

            EditForm();
            ClickUntilShown(ItemsPage.TechnicalDataSectionRow2, ItemsPage.TechnicalDataSectionRowHeadline);
            Click(ItemsPage.TechnicalDataSectionRow2DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.TechnicalDataSectionRow2),
                "Technical Data > row 2 should be deleted");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.TechnicalDataSectionChangeOrderButton),
                "Technical Data > Change Order button should be hidden if one row list displayed");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be saved successfully");
        }

        [Test, Regression]
        public void RT13060_SectionPropertyOrderingTransitions2()
        {
            CurrentTenant = TenantTitle.reordering2;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            var item = AddItem(ItemType.PorscheCar, isAddNew: true);
            TestStart();
            PrepareItem(item);

            EditForm();
            ClickUntilShown(ItemsPage.SerialDataSectionRow1, 
                ItemsPage.SerialDataSectionEntriesSectionEntry);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionAddButton),
                "Serial Data > Entries > +Add button should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionChangeOrderButton),
                "Serial Data > Entries > Change Order button should be hidden");             
            var entryLocation = GetElementLocation(ItemsPage.SerialDataSectionEntriesSectionEntry);
            var addButtonLocation = GetElementLocation(ItemsPage.SerialDataSectionEntriesSectionAddButton);
            Assert.IsTrue(entryLocation.X < addButtonLocation.X || entryLocation.Y < addButtonLocation.Y,
                "Serial Data > Entries > +Add button should be the last section element");

            ClickUntilConditionMet(ItemsPage.SerialDataSectionEntriesSectionAddButton, 
                () => CountElements(ItemsPage.SerialDataSectionEntriesSectionEntry) == 2);
            SendText(ItemsPage.SerialDataSectionEntriesSectionEntryLast, "E2S1");
            entryLocation = GetElementLocation(ItemsPage.SerialDataSectionEntriesSectionEntryLast);
            addButtonLocation = GetElementLocation(ItemsPage.SerialDataSectionEntriesSectionAddButton);
            var chOrderButtonLocation = 
                GetElementLocation(ItemsPage.SerialDataSectionEntriesSectionChangeOrderButton);
            Assert.IsTrue(entryLocation.X < addButtonLocation.X || entryLocation.Y < addButtonLocation.Y,
                "Serial Data > Entries > +Add button should be on right hand or below entry fields");
            Assert.IsTrue(entryLocation.X < chOrderButtonLocation.X 
                          || entryLocation.Y < chOrderButtonLocation.Y,
                "Serial Data > Entries > Change Order button should be on right hand or below entry fields");

            Click(ItemsPage.SerialDataSectionEntriesSectionChangeOrderButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionAddButton),
                "Serial Data > Entries > +Add button should be hidden on Change Order press");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionChangeOrderButton),
                "Serial Data > Entries > Change Order button should be hidden on Change Order press");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionCancelButton),
                "Serial Data > Entries > Cancel button should be shown on Change Order press");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionDoneButton),
                "Serial Data > Entries > Change Order button should be shown on Change Order press");
            var cancelButtonLocation = 
                GetElementLocation(ItemsPage.SerialDataSectionEntriesSectionCancelButton);
            var doneButtonLocation = GetElementLocation(ItemsPage.SerialDataSectionEntriesSectionDoneButton);
            Assert.IsTrue(entryLocation.X < cancelButtonLocation.X 
                          || entryLocation.Y < cancelButtonLocation.Y,
                "Serial Data > Entries > Cancel button should be on right hand or below entry fields");
            Assert.IsTrue(entryLocation.X < doneButtonLocation.X
                          || entryLocation.Y < doneButtonLocation.Y,
                "Serial Data > Entries > Done button should be on right hand or below entry fields");
            Assert.IsTrue(CountElements(ItemsPage.SerialDataSectionEntriesSectionHighlightedEntry) == 2,
                "Serial Data > Entries > all entries should be shown highlighted");

            DragAndDropHtml5(ItemsPage.SerialDataSectionEntriesSectionEntryLast,
                ItemsPage.SerialDataSectionEntriesSectionEntry);
            Assert.IsTrue(AreCollectionsEqual(GetValuesAsList(ItemsPage.SerialDataSectionEntriesSectionEntry), 
                    new List<string> { "E2S1", "E1S1" }, true),
                @"Serial Data > Entries > after drag-and-drop entries order should be: 'E2S1', 'E1S1'");

            Click(ItemsPage.SerialDataSectionEntriesSectionDoneButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionAddButton),
                "Serial Data > Entries > +Add button should be shown on Done press");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionChangeOrderButton),
                "Serial Data > Entries > Change Order button should be shown on Done press");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionCancelButton),
                "Serial Data > Entries > Cancel button should be hidden on Done press");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionDoneButton),
                "Serial Data > Entries > Change Order button should be hidden on Done press");
            Assert.IsTrue(IsElementNotFound(ItemsPage.SerialDataSectionEntriesSectionHighlightedEntry),
                "Serial Data > Entries > all entries should be not highlighted on Done press");
            Assert.IsTrue(AreCollectionsEqual(GetValuesAsList(ItemsPage.SerialDataSectionEntriesSectionEntry),
                    new List<string> { "E2S1", "E1S1" }, true),
                @"Serial Data > Entries > on Done press entries order should be: 'E2S1', 'E1S1'");

            ClickUntilShown(ItemsPage.SerialDataSectionRow3,
                ItemsPage.SerialDataSectionEntriesSectionEntry);

            ClickUntilConditionMet(ItemsPage.SerialDataSectionEntriesSectionAddButton,
                () => CountElements(ItemsPage.SerialDataSectionEntriesSectionEntry) == 2);
            Click(ItemsPage.SerialDataSectionEntriesSectionChangeOrderButton);
            Click(ItemsPage.SerialDataSectionEntriesSectionEntryLastDeleteButton);
            Assert.IsTrue(CountElements(ItemsPage.SerialDataSectionEntriesSectionEntry) == 2,
                "Serial Data > Entries > entry delete button should be in active during change order");
            
            Click(ItemsPage.SerialDataSectionEntriesSectionDoneButton);
            Click(ItemsPage.SerialDataSectionEntriesSectionEntryLastDeleteButton);
            Assert.IsTrue(CountElements(ItemsPage.SerialDataSectionEntriesSectionEntry) == 1,
                "Serial Data > Entries > last empty entry should be deleted");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionEntriesSectionChangeOrderButton),
                "Serial Data > Entries > Change Order button should be hidden on Done press");

            SubmitForm();
            ClickUntilShown(ItemsPage.SerialDataSectionRow1, 
                ItemsPage.SerialDataSectionRowHeadlineReadOnly);
            Assert.IsTrue(
                AreCollectionsEqual(GetValuesAsList(ItemsPage.SerialDataSectionEntriesSectionEntryReadOnly),
                    new List<string> { "E2S1", "E1S1" }, true),
                @"Serial Data > row 1 > Entries > on Submit entries order should be: 'E2S1', 'E1S1'");
        }

        [Test, Regression]
        public void RT13070_SectionImageOrdering()
        {
            CurrentTenant = TenantTitle.reordering2;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            var item = AddItem(ItemType.PorscheCar, isAddNew: true);
            TestStart();
            OpenEntityPage(item);
            PreparePictures();
            
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionChangeOrderButton);
            Assert.IsTrue(
                CountElements(
                    ItemsPage.PicturesSectionExteriorPicturesSectionPictureControlHighlighted) == 2,
                "Pictures > Exterior pictures > both picture controls should be highlighted on Change Order press");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton),
                "Pictures > Exterior pictures > +Add button should be hidden on Change Order press");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSectionChangeOrderButton),
                "Pictures > Exterior pictures > Change Order button should be hidden on Change Order press");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSectionCancelButton),
                "Pictures > Exterior pictures > Cancel button should be shown on Change Order press");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSectionDoneButton),
                "Pictures > Exterior pictures > Change Order button should be shown on Change Order press");

            var imgExt1Link = GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPicture, "src");
            var imgExt2Link = GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPictureLast, "src");
            DragAndDropHtml5(ItemsPage.PicturesSectionExteriorPicturesSectionPictureControlLast,
                ItemsPage.PicturesSectionExteriorPicturesSectionPictureControl);
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPicture, "src") == imgExt2Link,
                "Pictures > Exterior pictures > pictures should exchange their places after drag-and-drop");
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPictureLast, "src") == imgExt1Link,
                "Pictures > Exterior pictures > pictures should exchange their places after drag-and-drop");

            Click(ItemsPage.PicturesSectionInteriorPicturesSectionChangeOrderButton);
            Assert.IsTrue(
                CountElements(
                    ItemsPage.PicturesSectionInteriorPicturesSectionPictureControlHighlighted) == 2,
                "Pictures > Interior pictures > both picture controls should be highlighted on Change Order press");
            Assert.IsTrue(
                CountElements(
                    ItemsPage.PicturesSectionExteriorPicturesSectionPictureControlHighlighted) == 2,
                "Pictures > Exterior pictures > both picture controls should be stay highlighted");

            // exception flow is commented out due to exception prevention in DragAndDropHtml5()
            //
            //var imgInt1Link = GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPicture, "src");
            //var imgInt2Link = GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPictureLast, "src");
            //DragAndDropHtml5(ItemsPage.PicturesSectionExteriorPicturesSectionPictureControlLast,
            //    ItemsPage.PicturesSectionInteriorPicturesSectionPictureControl);
            //Assert.IsTrue(
            //    GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPicture, "src") == imgInt1Link,
            //    "Pictures > Exterior pictures > picture set should not change after drag-and-drop " +
            //    "from Interior Pictures section");
            //Assert.IsTrue(
            //    GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPictureLast, "src") == imgInt2Link,
            //    "Pictures > Exterior pictures > picture set should not change after drag-and-drop " +
            //    "from Interior Pictures section");

            Click(ItemsPage.PicturesSectionExteriorPicturesSectionDoneButton);
            Assert.IsTrue(
                IsElementNotFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSectionPictureControlHighlighted),
                "Pictures > Exterior pictures > both picture controls should be not highlighted on Done press");
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPicture, "src") == imgExt2Link,
                "Pictures > Exterior pictures > pictures should keep their places after Done press");
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPictureLast, "src") == imgExt1Link,
                "Pictures > Exterior pictures > pictures should keep their places after Done press");

            Click(ItemsPage.PicturesSectionInteriorPicturesSectionPicture);
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Pictures > Interior pictures > Media library should not open by click on asset while reordering");
            Click(ItemsPage.PicturesSectionInteriorPicturesSectionUploadButton);
            Assert.IsTrue(!FileManager.IsWindowOpen(FileManager.WindowTitle),
                "Pictures > Interior pictures > Open file window should not open by click on Upload while reordering");
            Click(ItemsPage.PicturesSectionInteriorPicturesSectionDeleteFirstButton);
            Assert.IsTrue(CountElements(
                    ItemsPage.PicturesSectionInteriorPicturesSectionPictureControl) == 2,
                "Pictures > Interior pictures > Pictures should not be deleted by click on Delete while reordering");

            var imgInt1Link = GetElementAttribute(ItemsPage.PicturesSectionInteriorPicturesSectionPicture, "src");
            DragAndDropHtml5(ItemsPage.PicturesSectionInteriorPicturesSectionPictureControlLast,
                ItemsPage.PicturesSectionInteriorPicturesSectionPictureControl);
            Click(ItemsPage.PicturesSectionInteriorPicturesSectionDoneButton);
            Click(ItemsPage.PicturesSectionInteriorPicturesSectionDeleteFirstButton);
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionInteriorPicturesSectionPicture, "src") == imgInt1Link,
                "Pictures > Interior pictures > wrong picture currently displayed after first picture delete");
            Assert.IsTrue(CountElements(
                    ItemsPage.PicturesSectionInteriorPicturesSectionPictureControl) == 1,
                "Pictures > Interior pictures > only one picture should be displayed after delete");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.PicturesSectionInteriorPicturesSectionChangeOrderButton),
                "Pictures > Interior pictures > Change Order button should be hidden when 1 picture is left");

            ClickUntilShown(ItemsPage.PicturesSectionInteriorPicturesSectionAddButton,
                ItemsPage.PicturesSectionInteriorPicturesSectionPictureEmpty);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionInteriorPicturesSectionPictureEmpty),
                "Pictures > Interior pictures > empty picture control has appeared on +Add press");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionInteriorPicturesSectionChangeOrderButton),
                "Pictures > Interior pictures > Change Order button should be shown when 2nd picture controls is present");

            ClickUntilConditionMet(ItemsPage.PicturesSectionInteriorPicturesSectionDeleteLastButton,
                () => IsElementNotFoundQuickly(ItemsPage.PicturesSectionInteriorPicturesSectionPictureEmpty));
            SubmitForm();
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPicture, "src") == imgExt2Link,
                "Pictures > Exterior pictures > wrong picture on 1st place after Submit press");
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPictureLast, "src") == imgExt1Link,
                "Pictures > Exterior pictures > wrong picture on 2nd place after Submit press");
            Assert.IsTrue(CountElements(
                    ItemsPage.PicturesSectionInteriorPicturesSectionPictureControl) == 1,
                "Pictures > Interior pictures > only one picture should be displayed after Submit press");
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionInteriorPicturesSectionPicture, "src") == imgInt1Link,
                "Pictures > Interior pictures > wrong picture on 1st place after Submit press");
            Assert.IsTrue(GetElementAttribute(ItemsPage.Image, "src") == imgExt2Link,
                "1st picture from Pictures > Exterior pictures section should become the main item picture");
        }

        [Test, Regression]
        public void RT13080_LanguagesSectionsOrdering()
        {
            CurrentTenant = TenantTitle.reordering2;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            var item = AddItem(ItemType.PorscheCar, isAddNew: true);
            TestStart();
            PrepareItem(item);
            PreparePictures();

            Click(ItemsPage.SerialDataSectionChangeOrderButton);
            DragAndDropHtml5(ItemsPage.SerialDataSectionRow1, ItemsPage.SerialDataSectionRow3);
            Click(ItemsPage.SerialDataSectionDoneButton);
            Click(ItemsPage.SerialDataSectionRow2);
            Click(ItemsPage.SerialDataSectionRow2DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionRow3),
                "Serial Data > row 3 should be deleted");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 2",
                @"Serial Data > row 1 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 1",
                @"Serial Data > row 2 should contain text 'Serial 1'");

            Click(ItemsPage.PicturesSectionExteriorPicturesSectionChangeOrderButton);
            DragAndDropHtml5(ItemsPage.PicturesSectionExteriorPicturesSectionPictureControlLast,
                ItemsPage.PicturesSectionExteriorPicturesSectionPictureControl);
            var imgExt1Link = GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPicture, "src");
            var imgExt2Link = GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPictureLast, "src");
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionDoneButton);
            Click(PageFooter.PropertiesOrderButton);
            Assert.IsTrue(IsElementFound(ItemsPage.ChangeOrderDialog),
                "Change Order dialog should be shown on Properties Order button press on page footer");
            DragAndDropHtml5(string.Format(ItemsPage.TablePropertyByText, "Pictures"),
                string.Format(ItemsPage.TablePropertyByText, "Technical Data"), "data-lev");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Pictures"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Technical Data"), "data-idx")) < 0,
                @"'Pictures' property should be shown above 'Technical Data' item in change order of properties list");
            Assert.IsTrue(
                string.CompareOrdinal(
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Pictures"), "data-idx"),
                    GetElementAttribute(string.Format(ItemsPage.TablePropertyByText, "Serial Data"), "data-idx")) > 0,
                @"'Pictures' property should be shown below 'Serial Data' item in change order of properties list");
            Click(ItemsPage.DoneButton);

            ClickUntilShown(ItemsPage.LanguageAddButton, ItemsPage.LanguageDeButtonActiveInMenu);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            ClickUntilShown(ItemsPage.PicturesButton, ItemsPage.PicturesSectionExteriorPicturesSection);
            var serDataSectionLocation = GetElementLocation(ItemsPage.SerialDataSectionHeader, true);
            var picturesSectionLocation = GetElementLocation(ItemsPage.PicturesSectionExteriorPicturesSection, true);
            var techDataSectionLocation = GetElementLocation(ItemsPage.TechnicalDataSectionRow1, true);
            Assert.IsTrue(serDataSectionLocation.Y < picturesSectionLocation.Y,
                "Lang DE: Pictures section should be located below Serial Data section");
            Assert.IsTrue(picturesSectionLocation.Y < techDataSectionLocation.Y,
                "Lang DE: Technical Data section should be located below Pictures section");
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPicture, "src") == imgExt1Link,
                "Lang DE: Pictures > Exterior pictures > pictures should have the same places");
            Assert.IsTrue(
                GetElementAttribute(ItemsPage.PicturesSectionExteriorPicturesSectionPictureLast, "src") == imgExt2Link,
                "Lang DE: Pictures > Exterior pictures > pictures should have the same places");

            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.SerialDataSectionRow3),
                "Lang DE: Serial Data > row 3 should be deleted");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 2",
                @"Lang DE: Serial Data > row 1 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 1",
                @"Lang DE: Serial Data > row 2 should contain text 'Serial 1'");

            Click(ItemsPage.SerialDataSectionChangeOrderButton); // Lang DE
            DragAndDropHtml5(ItemsPage.SerialDataSectionRow1, ItemsPage.SerialDataSectionRow2);
            Click(ItemsPage.SerialDataSectionDoneButton);
            SubmitForm();
            RefreshPage();
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 2",
                @"Serial Data > row 1 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 1",
                @"Serial Data > row 2 should contain text 'Serial 1'");

            ClickUntilShown(ItemsPage.LanguageDeButton, ItemsPage.LanguageDeButtonActive);
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 1",
                @"Lang DE: Serial Data > row 1 should contain text 'Serial 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 2",
                @"Lang DE: Serial Data > row 2 should contain text 'Serial 2'");

            Click(PageFooter.DuplicateButton);
            ClickUntilShown(ItemsPage.LanguageEnButton, ItemsPage.LanguageEnButtonActive);
            SendText(ItemsPage.Vin, $"{RandomNumber}77");
            ClickUntilShown(ItemsPage.PicturesButton,
                ItemsPage.PicturesSectionExteriorPicturesSection);
            serDataSectionLocation = GetElementLocation(ItemsPage.SerialDataSectionHeader, true);
            picturesSectionLocation = GetElementLocation(ItemsPage.PicturesSectionExteriorPicturesSection, true);
            techDataSectionLocation = GetElementLocation(ItemsPage.TechnicalDataSectionRow1, true);
            Assert.IsTrue(serDataSectionLocation.Y < picturesSectionLocation.Y,
                "Pictures section should be located below Serial Data section");
            Assert.IsTrue(picturesSectionLocation.Y < techDataSectionLocation.Y,
                "Technical Data section should be located below Pictures section");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 2",
                @"Serial Data > row 1 should contain text 'Serial 2'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 1",
                @"Serial Data > row 2 should contain text 'Serial 1'");
            ClickUntilShown(ItemsPage.LanguageDeButton, ItemsPage.LanguageDeButtonActive);
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow1) == "1Serial 1",
                @"Lang DE: Serial Data > row 1 should contain text 'Serial 1'");
            Assert.IsTrue(GetValuesAsString(ItemsPage.SerialDataSectionRow2) == "2Serial 2",
                @"Lang DE: Serial Data > row 2 should contain text 'Serial 2'");
        }

        [Test, Regression]
        public void RT13090_ReorderingInSimpleArray()
        {
            CurrentTenant = TenantTitle.reordering2;
            var app = AddAppIbeacon(TestConfig.IbeaconAppVersions[2]);
            TestStart();

            OpenEntityPage(app);
            EditForm();
            ClickUntilShown(AppsPage.InformationButton, AppsPage.SalutationProfile);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.InformationSectionAddressesChangeOrderButton),
                "Information > Addresses > Change Order button should be hidden");
            
            Click(AppsPage.InformationSectionAddressesAddButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.InformationSectionAddressesChangeOrderButton),
                "Information > Addresses > Change Order button should be shown");

            SendText(AppsPage.InformationSectionAddressesRow2Title, RandomNumber);
            SendText(AppsPage.InformationSectionAddressesRow2Address, RandomNumber);
            Click(AppsPage.InformationSectionAddressesChangeOrderButton);
            var row1 = GetValue(AppsPage.InformationSectionAddressesRow1, waitForValue: true).Substring(1);
            var row2 = GetValue(AppsPage.InformationSectionAddressesRow2).Substring(1);
            DragAndDropHtml5(AppsPage.InformationSectionAddressesRow2, AppsPage.InformationSectionAddressesRow1);
            Click(AppsPage.InformationSectionAddressesDoneButton);
            Assert.IsTrue(GetValue(AppsPage.InformationSectionAddressesRow1).Substring(1).Equals(row2),
                @"Information > Addresses > row 1 should equal previous row 2 content after drag and drop");
            Assert.IsTrue(GetValue(AppsPage.InformationSectionAddressesRow2).Substring(1).Equals(row1),
                @"Information > Addresses > row 2 should equal previous row 1 content after drag and drop");

            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            ClickUntilShown(AppsPage.EmailsButton, AppsPage.WelcomeNewCustomer);
            ClickUntilShown(AppsPage.ServiceBookingRecipientAddButton, AppsPage.ServiceBookingRecipientEmail);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ServiceBookingRecipientChangeOrderButton),
                "Texts > Emails > Service booking recipients > Change Order button should be hidden");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ServiceBookingRecipientAddButton),
                "Texts > Emails > Service booking recipients > +Add button should be shown");

            ClickUntilConditionMet(AppsPage.ServiceBookingRecipientAddButton, 
                () => CountElements(AppsPage.ServiceBookingRecipientEmail) == 2);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ServiceBookingRecipientChangeOrderButton),
                "Texts > Emails > Service booking recipients > Change Order button should be shown");

            SendText(AppsPage.ServiceBookingRecipientEmail, "aa@bb.cc");
            ClickUntilShown(AppsPage.ServiceBookingRecipientChangeOrderButton, 
                AppsPage.ServiceBookingRecipientDoneButton);
            SendText(AppsPage.ServiceBookingRecipientEmail, "xx@bb.cc", true);
            SendText(AppsPage.ServiceBookingRecipientEmailLast, "xx@bb.cc", true);
            Assert.IsFalse(IsAnyElementEquals(AppsPage.ServiceBookingRecipientEmail, "xx@bb.cc"),
                "Texts > Emails > Service booking recipients > all email fields should be read-only " +
                "during drag-and-drop");

            Click(AppsPage.ServiceBookingRecipientCancelButton);
            SendText(AppsPage.ServiceBookingRecipientEmailLast, "xx@bb.cc");
            Click(AppsPage.ServiceBookingRecipientChangeOrderButton);
            DragAndDropHtml5(AppsPage.ServiceBookingRecipientEmailLast, AppsPage.ServiceBookingRecipientEmail);
            Click(AppsPage.ServiceBookingRecipientDoneButton);
            Assert.IsTrue(GetValue(AppsPage.ServiceBookingRecipientEmail) == "xx@bb.cc" 
                    && GetValue(AppsPage.ServiceBookingRecipientEmailLast) == "aa@bb.cc",
                "Texts > Emails > Service booking recipients > 2nd and 1st email fields should swap after " +
                "drag-and-drop");

            // DE
            Click(AppsPage.LanguageDeButton); 
            ClickUntilShown(AppsPage.InformationButton, AppsPage.SalutationProfile);
            var row1De = GetValue(AppsPage.InformationSectionAddressesRow1).Substring(1);
            Assert.IsTrue(!row1De.Equals(row1) && !row1De.Equals(row2),
                "Lang DE: Information > Addresses > row 1 should stay unchanged");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.InformationSectionAddressesRow2),
                "Lang DE: Information > Addresses > row 2 should be absent");
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            ClickUntilShown(AppsPage.EmailsButton, AppsPage.WelcomeNewCustomer);
            Assert.IsTrue(GetValue(AppsPage.ServiceBookingRecipientEmailReadOnly) == "xx@bb.cc"
                           && GetValue(AppsPage.ServiceBookingRecipientEmailReadOnlyLast) == "aa@bb.cc",
                "Lang DE: Texts > Emails > Service booking recipients > 2nd and 1st email fields should " +
                "be shown swapped after drag-and-drop in lang EN");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ServiceBookingRecipientChangeOrderButton),
                "Texts > Emails > Service booking recipients > Change Order button should be hidden");

            SubmitForm();
            // EN
            ClickUntilShown(AppsPage.InformationButton, AppsPage.SalutationProfile);
            Assert.IsTrue(GetValue(AppsPage.InformationSectionAddressesRow1).Substring(1).Equals(row2),
                @"Information > Addresses > row 1 should equal previous row 2 content after submit");
            Assert.IsTrue(GetValue(AppsPage.InformationSectionAddressesRow2).Substring(1).Equals(row1),
                @"Information > Addresses > row 2 should equal previous row 1 content after submit");
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            ClickUntilShown(AppsPage.EmailsButton, AppsPage.WelcomeNewCustomerReadOnly);
            Assert.IsTrue(GetValue(AppsPage.ServiceBookingRecipientEmailReadOnly) == "xx@bb.cc"
                           && GetValue(AppsPage.ServiceBookingRecipientEmailReadOnlyLast) == "aa@bb.cc",
                "Texts > Emails > Service booking recipients > 2nd and 1st email fields should swap after " +
                "drag-and-drop");
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
