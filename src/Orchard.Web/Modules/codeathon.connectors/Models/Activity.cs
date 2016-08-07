using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace codeathon.connectors.Models {
    public  class ActivityRecord: ContentPartRecord
    {
        /// <summary>
        /// The type of the activity
        /// [message|contactRelationUpdate|converationUpdate|typing]
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Id for the activity
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Time when message was sent
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Service endpoint
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// ChannelId the activity was on
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// Sender address
        /// </summary>
        public ChannelAccount From { get; set; }

        /// <summary>
        /// Conversation
        /// </summary>
        public ConversationAccount Conversation { get; set; }

        /// <summary>
        /// (Outbound to bot only) Bot's address that received the message
        /// </summary>
        public ChannelAccount Recipient { get; set; }

        /// <summary>
        /// Format of text fields [plain|markdown] Default:markdown
        /// </summary>
        public string TextFormat { get; set; }

        /// <summary>
        /// AttachmentLayout - hint for how to deal with multiple attachments
        /// Values: [list|carousel] Default:list
        /// </summary>
        public string AttachmentLayout { get; set; }

        /// <summary>
        /// Array of address added
        /// </summary>
        public IList<ChannelAccount> MembersAdded { get; set; }

        /// <summary>
        /// Array of addresses removed
        /// </summary>
        public IList<ChannelAccount> MembersRemoved { get; set; }

        /// <summary>
        /// Conversations new topic name
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// the previous history of the channel was disclosed
        /// </summary>
        public bool? HistoryDisclosed { get; set; }

        /// <summary>
        /// The language code of the Text field
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Content for the message
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Text to display if you can't render cards
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Attachments
        /// </summary>
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Entities
        /// Collection of Entity which contain metadata about this activity
        /// (each is typed)
        /// </summary>
        public IList<Entity> Entities { get; set; }

        /// <summary>
        /// Channel specific payload
        /// </summary>
        public object ChannelData { get; set; }

        /// <summary>
        /// ContactAdded/Removed action
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// the original id this message is a response to
        /// </summary>
        public string ReplyToId { get; set; }

    }


    public class ActivityPart : ContentPart<ActivityRecord>
    {
        /// <summary>
        /// The type of the activity
        /// [message|contactRelationUpdate|converationUpdate|typing]
        /// </summary>
        public string Type {  get{return Retrieve(r => r.Type);} set{Store(r => r.Type, value);} }

        /// <summary>
        /// Id for the activity
        /// </summary>
        public string Id {  get{return Retrieve(r => r.Id);} set{Store(r => r.Id, value);} }

        /// <summary>
        /// Time when message was sent
        /// </summary>
        public DateTime? Timestamp {  get{return Retrieve(r => r.Timestamp);} set{Store(r => r.Timestamp, value);} }

        /// <summary>
        /// Service endpoint
        /// </summary>
        public string ServiceUrl {  get{return Retrieve(r => r.ServiceUrl);} set{Store(r => r.ServiceUrl, value);} }

        /// <summary>
        /// ChannelId the activity was on
        /// </summary>
        public string ChannelId {  get{return Retrieve(r => r.ChannelId);} set{Store(r => r.ChannelId, value);} }

        /// <summary>
        /// Sender address
        /// </summary>
        public ChannelAccount From {  get{return Retrieve(r => r.From);} set{Store(r => r.From, value);} }

        /// <summary>
        /// Conversation
        /// </summary>
        public ConversationAccount Conversation {  get{return Retrieve(r => r.Conversation);} set{Store(r => r.Conversation, value);} }

        /// <summary>
        /// (Outbound to bot only) Bot's address that received the message
        /// </summary>
        public ChannelAccount Recipient {  get{return Retrieve(r => r.Recipient);} set{Store(r => r.Recipient, value);} }

        /// <summary>
        /// Format of text fields [plain|markdown] Default:markdown
        /// </summary>
        public string TextFormat {  get{return Retrieve(r => r.TextFormat);} set{Store(r => r.TextFormat, value);} }

        /// <summary>
        /// AttachmentLayout - hint for how to deal with multiple attachments
        /// Values: [list|carousel] Default:list
        /// </summary>
        public string AttachmentLayout {  get{return Retrieve(r => r.AttachmentLayout);} set{Store(r => r.AttachmentLayout, value);} }

        /// <summary>
        /// Array of address added
        /// </summary>
        public IList<ChannelAccount> MembersAdded {  get{return Retrieve(r => r.MembersAdded);} set{Store(r => r.MembersAdded, value);} }

        /// <summary>
        /// Array of addresses removed
        /// </summary>
        public IList<ChannelAccount> MembersRemoved {  get{return Retrieve(r => r.MembersRemoved);} set{Store(r => r.MembersRemoved, value);} }

        /// <summary>
        /// Conversations new topic name
        /// </summary>
        public string TopicName {  get{return Retrieve(r => r.TopicName);} set{Store(r => r.TopicName, value);} }

        /// <summary>
        /// the previous history of the channel was disclosed
        /// </summary>
        public bool? HistoryDisclosed {  get{return Retrieve(r => r.HistoryDisclosed);} set{Store(r => r.HistoryDisclosed, value);} }

        /// <summary>
        /// The language code of the Text field
        /// </summary>
        public string Locale {  get{return Retrieve(r => r.Locale);} set{Store(r => r.Locale, value);} }

        /// <summary>
        /// Content for the message
        /// </summary>
        public string Text {  get{return Retrieve(r => r.Text);} set{Store(r => r.Text, value);} }

        /// <summary>
        /// Text to display if you can't render cards
        /// </summary>
        public string Summary {  get{return Retrieve(r => r.Summary);} set{Store(r => r.Summary, value);} }

        /// <summary>
        /// Attachments
        /// </summary>
        public IList<Attachment> Attachments {  get{return Retrieve(r => r.Attachments);} set{Store(r => r.Attachments, value);} }

        /// <summary>
        /// Entities
        /// Collection of Entity which contain metadata about this activity
        /// (each is typed)
        /// </summary>
        public IList<Entity> Entities {  get{return Retrieve(r => r.Entities);} set{Store(r => r.Entities, value);} }

        /// <summary>
        /// Channel specific payload
        /// </summary>
        public object ChannelData {  get{return Retrieve(r => r.ChannelData);} set{Store(r => r.ChannelData, value);} }

        /// <summary>
        /// ContactAdded/Removed action
        /// </summary>
        public string Action {  get{return Retrieve(r => r.Action);} set{Store(r => r.Action, value);} }

        /// <summary>
        /// the original id this message is a response to
        /// </summary>
        public string ReplyToId {  get{return Retrieve(r => r.ReplyToId);} set{Store(r => r.ReplyToId, value);} }

    }
}