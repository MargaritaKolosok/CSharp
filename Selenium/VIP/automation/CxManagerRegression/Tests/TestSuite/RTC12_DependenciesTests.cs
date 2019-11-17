using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.Apps;
using Models.Items;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC12_DependenciesTests : ParentTest
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

            CurrentTenant = TenantTitle.onelang;
            CurrentUser = TestConfig.AdminUser;
        }

        [Test, Regression]
        public void RT12010_ItemTypesModels()
        {
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.DptMobileAppFolder,
                TestConfig.DptMobileAppFile, TestConfig.DptAppVersions[1]);
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsEditMode(),
                $"DPT app v.{TestConfig.DptAppVersions[1]} should be imported without {AppTitle.IpadPlayer} " +
                "and in edit mode");
            var allPermissions = UserDirectoryApi.GetSupportedPermissions();
            var role = UserDirectoryApi.GetRole((long) UserRole.CxmAdmin);
            UserDirectoryApi.SetRolePermissions(role, allPermissions);
            RefreshPage();
            SendText(AppsPage.Market, "AA");
            SubmitForm();
            Assert.IsTrue(IsViewMode(),
                $"DPT app v.{TestConfig.DptAppVersions[1]} should be in view mode");

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            SendText(ItemsPage.Vin, $"{RandomNumber}55");
            SendText(ItemsPage.Title, $"Auto test {RandomNumber}");
            ClickUntilShown(ItemsPage.FinanceOfferButton, ItemsPage.FinanceOfferOptionDropDown);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.FinanceOfferOptionDropDown), 
                "Finance Offer > Option drop-down should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.FinanceOfferPrepayment),
                "Finance Offer > Prepayment should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.FinanceOfferTerm),
                "Finance Offer > Term should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.FinanceOfferMonthlyRate),
                "Finance Offer > Monthly Rate should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.FinanceOfferDisclaimer),
                "Finance Offer > Disclaimer should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(CommonElement.ValidationError),
                "Finance Offer > No validation errors should be shown");

            DropDownSelect(ItemsPage.FinanceOfferOptionDropDown, "Porsche Value S");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferPrepaymentRequired),
                @"Finance Offer > Error 'Prepayment required' should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferTermRequired),
                @"Finance Offer > Error 'Term required' should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferMonthlyRateRequired),
                @"Finance Offer > Error 'Monthly required' Rate should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferDisclaimerRequired),
                @"Finance Offer > Error 'Disclaimer required' should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferOptionRequired),
                @"Finance Offer > Error for Option drop-down should be not shown");

            SendText(ItemsPage.FinanceOfferPrepayment, "A");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferPrepaymentRequired),
                @"Finance Offer > Error 'Prepayment required' should be not shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferTermRequired),
                @"Finance Offer > Error 'Term required' should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferMonthlyRateRequired),
                @"Finance Offer > Error 'Monthly required' Rate should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferDisclaimerRequired),
                @"Finance Offer > Error 'Disclaimer required' should be shown");

            SendText(ItemsPage.FinanceOfferTerm, "A");
            SendText(ItemsPage.FinanceOfferMonthlyRate, "A");
            SendText(ItemsPage.FinanceOfferDisclaimer, "A");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferPrepaymentRequired),
                @"Finance Offer > Error 'Prepayment required' should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferTermRequired),
                @"Finance Offer > Error 'Term required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferMonthlyRateRequired),
                @"Finance Offer > Error 'Monthly required' Rate should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferDisclaimerRequired),
                @"Finance Offer > Error 'Disclaimer required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferOptionRequired),
                @"Finance Offer > Error for Option drop-down should be not shown");

            ClearTextInElement(ItemsPage.FinanceOfferTerm);
            ClearTextInElement(ItemsPage.FinanceOfferDisclaimer);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferTermRequired),
                @"Finance Offer > Error 'Term required' should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferDisclaimerRequired),
                @"Finance Offer > Error 'Disclaimer required' should be shown");

            ClearTextInElement(ItemsPage.FinanceOfferPrepayment);
            ClearTextInElement(ItemsPage.FinanceOfferMonthlyRate);
            DropDownSelect(ItemsPage.FinanceOfferOptionDropDown, string.Empty);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferPrepaymentRequired),
                @"Finance Offer > Error 'Prepayment required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferTermRequired),
                @"Finance Offer > Error 'Term required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferMonthlyRateRequired),
                @"Finance Offer > Error 'Monthly required' Rate should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferDisclaimerRequired),
                @"Finance Offer > Error 'Disclaimer required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferOptionRequired),
                @"Finance Offer > Error for Option drop-down should be not shown");

            SendText(ItemsPage.FinanceOfferDisclaimer, "A");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferPrepaymentRequired),
                @"Finance Offer > Error 'Prepayment required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferTermRequired),
                @"Finance Offer > Error 'Term required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferMonthlyRateRequired),
                @"Finance Offer > Error 'Monthly required' Rate should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferDisclaimerRequired),
                @"Finance Offer > Error 'Disclaimer required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferOptionRequired),
                @"Finance Offer > Error for Option drop-down should be not shown");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be successfully saved");
            var itemId = GetEntityIdFromUrl();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            pathFile = FileManager.GetFileByVersion(TestConfig.DptMobileAppFolder,
                TestConfig.DptMobileAppFile, TestConfig.DptAppVersions[2]);
            FileManager.UploadAsBackgroundTask(pathFile);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), 
                $"DPT app v.{TestConfig.DptAppVersions[2]} should be in view mode");

            RefreshPage();
            OpenEntityPage<Item>(itemId);
            EditForm();
            ClickUntilShown(ItemsPage.FinanceOfferButton, ItemsPage.FinanceOfferOptionDropDown);
            DropDownSelect(ItemsPage.FinanceOfferOptionDropDown, "Porsche Value S");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferPrepaymentRequired),
                @"Finance Offer > Error 'Prepayment required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferTermRequired),
                @"Finance Offer > Error 'Term required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferMonthlyRateRequired),
                @"Finance Offer > Error 'Monthly required' Rate should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferDisclaimerRequired),
                @"Finance Offer > Error 'Disclaimer required' should be not shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferOptionRequired),
                @"Finance Offer > Error for Option drop-down should be not shown");

            SendText(ItemsPage.FinanceOfferPrepayment, "A");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferPrepaymentRequired),
                @"Finance Offer > Error 'Prepayment required' should be not shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferTermRequired),
                @"Finance Offer > Error 'Term required' should be shown");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorFinanceOfferMonthlyRateRequired),
                @"Finance Offer > Error 'Monthly required' Rate should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFinanceOfferDisclaimerRequired),
                @"Finance Offer > Error 'Disclaimer required' should be not shown");

            Click(PageFooter.CancelButton);
        }

        [Test, Regression]
        public void RT12020_AppMasterData()
        {
            AddAppIpadPlayer();
            var app = AddAppDpt(AppStatus.Any, TestConfig.DptAppVersions[1]);
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.TestDependenciesDptAppFolder,
                TestConfig.TestDependenciesDptAppFile,
                TestConfig.TestDependenciesMdVersions[0]);
            FileManager.UploadAsBackgroundTask(pathFile);
            Assert.IsTrue(IsElementFound(AppsPage.AppTitle),
                $@"App '{AppTitle.TestDependenciesMd}' has not been imported");
            var allPermissions = UserDirectoryApi.GetSupportedPermissions();
            var role = UserDirectoryApi.GetRole((long) UserRole.CxmAdmin);
            UserDirectoryApi.SetRolePermissions(role, allPermissions);
            RefreshPage();

            const string market = "CC";
            SendText(AppsPage.Market, market);
            SubmitForm();
            Assert.IsTrue(IsViewMode(),
                $@"'{AppTitle.TestDependenciesMd}' app should be in view mode on submit");

            Click(PageFooter.AddVersionButton);
            pathFile = FileManager.GetFileByVersion(TestConfig.TestDependenciesDptAppFolder,
                TestConfig.TestDependenciesDptAppFile,
                TestConfig.TestDependenciesMdVersions[1]);
            FileManager.UploadAsBackgroundTask(pathFile);
            Assert.IsTrue(IsElementEquals(AppsPage.Market, market),
                $@"Market field should kept value '{market}' on 2nd app version upload");
            Assert.IsTrue(IsElementNotFoundQuickly(CommonElement.ValidationError),
                "Page should have no validation errors on 2nd app version upload");
            
            SendText(AppsPage.Area, "a");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorArea),
                "Area should have no validation errors");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorRegion),
                "Region should have validation error");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorMarket),
                "Market should have no validation errors");

            ClearTextInElement(AppsPage.Market);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorArea),
                "Area should have no validation errors");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorRegion),
                "Region should have validation error");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMarket),
                "Market should have validation error");

            ClearTextInElement(AppsPage.Area);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorArea),
                "Area should have no validation errors");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorRegion),
                "Region should have no validation errors");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMarket),
                "Market should have validation error");

            SendText(AppsPage.Market, market);
            SendText(AppsPage.Area, "a");
            SendText(AppsPage.Region, "a");
            SubmitForm();
            Assert.IsTrue(IsViewMode(),
                $"{AppTitle.TestDependenciesMd} app should be in view mode on submit");
            var appId = GetEntityIdFromUrl();

            OpenEntityPage(app);
            EditForm();
            SendText(AppsPage.Area, "a");
            Assert.IsTrue(IsElementNotFoundQuickly(CommonElement.ValidationError),
                $"App {AppTitle.Dpt} page should have no validation errors");

            Click(PageFooter.CancelButton);
            OpenEntityPage<AppResponse>(appId);
            Click(AppsPage.Versions);
            Click(string.Format(AppsPage.TableRowByText, TestConfig.TestDependenciesMdVersions[1]));
            Click(PageFooter.DeleteButton);
            Click(AppsPage.DeleteButton);
            Click(PageHeader.NavigateBackButton);
            EditForm();
            ClearTextInElement(AppsPage.Region);
            Assert.IsTrue(IsElementNotFoundQuickly(CommonElement.ValidationError),
                $"App {AppTitle.Dpt} page should have no validation errors after the latest version delete");
            
            SubmitForm();
            Assert.IsTrue(IsViewMode(),
                $"{AppTitle.TestDependenciesMd} app should be in view mode after the latest version delete and submit");
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
