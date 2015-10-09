using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace codeathon.connectors.Models
{
    public class TweetRecord : ContentPartRecord
    { 
        public virtual DateTime CreatedAt { get; set; }
        public virtual string TweetId { get; set; }
        public virtual string InReplyToScreenName { get; set; }
        public virtual long? InReplyToUserId { get; set; }
        public virtual string InReplyToUserIdStr { get; set; }
        public virtual bool IsRetweet { get; set; }

        public virtual string Source { get; set; }
        public virtual string Text { get; set; }
        public virtual int UserMentionsCount { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string CreatedById { get; set; }
    }



    public class TweetPart : ContentPart<TweetRecord> {

        public const string ContentItemTypeName = "Tweet";

        public DateTime CreatedAt { get{return Retrieve(r => r.CreatedAt);} set{Store(r => r.CreatedAt, value);} }
        [Required]
        public string TweetId { get{return Retrieve(r => r.TweetId);} set{Store(r => r.TweetId, value);} }
        public string InReplyToScreenName { get{return Retrieve(r => r.InReplyToScreenName);} set{Store(r => r.InReplyToScreenName, value);} }
        public long? InReplyToUserId { get{return Retrieve(r => r.InReplyToUserId);} set{Store(r => r.InReplyToUserId, value);} }
        public string InReplyToUserIdStr { get{return Retrieve(r => r.InReplyToUserIdStr);} set{Store(r => r.InReplyToUserIdStr, value);} }
        public bool IsRetweet { get{return Retrieve(r => r.IsRetweet);} set{Store(r => r.IsRetweet, value);} }

        public string Source { get{return Retrieve(r => r.Source);} set{Store(r => r.Source, value);} }
        [Required]
        public string Text { get{return Retrieve(r => r.Text);} set{Store(r => r.Text, value);} }
        public int UserMentionsCount { get{return Retrieve(r => r.UserMentionsCount);} set{Store(r => r.UserMentionsCount, value);} }
        public string CreatedBy { get { return Retrieve(r => r.CreatedBy); } set { Store(r => r.CreatedBy, value); } }
        public string CreatedById { get { return Retrieve(r => r.CreatedById); } set { Store(r => r.CreatedById, value); } }
    }
}
