using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using AutoIt;
using Common.Browsers;
using Common.Configuration;
using Common.Enums;
using Common.Resources;

namespace Common.Managers
{
    /// <summary>
    /// File manager class supports browser's dialog window. 
    /// Does not require AutoIt application installed locally.
    /// </summary>
    public static class FileManager
    {
        private const string FileNameControlName = "Edit1";
        private const string OkControlName = "Button1";
        private static IntPtr _windowHandle, _okControlHandle, _fileNameControlHandle;
        public static string WindowTitle
        {
            get
            {
                if (Equals(Browser.CurrentDriver.CurrentWindowHandle, Browser.Driver.CurrentWindowHandle))
                {
                    return TestConfig.PrimaryBrowser != BrowserType.Firefox ? "Open" : "File Upload";
                }
                return TestConfig.SecondaryBrowser != BrowserType.Firefox ? "Open" : "File Upload";
            }
        }

        /// <summary>
        /// Is window with specified title opened. Awaits for the window during timeout.
        /// </summary>
        /// <param name="windowTitle">Window title</param>
        /// <param name="timeout">Timeout in seconds</param>
        /// <returns>(<see cref="bool"/>) True if window opened</returns>
        public static bool IsWindowOpen(string windowTitle, int timeout = 1)
        {
            return AutoItX.WinWaitActive(windowTitle, string.Empty, timeout) != 0;
        }

        /// <summary>
        /// This method supports single and multiple (from the same folder) file(s) upload. 
        /// It sends text to a focused "File name" control of browser open file dialog
        /// window and presses {Enter} key. Finally, it checks whether file open dialog is
        /// closed, and upload spinner animated icon appears in page header (background
        /// task has started).
        /// </summary>
        /// <param name="paths">Paths to files</param>
        public static void UploadAsBackgroundTask(params string[] paths)
        {
            Upload(paths);
            var temp = ActionManager.IsUseAllPageReadyChecks;
            ActionManager.IsUseAllPageReadyChecks = false;
            ActionManager.IsElementFound(CommonElement.UploadSpinner);
            ActionManager.IsUseAllPageReadyChecks = temp;
        }

        /// <summary>
        /// Sends text to file name control of browser open file GUI dialog window and
        /// presses {Enter} key. Supports single and multiple (from the same folder)
        /// file(s) upload. Checks whether the dialog was closed afterwards.
        /// </summary>
        /// <param name="paths">File(s) with full path</param>
        public static void Upload(params string[] paths)
        {
            if (paths == null || paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
            {
                throw new ArgumentException(
                    @"Got null or empty parameter(s). Nothing to upload.", nameof(paths));
            }

            // is it multiple files upload?
            var isMultipleFilesUpload = paths.Length > 1;

            // waits for dialog window appearance
            if (AutoItX.WinWait(WindowTitle, string.Empty, 5) == 0)
            {
                throw new AggregateException($@"AutoItX: '{WindowTitle}' dialog window not found");
            }

            // get handles of dialog window, OK button, and File Name input controls
            _windowHandle = AutoItX.WinGetHandle(WindowTitle);
            _okControlHandle = AutoItX.ControlGetHandle(_windowHandle, OkControlName);
            _fileNameControlHandle = AutoItX.ControlGetHandle(_windowHandle, FileNameControlName);

            if ((int)_windowHandle <= 0 || (int)_okControlHandle <= 0)
            {
                throw new AggregateException("AutoItX: Cannot get window or/and control handle(s)");
            }
            
            // activate dialog window (just in case)
            AutoItX.WinActivate(_windowHandle);

            // set uploaded files folder as current folder in case of multiple files selection            
            var dir = Path.GetDirectoryName(paths[0])?.Replace(@"\\", @"\");
            if (isMultipleFilesUpload && !string.IsNullOrEmpty(dir))
            {
                // change path
                // mode: 1 -> no control symbols processing, send text "as is"
                AutoItX.ControlSend(
                    _windowHandle,
                    AutoItX.ControlGetHandle(_windowHandle, FileNameControlName),
                    dir,
                    mode: 1);                  
                // press OK button
                AutoItX.ControlClick(_windowHandle, _okControlHandle);
                Thread.Sleep(1000);
            }

            // create input string for file name control
            var path1 = string.Empty;
            foreach (var path in paths)
            {
                path1 += (isMultipleFilesUpload ? "\"" + Path.GetFileName(path) + "\" " : path)
                        .Replace(@"\\", @"\");
                
                // check if dialog is still active and activate it if needed
                if (AutoItX.WinActive(_windowHandle) != 0)
                {
                    continue;
                }
                
                if (AutoItX.WinActivate(_windowHandle) == 0)
                {
                    throw new AggregateException(
                        $@"AutoItX: Cannot activate '{WindowTitle}' dialog window. " +
                        "Is it closed, overlapped or desktop is locked?");
                }
            }

            // due to upper/lower case AutoIt input typos, following workaround is implemented:
            // input path to File Name control and verify it afterwards; if wrong, input it
            // over and over until succeed or timed out
            var sw = new Stopwatch();
            sw.Start();
            while (AutoItX.ControlGetText(_windowHandle, _fileNameControlHandle) != path1
                   && sw.Elapsed < TimeSpan.FromSeconds(60))
            {
                AutoItX.ControlSetText(_windowHandle, _fileNameControlHandle, string.Empty);
                AutoItX.ControlSend(_windowHandle, _fileNameControlHandle, path1, mode: 1);
            }
            sw.Stop();
            // if input is timed out, throw exception
            if (sw.Elapsed >= TimeSpan.FromSeconds(60))
            {
                throw new AggregateException($@"AutoItX: Failed to input path '{path1}'");
            }

            // check if dialog is still active and activate it if not
            if (AutoItX.WinActive(_windowHandle) == 0)
            {
                if (AutoItX.WinActivate(_windowHandle) == 0)
                {
                    throw new AggregateException(
                        $@"AutoItX: Cannot activate '{WindowTitle}' GUI dialog window. " +
                        "Is it closed, overlapped or desktop is locked?");
                }
            }

            // press OK button
            AutoItX.ControlClick(_windowHandle, _okControlHandle);

            // check if the dialog is closed
            var success = AutoItX.WinWaitClose(_windowHandle, 1);
            if (success > 0)
            {
                return;
            }

            // throw exception if dialog is not closed
            throw new AggregateException(
                $@"AutoItX: Failed to close '{WindowTitle}' GUI dialog window. Was this " +
                "window closed manually, desktop locked, or 'File not found' error " + 
                "displayed?"); 
        }

        /// <summary>
        /// Downloads file by <paramref name="url"/> parameter to folder <see
        /// cref="TestConfig.BrowserDownloadFolder"/> and gives it a name set by <paramref
        /// name="fileName"/> parameter.
        /// </summary>
        /// <param name="url">URL to file</param>
        /// <param name="fileName">Target file's name that will be saved in <see cref=
        /// "TestConfig.BrowserDownloadFolder"/> folder</param>
        public static void Download(string url, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            try
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFileAsync(new Uri(url), 
                        Path.Combine(TestConfig.BrowserDownloadFolder, Path.GetFileName(fileName)));
                }
            }
            catch (Exception e)
            {
                throw 
                    new WebException($"Error download file from {url} or save file. {e.Message}");
            }
        }

        /// <summary>
        /// Closes open file GUI window
        /// </summary>
        public static void CloseGuiWindow()
        {
            if (AutoItX.WinExists(WindowTitle) > 0)
            {
                AutoItX.WinClose(WindowTitle);
            }
        }

        /// <summary>
        /// Returns path with file name of the earliest file version by specified path and mask
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="mask">File name with extension</param>
        /// <returns>(<see cref="string"/>) Full path with file name and extension</returns>
        public static string GetEarliestFileVersion(string path, string mask)
        {
            var fileList = Directory.GetFiles(path, mask);
            if (fileList.Length == 0)
            {
                throw new FileNotFoundException($@"File '{Path.Combine(path, mask)}' not found. " +
                    "Please check path and file mask.");
            }
            return fileList.Min();
        }

        /// <summary>
        /// Returns path with file name by specified path, file name mask, and version
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="mask">File name with extension</param>
        /// <param name="version">Version number</param>
        /// <returns>(<see cref="string"/>) Full path with file name and extension</returns>
        public static string GetFileByVersion(string path, string mask, string version)
        {
            var fileList = Directory.GetFiles(path, mask);
            if (fileList.Length == 0)
            {
                throw new FileNotFoundException($"File {Path.Combine(path, mask)} not found. " +
                    "Please check path and file mask.");
            }
            return !string.IsNullOrEmpty(version) ? fileList.SingleOrDefault(x => x.Contains(version)) : fileList[0];
        }

        /// <summary>
        /// Deletes file on disk. Supports wildcard characters.
        /// </summary>
        /// <param name="pathFile">Path and file name/mask</param>
        public static void Delete(string pathFile)
        {
            if (string.IsNullOrEmpty(pathFile))
            {
                throw new ArgumentNullException(nameof(pathFile));
            }
            try
            {
                var dir = new DirectoryInfo(
                    Path.GetDirectoryName(pathFile) 
                        ?? throw new DirectoryNotFoundException($"Path {pathFile} not found"));

                foreach (var file in dir.EnumerateFiles(Path.GetFileName(pathFile)))
                {
                    file.Delete();
                }
            }
            catch (Exception e)
            {
                throw new IOException($"Error deleting files by path {pathFile}. {e.Message}");
            }
        }

        /// <summary>
        /// Checks during <see cref="TestConfig.FileDownloadTimeout"/> whether file with specified
        /// name exists in browser download folder (see <see cref="TestConfig.BrowserDownloadFolder"/>) 
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>(<see cref="bool"/>) True if file exists in browser download folder
        /// (see <see cref="TestConfig.BrowserDownloadFolder"/>)</returns>
        public static bool IsFileExist(string fileName)
        {
            var isFileExist = false;
            var sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed < TimeSpan.FromSeconds(TestConfig.FileDownloadTimeout) && !isFileExist)
            {
                isFileExist = File.Exists(Path.Combine(TestConfig.BrowserDownloadFolder, fileName));
            }
            sw.Stop();
            return isFileExist;
        }

        /// <summary>
        /// Returns full path to file which has been recently downloaded to browser download folder
        /// (see <see cref="TestConfig.BrowserDownloadFolder"/>) by its <paramref name="mask"/>
        /// during <see cref="TestConfig.FileDownloadTimeout"/> timeout
        /// </summary>
        /// <param name="mask">File name mask</param>
        /// <returns>(<see cref="string"/>) Full path with file name or null if timed out</returns>
        public static string GetRecentDownloadedFileName(string mask)
        {
            var lowLimit = DateTime.UtcNow - TimeSpan.FromSeconds(1);
            var sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed < TimeSpan.FromSeconds(TestConfig.FileDownloadTimeout))
            {
                var files = Directory.GetFiles(TestConfig.BrowserDownloadFolder, mask);
                if (files.Length <= 0)
                {
                    continue;
                }
                foreach (var file in files)
                {
                    if (new FileInfo(file).CreationTimeUtc < lowLimit)
                    {
                        continue;
                    }
                    sw.Stop();
                    return file;
                }
            }
            sw.Stop();
            return null;
        }
    }
}
