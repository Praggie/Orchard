using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace codeathon.connectors.Activities
{
    public class SendTweetActivity : Task {
        private ITwitterService tweetService;
        public SendTweetActivity(ITwitterService twitterService)
        {
            T = NullLocalizer.Instance;
            tweetService = twitterService;
        }

        public Localizer T { get; set; }

        public override string Name
        {
            get { return "SendTweet"; }
        }

        public override LocalizedString Category
        {
            get { return T("Tweet"); }
        }

        public override LocalizedString Description
        {
            get { return T("sends a tweet to a specific user"); }
        }

        public override string Form
        {
            get { return "SendTweet"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] { T("Done") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var twitterUser = activityContext.GetState<string>("TwitterUser");
            var textToSend = activityContext.GetState<string>("TextToSend");
            var inReplyToTweet = activityContext.GetState<string>("InReplyToTweet");
            if(twitterUser != "dumptyhumpty80")
                tweetService.ReplyToTweet(inReplyToTweet, '@'+ twitterUser + ' ' +textToSend);
            yield return T("Done");

          
        }
    }
}
