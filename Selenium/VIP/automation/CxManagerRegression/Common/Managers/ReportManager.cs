using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Common.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Common.Managers
{
    /// <summary>
    /// Creates test run HTML report. 
    /// </summary>
    public static class ReportManager
    {
        private static string _currentReportPath;
        private static ExtentReports _extentReport;
        private static ExtentTest _test;
        private const string Mask = "yyyyMMddHHmmss";

        /// <summary>
        /// Creates a new test folder and initializes a new HTML report within.
        /// Folder name will equal to "build" NUnit Console command line parameter. 
        /// If NUnit Console command line does not have "build" parameter or 
        /// test(s) run in VS, new folder will be created using template <see cref="Mask"/>.
        /// </summary>
        /// <param name="buildNo">"build" parameter in NUnit Console command line (optional)</param>
        public static Task InitReportAsync(string buildNo)
        {
            return Task.Run(() =>
            {
                var workDir = !string.IsNullOrEmpty(buildNo) ? buildNo : DateTime.Now.ToString(Mask);
                _currentReportPath = Path.GetFullPath(
                    Path.Combine(TestConfig.ReportParentFolder, workDir));

                try
                {
                    if (!Directory.Exists(_currentReportPath))
                    {
                        Directory.CreateDirectory(_currentReportPath);
                    }
                }
                catch 
                {
                    throw new AggregateException(
                        $@"Error creating report folder {_currentReportPath}. Check permissions.");
                }

                var firstDotPosition = TestContext.CurrentContext.Test.ClassName.IndexOf('.');

                // report initialization
                ExtentHtmlReporter htmlReporter;
                try
                {
                    htmlReporter = new ExtentHtmlReporter(Path.Combine(_currentReportPath, "index.html"));
                }
                catch
                {
                    throw new AggregateException(
                        $@"Error creating of report document in {_currentReportPath}. Check permissions.");
                }

                var pathConfig = Path.GetFullPath(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        $@"..\..\..\{
                                TestContext.CurrentContext.Test.ClassName.Substring(0, firstDotPosition)
                                }\extent-config.xml"));
                try
                {
                    htmlReporter.LoadConfig(pathConfig);
                }
                catch
                {
                    Trace.TraceWarning(
                        $"Extent Report config file {pathConfig} is damaged or not found");
                }

                _extentReport = new ExtentReports();
                //_extentReport.AnalysisStrategy = AnalysisStrategy.Class;
                _extentReport.AttachReporter(htmlReporter);
            });
        }

        /// <summary>
        /// Create a new test record in current report
        /// </summary>
        public static Task CreateTestAsync()
        {
            return Task.Run(() =>
            {
                var firstDotPosition = TestContext.CurrentContext.Test.ClassName.IndexOf('.');
                _test = _extentReport.CreateTest(
                    $@"{TestContext.CurrentContext.Test.ClassName.Substring(firstDotPosition + 1)}.{
                        TestContext.CurrentContext.Test.MethodName}");
            });
        }

        /// <summary>
        /// Gets test result and write it to a report file 
        /// </summary>
        public static void GenerateTestResultRecord()
        {
            Status logStatus;
            var screenshotFileName =
                Path.Combine(_currentReportPath, $"{TestContext.CurrentContext.Test.FullName}.png");
            switch (TestContext.CurrentContext.Result.Outcome.Status)
            {
                case TestStatus.Failed:
                    logStatus = Status.Fail;
                    TakeScreenshot(screenshotFileName);
                    if (File.Exists(screenshotFileName))
                        _test.AddScreenCaptureFromPath(screenshotFileName);
                    break;
                case TestStatus.Inconclusive:
                    logStatus = Status.Warning;
                    TakeScreenshot(screenshotFileName);
                    if (File.Exists(screenshotFileName))
                        _test.AddScreenCaptureFromPath(screenshotFileName);
                    break;
                case TestStatus.Warning:
                    logStatus = Status.Fail;
                    TakeScreenshot(screenshotFileName);
                    if (File.Exists(screenshotFileName))
                        _test.AddScreenCaptureFromPath(screenshotFileName);
                    break;
                case TestStatus.Skipped:
                    logStatus = Status.Skip;
                    break;
                case TestStatus.Passed:
                    logStatus = Status.Pass;
                    break;
                default:
                    logStatus = Status.Pass;
                    break;
            }

            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                ? string.Empty
                : $@"{TestContext.CurrentContext.Result.Message}<br/>Stack trace:<br/> 
                    {TestContext.CurrentContext.Result.StackTrace}";
            var testResultDetails = $"Test ended with {logStatus}";
            if (!string.IsNullOrEmpty(stacktrace))
            {
                testResultDetails += $@"<br/>Message: {stacktrace}";
            }
            // add test result and other info 
            _test.Log(logStatus, testResultDetails.Replace(@"\n", @"<br/>"));
            // write test result to report file
            try
            {
                _extentReport.Flush();
            }
            catch
            {
                Trace.TraceWarning(
                    $"Error updating report {_currentReportPath}. Check permissions.");
            }
    }

        /// <summary>
        /// Takes screenshot and saves it to file 
        /// </summary>
        /// <param name="screenshotFileName">Screenshot file name with path</param>
        private static void TakeScreenshot(string screenshotFileName)
        {
            try
            {
                //var ss = ((ITakesScreenshot) Browser.CurrentDriver).GetScreenshot();
                //ss.SaveAsFile(screenshotFileName, ScreenshotImageFormat.Png);
                var bounds = Screen.GetBounds(Point.Empty);
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    bitmap.Save(screenshotFileName, ImageFormat.Png);
                }
            }
            catch
            {
                Trace.TraceWarning(
                    $"Error saving screenshot {screenshotFileName}. Check permissions.");
            }
        }

        /// <summary>
        /// Finalizes report file and fixes links to images
        /// </summary>
        public static void FinalizeReport()
        {
            _extentReport?.Flush();
            // Make screenshots relative to *.html
            try
            {
                var resultsFolder = new DirectoryInfo(_currentReportPath);
                var filesInfo = resultsFolder.GetFiles("index.html");

                if (filesInfo.Length == 0)
                {
                    return;
                }

                var fileInfo = filesInfo[0];
                var reportContent = File.ReadAllText(fileInfo.FullName);
                if (string.IsNullOrEmpty(reportContent))
                {
                    return;
                }

                reportContent = reportContent.Replace($"=\"{_currentReportPath}\\", "=\"");
                File.WriteAllText(fileInfo.FullName, reportContent);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(
                    $"Error image link conversion in report {_currentReportPath}: \n" +
                    ex.Message);
                _extentReport?.AddTestRunnerLogs(ex.Message);
                _extentReport?.Flush();
            }
        }

        /// <summary>
        /// Removes expired report folders recursively by creation date
        /// </summary>
        public static Task DeleteExpiredReportsAsync()
        {
            return Task.Run(() =>
            {
                var dateLimit = DateTime.Now - TimeSpan.FromDays(TestConfig.ReportsExpiredInDays);

                try
                {
                    var foldersList = Directory.GetDirectories(TestConfig.ReportParentFolder);
                    foreach (var folder in foldersList)
                    {
                        var folderCreationDate = Directory.GetCreationTime(folder);
                        // current folder datetime older than dateLimit
                        if (DateTime.Compare(folderCreationDate, dateLimit) < 0)
                        {
                            Directory.Delete(folder, true);
                        }
                    }
                }
                catch 
                {
                    Trace.TraceWarning(
                        $"Error removing expired reports from folder {TestConfig.ReportParentFolder}. " +
                        "Check permissions and for read-only instances.");
                    _extentReport?.AddTestRunnerLogs(
                        $"Error removing expired reports from folder {TestConfig.ReportParentFolder}. " +
                        "Check permissions and for read-only instances.");
                }
            });
        }
    }
}
