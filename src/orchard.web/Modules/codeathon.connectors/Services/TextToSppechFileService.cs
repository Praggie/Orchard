/// Orchard Collaboration is a plugin for Orchard CMS that provides an integrated ticketing system for it.
/// Copyright (C) 2014-2015  Siyamand Ayubi
///
/// This file is part of Orchard Collaboration.
///
///    Orchard Collaboration is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License as published by
///    the Free Software Foundation, either version 3 of the License, or
///    (at your option) any later version.
///
///    Orchard Collaboration is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with Orchard Collaboration.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Orchard;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.MediaLibrary.Services;
using Orchard.ContentManagement;
using Orchard.Logging;
using Orchard.UI.Notify;

namespace codeathon.connectors.Services
{
    public class TextToSppechFileService : ITextToSppechFileService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMediaLibraryService mediaService;
        private static readonly object folderCreationLock = new object();

        public TextToSppechFileService(
            IMediaLibraryService mediaService, IOrchardServices orchardServices)
        {
            this.mediaService = mediaService;
            this._orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public bool AddFile(string fileName, Stream stream)
        {

            var path = EnsureFolder("Uploads", "TextToSpeech");
            var files = this.mediaService.GetMediaFiles(path);
           
            if (files.Any(x => x.Name == fileName))
            {
                this.mediaService.DeleteFile(path, fileName);
            }

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);


            try {

                this.mediaService.UploadMediaFile(path, fileName, bytes);

                var mediapart = this.mediaService.ImportMedia(path, fileName);
                this._orchardServices.ContentManager.Create(mediapart);
            }
            catch (Exception e)
            {
                _orchardServices.Notifier.Error(T("Error while saving media {0}. Please check the logs", path + "/" + fileName));
                Logger.Error(e, "Error while importing {0}", path + "/" + fileName);
                return false;
            }

            return true;
        }

        private string EnsureFolder(string relativePath, string name)
        {
            lock (folderCreationLock)
            {
                var folders = this.mediaService.GetMediaFolders(relativePath);
                if (!folders.Any(x => x.Name == name))
                {
                    this.mediaService.CreateFolder(relativePath, name);
                }
            }

            return Path.Combine(relativePath, name);
        }
    }
}