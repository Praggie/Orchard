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
    public class SendActivityActivity : Task
    {
        private readonly IBotFrameworkService ActivityService;
        public SendActivityActivity(IBotFrameworkService BotFrameworkService)
        {
            T = NullLocalizer.Instance;
            ActivityService = BotFrameworkService;
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
            var part = workflowContext.Content.As<ActivityPart>();
            IMessageActivity newMessage = Activity.CreateMessageActivity();
            newMessage.Type = ActivityTypes.Message;
            newMessage.From = new Microsoft.Bot.Connector.ChannelAccount() {Name = part.Recipient.Name, Id = part.Recipient.Id};
            newMessage.Conversation = new Microsoft.Bot.Connector.ConversationAccount() { Name = part.Conversation.Name, Id = part.Conversation.Id, IsGroup = part.Conversation.IsGroup}; ;
            newMessage.Recipient = new Microsoft.Bot.Connector.ChannelAccount() { Name = part.From.Name, Id = part.From.Id }; ;
            newMessage.Text = textToSend;
            newMessage.ServiceUrl = part.ServiceUrl;

            ActivityService.ReplyWithText(newMessage);
            
            yield return T("Done");
        }
    }
}
