using System;
using System.Collections.Generic;
using System.Data;
using codeathon.connectors.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace codeathon.connectors {
    public class Migrations : DataMigrationImpl {

        public int Create() {
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
			);

            ContentDefinitionManager.AlterPartDefinition("TweetPart", builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(TweetPart.ContentItemTypeName,
            cfg => cfg
               .WithPart("CommonPart")
               .WithPart("TweetPart")
            .DisplayedAs("Tweet"));
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("TweetRecord", table => table
                .AddColumn("CreatedBy", DbType.String));
            SchemaBuilder.AlterTable("TweetRecord", table => table
                .AddColumn("CreatedById", DbType.String)
           );
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.CreateTable("ShortMessageRecord", table => table
                 .ContentPartRecord()
                 .Column("MessageId", DbType.String)
                 .Column("Message", DbType.String)
                 .Column("Priority", DbType.String)
                 .Column("EmailSendTo", DbType.String)
                 .Column("SMSSendTo", DbType.String)
                 .Column("TwitterSendTo", DbType.String)
                 .Column("TargetSystem", DbType.String)
           );

            ContentDefinitionManager.AlterPartDefinition("ShortMessagePart", builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(ShortMessagePart.ContentItemTypeName,
            cfg => cfg
               .WithPart("CommonPart")
               .WithPart("ShortMessagePart")
            .DisplayedAs("MessageRequest"));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.CreateTable("SMSRecord", table => table
                 .ContentPartRecord()
                 .Column("Index", DbType.Int32)
                 .Column("DateInserted", DbType.DateTime)
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

            ContentDefinitionManager.AlterPartDefinition("SMSPart", builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(TweetPart.ContentItemTypeName,
            cfg => cfg
               .WithPart("CommonPart")
               .WithPart("SMSPart")
            .DisplayedAs("SMS"));
            return 4;
        }
    }
}
