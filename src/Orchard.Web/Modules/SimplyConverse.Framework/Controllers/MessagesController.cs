using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SimplyConverse.Framework.Activities;
using SimplyConverse.Framework.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.Workflows.Services;
using Task = System.Threading.Tasks.Task;

namespace SimplyConverse.Framework.Controllers
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.PostAsync("You said: " + message.Text);
            context.Wait(MessageReceivedAsync);
        }
    }

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public string Owner { get; set; }
        public Localizer T { get; set; }

        private readonly IOrchardServices _orchardServices;
        private readonly IWorkflowManager _workflowManager;
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly IMembershipService _membershipService;

        public MessagesController(IWorkflowManager workflowManager
            , IOrchardServices orchardServices
            , IContentManager contentManager
            , ISiteService siteService
            , IMembershipService membershipService) {

            _workflowManager = workflowManager;
            _membershipService = membershipService;
            _contentManager = contentManager;
            _orchardServices = orchardServices;
            _siteService = siteService;
        }
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                try {

                    if (String.IsNullOrEmpty(Owner))
                    {
                        Owner = _siteService.GetSiteSettings().SuperUser;
                    }
                    var owner = _membershipService.GetUser(Owner);

                    //await Conversation.SendAsync(activity, () => new EchoDialog());
                    var msg = _contentManager.New("BotActivity");
                    msg.As<ICommonPart>().Owner = owner;
                    var part = msg.As<ActivityPart>();

                    // I don't know why it is null
                    if (part == null)
                    {
                        part = new ActivityPart();
                        part.Record = new ActivityRecord();;
                        msg.Weld(part);
                    }

                    part.Action = activity.Action;
                    part.ActivityId = activity.Id;
                    part.AttachmentLayout = activity.AttachmentLayout;
                    //part.ChannelData = activity.ChannelData;
                 //   part.Attachments = activity.Attachments;
                    part.ChannelId = activity.ChannelId;
                    part.ServiceUrl = activity.ServiceUrl;
                    part.Conversation = new Models.ConversationAccount() {Name = activity.Conversation.Name, Id = activity.Conversation.Id, IsGroup = activity.Conversation.IsGroup != null && activity.Conversation.IsGroup.Value};
                  //  part.Entities = activity.Entities;
                    part.From = new Models.ChannelAccount() {Name = activity.From.Name, Id = activity.From.Id};
                    part.HistoryDisclosed = activity.HistoryDisclosed;
                    part.Locale = activity.Locale;
                  //  part.MembersAdded = activity.MembersAdded;
                  //  part.MembersRemoved = activity.MembersRemoved;
                    part.Recipient = new Models.ChannelAccount() { Name = activity.Recipient.Name, Id = activity.Recipient.Id };
                    part.Type = activity.Type;
                    part.TopicName = activity.TopicName;
                    part.Timestamp = activity.Timestamp;
                    part.TextFormat = activity.TextFormat;
                    part.Text = activity.Text;
                    part.ReplyToId = activity.ReplyToId;

                    _contentManager.Create(msg);

                    _workflowManager.TriggerEvent(ActivityReceivedActivity.ActivityName, msg, () => new Dictionary<string, object> { { "Content", msg } });


                }
                catch (Exception exception) {
                    Console.WriteLine(exception.Message);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}