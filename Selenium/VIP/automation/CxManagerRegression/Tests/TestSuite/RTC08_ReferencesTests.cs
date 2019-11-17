using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.Items;
using Models.Places;
using NUnit.Framework;
using OpenQA.Selenium;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC08_ReferencesTests : ParentTest
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
            CurrentTenant = TenantTitle.manylang;
            CurrentUser = TestConfig.AdminUser;
        }

        [Test, Regression]
        public void RT08010_AssignItemToPlace()
        {
            Place place1 = null, place2 = null, place3 = null, rootPlace = null;
            Item item = null;
            UserDirectoryApi.AssignRolesToUser(TestConfig.AdminUser, new [] { UserRole.CxmAdmin });
            Parallel.Invoke(
                () => AddAppIbeacon(TestConfig.IbeaconAppVersions[1], true),
                () => place1 = AddPlaceIbeacon(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => place2 = AddPlaceIbeacon(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => place3 = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => rootPlace = AddPlaceNoType(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => item = AddItem(ItemType.PorscheCar, isAddNew: true)
            );
            TestStart();

            OpenEntityPage(item);
            EditForm();
            ClickUntilShown(ItemsPage.AssignedPlacesAddButton, ItemsPage.AssignedPlacesPlaceDropDown);
            ClickUntilShown(ItemsPage.AssignedPlacesPlaceDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(CommonElement.DropDownOption, place1.Title)),
                $"Assigned places: dropdown should contain place {place1.Title}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(CommonElement.DropDownOption, place2.Title)),
                $"Assigned places: dropdown should contain place {place2.Title}");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(CommonElement.DropDownOption, place3.Title)),
                $"Assigned places: dropdown should not contain WW place {place3.Title}");
            if (rootPlace != null)
            {
                Assert.IsTrue(IsElementNotFoundQuickly(string.Format(CommonElement.DropDownOption, rootPlace.Title)),
                    $"Assigned places: dropdown should not contain root place {rootPlace.Title}");
            }

            DropDownSelect(ItemsPage.AssignedPlacesPlaceDropDown, place1.Title);
            SubmitForm();
            Assert.IsTrue(AreElementsContainText(ItemsPage.ReferencesSectionPlaces, place1.Title), 
                $"References: Field Places should contain place {place1.Title}");

            Click(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place1.Title));
            Assert.IsTrue(IsPageContainsUri($"{TestConfig.PlaceUri}/{place1.Id}"),
                $"Place {place1.Title} page should be opened");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.ItemsSectionItemByTitle, item.LangJsonData.EnJson.Title)),
                $"Items section: place should contain item {item.LangJsonData.EnJson.Title}");

            Click(string.Format(PlacesPage.ItemsSectionItemByTitle, item.LangJsonData.EnJson.Title));
            Assert.IsTrue(IsViewMode(), "Place should stay in view mode");

            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteButton);
            OpenEntityPage(item);
            Assert.IsTrue(AreElementsContainText(ItemsPage.ReferencesSectionPlaces, place1.Title),
                $"References: Field Places should contain place {place1.Title}");

            Click(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place1.Title));
            Click(PageFooter.RestoreButton);
            SubmitForm();
            OpenEntityPage(item);
            Assert.IsTrue(AreElementsContainText(ItemsPage.ReferencesSectionPlaces, place1.Title),
                $"References: Field Places should contain place {place1.Title}");

            EditForm();
            ClickUntilConditionMet(ItemsPage.AssignedPlacesAddButton,
                () => CountElements(ItemsPage.AssignedPlacesPlaceDropDown) == 2);
            ClickUntilShown(ItemsPage.AssignedPlacesPlaceLastDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(AreElementsContainText(CommonElement.DropDownOptionList, place2.Title),
                $"Assigned places: last dropdown should contain place {place2.Title}");

            DropDownSelect(ItemsPage.AssignedPlacesPlaceLastDropDown, place2.Title);
            ClickUntilConditionMet(ItemsPage.AssignedPlacesAddButton, 
                () => CountElements(ItemsPage.AssignedPlacesPlaceDropDown) == 3);
            Click(ItemsPage.AssignedPlacesPlaceLastDropDown);
            Assert.IsTrue(IsElementNotFoundQuickly(CommonElement.DropDownOptionList) 
                || (!AreElementsContainText(CommonElement.DropDownOptionList, place2.Title) 
                           && !AreElementsContainText(CommonElement.DropDownOptionList, place1.Title)),
                $"Assigned places: last dropdown should not contain places {place2.Title} and {place1.Title}");

            Click(ItemsPage.AssignedPlacesDeleteLastButton);
            SubmitForm();
            Assert.IsTrue(AreElementsContainText(ItemsPage.ReferencesSectionPlaces, place1.Title),
                $"References: Field Places should contain place {place1.Title}");
            Assert.IsTrue(AreElementsContainText(ItemsPage.ReferencesSectionPlaces, place2.Title),
                $"References: Field Places should contain place {place2.Title}");

            EditForm();
            Click(ItemsPage.AssignedPlacesDeleteButton);
            SubmitForm();
            Assert.IsTrue(!AreElementsContainText(ItemsPage.ReferencesSectionPlaces, place1.Title),
                $"References: Field Places should not contain place {place1.Title}");
            Assert.IsTrue(AreElementsContainText(ItemsPage.ReferencesSectionPlaces, place2.Title),
                $"References: Field Places should contain place {place2.Title}");

            Click(PageFooter.DeleteButton);
            Assert.IsTrue(IsElementFound(ItemsPage.ItemIsReferencedDialog),
                $@"'This item is referenced in {place2.Title}...' dialog should be displayed");

            Click(ItemsPage.OkButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.ItemIsReferencedDialog),
                $@"'This item is referenced in {place2.Title}...' dialog should be closed");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.RestoreButton),
                "Item should not be deleted");

            Click(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place2.Title));
            EditForm();
            var itemTitle = item.LangJsonData.EnJson.Title;
            Assert.IsTrue(IsElementFoundQuickly(
                    string.Format(PlacesPage.ItemsSectionItemByTitleUnassignButton, itemTitle)),
                @"Items section: All items should have text 'Unassign'");

            Click(string.Format(PlacesPage.ItemsSectionItemByTitleUnassignButton, itemTitle));
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DoYouWantToUnassignDialog),
                @"Dialog 'Do you really want to unassign this item?' should be displayed");

            Click(PlacesPage.UnassignButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DoYouWantToUnassignDialog),
                @"Dialog 'Do you really want to unassign this item?' should be closed");
            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(PlacesPage.ItemsSectionItemByTitleUnassignButton, place2.Title)),
                $"Items section: item {itemTitle} should be unassigned (absent)");
            Assert.IsTrue(IsEditMode(), "Place should stay in edit mode");

            SubmitForm();
            OpenEntityPage(item);
            Assert.IsTrue(IsElementNotFound(ItemsPage.ReferencesSectionPlaces),
                "Reference section should be absent");

            Click(PageFooter.DeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.DeleteConfirmationDialog),
                "Item delete confirmation dialog should be displayed");

            Click(ItemsPage.DeleteButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.ItemsUri), "Page Items should be opened");
        }

        [Test, Regression]
        public void RT08020_ItemReferenceToIbeaconApp1()
        {
            Item item1 = null, item2 = null, item3 = null;
            var app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1], true);
            Parallel.Invoke(
                () => item1 = AddItem(ItemType.PorscheCar, isAddNew: true),
                () => item2 = AddItem(ItemType.EmailTemplate, isAddNew: true),
                () => item3 = AddItem(ItemType.EmailTemplate, isAddNew: true),
                () => ItemApi.DeleteItems(ItemType.ServiceBooking)
            );
            TestStart();

            OpenEntityPage(item2);
            Assert.IsTrue(IsElementNotFound(ItemsPage.ReferencesSectionPlaces),
                "Reference section in item should be absent by default");

            OpenEntityPage(app);
            EditForm();
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.WelcomeNewCustomerDetailsButton);
            Assert.IsTrue(AreAllElementsContainText(ItemsPage.TableRowStatus, StatusActive),
                $@"All items in 'Select item' dialog should be {StatusActive}");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(ItemsPage.TableRowByTitle, item1.LangJsonData.EnJson.Title)),
                $@"All items in 'Select item' dialog should be {ItemTypeEmailTemplate} type");

            Click(AppsPage.CancelButton);
            ClickUntilShown(AppsPage.WelcomeNewCustomer, CommonElement.DropDownInput);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.WelcomeNewCustomerDetailsButton),
                @"Field 'Welcome new customer' should have details button");

            SendText(CommonElement.DropDownInput, "Porsche");
            Assert.IsTrue(IsElementNotFound(CommonElement.DropDownOptionList),
                @"Dropdown 'Welcome new customer' is not empty");

            SendText(CommonElement.DropDownInput, "Auto");
            Assert.IsTrue(IsElementFound(string.Format(CommonElement.DropDownOption, "Auto")),
                @"Field 'Welcome new customer' should show dropdown items");

            Click(string.Format(CommonElement.DropDownOption, item2.JsonDataTitle), noScroll: true);
            Assert.IsTrue(AreElementsContainText(AppsPage.WelcomeNewCustomer, item2.JsonDataTitle),
                $@"Field 'Welcome new customer' should contain {item2.JsonDataTitle}");

            ClickUntilShown(AppsPage.WelcomeNewCustomer, CommonElement.DropDownInput);
            SendText(CommonElement.DropDownInput, string.Empty);
            SubmitForm();
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Assert.IsTrue(AreElementsContainText(AppsPage.WelcomeNewCustomerReadOnly, item2.JsonDataTitle),
                $@"Field 'Welcome new customer' should contain {item2.JsonDataTitle}");

            OpenEntityPage(item2);
            Assert.IsTrue(IsElementFound(
                    string.Format(ItemsPage.ReferencesSectionPlaceByTitle, AppTitle.Ibeacon)),
                $"References: Apps field should contain {AppTitle.Ibeacon} app");

            EditForm();
            Click(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, AppTitle.Ibeacon));
            Assert.IsTrue(IsPageContainsUri($"{TestConfig.AppUri}/{app.AppId}"),
                $"User should be redirected to app {AppTitle.Ibeacon} page");

            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.WelcomeNewCustomerDetailsButton),
                @"Field 'Welcome new customer' still has details button");
            
            EditForm();
            Click(AppsPage.ProfileReportTemplateDropDownDetailsButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TableRowByTitle, item2.JsonDataTitle)),
                $@"Item {item2.JsonDataTitle} should be shown in 'Select item' dialog");

            Click(string.Format(ItemsPage.TableRowByTitle, item2.JsonDataTitle));
            Assert.IsTrue(AreElementsContainText(AppsPage.ProfileReportTemplateDropDown, item2.JsonDataTitle),
                $"Field 'Profile report template' should contain {item2.JsonDataTitle}");

            SubmitForm();
            OpenEntityPage(item2);
            Assert.IsTrue(CountElements(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, AppTitle.Ibeacon)) == 1,
                $"References: Apps field should contain 1 app {AppTitle.Ibeacon} record");

            Click(PageFooter.DeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ItemIsReferencedDialog), 
                @"Dialog 'item is referenced' should be displayed");

            Click(ItemsPage.OkButton);
            Click(PageFooter.DuplicateButton);
            SendText(ItemsPage.Title, ItemTypeWelcomeEmailTemplate + " 2");
            SendText(ItemsPage.Key, RandomNumber);
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(ItemsPage.ReferencesSectionPlaces),
                "There should be no References section for duplicated item");

            OpenEntityPage(app);
            EditForm();
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.WelcomeNewCustomerDetailsButton);
            Click(ItemsPage.ClearSelectionButton);
            Assert.IsTrue(IsElementEquals(AppsPage.WelcomeNewCustomer, string.Empty),
                @"Field 'Welcome new customer' should be empty");

            ClickUntilShown(AppsPage.ProfileReportTemplateDropDown, CommonElement.DropDownInput);
            SendText(CommonElement.DropDownInput, item3.JsonDataTitle);
            Assert.IsTrue(IsElementFound(string.Format(CommonElement.DropDownOption, item3.JsonDataTitle)),
                @"Field 'Profile report template' should show dropdown items");

            PressKeys(Keys.Enter);
            Assert.IsTrue(AreElementsContainText(AppsPage.ProfileReportTemplateDropDown, item3.JsonDataTitle),
                $@"Field 'Profile report template' should contain {item3.JsonDataTitle}");

            SubmitForm();
            EditForm();
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.WelcomeNewCustomerDetailsButton);
            PressKeys(Keys.Escape);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.SelectItemDialog),
                @"'Select item' dialog should be closed on Esc button press");
            Assert.IsTrue(IsEditMode(), "App page should be in edit mode");

            Click(PageFooter.CancelButton);
            OpenEntityPage(item2);
            Assert.IsTrue(IsElementNotFound(ItemsPage.ReferencesSectionPlaces),
                $"There should be no References section for item {item2.JsonDataTitle}");

            AppApi.DeleteApps(true, new [] { AppTitle.Ibeacon }, CurrentTenant);
            OpenEntityPage(item3);
            Assert.IsTrue(IsElementNotFound(ItemsPage.ReferencesSectionPlaces),
                $"There should be no References section for item {item3.JsonDataTitle}");

            Click(PageFooter.DeleteButton);
            Click(ItemsPage.DeleteButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.ItemsUri),
                $"Item {item3.JsonDataTitle} cannot be deleted");
        }

        [Test, Regression]
        public void RT08030_ItemReferenceToIbeaconApp2()
        {
            Place place1 = null, place2 = null, place3 = null;
            Item item1 = null, item2 = null, item3 = null;
            Parallel.Invoke(
                () => place1 = AddPlaceIbeacon(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => place2 = AddPlaceIbeacon(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => place3 = AddPlaceIbeacon(PlaceStatus.Deleted, pageToBeOpened: 0),
                () => AddAppIbeacon(TestConfig.IbeaconAppVersions[1], true),
                () => ItemApi.DeleteItems(ItemType.Poi, CurrentTenant)
            );
            Parallel.Invoke(
                () => item1 = AddItem(ItemType.PorscheCar, isAddNew: true),
                () => item2 = AddItem(ItemType.UsedCar, isAssignImage: true, isAddNew: true),
                () => AddItem(ItemType.EmailTemplate, isAddNew: true),
                () => item3 = AddItem(ItemType.Poi)
            );
            ItemApi.DeleteItem(item3.Id);
            TestStart();

            OpenEntityPage(place1);
            EditForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(AppsPage.TableCellIbeaconApp);
            ClickUntilShown(PlacesPage.AppsSectionTableRowDetailsPoiDropDown, CommonElement.DropDownInput);
            SendText(CommonElement.DropDownInput, item3.LangJsonData.EnJson.Title);
            Assert.IsTrue(IsElementNotFoundQuickly(CommonElement.DropDownOptionList),
                $@"Apps: Deleted item '{item3.LangJsonData.EnJson.Title}' is displayed in POI field dropdown list");

            Click(PlacesPage.Title);
            Click(PlacesPage.AppsSectionTableRowDetailsPoiDetailsButton);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.NoItemsToBeAddedDialog),
                @"Dialog 'No items to be added' should be displayed");

            Click(PlacesPage.OkButton);
            Click(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton);
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByTitle, item1.LangJsonData.EnJson.Title))
                          && IsElementFoundQuickly(string.Format(ItemsPage.TableRowByTitle, item2.LangJsonData.EnJson.Title)),
                $"Items {item1.LangJsonData.EnJson.Title} and {item2.LangJsonData.EnJson.Title} should be in list");

            Click(string.Format(ItemsPage.TableRowByTitle, item2.LangJsonData.EnJson.Title));
            
            SubmitForm();
            OpenEntityPage(item2);
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place1.Title)),
                $"References: Field Places should contain place {place1.Title}");
            
            Assert.IsTrue(IsElementNotFound(ItemsPage.AssignedPlacesPlaceReadOnly),
                "Assigned places: section should be empty");

            EditForm();
            ClickUntilShown(ItemsPage.AssignedPlacesAddButton, ItemsPage.AssignedPlacesPlaceDropDown);
            ClickUntilShown(ItemsPage.AssignedPlacesPlaceDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(CommonElement.DropDownOption, place1.Title)),
                $"Place {place1.Title} should not be available in dropdown");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(CommonElement.DropDownOption, place2.Title)),
                $"Place {place2.Title} should be available in dropdown");

            DropDownSelect(ItemsPage.AssignedPlacesPlaceDropDown, place2.Title);

            Click(ItemsPage.PicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionUploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            SubmitForm();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place1.Title))
                && IsElementFoundQuickly(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place2.Title)),
                $"References: Field Places should contain places {place1.Title} and {place2.Title}");

            Click(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place1.Title));
            EditForm();
            Click(PlacesPage.AppsSectionTableRow1);
            Click(PlacesPage.AppsSectionTableRow1DeleteButton);
            SubmitForm();
            OpenEntityPage(item2);
            Assert.IsTrue(IsElementNotFound(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place1.Title)),
                $"References: Field Places should not contain place {place1.Title}");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place3.Title)),
                $"References: Field Places should not contain place {place3.Title}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.ReferencesSectionPlaceByTitle, place2.Title)),
                $"References: Field Places should contain place {place2.Title}");

            OpenEntityPage(item1);
            EditForm();
            ClickUntilShown(ItemsPage.AssignedPlacesAddButton, ItemsPage.AssignedPlacesPlaceDropDown);
            ClickUntilShown(ItemsPage.AssignedPlacesPlaceDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(IsElementFound(string.Format(CommonElement.DropDownOption, place1.Title))
                          && IsElementFoundQuickly(string.Format(CommonElement.DropDownOption, place2.Title)),
                $"Assigned places: Place dropdown should contain places {place1.Title} and {place2.Title}");

            DropDownSelect(ItemsPage.AssignedPlacesPlaceDropDown, place2.Title);
            SubmitForm();
            OpenEntityPage(place2);
            Assert.IsTrue(
                IsElementFound(string.Format(PlacesPage.ItemsSectionItemByTitle, item1.LangJsonData.EnJson.Title)),
                $"Items: Should contain item {item1.LangJsonData.EnJson.Title}");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(PlacesPage.ItemsSectionItemByTitle, item2.LangJsonData.EnJson.Title)),
                $"Items: Should contain item {item2.LangJsonData.EnJson.Title}");

            EditForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(AppsPage.TableRowIbeaconApp);
            ClickUntilShown(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton, PlacesPage.SelectItemDialog);
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(ItemsPage.TableRowByTitle, item1.LangJsonData.EnJson.Title)),
                $"Item list: Should contain item {item1.LangJsonData.EnJson.Title}");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(ItemsPage.TableRowByTitle, item2.LangJsonData.EnJson.Title)),
                $"Item list: Should contain item {item2.LangJsonData.EnJson.Title}");

            Click(string.Format(ItemsPage.TableRowByTitle, item1.LangJsonData.EnJson.Title));
            SubmitForm();
            Click(PageFooter.DuplicateButton);
            var title = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Title, title);
            SubmitForm();
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(PlacesPage.ItemsSectionItemByTitle, item1.LangJsonData.EnJson.Title)),
                $"Items: Should contain item {item1.LangJsonData.EnJson.Title}");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(PlacesPage.ItemsSectionItemByTitle, item2.LangJsonData.EnJson.Title)),
                $"Items: Should contain item {item2.LangJsonData.EnJson.Title}");
            Click(PlacesPage.AppsSectionTableRow1);
            Assert.IsTrue(
                AreElementsContainText(PlacesPage.AppsSectionTableRowDetailsCarReadOnly, item1.LangJsonData.EnJson.Title),
                $"Apps section: Car field should contain item {item1.LangJsonData.EnJson.Title}");

            OpenEntityPage(item2);
            Assert.IsTrue(AreElementsContainText(ItemsPage.AssignedPlacesPlaceReadOnly, place2.Title)
                          && AreElementsContainText(ItemsPage.AssignedPlacesPlaceReadOnly, title),
                $"Assigned places: should contain both {place2.Title} and its duplicate {title}");
            Assert.IsTrue(AreElementsContainText(ItemsPage.ReferencesSectionPlaces, place2.Title)
                          && AreElementsContainText(ItemsPage.ReferencesSectionPlaces, title),
                $"References: Field Places should contain both {place2.Title} and its duplicate {title}");
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
