using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using codeathon.connectors.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace codeathon.connectors.Handlers
{
    public class SMSPartHandler : ContentHandler
    {
        public SMSPartHandler(IRepository<SMSRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }   
}