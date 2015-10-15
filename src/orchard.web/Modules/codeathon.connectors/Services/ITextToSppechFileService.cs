
using System;
using System.Collections.Generic;
using System.IO;
using Orchard;

namespace codeathon.connectors.Services
{
    public interface ITextToSppechFileService : IDependency {
        bool AddFile(string fileName, Stream stream);
    }
}