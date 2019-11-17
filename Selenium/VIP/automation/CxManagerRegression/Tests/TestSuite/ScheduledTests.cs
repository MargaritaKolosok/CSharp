using System;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Models.Items;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class ScheduledTests : TestHelper
    {
        private readonly MailManager _mm = new MailManager();

        [OneTimeSetUp]
        public async Task BeginFixtureTests()
        {
            await _mm.ConnectMailServerAsync(UserType.AdminCxM).ConfigureAwait(false);
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }
        }

        [Test, Scheduled]
        // daily report emails send at 8:00 AM (Europe/Kiev),
        // this test needs some time to prepare environment, so run it before 7:59 AM
        public void RT07310_FollowItemsEmailReport()
        {
            if (DateTime.Now.TimeOfDay > new TimeSpan(07, 59, 00))
            {
                Assert.Inconclusive("Canceled run. The test should be started before 07:59 AM");
            }
            CurrentTenant = TenantTitle.reportitems;
            var userProperties = UserDirectoryApi.GetUserData(TestConfig.NewUser);
            if (userProperties == null)
            {
                AccountApi.CreateNewAccount(TestConfig.NewUser);
                UserDirectoryApi.SetUserStatus(TestConfig.NewUser, UserStatus.Active);
                UserDirectoryApi.AssignRolesToUser(TestConfig.NewUser, 
                    new [] { UserRole.CxmAdmin });
                userProperties = UserDirectoryApi.GetUserData(TestConfig.NewUser);
            }
            CurrentUser = TestConfig.NewUser;
            const string itemTitle = "Daily report template";
            var itemsReportEmailSubject = $"Items report {CurrentTenantCode}";
            Item item = null, custItem1 = null, custItem2 = null;
            var app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1], true);
            Parallel.Invoke(
                () => item = ItemApi.SearchItem(itemTitle),
                () => AddPlaceNoType(PlaceStatus.NoDevice, isAddChild: false, pageToBeOpened: 0, 
                    isCreateNewPlace: true),
                () => AddItem(ItemType.Employee),
                () => AddItem(ItemType.CustomerProfile, isAddNew: true),
                () => AddItem(ItemType.Car),
                () => custItem1 = AddItem(ItemType.CustomerProfile, isAddNew: true),
                () => custItem2 = AddItem(ItemType.CustomerProfile, isAddNew: true),
                () => ItemApi.FollowItems(ItemType.CustomerProfile),
                () => ItemApi.FollowItems(ItemType.Car),
                () => ItemApi.FollowItems(ItemType.Employee)
            );
            Assert.IsNotNull(item,
                $@"Item '{itemTitle}' not found. Is iBeacon app imported and available on tenant " + 
                $"{CurrentTenantCode}?");
            AddItemToIbeaconApp(app, "$.texts.emails.itemsFollowReportTemplate", item);

            Parallel.Invoke(
                () => _mm.InboxHousekeeping(_mm.ClientCxM),
                () => ItemApi.DeleteItem(custItem1.Id),
                () => ItemApi.SaveItem(custItem2),
                () => AddItem(ItemType.PorscheCar, isAddNew: true),
                () => AddItem(ItemType.CustomerProfile, isAddNew: true),
                () => AddItem(ItemType.UsedCar, isAddNew: true),
                () => AddItem(ItemType.ServiceBooking, isAddNew: true)
            );

            // it's time to start checking mail box
            WaitForTimeOfDay(new TimeSpan(08, 00, 00));
            // expects a new email within 600 seconds
            var gotNewMail = WaitForNewMail(_mm.ClientCxM, 600);
            Assert.IsTrue(gotNewMail, $"There is no new daily report found in mailbox {TestConfig.MailServerLogin}");

            var hasUserName = _mm.IsMailBodyContainsText(
                    _mm.ClientCxM, 
                    itemsReportEmailSubject, 
                    $"{userProperties.GivenName} {userProperties.FamilyName}");
            Assert.IsTrue(hasUserName, "Item report email has no user firstname or lastname");

            var tenantTitle = CurrentTenant.ToString();
            var hasTenantTitle = _mm.IsMailBodyContainsText(
                    _mm.ClientCxM,
                    itemsReportEmailSubject,
                    tenantTitle);
            Assert.IsTrue(hasTenantTitle, $@"Item report email has no tenant title '{tenantTitle}'");

            var custProfileContent = $"{ItemTypeCustomerProfile}: 1 new {ItemTypeCustomerProfile}, " +
                                     $"1 updated {ItemTypeCustomerProfile}, 1 deleted {ItemTypeCustomerProfile}";
            var hasCustProfileInfo = _mm.IsMailBodyContainsText(
                    _mm.ClientCxM,
                    itemsReportEmailSubject,
                    custProfileContent);
            Assert.IsTrue(hasCustProfileInfo, $@"Item report email has no text: '{custProfileContent}'");

            var hasPorscheCarsAndUsedCars = _mm.IsMailBodyContainsText(
                    _mm.ClientCxM,
                    itemsReportEmailSubject,
                    $"{ItemTypePorscheCar}s, {ItemTypeUsedCar}s");
            Assert.IsTrue(hasPorscheCarsAndUsedCars, 
                "Item report email has no info about updated item types: " +
                $@"'{ItemTypePorscheCar}s, {ItemTypeUsedCar}s'");

            var hasPorscheCarsInfo = _mm.IsMailBodyContainsText(
                    _mm.ClientCxM,
                    itemsReportEmailSubject,
                    $"{ItemTypePorscheCar}: 1 new {ItemTypePorscheCar}");
            Assert.IsTrue(hasPorscheCarsInfo,
                $@"Item report email has no info about followed item: '{ItemTypePorscheCar}: 1 new " + 
                $@"{ItemTypePorscheCar}'");

            var hasUsedCarsInfo = _mm.IsMailBodyContainsText(
                    _mm.ClientCxM,
                    itemsReportEmailSubject,
                    $"{ItemTypeUsedCar}: 1 new {ItemTypeUsedCar}");
            Assert.IsTrue(hasUsedCarsInfo,
                $@"Item report email has no info about followed item: '{ItemTypeUsedCar}: 1 new " + 
                $@"{ItemTypeUsedCar}'");

            var hasNoEmployeeInfo = _mm.IsMailBodyContainsText(
                    _mm.ClientCxM,
                    itemsReportEmailSubject,
                    ItemTypeEmployee);
            Assert.IsTrue(hasNoEmployeeInfo,
                $"Item report email must contain no {ItemTypeEmployee} items info");

            var hasNoServiceBookingInfo = _mm.IsMailBodyContainsText(
                    _mm.ClientCxM,
                    itemsReportEmailSubject,
                    ItemTypeServiceBooking);
            Assert.IsTrue(hasNoServiceBookingInfo,
                $"Item report email must contain no {ItemTypeServiceBooking} items info");
        }

        [OneTimeTearDown]
        public async Task EndFixtureTests()
        {
            if (TestContext.Parameters.Count == 0)
            {
                PlaceApi.DeletePlaces();
                AppApi.DeleteApps();
                ItemApi.DeleteItems();
            }
            await _mm.InboxHousekeepingAsync(_mm.ClientCxM).ContinueWith(task => _mm.Dispose());
        }
    }
}
