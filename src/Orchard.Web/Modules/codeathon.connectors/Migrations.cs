using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using codeathon.connectors.Models;

namespace codeathon.connectors
    {
        public class Migrations : DataMigrationImpl
        {

            public int Create()
            {
                // Creating table ShortMessageRecord
                SchemaBuilder.CreateTable("ShortMessageRecord", table => table
                    .ContentPartRecord()
                    .Column("MessageId", DbType.String)
                    .Column("Message", DbType.String)
                    .Column("MessagePriority", DbType.String)
                    .Column("NotificationRequired", DbType.Boolean)
                    .Column("EmailMessageSendTo", DbType.String)
                    .Column("SMSMessageSendTo", DbType.String)
                    .Column("TwitterMessageSendTo", DbType.String)
                    .Column("TargetQueue", DbType.String)
                );

                // Creating table SMSRecord
                SchemaBuilder.CreateTable("SMSRecord", table => table
                    .ContentPartRecord()
                    .Column("Index", DbType.Int32)
                    .Column("MessageSid", DbType.String)
                    .Column("From", DbType.String)
                    .Column("To", DbType.String)
                    .Column("Body", DbType.String)
                    .Column("MessageStatus", DbType.String)
                    .Column("ErrorCode", DbType.String)
                    .Column("FromCity", DbType.String)
                    .Column("FromState", DbType.String)
                    .Column("FromZip", DbType.String)
                    .Column("FromCountry", DbType.String)
                    .Column("ToCity", DbType.String)
                    .Column("ToState", DbType.String)
                    .Column("ToZip", DbType.String)
                    .Column("ToCountry", DbType.String)
                    .Column("Direction", DbType.String)
                );

                // Creating table TweetRecord
                SchemaBuilder.CreateTable("TweetRecord", table => table
                    .ContentPartRecord()
                    .Column("CreatedAt", DbType.DateTime)
                    .Column("TweetId", DbType.String)
                    .Column("InReplyToScreenName", DbType.String)
                    .Column("InReplyToUserId", DbType.Int64)
                    .Column("InReplyToUserIdStr", DbType.String)
                    .Column("IsRetweet", DbType.Boolean)
                    .Column("Source", DbType.String)
                    .Column("Text", DbType.String)
                    .Column("UserMentionsCount", DbType.Int32)
                    .Column("CreatedBy", DbType.String)
                    .Column("CreatedById", DbType.String)
                );

                ContentDefinitionManager.AlterPartDefinition("TweetPart", builder => builder.Attachable());

                ContentDefinitionManager.AlterTypeDefinition(TweetPart.ContentItemTypeName,
                cfg => cfg
                   .WithPart("CommonPart")
                   .WithPart("TweetPart")
                .DisplayedAs("Tweet"));

                ContentDefinitionManager.AlterPartDefinition("ShortMessagePart", builder => builder.Attachable());

                ContentDefinitionManager.AlterTypeDefinition(ShortMessagePart.ContentItemTypeName,
                cfg => cfg
                   .WithPart("CommonPart")
                   .WithPart("ShortMessagePart")
                .DisplayedAs("MessageRequest"));

                ContentDefinitionManager.AlterPartDefinition("SMSPart", builder => builder.Attachable());

                ContentDefinitionManager.AlterTypeDefinition(SMSPart.ContentItemTypeName,
                cfg => cfg
                   .WithPart("CommonPart")
                   .WithPart("SMSPart")
                .DisplayedAs("SMS"));

                return 1;
            }



            public int UpdateFrom1() {
                // Creating table ShortMessageRecord
                // Creating table GatwaySMSPartRecord
                SchemaBuilder.CreateTable("GatwaySMSPartRecord", table => table
                    .ContentPartRecord()
                    .Column("SMSIndex", DbType.Int32)
                    .Column("SMSId", DbType.String)
                    .Column("SMSTo", DbType.String)
                    .Column("SMSBody", DbType.String)
                    .Column("SMSStatus", DbType.String)
                    .Column("SMSError", DbType.String)
                    .Column("SMSDirection", DbType.String)
                );


            ContentDefinitionManager.AlterPartDefinition("GatwaySMSPart", builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("GatwaySMS",
            cfg => cfg
               .WithPart("CommonPart")
               .WithPart("GatwaySMSPart")
            .DisplayedAs("GatwaySMS"));


            return 2;
            }

        }
    
}
