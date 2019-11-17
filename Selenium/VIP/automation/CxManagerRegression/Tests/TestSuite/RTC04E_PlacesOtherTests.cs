using System;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC04E_PlacesOtherTests : ParentTest
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
        }      

        [Test, Regression]
        public void RT04440_2UsersSimultaneousChanges()
        {
            TestStart();
            var place = AddPlaceNoType(PlaceStatus.Any, isCreateNewPlace: true);

            // 2
            OpenSecondaryBrowser();
            NavigateAndLogin(TestConfig.BaseUrl, TestConfig.AdminUser2);
            OpenEntityPage(place);
            Assert.IsTrue(IsElementFound(PlacesPage.TitleReadOnly));

            // 1
            SwitchToAnotherBrowser();
            EditForm();
            SendText(PlacesPage.PlaceWidth, "10");
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(PlacesPage.DialogPlaceholder), "No error dialogs should be displayed");
            Assert.IsTrue(IsElementEquals(PlacesPage.PlaceWidthReadOnly, "10 m"), "Place Width field value should be saved");

            // 2
            SwitchToAnotherBrowser();
            EditForm();
            SendText(PlacesPage.ToleranceRadius, "20");
            SubmitForm();
            Assert.IsTrue(IsElementFound(PlacesPage.DeviceHasBeenUpdatedDialog),
                @"Dialog 'The Device has been updated by another user...' should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.OkButton),
                @"Dialog 'The Device has been updated by another user...' should contain OK button");

            Click(PlacesPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.DeviceHasBeenUpdatedDialog),
                @"Dialog 'The Device has been updated by another user...' should be closed");
            Assert.IsTrue(IsViewMode(), "Place should be in view mode");
            Assert.IsFalse(IsElementEquals(PlacesPage.ToleranceRadiusReadOnly, "20 m"),
                "Tolerance Radius field new value should not be saved");

            EditForm();
            SendText(PlacesPage.ToleranceRadius, "20");
            SubmitForm();
            Assert.IsTrue(IsElementEquals(PlacesPage.ToleranceRadiusReadOnly, "20 m"),
                "Tolerance Radius field new value should be saved");
        }

        [Test, Regression]
        public void RT04450_CheckPlaceTimestamps()
        {
            TestStart();
            var place = AddPlaceNoType(PlaceStatus.Active, isCreateNewPlace: true);
            EditForm();
            Click(PlacesPage.PlaceMapUploadButton);
            FileManager.Upload(TestConfig.Image138);
            SendText(PlacesPage.PlaceWidth, "10");
            SendText(PlacesPage.ToleranceRadius, "20");
            SendText(PlacesPage.Position, "50.3333, 40.5555");
            SubmitForm();
            var positionParent = GetValue(PlacesPage.PositionReadOnly);
            var toleranceRadiusParent = GetValue(PlacesPage.ToleranceRadiusReadOnly);          

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeIbeacon);
            var placeIbeaconTitle = $"Auto test {RandomNumber}";
            SendText(PlacesPage.Title, placeIbeaconTitle);
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneKiev);
            OpenDevicesFromPlace();
            Click(Devices.TableRow); // any existing device
            SubmitForm();
            var created1 = GetTimestamp(PlacesPage.Created, true);
            var modified1 = GetTimestamp(PlacesPage.Modified);
            var since1 = GetTimestamp(PlacesPage.Since);
            Assert.IsTrue(Math.Abs((created1.Item1 - modified1.Item1).Seconds) <= 10, 
                "Created timestamp data should be equal to Modified field data");
            Assert.IsTrue(Math.Abs((created1.Item1 - since1.Item1).Seconds) <= 10, 
                "Since should display a timestamp that equals Created and Modified");
            var userProperties = UserDirectoryApi.GetUserData(TestConfig.AdminUser);
            var userName = $" ({userProperties.GivenName} {userProperties.FamilyName})";
            Assert.IsTrue(created1.Item2.Contains(userName), 
                "Created should contain current user firstname and lastname");
            Assert.IsTrue(modified1.Item2.Contains(userName), 
                "Modified should contain current user firstname and lastname");

            WaitTime(2);
            EditForm();
            var newTitle = $"Auto test {RandomNumber}";
            SendText(PlacesPage.Title, newTitle);
            SubmitForm();
            var created2 = GetTimestamp(PlacesPage.Created, true);
            var modified2 = GetTimestamp(PlacesPage.Modified);
            var since2 = GetTimestamp(PlacesPage.Since);
            Assert.IsTrue(created1.Equals(created2), "Created timestamp and user data should never change");
            Assert.IsTrue(since1.Item1 == since2.Item1, "Since should not change after this submit");
            Assert.IsTrue(modified2.Item1 > modified1.Item1, 
                "After submit Modified should be equal to current time");

            WaitTime(2);
            EditForm();
            OpenDevicesFromPlace();
            Click(Devices.ClearSelectionButton);
            SubmitForm();
            var created3 = GetTimestamp(PlacesPage.Created, true);
            var modified3 = GetTimestamp(PlacesPage.Modified);
            var since3 = GetTimestamp(PlacesPage.Since);
            Assert.IsTrue(created1.Equals(created3), "Created timestamp and user data should never change");
            Assert.IsTrue(modified3.Item1 > modified2.Item1, 
                "After submit Modified should be equal to current time");
            Assert.IsTrue(Math.Abs((modified3.Item1 - since3.Item1).Seconds) <= 10, 
                "After submit Since should be equal to current time");
            Assert.IsTrue(modified3.Item2.Contains(userName), 
                "Modified should contain current user firstname and lastname");

            WaitTime(2);
            EditForm();
            OpenDevicesFromPlace();
            Click(Devices.TableRow); // any existing device
            SubmitForm();
            var created4 = GetTimestamp(PlacesPage.Created, true);
            var modified4 = GetTimestamp(PlacesPage.Modified);
            var since4 = GetTimestamp(PlacesPage.Since);
            Assert.IsTrue(created1.Equals(created4), "Created timestamp and user data should never change");
            Assert.IsTrue(modified4.Item1 > modified3.Item1, 
                "After submit Modified should be equal to current time");
            Assert.IsTrue(since4.Item1 > since3.Item1, "After submit Since should be equal to current time");
            Assert.IsTrue(modified4.Item2.Contains(userName), 
                "Modified should contain current user firstname and lastname");

            WaitTime(2);
            EditForm();
            DropDownSelect(PlacesPage.TimezoneDropDown, TimezoneLondon);
            SubmitForm();
            var since = CleanUpString(GetValue(PlacesPage.Since, true));
            var created5 = GetTimestamp(PlacesPage.Created);
            var modified5 = GetTimestamp(PlacesPage.Modified);
            var since5 = GetTimestamp(PlacesPage.Since);
            var timeSince = ((DateTimeOffset) Convert.ToDateTime(since.Substring(since.IndexOf(' '))))
                            .ToOffset(TimeZoneInfo.Local.BaseUtcOffset.Subtract(
                                TimeSpan.FromHours(DateTime.Now.IsDaylightSavingTime() ? 0 : 1)));
            Assert.IsTrue(created1.Equals(created5), "Created timestamp and user data should never change");
            Assert.IsTrue(modified5.Item1 > modified4.Item1, "After submit Modified should be equal to current time");
            Assert.IsTrue(Math.Abs((since5.Item1 - timeSince).Seconds) <= 10, 
                "After submit Since should be equal to London time");
            Assert.IsTrue(modified5.Item2.Contains(userName), "Modified should contain current user firstname and lastname");

            OpenEntityPage(place);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddExistingPlaceAsChildButton);
            Click(string.Format(PlacesPage.TableRowByTitle, newTitle));
            SubmitForm();
            OpenPlacesPage();
            SetFilter(newTitle);
            Click(string.Format(PlacesPage.TableRowByTitle, newTitle));
            var created6 = GetTimestamp(PlacesPage.Created, true);
            var modified6 = GetTimestamp(PlacesPage.Modified);
            var since6 = GetTimestamp(PlacesPage.Since);
            Assert.IsTrue(created1.Equals(created6), $"Created for iBeacon place {newTitle} has changed");
            Assert.IsTrue(modified6.Item1 == modified5.Item1, $"Modified for iBeacon place {newTitle} has changed");
            Assert.IsTrue(since6.Item1 == since5.Item1, $"Since for iBeacon place {newTitle} has changed");
            Assert.IsTrue(GetValue(PlacesPage.TimezoneReadOnly).Contains(TimezoneLondon),
                $"Timezone for iBeacon place {newTitle} has changed");

            Click(PageFooter.DuplicateButton);
            var titleDuplicate = $"Auto test {RandomNumber}";
            SendText(PlacesPage.Title, titleDuplicate);
            SubmitForm();
            since = CleanUpString(GetValue(PlacesPage.Since, true));
            var created7 = GetTimestamp(PlacesPage.Created);
            var modified7 = GetTimestamp(PlacesPage.Modified);
            var since7 = GetTimestamp(PlacesPage.Since);
            timeSince = ((DateTimeOffset)Convert.ToDateTime(since.Substring(since.IndexOf(' '))))
                        .ToOffset(TimeZoneInfo.Local.BaseUtcOffset.Subtract(
                            TimeSpan.FromHours(DateTime.Now.IsDaylightSavingTime() ? 0 : 1)));
            Assert.IsTrue(Math.Abs((created7.Item1 - modified7.Item1).Seconds) <= 10, 
                "Duplicate place Created timestamp and user data should be equal to Modified field data");
            Assert.IsTrue(Math.Abs((since7.Item1 - timeSince).Seconds) <= 10,
                $"Duplicate place Since should be equal to its timezone time: {timeSince}");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ChildPlacesSectionTableHeader),
                "Duplicate place should not have any children");

            OpenEntityPage(place);
            Assert.IsFalse(AreElementsContainText(PlacesPage.ChildPlacesSectionTableRowsColumnTitle, titleDuplicate),
                $@"Duplicate place '{titleDuplicate}' should not be a child of parent '{place.Title}'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.PlaceMapImageInline),
                "Place Map in duplicate place should have a pre-loaded map");
            Assert.IsTrue(IsElementEquals(PlacesPage.PositionReadOnly, positionParent),
                "Position field has changed in duplicate place");
            Assert.IsTrue(GetValue(PlacesPage.ToleranceRadiusReadOnly) == toleranceRadiusParent,
                "Tolerance Radius field has changed in duplicate place");
        }

        [Test, Regression]
        public void RT04460_2UsersCheckTimestamps()
        {
            TestStart();
            var place1 = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: true);
            var created0 = CleanUpString(GetValue(PlacesPage.Created, waitForValue: true));
            var modified = CleanUpString(GetValue(PlacesPage.Modified));
            var startPos = modified.IndexOf(' ');
            var modified0 = Convert.ToDateTime(modified.Substring(startPos, modified.IndexOf('(') - startPos));
            var since = CleanUpString(GetValue(PlacesPage.Since));
            var since0 = Convert.ToDateTime(since.Substring(since.IndexOf(' ')));

            var place2 = AddPlaceIbeacon(PlaceStatus.Active, isAssignIbeacon: true, isCreateNewPlace: true);
            var created1 = CleanUpString(GetValue(PlacesPage.Created, waitForValue: true));
            modified = CleanUpString(GetValue(PlacesPage.Modified));
            startPos = modified.IndexOf(' ');
            var modified1 = Convert.ToDateTime(modified.Substring(startPos, modified.IndexOf('(') - startPos));
            since = CleanUpString(GetValue(PlacesPage.Since));
            var since1 = Convert.ToDateTime(since.Substring(since.IndexOf(' ')));
            // 2
            OpenSecondaryBrowser();
            NavigateAndLogin(TestConfig.BaseUrl, TestConfig.AdminUser2);
            OpenEntityPage(place2);
            WaitTime(2);
            EditForm();
            SendText(PlacesPage.ToleranceRadius, "50");
            SubmitForm();
            var created2 = CleanUpString(GetValue(PlacesPage.Created, waitForValue: true));
            modified = CleanUpString(GetValue(PlacesPage.Modified));
            var modified2 = Convert.ToDateTime(modified.Substring(startPos, modified.IndexOf('(') - startPos));
            since = CleanUpString(GetValue(PlacesPage.Since));
            var since2 = Convert.ToDateTime(since.Substring(since.IndexOf(' ')));
            Assert.IsTrue(created1 == created2, "Created dates should be the same");
            Assert.IsTrue(Math.Abs((since1 - since2).Seconds) <= 10, "Since dates should be the same");
            Assert.IsTrue(modified2 > modified1, "Modified date in browser 2 should be later than in browser 1");

            // 1
            SwitchToAnotherBrowser();
            OpenEntityPage(place1);
            WaitTime(2);
            EditForm();
            OpenDevicesFromPlace();
            Click(string.Format(Devices.TableRowByText, place2.Title));
            SubmitForm();
            //2
            SwitchToAnotherBrowser();
            OpenEntityPage(place1);
            var created3 = CleanUpString(GetValue(PlacesPage.Created, waitForValue: true));
            modified = CleanUpString(GetValue(PlacesPage.Modified));
            var modified3 = Convert.ToDateTime(modified.Substring(startPos, modified.IndexOf('(') - startPos));
            since = CleanUpString(GetValue(PlacesPage.Since));
            var since3 = Convert.ToDateTime(since.Substring(since.IndexOf(' ')));
            Assert.IsTrue(created3 == created0, "Created dates should be the same");
            Assert.IsTrue(since3 > since0, "Since date should change after iBeacon device replacement");
            Assert.IsTrue(modified3 > modified0, "Modified date should change after iBeacon device replacement");

            WaitTime(2);
            Click(PageFooter.DeleteButton);
            Click(PlacesPage.DeleteButton);
            OpenEntityPage(place1);
            var created4 = CleanUpString(GetValue(PlacesPage.Created, waitForValue: true));
            modified = CleanUpString(GetValue(PlacesPage.Modified));
            var modified4 = Convert.ToDateTime(modified.Substring(startPos, modified.IndexOf('(') - startPos));
            since = CleanUpString(GetValue(PlacesPage.Since));
            var since4 = Convert.ToDateTime(since.Substring(since.IndexOf(' ')));
            Assert.IsTrue(created4 == created0, "Created dates should be the same");
            Assert.IsTrue(since4 > since3, "Since date should change after place deletion");
            Assert.IsTrue(modified4 > modified3, "Modified date should change after place deletion");
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
