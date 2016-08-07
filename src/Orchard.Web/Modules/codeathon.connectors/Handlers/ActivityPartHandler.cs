using codeathon.connectors.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace codeathon.connectors.Handlers {
    public class ActivityPartHandler: ContentHandler
    {
        public ActivityPartHandler(IRepository<TweetRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}