using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Resources;
using Common.Enums;
using Models.BackgroundTasks;
using Models.Users;
using Newtonsoft.Json;
using RestSharp;
using TaskStatus = Common.Enums.TaskStatus;

namespace Api.Managers
{
    /// <summary>
    /// Requests to CX Manager API, endpoint group BackgroundTask
    /// </summary>
    public static class BackgroundTaskApi
    {
        /// <summary>
        /// Dictionary that contains info about all actual background tasks initiated
        /// by tests. Key equals to task id, value contains resulting task object,
        /// error message (if available), and "task is in progress" event notifier.
        /// </summary>
        internal static readonly 
            ConcurrentDictionary<string, (object, string, ManualResetEvent)> ProcessedObjects =
                new ConcurrentDictionary<string, (object, string, ManualResetEvent)>();       

        /// <summary>
        /// Deletes all existing background tasks for specified user
        /// </summary>
        /// <param name="user">User object</param>
        public static Task DeleteAllTasksAsync(User user)
        {
            return Task.Run(() => 
                RestController.HttpRequestJson(UriCxm.BackgroundTaskAll, Method.DELETE, user: user));
        }

        /// <summary>
        /// Deletes background task by ID
        /// </summary>
        /// <param name="id">Background task ID</param>
        public static void DeleteTaskId(string id)
        {
            RestController.HttpRequestJson(string.Format(UriCxm.BackgroundTaskId, id), Method.DELETE);
        }

        /// <summary>
        /// Gets package SAS URI
        /// </summary>
        /// <returns>(<see cref="string"/>) Package SAS URI</returns>
        internal static string GetPackageSas()
        {
            var response = RestController.HttpRequestJson(UriCxm.AppsGetPackageSas, Method.GET);
            return JsonConvert.DeserializeObject<string>(response.Content);
        }

        /// <summary>
        /// Creates a new background task upload and import  
        /// </summary>
        /// <param name="actionType">Action type (see <see cref="TaskActionType"/>)</param>
        /// <param name="fileName">Package file name</param>
        /// <param name="fileSize">Package size in bytes</param>
        /// <param name="sas">Azure SAS URI</param>
        /// <returns>(<see cref="BackgroundTask"/>) Background task object</returns>
        internal static BackgroundTask NewBackgroundTask(
            TaskActionType actionType, string fileName, long fileSize, string sas)
        {
            var body = new BackgroundTask
            {
                FileName = fileName,
                FileSas = sas,
                FileSize = fileSize,
                Status = (int) TaskStatus.New,
                ActionType = (int) actionType
            };

            var resp = RestController.HttpRequestJson(UriCxm.BackgroundTask,
                Method.POST,
                body);
            return JsonConvert.DeserializeObject<BackgroundTask>(resp.Content);
        }

        /// <summary>
        /// Adds new background task or updates existing task in processed objects
        /// dictionary
        /// </summary>
        /// <param name="task">Background task object</param>
        /// <param name="taskObject">Task's object</param>
        /// <param name="errorMessage">Error message text (if any)</param>
        internal static void AddOrUpdateTask(BackgroundTask task, object taskObject, string errorMessage)
        {
            if (ProcessedObjects == null || !ProcessedObjects.ContainsKey(task.TaskId))
            {
                return;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new AggregateException(
                    $"BackgroundTask API: Task {task.TaskId}. Error message: {errorMessage}");
            }

            using (var resetEvent = ProcessedObjects[task.TaskId].Item3)
            {
                ProcessedObjects.AddOrUpdate(
                    task.TaskId, 
                    (taskObject, errorMessage, null), 
                    (k, v) => (taskObject, errorMessage, null));
                resetEvent?.Set();
            }
        }
    }
}
