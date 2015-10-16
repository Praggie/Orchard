using codeathon.connectors;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Tasks;

namespace codeathon.conectors {
    public class MQBackgroundTask : IFeatureEventHandler {

        private readonly IMQService _iMQService;

        public MQBackgroundTask(IMQService iMQService)
        {
            _iMQService = iMQService;
        }

        public void Sweep() {
            _iMQService.Connect();
        }

        public void Installing(Orchard.Environment.Extensions.Models.Feature feature)
        {
            
        }

        public void Installed(Orchard.Environment.Extensions.Models.Feature feature)
        {
            
        }

        public void Enabling(Orchard.Environment.Extensions.Models.Feature feature)
        {
            
        }

        public void Enabled(Orchard.Environment.Extensions.Models.Feature feature)
        {
            _iMQService.Connect();
        }

        public void Disabling(Orchard.Environment.Extensions.Models.Feature feature)
        {
            
        }

        public void Disabled(Orchard.Environment.Extensions.Models.Feature feature)
        {
            
        }

        public void Uninstalling(Orchard.Environment.Extensions.Models.Feature feature)
        {
            
        }

        public void Uninstalled(Orchard.Environment.Extensions.Models.Feature feature)
        {
            
        }
    }
}