using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimplyConverse.Framework.Models;
using SimplyConverse.Framework.Services;
using Microsoft.Bot.Connector;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace SimplyConverse.Framework.Activities
{
    public class SendBbActivityToChatDashboardActivity : Task
    {
        private readonly IBotFrameworkService ActivityService;
        public SendBbActivityToChatDashboardActivity(IBotFrameworkService BotFrameworkService)
        {
            T = NullLocalizer.Instance;
            ActivityService = BotFrameworkService;
        }

        public Localizer T { get; set; }

        public override string Name
        {
            get { return "SendBBActivityToChatDashboard"; }
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
            get { return "SendBBActivityToChatDashboard"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] { T("Done") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var textToSend = workflowContext.Tokens["originalBBActivity"];

            try {
                     ActivityService.SendBBActivityToChatDashboard(textToSend as Activity);

            }
            catch (Exception exception) {
                
            }
            
            yield return T("Done");
        }
    }
}
