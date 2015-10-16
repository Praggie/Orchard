using Orchard;
using Orchard.ContentManagement;

namespace codeathon.connectors.Models {
	
    public class GatwaySMSPart : ContentPart<GatwaySMSPartRecord> {
        public int SMSIndex {
            get { return Record.SMSIndex; }
            set { Record.SMSIndex = value; }
        }
        public string SMSId {
            get { return Record.SMSId; }
            set { Record.SMSId = value; }
        }
        public string SMSTo {
            get { return Record.SMSTo; }
            set { Record.SMSTo = value; }
        }
        public string SMSBody {
            get { return Record.SMSBody; }
            set { Record.SMSBody = value; }
        }
        public string SMSStatus {
            get { return Record.SMSStatus; }
            set { Record.SMSStatus = value; }
        }
        public string SMSError {
            get { return Record.SMSError; }
            set { Record.SMSError = value; }
        }
        public string SMSDirection {
            get { return Record.SMSDirection; }
            set { Record.SMSDirection = value; }
        }

    }
}
