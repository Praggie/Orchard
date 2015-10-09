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

using Orchard.ContentManagement;
using Orchard.CRM.Core.Models;
using Orchard.CRM.Core.Services;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IFilterProvider = Orchard.Projections.Services.IFilterProvider;

namespace Orchard.CRM.Core.Providers.Filters
{
    public class TicketFieldsFilter : IFilterProvider
    {
        public Localizer T { get; set; }
        public const string CategoryName = "Ticket";
        public const string RequestingUserType = "RequestingUser";
        public const string RelatedContentItem = "RelatedContentItem";
        public const string StatusType = "Status";
        public const string TicketType = "TicketType";
        public const string NotEqualStatusType = "NotEqualStatus";
        public const string TicketDueDateType = "TicketDueDate";

        public TicketFieldsFilter()
        {
            T = NullLocalizer.Instance;
        }

        public void Describe(Projections.Descriptors.Filter.DescribeFilterContext describe)
        {
            var descriptor = describe.For(CategoryName, T("Ticket Fields"), T("Ticket Fields"));

            // RequestingUser
            descriptor.Element(
                type: RequestingUserType,
                name: new LocalizedString("RequestingUser"),
                description: T("RequestingUser Selection"),
                filter: (context) => ApplyRequestingUserFilter(context, false),
                display: context => Display(context, "RequestingUser_Id"),
                form: RequestingUserForm.FormName
                    );

            // RelatedContentItem
            descriptor.Element(
                type: RelatedContentItem,
                name: new LocalizedString("RelatedContentItem"),
                description: T("RelatedContentItem Selection"),
                filter: (context) => ApplyRelatedContentItemIdFilter(context, false),
                display: context => Display(context, "RelatedContentItem_Id"),
                form: RequestingUserForm.FormName
                    );

            // Status
            descriptor.Element(
                type: StatusType,
                name: new LocalizedString("Status"),
                description: T("Status Selection"),
                filter: (context) => ApplyStatusFilter(context, false),
                display: context => Display(context, "StatusRecord_Id"),
                form: TicketStatusForm.FormName
                    );

            // TicketType
            descriptor.Element(
                type: TicketType,
                name: new LocalizedString("Ticket Type"),
                description: T("Ticke Type Selection"),
                filter: (context) => ApplyTicketTypeFilter(context, false),
                display: context => Display(context, "TicketType_Id"),
                form: TicketTypeForm.FormName
                    );

            // status not equal
            descriptor.Element(
                type: NotEqualStatusType,
                name: new LocalizedString("NotEqualStatus"),
                description: T("Not Equal Status Selection"),
                filter: (context) => ApplyStatusFilter(context, false),
                display: context => Display(context, "StatusRecord_Id"),
                form: TicketStatusForm.FormName
                    );

            // Due Data
            descriptor.Element(
                type: TicketDueDateType,
                name: new LocalizedString("Ticket Due Date"),
                description: T("Ticket Due Date"),
                filter: (context) => ApplyDueDateFilter(context),
                display: context => Display(context, "DueDate"),
                form: TicketDueDateForm.FormName
                    );
        }

        public void ApplyStatusFilter(FilterContext context, bool notEqual)
        {
            Action<IHqlExpressionFactory> predicate = null;
            Action<IAliasFactory> alias = null;

            if (context.State.UnStatus != null && (bool)context.State.UnStatus)
            {
                alias = x=>x.ContentPartRecord<TicketPartRecord>();
                predicate = x => x.IsNull("StatusRecord");
                context.Query = context.Query.Where(alias, predicate);
                return;
            }

            if (context.State.Status_Id == null || string.IsNullOrEmpty(context.State.Status_Id.Value))
            {
                return;
            }

            string aliasName = "TicketPartRecord_Status";
            alias = x => x.ContentPartRecord<TicketPartRecord>().Property("StatusRecord", aliasName);

            predicate = x => x.Eq("Id", context.State.Status_Id.Value); ;

            if (notEqual)
            {
                predicate = x => x.Not(predicate);
            }

            context.Query = context.Query.Where(alias, predicate);
        }

        public void ApplyTicketTypeFilter(FilterContext context, bool notEqual)
        {
            Action<IHqlExpressionFactory> predicate = null;
            Action<IAliasFactory> alias = null;

            if (context.State.TicketType_Id == null || string.IsNullOrEmpty(context.State.TicketType_Id.Value))
            {
                return;
            }

            string aliasName = "TicketPartRecord_TicketType";
            alias = x => x.ContentPartRecord<TicketPartRecord>().Property("TicketType", aliasName);

            predicate = x => x.Eq("Id", context.State.TicketType_Id.Value); ;

            if (notEqual)
            {
                predicate = x => x.Not(predicate);
            }

            context.Query = context.Query.Where(alias, predicate);
        }

        public void ApplyRequestingUserFilter(FilterContext context, bool notEqual)
        {
            if (context.State.RequestingUser_Id == null || string.IsNullOrEmpty(context.State.RequestingUser_Id.Value))
            {
                return;
            }

            string aliasName = "TicketPartRecord_RequestingUser";
            Action<IAliasFactory> alias = x => x.ContentPartRecord<TicketPartRecord>().Property("RequestingUser", aliasName);
            Action<IHqlExpressionFactory> predicate = x => x.Eq("Id", context.State.RequestingUser_Id.Value);

            if (notEqual)
            {
                predicate = x => x.Not(predicate);
            }

            context.Query = context.Query.Where(alias, predicate);
        }

        public void ApplyRelatedContentItemIdFilter(FilterContext context, bool notEqual)
        {
            if (context.State.RelatedContentItemId == null || string.IsNullOrEmpty(context.State.RelatedContentItemId.Value))
            {
                return;
            }

            string aliasName = "TicketPartRecord_RelatedContentItem";
            Action<IAliasFactory> alias = x => x.ContentPartRecord<TicketPartRecord>().Property("RelatedContentItem", aliasName);
            Action<IHqlExpressionFactory> predicate = x => x.Eq("Id", context.State.RelatedContentItemId.Value);

            if (notEqual)
            {
                predicate = x => x.Not(predicate);
            }

            context.Query = context.Query.Where(alias, predicate);
        }

        public void ApplyDueDateFilter(FilterContext context)
        {
            if (context.State.MaxDueDate == null && context.State.MinDueDate == null)
            {
                return;
            }

            DateTime? maxDueDate = context.State.MaxDueDate;
            DateTime? minDueDate = context.State.MinDueDate;

            Action<IAliasFactory> alias = x => x.ContentPartRecord<TicketPartRecord>();

            if (maxDueDate.HasValue)
            {
                // shifted to the last second of the day
                maxDueDate = maxDueDate.Value.Subtract(maxDueDate.Value.TimeOfDay).AddDays(1).AddSeconds(-1);
                Action<IHqlExpressionFactory> predicate = x => x.Le("DueDate", maxDueDate.Value);
                context.Query = context.Query.Where(alias, predicate);
            }

            if (minDueDate.HasValue)
            {
                // shifted to the first second of the day
                minDueDate = minDueDate.Value.Subtract(minDueDate.Value.TimeOfDay);

                Action<IHqlExpressionFactory> predicate = x => x.Ge("DueDate", minDueDate.Value);
                context.Query = context.Query.Where(alias, predicate);
            }
        }

        private LocalizedString Display(FilterContext context, string name)
        {
            return T(name);
        }
    }

    public class TicketDueDateForm : SimpleTextBoxFilterForm
    {
        public const string FormName = "TicketDueDate";
        public TicketDueDateForm(IShapeFactory shapeFactory)
            : base(shapeFactory)
        {
            this.formName = FormName;
            this.textboxId = "MaxDueDate";
            this.textboxName = "MaxDueDate";
            this.textboxTitle = "Due Date deadline";
            this.textboxDescription = "Due date";
        }
    }

    public class RequestingUserForm : SimpleTextBoxFilterForm
    {
        public const string FormName = "RequestingUser";
        public RequestingUserForm(IShapeFactory shapeFactory)
            : base(shapeFactory)
        {
            this.formName = FormName;
            this.textboxId = "RequestingUser_Id";
            this.textboxName = "RequestingUser_Id";
            this.textboxTitle = "Id of the requesting user";
            this.textboxDescription = "Id of the requesting user";
        }
    }
  
    public class TicketStatusForm : BasicDataFilterForm
    {
        private IBasicDataService basicDataService;

        public const string FormName = "TicketPartRecord_StatusFilter";
        public TicketStatusForm(IShapeFactory shapeFactory, IBasicDataService basicDataService)
            : base(shapeFactory)
        {
            this.basicDataService = basicDataService;
            this.formName = FormName;
            this.selectName = "Status_Id";
            this.selectId = "Status_Id";
            this.selectTitle = "Status ID";
            this.selectDescription = "Status of the record";
            this.selectSize = 20;
        }

        protected override IList<SelectListItem> GetData()
        {
            var items = basicDataService.GetStatusRecords().OrderBy(c => c.OrderId).ToList().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(CultureInfo.InvariantCulture),
                Text = c.Name
            }).ToList();

            items.Add(new SelectListItem { Text = "{Request.Form:StatusId}", Value = "{Request.Form:StatusId}" });
            items.Add(new SelectListItem { Text = "{Request.QueryString:StatusId}", Value = "{Request.QueryString:StatusId}" });
            return items;
        }
    }

    public class TicketTypeForm : BasicDataFilterForm
    {
        private IBasicDataService basicDataService;

        public const string FormName = "TicketPartRecord_TicketTypeFilter";
        public TicketTypeForm(IShapeFactory shapeFactory, IBasicDataService basicDataService)
            : base(shapeFactory)
        {
            this.basicDataService = basicDataService;
            this.formName = FormName;
            this.selectName = "TicketType_Id";
            this.selectId = "TicketType_Id";
            this.selectTitle = "TicketType ID";
            this.selectDescription = "TicketType of the record";
            this.selectSize = 20;
        }

        protected override IList<SelectListItem> GetData()
        {
            var items = this.basicDataService.GetTicketTypes().OrderBy(c => c.OrderId).ToList().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(CultureInfo.InvariantCulture),
                Text = c.Name
            }).ToList();

            return items;
        }
    }
}