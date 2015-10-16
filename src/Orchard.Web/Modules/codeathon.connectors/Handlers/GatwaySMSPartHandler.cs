using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using codeathon.connectors.Models;

namespace codeathon.connectors.Handlers
{
	
    public class GatwaySMSPartHandler : ContentHandler
    {
        public GatwaySMSPartHandler(IRepository<GatwaySMSPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
			
        }
    }
}
