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
using Models.Places;
using Models.Places.Devices;
using Models.Places.Items;
using Models.Places.JsonData;
using Models.Places.Modules;
using Models.Places.ParametersInstances;
using Models.Places.Schedules;
using Models.UserDirectory;
using Newtonsoft.Json;
using RestSharp;
using Place = Models.Places.Place;

namespace Api.Managers
{
    /// <summary>
    /// Requests to CX Manager API, endpoint group Places
    /// </summary>
    public static class PlaceApi
    {
        /// <summary>
        /// Deletes all places (or all of specified type) and their children within all tenants or
        /// specified one
        /// </summary>
        /// <param name="type">Place type (optional)</param>
        /// <param name="tenantTitle">Tenant title where places should be deleted (optional)</param>
        public static void DeletePlaces(PlaceType type = PlaceType.Any, 
            TenantTitle tenantTitle = TenantTitle.All)
        {
            var tenantList = tenantTitle == TenantTitle.All ?
                ActionManager.Tenants.ToArray() :
                ActionManager.Tenants
                    .Where(x => x.Title == tenantTitle.ToString())
                    .ToArray();

            foreach (var tenant in tenantList)
            {
                var response = RestController.HttpRequestJson(
                    UriCxm.Places, Method.GET, tenantCode: tenant.Code, user: TestConfig.AdminUser);
                var places = JsonConvert.DeserializeObject<List<Place>>(response.Content)
                    .AsParallel()
                    .Where(x => x.Status != (int) PlaceStatus.Deleted && x.ParentId == null)
                    .ToArray();

                foreach (var place in places)
                {
                    place.PlaceModules = new List<PlaceModule>();
                    if (type == PlaceType.Any)
                    {
                        RestController.HttpRequestJson(
                            string.Format(UriCxm.PlacesDelete, place.Id, "true"),
                            Method.DELETE,
                            tenantCode: tenant.Code,
                            user: TestConfig.AdminUser);
                    }
                    else
                    {
                        if (place.DeviceTypeId == (type != 0 ? (int?) type : null))
                        {
                            RestController.HttpRequestJson(
                                string.Format(UriCxm.PlacesDelete, place.Id, "true"),
                                Method.DELETE,
                                tenantCode: tenant.Code,
                                user: TestConfig.AdminUser);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates iBeacon device
        /// </summary>
        /// <param name="major">Major parameter (optional)</param>
        /// <param name="minor">Minor parameter (optional)</param>
        /// <param name="guid">UUID parameter (optional)</param>
        /// <returns>(<see cref="Device"/>) New device object</returns>
        public static Device CreateIbeacon(string major = null, string minor = null, string guid = null)
        {
            var body = new Device
            {
                DeviceTypeId = (int)PlaceType.Ibeacon,
                Id = 0,
                JsonData = JsonConvert.SerializeObject(
                    new JsonData1
                    {
                        UUID = string.IsNullOrEmpty(guid) ? Guid.NewGuid().ToString() : guid,
                        Major = string.IsNullOrEmpty(major) 
                            ? ushort.Parse(ActionManager.RandomNumberWord) : ushort.Parse(major),
                        Minor = string.IsNullOrEmpty(minor) 
                            ? ushort.Parse(ActionManager.RandomNumberWord) : ushort.Parse(minor)
                    })
            };

            var response = RestController.HttpRequestJson(string.Format(UriCxm.DevicesById, "0"), Method.POST, body);
            var id = JsonConvert.DeserializeObject<Device>(response.Content).Id;
            response = RestController.HttpRequestJson(
                string.Format(UriCxm.DevicesByDevicesTypeId, (int) PlaceType.Ibeacon), Method.GET);
            return JsonConvert.DeserializeObject<List<Device>>(response.Content).SingleOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Deletes device
        /// </summary>
        /// <param name="device">Device object</param>
        /// <returns>(<see cref="Device"/>) Device object or null if device parameter is not set</returns>
        public static Device DeleteIbeacon(Device device)
        {
            if (device?.Id == null)
            {
                return null;
            }
            RestController.HttpRequestJson(string.Format(UriCxm.DevicesById, device.Id), Method.DELETE);
            var response = RestController.HttpRequestJson(string.Format(UriCxm.DevicesById, device.Id), Method.GET);
            return JsonConvert.DeserializeObject<Device>(response.Content);
        }

        /// <summary>
        /// Get existing iBeacon device as object
        /// </summary>
        /// <param name="majorValue">Major parameter (optional)</param>
        /// <param name="minorValue">Minor parameter (optional)</param>
        /// <returns>(<see cref="Device"/>) Device object or null if not found</returns>
        public static Device GetIbeacon(string majorValue = null, string minorValue = null)
        {
            var response = RestController.HttpRequestJson(UriCxm.Devices, Method.GET);
            var content = JsonConvert.DeserializeObject<Device[]>(response.Content)
                .AsParallel()
                .Where(x => x.Status != (int) DeviceStatus.Deleted)
                .ToArray();
            if (content.Length == 0)
            {
                return null;
            }
            var major = short.Parse(majorValue ?? "0");
            var minor = short.Parse(minorValue ?? "0");

            if (major == 0 && minor == 0)
            {
                return content.LastOrDefault(x => x.DeviceTypeId == (int)PlaceType.Ibeacon);
            }

            if (major > 0 && minor > 0)
            {
                return content
                    .AsParallel()
                    .LastOrDefault(x => x.DeviceTypeId == (int)PlaceType.Ibeacon && 
                                   x.Data.Major == major && 
                                   x.Data.Minor == minor);
            }

            if (major > 0 && minor == 0)
            {
                return content
                    .AsParallel()
                    .LastOrDefault(x => x.DeviceTypeId == (int)PlaceType.Ibeacon && 
                                   x.Data.Major == major);
            }

            return content
                .AsParallel()
                .LastOrDefault(x => x.DeviceTypeId == (int)PlaceType.Ibeacon && 
                               x.Data.Minor == minor);
        }

        /// <summary>
        /// Get existing Windows Workstation device object
        /// </summary>
        /// <param name="wwName">Windows Workstation device name (optional)</param>
        /// <returns>(<see cref="Device"/>) Device object or null if not found</returns>
        public static Device GetWw(string wwName = null)
        {
            var response = RestController.HttpRequestJson(UriCxm.Devices, Method.GET);
            var devices = JsonConvert.DeserializeObject<Device[]>(response.Content)
                .AsParallel()
                .Where(x => x.Status != (int) DeviceStatus.Deleted)
                .ToArray();
            
            if (devices.Length == 0)
            {
                return null;
            }

            // sort to move already assigned devices to the end of query results 
            // this gives a chance to return yet not assigned device as this method result
            var orderedQuery = devices.OrderBy(x => x.AttachmentPlaceTitle);
            
            return string.IsNullOrEmpty(wwName) ?
                orderedQuery.AsParallel().FirstOrDefault(x => x.DeviceTypeId == (int) PlaceType.Ww) :
                orderedQuery.AsParallel().FirstOrDefault(x => 
                    x.DeviceTypeId == (int) PlaceType.Ww && x.Code.Contains(wwName));
        }

        /// <summary>
        /// Get existing iOS device object
        /// </summary>
        /// <param name="iosName">iOS device name (optional)</param>
        /// <returns>(<see cref="Device"/>) Device object or null if not found</returns>
        public static Device GetIosDevice(string iosName = null)
        {
            var response = RestController.HttpRequestJson(UriCxm.Devices, Method.GET);
            var devices = JsonConvert.DeserializeObject<Device[]>(response.Content)
                .AsParallel()
                .Where(x => x.Status != (int) DeviceStatus.Deleted)
                .ToArray();

            if (devices.Length == 0)
            {
                return null;
            }

            // sort to move already assigned devices to the end of query results 
            // this gives a chance to return yet not assigned device as this method result
            var orderedQuery = devices.OrderBy(x => x.AttachmentPlaceTitle);

            return string.IsNullOrEmpty(iosName) ?
                orderedQuery.AsParallel().FirstOrDefault(x => x.DeviceTypeId == (int) PlaceType.IosDevice) :
                orderedQuery.AsParallel().FirstOrDefault(x =>
                    x.DeviceTypeId == (int) PlaceType.IosDevice && x.Code.Contains(iosName));
        }

        /// <summary>
        /// Creates a new place with empty Device Type
        /// </summary>
        /// <param name="isAddChild">Whether child place should assigned to the created place (optional)</param>
        /// <returns>(<see cref="Place"/>) New parent place object</returns>
        public static Place CreateNewPlaceNoType(bool isAddChild)
        {
            var bodyObject = new Place
            {
                DeviceTypeId = null,
                Id = null,
                PlaceModules = new List<PlaceModule>
                {
                    new PlaceModule
                    {
                        ModuleId = 2, 
                        Configuration = JsonConvert.SerializeObject(
                            new Configuration
                            {
                                apps = new List<string> ()
                            })
                    }
                },
                ChildPlaces = new List<ChildPlace>(),
                DirectItems = new List<DirectItem>(),
                Title = $"Auto test {ActionManager.RandomNumber}",
                Schedule = new Schedule
                {
                    ScheduleApps = new List<ScheduleApp>()
                },
                TimeZoneId = 8,
                Radius = 5,
                Status = (int) PlaceStatus.NoDevice
            };

            var response = RestController.HttpRequestJson(string.Format(UriCxm.PlacesById, "null"), Method.POST, bodyObject);
            var parent = JsonConvert.DeserializeObject<Place>(response.Content);

            if (isAddChild)
            {
                var child = bodyObject;
                bodyObject.Title = $"Auto test {ActionManager.RandomNumber}";
                child.ParentId = parent.Id;
                RestController.HttpRequestJson(string.Format(UriCxm.PlacesById, "null"), Method.POST, child);
            }

            parent = GetById(parent.Id);

            return parent;
        }

        /// <summary>
        /// Creates a new place iBeacon with or without a child
        /// </summary>
        /// <param name="status">Desired place status</param>
        /// <param name="isAssignIbeacon">Whether iBeacon device should be assigned to the new place(s) (optional)</param>
        /// <param name="isAddChild">Whether child place should be added or not (optional)</param>
        /// <returns>(<see cref="Place"/>) New parent place object</returns>
        public static Place CreateNewPlaceIbeacon(
            PlaceStatus status,
            bool isAssignIbeacon = false, 
            bool isAddChild = false)
        {
            var bodyObject = new Place
            {
                DeviceTypeId = (int) PlaceType.Ibeacon,
                Id = null,
                PlaceModules = new List<PlaceModule>
                {
                    new PlaceModule
                    {
                        ModuleId = (int)PlaceType.Ibeacon,
                        Configuration = JsonConvert.SerializeObject(
                            new Configuration
                            {
                                apps = new List<string>()
                            })
                    }
                },
                ChildPlaces = new List<ChildPlace>(),
                DirectItems = new List<DirectItem>(),
                Title = $"Auto test {ActionManager.RandomNumber}",
                Schedule = new Schedule
                {
                    ScheduleApps = new List<ScheduleApp>()
                },
                TimeZoneId = 8,
                Radius = 5,
                Status = (int) status
            };

            if (isAssignIbeacon)
            {
                var ibeacon = GetIbeacon() ?? CreateIbeacon();
                bodyObject.Device = ibeacon;
            }

            var response = RestController.HttpRequestJson(string.Format(UriCxm.PlacesById, "null"), Method.POST, bodyObject);
            var parentPlace = JsonConvert.DeserializeObject<Place>(response.Content);

            if (isAddChild)
            {
                var childPlace = bodyObject;
                childPlace.Title = $"Auto test {ActionManager.RandomNumber}";
                childPlace.ParentId = parentPlace.Id;
                RestController.HttpRequestJson(string.Format(UriCxm.PlacesById, childPlace.Id), Method.POST, childPlace);
            }

            if (status == PlaceStatus.Deleted)
            {
                RestController.HttpRequestJson(string.Format(UriCxm.PlacesById, parentPlace.Id), Method.DELETE);
            }

            parentPlace = GetById(parentPlace.Id);

            return parentPlace;
        }

        /// <summary>
        /// Creates a new place Windows Workstation with or without a child
        /// </summary>
        /// <param name="isAssignDevice">Whether iBeacon device should be assigned to the new place(s) (optional)</param>
        /// <param name="isAddChild">Whether child place should be added or not (optional)</param>
        /// <returns>(<see cref="Place"/>) New parent place object</returns>
        public static Place CreateNewPlaceWw(bool isAssignDevice = false, bool isAddChild = false)
        {
            var bodyObject = new Place
            {
                DeviceTypeId = (int) PlaceType.Ww,
                Id = null,
                PlaceModules = new List<PlaceModule>
                {
                    new PlaceModule
                    {
                        ModuleId = (int) PlaceType.Ww,
                        Configuration = JsonConvert.SerializeObject(
                            new Configuration
                            {
                                apps = new List<string>()
                            })
                    }
                },
                ChildPlaces = new List<ChildPlace>(),
                DirectItems = new List<DirectItem>(),
                Title = $"Auto test {ActionManager.RandomNumber}",
                Schedule = new Schedule
                {
                    PlaceId = null,
                    ScheduleApps = new List<ScheduleApp>()
                },
                TimeZoneId = 8,
                Radius = 5,
                Status = (int) PlaceStatus.Active
            };

            if (isAssignDevice)
            {
                bodyObject.Device = GetWw();
            }

            var response = RestController.HttpRequestJson(string.Format(UriCxm.PlacesById, "null"), Method.POST, bodyObject);
            var parentPlace = JsonConvert.DeserializeObject<Place>(response.Content);

            if (isAddChild)
            {
                var childPlace = bodyObject;
                childPlace.Title = $"Auto test {ActionManager.RandomNumber}";
                childPlace.ParentId = parentPlace.Id;
                RestController.HttpRequestJson(string.Format(UriCxm.PlacesById, "null"), Method.POST, childPlace);
            }

            parentPlace = GetById(parentPlace.Id);

            return parentPlace;
        }

        /// <summary>
        /// Creates a new place iOS Device with or without a child
        /// </summary>
        /// <param name="isAssignDevice">Whether iOS device should be assigned to the new place(s) (optional)</param>
        /// <param name="isAddChild">Whether child place should be added or not (optional)</param>
        /// <returns>(<see cref="Place"/>) New parent place object</returns>
        public static Place CreateNewPlaceIos(bool isAssignDevice = false, bool isAddChild = false)
        {
            var bodyObject = new Place
            {
                DeviceTypeId = (int)PlaceType.IosDevice,
                Id = null,
                PlaceModules = new List<PlaceModule>
                {
                    new PlaceModule
                    {
                        ModuleId = (int) PlaceType.Ww,
                        Configuration = JsonConvert.SerializeObject(
                            new Configuration
                            {
                                apps = new List<string>()
                            })
                    }
                },
                ChildPlaces = new List<ChildPlace>(),
                DirectItems = new List<DirectItem>(),
                Title = $"Auto test {ActionManager.RandomNumber}",
                Schedule = new Schedule
                {
                    PlaceId = null,
                    ScheduleApps = new List<ScheduleApp>()
                },
                TimeZoneId = 8,
                Radius = 5,
                Status = (int) PlaceStatus.Active
            };

            if (isAssignDevice)
            {
                bodyObject.Device = GetIosDevice();
            }

            var response = RestController.HttpRequestJson(string.Format(UriCxm.PlacesById, "null"), Method.POST, bodyObject);
            var parentPlace = JsonConvert.DeserializeObject<Place>(response.Content);

            if (isAddChild)
            {
                var childPlace = bodyObject;
                childPlace.Title = $"Auto test {ActionManager.RandomNumber}";
                childPlace.ParentId = parentPlace.Id;
                RestController.HttpRequestJson(string.Format(UriCxm.PlacesById, "null"), Method.POST, childPlace);
            }

            parentPlace = GetById(parentPlace.Id);

            return parentPlace;
        }

        /// <summary>
        /// Returns place object if place with specified parameters exists
        /// </summary>
        /// <param name="type">Place device type</param>
        /// <param name="status">Place status</param>
        /// <param name="hasDeviceAssigned">Should place have iBeacon assigned or not (optional)</param>
        /// <param name="hasChildren">Is place has children (optional)</param>
        /// <param name="hasParent">Is place has parent (optional)</param>
        /// <returns>(<see cref="Place"/>) Place object or null if not found</returns>
        public static Place GetPlace(
            PlaceType type,
            PlaceStatus status,
            bool hasDeviceAssigned,
            bool? hasChildren = false,
            bool? hasParent = null)
        {
            var response = RestController.HttpRequestJson(UriCxm.Places, Method.GET);
            var places = JsonConvert.DeserializeObject<Place[]>(response.Content);
            if (status != (int) PlaceStatus.Deleted)
            {
                places = places
                    .AsParallel()
                    .Where(x => x.Status != (int) PlaceStatus.Deleted)
                    .ToArray();
            }

            if (type != PlaceType.Any && places.Length > 0)
            {
                places = places
                    .AsParallel()
                    .Where(x => x.DeviceTypeId == (type != 0 ? (int?) type : null))
                    .ToArray();
            }

            if (status != PlaceStatus.Any && places.Length > 0)
            {
                places = places
                    .AsParallel()
                    .Where(x => x.Status == (int) status)
                    .ToArray();
            }

            if (hasDeviceAssigned && places.Length > 0)
            {
                places = places
                    .AsParallel()
                    .Where(x => x.Device != null)
                    .ToArray();
            }

            if (hasChildren != null && places.Length > 0)
                if ((bool) hasChildren)
                {
                    places = 
                        (from t1 in places.AsParallel()
                        join t2 in places.AsParallel() 
                            on t1.ParentId equals t2.Id
                        select t1)
                        .ToArray();
                }

            if (hasParent != null && places.Length > 0)
                if ((bool) hasParent)
                {
                    places = places
                        .AsParallel()
                        .Where(x => x.ParentId != null)
                        .ToArray();
                }
                else
                {
                    places = places
                        .AsParallel()
                        .Where(x => x.ParentId == null)
                        .ToArray();
                }

            var result = places?.AsParallel().AsOrdered().OrderBy(x => x.Id).LastOrDefault();

            if (result != null)
            {
                result = GetById(result.Id);
            }

            return result;
        }

        /// <summary>
        /// Get root place (first created place) for the current tenant
        /// </summary>
        /// <returns>(<see cref="Place"/>) Place object</returns>
        public static Place GetRootPlace()
        {
            var response = RestController.HttpRequestJson(UriCxm.PlacesAllRoots, Method.GET);
            var places = JsonConvert.DeserializeObject<Place[]>(response.Content);
            var place = places
                .AsParallel()
                .SingleOrDefault(x => x.TenantId == (int) ActionManager.CurrentTenant);
            
            return place != null ? GetById(place.Id) : null;
        }

        /// <summary>
        /// Returns place object by ID
        /// </summary>
        /// <param name="id">Property "id" of place object</param>
        /// <param name="tenantCode">Tenant code (optional)</param>
        /// <returns>(<see cref="Place"/>) Place object</returns>
        public static Place GetById(long? id, string tenantCode = null)
        {
            var response = RestController.HttpRequestJson(
                string.Format(UriCxm.PlacesById, id), 
                Method.GET, 
                null, 
                tenantCode);
            var place = JsonConvert.DeserializeObject<Place>(response.Content);
            return place;
        }

        /// <summary>
        /// Saves place data
        /// </summary>
        /// <param name="place">Place object</param>
        /// <param name="tenantCode">Tenant code (optional)</param>
        /// <returns>(<see cref="Place"/>) Place object</returns>
        public static Place SavePlace(Place place, string tenantCode = null)
        {
            foreach (var scheduleApp in place.Schedule.ScheduleApps)
            {
                if (scheduleApp.ParametersInstance == null)
                    scheduleApp.ParametersInstance = "{}";
            }

            place.Updated = DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss");
            var resp = RestController.HttpRequestJson(
                    string.Format(UriCxm.PlacesById, place.Id), 
                    Method.POST, 
                    place,
                    tenantCode);
            place = JsonConvert.DeserializeObject<Place>(resp.Content);
            foreach (var scheduleApp in place.Schedule.ScheduleApps)
            {
                scheduleApp.ParametersInstanceJson =
                    JsonConvert.DeserializeObject<ParametersInstance>(scheduleApp.ParametersInstance);
            }

            return place;
        }

        /// <summary>
        /// Uploads image file to storage
        /// </summary>
        /// <param name="pathFile">Full path to picture file</param>
        /// <returns>(<see cref="PlaceImage"/>) Place image object</returns>
        public static PlaceImage UploadImage(string pathFile)
        {
            string hash, token;

            using (var stream = File.Open(pathFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                hash = RestController.GetMd5Hash(stream);

                var resp1 = RestController.HttpRequestJson(
                    UriCxm.PlacesGetImageSas,
                    Method.GET);
                token = JsonConvert.DeserializeObject<string>(resp1.Content);

                var blob = new CloudBlockBlob(new Uri(token));
                stream.Position = 0;
                blob.UploadFromStream(stream);
                blob.SetProperties();
            }

            var response = RestController.HttpRequestJson(
                UriCxm.PlacesCreateThumbnails,
                Method.POST, 
                new
                {
                    hash,
                    sas = token
                });
            var tnUrl = JsonConvert.DeserializeObject<string>(response.Content);
            var result = new PlaceImage
            {
                ImageName = hash,
                FullImageUrl = string.Empty,
                ShowImageUrl = tnUrl,
                ThumbnailUrl = tnUrl
            };            

            return result;
        }

        /// <summary>
        /// Deletes place and its children
        /// </summary>
        /// <param name="place">Place object</param>
        public static Place DeletePlace(Place place)
        {
            var resp = RestController.HttpRequestJson(
                string.Format(UriCxm.PlacesDelete, place.Id, "true"),
                Method.DELETE);

            return JsonConvert.DeserializeObject<Place>(resp.Content);
        }
    }
}
