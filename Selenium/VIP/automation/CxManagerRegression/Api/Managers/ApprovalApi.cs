using System;
using Api.Controllers;
using Api.Resources;
using Common.Configuration;
using Common.Enums;
using Models.Approvals;
using Models.Apps;
using Models.Interfaces;
using Models.Items;
using RestSharp;

namespace Api.Managers
{
    /// <summary>
    /// Requests to CX Manager API, endpoint group Approval
    /// </summary>
    public static class ApprovalApi
    {
        /// <summary>
        /// Detects entity ID and type
        /// </summary>
        /// <typeparam name="T">Entity type (see <see cref="IApprovalEntity"/>)</typeparam>
        /// <param name="entity">Entity object</param>
        /// <returns>(<see cref="ApprovalResponse"/>) Approval data object</returns>
        private static ApprovalResponse GetIdAndType<T>(T entity)
        {
            long? id = 0;
            var type = 0;
            switch (entity)
            {
                case Item item:
                    id = item.Id;
                    type = (int) ApprovalEntities.Item;
                    break;
                case AppRequest app:
                    id = app.AppId;
                    type = (int) ApprovalEntities.Package;
                    break;
                case AppResponse app:
                    id = app.AppId;
                    type = (int) ApprovalEntities.Package;
                    break;
            }
            return new ApprovalResponse { EntityId = id, EntityType = type };
        }

        /// <summary>
        /// Approves uploaded entity (item or app)
        /// </summary>
        /// <typeparam name="T">Entity type (see <see cref="IApprovalEntity"/>)</typeparam>
        /// <param name="entity">Entity object that should be approved</param>
        public static T Approve<T>(T entity) where T : class, IApprovalEntity
        {
            var body = GetIdAndType(entity);
            body.Action = ApprovalConclusions.Approve.ToString();
            body.Comments = string.Empty;

            RestController.HttpRequestJson(UriCxm.Approve, Method.POST, body, user: TestConfig.AdminUser);
            return (T) Convert.ChangeType(
                body.EntityType == (int) ApprovalEntities.Item
                    ? ItemApi.GetById(body.EntityId)
                    : (object) AppApi.GetById((long) body.EntityId),
                typeof(T));
        }

        /// <summary>
        /// Rejects uploaded entity (item or app)
        /// </summary>
        /// <typeparam name="T">Entity type (see <see cref="IApprovalEntity"/>)</typeparam>
        /// <param name="entity">Entity object that should be rejected</param>
        /// <returns>()</returns>
        public static T Reject<T>(T entity) where T : class, IApprovalEntity
        {
            var body = GetIdAndType(entity);
            body.Action = ApprovalConclusions.Reject.ToString();
            body.Comments = string.Empty;

            RestController.HttpRequestJson(UriCxm.Approve, Method.POST, body, user: TestConfig.AdminUser);
            return (T) Convert.ChangeType(
                body.EntityType == (int) ApprovalEntities.Item
                    ? ItemApi.GetById(body.EntityId)
                    : (object) AppApi.GetById((long) body.EntityId),
                typeof(T));
        }

        /// <summary>
        /// Request approval of uploaded entity (item or app)
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity object that should be approved (see <see cref="IApprovalEntity"/>)</param>
        /// <param name="tenants">Tenant(s) where the entity is located</param>
        public static T RequestApproval<T>(T entity, params TenantTitle[] tenants) where T : class, IApprovalEntity
        {
            var temp = GetIdAndType(entity);
            var tenantIds = Array.ConvertAll(tenants, value => (int) value);
            var body = new ApprovalRequest
            {
                EntityId = temp.EntityId,
                Tenants = tenantIds,
                Comments = string.Empty,
                EntityType = temp.EntityType
            };

            RestController.HttpRequestJson(UriCxm.ApproveRequest, Method.POST, body, user: TestConfig.AdminUser);
            return (T) Convert.ChangeType(
                body.EntityType == (int) ApprovalEntities.Item
                    ? ItemApi.GetById(body.EntityId)
                    : (object) AppApi.GetById((long) body.EntityId), 
                typeof(T));
        }
    }
}
