using System;
using Microsoft.Bot.Connector;

namespace codeathon.connectors {
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
    }
}