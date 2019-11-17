using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Common.Enums;
using Common.Resources;
using Models.Users;

namespace Common.Configuration
{
    /// <summary>
    /// Sets test configuration parameters and constants
    /// </summary>
    public static class TestConfig
    {
        public static string BaseUrl { get; private set; }
        public static string LoginUrl { get; private set; }
        public static string RegisterUrl { get; private set; }
        public static string PlacesUri { get; private set; }
        public static string AppsUri { get; private set; }
        public static string ItemsUri { get; private set; }
        public static string ItemsImportUri { get; private set; }
        public static string PlaceUri { get; private set; }
        public static string AppUri { get; private set; }
        public static string ItemUri { get; private set; }
        public static string TenantsUrl { get; private set; }
        public static User AdminUser { get; private set; }
        public static User AdminUser2 { get; private set; }
        public static User AdminUserDirectory { get; private set; }
        public static User NewUser { get; private set; }
        public static User DisabledUser { get; private set; }
        public static User NoTenantUser { get; private set; }
        public static User OneTenantUser { get; private set; }
        public static User MultiTenantUser { get; private set; }
        public static User ComposerOnlyUser { get; private set; }
        public static User SystemUser { get; private set; }
        public static User PermissionsUser { get; private set; }
        public static User SystemImport { get; private set; }
        public static User TenantsListUser { get; private set; }
        public static User DistributionUser { get; private set; }
        public static BrowserType PrimaryBrowser { get; private set; }
        public static BrowserType SecondaryBrowser { get; private set; }
        public static double ImplicitWaitTimeout { get; private set; }
        public static double PageLoadWaitTimeout { get; private set; }
        public static double CheckIfElementExistsTimeout { get; private set; }
        public static double CheckIfElementDoesNotExistTimeout { get; private set; }
        public static double CheckIfPageRedirectedTimeout { get; private set; }
        public static double FileDownloadTimeout { get; private set; }
        public static double TextValueWaitTimeout { get; private set; }
        public static bool IsReportingEnabled { get; private set; }
        public static string ReportParentFolder { get; private set; }
        public static bool IsDeleteExpiredReports { get; private set; }
        public static uint ReportsExpiredInDays { get; private set; }
        public static string MailServerDomain { get; private set; }
        public static int MailServerPort { get; private set; }
        public static string MailServerLogin { get; private set; }
        public static string MailServerPassword { get; private set; }
        public static string MailServerEmailFrom { get; private set; }
        public static bool MailServerUseSsl { get; private set; }
        public static string MailServerDomainAdmin { get; private set; }
        public static int MailServerPortAdmin { get; private set; }
        public static string MailServerLoginAdmin { get; private set; }
        public static string MailServerPasswordAdmin { get; private set; }
        public static string MailServerEmailFromAdmin { get; private set; }
        public static bool MailServerUseSslAdmin { get; private set; }
        public static string BaseUrlApi { get; private set; }
        public static string UserDirectoryBaseUrlApi { get; private set; }
        public static string BrowserDownloadFolder { get; private set; }
        public static string DptMobileAppFolder { get; private set; }
        public static string DptMobileAppFile { get; private set; }
        public static string IbeaconAppFolder { get; private set; }
        public static string IbeaconAppFile { get; private set; }
        public static string ComposerHqApp1Folder { get; private set; }
        public static string ComposerHqApp1File { get; private set; }
        public static string ComposerHqApp2Folder { get; private set; }
        public static string ComposerHqApp2File { get; private set; }
        public static string ComposerVipbAppFolder { get; private set; }
        public static string ComposerVipbAppFile { get; private set; }
        public static string ComposerVipbApp2File { get; private set; }
        public static string PlayerAppFolder { get; private set; }
        public static string PlayerAppFile { get; private set; }
        public static string EventKitAppFolder { get; private set; }
        public static string EventKitAppFile { get; private set; }
        public static string LegoBoostAppFolder { get; private set; }
        public static string LegoBoostAppFile { get; private set; }
        public static string IpadPlayerAppFolder { get; private set; }
        public static string IpadPlayerAppFile { get; private set; }
        public static string TestDeviceTypesPackage { get; private set; }
        public static string App1Package { get; private set; }
        public static string App1Assets { get; private set; }
        public static string App1Config { get; private set; }
        public static string UeCarViewerAppFolder { get; private set; }
        public static string UeCarViewerAppFile { get; private set; }
        public static string UeCarCayenneNewAppFolder { get; private set; }
        public static string UeCarCayenneNewAppFile { get; private set; }
        public static string UeCarScenePlainAppFolder { get; private set; }
        public static string UeCarScenePlainAppFile { get; private set; }
        public static string TestDependenciesDptAppFolder { get; private set; }
        public static string TestDependenciesDptAppFile { get; private set; }
        public static string TestElementPermissionDptFolder { get; private set; }
        public static string TestElementPermissionDptFile { get; private set; }
        public static string SapPorscheAppFolder { get; private set; }
        public static string SapPorscheAppFile { get; private set; }
        public static string ImageFhdFile { get; private set; }
        public static string Image4KFile { get; private set; }
        public static string ImageJpeg { get; private set; }
        public static string ImageJpg { get; private set; }
        public static string ImageCar { get; private set; }
        public static string ImportItemFolder { get; private set; }
        public static string Image138 { get; private set; }
        public static string Image25 { get; private set; }
        public static string Image025 { get; private set; }
        public static string Image285 { get; private set; }
        public static string Image04 { get; private set; }
        public static string ImageSvg { get; private set; }
        public static string ImageGif { get; private set; }
        public static string Image08 { get; private set; }
        public static string Image08Png { get; private set; }
        public static string Image059 { get; private set; }
        public static string Image075 { get; private set; }
        public static string Image075Copy { get; private set; }
        public static string ImagePoi { get; private set; }
        public static string ImageUnique { get; private set; }
        public static string Asset1 { get; private set; }
        public static string Asset2 { get; private set; }
        public static string Video1Mp4 { get; private set; }
        public static string Video2Mp4 { get; private set; }
        public static string Video3Mov { get; private set; }
        public static string Pdf1 { get; private set; }
        public static string Pdf2 { get; private set; }
        public static string Pdf1Distribute { get; private set; }
        public static string Pdf2Distribute { get; private set; }
        public static string Otf { get; private set; }
        public static string Ttf { get; private set; }
        public static string Mp3 { get; private set; }
        public static string Arcar { get; private set; }
        public static string Zip { get; private set; }
        public static string DptMobileAppFolderRt09010 { get; private set; }
        public static string DptMobileAppFileRt09010 { get; private set; }
        public static string IbeaconAppFolderRt09020 { get; private set; }
        public static string IbeaconAppFileRt09020 { get; private set; }
        public static string DptAppFolderRt11090 { get; private set; }
        public static string DptAppFileRt11090 { get; private set; }
        public static string HariboApp { get; private set; }
        public static double AppImportTimeout { get; private set; }
        public static double TenantImportTimeout { get; private set; }
        public static double TenantExportTimeout { get; private set; }
        public static int ApiRequestTimeout { get; private set; }
        public static double PvmsImportTimeout { get; private set; }
        public static string SftpServer { get; private set; }
        public static int SftpPort { get; private set; }
        public static string SftpUser { get; private set; }
        public static string SftpPassword { get; private set; }
        public static string SftpImportDirectory { get; private set; }

        public static readonly string PlayerAppDescription = "";
        public const string ComposerHqApp1Version = "1.0.0";
        public const string ComposerHqApp1Version2 = "2.0.0";
        public const string ComposerHqApp1Version3 = "3.0.0";
        public const string ComposerHqApp1Description = "";
        public const string ComposerHqApp2Version = "1.0.0";
        public static readonly string ComposerHqApp2Description = "";
        public const string App1Version1 = "1.0.0";
        public const string App1Version2 = "2.0.0";
        public static string DptAppEarliestVersionRt09010;
        public static string DptAppLatestVersionRt09010;
        public static string ComposerVipbAppEarliestVersion;
        public static string ComposerVipbAppMiddleVersion;
        public static string ComposerVipbAppLatestVersion;
        public static string ComposerVipb2AppVersion;
        public static readonly List<string> DptAppVersions = new List<string>();
        public static readonly List<string> IbeaconAppVersions = new List<string>();
        public static readonly List<string> PlayerAppVersions = new List<string>();
        public static readonly List<string> EventKitAppVersions = new List<string>();
        public static readonly List<string> LegoBoostAppVersions = new List<string>();
        public static readonly List<string> IbeaconAppVersionsRt09020 = new List<string>();
        public static readonly List<string> SftpImportFiles = new List<string>();
        public static readonly List<string> IpadPlayerAppVersions = new List<string>();
        public static readonly List<string> SapPorscheAppVersions = new List<string>();
        public static readonly List<string> DptAppVersionsRt11090 = new List<string>();
        public static readonly List<string> UeCarViewerVersions = new List<string>();
        public static readonly List<string> UeCarCayenneNewVersions = new List<string>();
        public static readonly List<string> UeCarScenePlainVersions = new List<string>();
        public static readonly List<string> TestDependenciesMdVersions = new List<string>();
        public static readonly List<string> ElementPermissionDptVersions = new List<string>();

        private static readonly DataGetter Dg = new DataGetter();

        static TestConfig()
        {
            GetConnections();
            GetLogins();
            GetBrowserSettings();
            GetReportSettings();
            GetTestFilesInfo();
            GetOtherSettings();
            GetVersion();
            Dg.Dispose();
        }

        /// <summary>
        /// Parses path info and returns both path and file name
        /// </summary>
        /// <param name="row">Path info</param>
        /// <returns>(<see cref="ValueTuple{String, String}"/>) Path and file name</returns>
        private static (string, string) TakePathFileInfo(string row)
        {
            try
            {
                string path, fileName;
                if (!row.Contains(":"))
                {
                    var combinedPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, row);
                    fileName = Path.GetFileName(combinedPath);
                    path = Path.GetFullPath(
                        Path.GetDirectoryName(combinedPath) ??
                        throw new AggregateException($@"Wrong path in TestConfig.xlsx for row: '{row}'"));
                }
                else
                {
                    fileName = Path.GetFileName(row);
                    path = Path.GetFullPath(row);
                }

                return (path, fileName);
            }
            catch
            {
                throw new AggregateException(
                    $@"Wrong file path in config TestConfig.xlsx, sheet Test Files, row: '{row}'");
            }
        }

        /// <summary>
        /// Gets app files versions that are present in app file names and sets corresponding public variables 
        /// </summary>
        /// <param name="appType">App type (see App Types resource)</param>
        /// <param name="path">Path to app file</param>
        /// <param name="fileName">App file name</param>
        private static void GetAppVersions(string appType, string path, string fileName)
        {
            var files = Directory.GetFiles(path, fileName);
            var name = Path.GetFileNameWithoutExtension(files.FirstOrDefault());

            if (appType == AppTitle.Dpt)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    DptAppVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.Player)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    PlayerAppVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.DptRt09010)
            {
                DptAppEarliestVersionRt09010 = name.Substring(name.LastIndexOf('-') + 1);
                name = Path.GetFileNameWithoutExtension(files.LastOrDefault());
                DptAppLatestVersionRt09010 = name.Substring(name.LastIndexOf('-') + 1);
                return;
            }

            if (appType == AppTitle.Ibeacon)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    IbeaconAppVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.ComposerVipB)
            {
                ComposerVipbAppEarliestVersion = name.Substring(name.LastIndexOf('_') + 1);
                name = Path.GetFileNameWithoutExtension(files.LastOrDefault());
                ComposerVipbAppLatestVersion = name.Substring(name.LastIndexOf('_') + 1);
                name = Path.GetFileNameWithoutExtension(files.ElementAtOrDefault(1));
                ComposerVipbAppMiddleVersion = name.Substring(name.LastIndexOf('_') + 1);
                return;
            }

            if (appType == AppTitle.ComposerVipB2)
            {
                ComposerVipb2AppVersion = name.Substring(name.LastIndexOf('_') + 1);
                return;
            }

            if (appType == AppTitle.IbeaconRt09020)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    IbeaconAppVersionsRt09020.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.DptRt11090)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    DptAppVersionsRt11090.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.IpadPlayer)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    IpadPlayerAppVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.SapPorsche)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    SapPorscheAppVersions.Add(name.Substring(name.LastIndexOf('_') + 1));
                }

                return;
            }

            if (appType == AppTitle.EventKit)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    EventKitAppVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.LegoBoostUnity)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    LegoBoostAppVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.UeCarViewer)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    UeCarViewerVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.UeCarCayenneNew)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    UeCarCayenneNewVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.UeCarScenePlain)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    UeCarScenePlainVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.TestDependenciesMd)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    TestDependenciesMdVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }

            if (appType == AppTitle.TestElementPermissionDpt)
            {
                foreach (var file in files)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    ElementPermissionDptVersions.Add(name.Substring(name.LastIndexOf('-') + 1));
                }

                return;
            }
        }

        /// <summary>
        /// Gets data from Logins sheet
        /// </summary>
        private static void GetLogins()
        {
            var row = Dg.GetRowData("Logins", "User", "AdminUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    "Wrong AdminUser email or password in config file TestConfig.xlsx, sheet Logins");
            AdminUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "AdminUser2", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    "Wrong AdminUser2 email or password in config file TestConfig.xlsx, sheet Logins");
            AdminUser2 = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "AdminUserDirectory", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    @"Wrong AdminUserDirectory email or password in config file TestConfig.xlsx, sheet Logins");
            AdminUserDirectory = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "NewUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new FormatException(
                    "Wrong NewUser email or password in config file TestConfig.xlsx, sheet Logins");
            NewUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "DisabledUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    "Wrong DisabledUser email or password in config file TestConfig.xlsx, sheet Logins");
            DisabledUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "NoTenantUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    "Wrong NoTenantUser email or password in config file TestConfig.xlsx, sheet Logins");
            NoTenantUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "OneTenantUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    "Wrong OneTenantUser email or password in config file TestConfig.xlsx, sheet Logins");
            OneTenantUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "MultiTenantUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    "Wrong MultiTenantUser email or password in config file TestConfig.xlsx, sheet Logins");
            MultiTenantUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "ComposerOnlyUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    "Wrong ComposerOnlyUser email or password in config file TestConfig.xlsx, sheet Logins");
            ComposerOnlyUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "SystemUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    "Wrong SystemUser email or password in config file TestConfig.xlsx, sheet Logins");
            SystemUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "SystemImport", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]))
                throw new DataException(
                    "Wrong SystemImport email in config file TestConfig.xlsx, sheet Logins");
            SystemImport = new User
            {
                Email = row["LoginValue"]
            };

            row = Dg.GetRowData("Logins", "User", "TenantsListUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]))
                throw new DataException(
                    "Wrong TenantsListUser email in config file TestConfig.xlsx, sheet Logins");
            TenantsListUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "DistributionUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]))
                throw new DataException(
                    "Wrong DistributionUser email in config file TestConfig.xlsx, sheet Logins");
            DistributionUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };

            row = Dg.GetRowData("Logins", "User", "PermissionsUser", "LoginValue", "PasswordValue");
            if (string.IsNullOrEmpty(row["LoginValue"]) || string.IsNullOrEmpty(row["PasswordValue"]))
                throw new DataException(
                    "Wrong PermissionsUser email or password in config file TestConfig.xlsx, sheet Logins");
            PermissionsUser = new User
            {
                Email = row["LoginValue"],
                Password = row["PasswordValue"]
            };
        }

        /// <summary>
        /// Gets data from Browser sheet
        /// </summary>
        private static void GetBrowserSettings()
        {
            var row = Dg.GetRowData(
                "Browser", "Primary Browser", string.Empty, "Element Implicit Wait", "Secondary Browser",
                "Element Exists On Page", "Element Does Not Exist On Page", "Page Redirection Timeout", 
                "Page Load Wait", "File Download Timeout", "Wait For Element Text");
            if (!double.TryParse(row["Element Implicit Wait"], out var implicitWaitTimeout))
                throw new FormatException(
                    @"Wrong web driver 'Element Implicit Wait' timeout in config TestConfig.xlsx, sheet Browser");
            ImplicitWaitTimeout = Math.Abs(implicitWaitTimeout);
           
            if (!double.TryParse(row["Page Load Wait"], out var pageLoadTimeout))
                throw new FormatException(
                    @"Wrong web driver 'Page Load Wait' timeout in config TestConfig.xlsx, sheet Browser");
            PageLoadWaitTimeout = Math.Abs(pageLoadTimeout);

            if (!double.TryParse(row["Element Exists On Page"], out var elementExistsTimeout))
                throw new FormatException(
                    @"Wrong web driver 'Element Exists On Page' timeout in config TestConfig.xlsx, sheet Browser");
            CheckIfElementExistsTimeout = Math.Abs(elementExistsTimeout);

            if (!double.TryParse(row["Element Does Not Exist On Page"], out var elementNotExistTimeout))
                throw new FormatException(
                    @"Wrong web driver 'Element Does Not Exist On Page' timeout in config TestConfig.xlsx, sheet Browser");
            CheckIfElementDoesNotExistTimeout = Math.Abs(elementNotExistTimeout);

            if (!double.TryParse(row["Page Redirection Timeout"], out var redirectionTimeout))
                throw new FormatException(
                    @"Wrong web driver 'Page Redirection Timeout' timeout in config TestConfig.xlsx, sheet Browser");
            CheckIfPageRedirectedTimeout = Math.Abs(redirectionTimeout);

            if (!double.TryParse(row["File Download Timeout"], out var waitTimeDownload))
                throw new FormatException(
                    @"Wrong web driver 'File Download Timeout' timeout in config TestConfig.xlsx, sheet Browser");
            FileDownloadTimeout = Math.Abs(waitTimeDownload);

            if (!double.TryParse(row["Wait For Element Text"], out var waitTimeText))
                throw new FormatException(
                    @"Wrong web driver 'Wait For Element Text' timeout in config TestConfig.xlsx, sheet Browser");
            TextValueWaitTimeout = Math.Abs(waitTimeText);

            if (!Enum.TryParse(row["Primary Browser"], out BrowserType browserType))
                throw new FormatException(
                    @"Wrong 'Primary Browser' in config TestConfig.xlsx, sheet Browser");
            PrimaryBrowser = browserType;

            if (!Enum.TryParse(row["Secondary Browser"], out BrowserType browserType2))
                throw new FormatException(
                    @"Wrong 'Secondary Browser' in config TestConfig.xlsx, sheet Browser");
            SecondaryBrowser = browserType2;
        }

        /// <summary>
        /// Gets data from Connections sheet
        /// </summary>
        private static void GetConnections()
        {
            var row = Dg.GetRowData("Connections", "Connection", "CxManagerUrl", "Domain");
            if (string.IsNullOrEmpty(row["Domain"]) ||
                !Uri.TryCreate(row["Domain"], UriKind.Absolute, out _))
                throw new UriFormatException("Wrong site's URL value in config TestConfig.xlsx, sheet Connections");
            BaseUrl = row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "LoginPageUri", "Domain");
            LoginUrl = BaseUrl + row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "RegisterPageUri", "Domain");
            RegisterUrl = BaseUrl + row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "PlacesPageUri", "Domain");
            PlacesUri = row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "AppsPageUri", "Domain");
            AppsUri = row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "ItemsPageUri", "Domain");
            ItemsUri = row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "ItemsImportPageUri", "Domain");
            ItemsImportUri = row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "PlacePageUri", "Domain");
            PlaceUri = row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "AppPageUri", "Domain");
            AppUri = row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "ItemPageUri", "Domain");
            ItemUri = row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "TenantsPageUri", "Domain");
            TenantsUrl = BaseUrl + row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "Mail Box", "Domain", "Port", "Login",
                "Password", "Use SSL");
            MailServerDomain = row["Domain"];

            var port1 = row["Port"];
            if (!short.TryParse(port1, out var port) && !string.IsNullOrEmpty(port1))
                throw new FormatException("Wrong mail server port in config TestConfig.xlsx, sheet Connections");
            MailServerPort = string.IsNullOrEmpty(port1) ? 143 : port;
            MailServerLogin = row["Login"];
            MailServerPassword = row["Password"];
            //MailServerEmailFrom = _row["Emails Come From"];
            MailServerUseSsl = row["Use SSL"].ToLower() == "yes";

            row = Dg.GetRowData("Connections", "Connection", "Mail Box Admin", "Domain", "Port", "Login",
                "Password", "Use SSL" /*, "Emails Come From"*/);
            MailServerDomainAdmin = row["Domain"];

            port1 = row["Port"];
            if (!short.TryParse(port1, out port) && !string.IsNullOrEmpty(port1))
                throw new FormatException("Wrong mail server port  in config TestConfig.xlsx, sheet Connections");
            MailServerPortAdmin = string.IsNullOrEmpty(port1) ? 143 : port;
            MailServerLoginAdmin = row["Login"];
            MailServerPasswordAdmin = row["Password"];
            //MailServerEmailFromAdmin = _row["Emails Come From"];
            MailServerUseSslAdmin = row["Use SSL"].ToLower() == "yes";

            row = Dg.GetRowData("Connections", "Connection", "SFTP Import Server", "Domain", "Port", "Login",
                "Password", "Directory");
            SftpServer = row["Domain"];

            port1 = row["Port"];
            if (!short.TryParse(port1, out port) && !string.IsNullOrEmpty(port1))
                throw new FormatException("Wrong SFTP server port in config TestConfig.xlsx, sheet Connections");
            SftpPort = string.IsNullOrEmpty(port1) ? 22 : port;
            SftpUser = row["Login"];
            SftpPassword = row["Password"];
            SftpImportDirectory = row["Directory"];

            row = Dg.GetRowData("Connections", "Connection", "CxManagerApiUrl", "Domain");
            if (string.IsNullOrEmpty(row["Domain"]) ||
                !Uri.TryCreate(row["Domain"], UriKind.Absolute, out _))
                throw new UriFormatException(
                    "Wrong site's API URL value in config TestConfig.xlsx, sheet Connections");
            BaseUrlApi = row["Domain"];

            row = Dg.GetRowData("Connections", "Connection", "UserDirectoryApiUrl", "Domain");
            if (string.IsNullOrEmpty(row["Domain"]) ||
                !Uri.TryCreate(row["Domain"], UriKind.Absolute, out _))
                throw new UriFormatException(
                    "Wrong User Directory URL value in config TestConfig.xlsx, sheet Connections");
            UserDirectoryBaseUrlApi = row["Domain"];
        }

        /// <summary>
        /// Gets data from Reporting sheet
        /// </summary>
        private static void GetReportSettings()
        {
            var row = Dg.GetRowData("Reporting", "Parameter", "Reporting Enabled", "Value");
            IsReportingEnabled = row["Value"].ToLower() == "yes";

            row = Dg.GetRowData("Reporting", "Parameter", "Report Parent Folder", "Value");
            if (string.IsNullOrEmpty(row["Value"].Trim('\\')))
            {
                ReportParentFolder = Path.GetFullPath(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestResults"));
            }
            else
            {
                try
                {
                    ReportParentFolder = Path.GetFullPath(row["Value"].Trim('\\'));
                }
                catch
                {
                    throw new
                        DirectoryNotFoundException(@"Wrong value syntax of report parent folder in config 
                                            TestConfig.xlsx, sheet Reporting");
                }
            }

            if (!Directory.Exists(ReportParentFolder))
            {
                try
                {
                    Directory.CreateDirectory(ReportParentFolder);
                }
                catch (Exception ex)
                {
                    throw new IOException(
                        $@"{ex.Message} Cannot create new directory '{ReportParentFolder}'");
                }
            }

            row = Dg.GetRowData("Reporting", "Parameter", "Delete Expired Reports", "Value");
            IsDeleteExpiredReports = row["Value"].ToLower() == "yes";

            row = Dg.GetRowData("Reporting", "Parameter", "Expiration period", "Value");
            if (!uint.TryParse(row["Value"], out var expiredDays))
                throw new FormatException(
                    "Wrong report expiration period in config TestConfig.xlsx, sheet Reporting");
            ReportsExpiredInDays = expiredDays;
        }

        /// <summary>
        /// Gets data from 'Test files' sheet
        /// </summary>
        private static void GetTestFilesInfo()
        {
            var pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile,
                Environment.SpecialFolderOption.Create);
            BrowserDownloadFolder = Path.Combine(pathUser, "Downloads");
            Directory.CreateDirectory(BrowserDownloadFolder);

            var row = Dg.GetRowData("Test Files", "Description", "DPT Mobile App", "File Name");
            var result = TakePathFileInfo(row["File Name"]);
            DptMobileAppFolder = result.Item1;
            DptMobileAppFile = result.Item2;
            GetAppVersions(AppTitle.Dpt, DptMobileAppFolder, DptMobileAppFile);

            row = Dg.GetRowData("Test Files", "Description", "iBeacon App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            IbeaconAppFolder = result.Item1;
            IbeaconAppFile = result.Item2;
            GetAppVersions(AppTitle.Ibeacon, IbeaconAppFolder, IbeaconAppFile);

            row = Dg.GetRowData("Test Files", "Description", "DPT RT09010", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            DptMobileAppFolderRt09010 = result.Item1;
            DptMobileAppFileRt09010 = result.Item2;
            GetAppVersions(AppTitle.DptRt09010, DptMobileAppFolderRt09010, DptMobileAppFileRt09010);

            row = Dg.GetRowData("Test Files", "Description", "iBeacon RT09020", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            IbeaconAppFolderRt09020 = result.Item1;
            IbeaconAppFileRt09020 = result.Item2;
            GetAppVersions(AppTitle.IbeaconRt09020, IbeaconAppFolderRt09020, IbeaconAppFileRt09020);

            row = Dg.GetRowData("Test Files", "Description", "Composer HQ App1", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ComposerHqApp1Folder = result.Item1;
            ComposerHqApp1File = result.Item2;

            row = Dg.GetRowData("Test Files", "Description", "Composer HQ App2", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ComposerHqApp2Folder = result.Item1;
            ComposerHqApp2File = result.Item2;

            row = Dg.GetRowData("Test Files", "Description", "Composer VIPB App2", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ComposerVipbApp2File = Path.Combine(result.Item1, result.Item2);
            GetAppVersions(AppTitle.ComposerVipB2, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Composer VIPB App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ComposerVipbAppFolder = result.Item1;
            ComposerVipbAppFile = result.Item2;
            GetAppVersions(AppTitle.ComposerVipB, ComposerVipbAppFolder, ComposerVipbAppFile);

            row = Dg.GetRowData("Test Files", "Description", "Player App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            PlayerAppFolder = result.Item1;
            PlayerAppFile = result.Item2;
            GetAppVersions(AppTitle.Player, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Event Kit App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            EventKitAppFolder = result.Item1;
            EventKitAppFile = result.Item2;
            GetAppVersions(AppTitle.EventKit, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Lego Boost Unity App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            LegoBoostAppFolder = result.Item1;
            LegoBoostAppFile = result.Item2;
            GetAppVersions(AppTitle.LegoBoostUnity, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "iPad Player App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            IpadPlayerAppFolder = result.Item1;
            IpadPlayerAppFile = result.Item2;
            GetAppVersions(AppTitle.IpadPlayer, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "SAP Porsche App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            SapPorscheAppFolder = result.Item1;
            SapPorscheAppFile = result.Item2;
            GetAppVersions(AppTitle.SapPorsche, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "DPT RT11090", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            DptAppFolderRt11090 = result.Item1;
            DptAppFileRt11090 = result.Item2;
            GetAppVersions(AppTitle.DptRt11090, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Test Device Types Package", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            TestDeviceTypesPackage = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "App1 Package", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            App1Package = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "App1 Assets", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            App1Assets = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "App1 Config", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            App1Config = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "UE Car Viewer App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            UeCarViewerAppFolder = result.Item1;
            UeCarViewerAppFile = result.Item2;
            GetAppVersions(AppTitle.UeCarViewer, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "UE Car Cayenne New App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            UeCarCayenneNewAppFolder = result.Item1;
            UeCarCayenneNewAppFile = result.Item2;
            GetAppVersions(AppTitle.UeCarCayenneNew, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "UE Car Scene Plain App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            UeCarScenePlainAppFolder = result.Item1;
            UeCarScenePlainAppFile = result.Item2;
            GetAppVersions(AppTitle.UeCarScenePlain, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Test Dependencies MD", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            TestDependenciesDptAppFolder = result.Item1;
            TestDependenciesDptAppFile = result.Item2;
            GetAppVersions(AppTitle.TestDependenciesMd, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Test Element Permission App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            TestElementPermissionDptFolder = result.Item1;
            TestElementPermissionDptFile = result.Item2;
            GetAppVersions(AppTitle.TestElementPermissionDpt, result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Video1 MP4", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Video1Mp4 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Video2 MP4", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Video2Mp4 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Video3 MOV", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Video3Mov = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "PDF1", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Pdf1 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "PDF2", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Pdf2 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "OTF", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Otf = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "TTF", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Ttf = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "MP3", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Mp3 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "ArCar", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Arcar = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "ZIP", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Zip = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "PDF1 Distribute", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Pdf1Distribute = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "PDF2 Distribute", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Pdf2Distribute = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image POI", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ImagePoi = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image Unique", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ImageUnique = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 4K", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image4KFile = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image FHD", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ImageFhdFile = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "ImageJpeg", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ImageJpeg = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "ImageJpg", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ImageJpg = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "ImageCar", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ImageCar = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Import Item Folder", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ImportItemFolder = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 1.38", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image138 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 2.5", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image25 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 0.25", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image025 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 2.85", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image285 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 0.4", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image04 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image SVG", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ImageSvg = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image GIF", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            ImageGif = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 0.8", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image08 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 0.8 PNG", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image08Png = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 0.59", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image059 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 0.75", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image075 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Image 0.75 Copy", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Image075Copy = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Asset1", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Asset1 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Asset2", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            Asset2 = Path.Combine(result.Item1, result.Item2);

            row = Dg.GetRowData("Test Files", "Description", "Haribo App", "File Name");
            result = TakePathFileInfo(row["File Name"]);
            HariboApp = Path.Combine(result.Item1, result.Item2);

            for (var i = 1; i <= 15; i++)
            {
                row = Dg.GetRowData("Test Files", "Description", 
                    $"Car Import{i.ToString().PadLeft(2, '0')}", "File Name");
                result = TakePathFileInfo(row["File Name"]);
                SftpImportFiles.Add(Path.Combine(result.Item1, result.Item2));
            }
        }

        /// <summary>
        /// Gets data from 'Other Settings' sheet
        /// </summary>
        private static void GetOtherSettings()
        {
            var row = Dg.GetRowData("Other Settings", "Parameter", "App Import Timeout", "Value");
            if (!double.TryParse(row["Value"], out var value))
                throw new FormatException(
                    "Wrong App Import Timeout in config TestConfig.xlsx, sheet Other Settings");
            AppImportTimeout = Math.Abs(value);

            row = Dg.GetRowData("Other Settings", "Parameter", "REST API Request Timeout", "Value");
            if (!int.TryParse(row["Value"], out var value1))
                throw new FormatException(
                    "Wrong REST API Request Timeout in config TestConfig.xlsx, sheet Other Settings. " +
                    "It should be positive integer.");
            ApiRequestTimeout = Math.Abs(value1 * 1000);

            row = Dg.GetRowData("Other Settings", "Parameter", "Tenant Export Timeout", "Value");
            if (!double.TryParse(row["Value"], out value))
                throw new FormatException(
                    "Wrong Tenant Export Timeout in config TestConfig.xlsx, sheet Other Settings");
            TenantExportTimeout = Math.Abs(value);

            row = Dg.GetRowData("Other Settings", "Parameter", "Tenant Import Timeout", "Value");
            if (!double.TryParse(row["Value"], out value))
                throw new FormatException(
                    "Wrong Tenant Import Timeout in config TestConfig.xlsx, sheet Other Settings");
            TenantImportTimeout = Math.Abs(value);

            row = Dg.GetRowData("Other Settings", "Parameter", "PVMS Import Timeout", "Value");
            if (!double.TryParse(row["Value"], out value))
                throw new FormatException(
                    "Wrong app PVMS Import Timeout in config TestConfig.xlsx, sheet Other Settings");
            PvmsImportTimeout = Math.Abs(value);

        }

        /// <summary>
        /// Gets data from Customer sheet
        /// </summary>
        private static void GetVersion()
        {
            // Version sheet
            //var row = _dg.GetRowData("Customer", "CustomerName", string.Empty, "CustomerName", "Country", "Version");
            //Customer = row["CustomerName"];
            //Country = row["Country"];
            //Version = row["Version"];

            //var connectionRows = _dg.GetSheetData("Connections").ToList();
            //foreach (var uri in connectionRows)
            //    Uris.Add(uri["Connection"], uri["Domain"]);
        }
    }
}

