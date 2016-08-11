using SimplyConverse.Framework.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace SimplyConverse.Framework.Handlers {
    public class ActivityPartHandler: ContentHandler
    {
        public ActivityPartHandler(IRepository<BBActivityRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}