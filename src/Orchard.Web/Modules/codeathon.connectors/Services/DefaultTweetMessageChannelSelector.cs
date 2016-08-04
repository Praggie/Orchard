using Orchard;
using Orchard.Messaging.Services;

namespace codeathon.connectors.Services {
    public class DefaultTweetMessageChannelSelector : Component, IMessageChannelSelector {
        private readonly IWorkContextAccessor _workContextAccessor;
        public const string ChannelName = "Tweet";

        public DefaultTweetMessageChannelSelector(IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;
        }

        public MessageChannelSelectorResult GetChannel(string messageType, object payload) {
            if (messageType == "Tweet") {
                var workContext = _workContextAccessor.GetContext();
                return new MessageChannelSelectorResult {
                    Priority = 50,
                    MessageChannel = () => workContext.Resolve<ITweetRelay>()
                };
            }

            return null;
        }
    }
}
