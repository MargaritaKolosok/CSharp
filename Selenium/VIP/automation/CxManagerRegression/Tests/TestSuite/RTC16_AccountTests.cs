using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Managers;
using Common.Resources;
using Models.UserDirectory;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public class RTC16_AccountTests : ParentTest
    {
        private bool _hasPasswordChanged;
        private UserData _admin2;
        private readonly string _oldPassword = TestConfig.AdminUser2.Password;
        private const string NewPassword = "Password!2";

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            _admin2 = UserDirectoryApi.GetUserData(TestConfig.AdminUser2);
            TestConfig.AdminUser2.GivenName = _admin2.GivenName;
            TestConfig.AdminUser2.FamilyName = _admin2.FamilyName;
            TestConfig.AdminUser2.Company = _admin2.Company;
            TestConfig.AdminUser2.Phone = _admin2.Phone;
        }

        [SetUp]
        public async Task Setup()
        {
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }

            CurrentUser = TestConfig.AdminUser2;
        }

        private void OpenAccountForm()
        {
            if (IsElementFoundQuickly(PageHeader.UserMenuButton))
            {
                MouseOver(PageHeader.UserMenuButton);
                Click(PageHeader.UserMenuAccountButton);
            }
        }

        [Test, Regression]
        public void RT16010_AccountFormOpened()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            Assert.IsTrue(IsElementFound(Account.AccountDialog), "Account dialog should be opened");
        }

        [Test, Regression]
        public void RT16040_AccountFormElementsExist()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            Assert.IsTrue(IsElementFound(Account.GivenName), "Given Name field is absent");
            Assert.IsTrue(IsElementFound(Account.FamilyName), "Family Name field is absent");
            Assert.IsTrue(IsElementFound(Account.Company), "Company field is absent");
            Assert.IsTrue(IsElementFound(Account.Phone), "Phone field is absent");
            Assert.IsTrue(IsElementFound(Account.ChangePasswordButton), 
                "Change Password group button is absent");
            Click(Account.ChangePasswordButton);
            Assert.IsTrue(IsElementFound(Account.OldPassword),
                "Change Password > Old password field is absent");
            Assert.IsTrue(IsElementFound(Account.NewPassword),
                "Change Password > New password field is absent");
            Assert.IsTrue(IsElementFound(Account.PasswordRepeat),
                "Change Password > Password Repeat field is absent");
            Assert.IsTrue(IsElementFound(Account.SubmitButton), "Submit button is absent");
            Assert.IsTrue(IsElementFound(Account.CancelButton), "Cancel button is absent");
        }

        [Test, Regression]
        public void RT16050_AccountFormElementsText()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            Assert.IsTrue(IsElementEquals(Account.GivenName, TestConfig.AdminUser2.GivenName), 
                $@"Given Name field should be equal '{TestConfig.AdminUser2.GivenName}'");
            Assert.IsTrue(IsElementEquals(Account.FamilyName, TestConfig.AdminUser2.FamilyName),
                $@"Family Name field should be equal '{TestConfig.AdminUser2.FamilyName}'");
            Assert.IsTrue(IsElementEquals(Account.Company, TestConfig.AdminUser2.Company),
                $@"Company field should be equal '{TestConfig.AdminUser2.Company}'");
            Assert.IsTrue(IsElementEquals(Account.Phone, TestConfig.AdminUser2.Phone),
                $@"Phone field should be equal '{TestConfig.AdminUser2.Phone}'");
        }

        [Test, Regression]
        public void RT16070_AllFieldsMandatoryValidation()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            if (IsElementFoundQuickly(Account.ChangePasswordButton))
            {
                Click(Account.ChangePasswordButton);
            }
            SendText(Account.OldPassword, TestConfig.AdminUser2.Password);
            SendText(Account.NewPassword, NewPassword);
            SendText(Account.PasswordRepeat, NewPassword);
            ClearTextInElement(Account.GivenName);
            ClearTextInElement(Account.FamilyName);
            ClearTextInElement(Account.Company);
            ClearTextInElement(Account.Phone);
            ClearTextInElement(Account.OldPassword);
            ClearTextInElement(Account.NewPassword);
            ClearTextInElement(Account.PasswordRepeat);
            Assert.IsTrue(IsElementFoundQuickly(Account.ErrorGivenNameRequired),
                "No error 'Given name required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(Account.ErrorFamilyNameRequired),
                "No error 'Family name required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(Account.ErrorCompanyRequired),
                "No error 'Company is required' displayed");
            Assert.IsTrue(IsElementFoundQuickly(Account.ErrorPhoneRequired),
                "No error 'Phone required' displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(Account.ErrorOldPasswordRequired)
                    && IsElementNotFoundQuickly(Account.ErrorPasswordsDoNotMatch)
                    && IsElementNotFoundQuickly(Account.ErrorPasswordRepeatRequired)
                    && IsElementNotFoundQuickly(Account.ErrorNewPasswordRequired),
                "No errors related to password fields should be displayed");
        }

        [Test, Regression]
        public void RT16080_CancelAccountForm()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            Click(Account.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(Account.AccountDialog),
                "Account dialog should be closed");
        }

        [Test, Regression]
        public void RT16090_NewAccountFormAndNoErrors()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            Assert.IsTrue(IsElementNotFoundQuickly(CommonElement.ValidationError),
                "Just opened Account dialog contains strange error message(s)");
        }

        [Test, Regression]
        public void RT16140_InvalidGivenName()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "1");
            Assert.IsTrue(IsElementFound(Account.ErrorMustHave3Chars),
                @"Error message 'This must have at least 3 chars' is not displayed");
        }

        [Test, Regression]
        public void RT16150_ValidGivenName()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            Assert.IsTrue(IsElementNotFound(Account.ErrorMustHave3Chars),
                @"Error message 'This must have at least 3 chars' is displayed");
        }

        [Test, Regression]
        public void RT16160_InvalidFamilyName()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "1");
            Assert.IsTrue(IsElementFound(Account.ErrorMustHave3Chars),
                @"Error message 'This must have at least 3 chars' is not displayed");
        }

        [Test, Regression]
        public void RT16170_ValidLastName()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            Assert.IsTrue(IsElementNotFound(Account.ErrorMustHave3Chars),
                @"Error message 'This must have at least 3 chars' is not displayed");
        }

        [Test, Regression]
        public void RT16180_InvalidCompany()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "1");
            Assert.IsTrue(IsElementFound(Account.ErrorMustHave3Chars),
                @"Error message 'This must have at least 3 chars' is not displayed");
        }

        [Test, Regression]
        public void RT16190_ValidCompany()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            Assert.IsTrue(IsElementNotFound(Account.ErrorMustHave3Chars),
                @"Error message 'This must have at least 3 chars' is displayed");
        }

        [Test, Regression]
        public void RT16200_InvalidPhonePattern1()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "shdhjhgms");
            Assert.IsTrue(IsElementFound(Account.ErrorDoesNotMatchPattern),
                @"Error message 'This does not match the expected pattern' is not displayed");
        }

        [Test, Regression]
        public void RT16210_InvalidPhonePattern2()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "123456789");
            Assert.IsTrue(IsElementFound(Account.ErrorDoesNotMatchPattern),
                @"Error message 'This does not match the expected pattern' is not displayed");
        }

        [Test, Regression]
        public void RT16220_InvalidPhoneLengthTooShort()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "+12345");
            Assert.IsTrue(IsElementFound(Account.ErrorMustHave9Chars),
                @"Error message 'This must have at least 9 chars' is not displayed");
        }

        [Test, Regression]
        public void RT16230_InvalidPhoneLengthTooLong()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "+12345678901234567890");
            Assert.IsTrue(IsElementFound(Account.ErrorNotMore20Chars),
                @"Error message 'Not more than 20 characters are allowed' is not displayed");
        }

        [Test, Regression]
        public void RT16240_InvalidOldPasswordLength()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "+123456789");
            if (IsElementFoundQuickly(Account.ChangePasswordButton))
            {
                Click(Account.ChangePasswordButton);
            }
            SendText(Account.OldPassword, "dsfsd");
            Assert.IsTrue(IsElementFound(Account.ErrorMustHave8Chars),
                @"Error message 'This must have at least 8 chars' not displayed");
        }

        [Test, Regression]
        public void RT16241_InvalidNewPasswordLength()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "+123456789");
            if (IsElementFoundQuickly(Account.ChangePasswordButton))
            {
                Click(Account.ChangePasswordButton);
            }
            SendText(Account.OldPassword, TestConfig.AdminUser2.Password);
            SendText(Account.NewPassword, "dgdfg");
            Assert.IsTrue(IsElementFound(Account.ErrorMustHave8Chars),
                @"Error message 'This must have at least 8 chars' not displayed");
        }

        [Test, Regression]
        public void RT16242_InvalidPasswordRepeatLength()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "+123456789");
            if (IsElementFoundQuickly(Account.ChangePasswordButton))
            {
                Click(Account.ChangePasswordButton);
            }
            SendText(Account.OldPassword, TestConfig.AdminUser2.Password);
            SendText(Account.NewPassword, TestConfig.AdminUser2.Password);
            SendText(Account.PasswordRepeat, "gdgdfg");
            Assert.IsTrue(IsElementFound(Account.ErrorMustHave8Chars),
                @"Error message 'This must have at least 8 chars' not displayed");
            Assert.IsTrue(IsElementFound(Account.ErrorPasswordsDoNotMatch),
                @"Error message 'Passwords do not match' not displayed");
        }

        [Test, Regression]
        public void RT16250_PasswordsDoNotMatch()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "+123456789");
            if (IsElementFoundQuickly(Account.ChangePasswordButton))
            {
                Click(Account.ChangePasswordButton);
            }
            SendText(Account.OldPassword, TestConfig.AdminUser2.Password);
            SendText(Account.NewPassword, "11111111");
            SendText(Account.PasswordRepeat, "22222222");
            Assert.IsTrue(IsElementNotFound(Account.ErrorMustHave8Chars),
                @"Error message 'This must have at least 8 chars' should not be displayed");
            Assert.IsTrue(IsElementFound(Account.ErrorPasswordsDoNotMatch),
                @"Error message 'Passwords do not match' not displayed");
        }

        [Test, Regression]
        public void RT16280_PasswordBlacklisted()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "+123456789");
            if (IsElementFoundQuickly(Account.ChangePasswordButton))
            {
                Click(Account.ChangePasswordButton);
            }
            SendText(Account.OldPassword, TestConfig.AdminUser2.Password);
            SendText(Account.NewPassword, "11111111");
            SendText(Account.PasswordRepeat, "11111111");
            Click(Account.SubmitButton);
            Assert.IsTrue(IsElementFound(Account.PasswordBlacklistedDialog),
                @"Error message 'Entered password is blacklisted...' is not displayed");
        }

        [Test, Regression]
        public void RT16290_WrongOldPassword()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.GivenName, "Auto");
            SendText(Account.FamilyName, "Test");
            SendText(Account.Company, "Some company");
            SendText(Account.Phone, "+123456789");
            if (IsElementFoundQuickly(Account.ChangePasswordButton))
            {
                Click(Account.ChangePasswordButton);
            }
            SendText(Account.OldPassword, "dfsdfsdf");
            SendText(Account.NewPassword, NewPassword);
            SendText(Account.PasswordRepeat, NewPassword);
            Click(Account.SubmitButton);
            Assert.IsTrue(IsElementFound(Account.OldPasswordIncorrectDialog),
                @"No 'The old password you've entered is incorrect' window displayed");
            Click(Account.OkButton2);
            Assert.IsTrue(IsElementFoundQuickly(Account.GivenName), "Account dialog should stay open");
        }

        [Test, Regression]
        public void RT16300_PasswordChangeSuccess()
        {
            TestStart(TestConfig.LoginUrl);
            OpenAccountForm();
            SendText(Account.Phone, "+123456789");
            if (IsElementFoundQuickly(Account.ChangePasswordButton))
            {
                Click(Account.ChangePasswordButton);
            }
            SendText(Account.OldPassword, _oldPassword);
            SendText(Account.NewPassword, NewPassword);
            SendText(Account.PasswordRepeat, NewPassword);
            Click(Account.SubmitButton);
            Assert.IsTrue(IsElementNotFound(Account.AccountDialog),
                "Account dialog should be closed");
            _hasPasswordChanged = true;
            TestConfig.AdminUser2.Password = NewPassword;
        }

        [Test, Regression]
        public void RT16310_LoginWithOldPassword()
        {
            if (!_hasPasswordChanged)
            {
                RT16300_PasswordChangeSuccess();
            }
            if (!_hasPasswordChanged)
            {
                Assert.Warn("Cannot run because test method RT16300_PasswordChangeSuccess has failed");
            }

            TestStart(TestConfig.LoginUrl, doLogin: false);
            Login(TestConfig.AdminUser2.Email, _oldPassword);
            Assert.IsTrue(IsElementFound(LoginPage.ErrorWrongUserOrPassword),
                @"Error 'User does not exist or password did not match' is not displayed");
        }

        [Test, Regression]
        public void RT16320_LoginWithNewPassword()
        {
            if (!_hasPasswordChanged)
            {
                RT16300_PasswordChangeSuccess();
            }
            if (!_hasPasswordChanged)
            {
                Assert.Warn(
                    "Cannot run because test method RT16300_PasswordChangeSuccess has failed");
            }

            TestStart(TestConfig.LoginUrl, doLogin: false);
            Login(TestConfig.AdminUser2.Email, NewPassword);
            Assert.IsTrue(IsElementNotFound(LoginPage.ErrorWrongUserOrPassword),
                @"Error 'User does not exist or password did not match' is displayed");
            if (IsElementFoundQuickly(LoginPage.ErrorAccountLocked))
            {
                WaitTime(60);
                Login(TestConfig.AdminUser2.Email, NewPassword);
            }
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.TenantsUrl),
                "User should be redirected to Tenants page");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestEnd().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void EndFixtureTests()
        {
            _admin2.Password = _oldPassword;
            UserDirectoryApi.SaveUser(_admin2);
            TestConfig.AdminUser2.Password = _oldPassword;
            if (IsEachFixtureInNewBrowser)
            {
                ClosePrimaryBrowser();
            }           
        }
    }
}
