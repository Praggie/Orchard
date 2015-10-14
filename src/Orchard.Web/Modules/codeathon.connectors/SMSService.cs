using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Parameters;
using FidelitySMS;
using TwilioSMSGateway;

namespace codeathon.connectors
{
    public class SMSService : ISMSService
        
    {
        public SMSService()
        {
           
        }
        public void SendSMS(string smsUserId, string textToSend)
        {
            ISMSGateway twilioSmsGateway = new TwilioProxy();
            var message = twilioSmsGateway.SendMessage(smsUserId, textToSend);
        }


    }

    public interface ISMSService: IDependency
    {
        void SendSMS(string smsUserId, string textToSend);
    }
}
