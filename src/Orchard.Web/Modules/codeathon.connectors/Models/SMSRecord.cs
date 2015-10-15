using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace codeathon.connectors.Models
{
    public class SMSRecord : ContentPartRecord
    {
        public virtual int Index { get; set; }

        public virtual DateTime DateInserted { get; set; }

        public virtual string MessageSid { get; set; }

        public virtual string From { get; set; }

        public virtual string To { get; set; }

        public virtual string Body { get; set; }

        public virtual string MessageStatus { get; set; }

        public virtual string ErrorCode { get; set; }

        public virtual string FromCity { get; set; }

        public virtual string FromState { get; set; }

        public virtual string FromZip { get; set; }

        public virtual string FromCountry { get; set; }

        public virtual string ToCity { get; set; }

        public virtual string ToState { get; set; }

        public virtual string ToZip { get; set; }

        public virtual string ToCountry { get; set; }

        public virtual string Direction { get; set; }
    }


    public class SMSPart : ContentPart<SMSRecord>
    {

        public const string ContentItemTypeName = "SMS";

        public int Index { get { return Retrieve(r => r.Index); } set { Store(r => r.Index, value); } }
        public string MessageSid { get { return Retrieve(r => r.MessageSid); } set { Store(r => r.MessageSid, value); } }
        public DateTime DateInserted { get { return Retrieve(r => r.DateInserted); } set { Store(r => r.DateInserted, value); } }
        public string From { get { return Retrieve(r => r.From); } set { Store(r => r.From, value); } }
        public string To { get { return Retrieve(r => r.To); } set { Store(r => r.To, value); } }
        public string Body { get { return Retrieve(r => r.Body); } set { Store(r => r.Body, value); } }
        public string MessageStatus { get { return Retrieve(r => r.MessageStatus); } set { Store(r => r.MessageStatus, value); } }
        public string ErrorCode { get { return Retrieve(r => r.ErrorCode); } set { Store(r => r.ErrorCode, value); } }
        public string FromCity { get { return Retrieve(r => r.FromCity); } set { Store(r => r.FromCity, value); } }
        public string FromState { get { return Retrieve(r => r.FromState); } set { Store(r => r.FromState, value); } }
        public string FromZip { get { return Retrieve(r => r.FromZip); } set { Store(r => r.FromZip, value); } }
        public string FromCountry { get { return Retrieve(r => r.FromCountry); } set { Store(r => r.FromCountry, value); } }
        public string ToCity { get { return Retrieve(r => r.ToCity); } set { Store(r => r.ToCity, value); } }
        public string ToState { get { return Retrieve(r => r.ToState); } set { Store(r => r.ToState, value); } }
        public string ToZip { get { return Retrieve(r => r.ToZip); } set { Store(r => r.ToZip, value); } }
        public string ToCountry { get { return Retrieve(r => r.ToCountry); } set { Store(r => r.ToCountry, value); } }
        public string Direction { get { return Retrieve(r => r.Direction); } set { Store(r => r.Direction, value); } }
    }
}