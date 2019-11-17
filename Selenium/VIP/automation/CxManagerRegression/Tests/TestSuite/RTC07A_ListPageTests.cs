using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.Apps;
using Models.Items;
using Models.Places.Devices;
using Models.UserDirectory;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;
using Place = Models.Places.Place;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC07A_ListPageTests : ParentTest
    {
        private Place _placeNoType, _childPlace1, _childPlace11, _childPlace12, _childPlace13, _placeWw1, _placeWw2,
            _placeIbeacon;
        private AppResponse _appIbeacon, _appComposerVipb;
        private Item _itemPorsche1, _itemPorsche2, _itemPorsche3, _itemPdf1;
        private bool _isItemsPrepared;
        private string _rootTitle, _papaTitle, _mamaTitle, _child1Title, _child2Title, _porsche9699001Title;

        private readonly string[] _sortSamplePlaces =
        {
            StatusDeleted, StatusNoDevice, StatusUnknown, StatusError, StatusPoweredOff, StatusSleeping, StatusIdle, 
            StatusPlaying, StatusActive
        };
        private readonly string[] _sortSampleApps =
        {
            StatusDeleted, StatusAvailable, StatusPublished, StatusNew
        };
        private readonly string[] _sortSampleItemTypes =
        {
            ItemTypeAllTypes, ItemTypeCars, ItemTypePorscheCar, ItemTypeUsedCar, ItemTypePdfCar, ItemTypeCustomerProfile, 
            ItemTypeEmailTemplate, ItemTypeEmployee, ItemTypeEventOrPromotion, ItemTypePoi, ItemTypeSalesAppointment, 
            ItemTypeServiceBooking, ItemTypeTestDrive
        };
        private readonly string[] _sortSampleItemStatuses = { StatusAvailable, StatusDeleted };
        private readonly string[] _filterResultItems =
        {
            ItemTypeCars, ItemTypePorscheCar, ItemTypeUsedCar, ItemTypePdfCar, ItemTypeCustomerProfile,
            ItemTypeEmailTemplate, ItemTypeEventOrPromotion, ItemTypePoi, ItemTypeServiceBooking
        };
        private readonly string[] _filterResultCars = { ItemTypePorscheCar, ItemTypeUsedCar, ItemTypePdfCar };

        private const string AttrForSorting = "aria-sort";
        private const string DateTimeAttrForSorting = "data-updated";
        private const string AttrParentTitle = "parent-title";
        private const string AscOrderAttrValue = "ascending";
        private const string DescOrderAttrValue = "descending";
        private const string WwName = "DQZJF066";
        private const string RackNumber = "DQZJF666";
        private const string TitleColumnName = "Tenant";
        private const string TenantCodeColumnName = "Tenant code";
        private const string CreatedByColumnName = "Created by";
        private const string ModifiedColumnName = "Modified";

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            CurrentTenant = TenantTitle.manylang;
            CurrentUser = TestConfig.AdminUser;

            Parallel.Invoke(
                () => AddAppPlayer(AppStatus.Published),
                () => _appIbeacon = AddAppIbeacon(TestConfig.IbeaconAppVersions[0], isOverwriteItems: true),
                () => _appIbeacon = AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true),
                () => AddAppDpt(AppStatus.New, TestConfig.DptAppVersions[0]),
                () => _placeWw1 = AddPlaceWw(PlaceStatus.NoDevice, pageToBeOpened: 0, isCreateNewPlace: true),
                () => _placeWw2 = AddPlaceWw(PlaceStatus.Any, isAssignDevice: true, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => AddPlaceWw(PlaceStatus.NoDevice, isAssignDevice: true, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => _childPlace1 = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true)
            );
            Parallel.Invoke(
                () => _childPlace11 = AddPlaceIbeacon(PlaceStatus.Any, pageToBeOpened: 0, isAssignIbeacon: true,
                    isCreateNewPlace: true),
                () => _childPlace12 = AddPlaceIbeacon(PlaceStatus.NoDevice, pageToBeOpened: 0,
                    isCreateNewPlace: true),
                () => _childPlace13 = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => _placeNoType = AddPlaceNoType(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true),
                () => _placeIbeacon = AddPlaceIbeacon(PlaceStatus.Active, pageToBeOpened: 0, isAssignIbeacon: true,
                    isCreateNewPlace: true),
                () => _itemPorsche1 = AddItem(ItemType.PorscheCar)
            );

            _childPlace1 =
                AssignChildrenToParentPlace(_childPlace1, true, _childPlace11, _childPlace12, _childPlace13);
            AddAppComposerHq1(AppStatus.Deleted);

            Parallel.Invoke(
                () => _placeWw2 = AssignImageToPlace(_placeWw2),
                () => _placeWw1 = AssignChildrenToParentPlace(_placeWw1, true, _childPlace1),
                () => _appComposerVipb = AddAppComposerVipB(TestConfig.ComposerVipbAppLatestVersion),
                () => AddAppComposerHq2(AppStatus.Published),
                () => _placeIbeacon = AssignAppToPlace(_placeIbeacon, _appIbeacon, null, null, isAddSilently: true),
                () => _childPlace1 = AssignAppToPlace(_childPlace1, _appComposerVipb, null, null, isAddSilently: true)
            );
            _itemPorsche1 = AssignPlaceToItem(_itemPorsche1, _placeIbeacon, isAddSilently: true).Item1;

            _rootTitle = TenantTitle.Root.ToString();
            _papaTitle = TenantTitle.Papa.ToString();
            _mamaTitle = TenantTitle.Mama.ToString();
            _child1Title = TenantTitle.Child_1.ToString();
            _child2Title = TenantTitle.Child_2.ToString();
            _porsche9699001Title = TenantTitle.porsche9699001.ToString();
        }

        private void PrepareItems()
        {
            Parallel.Invoke(
                () => _itemPorsche2 = AddItem(ItemType.PorscheCar, isAddNew: true),
                () => _itemPorsche3 = AddItem(ItemType.PorscheCar, isAddNew: true),
                () => AddItem(ItemType.UsedCar, isAddNew: true),
                () => AddItem(ItemType.UsedCar, isAddNew: true),
                () => AddItem(ItemType.PdfCar, isAddNew: true),
                () => _itemPdf1 = AddItem(ItemType.PdfCar, isAddNew: true)
            );
             Parallel.Invoke(
                () => AddItem(ItemType.CustomerProfile),
                () => AddItem(ItemType.EmailTemplate),
                () => AddItem(ItemType.EventOrPromotion),
                () => AddItem(ItemType.Poi),
                () => AddItem(ItemType.ServiceBooking),
                () => ItemApi.DeleteItem(_itemPorsche3.Id)
            );
            _isItemsPrepared = true;
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
        public void RT07010_PlacesColumns()
        {
            TestStart();

            Assert.IsTrue(IsElementFound(PlacesPage.PictureColumn), "Places list table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.TitleColumn), "Places list table should have Title column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PictureColumn),
                "Places list table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DeviceColumn),
                "Places list table should have Device column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.StatusColumn),
                "Places list table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ModifiedColumn),
                "Places list table should have Modified column");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddPlaceButton), "Places page should have Add New button");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Places page should have Show Deleted button");

            OpenEntityPage(_itemPorsche1);
            EditForm();
            ClickUntilShown(ItemsPage.AssignedPlacesAddButton, ItemsPage.AssignedPlacesPlaceLastDropDown);
            DropDownSelect(ItemsPage.AssignedPlacesPlaceLastDropDown, _childPlace11.Title);
            SubmitForm();
            OpenPlacesPage();
            Assert.IsTrue(IsElementFound(PlacesPage.ItemsColumn), "Places list table should have Items column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PictureColumn),
                "Places list table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.TitleColumn), "Places list table should have Title column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PictureColumn),
                "Places list table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.DeviceColumn),
                "Places list table should have Device column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.StatusColumn),
                "Places list table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ModifiedColumn),
                "Places list table should have Modified column");
        }

        [Test, Regression]
        public void RT07020_PlacesColumnsSorting()
        {
            TestStart();

            Click(PlacesPage.TitleColumn);
            if (!IsAlphabeticallySorted(PlacesPage.TableRowTitle, isElementDropDown: false))
            {
                Click(PlacesPage.TitleColumn);
            }
            Assert.IsTrue(IsAlphabeticallySorted(PlacesPage.TableRowTitle, isElementDropDown: false),
                "Place titles should be in ascending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.TitleColumn, AttrForSorting) == AscOrderAttrValue,
                "Title column header should contain arrow for ascending order");

            Click(PlacesPage.TitleColumn);
            Assert.IsTrue(IsAlphabeticallySorted(PlacesPage.TableRowTitle, isElementDropDown: false, isReverseOrder: true),
                "Place titles should be in descending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.TitleColumn, AttrForSorting) == DescOrderAttrValue,
                "Title column header should contain arrow for descending order");

            Click(PlacesPage.DeviceColumn);
            Assert.IsTrue(IsAlphabeticallySorted(PlacesPage.TableRowDevice, isElementDropDown: false),
                "Places should be sorted by Device in ascending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.DeviceColumn, AttrForSorting) == AscOrderAttrValue,
                "Device column header should contain arrow for ascending order");

            Click(PlacesPage.DeviceColumn);
            Assert.IsTrue(IsAlphabeticallySorted(PlacesPage.TableRowDevice, isElementDropDown: false, isReverseOrder: true),
                "Places should be sorted by Device in descending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.DeviceColumn, AttrForSorting) == DescOrderAttrValue,
                "Device column header should contain arrow for descending order");

            Click(PlacesPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(PlacesPage.TableRowStatus, _sortSamplePlaces, isReverseOrder: true),
                "Places should be sorted by Status in descending order: " +
                string.Join(", ", _sortSamplePlaces.Reverse()));
            Assert.IsTrue(GetElementAttribute(PlacesPage.StatusColumn, AttrForSorting) == DescOrderAttrValue,
                "Status column header should contain arrow for descending order");

            Click(PlacesPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(PlacesPage.TableRowStatus, _sortSamplePlaces),
                "Places should be sorted by Status in ascending order: " +
                string.Join(", ", _sortSamplePlaces));
            Assert.IsTrue(GetElementAttribute(PlacesPage.StatusColumn, AttrForSorting) == AscOrderAttrValue,
                "Status column header should contain arrow for ascending order");
        }

        [Test, Regression]
        public void RT07030_PlacesHierarchyStates()
        {
            TestStart();

            RefreshPage();
            Assert.IsTrue(IsElementFound(string.Format(PlacesPage.TableRowByTitleCollapsed, _placeWw1.Title)),
                $"Place {_placeWw1.Title} row should be collapsed");

            Click(string.Format(PlacesPage.TableRowByTitleCollapseExpandButton, _placeWw1.Title));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitleExpanded, _placeWw1.Title)),
                $"Place {_placeWw1.Title} row should be expanded");

            Click(string.Format(PlacesPage.TableRowByTitleCollapseExpandButton, _childPlace1.Title));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitleExpanded, _childPlace1.Title)),
                $"Place {_childPlace1.Title} row should be expanded");

            Click(string.Format(PlacesPage.TableRowByTitleCollapseExpandButton, _childPlace1.Title));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitleCollapsed, _childPlace1.Title)),
                $"Place {_childPlace1.Title} row should be collapsed");
        }

        [Test, Regression]
        public void RT07040_PlacesHierarchySorting()
        {
            TestStart();
            ClickUntilShown(string.Format(PlacesPage.TableRowByTitleCollapseExpandButton, _placeWw1.Title),
                string.Format(PlacesPage.TableRowByTitleCollapseExpandButton, _childPlace1.Title));

            Click(string.Format(PlacesPage.TableRowByTitleCollapseExpandButton, _childPlace1.Title));
            Click(PlacesPage.StatusColumn);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitleExpanded, _placeWw1.Title)),
                $"Place {_placeWw1.Title} row should be expanded");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitleExpanded, _childPlace1.Title)),
                $"Child place {_childPlace1.Title} row should be expanded");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _childPlace11.Title)),
                $"Grandson place {_childPlace11.Title} row should be shown");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _childPlace12.Title)),
                $"Grandson place {_childPlace12.Title} row should be shown");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _childPlace13.Title)),
                $"Grandson place {_childPlace13.Title} row should be shown");
            Assert.IsTrue(
                GetElementAttribute(string.Format(PlacesPage.TableRowByTitleExpanded, _childPlace1.Title), AttrParentTitle)
                    == _placeWw1.Title, 
                $"Place {_childPlace1.Title} should be a child of {_placeWw1.Title}");
            Assert.IsTrue(
                GetElementAttribute(string.Format(PlacesPage.TableRowByTitleExpanded, _childPlace11.Title), AttrParentTitle)
                    == _childPlace1.Title,
                $"Place {_childPlace11.Title} should be a child of {_childPlace1.Title}");
            Assert.IsTrue(
                GetElementAttribute(string.Format(PlacesPage.TableRowByTitleExpanded, _childPlace12.Title), AttrParentTitle)
                    == _childPlace1.Title,
                $"Place {_childPlace12.Title} should be a child of {_childPlace1.Title}");
            Assert.IsTrue(
                GetElementAttribute(string.Format(PlacesPage.TableRowByTitleExpanded, _childPlace13.Title), AttrParentTitle)
                    == _childPlace1.Title,
                $"Place {_childPlace13.Title} should be a child of {_childPlace1.Title}");
            Assert.IsTrue(IsSortedInSpecificOrder(string.Format(PlacesPage.TableRowStatusByParentTitle, _childPlace1.Title), 
                    _sortSamplePlaces, isReverseOrder: true),
                $"Grandsons should be sorted by Status in descending order: " + 
                string.Join(", ", _sortSamplePlaces.Reverse()));

            Click(PlacesPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(string.Format(PlacesPage.TableRowStatusByParentTitle, _childPlace1.Title), 
                    _sortSamplePlaces),
                $"Grandsons should be sorted by Status in ascending order: " +
                string.Join(", ", _sortSamplePlaces));

            Click(PlacesPage.DeviceColumn);
            Assert.IsTrue(IsAlphabeticallySorted(string.Format(PlacesPage.TableRowDeviceByParentTitle, _childPlace1.Title), 
                    isElementDropDown: false),
                $"Grandsons should be sorted by Device in ascending order");

            Click(PlacesPage.DeviceColumn);
            Assert.IsTrue(IsAlphabeticallySorted(string.Format(PlacesPage.TableRowDeviceByParentTitle, _childPlace1.Title),
                    isElementDropDown: false, isReverseOrder: true),
                $"Grandsons should be sorted by Device in descending order");
        }

        [Test, Regression]
        public void RT07050_PlacesStatusAndHierarchySaved()
        {
            TestStart();
            if (string.IsNullOrEmpty(GetElementAttribute(PlacesPage.StatusColumn, AttrForSorting)))
            {
                Click(PlacesPage.StatusColumn);
            }

            var sortOrder = GetElementAttribute(PlacesPage.StatusColumn, AttrForSorting);
            ChangeTenant(TenantTitle.nolang);
            ChangeTenant(TenantTitle.manylang);
            Assert.IsTrue(
                IsSortedInSpecificOrder(PlacesPage.TableRowStatusForParents, _sortSamplePlaces, sortOrder == DescOrderAttrValue),
                $@"Places should be sorted by Status in tenant '{TenantTitle.manylang.ToString()}'");
            
            OpenEntityPage(_placeWw1);
            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteChildPlacesButton);
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlacesUri}"),
                "User should be redirected to Places page after place delete");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeWw1.Title)),
                $"Parent place {_placeWw1.Title} is deleted and should not be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _childPlace1.Title)),
                $"Child place {_childPlace1.Title} is deleted and should not be shown");

            Click(PageFooter.ShowDeletedButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeWw1.Title)),
                $"Parent place {_placeWw1.Title} should be shown");

            ClickUntilShown(string.Format(PlacesPage.TableRowByTitleCollapseExpandButton, _placeWw1.Title),
                string.Format(PlacesPage.TableRowStatusByTitle, _childPlace1.Title));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _childPlace1.Title)),
                $"Child place {_childPlace1.Title} should be shown");
            Assert.IsTrue(IsElementEquals(string.Format(PlacesPage.TableRowStatusByTitle, _childPlace1.Title), StatusDeleted),
                $"Child place {_childPlace1.Title} should have Status {StatusDeleted}");
            Assert.IsTrue(IsElementEquals(string.Format(PlacesPage.TableRowStatusByTitle, _placeWw1.Title), StatusDeleted),
                $"Parent place {_placeWw1.Title} should have Status {StatusDeleted}");

            OpenEntityPage(_placeWw1);
            Click(PageFooter.RestoreButton);
            SubmitForm();
            Click(PlacesPage.OkButton, ignoreIfNoElement: true, timeout: 1);
            OpenPlacesPage();
            Assert.IsTrue(IsElementEquals(string.Format(PlacesPage.TableRowStatusByTitle, _childPlace1.Title), StatusNoDevice),
                $"Child place {_childPlace1.Title} should have Status {StatusNoDevice}");
            Assert.IsTrue(IsElementEquals(string.Format(PlacesPage.TableRowStatusByTitle, _placeWw1.Title), StatusNoDevice),
                $"Parent place {_placeWw1.Title} should have Status {StatusNoDevice}");
            Assert.IsTrue(
                GetElementAttribute(string.Format(PlacesPage.TableRowHierarchyByTitle, _childPlace1.Title), AttrParentTitle)
                    == _placeWw1.Title,
                $"Place {_childPlace1.Title} should be a child of {_placeWw1.Title}");

            OpenEntityPage(_placeWw1);
            Click(PageFooter.DeleteButton);
            Click(PlacesPage.KeepChildPlacesButton);
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlacesUri}"),
                "User should be redirected to Place list page after place delete");
            Assert.IsTrue(IsElementEquals(string.Format(PlacesPage.TableRowStatusByTitle, _placeWw1.Title), StatusDeleted),
                $"Parent place {_placeWw1.Title} should have Status {StatusDeleted}");
            Assert.IsTrue(IsElementEquals(string.Format(PlacesPage.TableRowStatusByTitle, _childPlace1.Title), StatusNoDevice),
                $"Child place {_childPlace1.Title} should have Status {StatusNoDevice}");
            Assert.IsFalse(
                GetElementAttribute(string.Format(PlacesPage.TableRowHierarchyByTitle, _childPlace1.Title), AttrParentTitle)
                    == _placeWw1.Title,
                $"Place {_childPlace1.Title} should not be a child of {_placeWw1.Title}");
        }

        [Test, Regression]
        public void RT07060_PlaceListFilter()
        {
            TestStart();
            Parallel.Invoke(
                () => _placeWw2 = AssignDeviceToPlace(_placeWw2, new Device { Name = WwName }),
                () => _placeIbeacon = AssignDeviceToPlace(_placeIbeacon)
            );

            OpenPlacesPage();
            RefreshPage();
            SetFilter(_placeWw2.Title);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeWw2.Title)),
                $"Parent place {_placeWw2.Title} should be shown");

            OpenAppsPage();
            OpenPlacesPage();
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, string.Empty) || IsElementEquals(PageHeader.Filter, "Filter"), 
                "Switch to Apps page and back should clean Filter field");

            SetFilter(RandomNumber);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.TableRowEmpty), 
                "Nothing should be found on wrong Filter data");

            SetFilter(string.Empty);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeWw2.Title)),
                "Normal table content should be shown on empty Filter");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeIbeacon.Title)),
                "Normal table content should be shown on empty Filter");

            SetFilter(_childPlace11.Title);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _childPlace11.Title)),
                $"Parent place {_childPlace1.Title} should be shown with child {_childPlace11.Title}");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _childPlace1.Title)),
                $"Child place {_childPlace11.Title} should be shown with parent {_childPlace1.Title}");

            SetFilter(RackNumber);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeWw2.Title)),
                $"Place {_placeWw2.Title} should be shown on Filter by device rack number");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeIbeacon.Title)),
                $"Place {_placeIbeacon.Title} should not be shown on Filter by device rack number");

            SetFilter(WwName);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeWw2.Title)),
                $"Place {_placeWw2.Title} should be shown on Filter by device PC Name");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeIbeacon.Title)),
                $"Place {_placeIbeacon.Title} should not be shown on Filter by device PC Number");

            SetFilter(_placeIbeacon.Device.Data.UUID);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeIbeacon.Title)),
                $"Place {_placeIbeacon.Title} should be shown on Filter by device UUID");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeWw2.Title)),
                $"Place {_placeWw2.Title} should be shown on Filter by device UUID");
        }

        [Test, Regression]
        public void RT07070_PlaceImageThumbnailSize()
        {
            TestStart();
            OpenPlacesPage();
            SetFilter(string.Empty);

            var tn1Size = GetElementSize(PlacesPage.TableRowImage);
            ChangeWindowSize(new Size(450, 450));
            Assert.IsTrue(GetElementSize(PlacesPage.TableRowImage) == tn1Size,
                "Place image thumbnail size should not depend on browser window size");

            WindowMaximize();
        }

        [Test, Regression]
        public void RT07080_AppsColumns()
        {
            TestStart();
            
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(AppsPage.ScreenshotColumn), "Apps list table should have Screenshot column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.TitleColumn), "Apps list table should have Title column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.StatusColumn),
                "Apps list table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.CreatedByColumn),
                "Apps list table should have Created By column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ModifiedColumn),
                "Apps list table should have Modified column");
            
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddAppInAppsButton), "Apps page should have Add New button");
            Click(PageFooter.HideDeletedButton, ignoreIfNoElement: true, 1);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Apps page should have Show Deleted button");

            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.TableRowComposerHqApp1Deleted),
                $"App row {AppTitle.ComposerHq1} with status {StatusDeleted} should not be shown");
            Click(PageFooter.ShowDeletedButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.TableRowComposerHqApp1Deleted),
                $"App row {AppTitle.ComposerHq1} with status {StatusDeleted} should be shown");
        }

        [Test, Regression]
        public void RT07090_AppsColumnsSorting()
        {
            TestStart();
            OpenAppsPage();
            if (IsElementFoundQuickly(PageFooter.ShowDeletedButton))
            {
                Click(PageFooter.ShowDeletedButton);
            }

            Assert.IsTrue(IsSortedInSpecificOrder(AppsPage.TableRowStatus, _sortSampleApps, isReverseOrder: true),
                "Column Status should be sorted in following order by default: " +
                string.Join(", ", _sortSampleApps.Reverse()));
            
            Click(AppsPage.TitleColumn);
            Assert.IsTrue(IsAlphabeticallySorted(AppsPage.TableRowTitle, isElementDropDown: false),
                $"App titles should be in descending order");
            Assert.IsTrue(GetElementAttribute(AppsPage.TitleColumn, AttrForSorting) == AscOrderAttrValue,
                $"Title column header should contain arrow for ascending order");

            Click(AppsPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(AppsPage.TableRowStatus, _sortSampleApps, isReverseOrder: true),
                $"Apps should be sorted by Status in descending order: " +
                string.Join(", ", _sortSampleApps.Reverse()));
            Assert.IsTrue(GetElementAttribute(AppsPage.StatusColumn, AttrForSorting) == DescOrderAttrValue,
                $"Status column header should contain arrow for descending order");

            Click(AppsPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(AppsPage.TableRowStatus, _sortSampleApps),
                $"Apps should be sorted by Status in ascending order: " +
                string.Join(", ", _sortSampleApps));
            Assert.IsTrue(GetElementAttribute(AppsPage.StatusColumn, AttrForSorting) == AscOrderAttrValue,
                $"Status column header should contain arrow for ascending order");

            Click(AppsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySorted(AppsPage.TableRowModified, isElementDropDown: false, isReverseOrder: true),
                $"Modified should be in descending order");
            Assert.IsTrue(GetElementAttribute(AppsPage.ModifiedColumn, AttrForSorting) == DescOrderAttrValue,
                $"Modified column header should contain arrow for descending order");

            Click(AppsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySorted(AppsPage.TableRowModified, isElementDropDown: false),
                $"Modified should be in ascending order");
            Assert.IsTrue(GetElementAttribute(AppsPage.ModifiedColumn, AttrForSorting) == AscOrderAttrValue,
                $"Modified column header should contain arrow for ascending order");
        }

        [Test, Regression]
        public void RT07100_AppsListFilter()
        {
            TestStart();
            OpenAppsPage();

            SetFilter($"{RandomNumber}");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.TableEmpty), 
                "Apps list should not contain any app due to wrong Filter expression");
            
            OpenPlacesPage();
            OpenAppsPage();
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, string.Empty) || IsElementEquals(PageHeader.Filter, "Filter"), 
                "Filter field should be empty");

            SetFilter(AppTitle.Ibeacon);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(AppsPage.TableRowByText, AppTitle.Ibeacon)),
                $"App {AppTitle.Ibeacon} should be shown in Apps list");
            Assert.IsTrue(IsElementNotFound(string.Format(AppsPage.TableRowByText, AppTitle.Player)),
                $"App {AppTitle.Player} should be not shown in Apps list");
            Assert.IsTrue(CountElements(AppsPage.TableRowTitle) == 1,
                $"Only {AppTitle.Ibeacon} should be shown in Apps list");

            SetFilter(StatusAvailable);
            ChangeTenant(TenantTitle.nolang);
            ChangeTenant(TenantTitle.manylang);
            OpenAppsPage();
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, string.Empty) || IsElementEquals(PageHeader.Filter, "Filter"), 
                "Filter field should be reset");
            //Assert.IsTrue(IsEqual(PageHeader.Filter, StatusAvailable), $"Filter field should keep value {StatusAvailable}");
            //Assert.IsTrue(IsAllElementsContainText(GetTableColumnData(AppsPage.TableRowStatus), StatusAvailable),
            //    $"Only apps with status {StatusAvailable} should be shown in app list");
        }

        [Test, Regression]
        public void RT07110_SelectPlaceColumns()
        {
            TestStart();

            OpenEntityPage(_placeIbeacon);
            if (IsElementFoundQuickly(PageFooter.RestoreButton))
            {
                Click(PageFooter.RestoreButton);
            }
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);
            Assert.IsTrue(IsElementFound(PlacesPage.PictureColumn), "Places popup table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.TitleColumn), "Places popup table should have Title column");
            //Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ItemsColumn),
            //    "Places popup table should have Items column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.StatusColumn),
                "Places popup table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ModifiedColumn),
                "Places popup table should have Modified column");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.CancelButton), "Places popup should have Cancel button");

            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(PlacesPage.TableRowByStatus, StatusDeleted)),
                $"There should be no places with status {StatusDeleted} in places popup table");
        }

        [Test, Regression]
        public void RT07120_SelectPlaceColumnsSorting()
        {
            TestStart(TestConfig.LoginUrl, false);
            ClearBrowserCache();
            TestStart();
            if (IsElementNotFoundQuickly(PlacesPage.SelectPlaceDialog))
            {
                OpenEntityPage(_placeIbeacon);
                EditForm();
                MouseOver(PageFooter.AddPlaceSubMenu);
                Click(PageFooter.AddExistingPlaceAsChildButton);
            }

            Assert.IsTrue(IsAlphabeticallySorted(PlacesPage.TableRowTitle, isElementDropDown: false),
                $"Place titles should be in ascending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.TitleColumn, AttrForSorting) == AscOrderAttrValue,
                $"Title column header should contain arrow for ascending order");

            Click(PlacesPage.TitleColumn);
            Assert.IsTrue(IsAlphabeticallySorted(PlacesPage.TableRowTitle, isElementDropDown: false, isReverseOrder: true),
                $"Place titles should be in descending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.TitleColumn, AttrForSorting) == DescOrderAttrValue,
                $"Title column header should contain arrow for descending order");

            //Click(PlacesPage.ItemsColumn);
            //Assert.IsTrue(IsAlphabeticallyOrdered(PlacesPage.TableRowItems, isElementDropDown: false),
            //    $"Places should be sorted by Items in ascending order");
            //Assert.IsTrue(GetElementAttribute(PlacesPage.ItemsColumn, AttrForSorting) == AscOrderAttrValue,
            //    $"Items column header should contain arrow for ascending order");

            //Click(PlacesPage.ItemsColumn);
            //Assert.IsTrue(IsAlphabeticallyOrdered(PlacesPage.TableRowItems, isElementDropDown: false, isReverseOrder: true),
            //    $"Places should be sorted by Items in descending order");
            //Assert.IsTrue(GetElementAttribute(PlacesPage.ItemsColumn, AttrForSorting) == DescOrderAttrValue,
            //    $"Items column header should contain arrow for descending order");

            Click(PlacesPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(PlacesPage.TableRowStatus, _sortSamplePlaces, isReverseOrder: true),
                $"Places should be sorted by Status in descending order: " +
                string.Join(", ", _sortSamplePlaces.Reverse()));
            Assert.IsTrue(GetElementAttribute(PlacesPage.StatusColumn, AttrForSorting) == DescOrderAttrValue,
                $"Status column header should contain arrow for descending order");

            Click(PlacesPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(PlacesPage.TableRowStatus, _sortSamplePlaces),
                $"Places should be sorted by Status in ascending order: " +
                string.Join(", ", _sortSamplePlaces));
            Assert.IsTrue(GetElementAttribute(PlacesPage.StatusColumn, AttrForSorting) == AscOrderAttrValue,
                $"Status column header should contain arrow for ascending order");

            Click(AppsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(PlacesPage.TableRowModified, DateTimeAttrForSorting, isReverseOrder: true),
                $"Modified should be in descending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.ModifiedColumn, AttrForSorting) == DescOrderAttrValue,
                $"Modified column header should contain arrow for descending order");

            Click(AppsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(PlacesPage.TableRowModified, DateTimeAttrForSorting),
                $"Modified should be in ascending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.ModifiedColumn, AttrForSorting) == AscOrderAttrValue,
                $"Modified column header should contain arrow for ascending order");
        }

        [Test, Regression]
        public void RT07130_SelectPlaceAvailability()
        {
            TestStart();
            OpenEntityPage(_placeIbeacon);
            EditForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);

            Click(string.Format(PlacesPage.TableRowByTitle, _placeNoType.Title));
            SubmitForm();
            OpenEntityPage(_placeWw2);
            EditForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);
            Assert.IsTrue(IsElementNotFound(string.Format(PlacesPage.TableRowByTitle, _placeNoType.Title)),
                $"Child place {_placeNoType.Title} should be absent in select place dialog");

            OpenEntityPage(_placeIbeacon);
            EditForm();
            Click(PlacesPage.ChildPlacesSectionTableRowsDeleteButton);
            SubmitForm();
            OpenEntityPage(_placeWw2);
            EditForm();
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(PlacesPage.TableRowByTitle, _placeNoType.Title)),
                $"Child place {_placeNoType.Title} should be present in select place dialog");
        }

        [Test, Regression]
        public void RT07140_SelectAppColumnsAndSorting()
        {
            TestStart();

            OpenEntityPage(_placeWw2);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Assert.IsTrue(IsElementFound(AppsPage.ScreenshotColumn), "Apps list table should have Screenshot column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.TitleColumn), "Apps list table should have Title column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.StatusColumn),
                "Apps list table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.CreatedByColumn),
                "Apps list table should have Created By column");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ModifiedColumn),
                "Apps list table should have Modified column");

            Assert.IsTrue(IsSortedInSpecificOrder(AppsPage.TableRowStatus, _sortSampleApps, isReverseOrder: true),
                "Status column should be in following order by default: " +
                string.Join(", ", _sortSampleApps.Reverse()));

            Click(AppsPage.TitleColumn);
            Assert.IsTrue(IsAlphabeticallySorted(AppsPage.TableRowTitle, isElementDropDown: false),
                $"App titles should be in ascending order");
            Assert.IsTrue(GetElementAttribute(AppsPage.TitleColumn, AttrForSorting) == AscOrderAttrValue,
                $"Title column header should contain arrow for ascending order");

            Click(AppsPage.TitleColumn);
            Assert.IsTrue(IsAlphabeticallySorted(AppsPage.TableRowTitle, isElementDropDown: false, isReverseOrder: true),
                $"App titles should be in descending order");
            Assert.IsTrue(GetElementAttribute(AppsPage.TitleColumn, AttrForSorting) == DescOrderAttrValue,
                $"Title column header should contain arrow for descending order");

            Click(AppsPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(AppsPage.TableRowStatus, _sortSampleApps, isReverseOrder: true),
                $"Apps should be sorted by Status in descending order: " +
                string.Join(", ", _sortSampleApps.Reverse()));
            Assert.IsTrue(GetElementAttribute(AppsPage.StatusColumn, AttrForSorting) == DescOrderAttrValue,
                $"Status column header should contain arrow for descending order");

            Click(AppsPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(AppsPage.TableRowStatus, _sortSampleApps),
                $"Apps should be sorted by Status in ascending order: " +
                string.Join(", ", _sortSampleApps));
            Assert.IsTrue(GetElementAttribute(AppsPage.StatusColumn, AttrForSorting) == AscOrderAttrValue,
                $"Status column header should contain arrow for ascending order");

            Click(AppsPage.CreatedByColumn);
            Assert.IsTrue(IsAlphabeticallySorted(AppsPage.TableRowCreatedBy, isElementDropDown: false),
                $"Created By should be in ascending order");
            Assert.IsTrue(GetElementAttribute(AppsPage.CreatedByColumn, AttrForSorting) == AscOrderAttrValue,
                $"Created By column header should contain arrow for ascending order");

            Click(AppsPage.CreatedByColumn);
            Assert.IsTrue(IsAlphabeticallySorted(AppsPage.TableRowCreatedBy, isElementDropDown: false, isReverseOrder: true),
                $"Created By should be in descending order");
            Assert.IsTrue(GetElementAttribute(AppsPage.CreatedByColumn, AttrForSorting) == DescOrderAttrValue,
                $"Created By column header should contain arrow for descending order");

            Click(AppsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(AppsPage.TableRowModified, DateTimeAttrForSorting, isReverseOrder: true),
                $"Modified should be in descending order");
            Assert.IsTrue(GetElementAttribute(AppsPage.ModifiedColumn, AttrForSorting) == DescOrderAttrValue,
                $"Modified column header should contain arrow for descending order");

            Click(AppsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(AppsPage.TableRowModified, DateTimeAttrForSorting),
                $"Modified should be in ascending order");
            Assert.IsTrue(GetElementAttribute(AppsPage.ModifiedColumn, AttrForSorting) == AscOrderAttrValue,
                $"Modified column header should contain arrow for ascending order");

            ClickAtPoint(CommonElement.Backdrop, 10, 300); // click outside Select App modal window
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.SelectAppDialog), 
                "Select app dialog should be closed on click outside the dialog");
        }

        [Test, Regression]
        public void RT07150_ItemsColumns()
        {
            if (!_isItemsPrepared)
            {
                PrepareItems();
            }
            TestStart();

            OpenItemsPage();
            Assert.IsTrue(IsElementFound(ItemsPage.PictureColumn), "Items list table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TitleColumn), "Items list table should have Title column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TypeColumn), "Items list table should have Type column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.StatusColumn),
                "Items list table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.CreatedColumn),
                "Items list table should have Created column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ModifiedColumn),
                "Items list table should have Modified column");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddItemButton), "Items page should have Add New button");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ShowDeletedButton),
                "Items page should have Show Deleted button");
            Assert.IsTrue(IsElementFound(PageFooter.ImportSubMenu),
                "Items page should have Import submenu in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FollowButtonInactive),
                "Items page should have inactive Follow button in footer");

            Click(PageFooter.ShowDeletedButton);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.HideDeletedButton),
                "Items page should have Hide Deleted button in footer");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TableRowByStatus, StatusDeleted)),
                "Deleted items should be shown in Items list");
        }

        [Test, Regression]
        public void RT07160_ItemListFilter()
        {
            TestStart();
            if (!_isItemsPrepared)
            {
                PrepareItems();
            }

            OpenItemsPage();
            Click(PageFooter.HideDeletedButton, ignoreIfNoElement: true, timeout: 2);
            Assert.IsTrue(IsElementFound(PageHeader.FilterDropDown),
                "Filter should contain item type dropdown inside");

            ClickUntilShown(PageHeader.FilterDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(IsSortedInSpecificOrder(CommonElement.DropDownOptionList, _sortSampleItemTypes),
                "Filter dropdown should contain following item types: " +
                string.Join(", ", _sortSampleItemTypes) +
                ". But contains: " +
                string.Join(", ", GetElementsText(CommonElement.DropDownOptionList)));
            Assert.IsTrue(CountElements(PageHeader.DropDownOptionListWithIcons) == 
                    CountElements(string.Format(CommonElement.DropDownOption, string.Empty)),
                "All Filter dropdown items should have icon");

            DropDownSelect(PageHeader.FilterDropDown, ItemTypeServiceBooking, false);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.TableEmpty), 
                $"Table should not be empty on {ItemTypeServiceBooking} filter");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FollowButton), "Items page should have Follow button");

            DropDownSelect(PageHeader.FilterDropDown, ItemTypeEmployee, false);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TableEmpty),
                $"Table should be empty on {ItemTypeEmployee} filter");

            ClickUntilShown(PageHeader.FilterDropDown, CommonElement.DropDownOptionList);
            SendText(CommonElement.DropDownInput, "PDF");
            Assert.IsTrue(CountElements(CommonElement.DropDownOptionList) == 1 
                          && IsElementFoundQuickly(string.Format(CommonElement.DropDownOption, ItemTypePdfCar)),
                $@"Only '{ItemTypePdfCar}' item type should be shown in Filter dropdown");

            Click(string.Format(CommonElement.DropDownOption, ItemTypePdfCar));
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.TableEmpty),
                $"Table should not be empty on {ItemTypePdfCar} filter");
    
            SetFilter(_itemPdf1.LangJsonData.EnJson.Title);
            WaitTime(1);
            Assert.IsTrue(CountElements(ItemsPage.TableRow) == 1,
                $@"Only item '{ItemTypePdfCar}' with title '{_itemPdf1.LangJsonData.EnJson.Title}' should be shown");

            SetFilter(RandomNumber);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TableEmpty),
                $@"No items '{ItemTypePdfCar}' with non-existing title should be shown");

            OpenAppsPage();
            OpenItemsPage();
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, string.Empty) || IsElementEquals(PageHeader.Filter, "Filter"), 
                "Filter input field should be clean on tabs switch");
            Assert.IsTrue(GetElementAttribute(PageHeader.FilterDropDownValue, "title") == ItemTypePdfCar,
                $@"Current filter item type should be '{ItemTypePdfCar}'");

            Click(ItemsPage.StatusColumn);
            OpenAppsPage();
            OpenItemsPage();
            Assert.IsFalse(string.IsNullOrEmpty(GetElementAttribute(ItemsPage.StatusColumn, AttrForSorting)),
                "Sorting is still active on Status column after tabs switch");

            DropDownSelect(PageHeader.FilterDropDown, ItemTypeAllTypes, false);
            Assert.IsTrue(IsCollectionContainsCollection(_sortSampleItemTypes, GetValuesAsList(ItemsPage.TableRowType).Distinct()),
                "All items with following types should be present in Item list: " +
                string.Join(", ", _filterResultItems));
        }

        [Test, Regression]
        public void RT07170_ItemsColumnsSorting()
        {
            TestStart();
            if (!_isItemsPrepared)
            {
                PrepareItems();
            }
            OpenItemsPage();
            DropDownSelect(PageHeader.FilterDropDown, ItemTypeAllTypes, false);

            Click(ItemsPage.TitleColumn);
            Assert.IsTrue(IsAlphabeticallySorted(ItemsPage.TableRowTitle, isElementDropDown: false),
                $"Item titles should be in ascending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.TitleColumn, AttrForSorting) == AscOrderAttrValue,
                $"Title column header should contain arrow for ascending order");

            Click(ItemsPage.TitleColumn);
            Assert.IsTrue(IsAlphabeticallySorted(ItemsPage.TableRowTitle, isElementDropDown: false, isReverseOrder: true),
                $"Item titles should be in descending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.TitleColumn, AttrForSorting) == DescOrderAttrValue,
                $"Title column header should contain arrow for descending order");

            Click(ItemsPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(ItemsPage.TableRowStatus, _sortSampleItemStatuses, isReverseOrder: true),
                $"Items should be sorted by Status in descending order: " +
                string.Join(", ", _sortSampleItemStatuses.Reverse()));
            Assert.IsTrue(GetElementAttribute(ItemsPage.StatusColumn, AttrForSorting) == DescOrderAttrValue,
                $"Status column header should contain arrow for descending order");

            Click(ItemsPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(ItemsPage.TableRowStatus, _sortSampleItemStatuses),
                $"Items should be sorted by Status in ascending order: " +
                string.Join(", ", _sortSampleItemStatuses));
            Assert.IsTrue(GetElementAttribute(ItemsPage.StatusColumn, AttrForSorting) == AscOrderAttrValue,
                $"Status column header should contain arrow for ascending order");

            Click(ItemsPage.CreatedColumn); 
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(ItemsPage.TableRowCreated, DateTimeAttrForSorting, isReverseOrder: true),
                $"Created should be in descending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.CreatedColumn, AttrForSorting) == DescOrderAttrValue,
                $"Created column header should contain arrow for descending order");

            Click(ItemsPage.CreatedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(ItemsPage.TableRowCreated, DateTimeAttrForSorting),
                $"Created should be in ascending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.CreatedColumn, AttrForSorting) == AscOrderAttrValue,
                $"Created column header should contain arrow for ascending order");

            Click(ItemsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(ItemsPage.TableRowModified, DateTimeAttrForSorting, isReverseOrder: true),
                $"Modified should be in descending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.ModifiedColumn, AttrForSorting) == DescOrderAttrValue,
                $"Modified column header should contain arrow for descending order");

            Click(ItemsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(ItemsPage.TableRowModified, DateTimeAttrForSorting),
                $"Modified should be in ascending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.ModifiedColumn, AttrForSorting) == AscOrderAttrValue,
                $"Modified column header should contain arrow for ascending order");
        }

        [Test, Regression]
        public void RT07180_ItemListFilterByGroup()
        {
            TestStart();
            if (!_isItemsPrepared)
            {
                PrepareItems();
            }
            UnfollowItemTypes(ItemType.Car);
            RefreshPage();

            OpenItemsPage();
            DropDownSelect(PageHeader.FilterDropDown, ItemTypeCars, false);
            Assert.IsTrue(IsCollectionContainsCollection(GetValuesAsList(ItemsPage.TableRowType), _filterResultCars),
                "After Car selection in filter, items with following types should be present in Item list: " +
                string.Join(", ", _filterResultCars));
            Assert.IsTrue(IsElementFound(PageFooter.FollowButton), "Items page should have Follow button");

            Click(PageFooter.FollowButton);
            Assert.IsTrue(IsElementFound(PageFooter.UnfollowButton), "Items page should have Unfollow button after click on Follow");
        }

        [Test, Regression]
        public void RT07190_SelectItemAvailability()
        {
            TestStart();
            if (!_isItemsPrepared)
            {
                PrepareItems();
            }

            OpenEntityPage(_placeIbeacon);
            EditForm();
            Click(PlacesPage.AppsSectionTableRow1);
            Click(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton);
            Assert.IsTrue(IsElementFound(ItemsPage.PictureColumn), "Select item table should have Picture column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TitleColumn), "Select item table should have Title column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TypeColumn), "Select item table should have Type column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.StatusColumn),
                "Select item table should have Status column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.CreatedColumn),
                "Select item table should have Created column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ModifiedColumn),
                "Select item table should have Modified column");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.CancelButton),
                "Select item table should have Cancel button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ClearSelectionButton),
                "Select item table should have Clear Selection button");

            Assert.IsTrue(AreAllElementsContainText(ItemsPage.TableRowStatus, StatusActive),
                $"Select item table should contain items with status {StatusActive} only");
        }

        [Test, Regression]
        public void RT07200_SelectItemColumnSorting()
        {
            TestStart();
            if (!_isItemsPrepared)
            {
                PrepareItems();
            }
            if (IsElementNotFoundQuickly(PlacesPage.SelectItemDialog))
            {
                OpenEntityPage(_placeIbeacon);
                EditForm();
                Click(PlacesPage.AppsSectionTableRow1);
                Click(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton);
            }

            var itemTitle = _itemPorsche2.LangJsonData.EnJson.Title;
            Click(string.Format(ItemsPage.TableRowByTitle, itemTitle));
            Assert.IsTrue(AreElementsContainText(PlacesPage.AppsSectionTableRowDetailsCar, itemTitle),
                $"Apps section: Car field should have item with title {itemTitle}");

            Click(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton);
            Click(ItemsPage.ClearSelectionButton);
            Assert.IsTrue(IsElementEquals(PlacesPage.AppsSectionTableRowDetailsCar, string.Empty),
                "Apps section: Car field should be empty on clear selection");

            Click(PlacesPage.AppsSectionTableRowDetailsCarDetailsButton);
            Click(ItemsPage.TitleColumn);
            Assert.IsTrue(IsAlphabeticallySorted(ItemsPage.TableRowTitle, isElementDropDown: false),
                $"Item titles should be in ascending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.TitleColumn, AttrForSorting) == AscOrderAttrValue,
                $"Title column header should contain arrow for ascending order");

            Click(ItemsPage.TitleColumn);
            Assert.IsTrue(IsAlphabeticallySorted(ItemsPage.TableRowTitle, isElementDropDown: false, isReverseOrder: true),
                $"Item titles should be in descending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.TitleColumn, AttrForSorting) == DescOrderAttrValue,
                $"Title column header should contain arrow for descending order");

            Click(ItemsPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(ItemsPage.TableRowStatus, _sortSampleItemStatuses, isReverseOrder: true),
                $"Items should be sorted by Status in descending order: " +
                string.Join(", ", _sortSampleItemStatuses.Reverse()));
            Assert.IsTrue(GetElementAttribute(ItemsPage.StatusColumn, AttrForSorting) == DescOrderAttrValue,
                $"Status column header should contain arrow for descending order");

            Click(ItemsPage.StatusColumn);
            Assert.IsTrue(IsSortedInSpecificOrder(ItemsPage.TableRowStatus, _sortSampleItemStatuses),
                $"Items should be sorted by Status in ascending order: " +
                string.Join(", ", _sortSampleItemStatuses));
            Assert.IsTrue(GetElementAttribute(ItemsPage.StatusColumn, AttrForSorting) == AscOrderAttrValue,
                $"Status column header should contain arrow for ascending order");

            Click(ItemsPage.CreatedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(ItemsPage.TableRowCreated, DateTimeAttrForSorting, isReverseOrder: true),
                $"Created should be in descending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.CreatedColumn, AttrForSorting) == DescOrderAttrValue,
                $"Created column header should contain arrow for descending order");

            Click(ItemsPage.CreatedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(ItemsPage.TableRowCreated, DateTimeAttrForSorting),
                $"Created should be in ascending order");
            Assert.IsTrue(GetElementAttribute(ItemsPage.CreatedColumn, AttrForSorting) == AscOrderAttrValue,
                $"Created column header should contain arrow for ascending order");

            Click(ItemsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(PlacesPage.TableRowModified, DateTimeAttrForSorting, isReverseOrder: true),
                $"Modified should be in descending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.ModifiedColumn, AttrForSorting) == DescOrderAttrValue,
                $"Modified column header should contain arrow for descending order");

            Click(ItemsPage.ModifiedColumn);
            Assert.IsTrue(IsAlphabeticallySortedByAttribute(PlacesPage.TableRowModified, DateTimeAttrForSorting),
                $"Modified should be in ascending order");
            Assert.IsTrue(GetElementAttribute(PlacesPage.ModifiedColumn, AttrForSorting) == AscOrderAttrValue,
                $"Modified column header should contain arrow for ascending order");

            Click(PlacesPage.CancelButton);
            Click(PageFooter.CancelButton);
        }

        [Test, Regression]
        public void RT07300_FollowItems()
        {
            CurrentTenant = TenantTitle.porsche9699001;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            Parallel.Invoke(
                () => TestStart(),
                () => AddItem(ItemType.EmailTemplate, isAddNew: true),
                () => AddItem(ItemType.CustomerProfile, isAddNew: true)
            );
            UnfollowItemTypes(ItemType.CustomerProfile, ItemType.EmailTemplate);

            OpenItemsPage();
            RefreshPage();
            DropDownSelect(PageHeader.FilterDropDown, ItemTypeCustomerProfile, false);
            ClickUntilShown(PageFooter.FollowButton, PageFooter.UnfollowButton);

            DropDownSelect(PageHeader.FilterDropDown, ItemTypeEmailTemplate, false);
            Assert.IsTrue(IsElementFound(PageFooter.FollowButton),
                "Active Follow button should be shown in footer");

            SetFilter("aa");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FollowButtonInactive),
                "Inactive Follow button should be shown in footer");

            SetFilter(string.Empty);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FollowButton),
                "Active Follow button should be shown in footer");

            ClickUntilShown(PageFooter.FollowButton, PageFooter.UnfollowButton);
            Assert.IsTrue(IsElementFound(PageFooter.UnfollowButton),
                "Active Unfollow button should be shown in footer");

            DropDownSelect(PageHeader.FilterDropDown, ItemTypeCustomerProfile, false);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.UnfollowButton),
                "Active Unfollow button should be shown in footer");

            ClickUntilShown(PageFooter.UnfollowButton, PageFooter.FollowButton);
            Assert.IsTrue(IsElementFound(PageFooter.FollowButton),
                "Active Follow button should be shown in footer");

            var followedModels = GetFollowedItemTypes();
            Assert.IsTrue(followedModels.All(x => x.Id == (int) ItemType.EmailTemplate),
                $"API: Only item type {ItemTypeEmailTemplate} should be followed");

            ChangeTenant(TenantTitle.manylang);
            OpenItemsPage();
            DropDownSelect(PageHeader.FilterDropDown, ItemTypeEmailTemplate, false);
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.FollowButton),
                "Active Follow button should be shown in footer");

            ChangeTenant(TenantTitle.porsche9699001);
            OpenItemsPage();
            Assert.IsTrue(IsElementFound(PageFooter.FollowButton),
                "Active Follow button should be shown in footer");

            DropDownSelect(PageHeader.FilterDropDown, ItemTypeUsedCar, false);
            Assert.IsTrue(IsElementFound(PageFooter.FollowButton),
                "Active Follow button should be shown in footer");
        }

        [Test, Regression]
        public void RT07320_TenantList()
        {
            CurrentTenant = TenantTitle.Root;
            CurrentUser = TestConfig.TenantsListUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin }, TenantTitle.Root);
            TestStart(isSelectTenant: false);

            Assert.IsTrue(IsPageContainsUri(TestConfig.PlacesUri),
                $"Tenant page should not be opened for user {CurrentUser.Email}");
            Assert.IsTrue(IsPageContainsUri(CurrentTenantCode),
                $@"User {CurrentUser.Email} should be logged in to tenant '{_rootTitle}'");

            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin }, TenantTitle.Papa);
            Logout();
            CurrentTenant = TenantTitle.Papa;
            Login(CurrentUser, isPressTenantButton: false);
            Assert.IsTrue(IsPageContainsUri(TestConfig.PlacesUri),
                $"Tenant page should not be opened for user {CurrentUser.Email}");
            Assert.IsTrue(IsPageContainsUri(CurrentTenantCode),
                $@"User {CurrentUser.Email} should be logged in to tenant '{_papaTitle}'");

            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin }, 
                TenantTitle.Root, 
                TenantTitle.Papa);
            Logout();
            CurrentTenant = TenantTitle.Root;
            Login(CurrentUser, isPressTenantButton: false);
            Assert.IsTrue(IsPageContainsUri(TestConfig.TenantsUrl),
                $"Tenant page should be opened on user {CurrentUser.Email} login");

            Assert.IsTrue(
                IsElementFound(string.Format(TenantsPage.TenantRowCollapsed, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be collapsed");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Child tenant row '{_papaTitle}' should not be shown (collapsed parent)");

            Click(string.Format(TenantsPage.CollapseExpandButton, _rootTitle));
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Child tenant row '{_papaTitle}' should be shown (expanded parent)");

            Click(string.Format(TenantsPage.CollapseExpandButton, _rootTitle));
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Child tenant row '{_papaTitle}' should not be shown (collapsed parent again)");

            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableHeaderByName, TitleColumnName)),
                $"{TitleColumnName} column should be present in tenants table");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableHeaderByName, TenantCodeColumnName)),
                $"{TenantCodeColumnName} column should be present in tenants table");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableHeaderByName, CreatedByColumnName)),
                $"{CreatedByColumnName} column should be present in tenants table");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableHeaderByName, ModifiedColumnName)),
                $"{ModifiedColumnName} column should be present in tenants table");

            var userData = UserDirectoryApi.GetUserData(TestConfig.AdminUserDirectory);
            Assert.IsTrue(IsElementEquals(TenantsPage.TableRow1Title, _rootTitle),
                $@"Tenant table row 1 should have Tenant == '{_rootTitle}'");
            Assert.IsTrue(IsElementEquals(TenantsPage.TableRow1TenantCode, CurrentTenantCode),
                $@"Tenant table row 1 should have Tenant Code == '{CurrentTenantCode}'");
            Assert.IsTrue(
                IsElementEquals(TenantsPage.TableRow1CreatedBy, $"{userData.GivenName} {userData.FamilyName}"),
                $@"Tenant table row 1 should have Created By == '{userData.GivenName} {userData.FamilyName}'");

            var tenant = UserDirectoryApi.GetTenant(CurrentTenant);
            if (!tenant.Code.Contains(",2ndcode"))
            {
                tenant.Code = tenant.Code + ",2ndcode";
                UserDirectoryApi.SaveTenant(tenant);
            }
            RefreshPage();
            Assert.IsTrue(IsElementEquals(TenantsPage.TableRow1Title, _rootTitle),
                $@"Tenant table row 1 should have {TitleColumnName} == '{_rootTitle}'");
            Assert.IsTrue(IsElementEquals(TenantsPage.TableRow1TenantCode, CurrentTenantCode),
                $@"Tenant table row 1 should have {TenantCodeColumnName} == '{CurrentTenantCode}'");
            Assert.IsTrue(
                IsElementEquals(TenantsPage.TableRow1CreatedBy, $"{userData.GivenName} {userData.FamilyName}"),
                $@"Tenant table row 1 should have {CreatedByColumnName} == '{userData.GivenName} {userData.FamilyName}'");

            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin }, 
                TenantTitle.Root, 
                TenantTitle.Papa, 
                TenantTitle.Mama, 
                TenantTitle.Child_1);
            RefreshPage();
            Assert.IsTrue(
                IsElementFound(string.Format(TenantsPage.TenantRowCollapsed, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be collapsed");           
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be displayed");

            Click(string.Format(TenantsPage.CollapseExpandButton, _rootTitle));
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be displayed (expanded parent)");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Child tenant row '{_papaTitle}' should be shown (expanded parent)");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be displayed (expanded parent)");

            var tenantChild1 = UserDirectoryApi.GetTenant(TenantTitle.Child_1);
            tenantChild1.ParentId = (int) TenantTitle.Mama;
            UserDirectoryApi.SaveTenant(tenantChild1);
            RefreshPage();
            Assert.IsTrue(
                IsElementFound(string.Format(TenantsPage.TenantRowCollapsed, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be collapsed");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Child tenant row '{_child1Title}' should not be shown (collapsed parent)");

            Click(string.Format(TenantsPage.CollapseExpandButton, _mamaTitle));
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be displayed (expanded parent)");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be displayed (expanded parent)");

            Click(string.Format(TenantsPage.CollapseExpandButton, _rootTitle));
            Assert.IsTrue(
                IsElementFound(string.Format(TenantsPage.TenantRowCollapsed, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be collapsed");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should not be displayed (collapsed root)");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should not be shown (collapsed root)");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should not be displayed (collapsed root)");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.LoginUrl),
                "User should be redirected to login page after tenant selection cancellation");
        }

        [Test, Regression]
        public void RT07330_TenantListIdentsAndSorting()
        {
            UserData userAutoTest = null, userQaAuto = null;
            CurrentTenant = TenantTitle.Mama;
            CurrentUser = TestConfig.AdminUser;
            Parallel.Invoke(
                () => userAutoTest = UserDirectoryApi.GetUserData(TestConfig.AdminUserDirectory),
                () => userQaAuto = UserDirectoryApi.GetUserData(TestConfig.AdminUser),
                () => AddPlaceNoType(PlaceStatus.NoDevice, isAddChild: true, pageToBeOpened: 0, isCreateNewPlace: true),
                () => AddPlaceNoType(PlaceStatus.NoDevice, isAddChild: false, pageToBeOpened: 0, isCreateNewPlace: true),
                () => AddPlaceNoType(PlaceStatus.NoDevice, isAddChild: false, pageToBeOpened: 0, isCreateNewPlace: true)
            );
            CurrentUser = TestConfig.TenantsListUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Root, 
                TenantTitle.Papa, 
                TenantTitle.Mama, 
                TenantTitle.Child_1, 
                TenantTitle.Child_2, 
                TenantTitle.porsche9699001);
            //var tenant = UserDirectoryApi.GetTenant((int) TenantTitle.Child_1);
            //tenant.ParentId = (int) TenantTitle.Mama;
            //UserDirectoryApi.SaveTenant(tenant);
            //tenant = UserDirectoryApi.GetTenant((int) TenantTitle.Child_2);
            //tenant.ParentId = (int) TenantTitle.Mama;
            //UserDirectoryApi.SaveTenant(tenant);
            //tenant = UserDirectoryApi.GetTenant((int) TenantTitle.Mama);
            //tenant.ParentId = (int) TenantTitle.Root;
            //UserDirectoryApi.SaveTenant(tenant);
            //tenant = UserDirectoryApi.GetTenant((int) TenantTitle.Papa);
            //tenant.ParentId = (int) TenantTitle.Root;
            //UserDirectoryApi.SaveTenant(tenant);
            
            TestStart(isSelectTenant: false);
            if (!IsPageRedirectedTo(TestConfig.TenantsUrl))
            {
                NavigateTo(TestConfig.TenantsUrl);
            }
            Assert.IsTrue(IsAlphabeticallySorted(TenantsPage.TableRowTitle, isElementDropDown: false),
                $"{TitleColumnName} should be in ascending order");

            Click(string.Format(TenantsPage.CollapseExpandButton, _rootTitle)); // default asc
            Assert.IsTrue(IsAlphabeticallySorted(
                    string.Format(TenantsPage.ChildrenByParentTitle, _rootTitle), isElementDropDown: false),
                $"Tenant {_rootTitle} children titles should be in ascending order");
            var sample = new []
            {
                _porsche9699001Title,
                _rootTitle,
                _mamaTitle,
                _papaTitle
            };
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowTitle, sample),
                $"{TitleColumnName} should be in ascending order: " + string.Join(", ", sample));

            Click(string.Format(TenantsPage.CollapseExpandButton, _mamaTitle));
            Click(string.Format(TenantsPage.TableHeaderByName, TitleColumnName)); // desc
            sample = new []
            {
                _rootTitle,
                _papaTitle,
                _mamaTitle,
                _child2Title,
                _child1Title,
                _porsche9699001Title
            };
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowTitle, sample),
                $"{TitleColumnName} should be in descending order: " + string.Join(", ", sample));

            var mamaChildrenOriginal = new [] { _child1Title, _child2Title };
            var mamaChildren = GetValuesAsList(string.Format(TenantsPage.ChildrenByParentTitle, _mamaTitle));
            Assert.IsTrue(AreCollectionsEqual(mamaChildren, mamaChildrenOriginal),
                $"Tenant {_mamaTitle} should have children: " + string.Join(", ", mamaChildrenOriginal));
            var rootChildrenOriginal = new [] { _mamaTitle, _papaTitle };
            var rootChildren = GetValuesAsList(string.Format(TenantsPage.ChildrenByParentTitle, _rootTitle));
            Assert.IsTrue(AreCollectionsEqual(rootChildren, rootChildrenOriginal),
                $"Tenant {_rootTitle} should have children: " + string.Join(", ", rootChildrenOriginal));
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.ChildrenByParentTitle, _papaTitle)),
                $"Tenant {_papaTitle} should have no children");

            Click(string.Format(TenantsPage.TableHeaderByName, CreatedByColumnName)); // asc
            Click(string.Format(TenantsPage.TableHeaderByName, CreatedByColumnName)); // desc
            sample = new []
            {
                $"{userQaAuto.GivenName} {userQaAuto.FamilyName}",
                $"{userAutoTest.GivenName} {userAutoTest.FamilyName}",
                $"{userAutoTest.GivenName} {userAutoTest.FamilyName}",
                $"{userAutoTest.GivenName} {userAutoTest.FamilyName}",
                $"{userQaAuto.GivenName} {userQaAuto.FamilyName}",
                $"{userAutoTest.GivenName} {userAutoTest.FamilyName}"
            };
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowCreatedBy, sample),
                $"{CreatedByColumnName} should be in descending order: " + string.Join(", ", sample));

            Click(string.Format(TenantsPage.TableHeaderByName, TitleColumnName));
            Click(string.Format(TenantsPage.TableHeaderByName, TitleColumnName)); // desc
            Assert.IsTrue(
                GetElementAttribute(
                    string.Format(TenantsPage.TableHeaderByName, TitleColumnName), AttrForSorting) == DescOrderAttrValue,
                $"{TitleColumnName} column should be sorted in descending order");
            Assert.IsTrue(
                GetElementAttribute(
                    string.Format(TenantsPage.TableHeaderByName, CreatedByColumnName), AttrForSorting) == null,
                $"{CreatedByColumnName} column should have no sort icon in header");
            
            sample = new []
            {
                _rootTitle,
                _papaTitle,
                _mamaTitle,
                _child2Title,
                _child1Title,
                _porsche9699001Title
            };
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowTitle, sample),
                $"{TitleColumnName} should be in descending order: " + string.Join(", ", sample));

            RefreshPage();
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowTitle, sample),
                $"{TitleColumnName} stay in descending order after page refresh: " + 
                string.Join(", ", sample));
            
            Click(string.Format(TenantsPage.TableRowByTitle, _mamaTitle));
            CurrentTenant = TenantTitle.Mama;
            Assert.IsTrue(IsPageContainsUri($"{CurrentTenantCode}{TestConfig.PlacesUri}"),
                $"Places page should be opened on tenant {_mamaTitle} click");

            Assert.IsTrue(
                GetElementAttribute(PlacesPage.TableHeader, AttrForSorting) == AscOrderAttrValue,
                $"Title column should be sorted in ascending order");

            NavigateTo(TestConfig.TenantsUrl);
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowTitle, sample),
                $"{TitleColumnName} stay in descending order after return from Places: " +
                string.Join(", ", sample));

            OpenNewTab();
            var tabHandles = GetTabHandles();
            NavigateTo(TestConfig.TenantsUrl);
            Assert.IsTrue(IsPageContainsUri(TestConfig.TenantsUrl),
                "Tenants page should be opened in new browser tab");
            
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowTitle, sample),
                $"{TitleColumnName} should stay in descending order after open in new " +
                "browser tab: " + string.Join(", ", sample));
            CloseTab(tabHandles.Last());
        }

        [Test, Regression]
        public void RT07340_TenantListFilter()
        {
            const string newTenantTitle = "Mama Children";
            CurrentTenant = TenantTitle.Mama;
            CurrentUser = TestConfig.TenantsListUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Root,
                TenantTitle.Papa,
                TenantTitle.Mama,
                TenantTitle.Child_1,
                TenantTitle.Child_2,
                TenantTitle.porsche9699001);

            TestStart(isSelectTenant: false);
            SetFilter("child");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter applied)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be shown (filter applied)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be shown (filter applied)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should not be shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should not be shown (filter applied)");

            SetFilter(string.Empty);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be not shown (filter cleared)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be not shown (filter cleared)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be not shown (filter cleared)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should be not shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be shown (filter cleared)");

            Click(string.Format(TenantsPage.CollapseExpandButton, _rootTitle));
            Click(string.Format(TenantsPage.CollapseExpandButton, _mamaTitle));
            SetFilter("papa");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should be shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be not shown (filter applied)");

            SetFilter(string.Empty);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be shown (filter cleared)");

            SetFilter("child1");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter applied)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be shown (filter applied)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be not shown (filter applied)");

            SetFilter("child_2");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter applied)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be not shown (filter applied)");

            RefreshPage();
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, "child_2"), 
                "Filter value should be kept on page refresh");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter applied and page refreshed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be shown (filter applied and page refreshed)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be not shown (filter applied and page refreshed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be shown (filter applied and page refreshed)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should be not shown (filter applied and page refreshed)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be not shown (filter applied and page refreshed)");

            Click(string.Format(TenantsPage.TableRowByTitle, _child2Title));
            IsPageContainsUri(TestConfig.PlacesUri); // check that user has switched to places list before come back
            NavigateTo(TestConfig.TenantsUrl);
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, string.Empty) 
                          || IsElementEquals(PageHeader.Filter, "Filter"),
                "Filter value should be cleared when returned from Places page");
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be shown (filter cleared)");

            var tenant = UserDirectoryApi.GetTenant(TenantTitle.Mama);
            tenant.Title = newTenantTitle;
            UserDirectoryApi.SaveTenant(tenant);
            NavigateTo(TestConfig.TenantsUrl);
            RefreshPage();
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, newTenantTitle)),
                $@"Tenant row '{newTenantTitle}' should be shown (tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be shown (tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be shown (tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should be shown (tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be shown (tenant {_mamaTitle} renamed)");

            SetFilter("child");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter applied, tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, newTenantTitle)),
                $@"Tenant row '{newTenantTitle}' should be shown (filter applied, tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be shown (filter applied, tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be shown (filter applied, tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should not be shown (filter applied, tenant {_mamaTitle} renamed)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should not be shown (filter applied, tenant {_mamaTitle} renamed)");

            Click(string.Format(TenantsPage.TableHeaderByName, TitleColumnName));
            var sample = new []
            {
                _rootTitle,
                newTenantTitle,
                _child2Title,
                _child1Title
            };
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowTitle, sample),
                $"{TitleColumnName} should be in descending order: " + string.Join(", ", sample));

            SetFilter(string.Empty);
            sample = new []
            {
                _rootTitle,
                _papaTitle,
                newTenantTitle,
                _child2Title,
                _child1Title,
                _porsche9699001Title
            };
            Assert.IsTrue(IsSortedInSpecificOrder(TenantsPage.TableRowTitle, sample),
                $"{TitleColumnName} should be in descending order: " + string.Join(", ", sample));

            SetFilter("child23");
            Assert.IsTrue(IsElementFound(TenantsPage.TableEmpty), "Tenant table should be empty on wrong filter");

            SetFilter("969");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, newTenantTitle)),
                $@"Tenant row '{newTenantTitle}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be not shown (filter applied)");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should not be shown (filter applied)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be shown (filter applied)");

            Click(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title));
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, string.Empty)
                          || IsElementEquals(PageHeader.Filter, "Filter"),
                "Filter value should be cleared when returned from Places page");

            SetFilter("dgfsdgdfgj");
            Click(PageHeader.BreadCrumbTenants);
            Assert.IsTrue(IsElementEquals(PageHeader.Filter, string.Empty)
                          || IsElementEquals(PageHeader.Filter, "Filter"),
                "Filter value should be cleared when returned from Places page");
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $@"Tenant row '{_rootTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, newTenantTitle)),
                $@"Tenant row '{newTenantTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $@"Tenant row '{_child2Title}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Tenant row '{_papaTitle}' should be shown (filter cleared)");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _porsche9699001Title)),
                $@"Tenant row '{_porsche9699001Title}' should be shown (filter cleared)");
        }

        [Test, Regression]
        public void RT07350_TenantListAccess()
        {
            CurrentTenant = TenantTitle.Root;
            CurrentUser = TestConfig.TenantsListUser;
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Root,
                TenantTitle.Papa,
                TenantTitle.Mama,
                TenantTitle.Child_1,
                TenantTitle.Child_2,
                TenantTitle.porsche9699001);

            TestStart();
            Assert.IsTrue(IsPageContainsUri(TestConfig.PlacesUri),
                $@"Places page should be opened on tenant '{_rootTitle}'");

            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Papa,
                TenantTitle.Mama,
                TenantTitle.Child_1,
                TenantTitle.Child_2,
                TenantTitle.porsche9699001);
            RefreshPage();
            NavigateTo(TestConfig.TenantsUrl);
            Click(TenantsPage.OkButton, ignoreIfNoElement: true, timeout: 2);
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowDisabledByTitle, _rootTitle)),
                $"Tenant {_rootTitle} should be shown disabled");

            Click(string.Format(TenantsPage.TableRowByTitle, _rootTitle));
            Assert.IsTrue(IsPageContainsUri(TestConfig.TenantsUrl),
                $"Click on disabled tenant {_rootTitle} should not open Places page");

            Click(string.Format(TenantsPage.CollapseExpandButton, _rootTitle));
            Click(string.Format(TenantsPage.CollapseExpandButton, _mamaTitle));
            Click(string.Format(TenantsPage.TableRowByTitle, _child1Title));
            CurrentTenant = TenantTitle.Child_1;
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlacesUri}"),
                $"Click on tenant {_child1Title} should open Places page");

            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Papa,
                TenantTitle.Mama,
                TenantTitle.Child_2,
                TenantTitle.porsche9699001);
            RefreshPage();
            NavigateTo(TestConfig.TenantsUrl);
            if (IsElementFoundQuickly(TenantsPage.YouDontHaveAccessDialog))
            {
                Click(TenantsPage.OkButton);
            }
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _child1Title)),
                $@"Tenant row '{_child1Title}' should be not shown (no access anymore)");

            var tenant = UserDirectoryApi.GetTenant(TenantTitle.Papa);
            tenant.Status = new Status {Key = 0, Value = "Disabled"};
            UserDirectoryApi.SaveTenant(tenant);
            RefreshPage();
            Assert.IsTrue(IsElementNotFound(string.Format(TenantsPage.TableRowByTitle, _papaTitle)),
                $@"Disabled tenant's row '{_papaTitle}' should not be shown");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(TenantsPage.TableRowByTitle, _mamaTitle)),
                $@"Tenant row '{_mamaTitle}' should be shown (disabled {_papaTitle} tenant)");
            // restore tenant status
            tenant = UserDirectoryApi.GetTenant(TenantTitle.Papa);
            tenant.Status = new Status { Key = 1, Value = "Active" };
            UserDirectoryApi.SaveTenant(tenant);

            Click(string.Format(TenantsPage.TableRowByTitle, _mamaTitle));
            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Papa,
                TenantTitle.Child_2,
                TenantTitle.porsche9699001);
            RefreshPage();
            NavigateTo(TestConfig.TenantsUrl);
            if (IsElementFoundQuickly(TenantsPage.YouDontHaveAccessDialog))
            {
                Click(TenantsPage.OkButton);
            }
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowDisabledByTitle, _rootTitle)),
                $"Tenant {_rootTitle} should be shown disabled");
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowDisabledByTitle, _mamaTitle)),
                $"Tenant {_mamaTitle} should be shown disabled");
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $"Tenant {_child2Title} should be shown");

            UserDirectoryApi.AssignRolesToUser(CurrentUser, new [] { UserRole.CxmAdmin },
                TenantTitle.Root,
                TenantTitle.Papa,
                TenantTitle.Child_2,
                TenantTitle.porsche9699001);
            RefreshPage();
            if (IsElementFoundQuickly(TenantsPage.YouDontHaveAccessDialog))
            {
                Click(TenantsPage.OkButton);
            }
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowDisabledByTitle, _mamaTitle)),
                $"Tenant {_mamaTitle} should be shown disabled");
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $"Tenant {_rootTitle} should be shown");
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowByTitle, _child2Title)),
                $"Tenant {_child2Title} should be shown");

            Click(string.Format(TenantsPage.TableRowByTitle, _mamaTitle));
            Assert.IsTrue(IsPageContainsUri(TestConfig.TenantsUrl),
                $"Click on disabled tenant {_mamaTitle} should not open Places page");

            Click(string.Format(TenantsPage.CollapseExpandButton, _rootTitle));
            Assert.IsTrue(IsElementFound(string.Format(TenantsPage.TableRowByTitle, _rootTitle)),
                $"Tenant {_rootTitle} should be shown");

            CurrentTenant = TenantTitle.Root;
            Click(string.Format(TenantsPage.TableRowByTenantCode, CurrentTenantCode));
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.PlacesUri}"),
                $@"Click on tenant code '{CurrentTenantCode}' should open Places page");
        }

        [TearDown]
        public async Task TearDown()
        {
            var task = TestEnd();
            var tenant = UserDirectoryApi.GetTenant(TenantTitle.Mama);
            tenant.Title = TenantTitle.Mama.ToString();
            UserDirectoryApi.SaveTenant(tenant);
            await task;
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
