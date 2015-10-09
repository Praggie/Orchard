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

namespace Orchard.CRM.Core.Providers.ActivityStream
{
    using Orchard.CRM.Core.Models;
    using Orchard.CRM.Core.Services;
    using Orchard.Localization;
    using Orchard.Security;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Orchard.ContentManagement;
    using System.Linq;
    using System.Web;
    using System.Dynamic;
    using Orchard.Users.Models;

    public class ContentItemPermissionPartStreamWriter : IActivityStreamWriter
    {
        private readonly IOrchardServices services;
        private readonly IBasicDataService basicDataService;

        public ContentItemPermissionPartStreamWriter(IOrchardServices services, IBasicDataService basicDataService)
        {
            this.basicDataService = basicDataService;
            this.services = services;
            this.T = NullLocalizer.Instance;
        }

        public Localizer T;

        public bool CanApply(ActiviyStreamWriterContext context)
        {
            return context.ContentItem.As<ContentItemPermissionPart>() != null && (context.Snapshot != null);
        }

        public void AddChanges(ActiviyStreamWriterContext context)
        {
            if (!this.CanApply(context))
            {
                return;
            }

            List<string> changes = new List<string>();

            Func<byte, string> getAccessType = c =>
            {
                switch (c)
                {
                    case ContentItemPermissionAccessTypes.Assignee:
                        return T("Assignee").Text;
                    case ContentItemPermissionAccessTypes.SharedForEdit:
                        return T("Edit Access").Text;
                    case ContentItemPermissionAccessTypes.SharedForView:
                        return T("Watcher Access").Text;
                }

                return T("[NULL]").Text;
            };

            List<ContentItemPermissionDetailRecord> oldPermissions = 
                context.Snapshot != null && context.Snapshot.Items != null?
                context.Snapshot.Items as List<ContentItemPermissionDetailRecord> :
                new List<ContentItemPermissionDetailRecord>();

            ContentItemPermissionPart newValue = context.ContentItem.As<ContentItemPermissionPart>();

            var newPermissions = newValue.Record.Items ?? new List<ContentItemPermissionDetailRecord>();
            foreach (var oldPermission in oldPermissions)
            {
                var newPermission = newPermissions.FirstOrDefault(c => c.Id == oldPermission.Id);
                if (newPermission == null)
                {
                    changes.Add(this.GetContentItemPermissionChangeDescription(oldPermission, "Delete '{0}' of '{1}' to the ticket", getAccessType(oldPermission.AccessType)));
                    continue;
                }

                if (newPermission.AccessType != oldPermission.AccessType)
                {
                    changes.Add(this.GetContentItemPermissionChangeDescription(oldPermission, "Change Access of '{1}' to the ticket to '{0}'", getAccessType(newPermission.AccessType)));
                    continue;
                }
            }

            foreach (var newPermission in newPermissions)
            {
                var oldPermission = oldPermissions.FirstOrDefault(c => c.Id == newPermission.Id);
                if (oldPermission == null)
                {
                    string message = newPermission.AccessType == ContentItemPermissionAccessTypes.Assignee ?
                        "Assign the ticket to: '{1}'" : "Grant '{1}' the '{0}' to the ticket";
                    changes.Add(this.GetContentItemPermissionChangeDescription(newPermission, message, getAccessType(newPermission.AccessType)));
                    continue;
                }
            }


            context.AddRange(changes);
        }

        public KeyValuePair<string, int> GetContentDescription(ActiviyStreamWriterContext context)
        {
            return default(KeyValuePair<string, int>);
        }

        private string GetContentItemPermissionChangeDescription(ContentItemPermissionDetailRecord record, string format, string accessType)
        {
            IUser user = record.User != null ?
                 this.basicDataService.GetOperatorOrCustomerUser(record.User.Id) :
                 null;

            BusinessUnitPart businessUnit = record.BusinessUnit != null ?
                this.basicDataService.GetBusinessUnits()
                .Select(c => c.As<BusinessUnitPart>())
                .FirstOrDefault(c => c.Record.Id == record.BusinessUnit.Id)
                : null;

            if (user != null)
            {
                string fullName = CRMHelper.GetFullNameOfUser(user);
                return string.Format(
                    CultureInfo.CurrentUICulture,
                    T(format).Text,
                    accessType,
                    fullName);
            }

            if (businessUnit != null)
            {
                return string.Format(
                    CultureInfo.CurrentUICulture,
                    T(format).Text,
                    accessType,
                    businessUnit.Name + T(" group").Text);
            }

            return string.Empty;
        }


        public string Name
        {
            get { return "ContentItemPermission"; }
        }

        public dynamic TakeSnapshot(ContentItem contentItem)
        {
            var part = contentItem.As<ContentItemPermissionPart>();
            if (part == null)
            {
                return null;
            }

            dynamic oldData = new ExpandoObject();

            oldData.HasOwner = part.Record.HasOwner;
            List<ContentItemPermissionDetailRecord> items = new List<ContentItemPermissionDetailRecord>();
            oldData.Items = items;

            if (part.Record.Items != null)
            {
                foreach (var item in part.Record.Items)
                {
                    items.Add(new ContentItemPermissionDetailRecord
                    {
                        Id = item.Id,
                        AccessType = item.AccessType,
                        BusinessUnit = item.BusinessUnit != null ? new BusinessUnitPartRecord { Id = item.BusinessUnit.Id } : null,
                        User = item.User != null ? new UserPartRecord { Id = item.User.Id } : null
                    });
                }
            }

            return oldData;
        }
    }
}