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
            SchemaBuilder.CreateTable("WMQMessage", table => table
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

            ContentDefinitionManager.AlterTypeDefinition("ShortMessagePart",
            cfg => cfg
               .WithPart("CommonPart")
               .WithPart("ShortMessagePart")
            .DisplayedAs("Message Request"));
            return 3;
        }
    }
}