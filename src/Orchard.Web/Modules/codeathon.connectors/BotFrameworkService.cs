using System;

namespace codeathon.connectors {
    public class BotFrameworkService : IBotFrameworkService
    {
        public BotFrameworkService()
        {
            //  BotFrameworkServiceCredentials tc = new BotFrameworkServiceCredentials(ConfigurationManager.AppSettings["APIKey"], ConfigurationManager.AppSettings["APISecret"], ConfigurationManager.AppSettings["AccessToken"], ConfigurationManager.AppSettings["AccessTokenSecret"]);
            //  Auth.SetUserCredentials(ConfigurationManager.AppSettings["APIKey"], ConfigurationManager.AppSettings["APISecret"], ConfigurationManager.AppSettings["AccessToken"], ConfigurationManager.AppSettings["AccessTokenSecret"]);
            //  Auth.ApplicationCredentials = tc;
        }
      
        public void ReplyWithText(string textToSend) {
            throw new NotImplementedException();
        }
    }
}