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
    public class SendTweetActivity : Task
    {
        private readonly ITwitterService tweetService;
        private readonly IJobsQueueService _jobsQueueService;
        public SendTweetActivity(ITwitterService twitterService, IJobsQueueService jobsQueueService)
        {
            T = NullLocalizer.Instance;
            tweetService = twitterService;
            _jobsQueueService = jobsQueueService;
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
            var sendAsPM = activityContext.GetState<bool>("SendAsPM");
            var queued = activityContext.GetState<bool>("Queued");
            var priority = activityContext.GetState<string>("Priority");


            if (!queued)
            {
                if (sendAsPM)
                {

                    tweetService.SendPrivateMessage(twitterUser, textToSend);
                }
                else
                {
                    if (twitterUser != "dumptyhumpty80")
                        tweetService.ReplyToTweet(inReplyToTweet, twitterUser + ' ' + textToSend);
                    yield return T("Done");
                }
            }
            else
            {
                var parameters = new Dictionary<string, object> {
                {"TwitterUser", twitterUser},
                {"TextToSend", textToSend},
                {"InReplyToTweet", inReplyToTweet},
                    { "SendAsPM", sendAsPM}
            };
                int result;
              var passed =  int.TryParse(priority, out result);
                if(passed)
                this._jobsQueueService.Enqueue("IMessageService.Send", new { type = TweetMessageChannel.MessageType, parameters = parameters }, result);
            }


        }
    }
}
