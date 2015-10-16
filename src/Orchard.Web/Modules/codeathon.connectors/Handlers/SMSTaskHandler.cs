using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
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

                var contentManager = this.orchardServices.ContentManager;
                var smsAlreadyInDB = contentManager.Query<GatwaySMSPart, GatwaySMSPartRecord>()
                                  .OrderBy(tw => tw.SMSIndex)
                                      .ForType(new[] { "GatwaySMS" }).List().LastOrDefault();
                Task<JsonResponse> result = null;
                RestClientService restClientService = new RestClientService();
                if (smsAlreadyInDB == null || smsAlreadyInDB.SMSIndex == 0)
                {
                    result = restClientService.GetResponseAsync();
                    result.Wait();
                    var messages = result.Result.Messages;
                    RaiseWorkflow(messages);
                }
                else
                {
                    result = restClientService.GetResponseAsync(smsAlreadyInDB.SMSIndex);
                    result.Wait();
                    var messages = result.Result.Messages;
                    RaiseWorkflow(messages);
                    
                }
            }
        }

        private void RaiseWorkflow(Message[] messages)
        {
            var contentManager = this.orchardServices.ContentManager;

            foreach (var message in messages)
            {
                var smsContentItem = contentManager.New("GatwaySMS");
                var smsPart = smsContentItem.As<GatwaySMSPart>();

                // I don't know why it is null
                if (smsPart == null)
                {
                    smsPart = new GatwaySMSPart();
                    smsPart.Record = new GatwaySMSPartRecord();
                    smsContentItem.Weld(smsPart);
                }

                smsPart.SMSIndex = message.Index;
                // smsPart.SMSDateInserted = message.DateInserted;
                smsPart.SMSId = message.MessageSid;
                //smsPart.sms = message.From;
                smsPart.SMSTo = message.To;
                smsPart.SMSBody = message.Body;
                smsPart.SMSStatus = message.MessageStatus;
                smsPart.SMSError = message.ErrorCode;
                //smsPart.SMSFromCity = message.FromCity;
                //smsPart.SMSFromState = message.FromState;
                //smsPart.SMSFromZip = message.FromZip;
                //smsPart.SMSFromCountry = message.FromCountry;
                //smsPart.SMSToCity = message.ToCity;
                //smsPart.SMSToState = message.ToState;
                //smsPart.SMSToZip = message.ToZip;
                //smsPart.SMSToCountry = message.ToCountry;
                smsPart.SMSDirection = message.Direction;

                contentManager.Create(smsContentItem);
                contentManager.Publish(smsContentItem);
                
            }

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