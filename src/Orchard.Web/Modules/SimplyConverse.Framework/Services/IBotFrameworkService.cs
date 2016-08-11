using Microsoft.Bot.Connector;
using Orchard;

namespace SimplyConverse.Framework.Services
{
    public interface IBotFrameworkService : IDependency
    {
        void ReplyWithText(IMessageActivity textToSend);


        void SendBBActivityToChatDashboard(Activity newMessage);
    }
}
