using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace codeathon.connectors.Models
{
    public class ShortMessageRecord : ContentPartRecord
    {
        public string MessageId { get; set; }
        public string Message { get; set; }
        public string MessagePriority { get; set; }
        public bool NotificationRequired { get; set; }
        public string EmailMessageSendTo { get; set; }
        public string SMSMessageSendTo { get; set; }
        public string TwitterMessageSendTo { get; set; }        
        public string TargetQueue { get; set; }
    }

    public class ShortMessagePart : ContentPart<ShortMessageRecord>
    {
        public string MessageId { get { return Retrieve(r => r.MessageId); } set { Store(r => r.MessageId, value); } }
        public string Message { get { return Retrieve(r => r.Message); } set { Store(r => r.Message, value); } }
        public string MessagePriority { get { return Retrieve(r => r.MessagePriority); } set { Store(r => r.MessagePriority, value); } }
        public bool NotificationRequired { get { return Retrieve(r => r.NotificationRequired); } set { Store(r => r.NotificationRequired, value); } }
        public string EmailMessageSendTo { get { return Retrieve(r => r.EmailMessageSendTo); } set { Store(r => r.EmailMessageSendTo, value); } }
        public string SMSMessageSendTo { get { return Retrieve(r => r.SMSMessageSendTo); } set { Store(r => r.SMSMessageSendTo, value); } }
        public string TwitterMessageSendTo { get { return Retrieve(r => r.TwitterMessageSendTo); } set { Store(r => r.TwitterMessageSendTo, value); } }
        public string TargetQueue { get { return Retrieve(r => r.TargetQueue); } set { Store(r => r.TargetQueue, value); } }
    }
}