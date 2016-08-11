using System;
using KwiklyBot;
using Microsoft.Bot.Connector;

namespace SimplyConverse.Framework.Services {
    public class BotFrameworkService : IBotFrameworkService
    {
        public BotFrameworkService()
        {
            //  BotFrameworkServiceCredentials tc = new BotFrameworkServiceCredentials(ConfigurationManager.AppSettings["APIKey"], ConfigurationManager.AppSettings["APISecret"], ConfigurationManager.AppSettings["AccessToken"], ConfigurationManager.AppSettings["AccessTokenSecret"]);
            //  Auth.SetUserCredentials(ConfigurationManager.AppSettings["APIKey"], ConfigurationManager.AppSettings["APISecret"], ConfigurationManager.AppSettings["AccessToken"], ConfigurationManager.AppSettings["AccessTokenSecret"]);
            //  Auth.ApplicationCredentials = tc;
        }
      
        public void ReplyWithText(IMessageActivity textToSend) {
            var connector = new ConnectorClient(new Uri(textToSend.ServiceUrl));
            
            connector.Conversations.SendToConversation((Activity)textToSend);
        }

        public void SendBBActivityToChatDashboard(Activity newMessage) {
            DashboardHelper.SendMessageToDashboard(newMessage);
        }
    }
}