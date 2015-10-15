using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using codeathon.connectors.Activities;
using codeathon.connectors.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Tasks.Scheduling;
using Orchard.Workflows.Services;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Parameters;
using Tweetinvi.Core.Parameters;

namespace codeathon.connectors.Handlers
{
    public class SMSTaskHandler : IScheduledTaskHandler
    {
        private const string TaskType = "SMSFetch";
        private const int PeriodInMinutes = 1;

        private readonly IScheduledTaskManager _taskManager;
        private readonly IOrchardServices orchardServices;
        private readonly ITransactionManager transactionManager;
        private readonly IWorkflowManager workflowManager;
        public ILogger Logger { get; set; }

        public Localizer T { get; set; }

        public SMSTaskHandler(
            IWorkflowManager workflowManager,
            IOrchardServices orchardServices,
            ITransactionManager transactionManager,
            IScheduledTaskManager taskManager)
        {
            this.transactionManager = transactionManager;
            this.workflowManager = workflowManager;
            this.orchardServices = orchardServices;
            _taskManager = taskManager;
            Logger = NullLogger.Instance;
            this.T = NullLocalizer.Instance;

            try
            {
                DateTime firstDate = DateTime.UtcNow.AddMinutes(PeriodInMinutes);
                ScheduleNextTask(firstDate);
            }
            catch (Exception e)
            {
                this.Logger.Error(e, e.Message);
            }
        }

        public void Process(ScheduledTaskContext context)
        {
            if (context.Task.TaskType == TaskType)
            {

                this.transactionManager.Demand();
                RestClientService restClientService = new RestClientService();
                var result = restClientService.GetResponseAsync();
                result.Wait();
                var messages = result.Result.Messages;
                //RaiseWorkflow(tweet);
            }
        }

        private void RaiseWorkflow(Message[] messages)
        {
            return;
            var contentManager = this.orchardServices.ContentManager;

            var smsContentItem = contentManager.New(SMSPart.ContentItemTypeName);
            var smsPart = smsContentItem.As<SMSPart>();

            // I don't know why it is null
            if (smsPart == null)
            {
                smsPart = new SMSPart();
                smsPart.Record = new SMSRecord();
                smsContentItem.Weld(smsPart);
            }

            contentManager.Create(smsContentItem);
            contentManager.Publish(smsContentItem);

            //workflowManager.TriggerEvent(
            //    TweetReceivedActivity.ActivityName,
            //    smsContentItem,
            //    () => new Dictionary<string, object> { { "Content", smsContentItem } });

        }

        private void ScheduleNextTask(DateTime date)
        {
            if (date > DateTime.UtcNow)
            {
                var tasks = this._taskManager.GetTasks(TaskType);
                if (tasks == null || tasks.Count() == 0)
                    this._taskManager.CreateTask(TaskType, date, null);
            }
        }
    }

}