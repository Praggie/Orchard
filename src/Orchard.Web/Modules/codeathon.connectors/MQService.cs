using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Orchard;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Parameters;
using MessageListener.WMQClient;
using MessageListener.MeetingObserver.WMQObserver;
using MessageListener;
using IBM.XMS;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using codeathon.connectors.Models;
using Tweetinvi.Core.Interfaces;


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

            FilSmsRequest smsrequest = DeserializeMeetingEvent(e.Message.Text);
            if (smsrequest != null)
            {
                ShortMessagePart shortMessagePart = this.ConvertToShortMessagepart(smsrequest);
                //RaiseWorkFlow
            }
        }

        private static FilSmsRequest DeserializeMeetingEvent(string xmlDeserilizedMeetingEvent)
        {
            FilSmsRequest meetingEvent;
            XmlSerializer mySerializer = new XmlSerializer(typeof(FilSmsRequest));
            StringReader sr1 = new StringReader(xmlDeserilizedMeetingEvent);
            meetingEvent = (FilSmsRequest)mySerializer.Deserialize(sr1);
            return meetingEvent;
        }

        private ShortMessagePart ConvertToShortMessagepart(FilSmsRequest filSmsRequest)
        {
            ShortMessagePart part = new ShortMessagePart();
            if (filSmsRequest.body != null)
            {
                if (filSmsRequest.body.priority != null)
                {
                    part.MessagePriority = filSmsRequest.body.priority.ToString();
                }

                if (filSmsRequest.body.content != null)
                {
                    part.MessagePriority = filSmsRequest.body.content.Value;
                }

                if (filSmsRequest.body.notificationTarget != null && filSmsRequest.body.notificationTarget.queue != null)
                {
                    part.TargetQueue = filSmsRequest.body.notificationTarget.queue;
                }

                if (filSmsRequest.body.deliveryChannels != null && filSmsRequest.body.deliveryChannels.Count() > 0)
                {
                    var channel = filSmsRequest.body.deliveryChannels.SingleOrDefault(o => string.Compare(o.to, "1", true) == 0);
                    if (channel != null)
                    {
                        part.SMSMessageSendTo = channel.to;
                    }

                    channel = filSmsRequest.body.deliveryChannels.SingleOrDefault(o => string.Compare(o.to, "2", true) == 0);
                    if (channel != null)
                    {
                        part.EmailMessageSendTo = channel.to;
                    }

                    channel = filSmsRequest.body.deliveryChannels.SingleOrDefault(o => string.Compare(o.to, "3", true) == 0);
                    if (channel != null)
                    {
                        part.TwitterMessageSendTo = channel.to;
                    }
                }

                if (filSmsRequest.header != null)
                {
                    part.MessageId = filSmsRequest.header.messageId;
                }
            }
            
            return part;
        }

    }

    public interface IMQService : IDependency
    {
        void Connect();
    }
}
