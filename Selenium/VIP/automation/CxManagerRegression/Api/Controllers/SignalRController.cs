using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Api.Managers;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using Microsoft.AspNet.SignalR.Client;
using Models.Apps;
using Models.BackgroundTasks;
using Newtonsoft.Json;
using TaskStatus = Common.Enums.TaskStatus;

namespace Api.Controllers
{
    /// <summary>
    /// Class which implements ASP.NET SignalR client to interact with background tasks
    /// </summary>
    public static class SignalRController
    {
        /// <summary>
        /// Asynchronously creates and starts ASP.NET SignalR client with necessary subscriptions
        /// </summary>
        public static async Task StartClientAsync()
        {
            HubConnection hubConnection;
            IHubProxy hubProxy;
            const string proxyName = "vipbhub";
            try
            {
                hubConnection = new HubConnection(TestConfig.BaseUrlApi);
                hubProxy = hubConnection.CreateHubProxy(proxyName);
            }
            catch (Exception ex)
            {
                throw new AggregateException(
                    $"SignalR server {TestConfig.BaseUrlApi}signalr, proxy '{proxyName}' connection is " +
                    $"timed out.\n{ex.Message}");
            }
            hubConnection.Reconnected += () => Trace.TraceWarning("SignalR client: Connection restored");
            hubConnection.StateChanged += async change =>
            {
                Trace.TraceWarning($"SignalR client: state changed {change.OldState} => {change.NewState}");
                if (change.NewState != ConnectionState.Connected)
                {
                    return;
                }
                await hubProxy.Invoke("JoinGroup", TestConfig.AdminUser.Sid, ActionManager.CurrentTenantCode)
                    .ConfigureAwait(false);
                await hubProxy.Invoke("JoinUserGroup", TestConfig.AdminUser.Id).ConfigureAwait(false);
            };
            hubConnection.Error += exception =>
            {
                Trace.TraceError($"SignalR client: Error: {exception.Message}\n{exception.StackTrace}");
            };
            hubConnection.Closed += async () =>
            {
                Trace.TraceWarning("SignalR client: Connection closed");
                await Task.Delay(1000).ContinueWith(async task =>
                {
                    Trace.TraceWarning("SignalR client: Reconnection starts");
                    await hubConnection.Start().ConfigureAwait(false);
                }).ConfigureAwait(false);
            };
            //_hubConnection.TraceLevel = TraceLevels.All;
            //_hubConnection.TraceWriter = new TraceLogWriter();

            // register to catch event "TaskProgressChanged"
            hubProxy.On<string, string>(nameof(TaskProgressChanged), TaskProgressChanged);
            await hubConnection.Start().ConfigureAwait(false);
    }

        /// <summary>
        /// Method to be automatically called by SignalR event with the same name. Processes own
        /// parameters objects and (depending on task status) sets specific variables up in order
        /// to manage background tasks.
        /// </summary>
        /// <param name="task"><see cref="BackgroundTask"/> object as json</param>
        /// <param name="result">Imported/exported object as json or server error message</param>
        private static void TaskProgressChanged(string task, string result)
        {
            var task1 = JsonConvert.DeserializeObject<BackgroundTask>(task);
            if (BackgroundTaskApi.ProcessedObjects == null
                || !BackgroundTaskApi.ProcessedObjects.ContainsKey(task1.TaskId))
            {
                return;
            }

            Trace.TraceInformation(
                $@"SignalR client: Task {task1.TaskId}, file '{task1.FileName}' progress {task1.Progress}%");

            if (task1.Status < (int) TaskStatus.Completed)
            {
                return;
            }

            if (task1.Status == (int) TaskStatus.Failed || task1.Status == (int) TaskStatus.Canceled)
            {
                BackgroundTaskApi.AddOrUpdateTask(task1, null, result);
                return;
            }

            // if we're here, import/export has completed without errors => process the result               
            object taskObject = null;
            switch ((TaskActionType) task1.ActionType)
            {
                case TaskActionType.UploadApp:
                    taskObject = JsonConvert.DeserializeObject<AppWrapper>(result).Data;
                    break;
                case TaskActionType.ExportTenant:
                    taskObject = JsonConvert.DeserializeObject<BackgroundTaskWrapper>(result).Data;
                    break;
                case TaskActionType.ImportTenant:
                    // taskObject is not used (null) for this task type
                    break;
                default:
                    throw new AggregateException(
                        $@"Wrong or unknown background task type '{task1.ActionType}' detected");
            }

            BackgroundTaskApi.AddOrUpdateTask(task1, taskObject, null); 
        }

        // <summary>
        // (DRAFT) Asynchronously creates and starts ASP.NET Core SignalR client with necessary
        // subscriptions. ASP.NET Core SignalR server is not implemented yet.
        // </summary>
        //public static async Task StartCoreClientAsync()
        //{
        //    var user = RestController.Users.Single(x => x.Email == TestConfig.AdminUser.Email);
        //    var hubConnection = new HubConnectionBuilder()
        //        .WithUrl(TestConfig.SiteDomainApi + "signalr")
        //        .Build();
        //    hubConnection.Closed += exception =>
        //    {
        //        Trace.TraceError($"SignalR client: Error: {exception.Message}\n{exception.StackTrace}");
        //        return Task.Delay(1000).ContinueWith(task => hubConnection.StartAsync());
        //    };
        //    await hubConnection
        //        .InvokeCoreAsync("JoinGroup", new [] { user.Sid, ActionManager.CurrentTenantCode });
        //    await hubConnection
        //        .InvokeCoreAsync("JoinUserGroup", new [] {user.Id.ToString()})
        //        .ContinueWith(async task =>
        //        {
        //            hubConnection.On<string, string, string>(nameof(TaskProgressChanged), TaskProgressChanged);
        //            await hubConnection.StartAsync();
        //        });
        //}
    }

    //internal class TraceLogWriter : TextWriter
    //{
    //    public override Encoding Encoding => Encoding.UTF8;

    //    public override void WriteLine(string value)
    //    {
    //        Trace.TraceWarning($"SignalR.Log: {value}");
    //    }
    //}
}
