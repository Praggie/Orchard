using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using SimplyConverse.Framework.Models;

namespace SimplyConverse.Framework
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {

            // Creating table TweetRecord
            SchemaBuilder.CreateTable("BBActivityRecord", table => table
                .ContentPartRecord()
                .Column("Timestamp", DbType.DateTime)
                .Column("ActivityId", DbType.String)
                .Column("Type", DbType.String)
                .Column<int>("From_Id")
                .Column("ServiceUrl", DbType.String)
                .Column("ChannelId", DbType.String)
                .Column("Text", DbType.String)
                .Column<int>("Recipient_Id")
                .Column("TextFormat", DbType.String)
                .Column("AttachmentLayout", DbType.String)
                .Column("TopicName", DbType.String)
                .Column("Locale", DbType.String)
                .Column("Summary", DbType.String)
                // .Column("Attachments", DbType.Object)
                //  .Column("Entities", DbType.Object)
                // .Column("ChannelData", DbType.Object)
                .Column("Action", DbType.String)
                .Column("ReplyToId", DbType.String)
            );

            SchemaBuilder.CreateTable("ConversationAccount",
       table => table
           .Column<int>("Id", column => column.PrimaryKey().Identity())
           .Column<bool>("IsGroup")
           .Column<string>("Name")
       );
            SchemaBuilder.CreateTable("ChannelAccount",
     table => table
         .Column<int>("Id", column => column.PrimaryKey().Identity())
         .Column<string>("Name")
     );

            ContentDefinitionManager.AlterPartDefinition("BBActivityPart", builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(BBActivityPart.ContentItemTypeName,
            cfg => cfg
               .WithPart("CommonPart")
               .WithPart("BBActivityPart")
            .DisplayedAs("BotActivity"));
            return 1;
        }

        public int UpdateFrom1()
        {
            // Creating table TweetRecord
            SchemaBuilder.AlterTable("BBActivityRecord", table => table.AddColumn("HistoryDisclosed", DbType.Boolean));

            return 2;
        }
        public int UpdateFrom2()
        {
            // Creating table TweetRecord
            SchemaBuilder.AlterTable("BBActivityRecord", table => table.AddColumn("Conversation_Id", DbType.Int32));

            return 3;
        }
    }



}
