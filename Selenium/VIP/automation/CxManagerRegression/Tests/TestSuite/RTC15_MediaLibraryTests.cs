using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Common.Resources;
using NUnit.Framework;
using OpenQA.Selenium;
using Tests.Helpers;
using Tests.Resources;

namespace Tests.TestSuite
{
    [TestFixture]
    public sealed class RTC15_MediaLibraryTests : ParentTest
    {
        private const string AttrForSorting = "ng-reflect-ng-class";
        private const string AscOrderAttrValue = "order-asc";
        private const string DescOrderAttrValue = "order-desc";
        private const string NoOrderAttrValue = "order-none";
        private const string TitleColumnName = "Title";
        private const string TypeColumnName = "Type";
        private const string CreatedColumnName = "Created";
        private const string SizeColumnName = "Size";

        [OneTimeSetUp]
        public void BeginFixtureTests()
        {
            IsEachTestInNewBrowser = true;  // all tests in this fixture run in new browser
            IsUseAllPageReadyChecks = true; // use normal set of page readiness checks
        }

        [SetUp]
        public async Task Setup()
        { 
            if (TestConfig.IsReportingEnabled)
            {
                await ReportManager.CreateTestAsync().ConfigureAwait(false);
            }

            CurrentTenant = TenantTitle.media1;
            CurrentUser = TestConfig.AdminUser;
            await BackgroundTaskApi.DeleteAllTasksAsync(TestConfig.AdminUser).ConfigureAwait(false);
        }

        [Test, Regression]
        public void RT15010_FilesUpload()
        {
            TestStart();

            Assert.IsTrue(IsElementFound(PageHeader.PagePlacesButton),
                "Places tab button should be shown in page header");
            Assert.IsTrue(IsElementFound(PageHeader.PageAppsButton),
                "Apps tab button should be shown in page header");
            Assert.IsTrue(IsElementFound(PageHeader.PageMediaButton),
                "Media tab button should be shown in page header");

            OpenMediaPage();
            Assert.IsTrue(IsElementFound(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should open on Media button click in page header");

            Assert.IsTrue(IsElementNotFound(MediaLibrary.TableRows),
                "Media library should have no assets for current tenant");
            Assert.IsTrue(IsElementFound(MediaLibrary.UploadButton),
                "Media library should have Upload button in footer");
            Assert.IsTrue(IsElementFound(MediaLibrary.SelectAllButton),
                "Media library should have Select All button in footer");
            Assert.IsTrue(IsElementFound(MediaLibrary.DeselectAllButton),
                "Media library should have Deselect All button in footer");
            Assert.IsTrue(IsElementFound(MediaLibrary.CloseButton),
                "Media library should have Close button in footer");

            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.Image08Png);
            var fileName = Path.GetFileName(TestConfig.Image08Png);
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowSelectedByText, fileName)),
                $@"File '{fileName}' should be uploaded and selected");
            
            Assert.IsTrue(IsElementFound(MediaLibrary.UploadButton),
                "Media library should have Upload button in footer");
            Assert.IsTrue(IsElementFound(MediaLibrary.SelectAllButton),
                "Media library should have Select All button in footer");
            Assert.IsTrue(IsElementFound(MediaLibrary.DeselectAllButton),
                "Media library should have Deselect All button in footer");
            Assert.IsTrue(IsElementFound(MediaLibrary.CloseButton),
                "Media library should have Close button in footer");
            Assert.IsTrue(IsElementFound(MediaLibrary.ShareSubMenu),
                "Media library should have Share submenu in footer");
            Assert.IsTrue(IsElementFound(MediaLibrary.ModifySubMenu),
                "Media library should have Modify submenu in footer");

            MouseOver(MediaLibrary.ModifySubMenu);
            Assert.IsTrue(IsElementFound(MediaLibrary.CropButton),
                "Media library should have Modify submenu with button Crop");
            Assert.IsTrue(IsElementFound(MediaLibrary.ResizeButton),
                "Media library should have Modify submenu with button Resize");
            Assert.IsTrue(IsElementFound(MediaLibrary.ExpiryDateButton),
                "Media library should have Modify submenu with button Expiry Date");
            Assert.IsTrue(IsElementFound(MediaLibrary.TagsButton),
                "Media library should have Modify submenu with button Tags");

            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.ImageSvg, TestConfig.ImageCar);
            fileName = Path.GetFileName(TestConfig.ImageSvg);
            var fileName2 = Path.GetFileName(TestConfig.ImageCar);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName))
                    || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName2)),
                $@"Both files '{fileName}' and '{fileName2}' should be uploaded and one of them is selected");

            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.Otf, TestConfig.Ttf);
            fileName = Path.GetFileName(TestConfig.Otf);
            fileName2 = Path.GetFileName(TestConfig.Ttf);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName))
                    || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName2)),
                $@"Tile view: File both '{fileName}' and '{fileName2}' should be uploaded and one of them is selected");

            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.Mp3, TestConfig.Video1Mp4);
            fileName = Path.GetFileName(TestConfig.Mp3);
            fileName2 = Path.GetFileName(TestConfig.Video1Mp4);
            var fileNameSize = new FileInfo(TestConfig.Mp3).Length.ToString();
            var fileNameSizeInTable = GetValue(string.Format(MediaLibrary.TableRowSizeByTitle, fileName));
            fileNameSizeInTable = string.Join(string.Empty,
                fileNameSizeInTable.Trim().Split(new [] {" ", ".", ","}, StringSplitOptions.RemoveEmptyEntries));
            var fileName2Size = new FileInfo(TestConfig.Video1Mp4).Length.ToString();
            var fileName2SizeInTable = GetValue(string.Format(MediaLibrary.TableRowSizeByTitle, fileName2));
            fileName2SizeInTable = string.Join(string.Empty,
                fileName2SizeInTable.Trim().Split(new [] { " ", ".", "," }, StringSplitOptions.RemoveEmptyEntries));
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName))
                    || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName2)),
                $@"Tile view: Both files '{fileName}' and '{fileName2}' should be uploaded and one of them is selected");
            Assert.IsTrue(fileNameSizeInTable == fileNameSize, $"File {fileName} should have Size = {fileNameSize}");
            Assert.IsTrue(fileName2SizeInTable == fileName2Size, $"File {fileName2} should have Size = {fileName2Size}");

            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.Zip, TestConfig.Pdf1, TestConfig.Arcar, TestConfig.Video3Mov);
            fileName = Path.GetFileName(TestConfig.Zip);
            fileName2 = Path.GetFileName(TestConfig.Pdf1);
            var fileName3 = Path.GetFileName(TestConfig.Arcar);
            var fileName4 = Path.GetFileName(TestConfig.Video3Mov);
            Assert.IsTrue(IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName))
                    || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName2))
                    || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName3))
                    || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, fileName4)),
                $@"Files {fileName}, {fileName2}, {fileName3}, {fileName4} should be uploaded and the last is selected");

            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.ImageGif);
            Assert.IsTrue(IsElementFound(MediaLibrary.ErrorFormatIsNotSupportedDialog),
                "Error dialog that the GIF format is not supported should be displayed");

            Click(MediaLibrary.OkButton);
            Assert.IsTrue(IsElementNotFoundQuickly(MediaLibrary.ErrorFormatIsNotSupportedDialog),
                "Error dialog that the GIF format is not supported should be displayed");

            Assert.IsTrue(IsElementNotFoundQuickly(
                    string.Format(MediaLibrary.TableRowSelectedByText, Path.GetFileName(TestConfig.ImageGif))),
                "File GIF should be not uploaded");

            Click(MediaLibrary.CloseButton);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should close on Close button click in page footer");
            Assert.IsTrue(IsPageContainsUri(TestConfig.PlacesUri),
                "User should stay on Places list page after Media library dialog closure");
        }

        [Test, Regression]
        public void RT15020_FilesAndAllowedActions()
        {
            TestStart();
            
            OpenMediaPage();
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Image08Png);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".svg")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.ImageSvg);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".ttf")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Ttf);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".jpg")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.ImageCar);
                Click(string.Format(MediaLibrary.TableRowByText, ".jpg"));
            }
            Click(string.Format(MediaLibrary.TableRowByText, ".png"));
            Assert.IsTrue(IsElementFound(MediaLibrary.UploadButton),
                "Media library should have Upload button in footer for PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.SelectAllButton),
                "Media library should have Select All button in footer for PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.DeselectAllButton),
                "Media library should have Deselect All button in footer for PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.CloseButton),
                "Media library should have Close button in footer for PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ShareSubMenu),
                "Media library should have Share submenu in footer for PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ModifySubMenu),
                "Media library should have Modify submenu in footer for PNG");

            Click(string.Format(MediaLibrary.TableRowByText, ".jpg"));
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelected, ".jpg"))
                    && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelected, ".png")),
                "Both files JPG and PNG should be selected in tile view");

            Assert.IsTrue(IsElementFound(MediaLibrary.UploadButton),
                "Media library should have Upload button in footer for JPG and PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.SelectAllButton),
                "Media library should have Select All button in footer for JPG and PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.DeselectAllButton),
                "Media library should have Deselect All button in footer for JPG and PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.CloseButton),
                "Media library should have Close button in footer for JPG and PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ShareSubMenu),
                "Media library should have Share submenu in footer for JPG and PNG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ModifySubMenu),
                "Media library should have Modify submenu in footer for JPG and PNG");

            MouseOver(MediaLibrary.ModifySubMenu);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.CropButton),
                "Media library should have Modify submenu without button Crop for 2 assets selected");
            Assert.IsTrue(IsElementNotFound(MediaLibrary.ResizeButton),
                "Media library should have Modify submenu without button Resize for 2 assets selected");
            Assert.IsTrue(IsElementFound(MediaLibrary.ExpiryDateButton),
                "Media library should have Modify submenu with button Expiry Date for 2 assets selected");
            Assert.IsTrue(IsElementFound(MediaLibrary.TagsButton),
                "Media library should have Modify submenu with button Tags for 2 assets selected");

            Click(string.Format(MediaLibrary.TableRowSelectedByText, ".jpg"));
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, ".jpg"))
                    && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowSelectedByText, ".png")),
                "Both files JPG and PNG should be selected in list view");
            Assert.IsTrue(IsElementFound(MediaLibrary.UploadButton),
                "Media library should have Upload button in footer for JPG");
            Assert.IsTrue(IsElementFound(MediaLibrary.SelectAllButton),
                "Media library should have Select All button in footer for JPG");
            Assert.IsTrue(IsElementFound(MediaLibrary.DeselectAllButton),
                "Media library should have Deselect All button in footer for JPG");
            Assert.IsTrue(IsElementFound(MediaLibrary.CloseButton),
                "Media library should have Close button in footer for JPG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ShareSubMenu),
                "Media library should have Share submenu in footer for JPG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ModifySubMenu),
                "Media library should have Modify submenu in footer for JPG");

            MouseOver(MediaLibrary.ModifySubMenu);
            Assert.IsTrue(IsElementFound(MediaLibrary.CropButton),
                "Media library should have Modify submenu with button Crop for JPG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ResizeButton),
                "Media library should have Modify submenu with button Resize for JPG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ExpiryDateButton),
                "Media library should have Modify submenu with button Expiry Date for JPG");
            Assert.IsTrue(IsElementFound(MediaLibrary.TagsButton),
                "Media library should have Modify submenu with button Tags for JPG");

            Click(MediaLibrary.CloseButton);
            OpenMediaPage();
            Assert.IsTrue(IsElementNotFound(MediaLibrary.TableRowSelected),
                "Close and open Media library should reset selection");

            Click(string.Format(MediaLibrary.TableRowByText, ".ttf"));
            Assert.IsTrue(IsElementFound(MediaLibrary.UploadButton),
                "Media library should have Upload button in footer for TTF");
            Assert.IsTrue(IsElementFound(MediaLibrary.SelectAllButton),
                "Media library should have Select All button in footer for TTF");
            Assert.IsTrue(IsElementFound(MediaLibrary.DeselectAllButton),
                "Media library should have Deselect All button in footer for TTF");
            Assert.IsTrue(IsElementFound(MediaLibrary.CloseButton),
                "Media library should have Close button in footer for TTF");
            Assert.IsTrue(IsElementFound(MediaLibrary.ShareSubMenu),
                "Media library should have Share submenu in footer for TTF");
            Assert.IsTrue(IsElementFound(MediaLibrary.ModifySubMenu),
                "Media library should have Modify submenu in footer for TTF");

            MouseOver(MediaLibrary.ModifySubMenu);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.CropButton),
                "Media library should have Modify submenu without button Crop for TTF");
            Assert.IsTrue(IsElementNotFound(MediaLibrary.ResizeButton),
                "Media library should have Modify submenu without button Resize for TTF");
            Assert.IsTrue(IsElementFound(MediaLibrary.ExpiryDateButton),
                "Media library should have Modify submenu with button Expiry Date for TTF");
            Assert.IsTrue(IsElementFound(MediaLibrary.TagsButton),
                "Media library should have Modify submenu with button Tags for TTF");

            Click(MediaLibrary.SelectAllButton);
            Assert.IsTrue(
                IsElementNotFound(MediaLibrary.TableRowNotSelected),
                "All assets in Media library should be selected");

            Click(string.Format(MediaLibrary.TableRowSelectedByText, ".ttf"));
            Assert.IsTrue(
                CountElements(MediaLibrary.TableRowNotSelected) == 1,
                "All assets except one in Media library should be selected in list view");

            Click(MediaLibrary.DeselectAllButton);
            Assert.IsTrue(
                IsElementNotFound(string.Format(MediaLibrary.TableRowSelected, "")),
                "All assets in Media library should be deselected");

            Click(string.Format(MediaLibrary.TableRowByText, ".svg"));
            Assert.IsTrue(IsElementFound(MediaLibrary.UploadButton),
                "Media library should have Upload button in footer for SVG");
            Assert.IsTrue(IsElementFound(MediaLibrary.SelectAllButton),
                "Media library should have Select All button in footer for SVG");
            Assert.IsTrue(IsElementFound(MediaLibrary.DeselectAllButton),
                "Media library should have Deselect All button in footer for SVG");
            Assert.IsTrue(IsElementFound(MediaLibrary.CloseButton),
                "Media library should have Close button in footer for SVG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ShareSubMenu),
                "Media library should have Share submenu in footer for SVG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ModifySubMenu),
                "Media library should have Modify submenu in footer for SVG");

            MouseOver(MediaLibrary.ModifySubMenu);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.CropButton),
                "Media library should have Modify submenu without button Crop for SVG");
            Assert.IsTrue(IsElementNotFound(MediaLibrary.ResizeButton),
                "Media library should have Modify submenu without button Resize for SVG");
            Assert.IsTrue(IsElementFound(MediaLibrary.ExpiryDateButton),
                "Media library should have Modify submenu with button Expiry Date for SVG");
            Assert.IsTrue(IsElementFound(MediaLibrary.TagsButton),
                "Media library should have Modify submenu with button Tags for SVG");

            PressKeys(Keys.Escape);
            Assert.IsTrue(IsElementNotFound(MediaLibrary.MediaLibraryDialog),
                "Media library dialog should close on Esc key press");
            Assert.IsTrue(IsPageContainsUri(TestConfig.PlacesUri),
                "User should stay on Places list page after Media library dialog closure");
        }

        [Test, Regression]
        public void RT15030_FilterType()
        {
            TestStart();

            OpenMediaPage();
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Image08Png);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp4")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Video1Mp4);
            }
            
            ClickUntilShown(MediaLibrary.FilterDropDown, CommonElement.DropDownOptionList);
            var originalTypes = new[]
            {
                AssetTypeAll, AssetTypeImage, AssetTypeVideo, AssetTypeAudio, AssetTypePdf,
                AssetTypeFont, AssetTypeCar, AssetTypeZip
            };
            Assert.IsTrue(AreCollectionsEqual(
                    GetValuesAsList(CommonElement.DropDownOptionList), originalTypes),
                "Filter dropdown should contain following types: " + string.Join(", ", originalTypes));

            DropDownSelect(MediaLibrary.FilterDropDown, AssetTypeImage, false);
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png"))
                || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".svg"))
                || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".jpg")),
                "Tiles view: Assets of image types (PNG, JPG, SVG) " + 
                "should be displayed with image filter setting");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp4")),
                "Tiles view: Assets of image types only (PNG, JPG, SVG) " +
                "should be displayed with image filter setting");

            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png"))
                || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".svg"))
                || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".jpg")),
                "List view: Assets of image types (PNG, JPG, SVG) " +
                "should be displayed with image filter setting");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp4")),
                "List view: Assets of image types only (PNG, JPG, SVG) " +
                "should be displayed with image filter setting");

            DropDownSelect(MediaLibrary.FilterDropDown, AssetTypeVideo, false);
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mov"))
                || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp4")),
                "List view: Assets of video types (MOV, MP4) " +
                "should be displayed with video filter setting");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png"))
                && IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".svg"))
                && IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".jpg")),
                "List view: Assets of video types only (MOV, MP4) " +
                "should be displayed with video filter setting");

            Click(MediaLibrary.CloseButton);
            ChangeTenant(TenantTitle.media2);
            OpenMediaPage();
            Assert.IsTrue(
                IsElementNotFoundQuickly(MediaLibrary.TableRows),
                $@"'{TenantTitle.media2}': Media library should be empty");

            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.Mp3);
            Assert.IsTrue(
                IsElementFound(string.Format(MediaLibrary.TableRowSelectedByText, ".mp3")),
                $@"'{TenantTitle.media2}': MP3 file should be uploaded and selected");

            Click(MediaLibrary.CloseButton);
            ChangeTenant(TenantTitle.media1);
            OpenMediaPage();
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mov"))
                || IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp4")),
                "List view: Assets of video types (MOV, MP4) " +
                "should be displayed with video filter setting");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png"))
                && IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".svg"))
                && IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".jpg")),
                "List view: Assets of video types only (MOV, MP4) " +
                "should be displayed with video filter setting");

            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.Pdf2);
            Assert.IsTrue(IsElementNotFound(
                    string.Format(MediaLibrary.TableRowSelectedByText, ".pdf")),
                "PDF files should be uploaded not displayed for current Video filter");

            DropDownSelect(MediaLibrary.FilterDropDown, AssetTypeAll, false);
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mov"))
                && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png"))
                && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp3"))
                && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".pdf")),
                @"List view: All assets types should be displayed with 'All' filter setting");
        }

        [Test, Regression]
        public void RT15040_ListViewSorting()
        {
            CurrentTenant = TenantTitle.media2;
            TestStart();
            OpenMediaPage();
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Image08Png);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp4")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Video1Mp4);
            }
            ChangeTenant(TenantTitle.media1);
            OpenMediaPage();
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Image08Png);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".jpg")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.ImageCar);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".ttf")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Ttf);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".svg")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.ImageSvg);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp4")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Video1Mp4);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mov")))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Video3Mov);
            }

            Assert.IsTrue(
                IsAlphabeticallySorted(MediaLibrary.TableRows, isElementDropDown: false),
                $"Asset {TitleColumnName}s should be in ascending order");
            Assert.IsTrue(GetElementAttribute(MediaLibrary.TitleColumn, AttrForSorting) == AscOrderAttrValue,
                $"{TitleColumnName} column header should contain arrow for ascending order");

            Click(MediaLibrary.SizeColumn);
            Assert.IsTrue(IsSortedByNumber(string.Format(MediaLibrary.TableRowSizeByTitle, ".")),
                $"Asset {SizeColumnName}s should be in ascending order");
            Assert.IsTrue(GetElementAttribute(MediaLibrary.SizeColumn, AttrForSorting) == AscOrderAttrValue,
                $"{SizeColumnName} column header should contain arrow for ascending order");
            Assert.IsTrue(GetElementAttribute(MediaLibrary.TitleColumn, AttrForSorting) == NoOrderAttrValue,
                $"{SizeColumnName} sorting: {TitleColumnName} column header should contain no arrow");

            Click(MediaLibrary.SizeColumn);
            Assert.IsTrue(
                IsSortedByNumber(string.Format(MediaLibrary.TableRowSizeByTitle, "."), 
                    isDescendingOrder: true),
                $"Asset {SizeColumnName}s should be in ascending order");
            Assert.IsTrue(GetElementAttribute(MediaLibrary.SizeColumn, AttrForSorting) == DescOrderAttrValue,
                $"{SizeColumnName} column header should contain arrow for descending order");
            Assert.IsTrue(GetElementAttribute(MediaLibrary.TitleColumn, AttrForSorting) == NoOrderAttrValue,
                $"{SizeColumnName} sorting: {TitleColumnName} column header should contain no arrow");

            DropDownSelect(MediaLibrary.FilterDropDown, AssetTypeImage, false);
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png"))
                    && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".svg"))
                    && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".jpg")),
                "Assets of image types (PNG, JPG, SVG) should be displayed with Image filter setting");
            Assert.IsTrue(
                IsSortedByNumber(string.Format(MediaLibrary.TableRowSizeByTitle, "."),
                    isDescendingOrder: true),
                "Images filter: should contain the same assets in size descending sort order");

            Click(MediaLibrary.UploadButton);
            FileManager.Upload(TestConfig.ImageCar);
            IsElementFound(
                string.Format(MediaLibrary.TableRowByText, Path.GetFileName(TestConfig.ImageCar)));
            Assert.IsTrue(IsSortedByNumber(string.Format(MediaLibrary.TableRowSizeByTitle, "."),
                    isDescendingOrder: true),
                $"Asset {SizeColumnName}s should be in descending order after new asset upload");

            Click(MediaLibrary.CloseButton);
            ChangeTenant(TenantTitle.media2);
            OpenMediaPage();
            Assert.IsTrue(
                IsAlphabeticallySorted(MediaLibrary.TableRows, isElementDropDown: false),
                $"Asset {TitleColumnName}s should be in ascending order");

            Click(MediaLibrary.CloseButton);
            ChangeTenant(TenantTitle.media1);
            OpenMediaPage();
            Assert.IsTrue(
                IsSortedByNumber(string.Format(MediaLibrary.TableRowSizeByTitle, "."),
                    isDescendingOrder: true),
                $"Asset {SizeColumnName}s should be in descending order");
            Assert.IsTrue(GetElementAttribute(MediaLibrary.SizeColumn, AttrForSorting) == DescOrderAttrValue,
                $"{SizeColumnName} column header should contain arrow for descending order");
            Assert.IsTrue(
                IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png"))
                    && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".svg"))
                    && IsElementFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".jpg")),
                "Assets of image types (PNG, JPG, SVG) " +
                "should be displayed with Image filter setting");
            Assert.IsTrue(
                IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mov"))
                    && IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp4"))
                    && IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp3"))
                    && IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".ttf")),
                @"Non-image type assets should be hidden with Image filter setting");

            Click(MediaLibrary.TypeColumn);
            Assert.IsTrue(
                IsAlphabeticallySorted(string.Format(MediaLibrary.TableRowTypeByTitle, "."), 
                    isElementDropDown: false),
                $"Asset {TypeColumnName}s should be in ascending order");
            Assert.IsTrue(GetElementAttribute(MediaLibrary.TypeColumn, AttrForSorting) == AscOrderAttrValue,
                $"{TypeColumnName} column header should contain arrow for ascending order");

            HoldKeyAndClick(MediaLibrary.SizeColumn, Keys.LeftShift);
            Assert.IsTrue(
                IsAlphabeticallySorted(string.Format(MediaLibrary.TableRowTypeByTitle, "."),
                    isElementDropDown: false),
                $"Asset {TypeColumnName}s should be in ascending order");
            var types = GetElementsText(string.Format(MediaLibrary.TableRowTypeByTitle, ".")).Distinct();
            foreach (var type in types)
            {
                Assert.IsTrue(
                    IsSortedByNumber(string.Format(MediaLibrary.TableRowSizeByType, type)),
                    $"Asset {SizeColumnName}s should be in ascending order for type {type}");
            }
            Assert.IsTrue(GetElementAttribute(MediaLibrary.SizeColumn, "class").Contains(AscOrderAttrValue),
                $"{SizeColumnName} column header should contain arrow for ascending order");

            Click(MediaLibrary.CreatedColumn);
            Assert.IsTrue(
                IsAlphabeticallySorted(string.Format(MediaLibrary.TableRowCreatedByTitle, "."),
                    isElementDropDown: false, isReverseOrder: true),
                $"Asset {CreatedColumnName} dates should be in descending order");
            Assert.IsTrue(GetElementAttribute(MediaLibrary.CreatedColumn, AttrForSorting) == DescOrderAttrValue,
                $"{CreatedColumnName} column header should contain arrow for descending order");
        }

        [Test, Regression]
        public void RT15050_AspectRatioAndFormat()
        {
            CurrentTenant = TenantTitle.media1;
            AddAppIbeacon(TestConfig.IbeaconAppVersions[1]);
            TestStart();
            OpenMediaPage();
            var validJpg = Path.GetFileName(TestConfig.ImageCar);
            var invalidJpg = Path.GetFileName(TestConfig.ImageJpg);
            var validVideo = Path.GetFileName(TestConfig.Video1Mp4);
            var invalidRatio = Path.GetFileName(TestConfig.Image025);
            var validPng = Path.GetFileName(TestConfig.Image08Png);
            var validSvg = Path.GetFileName(TestConfig.ImageSvg);
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".png"), 0.5))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Image08Png);
            }
            if (IsElementNotFoundQuickly(
                string.Format(MediaLibrary.TableRowByText, validJpg), 0.5))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.ImageCar);
            }
            if (IsElementNotFoundQuickly(
                string.Format(MediaLibrary.TableRowByText, invalidJpg), 0.5))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.ImageJpg);
            }
            if (IsElementNotFoundQuickly(
                string.Format(MediaLibrary.TableRowByText, invalidRatio), 0.5))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Image025);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".ttf"), 0.5))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Ttf);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".svg"), 0.5))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.ImageSvg);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mp4"), 0.5))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Video1Mp4);
            }
            if (IsElementNotFoundQuickly(string.Format(MediaLibrary.TableRowByText, ".mov"), 0.5))
            {
                Click(MediaLibrary.UploadButton);
                FileManager.Upload(TestConfig.Video3Mov);
            }

            Click(MediaLibrary.CloseButton);
            OpenItemsPage();
            Click(PageFooter.AddItemButton);
            DropDownSelect(ItemsPage.TypeDropDown, ItemTypePoi);
            Click(ItemsPage.VideoEmpty);
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowByText, validVideo))
                    && CountElements(MediaLibrary.TableRows) == 1,
                $"Only valid format video file should be available in Media library: {validVideo}");

            Click(PageFooter.CancelButton);
            ClickUntilShown(ItemsPage.PictureSectionAddButton, ItemsPage.PicturesSectionImageEmpty);
            Click(ItemsPage.PicturesSectionImageEmpty);
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowByText, validJpg))
                          && CountElements(MediaLibrary.TableRows) == 1,
                $"Only valid format and aspect ratio JPG file should be available in Media library: {validJpg}");

            Click(PageFooter.CancelButton);
            Click(PageFooter.CancelButton);
            OpenPlacesPage();
            Click(PageFooter.AddPlaceButton);
            Click(PlacesPage.PlaceMapImageEmpty);
            Assert.IsTrue(IsElementFound(string.Format(MediaLibrary.TableRowByText, validJpg))
                          && IsElementFound(string.Format(MediaLibrary.TableRowByText, invalidJpg))
                          && IsElementFound(string.Format(MediaLibrary.TableRowByText, validPng))
                          && IsElementFound(string.Format(MediaLibrary.TableRowByText, validSvg))
                          && IsElementNotFound(string.Format(MediaLibrary.TableRowByText, invalidRatio))
                          && CountElements(MediaLibrary.TableRows) == 4,
                "Only valid format and aspect ratio files should be available in Media library: " +
                $"{validJpg}, {invalidJpg}, {validPng}, {validSvg}");

            Click(PageFooter.CancelButton);
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
        }
    }
}
