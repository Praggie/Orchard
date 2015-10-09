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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NHibernate.Transform;
    using Orchard.ContentManagement;
    using Orchard.ContentManagement.Records;
    using Orchard.CRM.Core.Models;
    using Orchard.CRM.Core.Providers.Filters;
    using Orchard.Data;
    using Orchard.Localization;
    using Orchard.Users.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.ContentManagement.FieldStorage.InfosetStorage;
    using Orchard.Security;
    using Orchard.CRM.Core.Providers.ActivityStream;

    public class ActivityStreamService : IActivityStreamService
    {
        private readonly IRepository<ActivityStreamRecord> repository;
        private readonly IOrchardServices services;
        private readonly IProjectionManagerWithDynamicSort projectionManagerWithDynamicSort;
        private readonly Lazy<ISessionLocator> sessionLocator;
        private readonly IBasicDataService basicDataService;
        private readonly IEnumerable<IActivityStreamWriter> activityStreamWriters;

        public Localizer T { get; set; }

        public ActivityStreamService(
            IBasicDataService basicDataService,
            IRepository<ActivityStreamRecord> repository,
            IProjectionManagerWithDynamicSort projectionManagerWithDynamicSort,
            IOrchardServices services,
            IEnumerable<IActivityStreamWriter> activityStreamWriters,
            Lazy<ISessionLocator> sessionLocator)
        {
            this.activityStreamWriters = activityStreamWriters;
            this.sessionLocator = sessionLocator;
            this.projectionManagerWithDynamicSort = projectionManagerWithDynamicSort;
            this.services = services;
            this.repository = repository;
            this.basicDataService = basicDataService;
            this.T = NullLocalizer.Instance;
        }

        public int TicketsHistoryVisibleByCurrentUserCount()
        {
            if (this.services.Authorizer.Authorize(Permissions.AdvancedOperatorPermission))
            {
                return this.repository.Table.Count();
            }

            var contentQuery = this.CreateTicketQuery();

            DefaultHqlQuery defaultQuery = contentQuery as DefaultHqlQuery;
            var hql = defaultQuery.ToHql(true);
            hql = string.Format("select COUNT(c) from Orchard.CRM.Core.Models.ActivityStreamRecord as c WHERE c.RelatedVersion.Id in ({0})", hql);

            var session = this.sessionLocator.Value.For(typeof(ActivityStreamRecord));
            return Convert.ToInt32(session.CreateQuery(hql)
                           .SetCacheable(true)
                           .UniqueResult());
        }

        public IEnumerable<ActivityStreamRecord> TicketsHistoryVisibleByCurrentUser(int pageId, int pageSize)
        {
            if (this.services.Authorizer.Authorize(Permissions.AdvancedOperatorPermission))
            {
                var items = this.repository.Table.OrderByDescending(c => c.Id).Skip(pageSize * pageId).Take(pageSize);
                return items;
            }

            var contentQuery = this.CreateTicketQuery();

            DefaultHqlQuery defaultQuery = contentQuery as DefaultHqlQuery;
            var hql = defaultQuery.ToHql(true);
            hql = string.Format("select c from Orchard.CRM.Core.Models.ActivityStreamRecord as c WHERE c.RelatedVersion.Id in ({0}) order by c.Id desc", hql);

            var session = this.sessionLocator.Value.For(typeof(ActivityStreamRecord));
            var query = session
              .CreateQuery(hql)
              .SetResultTransformer(Transformers.AliasToEntityMap)
              .SetCacheable(false)
              .SetFirstResult(pageId * pageSize)
              .SetMaxResults(pageSize).List<IDictionary>();

            return query.Select(c => (ActivityStreamRecord)c["0"]).ToList();
        }

        public dynamic TakeSnapshot(ContentItem contentItem)
        {
            dynamic oldData = new ExpandoObject();
            var oldDataDictionary = oldData as IDictionary<string, object>;

            foreach (var writer in this.activityStreamWriters)
            {
                oldDataDictionary[writer.Name] = writer.TakeSnapshot(contentItem);
            }

            return oldData;
        }

        public void WriteChangesToStreamActivity(ContentItem contentItem, dynamic snapshot)
       {
           this.WriteChangesToStreamActivity(contentItem, snapshot, false);
        }

        public void WriteChangesToStreamActivity(ContentItem contentItem, dynamic snapshot, bool createdBySystem)
        {
            var currentUser = this.services.WorkContext.CurrentUser;
            int? userId = !createdBySystem && currentUser != null ? (int?)currentUser.Id : null;

            List<string> changes = new List<string>();
            List<KeyValuePair<string, int>> descriptions = new List<KeyValuePair<string, int>>();
            string contentDescription = string.Empty;
            IDictionary<string, object> oldDataDictionary = snapshot != null? snapshot as IDictionary<string, object>: null;

            foreach (var writer in this.activityStreamWriters)
            {
                var writerSnapshot = oldDataDictionary != null? oldDataDictionary[writer.Name]: null;
                ActiviyStreamWriterContext context = new ActiviyStreamWriterContext(contentItem, writerSnapshot, currentUser, changes);

                if (writer.CanApply(context))
                {
                    writer.AddChanges(context);

                    descriptions.Add(writer.GetContentDescription(context));
                }
            }

            // description with highest weight
            contentDescription = descriptions
                .Where(c => !string.IsNullOrEmpty(c.Key))
                .OrderByDescending(c => c.Value)
                .Select(c => c.Key)
                .FirstOrDefault();

            // if it is an update, but noting is changed, then no activity record must be created
            if (snapshot != null && changes.Count == 0)
            {
                return;
            }

            this.Add(userId, contentItem.Id, contentItem.VersionRecord.Id, changes.ToArray(), contentDescription);
        }

        private string Encode(string[] changes, string contentDescription)
        {
            dynamic model = new JObject();
            model.Changes = JToken.FromObject(changes);
            model.ContentDescription = contentDescription;

            return JsonConvert.SerializeObject(model);
        }

        public void Add(int? userId, int contentId, int versionId, string[] changes, string contentDescription)
        {
            string description = this.Encode(changes, contentDescription);
            ActivityStreamRecord newRecord = new ActivityStreamRecord();
            newRecord.RelatedContent = new ContentItemRecord { Id = contentId };
            newRecord.RelatedVersion = new ContentItemVersionRecord { Id = versionId };
            newRecord.Description = description;
            newRecord.User = userId.HasValue ? new UserPartRecord { Id = userId.Value } : null;
            newRecord.CreationDateTime = DateTime.UtcNow;
            repository.Create(newRecord);
            repository.Flush();
        }

        private IHqlQuery CreateTicketQuery()
        {
            var contentQuery = this.services.ContentManager.HqlQuery().ForType("Ticket").ForVersion(VersionOptions.Published);

            if (this.services.Authorizer.Authorize(Permissions.AdvancedOperatorPermission))
            {
                return contentQuery;
            }

            dynamic state = new JObject();
            int userId = this.services.WorkContext.CurrentUser.Id;
            int? accessType = null;
            var userBusinessUnitIds = this.basicDataService
                .GetBusinessUnitMembers()
                .Where(c => c.UserPartRecord.Id == userId)
                .Select(c => c.BusinessUnitPartRecord.Id)
                .Distinct()
                .ToList();

            var teams = new List<int>();
            List<int> businessUnits = new List<int>();
            List<int> users = new List<int>();

            users.Add(userId);

            dynamic permissionsState = new
            {
                Users = users,
                Teams = teams,
                BusinessUnits = userBusinessUnitIds,
                AccessType = accessType
            };

            contentQuery = this.projectionManagerWithDynamicSort.ApplyFilter(contentQuery, ContentItemPermissionFilter.CategoryName, ContentItemPermissionFilter.AnySelectedUserTeamBusinessUnit, permissionsState);

            return contentQuery;
        }
    }
}