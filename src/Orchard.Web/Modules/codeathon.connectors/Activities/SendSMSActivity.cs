using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace codeathon.connectors.Activities
{
    public class SendsmsActivity : Task {
        private ISMSService smsService;
        public SendsmsActivity(ISMSService SMSService)
        {
            T = NullLocalizer.Instance;
            smsService = SMSService;
        }

        public Localizer T { get; set; }

        public override string Name
        {
            get { return "SendSMS"; }
        }

        public override LocalizedString Category
        {
            get { return T("SMS"); }
        }

        public override LocalizedString Description
        {
            get { return T("sends a SMS to a specific user"); }
        }

        public override string Form
        {
            get { return "SendSMS"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] { T("Done") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var SMSUser = activityContext.GetState<string>("SMSUser");
            var textToSend = activityContext.GetState<string>("TextToSend");
                smsService.SendSMS(SMSUser, textToSend);
            yield return T("Done");

          
        }
    }
}
