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
using MessageListener.WMQClient;
using MessageListener.MeetingObserver.WMQObserver;
using MessageListener;
using IBM.XMS;

namespace codeathon.connectors
{
    public class MQService : IMQService
    {
        private IMQFactory factory;
        private IConnection conection;
        private IMQFactory destinationFactory;
        private IConnection destinationConnection;

        public MQService()
        {
            //MQ initalization
        }

        public void Connect()
        {
            using (new Impersonator("ivmapp", "Inda215111", "Password@1"))
            {
                MessageListener.WMQClient.QueueConfiguration config = new MessageListener.WMQClient.QueueConfiguration("ent-hubdev1_svc.uk.fid-intl.com", 54371, "CH01.CLIENT.ENTH2D1", "ENTH2D1");
                factory = new WMQFactory(config);
                conection = factory.CreateConnection();
                WMQMessageObserver obj = new WMQMessageObserver("IM.GLXY.QUICKINS.BACKOUT", conection, "test");
                obj.OnMeetingTriggered += obj_OnMeetingTriggered;
                obj.StartObserving();

                MessageListener.WMQClient.QueueConfiguration desconfig = new MessageListener.WMQClient.QueueConfiguration("ent-hubdev1_svc.uk.fid-intl.com", 54371, "CH01.CLIENT.ENTH2D1", "ENTH2D1");
                destinationFactory = new WMQFactory(desconfig);
                destinationConnection = destinationFactory.CreateConnection();
            }
        }

        private void obj_OnMeetingTriggered(object sender, MesssageReceiveddEventArgs e)
        {
            FilSmsRequest smsrequest = DeserializeMeetingEvent(e.Message);
            SMSService sms = new SMSService();
            sms.SendSMS()
        }

        private static FilSmsRequest DeserializeMeetingEvent(string xmlDeserilizedMeetingEvent)
        {
            FilSmsRequest meetingEvent;
            XmlSerializer mySerializer = new XmlSerializer(typeof(FilSmsRequest));
            StringReader sr1 = new StringReader(xmlDeserilizedMeetingEvent);
            meetingEvent = (FilSmsRequest)mySerializer.Deserialize(sr1);
            return meetingEvent;
        }
    }

    public interface IMQService : IDependency
    {
        void Connect();
    }
}
