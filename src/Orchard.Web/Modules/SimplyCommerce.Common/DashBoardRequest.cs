using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Kwikly.Common
{
    public class DashBoardRequest
    {
        public string UserId { get; set; }
        public string BotOwnerId { get; set; }
        public string ResponseUri { get; set; }
        public string Message { get; set; }
        public bool IsQuery { get; set; }
        public bool IsBotReply { get; set; }
        public string Name { get; set; }
    }

    public class DashBoardResponse
    {
        public string ReplyTo { get; set; }
        public string ReplyFrom { get; set; }
        public string ResponseUri { get; set; }
        public string Message { get; set; }
        public int MessageId { get; set; }
    }

    public class DashboardRequestMsg
    {
        public string Id { get; set; }

        public string name { get; set; }

        public string avatar { get; set; }
        public string status { get; set; }
        public List<Msg> chats { get; set; }
        public string mood { get; set; }
        public bool IsTeamReply { get; set; }

        public string conversationId { get; set; }

        public string channelId { get; set; }
    }

    public class DashboardResponseMsg
    {
        public string Id { get; set; }

        public string name { get; set; }

        public string status { get; set; }
        public Msg Reply { get; set; }

        public bool IsTeamReply { get; set; }

        public string conversationId { get; set; }

        public string channelId { get; set; }
    }

     public  class Msg
    {
        public string who { get; set; }

        public string message { get; set; }

        public string time { get; set; }

        public string UserId { get; set; }

        public string conversationId { get; set; }

        public string channelId { get; set; }

        public string Id { get; set; }
         public IList<Attachment> Attachments { get; set; }
         public string botId { get; set; }
         public string serviceurl { get; set; }

         // public IList<Attachment> Attachments { get; set; }
    }

    public class Conv
    {
        public string name { get; set; }

        public string channelId { get; set; }

        public string avatar { get; set; }

        public DateTime timestamp { get; set; }

        public Msg lastMessage { get; set; }

        public string conversationId { get; set; }
        public string recipientId { get; set; }

        public string botId { get; set; }
        public string serviceurl { get; set; }
    }

    public class User
    {
        public string Id { get; set; }

        public string name { get; set; }

        public string avatar { get; set; }
        public string Address { get; set; }
    }

    public class convs
    {
        public Dictionary<string, List<Msg>> messages { get; set; }
    } 

}
