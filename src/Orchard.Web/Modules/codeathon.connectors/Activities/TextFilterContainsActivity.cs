using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace codeathon.connectors.Activities
{
    public class TextFilterContainsActivity : Task
    {
        public TextFilterContainsActivity()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override string Name
        {
            get { return "TextFilterContains"; }
        }

        public override LocalizedString Category
        {
            get { return T("Tweet"); }
        }

        public override LocalizedString Description
        {
            get { return T("Verifies if the specified text is contained in a text block."); }
        }

        public override string Form
        {
            get { return "TextFilterContains"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] {
                T("true"),
                T("false")
            };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var textblock = activityContext.GetState<string>("TextBlock");
            var textToVerify = activityContext.GetState<string>("TextToVerify");

            if (textblock.Contains(textToVerify))
            {
                yield return T("true");
            }
            else
            {
                yield return T("false");
            }
        }
    }
}
