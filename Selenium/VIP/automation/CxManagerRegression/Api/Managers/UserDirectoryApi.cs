using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Api.Controllers;
using Api.Helpers;
using Api.Resources;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Models.UserDirectory;
using Models.Users;
using Newtonsoft.Json;
using RestSharp;

namespace Api.Managers
{
    /// <summary>
    /// Requests to User Directory API
    /// </summary>
    public static class UserDirectoryApi
    {
        internal static readonly ConcurrentDictionary<string, string> Headers;
        internal static DateTime SidLastUpdateTime = DateTime.Now;

        static UserDirectoryApi()
        {
            Headers = new ConcurrentDictionary<string, string>
            {
                // all requests to UD API will be made from TestConfig.AdminUser user
                ["appname"] = SidsTenants.UdApplicationName,
                ["sid"] = TestConfig.AdminUser.SidUd 
            };
        }

        /// <summary>
        /// Returns application properties object
        /// </summary>
        /// <param name="applicationName">Application name to find</param>
        /// <returns>(<see cref="ApplicationData"/>) Application properties object</returns>
        private static ApplicationData GetApplicationProperties(string applicationName)
        {
            SidsTenants.GetSidUd();
            var body = new ApplicationListRequest
            {
                Name = applicationName,
                Pager = new Pager
                {
                    StartRecordIndex = 0,
                    RecordsPerPage = 2000,
                    SortBy = "Id",
                    SortDesc = false,
                    PageIndex = 0
                }
            };

            var response = RestController.HttpRequestJsonUserDirectory(UriUserDirectory.ApplicationList, Method.POST,
                Headers, body);

            var applicationList = JsonConvert.DeserializeObject<ApplicationListResponse>(response);
            var appProperties = applicationList?.Rows?.FirstOrDefault()?.Data;

            return appProperties;
        }

        /// <summary>
        /// Returns user properties 
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>(<see cref="UserData"/>) User properties object</returns>
        public static UserData GetUserData(User user)
        {
            return GetUserData(user.Email);
        }

        /// <summary>
        /// Returns user properties 
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>(<see cref="UserData"/>) User properties object</returns>
        private static UserData GetUserData(string email)
        {
            SidsTenants.GetSidUd();
            var body = new UserListRequest
            {
                Email = email,
                Pager = new Pager
                {
                    StartRecordIndex = 0,
                    RecordsPerPage = 2000,
                    SortBy = "Id",
                    SortDesc = false,
                    PageIndex = 0
                }
            };

            var response = RestController.HttpRequestJsonUserDirectory(UriUserDirectory.UserList, Method.POST,
                Headers, body);

            var userList = JsonConvert.DeserializeObject<UserListResponse>(response);
            var id = userList?.Rows?.FirstOrDefault()?.Data.Id;

            response = RestController.HttpRequestJsonUserDirectory(
                string.Format(UriUserDirectory.UserById, id), Method.GET, Headers);
            var userModel = JsonConvert.DeserializeObject<UserModel>(response);
            var userData = userModel.Model;

            return userData;
        }

        /// <summary>
        /// Creates new or updates an existing UD user. If the method parameter is null or user Id is null,
        /// a new UD user will be created. Do not use for CX Manager users creation.
        /// </summary>
        /// <param name="user">User data object</param>
        /// <returns>(<see cref="UserData"/>) User properties object</returns>
        public static UserModel SaveUser(UserData user)
        {
            const string password = "Password!1";
            if (user?.Id == null)
            {              
                user = new UserData
                {
                    GivenName = $"Auto{ActionManager.RandomNumber}",
                    FamilyName = $"Auto{ActionManager.RandomNumber}",
                    Company = "Ameria",
                    Email = $"{ActionManager.RandomNumber}@ameria.de",
                    Phone = "+380111111",
                    Password = password
                };
            }

            user.PasswordHash = RestController.GetIntermediateHash(user.Password);

            var resp = RestController.HttpRequestForm(
                TestConfig.UserDirectoryBaseUrlApi + UriUserDirectory.UserSave,
                Method.POST,
                Headers,
                user.ToDictionary());
            var userEmail = JsonConvert.DeserializeObject<UserData>(resp).Email;

            var userModel = new UserModel
            {
                Model = GetUserData(userEmail)
            };
            if (user.Id == null)
            {
                userModel.Model.Password = password;
            }

            return userModel;
        }

        /// <summary>
        /// The method does not really delete user. It changes user email to a random one and sets
        /// this user Status to Disabled. This gives the opportunity to create the same user.
        /// Endpoint Users/Save.
        /// </summary>
        /// <param name="user">User dictionary with keys "Email" and "Password"</param>
        public static void DeleteUser(User user)
        {
            SidsTenants.GetSidUd();
            var userProperties = GetUserData(user);
            if (userProperties == null)
            {
                throw new Exception($"User Directory API: user {user.Email} not found");
            }

            userProperties.Email = $"Auto{ActionManager.RandomNumber}@autotest.com";
            userProperties.Status.Key = (int)UserStatus.Disabled;

            RestController.HttpRequestJsonUserDirectory(UriUserDirectory.UserSave,
                Method.POST,
                Headers,
                userProperties);
        }

        /// <summary>
        /// The method sets user status
        /// </summary>
        /// <param name="user">User dictionary with keys "Email" and "Password"</param>
        /// <param name="newStatus">New user status</param>
        public static void SetUserStatus(User user, UserStatus newStatus)
        {
            var userProperties = GetUserData(user);
            userProperties.Status.Key = (int)newStatus;
            userProperties.Status.Value = null;

            RestController.HttpRequestForm(TestConfig.UserDirectoryBaseUrlApi + UriUserDirectory.UserSave,
                Method.POST,
                Headers,
                userProperties.ToDictionary());
        }

        /// <summary>
        /// Replaces existing user roles with specified roles in all or specified active tenant(s).
        /// </summary>
        /// <param name="user">User to assign the <paramref name="roles"/></param>
        /// <param name="roles">Roles collection to be assigned to the <paramref name="user"/>. Set to
        /// null when need to unassign all user roles.</param>
        /// <param name="tenants">Collection of tenants which has the <paramref name="roles"/> assigned
        /// to (optional). Ignore the parameter or set it to <see cref="TenantTitle.All"/> if need to
        /// apply the <paramref name="roles"/> to all active tenants.</param>
        public static void AssignRolesToUser(
            User user, 
            IEnumerable<UserRole> roles,
            params TenantTitle[] tenants)
        {
            var userProperties = GetUserData(user);
            userProperties.Roles = new List<Role>();
            var tenantList = tenants == null || tenants.Length == 0 || tenants.Contains(TenantTitle.All)
                ? Enum.GetValues(typeof(TenantTitle))
                    .Cast<TenantTitle>()
                    .Where(x => x != TenantTitle.All)
                    .ToList()
                : tenants.ToList();
            if (roles == null)
            {
                roles = new List<UserRole>();
            }

            foreach (var role in roles)
            {
                switch (role)
                {
                    case UserRole.CxmAdmin:
                        foreach (var tenant in tenantList)
                            userProperties.Roles.Add(
                                new Role
                                {
                                    Id = (int) UserRole.CxmAdmin,
                                    TenantId = (int) tenant
                                });
                        break;
                    case UserRole.ComposerAdmin:
                    case UserRole.ComposerUser:
                    case UserRole.ComposerUserDisabled:
                    case UserRole.UserDirectoryAdmin:
                        userProperties.Roles.Add(
                            new Role
                            {
                                Id = (int) role
                            });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            $@"Attempt to assign an unhandled user role: {role.ToString()}");
                }
            }

            RestController.HttpRequestForm(TestConfig.UserDirectoryBaseUrlApi + UriUserDirectory.UserSave,
                Method.POST,
                Headers,
                userProperties.ToDictionary());
        }

        /// <summary>
        /// Adds a new role to a user. Does not overwrite existing user role(s) unless <paramref name="role"/>
        /// parameter is null. Be aware of adding the same role to the same user and tenant more than one
        /// time - in this case an API exception is possible!
        /// </summary>
        /// <param name="user">User object</param>
        /// <param name="role">Role object</param>
        /// <param name="tenants">Array of tenants</param>
        public static void AddRoleToUser(User user, Role role, params TenantTitle[] tenants)
        {
            var userData = GetUserData(user);
            var tenantList = tenants.Any(x => x.ToString() == "All") ? Enum.GetValues(typeof(TenantTitle)) : tenants;
            if (role != null)
            {
                foreach (var tenant in tenantList)
                {
                    userData.Roles.Add(
                        new Role
                        {
                            Id = role.Id,
                            TenantId = (int) tenant
                        });
                }
            }
            else
            {
                userData.Roles = new List<Role>();
            }

            RestController.HttpRequestForm(TestConfig.UserDirectoryBaseUrlApi + UriUserDirectory.UserSave,
                Method.POST,
                Headers,
                userData.ToDictionary());
        }

        /// <summary>
        /// Returns collection of titles of all supported permissions for CX Manager application
        /// (except unnecessary ones)
        /// </summary>
        /// <returns>(<see cref="ICollection{String}"/>) Collection of titles</returns>
        public static string[] GetSupportedPermissions()
        {
            var appId = GetApplicationProperties(SidsTenants.CxmApplicationName).ApplicationId;
            var response = RestController.HttpRequestForm(
                string.Format(TestConfig.UserDirectoryBaseUrlApi + UriUserDirectory.GetSupportedPermissions, appId),
                Method.GET,
                Headers);
            var permissions = JsonConvert.DeserializeObject<List<Permission>>(response);
            return permissions
                .Select(x => x.Key)
                .Where(x => !x.Equals("RealFakeSmartdata") && !x.Equals("ComposerLivePreview"))
                .ToArray();
        }

        /// <summary>
        /// Returns user role data by its ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>(<see cref="Role"/>) Role object</returns>
        public static Role GetRole(long? id)
        {
            if (!id.HasValue)
            {
                return null;
            }
            var response = RestController.HttpRequestForm(
                TestConfig.UserDirectoryBaseUrlApi + string.Format(UriUserDirectory.RoleById, id),
                Method.GET,
                Headers);
            var roleObject = JsonConvert.DeserializeObject<RoleModel>(response)?.Model;
            return roleObject;
        }
        
        /// <summary>
        /// Replaces role permissions for existing role or creates a new role if <see cref="role"/>
        /// parameter is null.
        /// </summary>
        /// <param name="role">Role to be modified or created (optional)</param>
        /// <param name="permissions">Permission names array (optional). If empty or absent, the
        /// <paramref name="role"/> will have no permissions.</param>
        /// <returns>(<see cref="Role"/>) Role object</returns>
        public static Role SetRolePermissions(Role role = null, params string[] permissions)
        {
            var newName = $"Role {ActionManager.RandomNumber}";
            var newRoleSettings = new Role
            {
                Id = role == null ? 0 : role.Id,
                Application = new ApplicationKeyValue
                {
                    Key = GetApplicationProperties(SidsTenants.CxmApplicationName).ApplicationId
                },
                Name = role == null ? newName : role.Name,
                Permissions = new List<Permission>(),
                Status = new Status
                {
                    Key = 1, 
                    Value = "Active"
                }
            };

            foreach (var permission in permissions)
            {
                newRoleSettings.Permissions.Add(
                    new Permission
                    {
                        Key = permission
                    });
            }

            RestController.HttpRequestForm(
                TestConfig.UserDirectoryBaseUrlApi + UriUserDirectory.RolesSave,
                Method.POST,
                Headers,
                newRoleSettings.ToDictionary());
            
            if (role?.Id > 0)
            {
                return GetRole(role.Id);
            }

            var filter = new RoleListRequest
            {
                Pager = new Pager
                {
                    StartRecordIndex = 0,
                    RecordsPerPage = 2000,
                    SortBy = "Id",
                    SortDesc = false,
                    PageIndex = 0
                }
            };

            var response = RestController.HttpRequestForm(
                TestConfig.UserDirectoryBaseUrlApi + UriUserDirectory.RolesList,
                Method.POST,
                Headers,
                filter.ToDictionary());

            var roleId = JsonConvert.DeserializeObject<RoleListResponse>(response)
                .Rows
                .Single(x => x. Data.Name == newRoleSettings.Name 
                             && x.Data.Application.Value == SidsTenants.CxmApplicationName)
                .Data
                .Id;

            return roleId == null ? null : GetRole(roleId);
        }

        /// <summary>
        /// Returns active tenants collection
        /// </summary>
        /// <returns>(<see cref="IEnumerable{Tenant}"/>) Tenant objects collection</returns>
        public static IEnumerable<Tenant> GetTenantList()
        {
            var response = RestController.HttpRequestForm(
                TestConfig.UserDirectoryBaseUrlApi + string.Format(UriUserDirectory.TenantsList),
                Method.GET,
                Headers);
            var tenants = JsonConvert.DeserializeObject<List<Tenant>>(response)
                .Where(x => x.Status.Key > 0)
                .ToArray();

            foreach (var tenant in tenants)
            {
                // leave only main tenant code for each tenant
                if (tenant.Code.Contains(','))
                {
                    tenant.Code = tenant.Code.Substring(0, tenant.Code.IndexOf(','));
                }
            }
            return tenants;
        }
        
        /// <summary>
        /// Returns tenant data by its TenantId
        /// </summary>
        /// <param name="tenant">Tenant title</param>
        /// <returns>(<see cref="Tenant"/>) Tenant object</returns>
        public static Tenant GetTenant(TenantTitle tenant)
        {
            var response = RestController.HttpRequestForm(
                TestConfig.UserDirectoryBaseUrlApi + string.Format(UriUserDirectory.TenantLoadById, (int) tenant),
                Method.GET,
                Headers);
            var tenantObject = JsonConvert.DeserializeObject<Tenant>(response);
            return tenantObject;
        }

        /// <summary>
        /// Saves given tenant data to UD
        /// </summary>
        /// <param name="tenant"><see cref="Tenant"/> object</param>
        /// <returns>(<see cref="Tenant"/>) Updated tenant object</returns>
        public static Tenant SaveTenant(Tenant tenant)
        {
            var resp = RestController.HttpRequestForm(
                TestConfig.UserDirectoryBaseUrlApi + UriUserDirectory.TenantSave,
                Method.POST,
                Headers,
                tenant.ToDictionary());
        
            tenant = JsonConvert.DeserializeObject<Tenant>(resp);

            return tenant;
        }
    }
}
