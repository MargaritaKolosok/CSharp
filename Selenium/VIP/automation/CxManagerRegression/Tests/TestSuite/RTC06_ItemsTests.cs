using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using Models.Apps;
using Models.Items;
using Models.Places;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC06_ItemsTests : ParentTest
    {
        private readonly MailManager _mm = new MailManager();
        private readonly FtpManager _ftpM = new FtpManager();
        private const int LongTestTimeoutMilliseconds = 600000;
        private const int WaitTimeOfNonExistingEmailSeconds = 60;
        private const string VinImport1 = "WP000000000000001";
        private const string TitleImport = "Cayenne Diesel";
        private const string BtypImport = "92AAY1";
        private const string CodeImport = "3S1";
        private const string TextImport = "Roof rails in aluminum finish";
        private const string PriceImport = "940";
        private const string ExclusiveManufakturImport = "";
        private const string LastEntriesImport = "Privacy glass";
        private const string CategoryImport = "serviceAndWarranty";
        private const string CapacityImport = "2 967cm³";
        private const string DisclaimerImport = "";
        private const string MonthlyRateImport = "";
        private const string TotalCarPriceImport = "93,020";
        private const string VinImport2 = "WP000000000000002";
        private const string ExteriorColourHeadlineImport2 = "Exterior Colour";
        private const string TextImport2 = "GT Silver Metallic";
        private const string ExteriorColourHeadlineDeImport2 = "Außenfarben";
        private const string TextDeImport2 = "racinggelb";
        private const string BtypImport2 = "982321";
        private const string VinImport3 = "WP000000000000003";
        private const string VinImport4 = "WP000000000000004";
        private const string VinImport5 = "WP000000000000005";
        private const string VinImport6 = "WP000000000000006";
        private const string VinImport7 = "WP000000000000007";
        private const string VinImport8 = "WP000000000000008";
        private const string VinImport9 = "WP000000000000009";
        private const string VinImport10 = "WP000000000000010";
        private const string VinImport11 = "WP000000000000011";

        [OneTimeSetUp]
        public async Task BeginFixtureTests()
        {
            IsEachTestInNewBrowser = false;
            await _mm.ConnectMailServerAsync(UserType.AdminCxM).ConfigureAwait(false);
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }

            CurrentTenant = TenantTitle.porsche9699001;
            CurrentUser = TestConfig.AdminUser;
        }

        /// <summary>
        /// Checks Items list for text during predefined time period
        /// </summary>
        /// <param name="itemText">Item name and/or VIN</param>
        /// <returns>(bool) True if found</returns>
        private bool IsItemCreated(string itemText)
        {
            bool isItemCreated;
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                RefreshPage();
                isItemCreated = IsElementFound(string.Format(ItemsPage.TableRowByText, itemText), 8);
            } 
            while (!isItemCreated && sw.Elapsed < TimeSpan.FromSeconds(TestConfig.PvmsImportTimeout));

            return isItemCreated;
        }

        [Test, Regression]
        public void RT06010_ItemsPageAndItemTypes()
        {
            //if (TestContext.Parameters.Count == 0)
            //{
            //    Assert.Warn($"Run on clean tenant {CurrentTenantCode} environment only");
            //    return;
            //}
            TestStart();

            Assert.IsTrue(IsElementFound(PageHeader.PagePlacesButton), "Page header should contain Places tab");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PageAppsButton), "Page header should contain Apps tab");
            Assert.IsTrue(IsElementFoundQuickly(PageHeader.PageCarsButton), "Page header should contain Cars tab");

            OpenCarsPage();
            
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TableEmpty), "There should be no items for the tenant");

            Click(PageFooter.AddItemButton);
            Assert.IsTrue(IsElementFound(ItemsPage.Title), "Title field should be shown on the new item page");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Vin), "VIN field should be shown on the new item page");

            AddAppDpt(AppStatus.Available, TestConfig.DptAppVersions[0]);
            RefreshPage();
            Assert.IsTrue(IsElementNotFound(PageHeader.PageCarsButton), "Cars tab in page header should disappear");
            Assert.IsTrue(IsElementFound(PageHeader.PageItemsButton), 
                "Items tab in page header should substitute Cars tab");

            Assert.IsTrue(IsElementFound(ItemsPage.Title), "Title field should be shown on the new item page");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Vin), "VIN field should be shown on the new item page");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TypeDropDown), 
                "Type drop-down should be shown on the new item page");

            ClickUntilShown(ItemsPage.TypeDropDown, CommonElement.DropDownOptionList);
            var originalCarTypes = new List<string>
            {
                ItemTypeCars,
                ItemTypePorscheCar, 
                ItemTypeUsedCar,
                ItemTypePdfCar
            };
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), originalCarTypes),
                "Type drop-down should contain: " + string.Join(", ", originalCarTypes));

            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeCars);
            var itemTitle = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Title, itemTitle);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Cars item should be saved successfully");

            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            RefreshPage();
            OpenItemsPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByTitle, ItemTypeDailyItemsReport)),
                $@"Item '{ItemTypeDailyItemsReport}' should be present in items table");
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByTitle, ItemTypeWelcomeEmailTemplate)),
                $@"Item '{ItemTypeWelcomeEmailTemplate}' should be present in items table");
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByTitle, ItemTypeServiceBooking)),
                $@"Item '{ItemTypeServiceBooking}' should be present in items table");
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByTitle, ItemTypeActionButtonsTemplate)),
                $@"Item '{ItemTypeActionButtonsTemplate}' should be present in items table");

            Click(PageFooter.AddItemButton);
            originalCarTypes = new List<string>
            {
                ItemTypeCars,
                ItemTypePorscheCar,
                ItemTypeUsedCar,
                ItemTypePdfCar, 
                ItemTypeCustomerProfile,
                ItemTypeEmailTemplate,
                ItemTypeEmployee,
                ItemTypePoi,
                ItemTypeEventOrPromotion,
                ItemTypeServiceBooking,
                ItemTypeSalesAppointment,
                ItemTypeTestDrive
            };
            ClickUntilShown(ItemsPage.TypeDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(AreCollectionsEqual(GetElementsText(CommonElement.DropDownOptionList), originalCarTypes),
                "Type drop-down should contain: " + 
                string.Join(", ", originalCarTypes) +
                ". But contains: " + 
                string.Join(", ", GetElementsText(CommonElement.DropDownOptionList)));

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemsUri}?type="),
                "User is not redirected to Items page after item edit cancellation");
        }

        [Test, Regression]
        public void RT06020_EmailTemplate()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            ClickUntilShown(string.Format(ItemsPage.TableRowByTitle, ItemTypeWelcomeEmailTemplate), ItemsPage.Status);
            Assert.IsTrue(IsElementFound(PageFooter.DeleteButton), "Delete button should be available in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.DuplicateButton), "Duplicate button should be available in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.EditButton), "Edit button should be available in page footer");

            var checkBoxState = IsCheckBoxOn(ItemsPage.UseHtmlCheckBox);
            Click(ItemsPage.UseHtmlCheckBox);
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.UseHtmlCheckBox) == checkBoxState, 
                "Check box Use HTML should be read-only and should not change state on click");

            Click(ItemsPage.AssetsSectionImage);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionImagePreview), 
                "Assets section: click on image should open the image preview");

            var assetImageSource = GetElementAttribute(ItemsPage.AssetsSectionImage, "src");
            var itemImageSource = GetElementAttribute(ItemsPage.Image, "src");
            Assert.IsTrue(assetImageSource == itemImageSource, 
                "Item image should be the same as first image in Assets section");

            var bodySize = GetElementSize(ItemsPage.BodyReadOnly);
            Click(ItemsPage.BodyReadOnly);
            var bodySizeIncreased = GetElementSize(ItemsPage.BodyReadOnly);
            Assert.IsTrue(bodySize.Height < bodySizeIncreased.Height, "Body field should be expanded on click");

            Click2(ItemsPage.BodyReadOnly);
            var bodySizeDecreased = GetElementSize(ItemsPage.BodyReadOnly);
            Assert.IsTrue(bodySizeDecreased.Height == bodySize.Height, 
                "Body field should be collapsed back on second click");

            EditForm();
            Assert.IsTrue(GetElementSize(ItemsPage.Body).Height > bodySize.Height,
                "In edit mode Body size should be increased");

            Assert.IsTrue(IsElementFound(PageFooter.CancelButton), "Cancel button should be available in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton), "Submit button should be available in page footer");

            PutContentToClipboard(GetValue(ItemsPage.Body));
            SendText(ItemsPage.Body, " ");
            var bodyEmptySize = GetElementSize(ItemsPage.Body);
            Assert.IsTrue(bodyEmptySize.Height < bodySize.Height, "Body size should decrease on cleaning");
            var assetImagesCount = CountElements(ItemsPage.AssetsSectionImage);
            var assetUploadButtonCount = CountElements(ItemsPage.AssetsSectionImageUploadButton);
            var assetDeleteButtonCount = CountElements(ItemsPage.AssetsSectionImageDeleteButton);
            Assert.IsTrue(assetImagesCount == assetUploadButtonCount && assetImagesCount == assetDeleteButtonCount,
                "Number of asset images, upload buttons, and delete asset button should be the same");

            Click(ItemsPage.AssetsSectionImageDeleteButton);
            var assetImageSource2 = GetElementAttribute(ItemsPage.AssetsSectionImage, "src");
            Assert.IsTrue(assetImageSource != assetImageSource2,
                "On asset Delete button press, next available asset should move to a placeholder of deleted asset");

            var itemImageSource2 = GetElementAttribute(ItemsPage.Image, "src");
            Assert.IsTrue(itemImageSource != itemImageSource2 && itemImageSource2 == assetImageSource2,
                "Item image should be replaced with new first asset image");

            Click(ItemsPage.Body);
            PasteClipboardContent();
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be in view mode");
            Assert.IsTrue(GetElementAttribute(ItemsPage.Image, "src") == itemImageSource2,
                "Item image should be saved on submit");
        }

        [Test, Regression]
        public void RT06030_AssetsAndMediaLibrary()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeEmailTemplate);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorMustHaveAtLeastOneChar),
                "Error that Key must have at least 1 char should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImagePlaceholder), 
                "Item image should be replaced with an image placeholder");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.AssetsSectionImage),
                "Assets section: there should not be anything except +Add button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionAddButton), 
                "Assets section: there should be +Add button");

            var key = $"Key{RandomNumber}";
            SendText(ItemsPage.Key, key);
            SubmitForm();
            Assert.IsTrue(IsElementEquals(ItemsPage.KeyReadOnly, key), "Item changes are not saved");

            EditForm();
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TypeReadOnly), "Type drop-down should be read-only");
            var title = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Title, title);
            Click(PageHeader.PageItemsButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorChangesWillBeDiscardedDialog),
                "Dialog that is saying your changes will be discarded should be displayed");

            Click(ItemsPage.OkButton);
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemsUri}?type="));

            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeEmailTemplate);
            Click(ItemsPage.AssetsSectionAddButton);
            TurnOffInfoPopups();
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionNoImage),
                "Assets section: there should be no images");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.AssetsSectionImage),
                "Assets section: there should be no images");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorThisIsInvalidAsset),
                "Error that this is an invalid asset should be displayed");

            Click(ItemsPage.AssetsSectionImageUploadButton);
            FileManager.Upload(TestConfig.Image138);
            Assert.IsTrue(IsElementFound(ItemsPage.AssetsSectionImage),
                "Assets section: Image should be shown in the section");
            var imgSrc = GetElementAttribute(ItemsPage.Image, "src");
            var imgAsset = GetElementAttribute(ItemsPage.AssetsSectionImage, "src");
            Assert.IsTrue(imgAsset == imgSrc, "Preloaded asset image should be shown as item image");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorThisIsInvalidAsset),
                "Error that this is an invalid asset should not be displayed");

            Click(ItemsPage.AssetsSectionAddButton);
            TurnOffInfoPopups();
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionNoImage),
                "Assets section: there should be no images");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorThisIsInvalidAsset),
                "Error that this is an invalid asset should be displayed");

            Click(ItemsPage.AssetsSectionNoImage);
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library window should be opened");
            Assert.IsTrue(IsElementFound(PageFooter.UploadButton), "Upload button should be available in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.CancelButton), "Cancel button should be available in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton), "Submit button should be available in page footer");
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.TableRowSelected),
                "There should be no selected images in media library");

            Click(string.Format(MediaLibrary.TableRowByText, Path.GetFileName(TestConfig.Image138)));
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.TableRowSelected),
                "Click on image should make the image selected");
            // TODO
            //Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.SelectedImageZoomed),
            //    "Image zoomed in should be displayed on media selection");
            Assert.IsTrue(IsElementFound(PageFooter.UploadButton), "Upload button should be available in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.ShareSubMenu), "Share submenu should be available in page footer");
            MouseOver(PageFooter.ShareSubMenu);
            Assert.IsTrue(IsElementFound(PageFooter.DownloadButton), 
                "Download button should be available in Share submenu in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.CopyUrlButton), 
                "Copy URL button should be available in Share submenu in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton), "Submit button should be available in page footer");
            Assert.IsTrue(IsElementFound(PageFooter.ClearSelectionButton), 
                "Clear Selection button should be available in page footer");

            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog), "Media library window should be closed");
            Assert.IsTrue(CountElements(ItemsPage.AssetsSectionImage) == 2, "Two images should be uploaded to Assets");

            Click(ItemsPage.AssetsSectionImage);
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library window should be opened");
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.TableRowSelected),
                "Image selected should be present in media library");

            Click(ItemsPage.UploadButton);
            FileManager.Upload(TestConfig.ImageSvg);
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorFormatIsNotSupportedDialog),
                "Error dialog that file format is not supported should be displayed");

            Click(ItemsPage.OkButton);
            Click(ItemsPage.UploadButton);
            FileManager.Upload(TestConfig.Image285);

            Click(ItemsPage.SubmitButton);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog), "Media library window should be closed");
            Assert.IsTrue(CountElements(ItemsPage.AssetsSectionImage) == 2, "Two images should be uploaded to Assets section");

            Click(ItemsPage.AssetsSectionAddButton);
            Click(ItemsPage.AssetsSectionImageUploadLastButton);
            FileManager.Upload(TestConfig.Video1Mp4);
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorFormatIsNotSupportedDialog), 
                "Format is not supported dialog should be opened");

            Click(ItemsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorFormatIsNotSupportedDialog),
                "Format is not supported dialog should be closed");

            key = $"Key{RandomNumber}";
            SendText(ItemsPage.Key, key);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorThisIsInvalidAsset), 
                "Error 'invalid asset' should be displayed");

            Click(ItemsPage.AssetsSectionImageDeleteLastButton);
            SubmitForm();

            Assert.IsTrue(CountElements(ItemsPage.AssetsSectionImage) == 2, "Two images should be uploaded to Assets section");
        }

        [Test, Regression]
        public void RT06040_ItemLanguages()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeEmailTemplate);
            var title = $"Auto test {RandomNumber}";
            var subject = $"Subject {RandomNumber}";
            var key = $"Key{RandomNumber}";
            SendText(ItemsPage.Key, key);
            SendText(ItemsPage.Title, title);
            SendText(ItemsPage.Subject, subject);
            Click(ItemsPage.AssetsSectionAddButton);
            Click(ItemsPage.AssetsSectionNoImage);
            Click(string.Format(MediaLibrary.TableRows)); // any image
            Click(ItemsPage.SubmitButton);
            Click(ItemsPage.UseHtmlCheckBox);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN should be chosen on side panel");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageAddButton),
                @"Add language '+' button should be available on side panel");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageDeleteButton),
                "Delete language button should not be shown");

            Click(ItemsPage.LanguageAddButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonActiveInMenu),
                "Language DE button should be displayed in add language menu");

            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonActive),
                "Language DE should be present and active in side panel");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageAddButton),
                @"Add language '+' button should not be available on side panel");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeleteButton),
                "Delete language button should be shown");

            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, title),
                $@"Lang DE: Title should be read-only and equal '{title}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.KeyReadOnly, key), $@"Lang DE: Key should be read-only and equal '{key}'");
            Assert.IsTrue(IsCheckBoxOff(ItemsPage.UseHtmlCheckBoxReadOnly), 
                "Lang DE: Use HTML check box should be read-only and off");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionImage),
                "Lang DE: Assets section should have pre-loaded image");
            Assert.IsTrue(IsElementEquals(ItemsPage.Subject, string.Empty), "Lang DE: Subject should be empty");
            
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.AssetsSectionImageUploadButton),
                "Lang DE: In Assets section should be no upload asset button");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.AssetsSectionImageDeleteButton),
                "Lang DE: In Assets section should be no delete asset button");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.AssetsSectionAddButton),
                "Lang DE: In Assets section should be no add asset button");

            Click(ItemsPage.AssetsSectionImage);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionImagePreview),
                "Lang DE: Click on image should open preview");
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Lang DE: Click on image should not open media library");

            var subjectDe = $"Subject {RandomNumber}";
            SendText(ItemsPage.Subject, subjectDe);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be saved");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN should be active after item save");

            Click(ItemsPage.LanguageDeButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.SubjectReadOnly, subjectDe), 
                $"Lang DE: Subject should be equal {subjectDe}");

            EditForm();
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonActive),
                "Language DE should be active on start edit");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.TypeReadOnly),
                "Dropdown Type should be read-only after item save");

            Click(ItemsPage.LanguageEnButton);
            Click(ItemsPage.LanguageDeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeleteConfirmationDialog),
                "Delete language confirmation dialog should be displayed");

            Click(ItemsPage.DeleteButton);
            TurnOffInfoPopups();
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageDeleteConfirmationDialog),
                "Delete language confirmation dialog should be closed");

            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageEnButton) && 
                          IsElementNotFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN button should not be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonActive),
                "Language DE button should be displayed and active");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageDeleteButton),
                "Language delete button should not be displayed");

            title = $"Auto test {RandomNumber}";
            key = $"Key{RandomNumber}";
            SendText(ItemsPage.Key, key);
            SendText(ItemsPage.Title, title);
            Click(ItemsPage.UseHtmlCheckBox);
            
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionImage),
                "Lang DE: Assets section should have pre-loaded image");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionImageUploadButton),
                "Lang DE: In Assets section should be upload asset button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionImageDeleteButton),
                "Lang DE: In Assets section should be delete asset button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionAddButton),
                "Lang DE: In Assets section should be add asset button");

            Click(ItemsPage.AssetsSectionImageUploadButton);
            FileManager.Upload(TestConfig.Image285);
            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsViewMode(), "Item should be in view mode after save");
            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, title),
                $@"Title should be read-only and equal '{title}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.KeyReadOnly, key), 
                $@"Key should be read-only and equal '{key}'");
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.UseHtmlCheckBoxReadOnly),
                "Use HTML check box should be read-only and on");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssetsSectionImage),
                "Assets section should have pre-loaded image after save");
            Assert.IsTrue(IsElementEquals(ItemsPage.SubjectReadOnly, subjectDe), 
                $@"Subject should be equal '{subjectDe}'");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageEnButton) &&
                          IsElementNotFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN button should not be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonActive),
                "Language DE button should be displayed and active");
        }

        [Test, Regression]
        public void RT06050_ItemTimestamps()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            OpenSecondaryBrowser(); 
            // 2
            NavigateAndLogin(TestConfig.LoginUrl, TestConfig.AdminUser2);
            SwitchToAnotherBrowser();

            // 1
            OpenItemsPage();
            ClickUntilShown(string.Format(ItemsPage.TableRowByText, ItemTypeActionButtonsTemplate), 
                ItemsPage.Status);
            Assert.IsTrue(IsElementFound(ItemsPage.LanguageEnButtonActive),
                "Language EN button should be displayed and active");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButton),
                "Language DE button should be displayed");

            var created = CleanUpString(GetValue(AppsPage.Created));
            var modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(created) || string.IsNullOrEmpty(modified),
                "Created and/or Modified fields look empty");
            var startPos = created.IndexOf(' ');
            var length = created.IndexOf('(') - startPos;
            var created1 = Convert.ToDateTime(created.Substring(startPos, length));
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified1 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(modified1 >= created1, "Created timestamp should be less or equal to Modified field");
            var userProperties = UserDirectoryApi.GetUserData(TestConfig.AdminUser);
            var userProperties2 = UserDirectoryApi.GetUserData(TestConfig.AdminUser2);
            var userName = $"{userProperties.GivenName} {userProperties.FamilyName}";
            var userName2 = $"{userProperties2.GivenName} {userProperties2.FamilyName}";
            Assert.IsTrue(created.Contains(userName) || created.Contains(userName2), 
                "Created should contain firstname and lastname");
            Assert.IsTrue(modified.Contains(userName) || modified.Contains(userName2), 
                "Modified should contain firstname and lastname");
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), $"Item Status should be {StatusActive}");

            EditForm();
            var senderAddr = GetValue(ItemsPage.SenderAddress);
            Click(ItemsPage.LanguageDeButton);
            var bodyDe = GetValue(ItemsPage.Body);
            var assetsNumDe = CountElements(ItemsPage.AssetsSectionImage);
            Click(PageFooter.CancelButton);
            Click(PageFooter.DuplicateButton);
            Assert.IsTrue(IsEditMode(), "Item should be in edit after Duplicate button press");

            Click(ItemsPage.LanguageEnButton);
            var senderAddr1 = GetValue(ItemsPage.SenderAddress);
            Click(ItemsPage.LanguageDeButton);
            var body1De = GetValue(ItemsPage.Body);
            var assetsNum1De = CountElements(ItemsPage.AssetsSectionImage);
            Assert.IsTrue(senderAddr1 == senderAddr,
                "Sender Address field value should be the same for original and duplicate items");
            Assert.IsTrue(bodyDe == body1De,
                "Body field values in lang DE should be the same for original and duplicate items");
            Assert.IsTrue(assetsNumDe == assetsNum1De,
                "Number of assets in lang DE should be the same for original and duplicate items");

            SubmitForm();
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorItemAlreadyExistsDialog), 
                "Error that the item with the same Key already exists should be shown");

            Click(ItemsPage.OkButton);
            Click(ItemsPage.LanguageEnButton);
            var key = $"Key{RandomNumber}";
            var title = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Key, key);
            SendText(ItemsPage.Title, title);
            SubmitForm();
            var itemId = GetEntityIdFromUrl();
            Assert.IsTrue(IsViewMode(), "Item duplicate should be in view mode after submit");

            created = CleanUpString(GetValue(AppsPage.Created));
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(created) || string.IsNullOrEmpty(modified),
                "Created and/or Modified fields look empty");
            startPos = created.IndexOf(' ');
            length = created.IndexOf('(') - startPos;
            var created2 = Convert.ToDateTime(created.Substring(startPos, length));
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified2 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(created2 == modified2, "Modified timestamp should be updated");
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), $"Item Status should be {StatusActive}");

            WaitTime(2);
            // 2
            SwitchToAnotherBrowser();
            OpenItemsPage();
            NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemUri}/{itemId}");
            Click(PageFooter.DeleteButton);
            Click(ItemsPage.DeleteButton);
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemsUri}?type="),
                "User is not redirected to Items page");

            // 1
            SwitchToAnotherBrowser();
            RefreshPage();
            created = CleanUpString(GetValue(AppsPage.Created));
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(created) || string.IsNullOrEmpty(modified),
                "Created and/or Modified fields look empty");
            startPos = created.IndexOf(' ');
            length = created.IndexOf('(') - startPos;
            var created3 = Convert.ToDateTime(created.Substring(startPos, length));
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified3 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(created2 == created3, 
                $"Created timestamp should never change: {created2:ddd MMM d, yyyy H:mm:ss}");
            Assert.IsTrue(modified2 < modified3, "Modified timestamp should be updated");
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusDeleted), $"Item Status should be {StatusDeleted}");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.RestoreButton), 
                "Restore button should be displayed for deleted items");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButtonInactive),
                "Duplicate button should be displayed, but inactive for deleted items");

            WaitTime(2);
            Click(PageFooter.RestoreButton);
            Assert.IsTrue(IsEditMode(), "Item should be in edit mode on Restore button press");

            SubmitForm();
            created = CleanUpString(GetValue(AppsPage.Created));
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(created) || string.IsNullOrEmpty(modified),
                "Created and/or Modified fields look empty");
            startPos = created.IndexOf(' ');
            length = created.IndexOf('(') - startPos;
            var created4 = Convert.ToDateTime(created.Substring(startPos, length));
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified4 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(created2 == created4, 
                $"Created timestamp should never change: {created2:ddd MMM d, yyyy H:mm:ss}");
            Assert.IsTrue(modified3 < modified4, "Modified timestamp should be updated");
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), $"Item Status should be {StatusActive}");

            Click(PageFooter.DeleteButton);
            Click(ItemsPage.DeleteButton);
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeEmailTemplate);
            SendText(ItemsPage.Key, key);
            SubmitForm();
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorItemAlreadyExistsDialog),
                "Error that the item with the same Key already exists should be shown");

            Click(ItemsPage.OkButton);
            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemsUri}?type="),
                "User is not redirected to Items page");
        }

        [Test, Regression]
        public void RT06060_CustomerProfile()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeCustomerProfile);
            Assert.IsTrue(CountElements(ItemsPage.ErrorMustHaveAtLeastOneChar) == 3,
                "Error messages that Given name, Family name, Home showroom must have at least 1 char should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorMustHaveAtLeastTwoChars),
                "Error message that Preferred language must have at least 2 chars should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImagePlaceholder),
                "Item image placeholder should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.CarsOfInterestAddButton),
                "Car of interest section: +Add button should be displayed");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.CarsOfInterestCarModel),
                "Car of interest section: Car Model input should be displayed");

            SendText(ItemsPage.GivenName, "FirstName");
            SendText(ItemsPage.FamilyName, "LastName");
            SendText(ItemsPage.PreferredLanguage, "A");
            SendText(ItemsPage.HomeShowroom, "A");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorMustHaveAtLeastTwoChars),
                "Error message that Preferred language must have at least 2 chars should be displayed");

            SendText(ItemsPage.PreferredLanguage, "AA");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorMustHaveAtLeastTwoChars),
                "Error message that Preferred language must have at least 2 chars should not be displayed");

            SendText(ItemsPage.EmailAddress, "tes@gmailcom");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorDoesNotMatchExpectedPattern),
                "Error message Email Address does not match the expected pattern should be displayed");

            SendText(ItemsPage.EmailAddress, "aa@bb.com");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorDoesNotMatchExpectedPattern),
                "Error message Email Address does not match the expected pattern should not be displayed");

            var title = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Title, title);
            SendText(ItemsPage.MobileDevice, "sadsdsad");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorPlaceholder), "No error messages should be displayed");

            SendText(ItemsPage.MobilePhone, "12345");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorDoesNotMatchExpectedPattern),
                "Error message Mobile Phone does not match the expected pattern should be displayed");

            SendText(ItemsPage.MobilePhone, "123456");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorDoesNotMatchExpectedPattern),
                "Error message Mobile Phone does not match the expected pattern should not be displayed");

            SendText(ItemsPage.MobileAppId, "1234");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorMustHaveAtLeastFiveChars),
                "Error message Mobile App ID must have at least 5 chars should be displayed");

            SendText(ItemsPage.MobileAppId, "12345");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorMustHaveAtLeastFiveChars),
                "Error message Mobile App ID must have at least 5 chars should not be displayed");

            Click(ItemsPage.CarsOfInterestAddButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.CarsOfInterestCarModel),
                "Car model input field should appear on +Add press");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.CarsOfInterestDeleteButton),
                "Delete button for input field should appear on +Add press");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), 
                $"Item Status should be {StatusActive}");

            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.CarsOfInterestAddButton),
                "Car of interest section: +Add button should not be displayed in view mode");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.CarsOfInterestDeleteButton),
                "Delete button for input field should not appear in view mode");

            EditForm();
            Click(ItemsPage.CarsOfInterestAddButton);
            SendText(ItemsPage.CarsOfInterestCarModel, "text");
            Click(ItemsPage.CarsOfInterestAddButton);
            SubmitForm();
            RefreshPage();
            Assert.IsTrue(CountElements(ItemsPage.CarsOfInterestCarModelReadOnly) == 1,
                "Cars of interest section: only 1 car model field should be displayed");

            EditForm();
            Assert.IsTrue(!IsCheckBoxOff(ItemsPage.ContactByPhoneCheckBox) && !IsCheckBoxOn(ItemsPage.ContactByPhoneCheckBox),
                "Check box Contact By Phone should be in neutral state");
            Assert.IsTrue(!IsCheckBoxOff(ItemsPage.ContactByEmailCheckBox) && !IsCheckBoxOn(ItemsPage.ContactByEmailCheckBox),
                "Check box Contact By Email should be in neutral state");

            Click(ItemsPage.ContactByPhoneCheckBox);
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), $"Lang DE: Status should be {StatusActive}");
            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, title), $"Lang DE: Title should be {title}");
            Assert.IsTrue(IsElementEquals(ItemsPage.TypeReadOnly, ItemTypeCustomerProfile), 
                $"Lang DE: Type should be {ItemTypeCustomerProfile}");
            Assert.IsTrue(IsElementEquals(ItemsPage.PreferredLanguageReadOnly, "AA"), @"Lang DE: Preferred Language should be 'AA'");
            Assert.IsTrue(IsElementEquals(ItemsPage.HomeShowroomReadOnly, "A"), @"Lang DE: Home Showroom should be 'A'");
            Assert.IsTrue(IsElementEquals(ItemsPage.GivenNameReadOnly, "FirstName"), @"Lang DE: Given Name should be 'FirstName'");
            Assert.IsTrue(IsElementEquals(ItemsPage.FamilyNameReadOnly, "LastName"), @"Lang DE: Family Name should be 'LastName'");
            Assert.IsTrue(IsElementEquals(ItemsPage.EmailAddressReadOnly, "aa@bb.com"), 
                @"Lang DE: Email Address should be 'aa@bb.com'");
            Assert.IsTrue(IsElementEquals(ItemsPage.MobilePhoneReadOnly, "123456"), @"Lang DE: Mobile Phone should be '123456'");
            Assert.IsTrue(IsElementEquals(ItemsPage.MobileDeviceReadOnly, "sadsdsad"), @"Lang DE: Mobile Device should be 'sadsdsad'");
            Assert.IsTrue(IsElementEquals(ItemsPage.MobileAppIdReadOnly, "12345"), @"Lang DE: Mobile App ID should be '12345'");
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.ContactByPhoneCheckBox),
                @"Lang DE: Contact By Phone checkbox should be 'On'");
            Assert.IsTrue(!IsCheckBoxOn(ItemsPage.ContactByEmailCheckBox) && !IsCheckBoxOff(ItemsPage.ContactByEmailCheckBox),
                "Lang DE: Contact By Email checkbox should be in neutral state");
            Assert.IsTrue(IsElementEquals(ItemsPage.CarsOfInterestCarModelReadOnly, "text"),
                @"Lang DE: Cars of interest section: Car model field should contain 'text'");
            
            SubmitForm();
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), $"Status should be {StatusActive}");

            Click(PageFooter.DuplicateButton);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Duplicated item should be in view mode after submit");

            EditForm();
            ClearTextInElement(ItemsPage.Title);
            var lastName = RandomNumber;
            SendText(ItemsPage.FamilyName, lastName);
            SubmitForm();
            Click(PageFooter.DeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.DeleteConfirmationDialog),
                "Item delete confirmation dialog should be displayed");

            Click(ItemsPage.DeleteButton);
            Assert.IsTrue(IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemsUri}?type="),
                "User is not redirected to Items page after duplicate delete");

            Click(PageFooter.ShowDeletedButton);
            ClickUntilShown(string.Format(ItemsPage.TableRowByTitle, $"FirstName {lastName}"), ItemsPage.Status);
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusDeleted), $"Status should be {StatusDeleted}");

            Click(PageFooter.RestoreButton);
            Click(ItemsPage.LanguageDeButton);
            Click(ItemsPage.LanguageDeleteButton);
            Click(ItemsPage.DeleteButton);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Restored duplicate item should be in view mode after submit");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT06070_WelcomeEmails()
        {
            CurrentTenant = TenantTitle.emails;
            TestStart();
            var app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            var item = ItemApi.SearchItem(ItemTypeWelcomeEmailTemplate);
            if (item == null)
            {
                item = AddItem(ItemType.EmailTemplate, isAddNew: true);
                OpenEntityPage(item);
                EditForm();
                SendText(ItemsPage.Title, ItemTypeWelcomeEmailTemplate);
                SubmitForm();
                OpenItemsPage();
            }
            AddItemToIbeaconApp(app, "$.texts.emails.welcomeEmailTemplate", item);
            RefreshPage();
            OpenEntityPage(item);
            EditForm();
            var subjectEn = GetValue(ItemsPage.Subject);
            if (!subjectEn.Contains(" En"))
            {
                subjectEn = subjectEn + " En";
                SendText(ItemsPage.Subject, subjectEn);
            }
            var senderEn = GetValue(ItemsPage.SenderName);
            if (!senderEn.Contains(" En"))
            {
                senderEn = senderEn + " En";
                SendText(ItemsPage.SenderName, senderEn);
            }
            Click(ItemsPage.LanguageDeButton);
            var subjectDe = GetValue(ItemsPage.Subject);
            if (!subjectDe.Contains(" De"))
            {
                subjectDe = subjectDe + " De";
                SendText(ItemsPage.Subject, subjectDe);
            }
            var senderDe = GetValue(ItemsPage.SenderName);
            if (!senderDe.Contains(" De"))
            {
                senderDe = senderDe + " De";
                SendText(ItemsPage.SenderName, senderDe);
            }
            SubmitForm();

            _mm.InboxHousekeeping(_mm.ClientCxM);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeCustomerProfile);
            var salutation = "Mrs.";
            var firstName = "FN" + RandomNumber;
            var lastName = "LN" + RandomNumber;
            var preferredLang = "en";
            var homeShowroom = CurrentTenantCode;
            SendText(ItemsPage.Salutation, salutation);
            SendText(ItemsPage.GivenName, firstName);
            SendText(ItemsPage.FamilyName, lastName);
            SendText(ItemsPage.EmailAddress, TestConfig.MailServerLogin);
            SendText(ItemsPage.PreferredLanguage, preferredLang);
            SendText(ItemsPage.HomeShowroom, homeShowroom);
            SendText(ItemsPage.MobileAppId, TestData.IbeaconAppId);
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            SubmitForm();
            var id = GetEntityIdFromUrl();
            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            var senderFromEmail = _mm.GetSenderFromEmail(_mm.ClientCxM, subjectEn);
            CloseAlert();
            Assert.IsNotNull(senderFromEmail, $"Welcome email didn't arrive for item id = {id}");
            Assert.IsTrue(senderFromEmail.Contains(senderEn),
                $@"Welcome email has sender '{senderFromEmail}' that does not contain '{senderEn}' for item id = {id}");

            var isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn, salutation);
            Assert.IsTrue(isFoundInBody, $"Salutation is not found in Welcome email for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn, firstName);
            Assert.IsTrue(isFoundInBody, $@"Given Name '{firstName}' is not found in Welcome email for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn, lastName);
            Assert.IsTrue(isFoundInBody, $@"Family Name '{lastName}' is not found in Welcome email for item id = {id}");

            Click(PageFooter.DuplicateButton);
            SubmitForm();
            id = GetEntityIdFromUrl();
            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            senderFromEmail = _mm.GetSenderFromEmail(_mm.ClientCxM, subjectEn);
            CloseAlert();
            Assert.IsNotNull(senderFromEmail, $"Welcome email didn't arrive for item id = {id}");
            Assert.IsTrue(senderFromEmail.Contains(senderEn),
                $@"Welcome email has sender '{senderFromEmail}' that does not contain '{senderEn}' for item id = {id}");

            Click(PageFooter.DuplicateButton);
            SendText(ItemsPage.PreferredLanguage, "de");
            SubmitForm();
            id = GetEntityIdFromUrl();
            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            senderFromEmail = _mm.GetSenderFromEmail(_mm.ClientCxM, subjectDe);
            CloseAlert();
            Assert.IsNotNull(senderFromEmail, $"Welcome email didn't arrive for item id = {id}");
            Assert.IsTrue(senderFromEmail.Contains(senderDe),
                $@"Welcome email has sender '{senderFromEmail}' that does not contain '{senderDe}' for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectDe, salutation);
            Assert.IsTrue(isFoundInBody, $"Salutation is not found in Welcome email for language DE for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectDe, firstName);
            Assert.IsTrue(isFoundInBody, $"Given Name is not found in Welcome email for language DE for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectDe, lastName);
            Assert.IsTrue(isFoundInBody, $"Family Name is not found in Welcome email for language DE for item id = {id}");

            Click(PageFooter.DuplicateButton);
            Click(ItemsPage.LanguageDeleteButton);
            Click(ItemsPage.DeleteButton);
            SubmitForm();
            id = GetEntityIdFromUrl();
            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            senderFromEmail = _mm.GetSenderFromEmail(_mm.ClientCxM, subjectDe);
            CloseAlert();
            Assert.IsNotNull(senderFromEmail, $"Welcome email didn't arrive for item id = {id}");
            Assert.IsTrue(senderFromEmail.Contains(senderDe),
                $@"Welcome email has sender '{senderFromEmail}' that does not contain '{senderDe}' for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectDe, salutation);
            Assert.IsTrue(isFoundInBody, $"Salutation is not found in Welcome email for language DE for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectDe, firstName);
            Assert.IsTrue(isFoundInBody, $"Given Name is not found in Welcome email for language DE for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectDe, lastName);
            Assert.IsTrue(isFoundInBody, $"Family Name is not found in Welcome email for language DE for item id = {id}");

            Click(PageFooter.DuplicateButton);
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageEnButtonActiveInMenu);
            SendText(ItemsPage.PreferredLanguage, "fr");
            SubmitForm();
            id = GetEntityIdFromUrl();
            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            senderFromEmail = _mm.GetSenderFromEmail(_mm.ClientCxM, subjectEn);
            CloseAlert();
            Assert.IsNotNull(senderFromEmail, $"Welcome email didn't arrive for item id = {id}");
            Assert.IsTrue(senderFromEmail.Contains(senderEn),
                $@"Welcome email has sender '{senderFromEmail}' that does not contain '{senderEn}' for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn, salutation);
            Assert.IsTrue(isFoundInBody, $"Salutation is not found in Welcome email for language FR for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn, firstName);
            Assert.IsTrue(isFoundInBody, $"Given Name is not found in Welcome email for language FR for item id = {id}");
            isFoundInBody = _mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn, lastName);
            Assert.IsTrue(isFoundInBody, $"Family Name is not found in Welcome email for language FR for item id = {id}");

            Click(PageFooter.DuplicateButton);
            SendText(ItemsPage.MobileAppId, "55555");
            SubmitForm();
            id = GetEntityIdFromUrl();
            // now welcome email shouldn't come
            // wait for a new email for 60 sec to make sure
            ShowAlert($"Wait {WaitTimeOfNonExistingEmailSeconds} seconds for an email that should not come ...");
            var gotNewMail = WaitForNewMail(_mm.ClientCxM, WaitTimeOfNonExistingEmailSeconds);
            CloseAlert();
            Assert.IsFalse(gotNewMail, $"Welcome email should not be sent on Mobile App ID field change for item id = {id}");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT06080_CreateItemsInApi()
        {
            CurrentTenant = TenantTitle.emails;
            TestStart();
            var app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            var item1 = ItemApi.SearchItem(ItemTypeWelcomeEmailTemplate);
            Assert.IsNotNull(item1, @"Existing 'Welcome email template' item cannot be found");
            AddItemToIbeaconApp(app, "$.texts.emails.welcomeEmailTemplate", item1);
            OpenEntityPage(item1);
            EditForm();
            var subjectEn = GetValue(ItemsPage.Subject);
            if (!subjectEn.Contains(" En"))
            {
                subjectEn = subjectEn + " En";
                SendText(ItemsPage.Subject, subjectEn);
            }
            var senderEn = GetValue(ItemsPage.SenderName);
            if (!senderEn.Contains(" En"))
            {
                senderEn = senderEn + " En";
                SendText(ItemsPage.SenderName, senderEn);
            }
            Click(ItemsPage.LanguageDeButton);
            var subjectDe = GetValue(ItemsPage.Subject);
            if (!subjectDe.Contains(" De"))
            {
                subjectDe = subjectDe + " De";
                SendText(ItemsPage.Subject, subjectDe);
            }
            var senderDe = GetValue(ItemsPage.SenderName);
            if (!senderDe.Contains(" De"))
            {
                senderDe = senderDe + " De";
                SendText(ItemsPage.SenderName, senderDe);
            }
            SubmitForm();

            var user = UserDirectoryApi.GetUserData(TestConfig.AdminUser);
            _mm.InboxHousekeeping(_mm.ClientCxM);
            // new item
            // lang: en
            // preferredLanguage: en
            var jsonModel = "{ \"$schema\": \"http://virtualpromoter.com/schemas/Porsche/profile.json\", " +
                            "\"lang\": \"en\", " +
                            "\"salutation\": \"Mr.\", " +
                            "\"title\": \"Dr\", " +
                            "\"givenName\": \"" + user.GivenName + "\", " +
                            "\"familyName\": \"" + user.FamilyName + "\", " +
                            "\"email\": \"" + TestConfig.NewUser.Email + "\", " +
                            "\"mobileNumber\": \"+380930000000\", " +
                            "\"preferredLanguage\": \"en\", " +
                            "\"carsOfInterest\": [\"911\",\"Panamera\"], " +
                            "\"contactByPhone\": true, " +
                            "\"contactByMail\": false, " +
                            "\"defaultPlace\": \"" + CurrentTenantCode + "\", " +
                            "\"mobileDevice\": \"iPhone7,2\", " +
                            "\"mobileApplicationId\": \"" + TestData.IbeaconMobileApplicationId +
                            "\" }";

            var (item, error) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNotNull(item, $"Item (lang: en, preferredLanguage: en) is not created. \n{error}");

            OpenEntityPage(item);
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), $"Item Status should be {StatusActive}");
            var created = CleanUpString(GetValue(AppsPage.Created));
            var modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(created) || string.IsNullOrEmpty(modified),
                "Created and/or Modified fields look empty");
            var startPos = created.IndexOf(' ');
            var length = created.IndexOf('(') - startPos;
            var created1 = Convert.ToDateTime(created.Substring(startPos, length));
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified1 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(modified1 >= created1, "Created timestamp should be less or equal to Modified field");
            var systemUser = UserDirectoryApi.GetUserData(TestConfig.SystemUser);
            var userName = $"{systemUser.GivenName} {systemUser.FamilyName}";
            Assert.IsTrue(created.Contains(userName), "Created should contain system user firstname and lastname");
            Assert.IsTrue(modified.Contains(userName), "Modified should contain system user firstname and lastname");

            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, "Dr"), @"Title should be 'Dr'");
            Assert.IsTrue(IsElementEquals(ItemsPage.TypeReadOnly, ItemTypeCustomerProfile),
                $"Type should be {ItemTypeCustomerProfile}");
            Assert.IsTrue(IsElementEquals(ItemsPage.PreferredLanguageReadOnly, "en"), @"Preferred Language should be 'en'");
            Assert.IsTrue(IsElementEquals(ItemsPage.HomeShowroomReadOnly, CurrentTenantCode), 
                $@"Home Showroom should be '{CurrentTenantCode}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.GivenNameReadOnly, user.GivenName), 
                $@"Given Name should be '{user.GivenName}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.FamilyNameReadOnly, user.FamilyName), 
                $@"Family Name should be '{user.FamilyName}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.EmailAddressReadOnly, TestConfig.NewUser.Email), 
                $@"Email Address should be '{TestConfig.NewUser.Email}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.MobilePhoneReadOnly, "+380930000000"), @"Mobile Phone should be '+380930000000'");
            Assert.IsTrue(IsElementEquals(ItemsPage.MobileDeviceReadOnly, "iPhone7,2"), @"Mobile Device should be 'iPhone7,2'");
            Assert.IsTrue(IsElementEquals(ItemsPage.MobileAppIdReadOnly, TestData.IbeaconMobileApplicationId), 
                $@"Mobile App ID should be '{TestData.IbeaconMobileApplicationId}'");

            Assert.IsTrue(
                AreCollectionsEqual(GetValuesAsList(ItemsPage.CarsOfInterestCarModelReadOnly), new List<string> { "911", "Panamera" }),
                @"Cars of interest section: it should contain '911', 'Panamera'");
            
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.ContactByPhoneCheckBox),
                @"Contact By Phone checkbox should be 'On'");
            Assert.IsTrue(IsCheckBoxOff(ItemsPage.ContactByEmailCheckBox),
                @"Contact By Email checkbox should be 'Off'");

            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            var senderFromEmail = _mm.GetSenderFromEmail(_mm.ClientCxM, subjectEn);
            CloseAlert();
            Assert.IsNotNull(senderFromEmail, "Welcome email didn't arrive");
            Assert.IsTrue(senderFromEmail.Contains(senderEn),
                $@"Welcome email has sender '{senderFromEmail}' that does not contain '{senderEn}'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                $"Name: Mr. Dr {user.GivenName} {user.FamilyName}"),
                $@"Welcome email does not contain 'Name: Mr. Dr {user.GivenName} {user.FamilyName}'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                $"E-Mail Address: {TestConfig.NewUser.Email}"),
                $@"Welcome email does not contain 'E-Mail Address: {TestConfig.NewUser.Email}'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                "Mobile Number: +380930000000"),
                @"Welcome email does not contain 'Mobile Number: +380930000000'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                "Model of Interest: 911, Panamera"),
                @"Welcome email does not contain 'Model of Interest: 911, Panamera'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                "You give your consent to be contacted via phone."),
                @"Welcome email does not contain 'You give your consent to be contacted via phone.'");

            // new item 1
            var (item2, error2) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNotNull(item2, $"The same item (lang: en, preferredLanguage: en) is not created. \n{error2}");
            var savedId = item2.Id;

            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            senderFromEmail = _mm.GetSenderFromEmail(_mm.ClientCxM, subjectEn);
            CloseAlert();
            Assert.IsNotNull(senderFromEmail, "Welcome email didn't arrive");
            Assert.IsTrue(senderFromEmail.Contains(senderEn),
                $@"Welcome email has sender '{senderFromEmail}' that does not contain '{senderEn}'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                    $"Name: Mr. Dr {user.GivenName} {user.FamilyName}"),
                $@"Welcome email does not contain 'Name: Mr. Dr {user.GivenName} {user.FamilyName}'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                    $"E-Mail Address: {TestConfig.NewUser.Email}"),
                $@"Welcome email does not contain 'E-Mail Address: {TestConfig.NewUser.Email}'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                    "Mobile Number: +380930000000"),
                @"Welcome email does not contain 'Mobile Number: +380930000000'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                    "Model of Interest: 911, Panamera"),
                @"Welcome email does not contain 'Model of Interest: 911, Panamera'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn,
                    "You give your consent to be contacted via phone."),
                @"Welcome email does not contain 'You give your consent to be contacted via phone.'");

            EditForm();
            SendText(ItemsPage.MobilePhone, "+380931111111");
            SubmitForm();
            created = CleanUpString(GetValue(AppsPage.Created));
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(created) || string.IsNullOrEmpty(modified),
                "Created and/or Modified fields look empty");
            startPos = created.IndexOf(' ');
            length = created.IndexOf('(') - startPos;
            var created2 = Convert.ToDateTime(created.Substring(startPos, length));
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified2 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(created1 == created2, 
                $"Created timestamp should never change: {created1:ddd MMM d, yyyy H:mm:ss}");
            Assert.IsTrue(modified2 >= modified1, "Modified timestamp should increase");

            // update item 1
            // add saved id 
            jsonModel = "{ \"id\": " + savedId + ", " +
                        "\"$schema\": \"http://virtualpromoter.com/schemas/Porsche/profile.json\", " +
                        "\"lang\": \"en\", " +
                        "\"salutation\": \"Mr.\", " +
                        "\"title\": \"Dr\", " +
                        "\"givenName\": \"" + user.GivenName + "\", " +
                        "\"familyName\": \"" + user.FamilyName + "\", " +
                        "\"email\": \"" + TestConfig.NewUser.Email + "\", " +
                        "\"mobileNumber\": \"+380930000000\", " +
                        "\"preferredLanguage\": \"en\", " +
                        "\"carsOfInterest\": [\"911\", \"Panamera\"], " +
                        "\"contactByPhone\": true, " +
                        "\"contactByMail\": false, " +
                        "\"defaultPlace\": \"" + CurrentTenantCode + "\", " +
                        "\"mobileDevice\": \"iPhone7,2\", " +
                        "\"mobileApplicationId\": \"" + TestData.IbeaconMobileApplicationId +
                        "\" }";
            (item2, error2) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsTrue(item2 != null && item2.Id == savedId, $"Item (with saved item ID) is not updated. \n{error2}");
            OpenEntityPage(item2);
            created = CleanUpString(GetValue(AppsPage.Created));
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(created) || string.IsNullOrEmpty(modified),
                "Created and/or Modified fields look empty");
            startPos = created.IndexOf(' ');
            length = created.IndexOf('(') - startPos;
            var created5 = Convert.ToDateTime(created.Substring(startPos, length));
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified5 = Convert.ToDateTime(modified.Substring(startPos, length));

            // now email shouldn't come
            // wait for a new email for 60 sec to make sure
            ShowAlert($"Wait {WaitTimeOfNonExistingEmailSeconds} seconds for an email that should not come ...");
            var gotNewMail = WaitForNewMail(_mm.ClientCxM, WaitTimeOfNonExistingEmailSeconds);
            CloseAlert();
            Assert.IsFalse(gotNewMail, "Email should not be sent on item data update");

            // delete item 1
            Click(PageFooter.DeleteButton);
            Click(ItemsPage.DeleteButton);
            OpenEntityPage(item2);
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusDeleted), $"Item Status should be {StatusDeleted}");
            created = CleanUpString(GetValue(AppsPage.Created));
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(created) || string.IsNullOrEmpty(modified),
                "Created and/or Modified fields look empty");
            startPos = created.IndexOf(' ');
            length = created.IndexOf('(') - startPos;
            var created3 = Convert.ToDateTime(created.Substring(startPos, length));
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified3 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(created5 == created3, 
                $"Created timestamp should never change on item delete: {created5:ddd MMM d, yyyy H:mm:ss}");
            Assert.IsTrue(modified3 >= modified5, "Modified timestamp should increase for updated item");

            WaitTime(2);
            // restore item 1 by its update.
            // this workflow looks weird. customer profile item that was deleted by admin can be easily
            // restored by customer. wtf?!
            (item2, error2) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsTrue(item2 != null && item2.Id == savedId, $"Item is not restored. \n{error2}");
            RefreshPage();
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), $"Item Status should be {StatusActive}");
            created = CleanUpString(GetValue(AppsPage.Created));
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(created) || string.IsNullOrEmpty(modified),
                "Created and/or Modified fields look empty");
            startPos = created.IndexOf(' ');
            length = created.IndexOf('(') - startPos;
            var created4 = Convert.ToDateTime(created.Substring(startPos, length));
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified4 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(created3 == created4, 
                $"Created timestamp should never change for updated item: {created3:ddd MMM d, yyyy H:mm:ss}");
            Assert.IsTrue(modified4 >= modified3, "Modified timestamp should increase for updated item");

            // new item 2
            // lang: de
            // preferredLanguage: de
            jsonModel = "{ \"$schema\": \"http://virtualpromoter.com/schemas/Porsche/profile.json\", " +
                        "\"lang\": \"de\", " +
                        "\"salutation\": \"Mr.\", " +
                        "\"title\": \"Dr\", " +
                        "\"givenName\": \"" + user.GivenName + "\", " +
                        "\"familyName\": \"" + user.FamilyName + "\", " +
                        "\"email\": \"" + TestConfig.NewUser.Email + "\", " +
                        "\"mobileNumber\": \"+380930000000\", " +
                        "\"preferredLanguage\": \"de\", " +
                        "\"carsOfInterest\": [\"911\", \"Panamera\"], " +
                        "\"contactByPhone\": true, " +
                        "\"contactByMail\": false, " +
                        "\"defaultPlace\": \"" + CurrentTenantCode + "\", " +
                        "\"mobileDevice\": \"iPhone7,2\", " +
                        "\"mobileApplicationId\": \"" + TestData.IbeaconMobileApplicationId +
                        "\" }";
            var (item3, error3) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNotNull(item3, $"Item with lang DE and preferred language DE is not created. \n {error3}");

            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            senderFromEmail = _mm.GetSenderFromEmail(_mm.ClientCxM, subjectDe);
            CloseAlert();
            Assert.IsNotNull(senderFromEmail, "Welcome email didn't arrive");
            Assert.IsTrue(senderFromEmail.Contains(senderDe),
                $@"Welcome email has sender '{senderFromEmail}' that does not contain '{senderDe}'");
            Assert.IsTrue(_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectDe, "lang=\"de\"", parseAsIs: true),
                "Email body should be in German");

            // new item 3
            // lang: de
            // preferredLanguage: fr
            jsonModel = "{ \"$schema\": \"http://virtualpromoter.com/schemas/Porsche/profile.json\", " +
                        "\"lang\": \"de\", " +
                        "\"salutation\": \"Mr.\", " +
                        "\"title\": \"Dr\", " +
                        "\"givenName\": \"" + user.GivenName + "\", " +
                        "\"familyName\": \"" + user.FamilyName + "\", " +
                        "\"email\": \"" + TestConfig.NewUser.Email + "\", " +
                        "\"mobileNumber\": \"+380930000000\", " +
                        "\"preferredLanguage\": \"fr\", " +
                        "\"carsOfInterest\": [\"911\",\"Panamera\"], " +
                        "\"contactByPhone\": true, " +
                        "\"contactByMail\": false, " +
                        "\"defaultPlace\": \"" + CurrentTenantCode + "\", " +
                        "\"mobileDevice\": \"iPhone7,2\", " +
                        "\"mobileApplicationId\": \"" + TestData.IbeaconMobileApplicationId +
                        "\" }";
            var (item4, error1) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNotNull(item4, $"Item with lang DE and preferred language FR is not created. \n{error1}");

            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            senderFromEmail = _mm.GetSenderFromEmail(_mm.ClientCxM, subjectEn);
            CloseAlert();
            Assert.IsNotNull(senderFromEmail, "Welcome email didn't arrive");
            Assert.IsTrue(senderFromEmail.Contains(senderEn),
                $@"Welcome email has sender '{senderFromEmail}' that does not contain '{senderEn}'");
            Assert.IsTrue(!_mm.IsMailBodyContainsText(_mm.ClientCxM, subjectEn, "lang=\"de\"", parseAsIs: true),
                @"Email body should be in English despite 'lang: de' and 'preferredLanguage: fr'");

            // new item 4
            // lang: fr
            // preferredLanguage: fr
            jsonModel = "{ \"$schema\": \"http://virtualpromoter.com/schemas/Porsche/profile.json\", " +
                        "\"lang\": \"fr\", " +
                        "\"salutation\": \"Mr.\", " +
                        "\"title\": \"Dr\", " +
                        "\"givenName\": \"" + user.GivenName + "\", " +
                        "\"familyName\": \"" + user.FamilyName + "\", " +
                        "\"email\": \"" + TestConfig.NewUser.Email + "\", " +
                        "\"mobileNumber\": \"+380930000000\", " +
                        "\"preferredLanguage\": \"fr\", " +
                        "\"carsOfInterest\": [\"911\",\"Panamera\"], " +
                        "\"contactByPhone\": true, " +
                        "\"contactByMail\": false, " +
                        "\"defaultPlace\": \"" + CurrentTenantCode + "\", " +
                        "\"mobileDevice\": \"iPhone7,2\", " +
                        "\"mobileApplicationId\": \"" + TestData.IbeaconMobileApplicationId +
                        "\" }";
            var (item5, error5) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNull(item5, $"Item with 'lang: fr' and 'preferredLanguage: fr' should not be created. \n {error5}");

            // now email shouldn't come
            // wait for a new email for 60 sec to make sure
            ShowAlert($"Wait {WaitTimeOfNonExistingEmailSeconds} seconds for an email that should not come ...");
            gotNewMail = WaitForNewMail(_mm.ClientCxM, WaitTimeOfNonExistingEmailSeconds);
            CloseAlert();
            Assert.IsFalse(gotNewMail, "Email should not be sent on 'lang: fr' and 'preferredLanguage: fr'");

            // delete item 4
            ItemApi.DeleteItemByAppId(item4.Id, TestData.IbeaconAppId, TestData.IbeaconApiKey);

            OpenEntityPage(item4);
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusDeleted), $"Item Status should be {StatusDeleted}");
        }

        [Test, Regression]
        public void RT06090_Employees()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeEmployee);
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 5,
                "Given name, Family name, Job Title, Email Address, Picture fields must have validation errors");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImagePlaceholder),
                "Item image placeholder should be displayed");

            SendText(ItemsPage.GivenName, "A");
            SendText(ItemsPage.FamilyName, "A");
            SendText(ItemsPage.JobTitle, "A");
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 2,
                "Email Address and Picture fields must still have validation errors");

            SendText(ItemsPage.EmailAddress, "aa@bb.cc");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorPictureRequired),
                @"Error 'picture required' should be shown");

            Click(ItemsPage.PictureUploadButton);
            FileManager.Upload(TestConfig.Image075);
            Assert.IsTrue(IsElementFound(ItemsPage.Image), "Item image is not set");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), $"Item status should be {StatusActive}");

            EditForm();
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(IsElementEquals(ItemsPage.GivenName, "A"), "Given Name field value should be copied to lang DE");
            Assert.IsTrue(IsElementEquals(ItemsPage.FamilyName, "A"), "Family Name field value should be copied to lang DE");
            Assert.IsTrue(IsElementEquals(ItemsPage.EmailAddressReadOnly, "aa@bb.cc"), 
                "Email Address field value should be copied to lang DE");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Image), "Item image should be copied to lang DE");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureImage), 
                "Picture field image should be copied to lang DE");
            Assert.IsTrue(IsElementEquals(ItemsPage.JobTitle, string.Empty), "Job title should be empty for lang DE");

            SendText(ItemsPage.JobTitle, "B");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorPlaceholder), 
                "There should be no validation error messages");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), $"Item status should be {StatusActive}");
            Assert.IsTrue(IsElementEquals(ItemsPage.JobTitleReadOnly, "B"), @"Job title should be equal 'B'");
            
            RefreshPage();
            Assert.IsTrue(IsElementEquals(ItemsPage.JobTitleReadOnly, "A"), @"Job title should be equal 'A' for lang EN");
            Click(ItemsPage.LanguageDeButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.JobTitleReadOnly, "B"), @"Job title should be equal 'B' for lang DE");

            EditForm();
            Click(ItemsPage.LanguageEnButton);
            Click(ItemsPage.LanguageDeleteButton);
            Click(ItemsPage.DeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Image), 
                "Item image should be present on lang EN delete");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureImage),
                "Picture field image should be present on lang EN delete");

            Click(ItemsPage.PictureUploadButton);
            FileManager.Upload(TestConfig.Image059);
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorAssetDoesNotSatisfyDefinedConstraintDialog),
                @"'Asset does not satisfy defined constraint' error dialog should appear");
            
            Click(ItemsPage.OkButton);
            Click(ItemsPage.PictureImage);
            Assert.IsTrue(IsElementFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library dialog window is opened");

            Click(ItemsPage.UploadButton);
            FileManager.Upload(TestConfig.Image08Png);
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorFormatIsNotSupportedDialog),
                @"'File format is not supported' error dialog should appear");

            Click(ItemsPage.OkButton);
            Click(ItemsPage.ClearSelectionButton);
            Click(PageFooter.SubmitButton);
            TurnOffInfoPopups();
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.MediaLibraryDialog),
                @"Media library dialog window should be closed on Clear Selection & Submit");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorPictureRequired),
                @"Error 'picture required' should be shown");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsViewMode(), "Item should be in view mode on Cancel press");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN button should be present and active");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButton),
                "Language DE button should be present and not active");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Image), "Item image should be present");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureImage),
                "Picture field image should be present");
        }

        [Test, Regression]
        public void RT06100_EmployeesLanguagesAndImages()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeEmployee);
            SendText(ItemsPage.GivenName, "A");
            SendText(ItemsPage.FamilyName, "A");
            SendText(ItemsPage.JobTitle, "A");
            SendText(ItemsPage.EmailAddress, "aa@bb.cc");
            Click(ItemsPage.PictureUploadButton);
            FileManager.Upload(TestConfig.Image075);
            SubmitForm();
           
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeEmployee);
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonActive),
                "Language DE button should be active in lang panel");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonInvalid),
                "Language EN button should be red-colored in lang panel");

            Thread.Sleep(1000); // otherwise the page will look broken
            Click(ItemsPage.LanguageEnButtonInvalid);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN button should be active in lang panel");
            //if (IsElementNotFoundQuickly(ItemsPage.LanguageDeButtonInvalid))
            //{
            //    Click(ItemsPage.LanguageDeButton);
            //    Click(ItemsPage.LanguageEnButtonInvalid);
            //}
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonInvalid),
                "Language DE button should be red-colored in lang panel");

            Click(ItemsPage.LanguageDeleteButton);
            Click(ItemsPage.DeleteButton);
            SendText(ItemsPage.EmailAddress, "aa@bb.cc"); // to eliminate overlapping validation error message
            SendText(ItemsPage.FamilyName, "AAAA"); // to eliminate overlapping validation error message
            Click(ItemsPage.PictureEmpty);
            var image075Name = Path.GetFileName(TestConfig.Image075);
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowByText, image075Name)),
                $"Media library: Image {image075Name} should be present");

            Click(string.Format(MediaLibrary.TableRowByText, image075Name));
            MouseOver(PageFooter.ShareSubMenu);
            Thread.Sleep(1000); // otherwise info won't be copied
            Click(PageFooter.CopyUrlButton);
            var image075Url = GetClipboardContent();
            Assert.IsFalse(string.IsNullOrEmpty(image075Url),
                "Image URL has not been copied to Windows clipboard on Copy URL button click");

            var newTabHandle = OpenNewTab();
            NavigateTo(image075Url);
            Assert.IsTrue(IsPageRedirectedTo(image075Url), $"User is not redirected to {image075Url}");
            CloseTab(newTabHandle);

            var downloadedImage = Path.Combine(TestConfig.BrowserDownloadFolder, image075Name);
            FileManager.Delete(downloadedImage);
            MouseOver(PageFooter.ShareSubMenu);
            Click(PageFooter.DownloadButton);
            Assert.IsTrue(FileManager.IsFileExist(image075Name), 
                $"Image file {image075Name} has not been downloaded to {TestConfig.BrowserDownloadFolder}");
            CloseDownloadPanel();
            
            Click(ItemsPage.UploadButton);
            FileManager.Upload(TestConfig.Image075Copy);
            var image075CopyName = Path.GetFileName(TestConfig.Image075Copy);
            Assert.IsTrue(IsElementNotFound(string.Format(MediaLibrary.TableRowByText, image075CopyName)),
                $"Media library: Image {image075CopyName} should not be present after upload");

            Click(ItemsPage.UploadButton);
            FileManager.Upload(TestConfig.Image08);
            var image08Name = Path.GetFileName(TestConfig.Image08);
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowByText, image08Name)),
                $"Media library: Image {image08Name} should be present");
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, image08Name)),
                $"Media library: Image {image08Name} should be selected");

            Click(ItemsPage.SubmitButton);
            SendText(ItemsPage.GivenName, "A");
            SendText(ItemsPage.FamilyName, "A");
            SendText(ItemsPage.JobTitle, "A");
            SendText(ItemsPage.EmailAddress, "aa@bb.cc");
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(ItemsPage.LanguageEnButton) 
                          && IsElementNotFound(ItemsPage.LanguageEnButtonActive),
                "Lang EN should be unavailable for this item");

            Click(PageFooter.DuplicateButton);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Duplicated item should be saved");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageEnButton)
                          && IsElementNotFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Lang EN should be unavailable for duplicated item");
            var itemId = GetEntityIdFromUrl();

            Click(PageFooter.DeleteButton);
            Click(ItemsPage.DeleteButton);
            // otherwise next NavigateTo will not be executed correctly
            IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemsUri}?type=");

            NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemUri}/{itemId}");
            Click(PageFooter.RestoreButton);
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageEnButtonActiveInMenu);
            IsElementFoundQuickly(CommonElement.ValidationError); // otherwise Submit is pressed before SendText
            SendText(ItemsPage.JobTitle, "BBB");
            SubmitForm();
            Assert.IsTrue(IsElementFound(ItemsPage.LanguageEnButtonActive), 
                "Lang EN should be available and active for restored item");
            Assert.IsTrue(IsElementFound(ItemsPage.LanguageDeButton),
                "Lang DE should be available for restored item");

            WaitTime(1);
            Click(ItemsPage.Image);
            var browserTabs = GetTabHandles();
            Assert.IsTrue(browserTabs.Count == 2,
                "New browser tab that should contain only item image is not opened");
            Assert.IsTrue(IsElementFound(ItemsPage.Image), "Item image is not found at new tab");
            CloseTab(browserTabs.Last());
        }

        [Test, Regression]
        public void RT06110_PointOfInterest()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePoi);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorMustHaveAtLeastOneChar),
                "Error message for Title should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorPictureRequired),
                "Error messages 'picture required' for Picture should be displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImagePlaceholder),
                "Item image placeholder should be displayed");

            var title = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Title, title);
            Click(ItemsPage.PictureSectionAddButton);
            Click(ItemsPage.PictureSectionImageUploadButton);
            FileManager.Upload(TestConfig.ImagePoi);
            Assert.IsTrue(IsElementFound(ItemsPage.PictureSectionImage),
                "Picture section: Picture has not been uploaded");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Image), "Item image should be set");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.VideoUploadButton), 
                "Video field should have Upload button");

            Click(ItemsPage.VideoEmpty);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media library should be opened on Video click");

            // only for whole Regression run
            // otherwise there is a risk that at least one MP4 file been uploaded earlier
            if (TestContext.Parameters.Count != 0)
            {
                Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.TableRows),
                    "Media library: there should be no files displayed");
            }

            Click(ItemsPage.UploadButton);
            FileManager.Upload(TestConfig.Video1Mp4);
            Assert.IsTrue(IsElementFound(MediaLibrary.TableRowSelected, 60),
                "Media library: uploaded video should be selected");

            Click(ItemsPage.SubmitButton);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog),
                "Media library should be closed on Submit click");
            Assert.IsTrue(IsEditMode(), "Item should be in edit mode");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Video), "Video field should have a thumbnail");
            var videoUrl = GetValue(ItemsPage.VideoUrl);
            Assert.IsFalse(string.IsNullOrEmpty(videoUrl), "Video URL field should not be empty");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive), "Item is not saved");

            Click(ItemsPage.PictureSectionImage);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.PictureSectionImageUploadButton),
                "Picture section should have not Upload button");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.PictureSectionImageDeleteButton),
                "Picture section should have not Delete button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureSectionImagePreview),
                "Picture section: Picture preview should be shown on picture click");
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog),
                "Media library should not be opened on picture thumbnail click");
            ClickAtPoint(ItemsPage.Status, 0, 0); // close preview

            Click(ItemsPage.Video);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.VideoUploadButton),
                "Video field should have not Upload button");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.VideoDeleteButton),
                "Video field should have not Delete button");
            Assert.IsTrue(IsElementFound(ItemsPage.VideoPreview),
                "Video preview should be shown on video thumbnail click");
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog),
                "Media library should not appear on video thumbnail click");

            EditForm();
            Click(ItemsPage.Video);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media library should be opened on video thumbnail click");
            Click(ItemsPage.ClearSelectionButton);
            Click(ItemsPage.SubmitButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.VideoEmpty), "Video field should be empty");
            Assert.IsTrue(IsElementEquals(ItemsPage.VideoUrl, videoUrl),
                $"Video URL field should contain the same value as before video clear selection: {videoUrl}");

            Click(ItemsPage.VideoUploadButton);
            FileManager.Upload(TestConfig.Video2Mp4);
            Assert.IsTrue(IsElementFound(ItemsPage.Video),
                "Video field should have a thumbnail of second video uploaded");
            var videoUrl2 = GetValue(ItemsPage.VideoUrl);
            Assert.IsTrue(videoUrl2 != videoUrl,
                $"Current Video URL field should not be equal to previous value: {videoUrl}");

            SubmitForm();
            Assert.IsTrue(IsElementEquals(ItemsPage.VideoUrlReadOnly, videoUrl2), "Item is not saved");

            EditForm();
            SendText(ItemsPage.Description, "some text");
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            IsElementFoundQuickly(ItemsPage.VideoUrlReadOnly); // otherwise page won't refresh properly
            Assert.IsTrue(IsElementEquals(ItemsPage.Title, title), $@"Lang DE: Title is not equal to '{title}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.VideoUrlReadOnly, videoUrl2), 
                $@"Lang DE: Video URL is not equal to '{videoUrl2}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.Description, string.Empty), "Lang DE: Description should be empty");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureImage), 
                "Lang DE: Picture section does not have a picture");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Video), "Lang DE: Video does not have a thumbnail");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.VideoUrlReadOnly), "Lang DE: Video URL should be read-only");

            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.VideoUploadButton),
                "Lang DE: Video field should have not Upload button");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.VideoDeleteButton),
                "LAng DE: Video field should have not Delete button");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.PictureSectionImageUploadButton),
                "Lang DE: Picture section should have not Upload button");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.PictureSectionImageDeleteButton),
                "Lang DE: Picture section should have not Delete button");

            Click(ItemsPage.LanguageEnButton);
            Click(ItemsPage.LanguageDeleteButton);
            Click(ItemsPage.DeleteButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.VideoUploadButton),
                "Video field should have Upload button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureSectionImageUploadButton),
                "Picture section should have Upload button");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureSectionImageDeleteButton),
                "Picture section should have Delete button");

            ClickUntilConditionMet(ItemsPage.PictureSectionAddButton,
                () => CountElements(ItemsPage.PictureSectionNoImage) == 1);
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageEnButtonActiveInMenu);
            MouseOver(CommonElement.ValidationError, 2); // wait for picture field validation error
            ClickUntilShown(ItemsPage.LanguageDeButton, ItemsPage.VideoUrlReadOnly);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureEmpty), 
                "Lang DE: Second picture field should be empty");
            if (IsElementNotFoundQuickly(ItemsPage.LanguageEnButtonInvalid))
            {
                Click(ItemsPage.LanguageEnButton);
                WaitTime(1);
                Click(ItemsPage.LanguageDeButton);
            }
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonInvalid),
                "Lang DE: Item should have validation error for lang EN (due to 2nd picture empty field)");

            Click(ItemsPage.LanguageEnButton);
            Click(ItemsPage.PictureSectionImageDeleteLastButton);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item is not saved");
        }

        [Test, Regression]
        public void RT06120_PoiAssignedToPlace()
        {
            Place place = null;
            Item item = null;
            Parallel.Invoke(
                () => AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true),
                () => place = AddPlaceIbeacon(PlaceStatus.NoDevice, pageToBeOpened: 0, isCreateNewPlace: true),
                () => item = AddItem(ItemType.Poi, isAssignImage: true, isAddNew: true)
            );
            TestStart();

            OpenEntityPage(item);
            EditForm();
            Click(ItemsPage.VideoUploadButton);
            FileManager.Upload(TestConfig.Video2Mp4);
            SubmitForm();

            Assert.IsTrue(IsElementNotFound(ItemsPage.AssignedPlacesAddButton),
                "Assigned places: +Add button should not be present in view mode");

            EditForm();
            Assert.IsTrue(IsElementFound(ItemsPage.AssignedPlacesAddButton),
                "Assigned places: +Add button should be present in edit mode");

            Click(ItemsPage.AssignedPlacesAddButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorPlaceholder),
                @"Assigned places: Error 'field is required' for place drop-down should be displayed");

            ClickUntilShown(ItemsPage.AssignedPlacesPlaceDropDown, CommonElement.DropDownOptionList);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(CommonElement.DropDownOptionList, place.Title)),
                $@"Assigned places: place drop-down should contain place '{place.Title}'");
            
            DropDownSelect(ItemsPage.AssignedPlacesPlaceDropDown, place.Title);
            Click(ItemsPage.AssignedPlacesAddButton);
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 1,
                @"Assigned places: 1 error 'field is required' for 2nd place drop-down should be displayed");

            Click(ItemsPage.AssignedPlacesPlaceLastDropDown);
            Assert.IsTrue(IsElementNotFoundQuickly(CommonElement.DropDownOptionList) 
                          || IsElementNotFoundQuickly(string.Format(CommonElement.DropDownOption, place.Title)),
                $@"Assigned places: 2nd place drop-down should not contain place '{place.Title}'");

            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButton),
                "Lang EN button should be displayed, inactive and not highlighted");

            ClickUntilConditionMet(ItemsPage.AssignedPlacesDeleteLastButton, 
                () => CountElements(ItemsPage.AssignedPlacesPlaceDropDown) == 1);
            Assert.IsTrue(CountElements(ItemsPage.AssignedPlacesPlaceDropDown) == 1,
                @"Assigned places: only 1 place drop-down should be present after last place drop-down removal");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be in view mode on submit");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.AssignedPlacesPlaceReadOnly),
                "Assigned places: place field should be shown in read-only mode");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.AssignedPlacesDeleteButton),
                "Assigned places: place field should not have delete button in view mode");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.AssignedPlacesAddButton),
                "Assigned places: should not have +Add button in view mode");

            Click(PageFooter.DuplicateButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.AssignedPlacesPlaceDropDown),
                "Assigned places: there should be no place drop-downs for duplicate item");
            Assert.IsTrue(IsElementFound(ItemsPage.AssignedPlacesAddButton),
                "Assigned places: there should be +Add button for duplicate item");
            Assert.IsTrue(IsElementEquals(ItemsPage.Title, item.LangJsonData.EnJson.Title), 
                $@"Duplicate item title should be '{item.LangJsonData.EnJson.Title}'");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Image),
                "There should item main image for duplicate item");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PictureImage),
                "There should non-empty Picture section for duplicate item");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Video),
                "There should non-empty Video field for duplicate item");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be in view mode on submit");

            EditForm();
            ClickUntilShown(ItemsPage.AssignedPlacesAddButton, ItemsPage.AssignedPlacesPlaceDropDown);
            DropDownSelect(ItemsPage.AssignedPlacesPlaceDropDown, place.Title);
            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsViewMode(), "Item should be in view mode on cancel");
            Assert.IsTrue(IsElementNotFound(ItemsPage.AssignedPlacesPlaceDropDown),
                "Assigned places: there should be no place drop-downs for duplicate item on cancel");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT06130_ServiceBooking()
        {
            CurrentTenant = TenantTitle.emails;
            AppResponse app = null;
            Parallel.Invoke(
                () => app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true), 
                () => PlaceApi.GetRootPlace()
            );
            TestStart();

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeServiceBooking);
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 5,
                "Given name, Family name, Mobile phone, Email Address, Desired date fields must have validation errors");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImagePlaceholder),
                "Item image placeholder should be displayed");

            var firstName = RandomNumber;
            var lastName = RandomNumber;
            SendText(ItemsPage.GivenName, firstName);
            SendText(ItemsPage.FamilyName, lastName);
            SendText(ItemsPage.DesiredDate, "A");
            SendText(ItemsPage.EmailAddress, "aa@bb.cc");
            SendText(ItemsPage.MobilePhone, "-55555");
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 1,
                "Mobile Phone field must still have validation error");

            SendText(ItemsPage.MobilePhone, "+55555");
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be saved successfully");

            var itemId = GetEntityIdFromUrl();
            OpenItemsPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByTitle, $"{firstName} {lastName}")),
                $@"Row with title '{firstName} {lastName}' should be found in Items list");

            NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemUri}/{itemId}");
            EditForm();
            var title = $"Auto test {RandomNumber}";
            SendText(ItemsPage.Title, title);
            SubmitForm();
            OpenItemsPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByTitle, $"{firstName} {lastName}")),
                $@"Row with title '{firstName} {lastName}' should be found in Items list");
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(ItemsPage.TableRowByTitle, title)),
                $@"Row with title '{title}' should not be found in Items list");

            NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemUri}/{itemId}");
            EditForm();
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(IsElementEquals(ItemsPage.GivenNameReadOnly, firstName), "Given Name field value should be copied to lang DE");
            Assert.IsTrue(IsElementEquals(ItemsPage.FamilyNameReadOnly, lastName), "Family Name field value should be copied to lang DE");
            Assert.IsTrue(IsElementEquals(ItemsPage.EmailAddressReadOnly, "aa@bb.cc"),
                "Email Address field value should be copied to lang DE");
            Assert.IsTrue(IsElementEquals(ItemsPage.DesiredDateReadOnly, "A"), "Desired Date field value should be copied to lang DE");
            Assert.IsTrue(IsElementEquals(ItemsPage.MobilePhoneReadOnly, "+55555"), 
                "Mobile Phone field value should be copied to lang DE");

            Click(ItemsPage.ImagePlaceholder);
            Assert.IsTrue(IsElementFoundQuickly(CommonElement.InfoPopup), "Info popup should be shown on image placeholder click");

            SubmitForm();

            Click(PageFooter.DuplicateButton);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Duplicate item should be saved successfully");

            _mm.InboxHousekeeping(_mm.ClientCxM);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeServiceBooking);
            SendText(ItemsPage.GivenName, "A");
            SendText(ItemsPage.FamilyName, "A");
            SendText(ItemsPage.DesiredDate, "A");
            SendText(ItemsPage.EmailAddress, TestConfig.MailServerLogin);
            SendText(ItemsPage.MobilePhone, "+55555");
            SendText(ItemsPage.MobileAppId, TestData.IbeaconAppId);
            SubmitForm();
            // email shouldn't come
            // wait for a new email for 60 sec to make sure
            ShowAlert($"Wait {WaitTimeOfNonExistingEmailSeconds} seconds for an email that should not come ...");
            var gotNewMail = WaitForNewMail(_mm.ClientCxM, WaitTimeOfNonExistingEmailSeconds);
            Assert.IsFalse(gotNewMail, "Email should not be sent on service booking item creation");

            OpenEntityPage(app);
            EditForm();
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.ServiceBookingRecipientAddButton);
            SendText(AppsPage.ServiceBookingRecipientEmail, TestConfig.MailServerLogin);
            SubmitForm();
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeServiceBooking);
            SendText(ItemsPage.GivenName, "A");
            SendText(ItemsPage.FamilyName, "A");
            SendText(ItemsPage.DesiredDate, "A");
            SendText(ItemsPage.EmailAddress, "aaa@bbb.ccc");
            SendText(ItemsPage.MobilePhone, "+55555");
            SendText(ItemsPage.MobileAppId, TestData.IbeaconAppId);
            SubmitForm();
            // email shouldn't come
            // wait for a new email for 60 sec to make sure
            ShowAlert($"Wait {WaitTimeOfNonExistingEmailSeconds} seconds for an email that should not come ...");
            gotNewMail = WaitForNewMail(_mm.ClientCxM, WaitTimeOfNonExistingEmailSeconds);
            CloseAlert();
            Assert.IsFalse(gotNewMail, "Email should not be sent on service booking recipient setup");

            OpenEntityPage(app);
            EditForm();
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.ServiceBookingTemplateDetailsButton);
            Click(string.Format(AppsPage.TableRowByText, ItemTypeServiceBooking));
            SubmitForm();
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeServiceBooking);
            SendText(ItemsPage.GivenName, "A");
            SendText(ItemsPage.FamilyName, "A");
            SendText(ItemsPage.DesiredDate, "A");
            SendText(ItemsPage.EmailAddress, "aaa@bbb.ccc");
            SendText(ItemsPage.MobilePhone, "+55555");
            SendText(ItemsPage.MobileAppId, TestData.IbeaconAppId);
            SubmitForm();
            ShowAlert("Waiting for email ...");
            gotNewMail = WaitForNewMail(_mm.ClientCxM, 600);
            CloseAlert();
            Assert.IsTrue(gotNewMail, "Email should be sent on service booking recipient setup");
        }

        [Test, Regression]
        [Timeout(LongTestTimeoutMilliseconds)]
        public void RT06140_ServiceBookingEmails()
        {
            CurrentTenant = TenantTitle.emails;
            AppResponse app = null;
            const string emailSubject = "Service Booking";
            Parallel.Invoke(
                () => app = AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true),
                () => AddPlaceNoType(PlaceStatus.Any, pageToBeOpened: 0, isCreateNewPlace: true)
            );
            var place = PlaceApi.GetRootPlace();
            if (place == null)
            {
                Assert.Warn($@"No root place on tenant '{CurrentTenant.ToString()}'. Test canceled.");
            }
            TestStart();
            OpenEntityPage(app);
            EditForm();
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.ServiceBookingTemplateDetailsButton);
            Click(string.Format(AppsPage.TableRowByText, ItemTypeServiceBooking));
            if (IsElementNotFoundQuickly(AppsPage.ServiceBookingRecipientEmail))
            {
                Click(AppsPage.ServiceBookingRecipientAddButton);
            }
            SendText(AppsPage.ServiceBookingRecipientEmail, TestConfig.MailServerLogin);
            SubmitForm();

            _mm.InboxHousekeeping(_mm.ClientCxM);
            var jsonModel = "{ \"$schema\": \"http://virtualpromoter.com/schemas/Porsche/serviceBooking.json#\", " +
                           "\"lang\": \"en\"," +
                           "\"givenName\": \"QaAuto\"," +
                           "\"familyName\": \"QaAuto\"," +
                           "\"email\": \"" + TestConfig.MailServerLogin + "\"," +
                           "\"mobileNumber\": \"+666666\", " +
                           "\"desiredDate\": \"2018-01-05\" }";
            var (item, error) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNotNull(item, $"Item is not created by API request. \n{error}");

            var itemUrl = $"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemUri}/{item.Id}";
            NavigateTo(itemUrl);
            Assert.IsTrue(IsElementFound(ItemsPage.TitleReadOnly), "Item page is not opened");

            Assert.IsTrue(IsElementEquals(ItemsPage.GivenNameReadOnly, "QaAuto"), "Given Name field has wrong value");
            Assert.IsTrue(IsElementEquals(ItemsPage.FamilyNameReadOnly, "QaAuto"), "Family Name field has wrong value");
            Assert.IsTrue(IsElementEquals(ItemsPage.EmailAddressReadOnly, TestConfig.MailServerLogin),
                "Email Address field has wrong value");
            Assert.IsTrue(IsElementEquals(ItemsPage.DesiredDateReadOnly, "2018-01-05"), "Desired Date field has wrong value");
            Assert.IsTrue(IsElementEquals(ItemsPage.MobilePhoneReadOnly, "+666666"), "Mobile Phone field has wrong value");

            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            var emailLink = _mm.GetLinkFromEmail(_mm.ClientCxM, emailSubject);
            var isGreetingFound = _mm.IsMailBodyContainsText(_mm.ClientCxM, emailSubject,
                $"Dear {place.Title} team");
            CloseAlert();

            Assert.IsTrue(isGreetingFound != null, $@"Got no email with subject '{emailSubject}'");
            Assert.IsTrue((bool) isGreetingFound, 
                $@"Email greeting should be: 'Dear {place.Title} team'");

            Assert.IsTrue(emailLink == itemUrl, 
                $@"Email link to just created item should be '{itemUrl}', but it is '{emailLink}'");

            OpenEntityPage(place);
            var title = $"Auto test {RandomNumber}";
            if (place.Status != (int) PlaceStatus.Deleted)
            {
                EditForm();
            }
            else
            {
                Click(PageFooter.RestoreButton);
            }
            SendText(PlacesPage.Title, title);
            SubmitForm();
            (item, error) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNotNull(item, $"Item is not created by API request. \n{error}");

            ShowAlert("Waiting for email ...");
            WaitForNewMail(_mm.ClientCxM);
            isGreetingFound = _mm.IsMailBodyContainsText(_mm.ClientCxM, emailSubject,
                $"Dear {title} team");
            CloseAlert();
            Assert.IsTrue(isGreetingFound != null, $@"Got no email with subject '{emailSubject}'");
            Assert.IsTrue((bool)isGreetingFound,
                $@"After root place rename, email greeting should be: 'Dear {title} team'");

            OpenEntityPage(app);
            EditForm();
            ClickUntilShown(AppsPage.TextsButton, AppsPage.EmailsButton);
            Click(AppsPage.EmailsButton);
            Click(AppsPage.ServiceBookingTemplateDetailsButton);
            Click(AppsPage.ClearSelectionButton);
            Click(AppsPage.ServiceBookingRecipientDeleteButton);
            SubmitForm();
            (item, error) = ItemApi.CreateNewItemFromJson(jsonModel, TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNotNull(item, $"Item is not created by API request. \n{error}");
            // email shouldn't come
            // wait for a new email for 60 sec to make sure
            ShowAlert($"Wait {WaitTimeOfNonExistingEmailSeconds} seconds for an email that should not come ...");
            var gotNewMail = WaitForNewMail(_mm.ClientCxM, WaitTimeOfNonExistingEmailSeconds);
            CloseAlert();
            Assert.IsFalse(gotNewMail, 
                "Email should not be sent as service booking recipients are not configured in app");
        }

        [Test, Regression]
        public void RT06150_EventOrPromotion()
        {
            CurrentTenant = TenantTitle.manylang;
            Parallel.Invoke(
                () => AddAppDpt(AppStatus.Available, TestConfig.DptAppVersions[0]),
                () => AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true),
                () => AddAppComposerVipB(TestConfig.ComposerVipbAppEarliestVersion)
            );
            TestStart();

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeEventOrPromotion);
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 5,
                "Title, Title for push, Message for push, Description, Picture fields must have validation errors");

            Assert.IsTrue(IsElementEquals(ItemsPage.TitlePositionDropDown, "Bottom"),
                "Title Position drop-down should equal Bottom");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ActionButtonsButton),
                "Action Buttons button group should be collapsed");

            Click(ItemsPage.ActionButtonsButton);
            Assert.IsTrue(IsCheckBoxNeutral(ItemsPage.AttendCheckBox),
                "Attend check box should look not initialized");
            Assert.IsTrue(IsCheckBoxNeutral(ItemsPage.CallMeBackCheckBox),
                "Call Me Back check box should look not initialized");
            Assert.IsTrue(IsCheckBoxNeutral(ItemsPage.RejectCheckBox),
                "Reject check box should look not initialized");
            Assert.IsTrue(IsCheckBoxNeutral(ItemsPage.SendMeMoreDetailsCheckBox),
                "Send Me More Details check box should look not initialized");
            Assert.IsTrue(IsCheckBoxNeutral(ItemsPage.RequestSalesAppointmentCheckBox),
                "Request Sales Appointment check box should look not initialized");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be shown in footer");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.PushButton),
                "Push button should not be shown in footer");

            SendText(ItemsPage.Title, "A");
            SendText(ItemsPage.TitleForPush, "A");
            SendText(ItemsPage.MessageForPush, "A");
            SendText(ItemsPage.Description, "A");
            Click(ItemsPage.PictureUploadButton);
            FileManager.Upload(TestConfig.ImagePoi);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New item should be saved");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be shown in footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.PushButton),
                "Push button should be shown in footer");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ActionButtonsButton),
                "Action Buttons button group should be collapsed");

            Click(PageFooter.PushButton);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(ItemsPage.TableRowByText, AppTitle.Ibeacon)),
                "Select app window should be displayed");
            Assert.IsTrue(CountElements(string.Format(ItemsPage.TableRowByText, AppTitle.Ibeacon)) == 1,
                "Select app window should contain only iBeacon app");

            Click(string.Format(ItemsPage.TableRowByText, AppTitle.Ibeacon));
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.DoYouWantToPushDialog),
                @"Dialog 'Do you want to push this Event or Promotion to 'Porsche iBeacon App' mobile app ?' should be shown");

            Click(ItemsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(string.Format(ItemsPage.TableRowByText, AppTitle.Ibeacon)),
                "Select app window should be closed");

            // some steps are missed to avoid double checks

            EditForm();
            Click(ItemsPage.ActionButtonsButton);
            Click(ItemsPage.AttendCheckBox);
            Click(ItemsPage.CallMeBackCheckBox);
            Click(ItemsPage.CallMeBackCheckBox);
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(IsElementEquals(ItemsPage.TitlePositionReadOnly, "Bottom"),
                "Lang DE: Title Position should be read-only and equal Bottom");
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 4,
                "Lang DE: Title, Title for push, Message for push, Description, fields must have validation errors");
            Click(ItemsPage.ActionButtonsButton);
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.AttendCheckBox),
                "Lang DE: Attend check box should be On");
            Assert.IsTrue(IsCheckBoxOff(ItemsPage.CallMeBackCheckBox),
                "Lang DE: Call Me Back check box should be Off");
            Assert.IsTrue(IsCheckBoxNeutral(ItemsPage.RejectCheckBox),
                "Lang DE: Reject check box should look not initialized");
            Assert.IsTrue(IsCheckBoxNeutral(ItemsPage.SendMeMoreDetailsCheckBox),
                "Lang DE: Send Me More Details check box should look not initialized");
            Assert.IsTrue(IsCheckBoxNeutral(ItemsPage.RequestSalesAppointmentCheckBox),
                "Lang DE: Request Sales Appointment check box should look not initialized");
            Click(ItemsPage.AttendCheckBox);
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.AttendCheckBox),
                "Lang DE: Attend check box should be read-only");

            SendText(ItemsPage.Title, "A");
            SendText(ItemsPage.TitleForPush, "A");
            SendText(ItemsPage.MessageForPush, "A");
            SendText(ItemsPage.Description, "A");
            SubmitForm();
            var itemId = GetEntityIdFromUrl();
            Assert.IsTrue(IsViewMode(), "New item should be saved");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonActive),
                "Saved items should be displayed in lang DE");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.PushButton),
                "Push button should be shown in footer");

            OpenItemsPage();
            Click(string.Format(ItemsPage.TableRowByTitle, ItemTypeWelcomeEmailTemplate));
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.PushButton),
                "Push button should not be shown in footer");

            AppApi.DeleteApps(true, new [] { AppTitle.Ibeacon }, CurrentTenant);
            NavigateTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemUri}/{itemId}");
            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.PushButton),
                "Push button should not be shown in footer");
        }

        [Test, Regression]
        public void RT06160_EventOrPromotionNotificationsSend()
        {
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);

            var registrationId = NotificationApi.CreateRegistrationId(TestData.DeviceAppUuidAndroid, 
                TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNotNull(registrationId, "Android: Create notification registration ID request has failed");

            var result = NotificationApi.IsRegistrationIdResponseValid(registrationId, TestData.DeviceAppUuidAndroid,
                TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsTrue(result, "Android: Registration of created ID has failed");

            result = NotificationApi.IsRegistrationDeleted(registrationId, TestData.IbeaconAppId,
                TestData.IbeaconApiKey);
            Assert.IsTrue(result, "Android: Registration ID removal has failed");

            registrationId = NotificationApi.CreateRegistrationId(TestData.DeviceAppUuidIphone,
                TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsNotNull(registrationId, "iPhone: Create notification registration ID request has failed");

            result = NotificationApi.IsRegistrationIdResponseValid(registrationId, TestData.DeviceAppUuidIphone,
                TestData.IbeaconAppId, TestData.IbeaconApiKey);
            Assert.IsTrue(result, "iPhone: Registration of created ID has failed");

            result = NotificationApi.IsRegistrationDeleted(registrationId, TestData.IbeaconAppId,
                TestData.IbeaconApiKey);
            Assert.IsTrue(result, "iPhone: Registration ID removal has failed");
        }

        [Test, Regression]
        public void RT06170_UsedCar()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeUsedCar);
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 3,
                "Title, VIN and Pictures fields must have validation errors");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImagePlaceholder),
                "Item image placeholder should be displayed");

            Assert.IsTrue(IsElementFoundQuickly(PageFooter.ImportButton), 
                "Import button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.CancelButton),
                "Cancel button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.SubmitButton),
                "Submit button should be shown in page footer");

            Click(PageFooter.ImportButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.CancelButton),
                "Cancel button should be shown in import dialog");
            Assert.IsTrue(IsDisabledElement(ItemsPage.ImportButton),
                "Inactive Import button should be shown in import dialog");

            SendText(ItemsPage.ImportUrlInput, "www.pornhub.com");
            Assert.IsTrue(IsDisabledElement(ItemsPage.ImportButton),
                "Inactive Import button should be shown in import dialog");

            SendText(ItemsPage.ImportUrlInput, TestConfig.BaseUrl);
            Assert.IsFalse(IsDisabledElement(ItemsPage.ImportButton),
                "Active Import button should be shown in import dialog");

            Click(ItemsPage.ImportButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ImportUrlInput),
                "Import modal dialog should be closed");
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorImportDialog, 60),
                @"Error dialog 'An error occurred while parsing an item' should be displayed in 60 sec or earlier");

            Click(ItemsPage.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorImportDialog),
                @"Error dialog 'An error occurred while parsing an item' should be closed");
            Assert.IsTrue(IsEditMode(), "Item should stay in edit mode");

            Click(PageFooter.ImportButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.ImportUrlInput, string.Empty),
                "Import URL field should be empty on dialog open");

            Click(ItemsPage.CancelButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ImportUrlInput),
                "Import modal dialog should be closed");

            var title = $"Auto test {RandomNumber}";
            var vin = $"{RandomNumber}22";
            SendText(ItemsPage.Title, title);
            SendText(ItemsPage.Vin, vin);
            Click(ItemsPage.PicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionUploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New item should be saved successfully");

            Assert.IsTrue(IsElementNotFoundQuickly(PageFooter.ImportButton),
                "Import button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DeleteButton),
                "Delete button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.DuplicateButton),
                "Duplicate button should be shown in page footer");
            Assert.IsTrue(IsElementFoundQuickly(PageFooter.EditButton),
                "Edit button should be shown in page footer");

            EditForm();
            Click(PageFooter.ImportButton);
            SendText(ItemsPage.ImportUrlInput, TestConfig.BaseUrl);
            Click(ItemsPage.ImportButton);
            Click(ItemsPage.OkButton, timeout: 60);
            Assert.IsTrue(IsElementEquals(ItemsPage.Title, title), $"Title field should be equal to {title}");
            Assert.IsTrue(IsElementEquals(ItemsPage.Vin, vin), $"VIN field should be equal to {vin}");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Image), "Item should have image uploaded");
        }

        [Test, Regression]
        public void RT06180_UsedCarPropertiesAndLanguages()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypeUsedCar);
            var title = $"Auto test {RandomNumber}";
            var vin = $"{RandomNumber}AA";
            SendText(ItemsPage.Title, title);
            SendText(ItemsPage.Vin, $"{RandomNumber}aa");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorDoesNotMatchExpectedPattern),
                @"VIN field should have validation error 'does not match the expected pattern'");

            SendText(ItemsPage.Vin, vin);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorDoesNotMatchExpectedPattern),
                @"VIN field should not have validation error 'does not match the expected pattern'");
            
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorPicturesRequired),
                "Error 'Picture required' should be displayed");

            Click(ItemsPage.PicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionUploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New item should be saved successfully");

            EditForm();
            ClickUntilShown(ItemsPage.FuelConsumptionDataButton, ItemsPage.EnergyEfficiencyClass);
            const string eec = "5";
            SendText(ItemsPage.EnergyEfficiencyClass, eec);
            ClickUntilShown(ItemsPage.VehicleDataButton, ItemsPage.VideoUploadButton);
            Click(ItemsPage.VideoUploadButton);
            FileManager.Upload(TestConfig.Video1Mp4);
            Assert.IsTrue(IsElementFound(ItemsPage.Video, 60),
                "Video field should have a video thumbnail within");
            
            Assert.IsFalse(IsElementEquals(ItemsPage.VideoUrl, string.Empty),
                "Video URL field should contain uploaded video URL");
            var videoUrl = GetValue(ItemsPage.VideoUrl);

            Click(ItemsPage.TeaserCheckBox);
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.TeaserCheckBox),
                "Teaser checkbox should be turned On");

            Click(ItemsPage.EquipmentSectionAddButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Equipment),
                "Equipment section: new Equipment input field should appear on Add button press");
            Assert.IsTrue(IsElementEquals(ItemsPage.Equipment, string.Empty),
                "Equipment section: new Equipment input field should be empty");

            const string equipment = "Some equipment";
            SendText(ItemsPage.Equipment, equipment);
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            IsElementFoundQuickly(ItemsPage.VinReadOnly); // otherwise page won't refresh properly
            ClickUntilShown(ItemsPage.FuelConsumptionDataButton, ItemsPage.EnergyEfficiencyClass);
            ClickUntilShown(ItemsPage.VehicleDataButton, ItemsPage.VideoUrlReadOnly);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.Equipment),
                "Equipment section, lang DE: input field should be absent");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.EquipmentSectionAddButton),
                "Equipment section, lang DE: Add button should be present");
            Assert.IsTrue(IsElementEquals(ItemsPage.VideoUrlReadOnly, videoUrl),
                $"Vehicle data section, lang DE: Video URL should equal to {videoUrl}");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Video),
                "Vehicle data section, lang DE: Video thumbnail should be present");
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.TeaserCheckBox),
                "Equipment section, lang DE: Teaser checkbox should be On");
            Assert.IsTrue(IsElementEquals(ItemsPage.VinReadOnly, vin), 
                $"Lang DE: VIN should equal to {vin}");
            Assert.IsTrue(IsElementEquals(ItemsPage.EnergyEfficiencyClass, string.Empty),
                "Fuel Consumption Data, lang DE: Energy Efficiency Class should be empty");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionImage),
                "Pictures section, lang DE: picture thumbnail should be shown");

            Click(ItemsPage.EquipmentSectionAddButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Equipment),
                "Equipment section, lang DE: input field should appear on Add button press");

            Click(ItemsPage.TeaserCheckBox);
            Assert.IsTrue(IsCheckBoxOn(ItemsPage.TeaserCheckBox), 
                "Equipment section, lang DE: Teaser checkbox should be On (read-only state)");

            Click(ItemsPage.PicturesSectionImage);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionImagePreview),
                "Pictures section, lang DE: picture preview should be opened on thumbnail click");

            const string equipment2 = "Some DE equipment";
            SendText(ItemsPage.Equipment, equipment2);

            Click(ItemsPage.Video);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.VideoPreview),
                "Vehicle data section, lang DE: Video preview should be opened on thumbnail click");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New item should be saved successfully");

            EditForm();
            Click(ItemsPage.LanguageDeleteButton);
            Click(ItemsPage.DeleteButton);
            IsElementFoundQuickly(ItemsPage.Vin); // otherwise page won't refresh properly
            ClickUntilShown(ItemsPage.VehicleDataButton, ItemsPage.VideoUploadButton);
            Click(ItemsPage.VideoUploadButton);
            FileManager.Upload(TestConfig.Video2Mp4);
            Assert.IsTrue(IsElementFound(ItemsPage.Video),
                "Video field should have a video thumbnail within");
            
            Assert.IsFalse(IsElementEquals(ItemsPage.VideoUrl, videoUrl),
                "Video URL field should change on new video upload");

            Click(ItemsPage.TeaserCheckBox);
            Assert.IsTrue(IsCheckBoxOff(ItemsPage.TeaserCheckBox),
                "Equipment section: Teaser checkbox should be Off");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "New item should be saved successfully");
        }

        [Test, Regression]
        public void RT06200_PdfCar()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePdfCar);
            Assert.IsTrue(CountElements(ItemsPage.ErrorPlaceholder) == 2,
                "Title and PDF fields must have validation errors");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImagePlaceholder),
                "Image placeholder should be shown on item page");

            SendText(ItemsPage.Title, $"Auto test {RandomNumber}");
            Click(ItemsPage.PdfUploadButton);
            FileManager.Upload(TestConfig.Pdf2);
            Assert.IsTrue(IsElementFound(ItemsPage.PdfImage), "PDF thumbnail should be displayed in PDF field");
            var pdfSrc1 = GetElementAttribute(ItemsPage.PdfImageThumbnail, "src");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "PDF item not saved");

            EditForm();
            var vin = $"{RandomNumber}22";
            var extColor = "Blue";
            SendText(ItemsPage.Vin, vin);
            SendText(ItemsPage.ExteriorColor, extColor);
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(IsElementEquals(ItemsPage.VinReadOnly, vin),
                $"Lang DE: VIN should be equal to VIN in lang EN - {vin}");
            Assert.IsTrue(IsElementEquals(ItemsPage.ExteriorColor, string.Empty),
                "Lang DE: Exterior Color should be empty");
            Assert.IsTrue(IsElementFound(ItemsPage.PdfImage),
                "Lang DE: PDF thumbnail should be displayed in PDF field");

            Click(ItemsPage.PdfImage);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media library should be opened");
            var filename = Path.GetFileName(TestConfig.Pdf2);
            Assert.IsTrue(
                IsElementFoundQuickly(
                    string.Format(MediaLibrary.TableRowSelectedByText, filename)),
                $"File {filename} should be selected in media library");

            Click(PageFooter.UploadButton);
            FileManager.Upload(TestConfig.ImagePoi);
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorFormatIsNotSupportedDialog),
                @"Dialog 'file format is not supported' should be displayed");

            Click(ItemsPage.OkButton);
            // meaningless check - this file was uploaded in earlier tests
            //var imageFilename = Path.GetFileName(TestConfig.ImagePoi);
            //Assert.IsTrue(
            //    IsElementNotFoundQuickly(
            //        string.Format(
            //            ItemsPage.MediaLibrarySelectedImageByName, imageFilename)),
            //    $"File {imageFilename} should be selected in media library");

            Click(PageFooter.UploadButton);
            FileManager.Upload(TestConfig.Pdf1);
            var pdfFilename = Path.GetFileName(TestConfig.Pdf1);
            Assert.IsTrue(
                IsElementFound(
                    string.Format(MediaLibrary.TableRowSelectedByText, pdfFilename)),
                $"File {pdfFilename} should be selected in media library");

            Click(ItemsPage.SubmitButton);
            var pdfSrc2 = GetElementAttribute(ItemsPage.PdfImageThumbnail, "src");
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media library should be closed");
            Assert.IsFalse(GetElementAttribute(ItemsPage.PdfImageThumbnail, "src") == pdfSrc1,
                "PDF field should have updated image thumbnail");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "PDF item is not saved");

            Click(ItemsPage.LanguageEnButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.VinReadOnly, vin), $"VIN should be equal to {vin}");
            Assert.IsTrue(IsElementEquals(ItemsPage.ExteriorColorReadOnly, extColor),
                $"Exterior Color should be equal to {extColor}");
            Assert.IsTrue(IsElementFound(ItemsPage.PdfImage),
                "PDF thumbnail should be displayed in PDF field");

            Click(PageFooter.DuplicateButton);
            SubmitForm();
            Assert.IsTrue(IsElementFound(ItemsPage.PdfImage),
                "Duplicate, lang EN: PDF thumbnail should be displayed in PDF field");
            Assert.IsTrue(GetElementAttribute(ItemsPage.PdfImageThumbnail, "src") == pdfSrc1,
                "Duplicate, lang EN: PDF field should have correct image thumbnail");
            Click(ItemsPage.LanguageDeButton);
            Assert.IsTrue(IsElementFound(ItemsPage.PdfImage),
                "Duplicate, lang DE: PDF thumbnail should be displayed in PDF field");
            Assert.IsTrue(GetElementAttribute(ItemsPage.PdfImageThumbnail, "src") == pdfSrc2,
                "Duplicate, lang DE: PDF field should have correct image thumbnail");
        }

        [Test, Regression]
        public void RT06210_ImportWithPvms()
        {
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            TestStart();

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[0]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[0],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 1 upload to SFTP timed out");
            OpenItemsPage();                  
            Assert.IsTrue(IsItemCreated(VinImport1),
                $@"Item with VIN '{VinImport1}' not shown in Items list");

            Click(string.Format(ItemsPage.TableRowByText, VinImport1));
            Assert.IsTrue(IsElementEquals(ItemsPage.Status, StatusActive),
                $"Status should be {StatusActive}");
            var created1 = GetTimestamp(AppsPage.Created);
            var modified1 = GetTimestamp(AppsPage.Modified);
            Assert.IsTrue(created1.Equals(modified1), 
                "Modified timestamp and user data should be equal to Created");
            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, TitleImport),
                $@"Title should be '{TitleImport}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.TitleReadOnly, TitleImport),
                $@"VIN should be '{VinImport1}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.TypeReadOnly, ItemTypePorscheCar),
                $"Type should be {ItemTypePorscheCar}");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageBar),
                "Language bar should be present");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN should be present and active");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageDeButton),
                "Language DE should not be present");

            Assert.IsTrue(IsElementEquals(ItemsPage.BtypReadOnly, BtypImport),
                $"Btyp should be {BtypImport}");

            Click(ItemsPage.IndOptionsSectionRow4);
            Click(ItemsPage.IndOptionsSectionExteriorSectionRow6);
            Assert.IsTrue(IsElementEquals(ItemsPage.CodeReadOnly, CodeImport),
                $"Individual Options, row 4 > Exterior, row 6 > Code should be '{CodeImport}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.TextReadOnly, TextImport),
                $"Individual Options, row 4 > Exterior, row 6 > Text should be '{TextImport}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.PriceReadOnly, PriceImport),
                $"Individual Options, row 4 > Exterior, row 6 > Price should be '{PriceImport}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.ExclusiveManufakturOnly, ExclusiveManufakturImport),
                "Individual Options, row 4 > Exterior, row 6 > Exclusive Manufaktur " +
                $@"should be '{ExclusiveManufakturImport}'");

            Assert.IsTrue(CleanUpString(GetValuesAsString(ItemsPage.IndOptionsSectionExteriorSectionRow6)) 
                          == $"6{CodeImport}{TextImport}{PriceImport}",
                "Individual Options, row 4 > Exterior, row 6 should be like: " +
                $"6, {CodeImport}, {TextImport}, {PriceImport}");

            Assert.IsTrue(CountElements(ItemsPage.IndOptionsSectionExteriorRow6EntriesTableRows) == 1,
                "Individual Options, row 4 > Exterior, row 6 > Entries > Entries table should be empty");

            Assert.IsTrue(CleanUpString(GetValuesAsString(ItemsPage.SerialDataSectionHeader)) 
                          == "#HeadlineCategory",
                "Serial Data section table header should be like: #, Headline, Category");

            Click(ItemsPage.SerialDataSectionRow6);
            Assert.IsTrue(IsElementEquals(ItemsPage.SerialDataSectionRow6LastField, LastEntriesImport),
                $@"Serial Data section, row 6 > last field should be '{LastEntriesImport}'");

            Click(ItemsPage.TechnicalDataSectionRow9);
            //Assert.IsTrue(IsElementEquals(ItemsPage.TechnicalDataSectionRow9CategoryReadOnly, CategoryImport),
            //    $@"Technical Data section, row 9 > Category should be '{CategoryImport}'");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.VideoEmpty), "Video field should be empty");

            Click(ItemsPage.EngineButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.CapacityReadOnly, CapacityImport),
                $@"Engine section > Capacity should be '{CapacityImport}'");

            Click(ItemsPage.FinanceOfferButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.DisclaimerReadOnly, DisclaimerImport),
                $@"Finance Offer section > Disclaimer should be '{DisclaimerImport}'");

            Click(ItemsPage.LeasingButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.MonthlyRateReadOnly, MonthlyRateImport),
                $@"Leasing section > Monthly Rate should be '{MonthlyRateImport}'");

            Click(ItemsPage.PicturesButton);
            Assert.IsTrue(CountElements(ItemsPage.PicturesSectionExteriorPictures) == 2,
                "Pictures section > Exterior Pictures should contain 2 pictures");
            Assert.IsTrue(CountElements(ItemsPage.PicturesSectionInteriorPictures) == 2,
                "Pictures section > Interior Pictures should contain 2 pictures");

            Assert.IsTrue(GetElementAttribute(ItemsPage.Image, "src") == 
                          GetElementAttribute(ItemsPage.PicturesSectionExteriorFirstPicture, "src"),
                "Pictures section > 1st picture in Exterior Pictures should be the main picture of the item");

            Click(ItemsPage.PriceButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.TotalCarPriceReadOnly, TotalCarPriceImport),
                $@"Price section > Total Car Price should be '{TotalCarPriceImport}'");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PriceSectionLeasingButton),
                @"Permission 'ViewPorschePriceCarLeasing' not assigned to current user's role");
            Click(ItemsPage.PriceSectionLeasingButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.PriceSectionLeasingSectionMonthlyRate, MonthlyRateImport),
                $@"Price section > Leasing section > Monthly Rate should be '{MonthlyRateImport}'");
        }

        [Test, Regression]
        public void RT06220_ImportWithPvmsUpdateCars()
        {
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            TestStart();

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[1]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[1],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 2 upload to SFTP timed out");
            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[2]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[2],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 3 upload to SFTP timed out");
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport2),
                $@"Item with VIN '{VinImport2}' not shown in Items list");

            Click(string.Format(ItemsPage.TableRowByText, VinImport2));
            var modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(modified), "Modified field looks empty");
            var startPos = modified.IndexOf(' ');
            var length = modified.IndexOf('(') - startPos;
            var modified1 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(IsElementFound(ItemsPage.LanguageBar),
                "Language bar should be present");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN should be present and active");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButton),
                "Language DE should be present");

            Click(ItemsPage.IndOptionsSectionRow1);
            Assert.IsTrue(IsElementEquals(ItemsPage.IndOptionsSectionExteriorColourHeadlineReadOnly, 
                    ExteriorColourHeadlineImport2),
                $@"Individual Options section, row 1 > Headline should be '{ExteriorColourHeadlineImport2}'");

            Click(ItemsPage.IndOptionsSectionExteriorColourRow1);
            Assert.IsTrue(IsElementEquals(ItemsPage.IndOptionsSectionExteriorColourRow1TextReadOnly,
                    TextImport2),
                $@"Individual Options section, row 1 > Entries, row 1 > Text should be '{TextImport2}'");

            Click(ItemsPage.LanguageDeButton);
            Click(ItemsPage.IndOptionsSectionRow1);
            Assert.IsTrue(IsElementEquals(ItemsPage.IndOptionsSectionExteriorColourHeadlineReadOnly,
                    ExteriorColourHeadlineDeImport2),
                $@"Individual Options section, row 1 > Headline should be '{ExteriorColourHeadlineDeImport2}'");

            Click(ItemsPage.IndOptionsSectionExteriorColourRow1);
            Assert.IsTrue(IsElementEquals(ItemsPage.IndOptionsSectionExteriorColourRow1TextReadOnly,
                    TextDeImport2),
                $@"Individual Options section, row 1 > Entries, row 1 > Text should be '{TextDeImport2}'");

            EditForm();
            SendText(ItemsPage.IndOptionsSectionExteriorColourRow1Text, RandomNumber);
            WaitTime(1);
            SubmitForm();
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(modified), "Modified field looks empty");
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified2 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(modified2 > modified1, "Modified time is not updated after item submit" +
                $"Was {modified1}, now {modified2}.");
            var userProperties = UserDirectoryApi.GetUserData(TestConfig.AdminUser);
            var userName = $"{userProperties.GivenName} {userProperties.FamilyName}";
            Assert.IsTrue(modified.Contains(userName),
                "Modified should contain firstname and lastname of current user");

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[2]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[2],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 3 upload #2 to SFTP timed out");
            WaitTime(60);
            RefreshPage();
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(modified), "Modified field looks empty");
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified3 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(modified3 > modified2, "Modified time is not updated after item submit." + 
                $"Was {modified2}, now {modified3}");
            userProperties = UserDirectoryApi.GetUserData(TestConfig.SystemImport);
            userName = $"{userProperties.GivenName} {userProperties.FamilyName}";
            Assert.IsTrue(modified.Contains(userName),
                "Modified should contain firstname and lastname of DPT System user");

            Click(ItemsPage.LanguageDeButton);
            Click(ItemsPage.IndOptionsSectionRow1);
            Click(ItemsPage.IndOptionsSectionExteriorColourRow1);
            Assert.IsTrue(IsElementEquals(ItemsPage.IndOptionsSectionExteriorColourRow1TextReadOnly,
                    TextDeImport2),
                $@"Individual Options section, row 1 > Entries, row 1 > Text should be '{TextDeImport2}'");

            EditForm();
            Click(ItemsPage.LanguageEnButton);
            Click(ItemsPage.LanguageDeleteButton);
            Click(ItemsPage.DeleteButton);
            SendText(ItemsPage.Btyp, RandomNumberWord);
            SubmitForm();
            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[1]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[1],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 2 upload #2 to SFTP timed out");
            WaitTime(60);
            RefreshPage();
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(modified), "Modified field looks empty");
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified4 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(modified4 > modified3, "Modified time is not updated after item submit" +
                $"Was {modified3}, now {modified4}");
            Assert.IsTrue(modified.Contains(userName),
                "Modified should contain firstname and lastname of DPT System user");

            Assert.IsTrue(IsElementFound(ItemsPage.LanguageBar),
                "Language bar should be present");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN should be present and active");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButton),
                "Language DE should be present");

            Assert.IsTrue(IsElementEquals(ItemsPage.BtypReadOnly, BtypImport2),
                $@"Lang EN - Btyp files should be '{BtypImport2}'");
            Click(ItemsPage.LanguageDeButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.BtypReadOnly, BtypImport2),
                $@"Lang DE - Btyp files should be '{BtypImport2}'");

            Click(PageFooter.DeleteButton);
            Click(ItemsPage.DeleteButton);
            if (IsElementFoundQuickly(PageFooter.HideDeletedButton))
            {
                Click(PageFooter.HideDeletedButton);
            }
            Assert.IsTrue(IsElementNotFound(string.Format(ItemsPage.TableRowByText, VinImport2)),
                $@"Item with VIN '{VinImport2}' is shown in Items list after removal");

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[2]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[2],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 3 upload try #3 to SFTP timed out");
            Assert.IsTrue(IsItemCreated(VinImport2),
                $@"Item with VIN '{VinImport2}' not shown in Items list (upload file 3, try #3)");

            Click(string.Format(ItemsPage.TableRowByText, VinImport2));
            modified = CleanUpString(GetValue(AppsPage.Modified));
            Assert.IsFalse(string.IsNullOrEmpty(modified), "Modified field looks empty");
            startPos = modified.IndexOf(' ');
            length = modified.IndexOf('(') - startPos;
            var modified5 = Convert.ToDateTime(modified.Substring(startPos, length));
            Assert.IsTrue(modified5 > modified4, "Modified time is not updated after item submit" +
                $"Was {modified4}, now {modified5}");
            Assert.IsTrue(modified.Contains(userName),
                "Modified should contain firstname and lastname of DPT System user");

            Assert.IsTrue(IsElementFound(ItemsPage.LanguageBar),
                "Language bar should be present");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN should be present and active");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButton),
                "Language DE should be present");

            EditForm();
            var newVin = $"{RandomNumber}11";
            SendText(ItemsPage.Vin, newVin);
            SubmitForm();
            OpenItemsPage();
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByText, newVin)),
                $@"Item with VIN '{newVin}' not shown in Items list");

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[1]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[1],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 2 upload try #3 to SFTP timed out");
            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[2]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[2],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 3 upload try #4 to SFTP timed out");
            Assert.IsTrue(IsItemCreated(VinImport2),
                $@"Item with VIN '{VinImport2}' not shown in Items list (upload files 2 and 3, try #2)");
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.TableRowByText, newVin)),
                $@"Item with new VIN '{newVin}' not shown in Items list (upload files 2 and 3, try #2)");
        }

        [Test, Regression]
        public void RT06230_ImportWithPvmsExceptionFlows()
        {
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            TestStart();

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[3]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[3],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 4 upload to SFTP timed out");
            OpenItemsPage();
            Assert.IsFalse(IsItemCreated(VinImport3),
                $@"Item with VIN '{VinImport3}' is shown in Items list");

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[4]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[4],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 5 upload to SFTP timed out");
            OpenItemsPage();
            Assert.IsFalse(IsItemCreated(VinImport3),
                $@"Item with VIN '{VinImport3}' is shown in Items list");

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[5]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[5],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 6 upload to SFTP timed out");
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport3),
                $@"Item with VIN '{VinImport3}' not shown in Items list");

            Click(string.Format(ItemsPage.TableRowByText, VinImport3));
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImageEmpty),
                "Item image should contain default placeholder");

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[3]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[3],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 4 upload to SFTP timed out");
            WaitTime(60);
            RefreshPage();
            Assert.IsTrue(IsElementFound(ItemsPage.LanguageBar),
                "Language bar should be present");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageDeButtonActive),
                "Language EN should be present and active");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageZhButton),
                "Language ZH should not be present");

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[6]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[6],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 7 upload to SFTP timed out");
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport4),
                $@"Item with VIN '{VinImport4}' not shown in Items list");

            Click(string.Format(ItemsPage.TableRowByText, VinImport4));
            Assert.IsTrue(AreElementsContainText(ItemsPage.ErrorsDuringImportReadOnly, "Not valid json"),
                @"Errors During Import field should contain 'Not valid json'");
        }

        [Test, Regression]
        public void RT06240_ImportWithPvmsOtherTenantCode()
        {
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentTenant = TenantTitle.porsche9520000;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            CurrentTenant = TenantTitle.china;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            TestStart();

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[7]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[7],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 8 upload to SFTP timed out");
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport5),
                $@"Item with VIN '{VinImport5}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[8]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[8],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 9 upload to SFTP timed out");
            Assert.IsTrue(IsItemCreated(VinImport6),
                $@"Item with VIN '{VinImport6}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            var tenantChina = UserDirectoryApi.GetTenant(TenantTitle.china);
            var tenantPorsche9699001 = UserDirectoryApi.GetTenant(TenantTitle.porsche9699001);
            tenantChina.DuplicateTenants.Add(tenantPorsche9699001.TenantId);
            UserDirectoryApi.SaveTenant(tenantChina);
            RefreshPage();
            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[9]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[9],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 10 upload to SFTP timed out");
            Assert.IsTrue(IsItemCreated(VinImport7),
                $@"Item with VIN '{VinImport7}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            ChangeTenant(TenantTitle.porsche9699001);
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport7),
                $@"Item with VIN '{VinImport7}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            ChangeTenant(TenantTitle.china);
            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[10]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[10],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 11 upload to SFTP timed out");
            WaitTime(60);
            OpenItemsPage();
            Click(string.Format(ItemsPage.TableRowByText, VinImport7));
            Assert.IsTrue(IsElementFound(ItemsPage.LanguageBar),
                "Language bar should be present");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageZhButtonActive),
                "Language EN should be present and active");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButton),
                "Language ZH should be present");

            ChangeTenant(TenantTitle.porsche9699001);
            OpenItemsPage();
            Click(string.Format(ItemsPage.TableRowByText, VinImport7));
            Assert.IsTrue(IsElementFound(ItemsPage.LanguageBar),
                "Language bar should be present");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.LanguageEnButtonActive),
                "Language EN should be present and active");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.LanguageZhButton),
                "Language ZH should not be present");

            tenantChina.DuplicateTenants.Add((int) TenantTitle.porsche9520000);
            UserDirectoryApi.SaveTenant(tenantChina);
            ChangeTenant(TenantTitle.china);
            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[11]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[11],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 12 upload to SFTP timed out");
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport8),
                $@"Item with VIN '{VinImport8}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            ChangeTenant(TenantTitle.porsche9699001);
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport8),
                $@"Item with VIN '{VinImport8}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            ChangeTenant(TenantTitle.porsche9520000);
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport8),
                $@"Item with VIN '{VinImport8}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            tenantChina.DuplicateTenants.Remove((int) TenantTitle.porsche9520000);
            UserDirectoryApi.SaveTenant(tenantChina);
            tenantPorsche9699001.DuplicateTenants.Add((int) TenantTitle.porsche9520000);
            UserDirectoryApi.SaveTenant(tenantPorsche9699001);
            ChangeTenant(TenantTitle.china);
            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[12]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[12],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 13 upload to SFTP timed out");
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport9),
                $@"Item with VIN '{VinImport9}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            ChangeTenant(TenantTitle.porsche9699001);
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport9),
                $@"Item with VIN '{VinImport9}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            ChangeTenant(TenantTitle.porsche9520000);
            OpenItemsPage();
            Assert.IsTrue(!IsItemCreated(VinImport9),
                $@"Item with VIN '{VinImport9}' is shown in Items list on tenant {CurrentTenant.ToString()}");

            tenantChina.Code = tenantChina.Code
                .Replace($",{TenantCode.porsche7020000.ToString("f")}", string.Empty);
            UserDirectoryApi.SaveTenant(tenantChina);
            ChangeTenant(TenantTitle.china);
            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[13]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[13],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 14 upload to SFTP timed out");
            WaitTime(60);
            OpenItemsPage();
            Assert.IsTrue(IsElementNotFound(string.Format(ItemsPage.TableRowByText, VinImport10)),
                $@"Item with VIN '{VinImport10}' is shown in Items list on tenant {CurrentTenant.ToString()}");

            ChangeTenant(TenantTitle.porsche9699001);
            OpenItemsPage();
            Assert.IsTrue(IsElementNotFound(string.Format(ItemsPage.TableRowByText, VinImport10)),
                $@"Item with VIN '{VinImport10}' is shown in Items list on tenant {CurrentTenant.ToString()}");

            tenantChina.DuplicateTenants = new List<int>();
            UserDirectoryApi.SaveTenant(tenantChina);
            ChangeTenant(TenantTitle.china);
            _ftpM.UploadFile(TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[14]);
            Assert.IsTrue(WaitForCarImportComplete(_ftpM, TestConfig.SftpImportDirectory, TestConfig.SftpImportFiles[14],
                    TestConfig.PvmsImportTimeout),
                "JSON car import file 15 upload to SFTP timed out");
            OpenItemsPage();
            Assert.IsTrue(IsItemCreated(VinImport11),
                $@"Item with VIN '{VinImport11}' is not shown in Items list on tenant {CurrentTenant.ToString()}");

            ChangeTenant(TenantTitle.porsche9699001);
            OpenItemsPage();
            Assert.IsTrue(IsElementNotFound(string.Format(ItemsPage.TableRowByText, VinImport11)),
                $@"Item with VIN '{VinImport11}' is shown in Items list on tenant {CurrentTenant.ToString()}");
        }

        [Test, Regression]
        public void RT06250_PorscheCar()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            var title = $"Auto test {RandomNumber}";
            var vin = $"{RandomNumber}AA";
            SendText(ItemsPage.Title, title);
            SendText(ItemsPage.Vin, $"{RandomNumber}aa");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorDoesNotMatchExpectedPattern),
                @"VIN field should have validation error 'does not match the expected pattern'");

            SendText(ItemsPage.Vin, vin);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorDoesNotMatchExpectedPattern),
                @"VIN field should not have validation error 'does not match the expected pattern'");

            Click(ItemsPage.IndOptionsSectionAddButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.IndOptionsSectionRow1),
                "Individual Options section: row 1 should be added");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorMustHaveAtLeastOneChar),
                @"Individual Options section: Headline error 'This must have at least one character' not displayed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorEntriesRequired),
                @"Individual Options section: Headline error 'Entries required' not displayed");

            ScrollBottom();
            Click(PageFooter.SubmitButton);
            WaitTime(1);
            Assert.IsTrue(IsEditMode(), "Item should not be saved");
            Assert.IsTrue(IsScrolledIntoView(ItemsPage.ErrorEntriesRequired),
                "Page should be scrolled automatically to validation errors on Submit button click");
            
            SendText(ItemsPage.IndOptionsSectionRow1Headline, "a");
            Click(ItemsPage.IndOptionsSectionAddButton);
            SubmitForm();
            Assert.IsTrue(IsElementNotFound(ItemsPage.IndOptionsSectionExteriorColourHeadlineReadOnly),
                "Individual Options section should be collapsed after Submit");

            EditForm();
            Click(ItemsPage.VideoUploadButton);
            FileManager.Upload(TestConfig.Video1Mp4);
            Assert.IsTrue(IsElementFound(ItemsPage.Video, 60), "Video thumbnail should be displayed");

            Assert.IsFalse(IsElementEquals(ItemsPage.VideoUrl, string.Empty),
                "Video URL should contain URL to a video uploaded to the item");

            ClearTextInElement(ItemsPage.VideoUrl);
            Click(ItemsPage.Video);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media library should be opened on Video field click");
            Assert.IsTrue(IsElementFound(
                    string.Format(MediaLibrary.TableRowSelectedByText, Path.GetFileName(TestConfig.Video1Mp4))),
                "Media library: uploaded video file should be shown and selected");
            //Assert.IsTrue(IsElementFound(MediaLibrary.VideoPlayerStarted),
            //    "Media library: uploaded video file playback is not started");

            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog),
                "Media library should be closed on Submit click");
            Assert.IsFalse(IsElementEquals(ItemsPage.VideoUrl, string.Empty),
                "Video URL should contain URL to a video chosen in Media Library");

            Click(ItemsPage.VideoUploadButton);
            FileManager.Upload(TestConfig.Video3Mov);
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorFormatIsNotSupportedDialog), 
                @"Error 'File format is not supported' should be shown on MOV file upload try");

            Click(ItemsPage.OkButton);
            Assert.IsFalse(IsElementEquals(ItemsPage.VideoUrl, string.Empty),
                "Video URL should contain URL to a video chosen in Media Library");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Video), "Video thumbnail should be displayed");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be submitted successfully");

            Click(ItemsPage.Video);
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.MediaLibraryDialog),
                "Media Library should not open on Video click in view mode");
            Assert.IsTrue(IsElementFound(ItemsPage.VideoPreview),
                "Video preview should be shown on click in view mode");

            Click(ItemsPage.PicturesButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSection),
                "Pictures section: Exterior Pictures section should be visible");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionInteriorPicturesSection),
                "Pictures section: Interior Pictures section should be visible");

            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton)
                && IsElementNotFoundQuickly(ItemsPage.PicturesSectionInteriorPicturesSectionAddButton),
                "Pictures section: +Add buttons should not be shown");

            EditForm();
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            Assert.IsTrue(CountElements(ItemsPage.PicturesSectionExteriorPicturesSectionPicture) == 1,
                "Pictures section > Exterior Pictures section > only 1 picture field should be shown");
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorThisIsInvalidAsset),
                @"Pictures section > Exterior Pictures section > error 'this is invalid asset' should be shown");

            Click(ItemsPage.PicturesSectionCloseButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSection)
                    && IsElementFoundQuickly(ItemsPage.PicturesButton),
                "Pictures section should be collapsed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorConfigurationIsNotValid),
                @"Error 'Configuration is not valid' should be displayed");

            ScrollTop();
            Click(PageFooter.SubmitButton);
            WaitTime(1);
            Assert.IsTrue(IsEditMode(), "Item should not be saved");
            Assert.IsTrue(IsScrolledIntoView(ItemsPage.ErrorConfigurationIsNotValid),
                "Page should be scrolled automatically to validation error on Submit button click");

            Click(ItemsPage.PicturesButton);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionUploadButton);
            FileManager.Upload(TestConfig.ImageJpg);
            Assert.IsTrue(IsElementFound(ItemsPage.PicturesSectionExteriorPicturesSectionPicture),
                "Pictures section > Exterior Pictures > picture has not been uploaded");

            Click(ItemsPage.PicturesSectionInteriorPicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionInteriorPicturesSectionPictureEmpty);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media Library should be shown on picture field click");
            Assert.IsTrue(IsElementNotFound(MediaLibrary.TableRowSelected),
                "Media Library should not have selected image");

            Click(PageFooter.UploadButton);
            FileManager.Upload(TestConfig.Image08Png);
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorFormatIsNotSupportedDialog),
                "Error 'format is not supported' should be displayed");

            Click(ItemsPage.OkButton);
            Click(PageFooter.UploadButton);
            FileManager.Upload(TestConfig.Image025);
            Assert.IsTrue(IsElementFound(
                    string.Format(MediaLibrary.TableRowSelectedByText, Path.GetFileName(TestConfig.Image025))),
                "Media Library should have just uploaded and selected image");

            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsElementFound(ItemsPage.PicturesSectionInteriorPicturesSectionPicture),
                "Pictures section > Interior Pictures > Picture thumbnail should be shown");

            Click(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsEditMode(), 
                "Item cannot be saved because of validation error in Pictures section > Exterior Pictures section");
            Assert.IsTrue(CountElements(ItemsPage.PicturesSectionExteriorPicturesSectionUploadButton) == 2,
                "Pictures section > Interior Pictures > 2 picture fields should be present");

            Click(ItemsPage.PicturesSectionExteriorPicturesSectionDeleteFirstButton);
            Assert.IsTrue(CountElements(ItemsPage.PicturesSectionExteriorPicturesSectionUploadButton) == 1,
                "Pictures section > Interior Pictures > 1 picture field should be present after removal");

            var link1 = GetElementAttribute(ItemsPage.Image, "src");
            var link2 = GetElementAttribute(ItemsPage.PicturesSectionInteriorPicturesSectionPicture, "src");
            Assert.IsTrue(link1 == link2, 
                "Image in Pictures section > Interior section and main item image should be the same");

            Click(ItemsPage.PicturesSectionExteriorPicturesSectionDeleteFirstButton);
            SubmitForm();
            Assert.IsTrue(IsElementFound(ItemsPage.PicturesSectionExteriorPicturesSection),
                "Pictures section > Exterior Pictures section should stay expanded after Submit");

            RefreshPage();
            link1 = GetElementAttribute(ItemsPage.Image, "src");
            Assert.IsTrue(link1 == link2,
                "Image in Pictures section > Interior section and main item image should be the same");
            
            Assert.IsTrue(IsElementNotFound(ItemsPage.PicturesSectionExteriorPicturesSectionPicture)
                        && IsElementNotFound(ItemsPage.PicturesSectionInteriorPicturesSectionPicture),
                "Pictures section should be collapsed after page refresh");
        }

        [Test, Regression]
        public void RT06260_PorscheCarLanguages()
        {
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePorscheCar);
            var title = $"Auto test {RandomNumber}";
            var vin = $"{RandomNumber}AA";
            const string transmission = "some text";
            const string currency = "EUR";
            SendText(ItemsPage.Title, title);
            SendText(ItemsPage.Vin, vin);
            SendText(ItemsPage.Transmission, transmission);
            Click(ItemsPage.VideoUploadButton);
            FileManager.Upload(TestConfig.Video1Mp4);
            Click(ItemsPage.PicturesButton, ignoreIfNoElement: false, timeout: 60);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton);
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionUploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            Click(ItemsPage.PriceButton);
            SendText(ItemsPage.PriceSectionCurrency, currency);
            SubmitForm();

            EditForm();
            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageDeButtonActiveInMenu);
            Assert.IsTrue(IsElementEquals(ItemsPage.Title, title),
                $@"Lang DE: Title is not equal to '{title}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.VinReadOnly, vin),
                $@"Lang DE: VIN is not equal to '{vin}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.Transmission, string.Empty),
                "Lang DE: Transmission should be empty");
            Click(ItemsPage.PriceButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.PriceSectionCurrency, currency),
                $@"Lang DE: Price section > Currency is not equal to '{currency}'");
            Assert.IsTrue(IsElementFound(ItemsPage.Video),
                "Lang DE: Video should contain a thumbnail");
            Click(ItemsPage.PicturesButton);
            Assert.IsTrue(IsElementFound(ItemsPage.PicturesSectionExteriorPicturesSectionPicture),
                "Lang DE: Pictures section > Exterior section > should contain a thumbnail");

            Assert.IsTrue(IsElementNotFound(ItemsPage.VideoUploadButton),
                "Lang DE: There should be no upload video button in Video field");
            Click(ItemsPage.Video);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog),
                "Lang DE: Media library should not be opened on video thumbnail click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.VideoPreview),
                "Lang DE: Video preview picture should be opened");

            Assert.IsTrue(IsElementNotFound(ItemsPage.PicturesSectionExteriorPicturesSectionUploadButton),
                "Lang DE: There should be no upload picture button in Pictures section");
            Assert.IsTrue(IsElementNotFound(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton),
                "Lang DE: There should be no +Add button in Pictures section");
            Click(ItemsPage.PicturesSectionExteriorPicturesSectionPicture);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog),
                "Lang DE: Media library should not be opened on picture in Pictures section thumbnail click");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSectionPreview),
                "Lang DE: Pictures section > preview picture should be opened");

            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be saved successfully");

            Assert.IsTrue(IsElementFound(ItemsPage.LanguageDeButtonActive),
                "Lang DE should be active after item save");

            EditForm();
            Click(ItemsPage.LanguageEnButton);
            Click(ItemsPage.LanguageDeleteButton);
            Click(ItemsPage.DeleteButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.LanguageEnButtonActive),
                "Lang EN should be deleted from item");
            Assert.IsTrue(IsElementFound(ItemsPage.LanguageAddButton),
                "Add language button should be displayed in language panel");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.Vin),
                "VIN field should be available for edit");

            Click(ItemsPage.Video);
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media library should be opened on video thumbnail click");

            Click(PageFooter.CancelButton);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog),
                "Media library should be closed on Cancel button click");

            Click(ItemsPage.PicturesButton);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSectionUploadButton),
                "There should be upload picture button in Pictures section");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.PicturesSectionExteriorPicturesSectionAddButton),
                "There should be +Add button in Pictures section");
            
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be saved successfully");

            Click(PageFooter.DuplicateButton);
            SubmitForm();
            Assert.IsTrue(IsElementFound(ItemsPage.ErrorItemAlreadyExistsDialog),
                $@"Error 'VIN {vin} already exists' should be displayed");

            Click(ItemsPage.OkButton);

            Click(ItemsPage.LanguageAddButton);
            Click(ItemsPage.LanguageEnButtonActiveInMenu);
            WaitTime(2); // otherwise submitted before SendText processing
            SendText(ItemsPage.Vin, $"{RandomNumber}BB");
            WaitTime(1);
            SubmitForm();
            Assert.IsTrue(IsViewMode(), "Item should be saved successfully");
        }

        [Test, Regression]
        public void RT06270_PorscheCarUiImport()
        {
            const string code = "1234567A";
            const string totalCarPrice = @"asdf232'))(#*&*$";
            var vin = $"{RandomNumber}44";
            TestStart();
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1], isOverwriteItems: true);

            OpenItemsPage();
            MouseOver(PageFooter.ImportSubMenu);
            Assert.IsTrue(IsElementFound(PageFooter.PorscheCarButton),
                "Import submenu in page footer should contain Porsche Car button");

            Click(PageFooter.PorscheCarButton);
            Assert.IsTrue(IsPageContainsUri(TestConfig.ItemsImportUri),
                $@"User should be redirected to page with URI '{TestConfig.ItemsImportUri}'");

            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.ImportPageTableColumnByName, "#")),
                @"Import page: column '#' not found");
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.ImportPageTableColumnByName, "Code")),
                @"Import page: column 'Code' not found");
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.ImportPageTableColumnByName, "Total car price")),
                @"Import page: column 'Total car price' not found");
            Assert.IsTrue(IsElementFound(string.Format(ItemsPage.ImportPageTableColumnByName, "VIN")),
                @"Import page: column 'VIN' not found");

            Assert.IsTrue(CountElements(ItemsPage.ImportPageTableRows) == 1,
                "Only 1 data row should be shown in Import table");

            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorMustHaveAtLeastEightChars),
                @"Error 'This must have at least 8 characters' is not shown for Code field");
            Assert.IsTrue(CountElements(ItemsPage.ErrorDoesNotMatchExpectedPattern) == 2,
                @"Error 'This does not match the expected pattern' is not shown for Code and/or VIN");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorMustHaveAtLeastSeventeenChars),
                @"Error 'This must have at least 17 characters' is not shown for VIN field");

            SendText(ItemsPage.ImportTableRow1DetailsCode, "1234567a");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorMustHaveAtLeastEightChars),
                @"Error 'This must have at least 8 characters' is shown for Code field input '1234567a'");
            Assert.IsTrue(CountElements(ItemsPage.ErrorDoesNotMatchExpectedPattern) == 2,
                @"Error 'This does not match the expected pattern' is not shown for Code");

            SendText(ItemsPage.ImportTableRow1DetailsCode, code);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ErrorMustHaveAtLeastEightChars),
                @"Error 'This must have at least 8 characters' is shown for Code field input '1234567a'");
            Assert.IsTrue(CountElements(ItemsPage.ErrorDoesNotMatchExpectedPattern) == 1,
                @"Error 'This does not match the expected pattern' is shown for Code");

            SendText(ItemsPage.ImportTableRow1DetailsTotalCarPrice, totalCarPrice);
            Assert.IsTrue(CountElements(ItemsPage.ErrorDoesNotMatchExpectedPattern) == 1,
                @"Error 'This does not match the expected pattern' is shown for Code");

            Click(PageFooter.SubmitButton);
            Assert.IsTrue(IsEditMode() && IsElementFoundQuickly(CommonElement.ValidationError), 
                "There are validation errors on page. Submit should be impossible.");

            Click(ItemsPage.ImportTableAddButton);
            Assert.IsTrue(IsElementFound(ItemsPage.ImportTableRow2DetailsCode),
                "Import page: table row 2 should be added and shown expanded");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ImportTableRow1DetailsCode),
                "Import page: table row 1 should be shown collapsed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorConfigurationIsNotValid),
                @"Import page: table row 1 should have error 'Configuration is not valid'");

            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ImportTableRow1DeleteButton),
                "Import page: table row 1 should not have delete button");

            Click(ItemsPage.ImportTableRow2DeleteButton);
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ImportTableRow2),
                "Import page: table row 2 should be deleted");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImportTableRow1),
                "Import page: table row 1 should be shown");
            Assert.IsTrue(IsElementNotFoundQuickly(ItemsPage.ImportTableRow1DetailsCode),
                "Import page: table row 1 should be shown collapsed");
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ErrorConfigurationIsNotValid),
                @"Import page: table row 1 should have error 'Configuration is not valid'");

            Click(ItemsPage.ImportTableRow1);
            Assert.IsTrue(IsElementFoundQuickly(ItemsPage.ImportTableRow1DetailsCode),
                "Import page: table row 1 should be shown expanded");

            SendText(ItemsPage.Vin, vin);
            SubmitForm();
            
            Assert.IsTrue(IsElementFound(PageFooter.SubmitButton), 
                "Item should be saved successfully. Submit button disappears during import " + 
                "and appears afterwards.");

            Assert.IsTrue(IsElementEquals(ItemsPage.ImportTableRow1DetailsCode, code),
                $@"Import page: row 1 field Code not equal to '{code}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.ImportTableRow1DetailsTotalCarPrice, totalCarPrice),
                $@"Import page: row 1 field Total Car Price not equal to '{totalCarPrice}'");
            Assert.IsTrue(IsElementEquals(ItemsPage.ImportTableRow1DetailsVin, vin),
                $@"Import page: row 1 field VIN not equal to '{vin}'");

            Click(PageHeader.NavigateBackButton);
            Assert.IsTrue(IsElementNotFound(ItemsPage.ErrorChangesWillBeDiscardedDialog),
                @"Dialog 'Do you really want to leave?' should not be displayed");
            Assert.IsTrue(
                IsPageRedirectedTo($"{TestConfig.BaseUrl}#/{CurrentTenantCode}{TestConfig.ItemsUri}?type="),
                "User should be redirected to Items list page");

            MouseOver(PageFooter.ImportSubMenu);
            Click(PageFooter.PorscheCarButton);
            Assert.IsTrue(IsElementEquals(ItemsPage.ImportTableRow1DetailsCode, string.Empty),
                "Import page: row 1 field Code is not empty");
            Assert.IsTrue(IsElementEquals(ItemsPage.ImportTableRow1DetailsTotalCarPrice, string.Empty),
                "Import page: row 1 field Total Car Price is not empty");
            Assert.IsTrue(IsElementEquals(ItemsPage.ImportTableRow1DetailsVin, string.Empty),
                "Import page: row 1 field VIN is not empty");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestEnd().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public async Task EndFixtureTests()
        {
            var task = _mm.InboxHousekeepingAsync(_mm.ClientCxM);
            if (IsEachFixtureInNewBrowser)
            {
                ClosePrimaryBrowser();
            }
            if (TestContext.Parameters.Count == 0)
            {
                PlaceApi.DeletePlaces();
                AppApi.DeleteApps();
                //ItemApi.DeleteItems(ItemType.Any);
            }
            _ftpM.Dispose();
            await task.ContinueWith(t => _mm.Dispose());
        }
    }
}
