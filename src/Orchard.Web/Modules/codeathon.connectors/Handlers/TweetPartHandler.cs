using codeathon.connectors.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace codeathon.connectors.Handlers {
    public class TweetPartHandler: ContentHandler
    {
        public TweetPartHandler(IRepository<TweetRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}