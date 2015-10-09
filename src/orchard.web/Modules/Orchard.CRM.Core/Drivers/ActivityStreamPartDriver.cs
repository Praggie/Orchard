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

namespace Orchard.CRM.Core.Drivers
{
    using System.Linq;
    using Orchard.ContentManagement.Drivers;
    using Orchard.CRM.Core.Models;
    using Orchard.CRM.Core.Services;
    using Orchard.UI.Navigation;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Dynamic;
    using System;
    using Orchard.Localization;
    using Orchard.Data;

    public class ActivityStreamPartDriver : CRMContentPartDriver<ActivityStreamPart>
    {
        private readonly IActivityStreamService activityStreamService;
        private readonly IBasicDataService basicDataService;
        private readonly Lazy<ISessionLocator> sessionLocator;

        public ActivityStreamPartDriver(
            IOrchardServices services,
            Lazy<ISessionLocator> sessionLocator,
            IBasicDataService basicDataService,
            IActivityStreamService activityStreamService)
            : base(services)
        {
            this.basicDataService = basicDataService;
            this.activityStreamService = activityStreamService;
            this.sessionLocator = sessionLocator;
            this.T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override DriverResult Display(ActivityStreamPart part, string displayType, dynamic shapeHelper)
        {
            if (this.services.WorkContext.CurrentUser == null)
            {
                return null;
            }

            // retrieving paging parameters
            var queryString = this.services.WorkContext.HttpContext.Request.QueryString;

            var pageKey = "page";
            var page = 0;
            int pageSize = 20;

            // don't try to page if not necessary
            if (queryString.AllKeys.Contains(pageKey))
            {
                int.TryParse(queryString[pageKey], out page);
            }

            // query the database
            int count = this.activityStreamService.TicketsHistoryVisibleByCurrentUserCount();
            var items = this.activityStreamService.TicketsHistoryVisibleByCurrentUser(page == 0 ? page : page - 1, pageSize).ToList();
            var session = this.sessionLocator.Value.For(typeof(ActivityStreamRecord));

            // set time zone
            items.ForEach(c =>
            {
                session.Evict(c);
                c.CreationDateTime = this.SetSiteTimeZone(c.CreationDateTime);
            });

            dynamic model = new ExpandoObject();
            DateTime today = this.SetSiteTimeZone(DateTime.UtcNow);

            // create pager
            var currentSite = this.services.WorkContext.CurrentSite;
            var pager = new Pager(currentSite, page, pageSize);
            model.Pager = this.services.New.Pager(pager).TotalItemCount(count);

            // contains the list of days, each day will contain the list of items in that day
            List<dynamic> dayModels = new List<dynamic>();
            model.Days = dayModels;
            var groupsByDay = items.GroupBy(c => c.CreationDateTime.Date).OrderByDescending(c => c.Key).ToList();

            foreach (var group in groupsByDay)
            {
                dynamic dayModel = new ExpandoObject();
                dayModels.Add(dayModel);
                dayModel.Date = group.Key;
                dayModel.Title = group.Key.Date == today.Date ? T("Today").Text : group.Key.ToLongDateString();

                List<dynamic> itemModels = new List<dynamic>();
                dayModel.Items = itemModels;
                foreach (var item in group)
                {
                    dynamic itemModel = new ExpandoObject();
                    dynamic description = JObject.Parse(item.Description);
                    itemModel.Changes = description.Changes;
                    itemModel.Id = item.RelatedContent.Id;
                    itemModel.ContentDescription = description.ContentDescription;
                    itemModel.DateTime = item.CreationDateTime;

                    if (item.User != null)
                    {
                        var user = this.basicDataService.GetOperatorOrCustomerUser(item.User.Id);
                        itemModel.UserFullName = user != null ? CRMHelper.GetFullNameOfUser(user) : item.User.UserName;
                    }
                    else
                    {
                        itemModel.UserFullName = T("System").Text;
                    }

                    itemModels.Add(itemModel);
                }
            }

            return this.ContentShape("Parts_DashboardActivityStream",
                () => shapeHelper.Parts_DashboardActivityStream(Model: model));
        }

        private DateTime SetSiteTimeZone(DateTime dateTime)
        {
            var currentSite = this.services.WorkContext.CurrentSite;

            if (String.IsNullOrEmpty(currentSite.SiteTimeZone))
            {
                return dateTime;
            }

            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, TimeZoneInfo.Utc.Id, currentSite.SiteTimeZone);
        }
    }
}