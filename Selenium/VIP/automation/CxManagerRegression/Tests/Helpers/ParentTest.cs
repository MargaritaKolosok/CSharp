using System.Linq;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Managers;

namespace Tests.Helpers
{
    /// <summary>
    /// Class that contains most use methods in tests and their settings
    /// </summary>
    public class ParentTest : TestHelper
    {
        /// <summary>
        /// Instructs test fixture to close browser on every test end
        /// </summary>
        protected bool IsEachTestInNewBrowser { private get; set; }

        /// <summary>
        /// Instructs test fixture to close browser on every test fixture end
        /// </summary>
        protected bool IsEachFixtureInNewBrowser = false;

        /// <summary>
        /// Starts current test: opens browser instance if none exists and logs in
        /// as <see cref="ActionManager.CurrentUser" /> when this allowed, login
        /// page is opened or nobody is logged in yet. Use this method (or overload)
        /// before first step of every UI test.
        /// </summary>
        /// <param name="url">URL to navigate in browser</param>
        /// <param name="doLogin">Whether <see cref="ActionManager.CurrentUser" />
        /// should log in or should not (optional)</param>
        protected void TestStart(string url, bool doLogin = true)
        {
            OpenPrimaryBrowser();
            NavigateTo(url);
            ClearBrowserCache();
            if ((!IsUserLoggedIn || IsPageContainsUri(TestConfig.LoginUrl, true)) && doLogin)
            {
                Login();
            }
        }

        /// <summary>
        /// <para>Starts current test: opens browser instance if none exists, logs in
        /// with current user credentials taken from property <see cref=
        /// "ActionManager.CurrentUser" /> if no user or different user logged in, and
        /// on Tenants page presses tenant button with name taken from property <see
        /// cref="ActionManager.CurrentUser" /> (if allowed to press).</para>
        /// <para>In case when current user is already logged in, but uses a tenant that
        /// differs to <see cref="ActionManager.CurrentTenant"/>, this method just changes
        /// current tenant to a tenant set in <see cref="ActionManager.CurrentTenant" />
        /// property.</para>
        /// Use this method (or its overload) before first step of every UI test.
        /// </summary>
        /// <param name="isSelectTenant">Whether press tenant button on Tenants page
        /// or not (optional)</param>
        protected void TestStart(bool isSelectTenant = true)
        {
            OpenPrimaryBrowser();
            if (!IsUserLoggedIn || !IsLoggedInAsCurrentUser())
            {
                FileManager.CloseGuiWindow();
                NavigateTo(TestConfig.LoginUrl);
                ClearBrowserCache();
                RefreshPage();
                if (isSelectTenant)
                {
                    Login(CurrentUser, CurrentTenant);
                }
                else
                {
                    Login(CurrentUser, isPressTenantButton: false);
                }
            }
            else
            {
                CloseModalDialogs();
                if (IsPageContainsUri(TestConfig.LoginUrl, doNotWait: true))
                {
                    if (isSelectTenant)
                    {
                        Login(CurrentUser, CurrentTenant);
                        return;
                    }
                    Login(CurrentUser, isPressTenantButton: false);
                }
                if (isSelectTenant)
                {
                    ChangeTenant(CurrentTenant);
                }
            }
        }

        /// <summary>
        /// Finalizes execution of every test. Use this method in TearDown section of
        /// every TestFixture
        /// </summary>
        protected async Task TestEnd()
        {
            var deleteAllTasksTask = BackgroundTaskApi.DeleteAllTasksAsync(CurrentUser);
            if (TestConfig.IsReportingEnabled)
            {               
                ReportManager.GenerateTestResultRecord();
            }

            CloseSecondaryBrowser();
            if (IsEachTestInNewBrowser)
            {
                ClosePrimaryBrowser();
                IsUserLoggedIn = false;
                await deleteAllTasksTask.ConfigureAwait(false);
                return;
            }

            var browserTabs = GetTabHandles();
            if (browserTabs == null || browserTabs.Count <= 1)
            {
                await deleteAllTasksTask.ConfigureAwait(false);
                return;
            }
            foreach (var browserTab in browserTabs.Skip(1))
            {
                CloseTab(browserTab);
            }
            await deleteAllTasksTask.ConfigureAwait(false);
        }
    }
}
