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
    public sealed class RTC02_LoginTests : ParentTest
    {
        private string _newPassword;
        private bool _isForgotPasswordEmailHasSent, _isAccountLocked;
        private const int LongTestTimeoutMilliseconds = 300000;
        private readonly int _waitMailTimeoutSeconds;
        private readonly MailManager _mm = new MailManager();

        public RTC02_LoginTests()
        {
            _waitMailTimeoutSeconds = LongTestTimeoutMilliseconds / 1000 - 10;
        }

        [OneTimeSetUp]
        public async Task Begin()
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
            TestStart(TestConfig.LoginUrl, doLogin: false);
        }

        [Test, Regression]
        public void RT02010_LoginPageOpened()
        {
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.LoginUrl),
                "User is not redirected to login page");
        }

        [Test, Regression]
        public void RT02020_LoginPageElementsExist()
        {
            Assert.IsTrue(IsElementFound(LoginPage.Logo),
                "Logo is absent");
            Assert.IsTrue(IsElementFound(LoginPage.LoginEmail),
                "Email field is absent");
            Assert.IsTrue(IsElementFound(LoginPage.LoginPassword),
                "Password field is absent");
            Assert.IsTrue(IsElementFound(LoginPage.LoginButton),
                "Login button is absent");
            Assert.IsTrue(IsElementFound(LoginPage.RegisterButton),
                "Register button is absent");
            Assert.IsTrue(IsElementFound(LoginPage.ForgotPasswordButton),
                "Forgot Password button is absent");
            Assert.IsTrue(IsDefaultElement(LoginPage.LoginButton),
                "Login is not a default button");
        }

        [Test, Regression]
        public void RT02030_EmailValidation()
        {
            SendText(LoginPage.LoginEmail, "ewrrw.com");
            Assert.IsTrue(IsElementFound(LoginPage.ErrorEmailDoesntMatch),
                @"Error message 'Email doesn't match pattern' not displayed");
        }

        [Test, Regression]
        public void RT02040_InvalidPassword()
        {
            Login(TestConfig.NewUser.Email, "somewrongvalue");
            Assert.IsTrue(IsElementFound(LoginPage.ErrorWrongUserOrPassword),
                @"Error message 'User does not exist or password did not match' not displayed");
        }

        [Test, Regression]
        public void RT02050_InvalidEmailAndPassword()
        {
            Login("freak@hauz.xxx", "somewrongvalue");
            Assert.IsTrue(IsElementFound(LoginPage.ErrorWrongUserOrPassword),
                @"Error message 'User does not exist or password did not match' not displayed");
        }

        [Test, Regression]
        public void RT02060_EmptyEmailAndPassword()
        {
            SendText(LoginPage.LoginEmail, "1");
            ClearTextInFocusedElement();
            SendText(LoginPage.LoginPassword, "1");
            ClearTextInFocusedElement();
            Assert.IsTrue(IsElementFound(LoginPage.ErrorEmailIsRequired),
                @"Error message 'Email is required' not displayed");
            Assert.IsTrue(IsElementFound(LoginPage.ErrorPasswordIsRequired),
                @"Error message 'Password is required' not displayed");
        }

        [Test, Regression]
        public void RT02070_ValidEmailAndEmptyPassword()
        {
            SendText(LoginPage.LoginEmail, TestConfig.AdminUser.Email);
            SendText(LoginPage.LoginPassword, "AAAAAAAAAAAAAA");
            ClearTextInFocusedElement();
            Assert.IsTrue(IsElementFound(LoginPage.ErrorPasswordIsRequired),
                @"Error message 'Password is required' not displayed");
        }

        [Test, Regression]
        public void RT02080_EmptyEmailAndValidPassword()
        {
            SendText(LoginPage.LoginEmail, "a@b.cc");
            ClearTextInFocusedElement();
            SendText(LoginPage.LoginPassword, TestConfig.AdminUser.Password);
            Click(LoginPage.LoginButton);
            Assert.IsTrue(IsElementFound(LoginPage.ErrorEmailIsRequired),
                @"Error message 'Email is required' not displayed");
        }

        [Test, Regression]
        public void RT02090_LoginAsDisabledUser()
        {
            Login(TestConfig.DisabledUser, isPressTenantButton: false);
            Assert.IsTrue(IsElementFound(LoginPage.ErrorAccountHasNotBeenActivated),
                "Error 'Your account has not been activated yet' not displayed");
        }

        [Test, Regression]
        public void RT02100_LoginAsNoTenantUser()
        {
            Login(TestConfig.NoTenantUser, isPressTenantButton: false);
            Assert.IsTrue(IsElementFound(LoginPage.ErrorNoRolesAndPermissionsAssigned),
                "Error 'You have no roles and permissions assigned to you...' not displayed");
        }

        [Test, Regression]
        public void RT02110_LoginAsOneTenantUser()
        {
            SendText(LoginPage.LoginEmail, TestConfig.OneTenantUser.Email);
            PressKeys(Keys.Tab);
            Assert.IsTrue(IsFocused(LoginPage.LoginPassword),
                "Password field has no focus");
            PressKeys(TestConfig.OneTenantUser.Password);
            PressKeys(Keys.Enter);
            Assert.IsTrue(IsElementNotFound(TenantsPage.TenantButton),
                "Tenant window is displayed");
            Assert.IsTrue(IsElementNotFound(LoginPage.LoginEmail),
                "User is not logged in");
            Assert.IsTrue(IsPageContainsUri(TestConfig.PlacesUri),
                "Places page is not opened after login");
            Assert.IsTrue(IsElementNotFoundQuickly(PageHeader.BreadCrumbsWebElement),
                "Tenants breadcrumbs is present");
            Logout();
        }

        [Test, Regression]
        public void RT02120_TenantWindowOpened()
        {
            Login(TestConfig.MultiTenantUser, isPressTenantButton: false);
            Assert.IsTrue(IsElementFound(TenantsPage.TenantButton),
                "Tenant window is absent after user login");
        }

        [Test, Regression]
        public void RT02130_TenantSelectionCanceled()
        {
            Login(TestConfig.MultiTenantUser, isPressTenantButton: false);
            Click(LoginPage.CancelButton);
            Assert.IsTrue(IsElementFound(LoginPage.LoginEmail),
                "Login window is absent after tenant selection cancellation");
        }

        [Test, Regression]
        public void RT02140_TenantAvailableInBreadCrumbs()
        {
            Login(TestConfig.MultiTenantUser, isPressTenantButton: false);
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.TenantsUrl),
                "Tenants page is not opened");
            var tenantName = GetValue(TenantsPage.TenantButton);
            Click(TenantsPage.TenantButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.PlacesUri),
                "Places page is not opened after login");
            Assert.IsTrue(IsElementFound(PageHeader.BreadCrumbsWebElement),
                "Tenant breadcrumbs element is absent");
            Assert.IsTrue(IsElementFound(string.Format(PageHeader.BreadCrumb, tenantName)),
                $@"Tenant breadcrumbs element does not contain current tenant '{tenantName}'");
        }

        [Test, Regression]
        public void RT02150_ForgotPassword()
        {
            Click(LoginPage.ForgotPasswordButton);
            Assert.IsTrue(IsElementFound(ForgotPasswordPage.Email),
                "Email field is absent");
            Assert.IsTrue(IsElementFound(ForgotPasswordPage.CloseButton),
                "Close button is absent");
            Assert.IsTrue(IsElementFound(ForgotPasswordPage.ResetPasswordDisabledButton),
                "Reset Password button is absent");
        }

        [Test, Regression]
        public void RT02160_CloseForgotPassword()
        {
            Click(LoginPage.ForgotPasswordButton);
            Click(ForgotPasswordPage.CloseButton);
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.LoginUrl),
                "Login page is not opened");
        }

        [Test, Regression]
        public void RT02170_ResetPasswordWrongEmail()
        {
            Click(LoginPage.ForgotPasswordButton);
            SendText(ForgotPasswordPage.Email, "non-existinguser@domain.com");
            Click(ForgotPasswordPage.ResetPasswordButton);
            Assert.IsTrue(IsElementFound(ForgotPasswordPage.ErrorUserNotExist),
                "Error 'This user does not exist' not displayed");
        }

        [Test, Regression]
        public void RT02180_ForgotPageInvalidEmail()
        {
            Click(LoginPage.ForgotPasswordButton);
            SendText(ForgotPasswordPage.Email, "ewrrw.com");
            Assert.IsTrue(IsElementFound(ForgotPasswordPage.ErrorEmailDoesntMatch),
                @"Error message 'Email doesn't match pattern' not displayed");
        }

        [Test, Regression]
        public void RT02190_EmptyEmailValidation()
        {
            Click(LoginPage.ForgotPasswordButton);
            SendText(ForgotPasswordPage.Email, "sometext");
            ClearTextInFocusedElement();
            Assert.IsTrue(IsElementFoundQuickly(LoginPage.ErrorEmailIsRequired),
                @"Error message 'Email is required' not displayed");
        }

        [Test, Regression]
        public void RT02200_ResetPasswordDisabledUser()
        {
            Click(LoginPage.ForgotPasswordButton);
            SendText(ForgotPasswordPage.Email, TestConfig.DisabledUser.Email);
            Click(ForgotPasswordPage.ResetPasswordButton);
            Assert.IsTrue(IsElementFound(ForgotPasswordPage.ErrorUserNotActivated),
                @"Error 'Your account hasn't been activated yet' not displayed");
        }

        [Test, Regression]
        public void RT02210_RequestNewPassword()
        {
            _mm.InboxHousekeeping(_mm.ClientUserDirectory);
            Click(LoginPage.ForgotPasswordButton);
            SendText(ForgotPasswordPage.Email, TestConfig.AdminUserDirectory.Email);
            Click(ForgotPasswordPage.ResetPasswordButton);
            Assert.IsTrue(IsElementFound(ForgotPasswordPage.CheckInboxWindow),
                @"No 'check your inbox' window displayed");
            Click(ForgotPasswordPage.OkButton);
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.LoginUrl),
                "User has not been redirected to a login page");
            _isForgotPasswordEmailHasSent = true;
        }

        /// <summary>
        /// To get a temporary user password User Directory administrator's mail account is used.
        /// Reason: limited number of mail boxes available.
        /// </summary>
        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT02230_GetNewPasswordEmail()
        {
            if (!_isForgotPasswordEmailHasSent)
            {
                RT02210_RequestNewPassword();
            }
            if (!_isForgotPasswordEmailHasSent)
            {
                Assert.Warn("Cannot run because test method RT02210_RequestNewPassword has failed");
            }

            WaitForNewMail(_mm.ClientUserDirectory, _waitMailTimeoutSeconds);
            _newPassword = _mm.GetPasswordFromEmail(
                        _mm.ClientUserDirectory, MailManager.PasswordResetSubject);
            Assert.IsTrue(!string.IsNullOrEmpty(_newPassword),
                @"Haven't got an email with temporary password");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT02240_LoginWithNewPassword()
        {
            if (string.IsNullOrEmpty(_newPassword))
            {
                RT02230_GetNewPasswordEmail();
            }
            if (!_isForgotPasswordEmailHasSent || string.IsNullOrEmpty(_newPassword))
            {
                Assert.Warn(@"Cannot run because test method(s) RT02210_RequestNewPassword 
                    or RT02230_GetNewPasswordEmail have failed");
            }
            Login(TestConfig.AdminUserDirectory.Email, _newPassword);
            Assert.IsTrue(IsUserLoggedIn,
                "Cannot login with temporary password");
        }

        [Test, Regression]
        public void RT02250_LoginComposerOnlyUser()
        {
            Login(TestConfig.ComposerOnlyUser, isPressTenantButton: false);
            Assert.IsTrue(IsElementFoundQuickly(LoginPage.RequestAccessToCxmWindow),
                "Request access dialog is not displayed");
        }

        [Test, Regression]
        public void RT02260_LoginComposerOnlyUserCancelAccessRequest()
        {
            Login(TestConfig.ComposerOnlyUser, isPressTenantButton: false);
            Click(LoginPage.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(LoginPage.RequestAccessToCxmWindow),
                "Request access to CX Manager dialog is still opened");
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.LoginUrl), "User is not redirected to Login page");
        }

        [Test, Regression]
        public void RT02270_ResetPasswordComposerOnlyUser()
        {
            Click(LoginPage.ForgotPasswordButton);
            SendText(ForgotPasswordPage.Email, TestConfig.ComposerOnlyUser.Email);
            Click(ForgotPasswordPage.ResetPasswordButton);
            Assert.IsTrue(IsElementFound(ForgotPasswordPage.ErrorNoPermissionsAssigned),
                "No error message 'You have no permissions assigned to you' on reset password request");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT02280_LoginComposerOnlyUserRequestAccess()
        {
            _mm.InboxHousekeeping(_mm.ClientUserDirectory);
            Login(TestConfig.ComposerOnlyUser, isPressTenantButton: false);
            Click(LoginPage.RequestAccessButton);
            Assert.IsTrue(IsElementFoundQuickly(LoginPage.RequestSentWindow),
                "Request sent confirmation window is absent");
            Assert.IsTrue(IsElementFoundQuickly(LoginPage.CloseButton),
                @"Close button is not present on modal 'Request sent'");
            Click(LoginPage.CloseButton);
            ShowAlert("Waiting for email...");
            WaitForNewMail(_mm.ClientUserDirectory, _waitMailTimeoutSeconds);
            CloseAlert();
            var emailReceived = _mm.IsAccessRequestEmailReceived();
            Assert.IsTrue(emailReceived,
                @"Haven't got an email with temporary password");
        }

        [Test, Regression]
        public void RT02290_LoginComposerOnlyCloseConfirmation()
        {
            Login(TestConfig.ComposerOnlyUser, isPressTenantButton: false);
            Click(LoginPage.RequestAccessButton);
            Click(LoginPage.CloseButton);
            Assert.IsTrue(IsElementNotFoundQuickly(LoginPage.RequestSentWindow),
                "Request sent confirmation window is still open");
        }

        [Test, Regression]
        public void RT02310_UdAdminAssignsRoleWithLoginAndUserCanLogin()
        {
            UserDirectoryApi.AssignRolesToUser(TestConfig.ComposerOnlyUser, new [] {UserRole.ComposerAdmin, UserRole.CxmAdmin});
            Login(TestConfig.ComposerOnlyUser, isPressTenantButton: false);
            Assert.IsTrue(IsPageRedirectedTo(TestConfig.TenantsUrl),
                @"Composer user with granted login permission and tenants cannot login to CX Manager");

            UserDirectoryApi.AssignRolesToUser(
                TestConfig.ComposerOnlyUser, new[] { UserRole.ComposerAdmin });
        }

        [Test, Regression]
        public void RT02350_WrongPasswordAccountLockedError()
        {
            Login(TestConfig.AdminUser2.Email, $"Password!{RandomNumberWord}");
            Click(LoginPage.LoginButton);
            Click(LoginPage.LoginButton);
            Assert.IsTrue(IsElementFoundQuickly(LoginPage.ErrorAccountLocked),
                "Error 'Due to 3 wrong password attempts, your account has been locked for ...' should be displayed");
            _isAccountLocked = true;
        }

        [Test, Regression]
        public void RT02360_WrongPasswordAccountStillLocked()
        {
            if (!_isAccountLocked)
            {
                RT02350_WrongPasswordAccountLockedError();
            }

            Login(TestConfig.AdminUser2, isPressTenantButton: false);
            Assert.IsTrue(IsElementFoundQuickly(LoginPage.ErrorAccountLocked),
                "Error 'Due to 3 wrong password attempts, your account has been locked for ...' should be " + 
                "still displayed until countdown is over");
        }

        [Test, Regression]
        public void RT02370_AccountUnlocked()
        {
            if (!_isAccountLocked)
            {
                RT02360_WrongPasswordAccountStillLocked();
            }

            WaitTime(60);
            Login(TestConfig.AdminUser2, isPressTenantButton: false);
            Assert.IsTrue(IsElementNotFoundQuickly(LoginPage.ErrorAccountLocked)
                    && IsPageRedirectedTo(TestConfig.TenantsUrl),
                "User should be able to log in with correct password after lock timeout");
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
            await task1.ConfigureAwait(false);
            await task2.ConfigureAwait(false);
            _mm.Dispose();
        }
    }
}
