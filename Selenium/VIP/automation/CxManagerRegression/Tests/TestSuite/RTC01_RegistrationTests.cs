using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using NUnit.Framework;
using OpenQA.Selenium;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public class RTC01_RegistrationTests : ParentTest
    {
        private string _activationLink;
        private bool _isConfirmationEmailHasSent;
        private string _activationLinkUdAdmin;
        private bool _activationLinkClicked;
        private bool _isNewUserActivatedByAdmin;
        private const int LongTestTimeoutMilliseconds = 300000;
        private readonly int _waitMailTimeoutSeconds;
        private readonly MailManager _mm = new MailManager();

        public RTC01_RegistrationTests()
        {
            _waitMailTimeoutSeconds = LongTestTimeoutMilliseconds / 1000 - 10;
        }

        [OneTimeSetUp]
        public async Task BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            await _mm.ConnectMailServerAsync(UserType.AdminCxM).ConfigureAwait(false);
            await _mm.ConnectMailServerAsync(UserType.AdminUserDirectory).ConfigureAwait(false);
        }

        [SetUp]
        public async Task Setup()
        {
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }
            TestStart(TestConfig.BaseUrl, doLogin: false);
        }

        [Test, Regression]
        public void RT01010_RegistrationPageOpened()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.RegisterUrl),
                "User is not redirected to register page");
        }

        [Test, Regression]
        public void RT01040_RegistrationPageElementsExist()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            Assert.IsTrue(IsElementFound(RegistrationPage.Logo),
                "Logo image is absent");
            Assert.IsTrue(IsElementFound(RegistrationPage.Email),
                "Email field is absent");
            Assert.IsTrue(IsElementFound(RegistrationPage.Password),
                "Password field is absent");
            Assert.IsTrue(IsElementFound(RegistrationPage.RepeatPassword),
                "Repeat Password field is absent");
            Assert.IsTrue(IsElementFound(RegistrationPage.FirstName),
                "First Name field is absent");
            Assert.IsTrue(IsElementFound(RegistrationPage.LastName),
                "Last Name field is absent");
            Assert.IsTrue(IsElementFound(RegistrationPage.RegisterButton),
                "Register button is absent");
            Assert.IsTrue(IsElementFound(RegistrationPage.CancelButton),
                "Cancel button is absent");
            Assert.IsTrue(IsDisabledElement(RegistrationPage.RegisterButton),
                "Register button is enabled on empty form");
        }

        [Test, Regression]
        public void RT01050_OpenTermsAndConditionsWindow()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            Click(RegistrationPage.AcceptTermsLink);
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.AcceptTermsDialog),
                "Accept Terms And Conditions window is absent");
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.OkButton),
                "OK button does not exist on Accept Terms window");
        }

        [Test, Regression]
        public void RT01060_CloseTermsAndConditionsWindow()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            Click(RegistrationPage.AcceptTermsLink);
            Click(RegistrationPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(RegistrationPage.AcceptTermsDialog),
                "Accept Terms And Conditions window still open");
        }

        [Test, Regression]
        public void RT01070_AllFieldsMandatoryValidation()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company name");
            SendText(RegistrationPage.Phone, "+834594967468");
            SendText(RegistrationPage.Password, TestConfig.NewUser.Password);
            SendText(RegistrationPage.RepeatPassword, TestConfig.NewUser.Password);
            ClearTextInElement(RegistrationPage.Email);
            ClearTextInElement(RegistrationPage.FirstName);
            ClearTextInElement(RegistrationPage.LastName);
            ClearTextInElement(RegistrationPage.Company);
            ClearTextInElement(RegistrationPage.Phone);
            ClearTextInElement(RegistrationPage.Password);
            ClearTextInElement(RegistrationPage.RepeatPassword);
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.ErrorEmailIsRequired),
                "No error 'Email is required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.ErrorFirstNameIsRequired),
                "No error 'First Name is required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.ErrorLastNameIsRequired),
                "No error 'Last Name is required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.ErrorCompanyIsRequired),
                "No error 'Company is required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.ErrorPhoneIsRequired),
                "No error 'Phone is required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.ErrorPasswordIsRequired),
                "No error 'Password is required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.ErrorRepeatPasswordIsRequired),
                "No error 'Repeat Password is required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.ErrorAcceptTerms),
                "No error 'Please accept terms and conditions' displayed");
        }

        [Test, Regression]
        public void RT01080_CancelRegistration()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            Click(RegistrationPage.CancelButton);
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.LoginUrl),
                "User is not redirected to a login page");
        }

        [Test, Regression]
        public void RT01090_NewRegistrationFormAndNoErrors()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            Assert.IsTrue(IsElementNotFoundQuickly(RegistrationPage.ErrorMessagePlaceholder),
                @"Registration page contains strange error message(s)");
        }

        [Test, Regression]
        public void RT01100_InvalidEmailPattern()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, "somewrongvalue@");
            Assert.IsTrue(IsElementFound(LoginPage.ErrorEmailDoesntMatch),
                @"Error message 'Email doesn't match pattern' not displayed");
        }

        [Test, Regression]
        public void RT01110_InvalidEmailMinLength()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, "a@bb.c");
            Assert.IsTrue(IsElementFound(LoginPage.ErrorEmailDoesntMatch),
                @"Error message 'Email doesn't match pattern' not displayed");
        }

        [Test, Regression]
        public void RT01120_RegisterExistingEmail()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.AdminUser.Email);
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementFound(RegistrationPage.ErrorYourAlreadyHaveAnActiveAccount),
                @"Error message 'You already have an active account associated with this email' not displayed");
        }

        [Test, Regression]
        public void RT01130_RegisterValidEmail()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementNotFound(RegistrationPage.ErrorYourAlreadyHaveAnActiveAccount),
                @"Error message 'You already have an active account associated with this email' is displayed");
        }

        [Test, Regression]
        public void RT01140_InvalidFirstName()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "1");
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementFound(RegistrationPage.ErrorFirstNameMinLength),
                @"Error message 'First Name min length is...' not displayed");
        }

        [Test, Regression]
        public void RT01150_ValidFirstName()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementNotFound(RegistrationPage.ErrorFirstNameMinLength),
                @"Error message 'First Name min length is...' is displayed");
        }

        [Test, Regression]
        public void RT01160_InvalidLastName()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "1");
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementFound(RegistrationPage.ErrorLastNameMinLength),
                @"Error message 'Last Name min length is...' not displayed");
        }

        [Test, Regression]
        public void RT01170_ValidLastName()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementNotFound(RegistrationPage.ErrorLastNameMinLength),
                @"Error message 'Last Name min length is...' is displayed");
        }

        [Test, Regression]
        public void RT01180_InvalidCompany()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "1");
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementFound(RegistrationPage.ErrorCompanyMinLength),
                @"Error message 'Company min length is...' not displayed");
        }

        [Test, Regression]
        public void RT01190_ValidCompany()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementNotFound(RegistrationPage.ErrorCompanyMinLength),
                @"Error message 'Company min length is...' is displayed");
        }

        [Test, Regression]
        public void RT01200_InvalidPhonePattern1()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            const string phone = "shdhjhgms";
            SendText(RegistrationPage.Phone, phone);
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementFound(RegistrationPage.ErrorPhoneDoesntMatch),
                $@"Error message 'Phone doesn't match pattern' not displayed. Phone: '{phone}'");
        }

        [Test, Regression]
        public void RT01210_InvalidPhonePattern2()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            const string phone = "123456789";
            SendText(RegistrationPage.Phone, phone);
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementFound(RegistrationPage.ErrorPhoneDoesntMatch),
                $@"Error message 'Phone doesn't match pattern' not displayed. Phone: '{phone}'");
        }

        [Test, Regression]
        public void RT01220_InvalidPhoneLengthTooShort()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            const string phone = "+12345";
            SendText(RegistrationPage.Phone, phone);
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementFound(RegistrationPage.ErrorPhoneMinLength),
                $@"Error message 'Phone min length is...' not displayed. Phone: '{phone}'");
        }

        [Test, Regression]
        public void RT01230_InvalidPhoneLengthTooLong()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            const string phone = "+12345678901234567890";
            SendText(RegistrationPage.Phone, phone);
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsElementFound(RegistrationPage.ErrorPhoneMaxLength),
                $@"Error message 'Phone max length is...' not displayed. Phone: '{phone}'");
        }

        [Test, Regression]
        public void RT01240_InvalidPasswordLength()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            SendText(RegistrationPage.Phone, "+123456789");
            const string pwd = "dsfsd";
            SendText(RegistrationPage.Password, pwd);
            Assert.IsTrue(IsElementFound(RegistrationPage.ErrorPasswordMinLength),
                $@"Error message 'Password min length is...' not displayed. Password: '{pwd}'");
        }

        [Test, Regression]
        public void RT01250_PasswordsDoNotMatch()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            SendText(RegistrationPage.Phone, "+123456789");
            SendText(RegistrationPage.Password, "11111111");
            SendText(RegistrationPage.RepeatPassword, "22222222");
            Assert.IsTrue(IsElementNotFound(RegistrationPage.ErrorPasswordsDoNotMatch),
                @"Error message 'Passwords do not match' not displayed");
        }

        [Test, Regression]
        public void RT01260_AcceptTermsAndConditionsError()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            SendText(RegistrationPage.Phone, "+123456789");
            SendText(RegistrationPage.Password, "11111111");
            SendText(RegistrationPage.RepeatPassword, "22222222");
            ClearTextInFocusedElement();
            SendText(RegistrationPage.RepeatPassword, "11111111");
            Assert.IsTrue(IsElementNotFound(RegistrationPage.ErrorAcceptTerms),
                @"Error message 'Please accept terms and conditions' not displayed");
        }

        [Test, Regression]
        public void RT01270_AcceptTermsAndConditionsError()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            SendText(RegistrationPage.Phone, "+123456789");
            SendText(RegistrationPage.Password, "11111111");
            SendText(RegistrationPage.RepeatPassword, "11111111");
            Click(RegistrationPage.AcceptTermsCheckBox);
            Assert.IsFalse(IsDisabledElement(RegistrationPage.RegisterButton),
                "Register button is still disabled");
        }

        [Test, Regression]
        public void RT01280_PasswordBlacklisted()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            SendText(RegistrationPage.Phone, "+123456789");
            SendText(RegistrationPage.Password, "11111111");
            SendText(RegistrationPage.RepeatPassword, "11111111");
            Click(RegistrationPage.AcceptTermsCheckBox);
            Click(RegistrationPage.RegisterButton);
            Assert.IsTrue(IsElementNotFound(RegistrationPage.ErrorAcceptTerms),
                @"Error message 'Entered password is blacklisted...' not displayed");
        }

        [Test, Regression]
        public void RT01290_CheckInboxWindow()
        {
            ClickUntilShown(LoginPage.RegisterButton, RegistrationPage.FirstName);
            SendText(RegistrationPage.Email, TestConfig.NewUser.Email);
            SendText(RegistrationPage.FirstName, "Auto");
            SendText(RegistrationPage.LastName, "Test");
            SendText(RegistrationPage.Company, "Some company");
            SendText(RegistrationPage.Phone, "+123456789");
            SendText(RegistrationPage.Password, TestConfig.NewUser.Email);
            SendText(RegistrationPage.RepeatPassword, TestConfig.NewUser.Email);
            Click(RegistrationPage.AcceptTermsCheckBox);
            Click(RegistrationPage.RegisterButton);
            Assert.IsTrue(IsElementFound(RegistrationPage.CheckYourInboxDialog),
                @"No 'check your inbox' window displayed");
            Assert.IsTrue(IsElementFoundQuickly(RegistrationPage.OkButton),
                @"OK button not found on dialog window");
            Click(RegistrationPage.OkButton);
            _isConfirmationEmailHasSent = true;
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.LoginUrl),
                "User is not redirected to a login page");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT01310_GetRegistrationConfirmationEmail()
        {
            if (!_isConfirmationEmailHasSent)
            {
                RT01290_CheckInboxWindow();
            }
            if (!_isConfirmationEmailHasSent)
            {
                Assert.Warn("Cannot run because test method RT01290_CheckInboxWindow has failed");
            }

            WaitForNewMail(_mm.ClientCxM, _waitMailTimeoutSeconds);
            _activationLink = 
                _mm.GetLinkFromEmail(_mm.ClientCxM, MailManager.RegistrationConfirmationSubject);
            Assert.IsNotEmpty(_activationLink,
                @"New user haven't got an email with registration confirmation link or link not found");
        }

        [Test, Regression]
        public void RT01320_NavigateToUserActivationPage()
        {
            if (string.IsNullOrEmpty(_activationLink))
            {
                RT01310_GetRegistrationConfirmationEmail();
            }
            if (string.IsNullOrEmpty(_activationLink))
            {
                Assert.Warn("Cannot run because test method " +
                    "RT01310_GetRegistrationConfirmationEmail has failed");
            }
            ClearBrowserCache();
            NavigateTo(_activationLink);
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.LoginUrl),
                "User clicked activation link, but has not been redirected to a login page");
            _activationLinkClicked = true;
        }

        [Test, Regression]
        public void RT01330_NewUserLoginWithoutAdminActivation()
        {
            if (!_activationLinkClicked)
            {
                RT01320_NavigateToUserActivationPage();
            }

            if (!_activationLinkClicked)
            {
                Assert.Warn(@"Cannot run because test method RT01320_NavigateToUserActivationPage 
                    has failed");
            }
            Login(TestConfig.NewUser, isPressTenantButton: false);
            Assert.IsFalse(IsUserLoggedIn,
                "New user has logged in without user activation by UD administrator");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT01340_GetUserActivationEmailForUdAdmin()
        {
            if (!_activationLinkClicked)
            {
                RT01320_NavigateToUserActivationPage();
            }

            if (!_activationLinkClicked)
            {
                Assert.Warn("Cannot run because test method " + 
                            "RT01320_NavigateToUserActivationPage has failed");
            }

            WaitForNewMail(_mm.ClientUserDirectory, _waitMailTimeoutSeconds);
            _activationLinkUdAdmin =
                _mm.GetLinkFromEmail(_mm.ClientCxM, MailManager.NewUserRegisteredSubject);
            Assert.IsNotEmpty(_activationLinkUdAdmin,
                @"UD Admin haven't got an email with new user UD link or link not found");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT01350_ActivateUserAndFindEmailUserActivated()
        {
            if (!_activationLinkClicked)
            {
                RT01320_NavigateToUserActivationPage();
            }

            if (!_activationLinkClicked)
            {
                Assert.Warn("Cannot run because test method " 
                            + "RT01320_NavigateToUserActivationPage has failed");
            }
            UserDirectoryApi.SetUserStatus(TestConfig.NewUser, UserStatus.Active);
            WaitForNewMail(_mm.ClientCxM, _waitMailTimeoutSeconds);
            var isReceived = _mm.IsUserActivationEmailReceived();
            Assert.IsTrue(isReceived, "No user activated email received");        
            _isNewUserActivatedByAdmin = true;
        }

        [Test, Regression]
        public void RT01360_NewUserLoginWithoutPermissions()
        {
            if (!_isNewUserActivatedByAdmin)
            {
                RT01350_ActivateUserAndFindEmailUserActivated();
            }
            if (!_activationLinkClicked)
            {
                Assert.Warn("Cannot run because test method " + 
                            "RT01350_ActivateUserAndFindEmailUserActivated has failed");
            }
            Login(TestConfig.NewUser, isPressTenantButton: false);
            Assert.IsTrue(IsElementFound(LoginPage.ErrorNoRolesAndPermissionsAssigned),
                "Error 'You have no permissions assigned to you...' not displayed");
        }

        [Test, Regression]
        public void RT01370_AddPermissionsToNewUserAndLogin()
        {
            if(!_isNewUserActivatedByAdmin)
            {
                RT01350_ActivateUserAndFindEmailUserActivated();
            }
            if (!_activationLinkClicked)
            {
                Assert.Warn("Cannot run because test method " + 
                            "RT01350_ActivateUserAndFindEmailUserActivated has failed");
            }
            UserDirectoryApi.AssignRolesToUser(TestConfig.NewUser, new [] { UserRole.CxmAdmin });
            Login(TestConfig.NewUser, isPressTenantButton: false);
            Assert.IsTrue(IsElementNotFound(LoginPage.ErrorNoRolesAndPermissionsAssigned),
                "Error 'You have no permissions assigned to you...' is displayed");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestEnd().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public async Task EndFixtureTests()
        {
            var task1 = _mm.InboxHousekeepingAsync(_mm.ClientCxM);
            var task2 = _mm.InboxHousekeepingAsync(_mm.ClientUserDirectory);
            if (IsEachFixtureInNewBrowser)
            {
                ClosePrimaryBrowser();
            }
            if (_isConfirmationEmailHasSent)
            {
                UserDirectoryApi.DeleteUser(TestConfig.NewUser);
            }
            await task1.ConfigureAwait(false);
            await task2.ConfigureAwait(false);
            _mm.Dispose();            
        }
    }
}
