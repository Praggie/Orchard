using Orchard;
using Orchard.ContentManagement.Records;

namespace codeathon.connectors.Models {
	
    public class GatwaySMSPartRecord : ContentPartRecord {
        public virtual int SMSIndex{ get; set; }
        public virtual string SMSId{ get; set; }
        public virtual string SMSTo{ get; set; }
        public virtual string SMSBody{ get; set; }
        public virtual string SMSStatus{ get; set; }
        public virtual string SMSError{ get; set; }
        public virtual string SMSDirection{ get; set; }

    }
}