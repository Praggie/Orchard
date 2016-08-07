using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Orchard;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Parameters;

namespace codeathon.connectors
{
    public interface IBotFrameworkService : IDependency
    {
        void ReplyWithText(IMessageActivity textToSend);

    }
}
