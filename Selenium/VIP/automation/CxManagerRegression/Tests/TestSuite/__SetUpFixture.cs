using System.Diagnostics;
using System.Threading.Tasks;
using Api.Controllers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using NUnit.Framework;

namespace Tests.TestSuite
{
    /// <summary>
    /// The main setup class for NUnit 3.x test suite. To apply <see cref="BeginExecutionAsync"/>
    /// and <see cref="EndExecutionAsync"/> to all test fixtures, they are all must belong to the
    /// same namespace as this class belongs to.
    /// </summary> 
    [SetUpFixture]
    public class GlobalSetUpFixture
    {
        /// <summary>
        /// Starts before test run
        /// </summary>
        /// <returns>(<see cref="Task"/>) Task object</returns>
        [OneTimeSetUp]
        public async Task BeginExecutionAsync()
        {
            TestContext.Progress.WriteLine("****** Begin test run ******");
            await RestController.StartClientAsync().ConfigureAwait(false);
            ActionManager.CurrentUser = TestConfig.AdminUser;
            ActionManager.CurrentTenant = TenantTitle.manylang;
            
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.InitReportAsync(TestContext.Parameters.Get("build")).ConfigureAwait(false);
            }

            if (TestConfig.IsDeleteExpiredReports)
            {
                await ReportManager.DeleteExpiredReportsAsync().ConfigureAwait(false);
            }

            await KillWebDriverProcessesAsync().ConfigureAwait(false);
            await SignalRController.StartClientAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Finalizes test run
        /// </summary>
        /// <returns>(<see cref="Task"/>) Task object</returns>
        [OneTimeTearDown]
        public async Task EndExecutionAsync()
        {
            if (TestConfig.IsReportingEnabled)
            {
                ReportManager.FinalizeReport();
            }

            WindowManager.ClosePrimaryBrowser();
            await KillWebDriverProcessesAsync().ConfigureAwait(false);
            TestContext.Progress.WriteLine("****** End of test run ******");
        }

        /// <summary>
        /// Silently kills browser drivers processes if they are still running
        /// </summary>
        /// <returns>(<see cref="Task"/>) Task object</returns>
        private static Task KillWebDriverProcessesAsync()
        {
            return Task.Run(() =>
            {
                var procNames = new []
                {
                    "chromedriver",
                    "geckodriver"
                };

                foreach (var procName in procNames)
                {
                    foreach (var process in Process.GetProcessesByName(procName))
                    {
                        process.Kill();
                    }
                }
            });
        }
    }
}
