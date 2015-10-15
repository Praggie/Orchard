using codeathon.connectors.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace codeathon.connectors.Handlers
{
    public class ShortMessagePartHandler : ContentHandler
    {
        public ShortMessagePartHandler(IRepository<ShortMessageRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }   
}