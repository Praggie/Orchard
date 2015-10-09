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

namespace Orchard.CRM.Core.ViewModels
{
    using Orchard.CRM.Core.Models;
    using System.Collections.ObjectModel;

    public class CommentsViewModel
    {
        private Collection<CRMCommentPartRecord> comments = new Collection<CRMCommentPartRecord>();
        
        public int Id { get; set; }
        public int ContentItemId { get; set; }
        public int CommentsCount { get; set; }

        public Collection<CRMCommentPartRecord> Comments
        {
            get
            {
                return this.comments;
            }
        }
    }
}