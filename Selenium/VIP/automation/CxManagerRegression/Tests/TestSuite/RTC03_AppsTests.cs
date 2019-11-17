using System.Linq;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.Apps;
using NUnit.Framework;
using OpenQA.Selenium;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC03_AppsTests : ParentTest
    {
        private const int LongTestTimeoutMilliseconds = 180000;

        private const string MarketVal = "PCNA";
        private const string ImprintUrlVal = "https://porsche.com/germany";
        private const string DisplayBrightnessVal = "50";
        private const string UnitForMileage = "km";
        private const string MarketCode = "POI";
        private const string DefaultCurrency = "EUR";

        private readonly string _textInGoodByeMessage = "Some text";
        private readonly string _emailSubjectInNewCars = "Some text";
        private readonly string _emailBodyInNewCars = "Some text";
        private readonly string _twitterInNewCars = "Some text";
        private readonly string _messengerInNewCars = "Some text";
        private readonly string _emailSubjectInUsedCars = "Some text";
        private readonly string _emailBodyInUsedCars = "Some text";
        private readonly string _twitterInUsedCars = "Some text";
        private readonly string _messengerInUsedCars = "Some text";

        private readonly string _textInGoodByeMessageDe = "Some text";
        private readonly string _emailSubjectInNewCarsDe = "Some text";
        private readonly string _emailBodyInNewCarsDe = "Some text";
        private readonly string _twitterInNewCarsDe = "Some text";
        private readonly string _messengerInNewCarsDe = "Some text";
        private readonly string _emailSubjectInUsedCarsDe = "Some text";
        private readonly string _emailBodyInUsedCarsDe = "Some text";
        private readonly string _twitterInUsedCarsDe = "Some text";
        private readonly string _messengerInUsedCarsDe = "Some text";

        private string _enAccuracy = "5.25";
        private string _showingDelay = "0";
        private string _enWifiName = "";
        private string _enAddress = "TEST EN ADDRESS";
        private string _deAddress = "TEST DE ADDRESS";
        private string _deSalutationProfile = "DE TEST SALUTATION";
        private string _enSalutationProfile = "EN TEST SALUTATION";
        private readonly string _title = "Some Title";

        private readonly string _salutationNoProfileTooLong = 
            "123456789012345678901234567890123456789012345678901";
        private readonly string _salutationNoProfile =        
            "12345678901234567890123456789012345678901234567890";

        private string _composerApp2Title;
        //private string composerApp2Description;
        private AppResponse _app;

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            PlaceApi.DeletePlaces(PlaceType.Any, TenantTitle.manylang);
            AppApi.DeleteApps(true, new [] {AppTitle.Any}, TenantTitle.manylang);
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

        private void PrepareEnvBeaconAppTests()
        {
            TestStart();
            _app = AddAppIbeacon(TestConfig.IbeaconAppVersions[0]);
            OpenEntityPage(_app);
            if (IsElementFoundQuickly(AppsPage.LanguageDeButtonActive))
            {
                Click(AppsPage.LanguageEnButton);
            }
            TurnOffInfoPopups();
        }

        private void PrepareEnvComposerAppTests(bool addComposerApp1 = true, bool addComposerApp2 = false, bool addPlayer = true)
        {
            CurrentTenant = TenantTitle.nolang;
            TestStart();
            if (addPlayer)
            {
                PrepareEnvPlayerAppTests();
            }
            if (addComposerApp1)
            {
                AddAppComposerHq1();
            }
            if (addComposerApp2)
            {
                AddAppComposerHq2();
            }
            OpenAppsPage();
            TurnOffInfoPopups();
        }

        private void PrepareEnvPlayerAppTests(bool addPlayerApp = true)
        {
            CurrentTenant = TenantTitle.nolang;
            TestStart();
            if (addPlayerApp)
            {
                AddAppPlayer();
            }
            OpenAppsPage();
            TurnOffInfoPopups();
        }

        private void AddAppImageIfAbsent(ImageSize imageSize)
        {
            if (IsElementNotFoundQuickly(AppsPage.Image))
            {
                Click(AppsPage.ImageUploadButton);
                FileManager.Upload(
                    imageSize == ImageSize.Fhd ? TestConfig.ImageFhdFile : TestConfig.Image4KFile
                );
            }
        }

        [Test, Regression]
        public void RT03000_AddDptApp()
        {
            CurrentTenant = TenantTitle.onelang;
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            
            var latestDptAppFileVersion = FileManager.GetFileByVersion(
                TestConfig.DptMobileAppFolder, TestConfig.DptMobileAppFile, TestConfig.DptAppVersions[0]);
            FileManager.Upload(latestDptAppFileVersion);
            Assert.IsTrue(IsEditMode(), "Uploaded DPT app should be in edit mode");
            
            Assert.IsTrue(IsElementNotFound(AppsPage.LanguageBar),
                "Language bar is displayed");
            
            SubmitForm();
            Assert.IsTrue(IsElementFound(AppsPage.ErrorDoesNotMatchPattern),
                @"Validation error below Market 'does not match the expected pattern' should be shown");
            
            SendText(AppsPage.Market, MarketVal);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorDoesNotMatchPattern),
                @"Validation error below Market 'This does not match the expected pattern' should not be shown");

            SendText(AppsPage.ImprintUrl, "sometext/sometext");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorDoesNotMatchPattern),
                @"Validation error below Imprint URL 'This does not match the expected pattern' should be shown");

            SendText(AppsPage.ImprintUrl, ImprintUrlVal);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorDoesNotMatchPattern),
                @"Validation error below Imprint URL 'This does not match the expected pattern' should not be shown");

            DropDownSelect(AppsPage.UnitForMileageOfLeasing, AppsPage.UnitForMileageKm);
            SendText(AppsPage.DisplayBrightness, DisplayBrightnessVal);

            SubmitForm();
            Assert.IsTrue(IsElementEquals(AppsPage.MarketReadOnly, MarketVal),
                $"Market field value is different to {MarketVal}");
            Assert.IsTrue(IsElementEquals(AppsPage.ImprintUrlVal, ImprintUrlVal),
                $"Imprint url field value is different to {ImprintUrlVal}");
            Assert.IsTrue(IsElementEquals(AppsPage.DisplayBrightnessVal, DisplayBrightnessVal),
                $"Display brightness field value is different to {DisplayBrightnessVal}");
            Assert.IsTrue(IsElementEquals(AppsPage.UnitForMileageOfLeasingVal, UnitForMileage),
                $"Unit For Mileage of Leasing dropdown value is different to {UnitForMileage}");
        }

        [Test, Regression]
        public void RT03010_AddIbeaconApp()
        {
            TestStart();
            OpenAppsPage();
            TurnOffInfoPopups();
            Click(PageFooter.AddAppInAppsButton);
            var earliestIbeaconAppFileVersion = FileManager.GetFileByVersion(
                TestConfig.IbeaconAppFolder,
                TestConfig.IbeaconAppFile,
                TestConfig.IbeaconAppVersions[0]);
            FileManager.Upload(earliestIbeaconAppFileVersion);
            
            //En
            
            //Assert.IsTrue(IsElementFound(AppsPage.UploadNotification),
            //    @"'New app version uploaded' popup is not displayed");
            Assert.IsTrue(IsElementFound(PageFooter.CancelButton, timeout: 60),
                "Cancel button is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddVersionButton),
                "Add Version button is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageEnButton),
                @"'En' language tab is absent");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageDeButton),
                @"'De' language tab is absent");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.BeaconButton),
                "Beacon group button is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.InformationButton),
                "Information group button is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.TextsButton),
                "Texts group button is not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.MarketButton),
                "Market group button is not displayed");
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusNew),
                $@"Application status is not '{StatusNew}'");
            SubmitForm();

            //En - Market group button
            Click(AppsPage.MarketButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMarketCodeTooShort),
                "Market code is empty, but no error displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorDefaultCurrencyTooShort),
                "Default Currency is empty, but no error displayed");
            SendText(AppsPage.MarketCode, "dsfDFSD");
            SendText(AppsPage.DefaultCurrency, "ASCASCAS");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMarketCodeTooLong),
                @"Error 'Not more than 5 characters are allowed...' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorDefaultCurrencyTooLong),
                @"Error 'Not more than 3 characters are allowed...' not displayed");
            ClearTextInElement(AppsPage.MarketCode);
            ClearTextInElement(AppsPage.DefaultCurrency);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMarketCodeRequired),
                @"Error 'Market code required' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorDefaultCurrencyRequired),
                @"Error 'Default currency required' not displayed");
            SendText(AppsPage.MarketCode, MarketCode);
            SendText(AppsPage.DefaultCurrency, DefaultCurrency);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorMarketCodePlaceholderForErrorMsg),
                "Error displayed for Market Code");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorDefaultCurrencyPlaceholderForErrorMsg),
                "Error displayed for Default Currency");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorTextsButtonConfigurationIsNotValid),
                @"Texts button: error 'Configuration is not valid' not displayed");

            //En - Texts group button
            Click(AppsPage.TextsButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorGoodByeMessageButtonConfigurationIsNotValid),
                @"Good Bye Message button: error 'Configuration is not valid' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorSharingTemplatesForNewCarsButtonConfigurationIsNotValid),
                @"Sharing Templates For New Cars button: error 'Configuration is not valid' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorSharingTemplatesForUsedCarsButtonConfigurationIsNotValid),
                @"Sharing Templates For Used Cars button: error 'Configuration is not valid' not displayed");

            //En - Goodbye group Button
            Click(AppsPage.GoodByeMessageButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorTextInGoodByeMessageTooShort),
                @"Good Bye Message - Text: error 'This must have at least one character' not displayed");
            SendText(AppsPage.TextInGoodByeMessage, _textInGoodByeMessage);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorTextInGoodByeMessageTooShort),
                @"Good Bye Message - Text: error 'This must have at least one character' is displayed");

            //En - Sharing Templates For New Cars group Button
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorEmailSubjectInNewCarsTooShort),
                @"Sharing Templates For New Cars - Email Subject: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorEmailBodyInNewCarsTooShort),
                @"Sharing Templates For New Cars - Email Body: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorTwitterInNewCarsTooShort),
                @"Sharing Templates For New Cars - Twitter: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMessengertInNewCarsTooShort),
                @"Sharing Templates For New Cars - Messenger: error 'This must have at least one character' not displayed");
            SendText(AppsPage.EmailSubjectInNewCars, _emailSubjectInNewCars);
            SendText(AppsPage.EmailBodyInNewCars, _emailBodyInNewCars);
            SendText(AppsPage.TwitterInNewCars, _twitterInNewCars);
            SendText(AppsPage.MessengerInNewCars, _messengerInNewCars);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorEmailSubjectInNewCarsTooShort),
                @"Sharing Templates For New Cars - Email Subject: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorEmailBodyInNewCarsTooShort),
                @"Sharing Templates For New Cars - Email Body: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorTwitterInNewCarsTooShort),
                @"Sharing Templates For New Cars - Twitter: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorMessengertInNewCarsTooShort),
                @"Sharing Templates For New Cars - Messenger: error 'This must have at least one character' is displayed");

            //En - Sharing Templates For Used Cars group Button
            Click(AppsPage.SharingTemplatesForUsedCarsButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorEmailSubjectInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Email Subject: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorEmailBodyInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Email Body: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorTwitterInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Twitter: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMessengertInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Messenger: error 'This must have at least one character' not displayed");
            SendText(AppsPage.EmailSubjectInUsedCars, _emailSubjectInUsedCars);
            SendText(AppsPage.EmailBodyInUsedCars, _emailBodyInUsedCars);
            SendText(AppsPage.TwitterInUsedCars, _twitterInUsedCars);
            SendText(AppsPage.MessengerInUsedCars, _messengerInUsedCars);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorEmailSubjectInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Email Subject: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorEmailBodyInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Email Body: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorTwitterInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Twitter: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorMessengertInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Messenger: error 'This must have at least one character' is displayed");
            
            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsEditMode(), "Submit form should not be possible as validation errors are on page");

            // De
            Click(AppsPage.LanguageDeButtonInvalid);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageDeButtonActive),
                @"Language DE button doesn't look active");

            //De - Texts group Button
            Click(AppsPage.TextsButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorGoodByeMessageButtonConfigurationIsNotValid),
                @"Good Bye Message button: error 'Configuration is not valid' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorSharingTemplatesForNewCarsButtonConfigurationIsNotValid),
                @"Sharing Templates For New Cars button: error 'Configuration is not valid' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorSharingTemplatesForUsedCarsButtonConfigurationIsNotValid),
                @"Sharing Templates For Used Cars button: error 'Configuration is not valid' not displayed");

            //De - Good Bye Message group Button
            Click(AppsPage.GoodByeMessageButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorTextInGoodByeMessageTooShort),
                @"Good Bye Message - Text: error 'This must have at least one character' not displayed");
            SendText(AppsPage.TextInGoodByeMessage, _textInGoodByeMessageDe);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorTextInGoodByeMessageTooShort),
                @"Good Bye Message - Text: error 'This must have at least one character' is displayed");

            //De - Sharing Templates For New Cars group Button
            Click(AppsPage.SharingTemplatesForNewCarsButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorEmailSubjectInNewCarsTooShort),
                @"Sharing Templates For New Cars - Email Subject: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorEmailBodyInNewCarsTooShort),
                @"Sharing Templates For New Cars - Email Body: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorTwitterInNewCarsTooShort),
                @"Sharing Templates For New Cars - Twitter: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMessengertInNewCarsTooShort),
                @"Sharing Templates For New Cars - Messenger: error 'This must have at least one character' not displayed");
            SendText(AppsPage.EmailSubjectInNewCars, _emailSubjectInNewCarsDe);
            SendText(AppsPage.EmailBodyInNewCars, _emailBodyInNewCarsDe);
            SendText(AppsPage.TwitterInNewCars, _twitterInNewCarsDe);
            SendText(AppsPage.MessengerInNewCars, _messengerInNewCarsDe);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorEmailSubjectInNewCarsTooShort),
                @"Sharing Templates For New Cars - Email Subject: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorEmailBodyInNewCarsTooShort),
                @"Sharing Templates For New Cars - Email Body: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorTwitterInNewCarsTooShort),
                @"Sharing Templates For New Cars - Twitter: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorMessengertInNewCarsTooShort),
                @"Sharing Templates For New Cars - Messenger: error 'This must have at least one character' is displayed");

            //De - Sharing Templates For Used Cars group Button
            Click(AppsPage.SharingTemplatesForUsedCarsButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorEmailSubjectInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Email Subject: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorEmailBodyInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Email Body: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorTwitterInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Twitter: error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorMessengertInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Messenger: error 'This must have at least one character' not displayed");
            SendText(AppsPage.EmailSubjectInUsedCars, _emailSubjectInUsedCarsDe);
            SendText(AppsPage.EmailBodyInUsedCars, _emailBodyInUsedCarsDe);
            SendText(AppsPage.TwitterInUsedCars, _twitterInUsedCarsDe);
            SendText(AppsPage.MessengerInUsedCars, _messengerInUsedCarsDe);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorEmailSubjectInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Email Subject: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorEmailBodyInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Email Body: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorTwitterInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Twitter: error 'This must have at least one character' is displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorMessengertInUsedCarsTooShort),
                @"Sharing Templates For Used Cars - Messenger: error 'This must have at least one character' is displayed");
            SubmitForm();

            // Checks after app save
            //Assert.IsTrue(IsElementFound(AppsPage.SaveNotification),
            //    @"'Save operation was completed successfully' popup is not displayed");
            Assert.IsTrue(IsElementFound(AppsPage.TextsButton), 
                "Texts group is not collapsed after save");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.BeaconButton),
                "Beacon group is not collapsed after save");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.InformationButton),
                "Information group is not collapsed after save");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.TextsButton),
                "Market group is not collapsed after save");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton), 
                "Edit button is not displayed. Application in edit mode?");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddVersionButton),
                "Add Version button is not displayed. Application in edit mode?");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageEnButtonActive),
                @"Language EN tab doesn't look active");
            Assert.IsTrue(IsElementEquals(AppsPage.Status, StatusAvailable),
                $@"App status is not '{StatusAvailable}'");
        }

        [Test, Regression]
        public void RT03020_AddNewLangToIbeaconApp()
        {
            PrepareEnvBeaconAppTests();  

            Click(PageFooter.EditButton);
            Assert.IsTrue(IsElementFound(AppsPage.LanguageAddButton),
                @"Language '+' button not displayed");
            Click(AppsPage.LanguageAddButton);
            Assert.IsTrue(IsElementFound(AppsPage.LanguageArButtonActiveInMenu),
                "Language AR button doesn't look active in add language menu"); 
            Click(AppsPage.LanguageArButtonActiveInMenu);
            Assert.IsTrue(IsElementFound(AppsPage.LanguageArButtonActive),
                "Language AR button doesn't look active in language bar");
            Click(AppsPage.LanguageDeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageDeleteConfirmationDialog),
                "Delete language confirmation dialog not displayed");
            Click(AppsPage.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.LanguageDeleteConfirmationDialog),
                "Delete language confirmation dialog is still displayed");
            Assert.IsTrue(IsElementFound(AppsPage.LanguageArButton),
                "Language AR button doesn't look active");
            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.LanguageArButton),
                "Language AR button still exists");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageEnButton),
                "Language EN button is absent");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageDeButton),
                "Language DE button is absent");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.LanguageAddButton),
                @"Language '+' button is displayed");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button is not displayed. Application in edit mode?");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddVersionButton),
                "Add Version button is not displayed. Application in edit mode?");
        }

        [Test, Regression]
        public void RT03021_AddDeleteNewLangIbeaconApp()
        {
            PrepareEnvBeaconAppTests();

            Click(PageFooter.EditButton);
            Click(AppsPage.LanguageAddButton);
            Click(AppsPage.LanguageArButtonActiveInMenu);
            IsElementFound(CommonElement.ValidationError);
            Click(AppsPage.LanguageDeleteButton);
            Click(AppsPage.DeleteButton);
            IsElementNotFound(CommonElement.ValidationError);
            SubmitForm();
            Assert.IsTrue(IsViewMode(),
                "Edit button is not displayed. Application in edit mode?");
            Assert.IsTrue(IsElementNotFound(AppsPage.OkButton),
                "There should be no modal dialogs");
        }

        [Test, Regression]
        public void RT03030_ExistingLanguageRemoval()
        {
            PrepareEnvBeaconAppTests();

            Click(PageFooter.EditButton);
            if (!IsElementFoundQuickly(AppsPage.LanguageDeButton))
            {
                Assert.Warn("Language DE button is absent in app. Nothing to test.");
                return;
            }

            Click(AppsPage.LanguageDeButton);
            Click(AppsPage.LanguageDeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageDeleteConfirmationDialog),
                "Delete language confirmation dialog not displayed");
            Click(AppsPage.DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.LanguageDeButton),
                "Language DE button is still displayed");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageEnButton),
                "Language EN button is absent");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.LanguageAddButton),
                @"Language '+' button is absent");
            SubmitForm();
            Assert.IsTrue(IsElementFound(PageFooter.EditButton),
                "Edit button is not displayed. Application in edit mode?");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddVersionButton),
                "Add Version button is not displayed. Application in edit mode?");
        }

        [Test, Regression]
        public void RT03040_RestoreLanguage()
        {
            PrepareEnvBeaconAppTests();

            if (IsElementFoundQuickly(AppsPage.LanguageDeButton))
            {
                RT03030_ExistingLanguageRemoval();
            }
            if (IsElementFoundQuickly(AppsPage.LanguageDeButton))
            {
                Assert.Warn("Language DE button is present in app. Nothing to test.");
                return;
            }

            EditForm();
            Click(AppsPage.LanguageAddButton);
            Click(AppsPage.LanguageDeButtonActiveInMenu);

            ClickUntilShown(AppsPage.TextsButton, AppsPage.GoodByeMessageButton);
            Click(AppsPage.GoodByeMessageButton);
            SendText(AppsPage.TextInGoodByeMessage, _textInGoodByeMessageDe);

            Click(AppsPage.SharingTemplatesForNewCarsButton);
            SendText(AppsPage.EmailSubjectInNewCars, _emailSubjectInNewCarsDe);
            SendText(AppsPage.EmailBodyInNewCars, _emailBodyInNewCarsDe);
            SendText(AppsPage.TwitterInNewCars, _twitterInNewCarsDe);
            SendText(AppsPage.MessengerInNewCars, _messengerInNewCarsDe);

            Click(AppsPage.SharingTemplatesForUsedCarsButton);
            SendText(AppsPage.EmailSubjectInUsedCars, _emailSubjectInUsedCarsDe);
            SendText(AppsPage.EmailBodyInUsedCars, _emailBodyInUsedCarsDe);
            SendText(AppsPage.TwitterInUsedCars, _twitterInUsedCarsDe);
            SendText(AppsPage.MessengerInUsedCars, _messengerInUsedCarsDe);

            SubmitForm();
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button is not displayed. Application in edit mode?");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.AddVersionButton),
                "Add Version button is not displayed. Application in edit mode?");
        }

        [Test, Regression]
        public void RT03050_CheckCrossLanguageValues()
        {
            PrepareEnvBeaconAppTests();

            if (IsElementNotFoundQuickly(AppsPage.LanguageDeButton))
            {
                RT03040_RestoreLanguage();
            }
            if (IsElementNotFoundQuickly(AppsPage.LanguageDeButton))
            {
                Assert.Warn("Language DE button is absent in app. Nothing to test.");
                return;
            }

            EditForm();
            Click(AppsPage.BeaconButton);
            Click(AppsPage.InformationButton);
            _enAccuracy = "10";
            SendText(AppsPage.Accuracy, _enAccuracy);
            _enWifiName = "EnWiFiName";
            SendText(AppsPage.WifiName, _enWifiName);
            Click(AppsPage.LanguageDeButton);
            Click(AppsPage.BeaconButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{_enAccuracy} m"),
                $@"DE tab contains different Accuracy value. Should be '{_enAccuracy} m'");
            Click(AppsPage.InformationButton);
            Assert.IsTrue(IsElementEquals(AppsPage.WifiNameReadOnly, _enWifiName),
                $"DE tab contains different WiFi Name value. Should be {_enWifiName}");

            Click(AppsPage.LanguageEnButton);
            Click(AppsPage.InformationButton);
            Click(AppsPage.AddressLine1);
            _enAddress = "Eng Address";
            SendText(AppsPage.Address, _enAddress);

            Click(AppsPage.LanguageDeButton);
            Click(AppsPage.InformationButton);
            Click(AppsPage.AddressLine1);
            Assert.IsFalse(IsElementEquals(AppsPage.Address, _enAddress),
                "DE tab contains the same Address value in Address line 1");
            _deAddress = "German Address";
            SendText(AppsPage.Address, _deAddress);
            _deSalutationProfile = "German salutation";
            SendText(AppsPage.SalutationProfile, _deSalutationProfile);

            Click(AppsPage.LanguageEnButton);
            Click(AppsPage.InformationButton);
            Assert.IsFalse(IsElementEquals(AppsPage.SalutationProfile, _deSalutationProfile),
                "EN tab contains the same Salutation Profile value in Address line 1");
            _enSalutationProfile = "English Salutation";
            SendText(AppsPage.SalutationProfile, _enSalutationProfile);
            SubmitForm();

            RefreshPage();
            Click(AppsPage.LanguageDeButton);
            Click(AppsPage.BeaconButton);
            Click(AppsPage.InformationButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{_enAccuracy} m"),
                "DE tab contains different Accuracy value");
            Assert.IsTrue(IsElementEquals(AppsPage.WifiNameReadOnly, _enWifiName),
                "DE tab contains different WiFi Name value");
            Click(AppsPage.AddressLine1);
            Assert.IsFalse(IsElementEquals(AppsPage.AddressReadOnly, _enAddress),
                "DE tab contains the same as in EN tab Address value in Address line 1");
            Assert.IsFalse(IsElementEquals(AppsPage.SalutationProfileReadOnly, _enSalutationProfile),
                "DE tab contains the same as in EN tab Salutation Profile value in Address line 1");
        }

        [Test, Regression]
        public void RT03060_CheckCrossLanguageValuesForNewLanguage()
        {
            PrepareEnvBeaconAppTests();

            EditForm();
            //Click(AppsPage.InformationButton);
            //Click(AppsPage.AddressLine1);
            Click(AppsPage.LanguageAddButton);
            Click(AppsPage.LanguageArButtonActiveInMenu);
            ClickUntilShown(AppsPage.InformationButton, AppsPage.AddressLine1);
            Click(AppsPage.AddressLine1);
            Assert.IsTrue(IsElementEquals(AppsPage.Address, _enAddress),
                @"AR language tab should contain the same Address value as main language has " +
                $@"in Address line 1: '{_enAddress}'");
            Assert.IsEmpty(GetValue(AppsPage.SalutationProfile),
                "AR language tab contains non-empty Salutation Profile value in Address line 1");
            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.LanguageArButton), 
                "Language AR tab has saved");
        }

        [Test, Regression]
        public void RT03070_CheckValuesForNewAppVersion()
        {
            PrepareEnvBeaconAppTests();

            Click(PageFooter.AddVersionButton);
            var latestIbeaconAppFileVersion = FileManager.GetFileByVersion(
                TestConfig.IbeaconAppFolder,
                TestConfig.IbeaconAppFile,
                TestConfig.IbeaconAppVersions[1]);
            FileManager.Upload(latestIbeaconAppFileVersion);

            ClickUntilShown(AppsPage.BeaconButton, AppsPage.Accuracy);
            ClickUntilShown(AppsPage.InformationButton, AppsPage.AddressAddButton);
            Assert.IsTrue(IsElementEquals(AppsPage.Accuracy, _enAccuracy),
                "EN tab contains different Accuracy value");
            Click(AppsPage.AddressLine1);
            Assert.IsTrue(IsElementEquals(AppsPage.Address, _enAddress),
                "EN tab contains different Address value in Address line 1");
            Assert.IsTrue(IsElementEquals(AppsPage.SalutationProfile, _enSalutationProfile),
                "EN tab contains different Salutation Profile value in Address line 1");
            Click(AppsPage.LanguageDeButton);
            Click(AppsPage.BeaconButton);
            Click(AppsPage.InformationButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{_enAccuracy} m"),
                "DE tab contains different Accuracy value");
            Click(AppsPage.AddressLine1);
            Assert.IsTrue(IsElementEquals(AppsPage.Address, _deAddress),
                "DE tab contains different Address value in Address line 1");
            Assert.IsTrue(IsElementEquals(AppsPage.SalutationProfile, _deSalutationProfile),
                "DE tab contains different Salutation Profile value in Address line 1");
           
            SubmitForm();
            Click(AppsPage.BeaconButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{_enAccuracy} m"),
                "EN tab contains different Accuracy value");
            Click(AppsPage.InformationButton);
            Click(AppsPage.AddressLine1);
            Assert.IsTrue(IsElementEquals(AppsPage.AddressReadOnly, _enAddress),
                "EN tab contains different Address value in Address line 1");
            Assert.IsTrue(IsElementEquals(AppsPage.SalutationProfileReadOnly, _enSalutationProfile),
                "EN tab contains different Salutation Profile value in Address line 1");
            Click(AppsPage.LanguageDeButton);
            Click(AppsPage.BeaconButton);
            Click(AppsPage.InformationButton);
            Assert.IsTrue(IsElementEquals(AppsPage.AccuracyReadOnly, $"{_enAccuracy} m"),
                "DE tab contains different Accuracy value");
            Click(AppsPage.AddressLine1);
            Assert.IsTrue(IsElementEquals(AppsPage.AddressReadOnly, _deAddress),
                "DE tab contains different Address value in Address line 1");
            Assert.IsTrue(IsElementEquals(AppsPage.SalutationProfileReadOnly, _deSalutationProfile),
                "DE tab contains different Salutation Profile value in Address line 1");
        }

        [Test, Regression]
        public void RT03071_AppNumericFieldValidation()
        {
            PrepareEnvBeaconAppTests();

            EditForm();
            Click(AppsPage.BeaconButton);
            Click(AppsPage.ShowingDelayDecreaseButton);
            Assert.IsTrue(IsElementEquals(AppsPage.ShowingDelay, "0"),
                "Showing Delay contains different value than 0");
            Click(AppsPage.ShowingDelayIncreaseButton);

            _showingDelay = "0.1";
            Assert.IsTrue(IsElementEquals(AppsPage.ShowingDelay, _showingDelay),
                "Showing Delay contains different value than 0.1");
            SendText(AppsPage.ShowingDelay, "*a");
            Assert.IsTrue(IsElementEquals(AppsPage.ShowingDelay, string.Empty),
                "Showing Delay allows non-numeric characters input");
            ClearTextInFocusedElement();
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorShowingDelayRequired),
                @"Error 'Showing Delay required' not displayed");

            _showingDelay = "1.1";
            SendText(AppsPage.ShowingDelay, _showingDelay);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorShowingDelayRequired),
                @"Error 'Showing Delay required' is still displayed");

            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ChangesWillBeDiscardedDialog),
                "Confirmation dialog is not displayed");

            Click(AppsPage.CancelButton);
            Assert.IsTrue(IsEditMode(), "Application is not in edit mode");
            Click(PageHeader.PageAppsButton); // do not change
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ChangesWillBeDiscardedDialog),
                "Confirmation dialog is not displayed");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFound(PageFooter.SubmitButton), 
                "Application is still in edit mode");
            _showingDelay = "0";
            Click(AppsPage.TableCellIbeaconApp);
            Click(AppsPage.BeaconButton);
            Assert.IsTrue(IsElementEquals(AppsPage.ShowingDelayReadOnly, $"{_showingDelay} s"),
                "Showing Delay field has been changed and saved unless changes were discarded");
        }

        [Test, Regression]
        public void RT03072_TooLongValues()
        {
            PrepareEnvBeaconAppTests();
            if (IsElementFoundQuickly(PageFooter.CancelButton))
            {
                Click(PageFooter.CancelButton);
            }

            Click(AppsPage.InformationButton);
            //Click(AppsPage.SalutationNoProfileLabel);
            //Assert.IsTrue(IsElementFound(AppsPage.SalutationNoProfileNotification),
            //    @"Tooltip 'Salutation if no profile exists, without placeholders' not displayed");
            //Click(AppsPage.AppTitleLabel);
            //Assert.IsTrue(IsElementFoundQuickly(AppsPage.AppTitleNotification),
            //    @"Tooltip 'App title: Speaking name of the app...' not displayed");

            EditForm();
            Click(AppsPage.SalutationNoProfile);
            //Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.SalutationNoProfileNotification),
            //    @"Tooltip 'Salutation if no profile exists, without placeholders' not displayed");
            SendText(AppsPage.SalutationNoProfile, _salutationNoProfileTooLong);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorSalutationNoProfileTooLong),
                @"Error 'Not more than 50 characters are allowed' not displayed");
            Click(AppsPage.SalutationNoProfile);
            PressKeys(Keys.End + Keys.Backspace);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorSalutationNoProfileTooLong),
                @"Error 'Not more than 50 characters are allowed' is still displayed");
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(AppsPage.SalutationNoProfileReadOnly, timeout: 5),
                "Information group is not collapsed after Submit");
            Click(AppsPage.InformationButton);
            Assert.IsTrue(IsElementEquals(AppsPage.SalutationNoProfileReadOnly, _salutationNoProfile),
                $@"'Salutation No Profile' field value should be '{_salutationNoProfile}'");
            Assert.IsTrue(IsViewMode(), "Application is still in edit mode");
        }

        [Test, Regression]
        public void RT03073_FieldAddresses()
        {
            PrepareEnvBeaconAppTests();
            if (IsElementFoundQuickly(PageFooter.CancelButton))
            {
                Click(PageFooter.CancelButton);
            }

            Click(AppsPage.InformationButton, ignoreIfNoElement: true);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.Address),
                "Address line 1 section is shown expanded");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.AddressAddButton),
                @"Address add button '+' is shown in view mode");
            Click(AppsPage.AddressLine1);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.Address),
                "Address line 1 is still collapsed after click");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.AddressLineDeleteButtonsVisible),
                @"Address line 1 trash bin button is shown in view mode");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.Title),
                "Title field in Address line 1 can be modified");

            Click(PageFooter.EditButton);
            Click(AppsPage.AddressAddButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.AddressLine2),
                "Address line 2 not added");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.Title),
                "Address line 2 is shown collapsed when added");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorTitleTooShort),
                @"Error 'This must have at least one character' should be shown for Title field");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ErrorAddressTooShort),
                @"Error 'This must have at least one character' should be shown for Address field");
            _enAddress = "Some address";
            SendText(AppsPage.Title, _title);
            SendText(AppsPage.Address, _enAddress);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorTitleTooShort),
                @"Error 'This must have at least one character' still shown for Title field");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorAddressTooShort),
                @"Error 'This must have at least one character' still shown for Address field");
            Click(AppsPage.AddressAddButton);
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.AddressLine3),
                "Address line 3 not added");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.TitleLine2),
                "Address line 2 is still expanded on adding a next address line");
            Assert.IsTrue(CountElements(AppsPage.AddressLineDeleteButtonsVisible) == 1,
                "There is more than 1 trash button displayed for address lines");
            Click(AppsPage.AddressLineDeleteButtonsVisible);
            Assert.IsTrue(IsElementNotFound(AppsPage.AddressLine3),
                "Address line 3 is still exist after removal");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.TitleLine2),
                "Address line 2 is expanded on removing 3rd address line");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.Title),
                "Address line 1 is expanded on removing 3rd address line");
            SubmitForm();
            Assert.IsTrue(IsElementFound(PageFooter.EditButton),
                "Edit button is not displayed. Application in edit mode?");
            Assert.IsTrue(IsElementFound(AppsPage.BeaconButton),
                "Beacon group is not collapsed");
            Assert.IsTrue(IsElementFound(AppsPage.InformationButton),
                "Information group is not collapsed");
            Assert.IsTrue(IsElementFound(AppsPage.MarketButton),
                "Market group is not collapsed");
            Assert.IsTrue(IsElementFound(AppsPage.TextsButton),
                "Texts group is not collapsed");
            Click(AppsPage.InformationButton);
            Click(AppsPage.AddressLine2);
            Assert.IsTrue(IsElementEquals(AppsPage.TitleReadOnly, _title),
                "Address line 2 field Title contains wrong value after save");
            Assert.IsTrue(IsElementEquals(AppsPage.AddressReadOnly, _enAddress),
                "Address line 2 field Address contains wrong value after save");
        }

        [Test, Regression]
        public void RT03074_GroupNodesExpandCollapse()
        {
            PrepareEnvBeaconAppTests();
            if (IsElementFoundQuickly(PageFooter.CancelButton))
            {
                Click(PageFooter.CancelButton);
            }

            EditForm();
            Click(AppsPage.MarketButton);
            ClickUntilShown(AppsPage.CurrencyFormattingDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(CountElements(string.Format(CommonElement.DropDownOption, string.Empty)) == 2,
                "Currency Formatting contains more than 2 options");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(CommonElement.DropDownOption, AppsPage.AmountFirstItem)),
                "Currency Formatting dropdown: Amount First option not found");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(CommonElement.DropDownOption, AppsPage.CurrencyFirstItem)),
                "Currency Formatting dropdown: Currency First option not found");
            DropDownSelect(AppsPage.CurrencyFormattingDropDown, AppsPage.CurrencyFirstItem);
            Assert.IsTrue(IsElementEquals(AppsPage.CurrencyFormattingDropDown, "Currency First"),
                "Dropdown Currency First is not set to Currency First");
            Click(AppsPage.TextsButton);
            Click(AppsPage.GoodByeMessageButton);
            Click(AppsPage.EnableInGoodByeMessageCheckBox);
            Assert.IsTrue(IsCheckBoxOn(AppsPage.EnableInGoodByeMessageCheckBox),
                "Enable checkbox in Good Bye Message section is not set to On");
            Click(AppsPage.TextsGroupCollapseButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.EnableInGoodByeMessageCheckBox),
                "Good Bye Message subgroup is still expanded");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.InteriorColorDisclaimer),
                "Texts group is still expanded");
            SubmitForm();

            Click(AppsPage.MarketButton);
            Click(AppsPage.TextsButton);
            Click(AppsPage.GoodByeMessageButton);
            Assert.IsTrue(IsElementEquals(AppsPage.CurrencyFormattingReadOnly, "Currency First"),
                "Dropdown Currency First is not set to Currency First after save");
            Assert.IsTrue(IsCheckBoxOn(AppsPage.EnableInGoodByeMessageCheckBox),
                "Enable checkbox in Good Bye Message subsection is not set to On after save");
        }

        [Test, Regression]
        public void RT03080_UploadComposerHqApp1WithoutPlayer()
        {
            PrepareEnvComposerAppTests(addComposerApp1: false, addComposerApp2: false, addPlayer: false);

            if (IsElementFound(AppsPage.TableRowPlayerApp, 1))
            {
                AppApi.DeleteApps(true, new [] { AppTitle.Player }, CurrentTenant);
            }
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.ComposerHqApp1Folder + "\\" + TestConfig.ComposerHqApp1File);

            Assert.IsTrue(IsElementFound(AppsPage.ErrorPackageImportingWindow),
                $"{AppTitle.ComposerHq1} app file seems imported without {AppTitle.Player} app");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT03090_UploadPlayerAppCanceled()
        {
            PrepareEnvPlayerAppTests(addPlayerApp: false);

            Click(PageFooter.AddAppInAppsButton);
            var playerAppFile = FileManager.GetEarliestFileVersion(
                TestConfig.PlayerAppFolder,
                TestConfig.PlayerAppFile);
            FileManager.Upload(playerAppFile);

            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton, timeout: 120),
                "Player application is not in edit mode");
            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementNotFound(AppsPage.TableRowPlayerApp),
                "Player application has been saved after import without submit");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT03100_UploadPlayerApp()
        {
            PrepareEnvPlayerAppTests(addPlayerApp: false);

            Click(PageFooter.AddAppInAppsButton);
            var playerAppFile = FileManager.GetEarliestFileVersion(
                TestConfig.PlayerAppFolder,
                TestConfig.PlayerAppFile);
            FileManager.Upload(playerAppFile);

            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton, timeout: 180),
                "Player application is not in edit mode or not loaded in 180 sec");
            SubmitForm();
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableRowPlayerAppAvailable),
                $@"Player application has not been saved or {StatusAvailable} status is not set after import and submit");
            Click(AppsPage.TableRowPlayerAppAvailable);
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitleReadOnly, AppTitle.Player),
                $@"App title is not '{AppTitle.Player}'");
            Assert.IsTrue(IsElementEquals(AppsPage.DescriptionReadOnly, TestConfig.PlayerAppDescription),
                $@"App Description is not '{TestConfig.PlayerAppDescription}'");
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.PlayerAppVersions[0]),
                $@"Player app version is not '{TestConfig.PlayerAppVersions[0]}'");
            Assert.IsTrue(IsElementFound(AppsPage.Image),
                "Player app image not found");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.LanguageBar),
                "Language bar is shown for Player app");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT03110_UploadComposerHqApp1Success()
        {
            PrepareEnvComposerAppTests(addComposerApp1: false);

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.ComposerHqApp1Folder + "\\" + TestConfig.ComposerHqApp1File);

            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton, timeout: 120),
                "Composer HQ app 1 is not in edit mode or not loaded in 120 sec");
            SubmitForm();
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableRowComposerHqApp1Available),
                $@"Composer HQ app 1 has not been saved or {StatusAvailable} status is not set after import and submit");
            Click(AppsPage.TableRowComposerHqApp1Available);
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitleReadOnly, AppTitle.ComposerHq1),
                $@"App title is not '{AppTitle.ComposerHq1}'");
            Assert.IsTrue(IsElementEquals(AppsPage.DescriptionReadOnly, TestConfig.ComposerHqApp1Description),
                $@"App Description is not '{TestConfig.ComposerHqApp1Description}'");
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.ComposerHqApp1Version),
                $@"Composer HQ app 1 version is not '{TestConfig.ComposerHqApp1Version}'");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.Image),
                "Composer HQ app 1 image not found");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ImageUploadButton),
                "Upload image button is absent");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT03120_UploadComposerHqApp2SuccessAndAddImage()
        {
            PrepareEnvComposerAppTests(addComposerApp1: false);

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.ComposerHqApp2Folder + "\\" + TestConfig.ComposerHqApp2File);

            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton, timeout: 120),
                "Composer HQ app 2 is not in edit mode or not loaded in 120 sec");
            _composerApp2Title = $"Auto test {RandomNumber}";
            //composerApp2Description = "Some description app_2";
            SendText(AppsPage.AppTitle, AppTitle.ComposerHq2, isCheckInput: true); 
            SendText(AppsPage.Description, TestConfig.ComposerHqApp2Description);
            Click(AppsPage.ImageUploadButton);
            FileManager.Upload(TestConfig.ImageFhdFile);

            Assert.IsTrue(IsElementFound(AppsPage.Image, 60),
                "Valid FHD image is not uploaded in 60 sec");
            SubmitForm();
            Assert.IsTrue(IsElementFound(PageFooter.EditButton),
                "Composer HQ app 2 is still in edit mode");
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitleReadOnly, AppTitle.ComposerHq2),
                $@"App title is not '{AppTitle.ComposerHq2}'");
            Assert.IsTrue(IsElementEquals(AppsPage.DescriptionReadOnly, TestConfig.ComposerHqApp2Description),
                $@"App Description is not '{TestConfig.ComposerHqApp2Description}'");
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.ComposerHqApp2Version),
                $@"Composer HQ app 2 version is not '{TestConfig.ComposerHqApp2Version}'");
            Assert.IsTrue(IsElementFound(AppsPage.Image),
                "Composer HQ app 2 image not found");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT03130_AppTitleValidation()
        {
            PrepareEnvComposerAppTests(addComposerApp1: true, addComposerApp2: true);

            Click(AppsPage.TableRowComposerHqApp2Available);
            EditForm();
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitle, AppTitle.ComposerHq2),
                $@"Composer HQ 2 app title should be '{AppTitle.ComposerHq2}'");
            ClearTextInElement(AppsPage.AppTitle);
            SendText(AppsPage.AppTitle, AppTitle.ComposerHq1, isCheckInput: true);
            SubmitForm();
            Assert.IsTrue(IsElementFound(AppsPage.ErrorAppTitleIsUsedDialog),
                $@"Error '{AppTitle.ComposerHq1} title is used by another app...' should be displayed");
            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorAppTitleIsUsedDialog),
                $@"Error '{AppTitle.ComposerHq1} title is used by another app...' is still displayed");
            ClearTextInElement(AppsPage.AppTitle);
            SubmitForm();
            OpenAppsPage();
            Assert.IsTrue(IsElementFound(AppsPage.TableRowComposerHqApp2Available),
                $@"App '{AppTitle.ComposerHq2}' is not saved");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT03140_AppImageOpensInNewTab()
        {
            PrepareEnvComposerAppTests(addComposerApp1: false, addComposerApp2: true);

            Click(AppsPage.TableRowComposerHqApp2Available);
            Click(PageFooter.EditButton);

            AddAppImageIfAbsent(ImageSize.Fhd);
            SubmitForm();
            Click(AppsPage.Image);

            var browserTabs = GetTabHandles();
            Assert.IsTrue(browserTabs.Count == 2, 
                "New browser tab which should contain app image is not open");
            Assert.IsTrue(IsElementFound(AppsPage.Image),
                "App image is not found at new tab");
            CloseTab(browserTabs.Last());
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT03150_AppImageRemoveWithoutSave()
        {
            PrepareEnvComposerAppTests(addComposerApp1: false, addComposerApp2: true);

            Click(AppsPage.TableRowComposerHqApp2Available);
            Click(PageFooter.EditButton);
            AddAppImageIfAbsent(ImageSize.Fhd);
            SubmitForm();
            EditForm();

            Assert.IsTrue(IsElementFound(AppsPage.ImageReplaceButton),
                @"Image 'Replace' button not found");
            Assert.IsTrue(IsElementFound(AppsPage.ImageRemoveButton),
                @"Image 'Remove' button not found");

            //var imageLink = GetElementAttribute(AppsPage.Image, "src");
            Click(AppsPage.ImageRemoveButton);
            Assert.IsTrue(IsElementNotFound(AppsPage.Image),
                "App image not deleted after Remove button press");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ImageReplaceButton),
                @"Image 'Replace' button still present when no image uploaded");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ImageRemoveButton),
                @"Image 'Remove' button still present when no image uploaded");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ImageUploadButton),
                @"Image 'Upload' button is absent when no image uploaded");
            SendText(AppsPage.AppTitle, "Some text ABC", isCheckInput: true);
            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementFound(AppsPage.Image),
                "App image is absent, but app changes were discarded");
            Assert.IsTrue(IsElementEquals(AppsPage.AppTitleReadOnly, AppTitle.ComposerHq2),
                "App title has changed, but app changes were discarded");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT03160_AppImageReplace()
        {
            PrepareEnvComposerAppTests(addComposerApp1: false, addComposerApp2: true);

            Click(AppsPage.TableRowComposerHqApp2Available);
            Click(PageFooter.EditButton);
            AddAppImageIfAbsent(ImageSize.Fhd);

            Click(AppsPage.ImageReplaceButton);
            FileManager.Upload(TestConfig.Image4KFile);

            Assert.IsTrue(IsElementFound(AppsPage.Image),
                "App image 4K not uploaded after Replace button press");
            Assert.IsTrue(IsElementFound(AppsPage.ImageReplaceButton),
                @"Image 'Replace' button still present when no image uploaded");
            Assert.IsTrue(IsElementFoundQuickly(AppsPage.ImageRemoveButton),
                @"Image 'Remove' button still present when no image uploaded");
            SubmitForm();
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                @"Edit button is not displayed. App still in edit mode?");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT03170_AppImageRemove()
        {
            PrepareEnvComposerAppTests(addComposerApp1: false, addComposerApp2: true);

            Click(AppsPage.TableRowComposerHqApp2Available);
            Click(PageFooter.EditButton);
            AddAppImageIfAbsent(ImageSize.Fhd);

            Click(AppsPage.ImageRemoveButton);

            Assert.IsTrue(IsElementNotFound(AppsPage.Image),
                "App image not deleted after Remove button press");
            Assert.IsTrue(IsElementNotFound(AppsPage.ImageReplaceButton),
                @"Image 'Replace' button still present when no image uploaded");
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ImageRemoveButton),
                @"Image 'Remove' button still present when no image uploaded");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                @"Submit button is not displayed");
            SubmitForm();
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                @"Edit button is not displayed. App still in edit mode?");
        }

        [Test, Regression]
        public void RT03180_FrameworkOneAppWithAssetsAndConfig()
        {
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.App1Assets);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorIsNotAVersionDialog),
                "Error 'Uploaded app is not a version of \"resource_pack_bwi\"' should be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorIsNotAVersionDialog),
                "Error 'Uploaded app is not a version of \"resource_pack_bwi\"' should be closed");
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppsUri),
                "On App1 Assets package import fail, Apps list page should be shown");

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.App1Config);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorIsNotAVersionDialog),
                "Error 'Uploaded app is not a version of \"bwi_configuration\"' should be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorIsNotAVersionDialog),
                "Error 'Uploaded app is not a version of \"bwi_configuration\"' should be closed");
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppsUri),
                "On App1 Config package import fail, Apps list page should be shown");

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.App1Package);
            Assert.IsTrue(IsEditMode(), "App1 main package should be uploaded successfully");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "App1 main package should be submitted successfully");
            var appId = GetEntityIdFromUrl();

            Click(PageFooter.AddVersionButton);
            FileManager.Upload(TestConfig.App1Assets);
            Assert.IsTrue(IsElementFound(AppsPage.ErrorIsNotAVersionDialog),
                "Error 'Uploaded app is not a version of \"HQ-bwi\" app' should be shown");

            Click(AppsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(AppsPage.ErrorIsNotAVersionDialog),
                "Error 'Uploaded app is not a version of \"HQ-bwi\" app' should be closed");
            Assert.IsTrue(IsPageContainsUri($@"{TestConfig.AppUri}/{appId}"),
                "App1 main package page should be shown");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.App1Assets);
            Assert.IsTrue(IsEditMode(), "App1 Assets package should be uploaded successfully");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "App1 Assets package should be submitted successfully");

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.App1Config);
            Assert.IsTrue(IsEditMode(), "App1 Config package should be uploaded successfully");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "App1 Config package should be submitted successfully");

            Click(PageFooter.AddVersionButton);
            FileManager.Upload(TestConfig.App1Config);
            Assert.IsTrue(IsEditMode(), 
                "App1 Config package should be uploaded successfully 2nd time");
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.App1Version1),
                $"Versions field should contain version {TestConfig.App1Version1}");
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.App1Version2),
                $"Versions field should contain version {TestConfig.App1Version2}");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.AppsUri),
                "On Cancel button press in page footer, Apps list page should be shown");
            
            Click(string.Format(AppsPage.TableRowByText, AppTitle.ConfigurationApplication1));
            Assert.IsTrue(AreElementsContainText(AppsPage.Versions, TestConfig.App1Version1),
                $"Versions field should contain version {TestConfig.App1Version1}");
            Assert.IsFalse(AreElementsContainText(AppsPage.Versions, TestConfig.App1Version2),
                $"Versions field should not contain version {TestConfig.App1Version2}");
        }

        [Test, Regression]
        public void RT03190_UeCarApps()
        {
            AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion);
            var placeWw = AddPlaceWw(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true);
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.UeCarCayenneNewAppFolder,
                TestConfig.UeCarCayenneNewAppFile, TestConfig.UeCarCayenneNewVersions[0]);
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsEditMode(), 
                $"App {AppTitle.UeCarCayenneNew} should be in edit mode on import complete");
            
            SubmitForm();
            OpenEntityPage(placeWw);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Assert.IsTrue(IsElementNotFound(string.Format(AppsPage.TableRowByText, AppTitle.UeCarCayenneNew)),
                $"Select App dialog should not display app {AppTitle.UeCarCayenneNew}");
            ClickAtPoint(CommonElement.Backdrop, 10, 300); // close modal

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            pathFile = FileManager.GetFileByVersion(TestConfig.UeCarScenePlainAppFolder,
                TestConfig.UeCarScenePlainAppFile, TestConfig.UeCarScenePlainVersions[0]);
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsEditMode(),
                $"App {AppTitle.UeCarScenePlain} should be in edit mode on import complete");

            SubmitForm();
            OpenEntityPage(placeWw);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Assert.IsTrue(IsElementNotFound(string.Format(AppsPage.TableRowByText, AppTitle.UeCarCayenneNew)),
                $"Select App dialog should not display app {AppTitle.UeCarCayenneNew}");
            Assert.IsTrue(IsElementNotFound(string.Format(AppsPage.TableRowByText, AppTitle.UeCarScenePlain)),
                $"Select App dialog should not display app {AppTitle.UeCarScenePlain}");
            ClickAtPoint(CommonElement.Backdrop, 10, 300); // close modal

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            pathFile = FileManager.GetFileByVersion(TestConfig.UeCarViewerAppFolder,
                TestConfig.UeCarViewerAppFile, TestConfig.UeCarViewerVersions[0]);
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton, 120),
                $"App {AppTitle.UeCarViewer} should be in edit mode on import complete");

            SubmitForm();
            OpenEntityPage(placeWw);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Assert.IsTrue(IsElementNotFound(string.Format(AppsPage.TableRowByText, AppTitle.UeCarCayenneNew)),
                $"Select App dialog should not display app {AppTitle.UeCarCayenneNew}");
            Assert.IsTrue(IsElementNotFound(string.Format(AppsPage.TableRowByText, AppTitle.UeCarScenePlain)),
                $"Select App dialog should not display app {AppTitle.UeCarScenePlain}");
            Assert.IsTrue(IsElementFound(string.Format(AppsPage.TableRowByText, AppTitle.UeCarViewer)),
                $"Select App dialog should not display app {AppTitle.UeCarViewer}");

            ClickUntilConditionMet(string.Format(AppsPage.TableRowByText, AppTitle.UeCarViewer),
                () => IsElementNotFoundQuickly(PlacesPage.SelectAppDialog));
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionTableRow1) &&
                    IsElementEquals(PlacesPage.AppsSectionTableRow1TitleCell, AppTitle.UeCarViewer),
                $"Apps section: App {AppTitle.UeCarViewer} should be added");

            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsScenes),
                "Apps section > row 1 > Scenes subsection should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsCars),
                "Apps section > row 1 > Cars subsection should be shown");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorAppsSectionRow1DetailsScenesRequired),
                @"Apps section > row 1 > Scenes subsection should show error 'Scenes required'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorAppsSectionRow1DetailsCarsRequired),
                @"Apps section > row 1 > Cars subsection should show error 'Cars required'");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsScenesAddButton),
                @"Apps section > row 1 > Scenes subsection should contain +Add button");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsCarsAddButton),
                @"Apps section > row 1 > Cars subsection should contain +Add button");

            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsScenesAddButton, 
                PlacesPage.AppsSectionTableRow1, 4);
            Click(PlacesPage.AppsSectionRow1DetailsScenesRowDeleteButton);
            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsScenesAddButton,
                PlacesPage.AppsSectionTableRow1, 4);
            Assert.IsTrue(IsElementFound(PlacesPage.AppsSectionRow1DetailsScenesRow1DetailsSceneDropDown),
                @"Apps section > row 1 > Scenes subsection > row 1 should contain Scene drop-down");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsScenesRow1DetailsAppPackageUpdateDropDown),
                @"Apps section > row 1 > Scenes subsection > row 1 should contain App Package Update drop-down");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorAppsSectionRow1DetailsScenesRow1DetailsSceneRequired),
                @"Apps section > row 1 > Scenes subsection > row 1 should contain error 'required' for Scene drop-down");

            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsScenesRow1DetailsSceneDropDown,
                CommonElement.DropDownOptionList);
            Assert.IsTrue(CountElements(CommonElement.DropDownOptionList) == 1,
                @"Apps section > row 1 > Scenes subsection > row 1 > Scene drop-down should contain only " +
                $"{AppTitle.UeCarScenePlain} ({TestConfig.UeCarScenePlainVersions[0]})");
            Assert.IsTrue(
                CountElements(string.Format(CommonElement.DropDownOption, 
                    $"{AppTitle.UeCarScenePlain} ({TestConfig.UeCarScenePlainVersions[0]})")) == 1,
                @"Apps section > row 1 > Scenes subsection > row 1 > Scene drop-down should contain " +
                $"{AppTitle.UeCarScenePlain} ({TestConfig.UeCarScenePlainVersions[0]})");
            
            DropDownSelect(PlacesPage.AppsSectionRow1DetailsScenesRow1DetailsSceneDropDown,
                $"{AppTitle.UeCarScenePlain} ({TestConfig.UeCarScenePlainVersions[0]})");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorAppsSectionRow1DetailsScenesRow1DetailsSceneRequired),
                @"Apps section > row 1 > Scenes subsection > row 1 should contain no errors for Scene drop-down");

            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsCarsAddButton,
                PlacesPage.AppsSectionRow1DetailsCarsRow1);
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsCarsRow1DetailsCarDropDown),
                "Apps section > row 1 > Cars subsection > row 1 should contain Car drop-down");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.AppsSectionRow1DetailsCarsRow1DetailsAppPackageUpdateDropDown),
                "Apps section > row 1 > Cars subsection > row 1 should contain App Package Update drop-down");
            Assert.IsTrue(IsElementFoundQuickly(PlacesPage.ErrorAppsSectionRow1DetailsCarsRow1DetailsCarRequired),
                @"Apps section > row 1 > Cars subsection > row 1 should contain error 'required' for Car drop-down");

            ClickUntilShown(PlacesPage.AppsSectionRow1DetailsCarsRow1DetailsCarDropDown,
                CommonElement.DropDownOptionList);
            Assert.IsTrue(CountElements(CommonElement.DropDownOptionList) == 1,
                @"Apps section > row 1 > Cars subsection > row 1 > Car drop-down should contain only " +
                $"{AppTitle.UeCarCayenneNew} ({TestConfig.UeCarCayenneNewVersions[0]})");
            Assert.IsTrue(
                CountElements(string.Format(CommonElement.DropDownOption,
                    $"{AppTitle.UeCarCayenneNew} ({TestConfig.UeCarCayenneNewVersions[0]})")) == 1,
                @"Apps section > row 1 > Cars subsection > row 1 > Car drop-down should contain " +
                $"{AppTitle.UeCarCayenneNew} ({TestConfig.UeCarCayenneNewVersions[0]})");

            DropDownSelect(PlacesPage.AppsSectionRow1DetailsCarsRow1DetailsCarDropDown,
                $"{AppTitle.UeCarCayenneNew} ({TestConfig.UeCarCayenneNewVersions[0]})");
            Assert.IsTrue(IsElementNotFoundQuickly(PlacesPage.ErrorAppsSectionRow1DetailsCarsRow1DetailsCarRequired),
                @"Apps section > row 1 > Cars subsection > row 1 should contain no errors for Car drop-down");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Place should be in view mode on submit");
        }

        [Test, Regression]
        public void RT03200_DeviceTypes()
        {
            AppResponse playerApp = null, iBeaconApp = null, dptApp = null;
            Parallel.Invoke(() =>
            {
                playerApp = AddAppPlayer();
                iBeaconApp = AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
                dptApp = AddAppDpt(AppStatus.Any, TestConfig.DptAppVersions[0]);
            });
            TestStart();

            OpenAppsPage();
            Assert.IsTrue(
                IsElementEquals(string.Format(AppsPage.TableRowDeviceTypeByTitle, playerApp.ActualAppVersion.Title), DeviceTypeWw),
                $@"Player app should have Device Type '{DeviceTypeWw}'");

            Assert.IsTrue(
                IsElementEquals(string.Format(AppsPage.TableRowDeviceTypeByTitle, iBeaconApp.ActualAppVersion.Title), DeviceTypeIbeacon),
                $@"iBeacon app should have Device Type '{DeviceTypeIbeacon}'");

            Assert.IsTrue(
                IsElementEquals(string.Format(AppsPage.TableRowDeviceTypeByTitle, dptApp.ActualAppVersion.Title), DeviceTypeNone),
                $@"DPT app should have Device Type '{DeviceTypeNone}'");

            Click(PageFooter.AddAppInAppsButton);
            FileManager.Upload(TestConfig.TestDeviceTypesPackage);
            Assert.IsTrue(
                IsElementEquals(AppsPage.DeviceType, 
                    $"{DeviceTypeAndroidWorkstation}, {DeviceTypeIbeacon}, {DeviceTypeIosDevice}, {DeviceTypeNone}, {DeviceTypeWw}"),
                "Device Type field should display " +
                $@"'{DeviceTypeAndroidWorkstation}, {DeviceTypeIbeacon}, {DeviceTypeIosDevice}, {DeviceTypeNone}, {DeviceTypeWw}'");
            var title = GetValue(AppsPage.AppTitle);
            
            SubmitForm();
            OpenAppsPage();
            Assert.IsTrue(IsElementEquals(string.Format(AppsPage.TableRowDeviceTypeByTitle, title), 
                    $"{DeviceTypeAndroidWorkstation}, {DeviceTypeIbeacon}, {DeviceTypeIosDevice}, ..."),
                $"Device Type column for app {title} should display " +
                $"{DeviceTypeAndroidWorkstation}, {DeviceTypeIbeacon}, {DeviceTypeIosDevice}, ...");
        }

        [Test, Regression]
        public void RT03210_UploadNewComposerAppsWithoutPlayer()
        {
            AppApi.DeleteApps(true, new [] { AppTitle.Player }, CurrentTenant);
            AppApi.DeleteApps(true, new [] { AppTitle.ComposerVipB }, CurrentTenant);
            TestStart();

            OpenAppsPage();
            Click(PageFooter.AddAppInAppsButton);
            var pathFile = FileManager.GetFileByVersion(TestConfig.ComposerVipbAppFolder,
                TestConfig.ComposerVipbAppFile, TestConfig.ComposerVipbAppEarliestVersion);
            FileManager.Upload(pathFile);
            Assert.IsTrue(IsElementNotFound(AppsPage.ErrorPackageImportingWindow) && IsEditMode(),
                $"App {AppTitle.ComposerVipB} should be imported without {AppTitle.Player} installed");
        }

        [Test, Regression]
        public void RT03220_AssignComposerAppToPlaceWithoutPlayer()
        {
            AppApi.DeleteApps(true, new[] { AppTitle.Player }, CurrentTenant);
            AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion);
            TestStart();

            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            SendText(PlacesPage.Title, "dfsdfdsf");
            DropDownSelect(PlacesPage.DeviceTypeDropDown, DeviceTypeWw);
            MouseOver(PageFooter.AddPlaceSubMenu);
            Click(PageFooter.AddAppInPlacesButton);
            Click(AppsPage.TableRowComposerVipbApp);
            Assert.IsTrue(IsElementFound(PlacesPage.ThereIsNoPlayerDialog),
                $"App {AppTitle.ComposerVipB} can be added to place without {AppTitle.Player} installed");
            
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
                AppApi.DeleteApps();
            }
            else
            {
                AppApi.DeleteApps(true, new[] { AppTitle.TestDeviceTypes });
            }      
        }
    }
}
