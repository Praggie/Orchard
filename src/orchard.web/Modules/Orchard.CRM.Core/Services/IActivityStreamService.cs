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

namespace Orchard.CRM.Core.Services
{
    using Orchard.ContentManagement;
    using Orchard.CRM.Core.Models;
    using System.Collections.Generic;

    public interface IActivityStreamService : IDependency
    {
        void Add(int? userId, int contentId, int versionId, string[] changes, string contentDescription);
        IEnumerable<ActivityStreamRecord> TicketsHistoryVisibleByCurrentUser(int pageId, int pageSize);
        int TicketsHistoryVisibleByCurrentUserCount();
        void WriteChangesToStreamActivity(ContentItem contentItem, dynamic snapshot);
        void WriteChangesToStreamActivity(ContentItem contentItem, dynamic snapshot, bool createdBySystem);
        dynamic TakeSnapshot(ContentItem contentItem);
    }
}