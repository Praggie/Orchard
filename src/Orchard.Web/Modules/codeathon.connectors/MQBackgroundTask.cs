using codeathon.connectors;
using Orchard.Environment.Extensions;
using Orchard.Tasks;

namespace codeathon.conectors {
    public class MQBackgroundTask : IBackgroundTask {

        private readonly IMQService _iMQService;

        public MQBackgroundTask(IMQService iMQService)
        {
            _iMQService = iMQService;
        }

        public void Sweep() {
            _iMQService.Connect();
        }
    }

}