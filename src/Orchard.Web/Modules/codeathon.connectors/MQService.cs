using codeathon.connectors.Activities;
using codeathon.connectors.Models;
using IBM.XMS;
using MessageListener;
using MessageListener.MeetingObserver.WMQObserver;
using MessageListener.WMQClient;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Tasks.Scheduling;
using Orchard.Workflows.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;


namespace codeathon.connectors
{
    public class MQService : IMQService
    {
        private IMQFactory factory;
        private IConnection conection;
        private IMQFactory destinationFactory;
        private IConnection destinationConnection;
        private readonly IScheduledTaskManager _taskManager;
        private readonly IOrchardServices orchardServices;
        private readonly ITransactionManager transactionManager;
        private readonly IWorkflowManager workflowManager;

        public MQService(IWorkflowManager workflowManager,
            IOrchardServices orchardServices,
            ITransactionManager transactionManager)
        {
            this.transactionManager = transactionManager;
            this.workflowManager = workflowManager;
            this.orchardServices = orchardServices;
        }

        public void Connect()
        {

            using (new Impersonator("ivmapp", "Inla214846", "Password@1"))
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
                RaiseWorkFlow(smsrequest);     
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

        private void RaiseWorkFlow(FilSmsRequest filSmsRequest)
        {
            var contentManager = this.orchardServices.ContentManager;

            var messageContentItem = contentManager.New(ShortMessagePart.ContentItemTypeName);
            var messagePart = messageContentItem.As<ShortMessagePart>();

            if (messagePart == null)
            {
                messagePart = new ShortMessagePart();
                messagePart.Record = new ShortMessageRecord();
                messageContentItem.Weld(messagePart);
            }

            if (filSmsRequest.body != null)
            {
                if (filSmsRequest.body.priority != null)
                {
                    messagePart.MessagePriority = filSmsRequest.body.priority.ToString();
                }

                if (filSmsRequest.body.content != null)
                {
                    messagePart.Message = filSmsRequest.body.content.Value;
                }

                if (filSmsRequest.body.notificationTarget != null && filSmsRequest.body.notificationTarget.queue != null)
                {
                    messagePart.TargetQueue = filSmsRequest.body.notificationTarget.queue;
                }

                if (filSmsRequest.body.deliveryChannels != null && filSmsRequest.body.deliveryChannels.Count() > 0)
                {
                    var channel = filSmsRequest.body.deliveryChannels.SingleOrDefault(o => string.Compare(o.id, "1", true) == 0);
                    if (channel != null)
                    {
                        messagePart.SMSMessageSendTo = channel.to;
                    }

                    channel = filSmsRequest.body.deliveryChannels.SingleOrDefault(o => string.Compare(o.id, "2", true) == 0);
                    if (channel != null)
                    {
                        messagePart.EmailMessageSendTo = channel.to;
                    }

                    channel = filSmsRequest.body.deliveryChannels.SingleOrDefault(o => string.Compare(o.id, "3", true) == 0);
                    if (channel != null)
                    {
                        messagePart.TwitterMessageSendTo = channel.to;
                    }
                }

                if (filSmsRequest.header != null)
                {
                    messagePart.MessageId = filSmsRequest.header.messageId;
                }
            }

            contentManager.Create(messageContentItem);
            contentManager.Publish(messageContentItem);

            workflowManager.TriggerEvent(
                ShortMessageReceivedActivity.ActivityName,
                messageContentItem,
                () => new Dictionary<string, object> { { "Content", messageContentItem } });          
        }

    }

    public interface IMQService : IDependency
    {
        void Connect();
    }
}
