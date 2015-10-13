using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using codeathon.connectors.Activities;
using codeathon.connectors.Models;
using Orchard;
using Orchard.ContentManagement;
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
    /// <summary>
    /// Take a look at http://stackoverflow.com/questions/8916146/scheduled-tasks-using-orchard-cms
    /// and http://stackoverflow.com/questions/11475187/how-to-run-scheduled-tasks-in-orchard
    /// </summary>
    public class TweetFetchTaskHandler : IScheduledTaskHandler
    {
        private const string TaskType = "TweetFetch";
        private const int PeriodInMinutes = 1;

        private readonly IScheduledTaskManager _taskManager;
        private readonly IOrchardServices orchardServices;
        private readonly ITransactionManager transactionManager;
        private readonly IWorkflowManager workflowManager;
        public ILogger Logger { get; set; }

        public Localizer T { get; set; }

        public TweetFetchTaskHandler(
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
                TwitterCredentials tc = new TwitterCredentials(ConfigurationManager.AppSettings["APIKey"], ConfigurationManager.AppSettings["APISecret"], ConfigurationManager.AppSettings["AccessToken"], ConfigurationManager.AppSettings["AccessTokenSecret"]);
                Auth.SetUserCredentials(ConfigurationManager.AppSettings["APIKey"], ConfigurationManager.AppSettings["APISecret"], ConfigurationManager.AppSettings["AccessToken"], ConfigurationManager.AppSettings["AccessTokenSecret"]);
                Auth.ApplicationCredentials = tc;
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
                //var imapSetting = this.orchardServices.WorkContext.CurrentSite.As<IMAPSettingPart>();

                this.transactionManager.Demand();
                try {
                    TwitterCredentials creds = new TwitterCredentials(ConfigurationManager.AppSettings["APIKey"], ConfigurationManager.AppSettings["APISecret"], ConfigurationManager.AppSettings["AccessToken"], ConfigurationManager.AppSettings["AccessTokenSecret"]);

                    //var tweet1 = Auth.ExecuteOperationWithCredentials(creds, () =>
                    //{
                    //    return Tweet.PublishTweet("Hello World");
                    //});

                    //var tweets = Timeline.GetUserTimeline("dumptyhumpty80");

                    var mentionsTimelineParameters = new MentionsTimelineParameters();
                    var tweets = Timeline.GetMentionsTimeline(mentionsTimelineParameters);

                    var searchParameter = new TweetSearchParameters("#MachineLearning")
                    {
                        Lang = Language.English,
                        SearchType = SearchResultType.Popular,
                        MaximumNumberOfResults = 100,
                        Until = DateTime.Now,
                        Filters = TweetSearchFilters.Images
                    };

                 /*   var tweets = Auth.ExecuteOperationWithCredentials(creds, () =>
                    {
                        //return Timeline.GetUserTimeline("fidelity_uk");
                        return Search.SearchTweets(searchParameter);

                    });*/

                    

                    //var tweets = Search.SearchTweets(searchParameter);

                    if (tweets != null)
                    {
                       // read first 10 emails. The reason is preventing reading thousands of emails at once
                        foreach (var tweet in tweets.Take(10))
                        {
                            RaiseWorkflow(tweet);
                        }

                    }
                }
                catch (Exception e)
                {
                    this.transactionManager.Cancel();

                    this.Logger.Error(e, e.Message);

                    // We need a new transaction for storing the imapSetting
                    this.transactionManager.RequireNew();

                }
                finally
                {
                    DateTime nextTaskDate = DateTime.UtcNow.AddMinutes(PeriodInMinutes);
                    this.ScheduleNextTask(nextTaskDate);
                }
            }
        }

        private void RaiseWorkflow(ITweet tweet)
        {
            var contentManager = this.orchardServices.ContentManager;

         var tweetAlreadyInDB =   contentManager.Query<TweetPart, TweetRecord>()
                            .Where(tw => tw.TweetId == tweet.IdStr)
                                .ForType(new[] { "Tweet" }).List().Select(p=>p.ContentItem).SingleOrDefault();
            if (tweetAlreadyInDB != null)
                return;

            var emailContentItem = contentManager.New(TweetPart.ContentItemTypeName);
            var emailPart = emailContentItem.As<TweetPart>();

            // I don't know why it is null
            if (emailPart == null)
            {
                emailPart = new TweetPart();
                emailPart.Record = new TweetRecord();
                emailContentItem.Weld(emailPart);
            }

            emailPart.Text = tweet.Text;
            emailPart.CreatedAt = tweet.CreatedAt;
            emailPart.InReplyToScreenName = tweet.InReplyToScreenName;
            emailPart.InReplyToUserId = tweet.InReplyToUserId;
            emailPart.InReplyToUserIdStr = tweet.InReplyToUserIdStr;
            emailPart.IsRetweet = tweet.IsRetweet;
            emailPart.Source = tweet.Source;
            emailPart.TweetId = tweet.IdStr;
            if(!String.IsNullOrWhiteSpace(tweet.TweetDTO.CreatedBy.ScreenName))
                emailPart.CreatedBy = "@"+tweet.TweetDTO.CreatedBy.ScreenName;
            emailPart.CreatedById = tweet.TweetDTO.CreatedBy.IdStr;
            
            contentManager.Create(emailContentItem);
            contentManager.Publish(emailContentItem);

            workflowManager.TriggerEvent(
                TweetReceivedActivity.ActivityName,
                emailContentItem,
                () => new Dictionary<string, object> { { "Content", emailContentItem } });

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