using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using codeathon.connectors.Services;
using Orchard.JobsQueue.Services;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace codeathon.connectors.Activities
{
    public class SendActivityActivity : Task
    {
        private readonly ITwitterService ActivityService;
        private readonly IJobsQueueService _jobsQueueService;
        public SendActivityActivity(ITwitterService twitterService, IJobsQueueService jobsQueueService)
        {
            T = NullLocalizer.Instance;
            ActivityService = twitterService;
            _jobsQueueService = jobsQueueService;
        }

        public Localizer T { get; set; }

        public override string Name
        {
            get { return "SendActivity"; }
        }

        public override LocalizedString Category
        {
            get { return T("Bot"); }
        }

        public override LocalizedString Description
        {
            get { return T("sends a Activity to a specific user"); }
        }

        public override string Form
        {
            get { return "SendActivity"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] { T("Done") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var textToSend = activityContext.GetState<string>("TextToSend");

            var activity =
                ActivityService.SendPrivateMessage(twitterUser, textToSend);
            
            yield return T("Done");
        }
    }
}
