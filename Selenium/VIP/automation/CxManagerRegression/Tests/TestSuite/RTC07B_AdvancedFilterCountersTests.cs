using System;
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
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC07B_AdvancedFilterCountersTests : ParentTest
    {      
        private const string ConditionBefore = "Before";
        private const string ConditionEqual = "Equal";
        private const string ConditionBetween = "Between";
        private const string ConditionAfter = "After";

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = true;
            CurrentTenant = TenantTitle.advfilter;
            CurrentUser = TestConfig.AdminUser;
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }
        }

        private string ConvertDateUtc(DateTime datetime)
        {
            return datetime.Date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        [Test, Regression]
        public void RT07400_FilterBasics()
        {
            TestStart();
            var item3 = AddItem(ItemType.Car);

            OpenItemsPage();
            Assert.IsTrue(IsElementFound(PageHeader.AdvancedFilterButton),
                "Advanced filter button should be displayed within Filter block");

            Click(PageHeader.AdvancedFilterButton);
            Assert.IsTrue(IsElementFound(AdvancedFilter.CreatedDropDown), 
                "Advanced filter: modal dialog should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.ModifiedDropDown),
                "Advanced filter: Modified dropdown should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.StatusDropDown),
                "Advanced filter: Status dropdown should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.CloseButton),
                "Advanced filter: Close button should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.ResetButton),
                "Advanced filter: Reset button should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.ModifiedDropDown),
                "Advanced filter: Apply button should be displayed");
            Assert.IsTrue(IsElementEquals(AdvancedFilter.CreatedDropDown, ConditionEqual),
                $"Created dropdown should have default value {ConditionEqual}");
            Assert.IsTrue(IsElementEquals(AdvancedFilter.ModifiedDropDown, ConditionEqual),
                $"Modified dropdown should have default value {ConditionEqual}");
            Assert.IsTrue(IsElementFound(AdvancedFilter.Created1),
                "Advanced filter: Created date picker field should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.Modified1),
                "Advanced filter: Modified date picker field should be displayed");
            Assert.IsTrue(IsElementNotFound(AdvancedFilter.Modified2) 
                    && IsElementNotFound(AdvancedFilter.Created2),
                @"Advanced filter: Created & Modified should have only 1 date picker field each");

            var conditions = new []
            {
                ConditionBefore, ConditionAfter, ConditionBetween, ConditionEqual
            };
            ClickUntilShown(AdvancedFilter.CreatedDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), conditions),
                "Created dropdown should contain options: " + string.Join(", ", conditions));
            
            ClickUntilShown(AdvancedFilter.ModifiedDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), conditions),
                "Modified dropdown should contain options: " + string.Join(", ", conditions));

            ClickUntilShown(AdvancedFilter.StatusDropDown, CommonElement.DropDownOptionList);
            conditions = new []
            {
                StatusNew, StatusActive, StatusRejected, StatusPendingApproval
            };
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), conditions),
                "Status dropdown should contain options: " + string.Join(", ", conditions));

            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionBetween);
            Assert.IsTrue(IsElementFound(AdvancedFilter.Created1)
                          && IsElementFound(AdvancedFilter.Created2),
                "Created should have 2 date picker fields");
            Assert.IsTrue(IsElementFound(AdvancedFilter.Modified1)
                          && IsElementNotFound(AdvancedFilter.Modified2),
                "Modified should have only 1 date picker field");

            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionEqual);
            Assert.IsTrue(IsElementFound(AdvancedFilter.Created1)
                          && IsElementNotFound(AdvancedFilter.Created2),
                "Created should have only 1 date picker field");
            Click(AdvancedFilter.Created1);
            Assert.IsTrue(IsElementFound(AdvancedFilter.DatePicker),
                "Click on Created date picker field should call calendar");
            Assert.IsTrue(
                IsElementFound(string.Format(AdvancedFilter.DatePickerDateSelectedByText, 
                        DateTime.Now.ToString("d MMMM yyyy"))),
                $"Calendar element should be opened and current date {DateTime.Now:d MMMM yyyy} is selected");

            Click(string.Format(AdvancedFilter.DatePickerDateByText, 
                (DateTime.Now + TimeSpan.FromDays(1)).ToString("d MMMM yyyy")), ignoreIfNoElement: true, 1, 
                noScroll: true);
            Assert.IsTrue(
                IsElementNotFound(string.Format(AdvancedFilter.DatePickerDateSelectedByText,
                        (DateTime.Now + TimeSpan.FromDays(1)).ToString("d MMMM yyyy"))),
                "Calendar should deny future dates selection");

            var item = ItemApi.GetItem(ItemType.EmailTemplate, ItemStatus.Active, hasImage: true);
            var createdDate = Convert.ToDateTime(item.CreateDate).ToString("d MMMM yyyy");
            Click(string.Format(AdvancedFilter.DatePickerDateByText, createdDate), noScroll: true);
            Click(AdvancedFilter.ApplyButton);
            Click(PageFooter.ShowDeletedButton, ignoreIfNoElement: true, 5);
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 6,
                "Only items imported from iBeacon app should be shown in Items list");
            Assert.IsTrue(
                IsElementNotFound(string.Format(ItemsPage.TableRowByText, item3.LangJsonData.EnJson.Title)),
                "Newly created item should be hidden");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.AdvancedFilterButtonActive),
                "Blue colored advance filter button should be shown in page header");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionBetween);
            var yesterday = DateTime.Now - TimeSpan.FromDays(1);
            SelectDate(AdvancedFilter.Created1, yesterday);
            SelectDate(AdvancedFilter.Created2, DateTime.Now);
            Assert.IsTrue(
                IsElementEquals(AdvancedFilter.Created1, yesterday.ToString("dd.MM.yyyy")),
                $"First date picker field should be equal {yesterday:dd.MM.yyyy}");
            Assert.IsTrue(
                IsElementEquals(AdvancedFilter.Created2, DateTime.Now.ToString("dd.MM.yyyy")),
                $"Second date picker field should be equal {DateTime.Now:dd.MM.yyyy}");

            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(
                IsElementFound(string.Format(ItemsPage.TableRowByText, item3.LangJsonData.EnJson.Title))
                    && CountElements(ItemsPage.TableRow) == 1,
                "Only newly created item should be shown in Items list");

            Click(PageHeader.AdvancedFilterButton);
            ClearTextInElement(AdvancedFilter.Created1);
            ClearTextInElement(AdvancedFilter.Created2);
            Click(AdvancedFilter.CloseButton);
            Assert.IsTrue(IsElementNotFound(AdvancedFilter.CreatedDropDown),
                "Advanced filter dialog should be closed");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.AdvancedFilterButtonActive),
                "Blue colored advance filter button should be shown in page header");
            Assert.IsTrue(
                IsElementFound(string.Format(ItemsPage.TableRowByText, item3.LangJsonData.EnJson.Title))
                    && CountElements(ItemsPage.TableRow) == 1,
                "Only newly created item should be shown in Items list");

            Click(PageHeader.AdvancedFilterButton);
            Click(AdvancedFilter.ResetButton);
            Assert.IsTrue(IsElementNotFound(AdvancedFilter.CreatedDropDown),
                "Advanced filter dialog should be closed");
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.AdvancedFilterButtonActive),
                "Blue colored advance filter button should be not shown in page header after Reset");
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 7,
                "All items should be shown in Items list on filter Reset");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.StatusDropDown, StatusActive);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 6,
                $"Only active items should be shown in Items list on Status = {StatusActive}");
        }

        [Test, Regression]
        public void RT07410_ItemsFilter()
        {
            TestStart();
            var item3 = AddItem(ItemType.Car);
            var item1 = ItemApi.GetItem(ItemType.EmailTemplate, ItemStatus.Active, 
                hasImage: true);
            item1 = ItemApi.SaveItem(item1);

            OpenItemsPage();
            Click(PageFooter.ShowDeletedButton, ignoreIfNoElement: true, 2);
            Click(PageHeader.AdvancedFilterButton);
            Click(AdvancedFilter.ResetButton);
            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionBetween);
            var yesterday = DateTime.Now - TimeSpan.FromDays(1);
            SelectDate(AdvancedFilter.Created2, yesterday);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(
                IsElementNotFound(string.Format(ItemsPage.TableRowByText, item3.LangJsonData.EnJson.Title))
                && CountElements(ItemsPage.TableRow) == 6,
                "Only items imported from iBeacon app should be shown in Items list");

            Click(PageHeader.AdvancedFilterButton);
            ClearTextInElement(AdvancedFilter.Created1);
            Click(AdvancedFilter.Created1);
            Click(string.Format(AdvancedFilter.DatePickerDateByText,
                    DateTime.Now.ToString("d MMMM yyyy")), ignoreIfNoElement: true, 1,
                noScroll: true);
            Assert.IsTrue(IsElementEquals(AdvancedFilter.Created1, string.Empty),
                @"Calendar should deny 'created from' later than 'created to'");

            ClearTextInElement(AdvancedFilter.Created2);
            SelectDate(AdvancedFilter.Created1, yesterday);
            Click(AdvancedFilter.Created2);
            Click(string.Format(AdvancedFilter.DatePickerDateByText,
                (DateTime.Now - TimeSpan.FromDays(2)).ToString("d MMMM yyyy")), noScroll: true);
            Assert.IsTrue(IsElementEquals(AdvancedFilter.Created2, string.Empty),
                @"Calendar should deny 'created to' earlier than 'created from'");

            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionEqual);
            var createdDate = Convert.ToDateTime(item1.CreateDate);
            var modifiedDate = Convert.ToDateTime(item1.UpdateDate);
            SelectDate(AdvancedFilter.Created1, createdDate);
            SelectDate(AdvancedFilter.Modified1, modifiedDate);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(
                IsElementFound(string.Format(ItemsPage.TableRowByText, item1.LangJsonData.EnJson.Title))
                    && CountElements(ItemsPage.TableRow) == 1,
                "Only just updated standard item should be shown in Items list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionEqual);
            SelectDate(AdvancedFilter.Created1, createdDate);
            DropDownSelect(AdvancedFilter.ModifiedDropDown, ConditionBetween);
            var dayBeforeCreatedDate = 
                Convert.ToDateTime(item1.CreateDate) - TimeSpan.FromDays(1);
            SelectDate(AdvancedFilter.Modified1, dayBeforeCreatedDate);
            SelectDate(AdvancedFilter.Modified2, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 6,
                "Only items imported from iBeacon app should be shown in Items list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionBetween);
            SelectDate(AdvancedFilter.Created1, dayBeforeCreatedDate);
            SelectDate(AdvancedFilter.Created2, DateTime.Now);
            DropDownSelect(AdvancedFilter.ModifiedDropDown, ConditionBetween);
            SelectDate(AdvancedFilter.Modified1, yesterday);
            SelectDate(AdvancedFilter.Modified2, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 2,
                "Only just modified imported item and just created item should be shown in Items list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionBefore);
            SelectDate(AdvancedFilter.Created1, yesterday);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 1,
                "Only just modified item imported from iBeacon app should be shown in Items list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionAfter);
            SelectDate(AdvancedFilter.Modified1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 1,
                "Only just created item should be shown in Items list");

            Click(PageHeader.AdvancedFilterButton);
            SelectDate(AdvancedFilter.Created1, yesterday);
            DropDownSelect(AdvancedFilter.ModifiedDropDown, ConditionBefore);
            SelectDate(AdvancedFilter.Modified1, yesterday);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(IsElementFound(ItemsPage.TableEmpty),
                "No items should be shown in Items list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionBefore);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 5,
                "Only unchanged items imported from iBeacon app should be shown in Items list");

            ChangeTenant(TenantTitle.manylang);
            OpenItemsPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set for this tenant");

            ChangeTenant(TenantTitle.advfilter);
            OpenItemsPage();
            Assert.IsTrue(IsElementFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be still set for this tenant");

            OpenPlacesPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set for Places list page");

            OpenAppsPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set for Apps list page");
        }

        [Test, Regression]
        public void RT07420_PlacesFilter()
        {
            TestStart();
            var place4 = AddPlaceNoType(PlaceStatus.Deleted, pageToBeOpened: 0);
            var place1 = PlaceApi.GetPlace(PlaceType.Ww, PlaceStatus.Any, hasDeviceAssigned: false);
            place1.Description = "some text";
            PlaceApi.SavePlace(place1);

            OpenPlacesPage();
            Click(PageFooter.ShowDeletedButton, ignoreIfNoElement: true, 2);
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Places list page advanced filter should be inactive");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 4, "All 4 places should be shown");

            SetFilter("iBeacon");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 2,
                @"All 2 iBeacon places should be shown by 'Windows Workstation' filter expression");

            Click(PageHeader.AdvancedFilterButton);
            Assert.IsTrue(IsElementFound(AdvancedFilter.CreatedDropDown),
                "Advanced filter: modal dialog should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.ModifiedDropDown),
                "Advanced filter: Modified dropdown should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.StatusDropDown),
                "Advanced filter: Status dropdown should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.CloseButton),
                "Advanced filter: Close button should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.ResetButton),
                "Advanced filter: Reset button should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.ModifiedDropDown),
                "Advanced filter: Apply button should be displayed");
            Assert.IsTrue(IsElementEquals(AdvancedFilter.CreatedDropDown, ConditionEqual),
                $"Created dropdown should have default value {ConditionEqual}");
            Assert.IsTrue(IsElementEquals(AdvancedFilter.ModifiedDropDown, ConditionEqual),
                $"Modified dropdown should have default value {ConditionEqual}");
            Assert.IsTrue(IsElementFound(AdvancedFilter.Created1),
                "Advanced filter: Created date picker field should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.Modified1),
                "Advanced filter: Modified date picker field should be displayed");
            Assert.IsTrue(IsElementNotFound(AdvancedFilter.Modified2)
                    && IsElementNotFound(AdvancedFilter.Created2),
                @"Advanced filter: Created & Modified should have only 1 date picker field each");

            var conditions = new []
            {
                ConditionBefore, ConditionAfter, ConditionBetween, ConditionEqual
            };
            ClickUntilShown(AdvancedFilter.CreatedDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), conditions),
                "Created dropdown should contain options: " + string.Join(", ", conditions));

            ClickUntilShown(AdvancedFilter.ModifiedDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), conditions),
                "Modified dropdown should contain options: " + string.Join(", ", conditions));

            ClickUntilShown(AdvancedFilter.StatusDropDown, CommonElement.DropDownOptionList);
            conditions = new []
            {
                StatusNoDevice, StatusActive, StatusError, StatusIdle, StatusSyncing, StatusPlaying,
                StatusSleeping, StatusPoweredOff, StatusRebooting, StatusUnknown
            };
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), conditions),
                "Status dropdown should contain options: " + string.Join(", ", conditions));

            DropDownSelect(AdvancedFilter.StatusDropDown, StatusActive);
            DropDownSelect(AdvancedFilter.StatusDropDown, StatusNoDevice);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 2,
                "Only 2 iBeacon places should be shown in Places list");

            SetFilter(string.Empty);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 4,
                "All 4 places should be shown in Places list");

            Click(PageHeader.AdvancedFilterButton);
            Click(string.Format(AdvancedFilter.StatusDropDownSelectionByTextDeleteButton, StatusActive));
            Click(AdvancedFilter.CreatedDropDown); //
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 3,
                "Only 3 places should be shown in Places list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionAfter);
            SelectDate(AdvancedFilter.Created1, DateTime.Now - TimeSpan.FromDays(1));
            DropDownSelect(AdvancedFilter.ModifiedDropDown, ConditionEqual);
            SelectDate(AdvancedFilter.Modified1, DateTime.Now);
            Click(string.Format(AdvancedFilter.StatusDropDownSelectionByTextDeleteButton, StatusNoDevice));
            Click(AdvancedFilter.CreatedDropDown); //
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 1 
                    && IsElementFound(string.Format(PlacesPage.TableRowByTitle, place4.Title)),
                "Only new place should be shown in Places list");

            SetFilter("lfdlsdkfhsdlkf");
            Assert.IsTrue(IsElementFound(PlacesPage.TableRowEmpty),
                "No places should be shown on random text in filter");

            ChangeTenant(TenantTitle.manylang);
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set for this tenant");
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, "Filter")
                    || IsElementEquals(PageHeader.Filter, string.Empty),
                "Filter field should be empty for this tenant");

            ChangeTenant(TenantTitle.advfilter);
            Assert.IsTrue(IsElementFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be still set on switch tenant back");
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, "Filter")
                    || IsElementEquals(PageHeader.Filter, string.Empty),
                "Filter field should be cleared on switch tenant back");

            OpenItemsPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set for Items list page");

            OpenAppsPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set for Apps list page");

            OpenPlacesPage();
            Click(PageHeader.AdvancedFilterButton);
            Click(AdvancedFilter.ResetButton);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 4,
                "All 4 places should be shown on Reset button click");
        }

        [Test, Regression]
        public void RT07430_AppsFilter()
        {
            TestStart();

            OpenAppsPage();
            Click(PageFooter.ShowDeletedButton, ignoreIfNoElement: true, 2);
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Apps list page advanced filter should be inactive");
            Assert.IsTrue(CountElements(AppsPage.TableRow) == 5, 
                "All 5 apps should be shown in Apps list");

            Click(PageHeader.AdvancedFilterButton);
            Assert.IsTrue(IsElementFound(AdvancedFilter.CreatedDropDown),
                "Advanced filter: modal dialog should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.ModifiedDropDown),
                "Advanced filter: Modified dropdown should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.StatusDropDown),
                "Advanced filter: Status dropdown should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.CloseButton),
                "Advanced filter: Close button should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.ResetButton),
                "Advanced filter: Reset button should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.ModifiedDropDown),
                "Advanced filter: Apply button should be displayed");
            Assert.IsTrue(IsElementEquals(AdvancedFilter.CreatedDropDown, ConditionEqual),
                $"Created dropdown should have default value {ConditionEqual}");
            Assert.IsTrue(IsElementEquals(AdvancedFilter.ModifiedDropDown, ConditionEqual),
                $"Modified dropdown should have default value {ConditionEqual}");
            Assert.IsTrue(IsElementFound(AdvancedFilter.Created1),
                "Advanced filter: Created date picker field should be displayed");
            Assert.IsTrue(IsElementFound(AdvancedFilter.Modified1),
                "Advanced filter: Modified date picker field should be displayed");
            Assert.IsTrue(IsElementNotFound(AdvancedFilter.Modified2)
                          && IsElementNotFound(AdvancedFilter.Created2),
                @"Advanced filter: Created & Modified should have only 1 date picker field each");

            var conditions = new []
            {
                ConditionBefore, ConditionAfter, ConditionBetween, ConditionEqual
            };
            ClickUntilShown(AdvancedFilter.CreatedDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), conditions),
                "Created dropdown should contain options: " + string.Join(", ", conditions));

            ClickUntilShown(AdvancedFilter.ModifiedDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), conditions),
                "Modified dropdown should contain options: " + string.Join(", ", conditions));

            ClickUntilShown(AdvancedFilter.StatusDropDown, CommonElement.DropDownOptionList);
            conditions = new []
            {
                StatusNew, StatusAvailable, StatusPublished
            };
            Assert.IsTrue(
                AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), conditions),
                "Status dropdown should contain options: " + string.Join(", ", conditions));

            DropDownSelect(AdvancedFilter.StatusDropDown, StatusNew);
            DropDownSelect(AdvancedFilter.StatusDropDown, StatusPublished);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 3,
                "Only 3 apps should be shown in Apps list");

            Click(PageHeader.AdvancedFilterButton);
            Click(string.Format(AdvancedFilter.StatusDropDownSelectionByTextDeleteButton, StatusNew));
            Click(string.Format(AdvancedFilter.StatusDropDownSelectionByTextDeleteButton, StatusPublished));
            Click(AdvancedFilter.CreatedDropDown);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 5,
                "All 5 apps should be shown in Apps list");

            Click(string.Format(AppsPage.TableRowComposerHqApp1Published));
            EditForm();
            SendText(AppsPage.Description, "some text");
            SubmitForm();
            OpenAppsPage();
            Click(PageHeader.AdvancedFilterButton);
            SelectDate(AdvancedFilter.Modified1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 1,
                "Only app Composer HQ1 should be shown in Apps list");

            SetFilter("gfdsgdfhgfghg");
            ChangeTenant(TenantTitle.manylang);
            OpenAppsPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set for this tenant");

            ChangeTenant(TenantTitle.advfilter);
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be still set on switch tenant back");
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, "Filter")
                          || IsElementEquals(PageHeader.Filter, string.Empty),
                "Filter field should be cleared on switch tenant back");

            OpenItemsPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set for Items list page");

            OpenPlacesPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set for Places list page");

            OpenAppsPage();
            Click(PageHeader.AdvancedFilterButton);
            Click(AdvancedFilter.ResetButton);
            Click(PageFooter.ShowDeletedButton, true, 1);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 5,
                "All 5 apps should be shown on Reset button click");
        }

        [Test, Regression]
        public void RT07440_FilterAfterRelogin()
        {
            TestStart();

            OpenPlacesPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Advanced filter should be not set");

            Click(PageHeader.AdvancedFilterButton);
            SelectDate(AdvancedFilter.Created1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(IsElementFound(PageHeader.AdvancedFilterButtonActive),
                "Places page: Advanced filter should be set");

            Logout();
            Login(CurrentUser, CurrentTenant);
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Places list: Advanced filter should be not set after re-login");

            OpenAppsPage();
            Click(PageHeader.AdvancedFilterButton);
            SelectDate(AdvancedFilter.Created1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(IsElementFound(PageHeader.AdvancedFilterButtonActive),
                "Apps list: Advanced filter should be set");

            Logout();
            Login(CurrentUser, CurrentTenant);
            OpenAppsPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Apps list: Advanced filter should be not set after re-login");

            OpenItemsPage();
            Click(PageHeader.AdvancedFilterButton);
            SelectDate(AdvancedFilter.Created1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(IsElementFound(PageHeader.AdvancedFilterButtonActive),
                "Items list: Advanced filter should be set");

            Logout();
            Login(CurrentUser, CurrentTenant);
            OpenItemsPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.AdvancedFilterButtonActive),
                "Items list: Advanced filter should be not set after re-login");
        }

        [Test, Regression]
        public void RT07450_FilterUrlParameters()
        {
            var place4 = AddPlaceNoType(PlaceStatus.Deleted, pageToBeOpened: 0);
            var place1 = PlaceApi.GetPlace(PlaceType.Ww, PlaceStatus.Any, hasDeviceAssigned: false);
            place1.Description = "some text";
            PlaceApi.SavePlace(place1);
            TestStart();

            OpenPlacesPage();
            Click(PageFooter.ShowDeletedButton, ignoreIfNoElement: true, 2);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 4, "All 4 places should be shown");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionEqual);
            SelectDate(AdvancedFilter.Created1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            var uri = @"?created=0&created_from=" + ConvertDateUtc(DateTime.Now);
            Assert.IsTrue(IsPageContainsUri(uri), $@"URL should contain '{uri}'");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 1,
                "1 place should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place4.Title)),
                $@"Place '{place4.Title}' should be displayed in list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionBetween);
            SelectDate(AdvancedFilter.Created1, DateTime.Now - TimeSpan.FromDays(1));
            SelectDate(AdvancedFilter.Created2, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            uri = @"?created=1&created_from=" + ConvertDateUtc(DateTime.Now - TimeSpan.FromDays(1)) +
                @"&created_to=" + ConvertDateUtc(DateTime.Now);
            Assert.IsTrue(IsPageContainsUri(uri), $@"URL should contain '{uri}'");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 1,
                "1 place should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place4.Title)),
                $@"Place '{place4.Title}' should be displayed in list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionBefore);
            SelectDate(AdvancedFilter.Created1, DateTime.Now - TimeSpan.FromDays(1));
            Click(AdvancedFilter.ApplyButton);
            uri = @"?created=2&created_from=" + ConvertDateUtc(DateTime.Now - TimeSpan.FromDays(1));
            Assert.IsTrue(IsPageContainsUri(uri), $@"URL should contain '{uri}'");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 3,
                "3 places should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place1.Title)),
                $@"Place '{place1.Title}' should be displayed in list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionAfter);
            SelectDate(AdvancedFilter.Created1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            uri = @"?created=3&created_from=" + ConvertDateUtc(DateTime.Now);
            Assert.IsTrue(IsPageContainsUri(uri), $@"URL should contain '{uri}'");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 1,
                "1 place should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place4.Title)),
                $@"Place '{place4.Title}' should be displayed in list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionEqual);
            ClearTextInElement(AdvancedFilter.Created1);
            DropDownSelect(AdvancedFilter.ModifiedDropDown, ConditionEqual);
            SelectDate(AdvancedFilter.Modified1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            uri = @"?modified=0&modified_from=" + ConvertDateUtc(DateTime.Now);
            Assert.IsTrue(IsPageContainsUri(uri), $@"URL should contain '{uri}'");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 2,
                "2 places should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place1.Title)),
                $@"Place '{place1.Title}' should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place4.Title)),
                $@"Place '{place4.Title}' should be displayed in list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.ModifiedDropDown, ConditionBetween);
            SelectDate(AdvancedFilter.Modified1, DateTime.Now - TimeSpan.FromDays(1));
            SelectDate(AdvancedFilter.Modified2, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            uri = @"?modified=1&modified_from=" + ConvertDateUtc(DateTime.Now - TimeSpan.FromDays(1)) +
                @"&modified_to=" + ConvertDateUtc(DateTime.Now);
            Assert.IsTrue(IsPageContainsUri(uri), $@"URL should contain '{uri}'");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 2,
                "2 places should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place1.Title)),
                $@"Place '{place1.Title}' should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place4.Title)),
                $@"Place '{place4.Title}' should be displayed in list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.ModifiedDropDown, ConditionBefore);
            SelectDate(AdvancedFilter.Modified1, DateTime.Now - TimeSpan.FromDays(1));
            Click(AdvancedFilter.ApplyButton);
            uri = @"?modified=2&modified_from=" + ConvertDateUtc(DateTime.Now - TimeSpan.FromDays(1));
            Assert.IsTrue(IsPageContainsUri(uri), $@"URL should contain '{uri}'");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 2,
                "2 places should be displayed in list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.ModifiedDropDown, ConditionAfter);
            SelectDate(AdvancedFilter.Modified1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            uri = @"?modified=3&modified_from=" + ConvertDateUtc(DateTime.Now);
            Assert.IsTrue(IsPageContainsUri(uri), $@"URL should contain '{uri}'");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 2,
                "2 places should be displayed in list");

            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionBefore);
            SelectDate(AdvancedFilter.Created1, DateTime.Now - TimeSpan.FromDays(1));
            DropDownSelect(AdvancedFilter.ModifiedDropDown, ConditionBetween);
            SelectDate(AdvancedFilter.Modified1, DateTime.Now - TimeSpan.FromDays(1));
            SelectDate(AdvancedFilter.Modified2, DateTime.Now);
            DropDownSelect(AdvancedFilter.StatusDropDown, StatusActive, false);
            DropDownSelect(AdvancedFilter.StatusDropDown, StatusNoDevice, false);
            Click(AdvancedFilter.ApplyButton);
            uri = $@"?statuses={(int) PlaceStatus.Active},{(int) PlaceStatus.NoDevice}" + 
                  @"&created=2&created_from=" + ConvertDateUtc(DateTime.Now - TimeSpan.FromDays(1)) +
                  @"&modified=1&modified_from=" + ConvertDateUtc(DateTime.Now - TimeSpan.FromDays(1)) +
                  @"&modified_to=" + ConvertDateUtc(DateTime.Now);
            Assert.IsTrue(IsPageContainsUri(uri), $@"URL should contain '{uri}'");
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 1,
                "1 place should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place1.Title)),
                $@"Place '{place1.Title}' should be displayed in list");

            var url = GetCurrentUrl();
            var handle = OpenNewTab();
            Navigate(url);
            Assert.IsTrue(CountElements(PlacesPage.TableRow) == 1,
                "1 place should be displayed in list");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, place1.Title)),
                $@"Place '{place1.Title}' should be displayed in list");
            CloseTab(handle);
        }

        [Test, Regression]
        public void RT07500_CounterBasics()
        {
            CurrentTenant = TenantTitle.counter;
            TestStart();

            OpenAppsPage();
            Assert.IsTrue(GetCounter() == null,
                "There should be no counter on empty Apps list page header");

            //OpenItemsPage();
            // Assert.IsTrue(GetCounter() == null,
            //    "There should be no counter on empty Items list page header");

            OpenPlacesPage();
            Assert.IsTrue(GetCounter() == null,
                "There should be no counter on empty Places list page header");

            Click(PageFooter.AddPlaceButton);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            SubmitForm();
            var placeId = GetEntityIdFromUrl();
            OpenPlacesPage();
            Assert.IsTrue(GetCounter() == 1,
                "There should be counter (1) on Places list page header");

            OpenEntityPage<Place>(placeId);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.CreateNewChildPlaceButton);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            SubmitForm();
            placeId = GetEntityIdFromUrl();
            OpenPlacesPage();
            Assert.IsTrue(GetCounter() == 2,
                "There should be counter (2) on Places list page header");

            OpenEntityPage<Place>(placeId);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.CreateNewChildPlaceButton);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            SubmitForm();
            OpenEntityPage<Place>(placeId);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.CreateNewChildPlaceButton);
            SendText(PlacesPage.Title, $"Auto test {RandomNumber}");
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            SubmitForm();
            var grandsonPlaceId = GetEntityIdFromUrl();
            OpenPlacesPage();
            Assert.IsTrue(GetCounter() == 4,
                "There should be counter (4) on Places list page header");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(
                TestConfig.PlayerAppFolder, TestConfig.PlayerAppFile, TestConfig.PlayerAppVersions[0]);
            FileManager.Upload(pathFile);
            SubmitForm();
            OpenAppsPage();
            Assert.IsTrue(GetCounter() == 1,
                "There should be counter (1) on Apps list page header");

            ChangeTenant(TenantTitle.manylang);
            var app = AddAppIbeacon(TestConfig.IbeaconAppVersions[0]);
            OpenEntityPage(app);
            MouseOver(PageFooter.DistributeSubMenu);
            Click(PageFooter.DistributeAppButton);
            Click(string.Format(TenantsPage.TableRowByTitle, TenantTitle.counter));
            Click(TenantsPage.UploadButton);
            if (IsElementFoundQuickly(AppsPage.CrossLanguageMdValuesOverwrittenDialog))
            {
                Click(AppsPage.YesButton);
                if (IsElementFoundQuickly(AppsPage.DistributeOverwriteFollowingItemsDialog))
                {
                    Click(AppsPage.YesButton);
                }
            }
            Click(AppsPage.OkButton);
            ChangeTenant(TenantTitle.counter);
            OpenAppsPage();
            Assert.IsTrue(GetCounter() == 2,
                "There should be counter (2) on Apps list page header");

            Click(AppsPage.TableRowIbeaconApp);
            var appId = GetEntityIdFromUrl();
            Click(AppsPage.Versions);
            Click(AppsPage.TableRow);
            Click(PageFooter.DeleteButton);
            Click(AppsPage.MarkAsDeletedButton);
            Click(PageFooter.HideDeletedButton, true, 1);
            Assert.IsTrue(GetCounter() == 1,
                "There should be counter (1) on Apps list page header");

            Click(PageFooter.ShowDeletedButton);
            Assert.IsTrue(GetCounter() == 2,
                "There should be counter (2) on Apps list page header");

            OpenEntityPage<Place>(grandsonPlaceId);
            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteButton);
            OpenPlacesPage();
            Click(PageFooter.HideDeletedButton, true, 1);
            Assert.IsTrue(GetCounter() == 3,
                "There should be counter (3) on Places list page header");

            Click(PageFooter.ShowDeletedButton, true, 1);
            Assert.IsTrue(GetCounter() == 4,
                "There should be counter (4) on Places list page header");

            OpenItemsPage();
            Click(string.Format(ItemsPage.TableRowByText, ItemTypeDailyItemsReport));
            Click(PageFooter.DeleteButton);
            var itemId = GetEntityIdFromUrl();
            Click(PlacesPage.DeleteButton);
            OpenItemsPage();
            Assert.IsTrue(GetCounter() == 5,
                "There should be counter (5) on Items list page header");

            Click(PageFooter.ShowDeletedButton);
            Assert.IsTrue(GetCounter() == 6,
                "There should be counter (6) on Items list page header");

            Click(PageFooter.HideDeletedButton);
            Assert.IsTrue(GetCounter() == 5,
                "There should be counter (5) on Items list page header");

            OpenEntityPage<Item>(itemId);
            Click(PageFooter.RestoreButton);
            SubmitForm();
            OpenItemsPage();
            Assert.IsTrue(GetCounter() == 6,
                "There should be counter (6) on Items list page header");

            OpenEntityPage<AppResponse>(appId);
            Click(AppsPage.VersionInactive);
            Click(AppsPage.TableRow);
            Click(PageFooter.RestoreButton);
            OpenAppsPage();
            Assert.IsTrue(GetCounter() == 2,
                "There should be counter (2) on Apps list page header");

            OpenEntityPage<Place>(grandsonPlaceId);
            Click(PageFooter.RestoreButton);
            SubmitForm();
            OpenPlacesPage();
            Assert.IsTrue(GetCounter() == 4,
                "There should be counter (4) on Places list page header");
        }

        [Test, Regression]
        public void RT07510_CounterAndFilter()
        {
            CurrentTenant = TenantTitle.counter;
            Place place1 = null;
            PlaceApi.DeletePlaces(PlaceType.Any, TenantTitle.counter);
            Parallel.Invoke(() =>
            {
                place1 = AddPlaceIbeacon(PlaceStatus.NoDevice, pageToBeOpened: 0, isCreateNewPlace: true);
                AddPlaceIbeacon(PlaceStatus.NoDevice, pageToBeOpened: 0, isCreateNewPlace: true);
                AddAppIbeacon(TestConfig.IbeaconAppVersions[0]);
                AddAppPlayer();
            });
            AddAppComposerHq1();
            TestStart();

            OpenEntityPage(place1);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            var number = CountElements(PlacesPage.TableRow);
            Assert.IsTrue(GetCounterModal() == number, 
                $"There should be counter ({number}) in modal dialog");

            ClickAtPoint(CommonElement.Backdrop, 10, 300); // close modal
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);
            number = CountElements(PlacesPage.TableRow);
            Assert.IsTrue(GetCounterModal() == number,
                $"There should be counter ({number}) in modal dialog");

            ClickAtPoint(CommonElement.Backdrop, 10, 300); // close modal
            OpenPlacesPage();
            Click(PageFooter.HideDeletedButton, true, 1);
            OpenMediaPage();
            Click(MediaLibrary.CloseButton);
            Assert.IsTrue(GetCounter() == 2,
                "There should be counter (2) on Places list page header");

            OpenAppsPage();
            OpenMediaPage();
            Click(MediaLibrary.CloseButton);
            Assert.IsTrue(GetCounter() == 3,
                "There should be counter (3) on Apps list page header");

            OpenItemsPage();
            OpenMediaPage();
            Click(MediaLibrary.CloseButton);
            Assert.IsTrue(GetCounter() == 6,
                "There should be counter (6) on Items list page header");

            OpenPlacesPage();
            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionEqual);
            SelectDate(AdvancedFilter.Created1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(GetCounter() == 2,
                "There should be counter (2) on Places list page header");

            SetFilter("Auto");
            Assert.IsTrue(GetCounter() == 2,
                "There should be counter (2) on Places list page header");

            SetFilter("DFSDGFSDGFFDGDGFD");
            Assert.IsTrue(GetCounter() == null,
                "There should be no counter on Places list page header");

            OpenAppsPage();
            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionEqual);
            SelectDate(AdvancedFilter.Created1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(GetCounter() == 3,
                "There should be counter (3) on Apps list page header");

            SetFilter("iBeacon");
            Assert.IsTrue(GetCounter() == 1,
                "There should be counter (1) on Apps list page header");

            SetFilter("DFSDGFSDGFFDGDGFD");
            Assert.IsTrue(GetCounter() == null,
                "There should be no counter on Apps list page header");

            OpenItemsPage();
            Click(PageHeader.AdvancedFilterButton);
            DropDownSelect(AdvancedFilter.CreatedDropDown, ConditionEqual);
            SelectDate(AdvancedFilter.Created1, DateTime.Now);
            Click(AdvancedFilter.ApplyButton);
            Assert.IsTrue(GetCounter() == 6,
                "There should be counter (6) on Items list page header");

            SetFilter("booking");
            Assert.IsTrue(GetCounter() == 1,
                "There should be counter (1) on Items list page header");

            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            SendText(ItemsPage.Title, "Porsche Car");
            SendText(ItemsPage.Vin, $"{RandomNumber}AA");
            SubmitForm();
            OpenItemsPage();
            SetFilter(string.Empty);
            DropDownSelect(PageHeader.FilterDropDown, ItemTypePorscheCar, false);
            Assert.IsTrue(GetCounter() == 1,
                "There should be counter (1) on Items list page header");

            SetFilter("DFSDGFSDGFFDGDGFD");
            Assert.IsTrue(GetCounter() == null,
                "There should be no counter on Items list page header");

            SetFilter(string.Empty);
            DropDownSelect(PageHeader.FilterDropDown, ItemTypeAllTypes, false);
            Assert.IsTrue(GetCounter() == 7,
                "There should be counter (7) on Items list page header");
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
        }
    }
}
