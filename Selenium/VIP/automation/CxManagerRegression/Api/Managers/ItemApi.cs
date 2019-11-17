using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Api.Controllers;
using Api.Resources;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Microsoft.WindowsAzure.Storage.Blob;
using Models.Apps.Assets;
using Models.Items;
using Models.Items.LangData;
using Newtonsoft.Json;
using RestSharp;

namespace Api.Managers
{
    /// <summary>
    /// Requests to CX Manager API, endpoint group Item
    /// </summary>
    public static class ItemApi
    {
        /// <summary>
        /// Deletes all items (or all items of specified type) within all or specified tenant
        /// </summary>
        /// <param name="type">Item type to be deleted (optional)</param>
        /// <param name="tenantTitle">Tenant title (optional)</param>
        public static void DeleteItems(ItemType type = ItemType.Any, TenantTitle tenantTitle = TenantTitle.All)
        {
            var tenantList = tenantTitle == TenantTitle.All
                ? ActionManager.Tenants.ToArray()
                : ActionManager.Tenants
                    .Where(x => x.Title == tenantTitle.ToString())
                    .ToArray();

            foreach (var tenant in tenantList)
            {
                var response = RestController.HttpRequestJson(
                    UriCxm.Items, Method.GET, tenantCode: tenant.Code, user: TestConfig.AdminUser);
                var items = JsonConvert.DeserializeObject<ItemFromList[]>(response.Content);
                if (type == ItemType.Any)
                {
                    foreach (var item in items)
                    {
                        RestController.HttpRequestJson(string.Format(UriCxm.ItemById, item.Id),
                            Method.DELETE, tenantCode: tenant.Code, user: TestConfig.AdminUser);
                    }

                    continue;
                }

                items = items.AsParallel().Where(x => x.ModelId == (int) type).ToArray();
                foreach (var item in items)
                {
                    RestController.HttpRequestJson(string.Format(UriCxm.ItemById, item.Id),
                        Method.DELETE, 
                        tenantCode: tenant.Code,
                        user: TestConfig.AdminUser);
                }
            }
        }

        /// <summary>
        /// Deletes an item by its ID using API Key and App ID
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <param name="appId">App ID (see app package)</param>
        /// <param name="apiKey">API Key (see app package)</param>
        public static void DeleteItemByAppId(long? id, string appId, string apiKey)
        {
            RestController.HttpRequestJsonByAppId(
                string.Format(UriCxm.ItemById, id), Method.DELETE, null, appId, apiKey);
        }

        /// <summary>
        /// Deletes an item by its ID
        /// </summary>
        /// <param name="id">Item ID</param>
        public static void DeleteItem(long? id)
        {
            RestController.HttpRequestJson(string.Format(UriCxm.ItemById, id), Method.DELETE);
        }

        /// <summary>
        /// Follow items by specified item type
        /// </summary>
        /// <param name="type">Item type</param>
        public static void FollowItems(ItemType type)
        {
            RestController.HttpRequestJson(
                string.Format(UriCxm.ItemsFollow, (int) type), Method.POST);
        }

        /// <summary>
        /// Unfollow items by specified item type
        /// </summary>
        /// <param name="type">Item type</param>
        public static void UnfollowItems(ItemType type)
        {
            RestController.HttpRequestJson(
                string.Format(UriCxm.ItemsFollow, (int)type), Method.DELETE);
        }

        /// <summary>
        /// Sets title and some additional properties of item
        /// </summary>
        /// <param name="item">Item object</param>
        private static void SetTitle(Item item)
        {
            try
            {
                item.LangJsonData.EnJson = JsonConvert.DeserializeObject<LangData>(
                    item.LangJsonData.en 
                    ?? item.LangJsonData.de 
                    ?? item.LangJsonData.fr 
                    ?? item.LangJsonData.zh
                    ?? item.LangJsonData.ar);
                if (item.LangJsonData.EnJson.Title == null)
                {
                    item.LangJsonData.EnJson = JsonConvert.DeserializeObject<LangData>(item.JsonData);
                }

                item.JsonDataTitle = item.LangJsonData.EnJson.Title;
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Creates a new item using JSON model
        /// </summary>
        /// <param name="jsonModel">JSON formatted model</param>
        /// <param name="appId">App ID (see app package)</param>
        /// <param name="apiKey">API Key (see app package)</param>
        /// <param name="tenantCode">Tenant code (optional)</param>
        /// <returns>(<see cref="ValueTuple{Item, String}"/>) New item object or
        /// error message (if available)</returns>
        public static (Item, string) CreateNewItemFromJson(
            string jsonModel, string appId, string apiKey, string tenantCode = null)
        {
            var response = RestController.HttpRequestJsonByAppId(
                UriCxm.Items, Method.POST, jsonModel, appId, apiKey, tenantCode);
            try
            {
                var item = JsonConvert.DeserializeObject<Item>(response.Content);
                SetTitle(item);
                return (item, null);
            }
            catch
            {
                return (null, response?.Content);
            }
        }

        /// <summary>
        /// Returns item object by ID
        /// </summary>
        /// <param name="id">Property "id" of <see cref="Item"/> object</param>
        /// <returns>(<see cref="Item"/>) Item object</returns>
        public static Item GetById(long? id)
        {
            if (id == null)
            {
                return null;
            }
            var response = RestController.HttpRequestJson(string.Format(UriCxm.ItemById, id), Method.GET);
            var item = JsonConvert.DeserializeObject<Item>(response.Content);
            SetTitle(item);
            return item;
        }

        /// <summary>
        /// Imports item from JSON file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="mask">File name mask (like name*.json) or file name with extension</param>
        /// <param name="jsonParameters">Settings collection to be put into JSON (optional)</param>
        /// <returns>(<see cref="Item"/>) Item object</returns>
        public static Item ImportItem(
            string path,
            string mask,
            IList<string> jsonParameters = null)
        {
            string itemBody;
            try
            {
                itemBody = File.ReadAllText(Path.Combine(path, mask));
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"{ex.Message} Item file loading error");
            }

            if (jsonParameters != null)
            {
                for (var i = 0; i < jsonParameters.Count; i++)
                {
                    itemBody = itemBody.Replace("{" + i + "}", jsonParameters[i]);
                }
            }

            var response = RestController.HttpRequestJson(UriCxm.Items, Method.POST, itemBody);
            var item = JsonConvert.DeserializeObject<Item>(response.Content);
            SetTitle(item);
            return item;
        }

        /// <summary>
        /// Get item object if it exists
        /// </summary>
        /// <param name="type">Item type</param>
        /// <param name="status">Item status</param>
        /// <param name="hasImage">Check whether item has image assets</param>
        /// <returns>(<see cref="Item"/>) Item object or null if not found</returns>
        public static Item GetItem(
            ItemType type,
            ItemStatus status,
            bool hasImage = false)
        {
            var response = RestController.HttpRequestJson(UriCxm.Items, Method.GET);
            var items = JsonConvert.DeserializeObject<ItemFromList[]>(response.Content);
            if (items.Length > 0 && type != ItemType.Any)
            {
                items = items
                    .AsParallel()
                    .Where(x => x.ModelId == (int) type)
                    .ToArray();
            }

            if (items.Length > 0 && status != ItemStatus.Any)
            {
                items = items
                    .AsParallel()
                    .Where(x => x.Status == (int) status)
                    .ToArray();
            }

            if (items.Length > 0)
            {
                items = items
                    .AsParallel()
                    .Where(x => hasImage ? x.Picture.Contains("asset") : x.Picture.Contains("resourcefiles"))
                    .ToArray();
            }

            if (items.Length == 0)
            {
                return null;
            }

            var item = GetById(items.Last().Id);
            SetTitle(item);
            return item;
        }

        /// <summary>
        /// Searches item by expression and other parameters
        /// </summary>
        /// <param name="searchExp">Search string</param>
        /// <param name="isIncludeDeleted">Should deleted items be included or not (optional)</param>
        /// <returns>(<see cref="Item"/>) Returns first found item object if title is not unique,
        /// or returns null if not found</returns>
        public static Item SearchItem(string searchExp, bool isIncludeDeleted = false)
        {
            var response = RestController.HttpRequestJson(
                string.Format(UriCxm.ItemsSearch, searchExp, isIncludeDeleted.ToString().ToLower()), 
                Method.GET);
            var itemTemp = JsonConvert.DeserializeObject<List<ItemFromList>>(response.Content)
                .FirstOrDefault();
            if (itemTemp == null)
            {
                return null;
            }
            var item = GetById(itemTemp.Id);
            if (item == null)
            {
                return null;
            }
            SetTitle(item);
            return item;
        }

        /// <summary>
        /// Saves item data
        /// </summary>
        /// <param name="item"><see cref="Item"/> object</param>
        /// <returns>(<see cref="Item"/>) Item object</returns>
        public static Item SaveItem(Item item)
        {
            item.UpdateDate = DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss");
            var resp = RestController.HttpRequestJson(UriCxm.Items + $@"/{item.Id}", Method.POST, item);
            item = JsonConvert.DeserializeObject<Item>(resp.Content);
            if (item == null)
            {
                return null;
            }
            SetTitle(item);
            return item;
        }

        /// <summary>
        /// Uploads asset file to storage and creates thumbnails (if applicable)
        /// </summary>
        /// <param name="pathFile">Full path to asset file</param>
        /// <returns>(<see cref="string"/>) Asset URL in storage</returns>
        public static string UploadFile(string pathFile)
        {
            string hash, token;
            using (var stream = File.Open(pathFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                hash = RestController.GetMd5Hash(stream);

                var response = RestController.HttpRequestJson(
                    UriCxm.AssetGetImageSas,
                    Method.GET);
                token = JsonConvert.DeserializeObject<string>(response.Content);
                var blob = new CloudBlockBlob(new Uri(token));
                stream.Position = 0;
                blob.UploadFromStream(stream);
                blob.SetProperties();
            }

            AssetRequest body;
            switch (Path.GetExtension(pathFile).ToLower())
            {
                case ".png":
                    body = new AssetRequest
                    {
                        hash = hash,
                        title = Path.GetFileName(pathFile),
                        minWidth = 100,
                        minHeight = 100,
                        maxHeight = 1920,
                        maxWidth = 1080,
                        mime = "image/png",
                        maxAspectRatio = 2,
                        minAspectRatio = 0.8M,
                        sas = token
                    };
                    break;
                case ".jpg":
                case ".jpeg":
                    body = new AssetRequest
                    {
                        hash = hash,
                        title = Path.GetFileName(pathFile),
                        minWidth = 100,
                        minHeight = 100,
                        maxHeight = 1920,
                        maxWidth = 1080,
                        mime = "image/jpeg",
                        maxAspectRatio = 2,
                        minAspectRatio = 0.8M,
                        sas = token
                    };
                    break;
                case ".pdf":
                    body = new AssetRequest
                    {
                        hash = hash,
                        title = Path.GetFileName(pathFile),
                        mime = "application/pdf",
                        sas = token
                    };
                    break;
                default:
                    body = new AssetRequest();
                    break;
            }

            RestController.HttpRequestJson(UriCxm.AssetCreateThumbnails, Method.POST, body);
       
            return hash;
        }
    }
}
