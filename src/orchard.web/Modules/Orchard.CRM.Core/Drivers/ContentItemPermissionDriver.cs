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
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.CRM.Core.Models;
using Orchard.CRM.Core.Services;
using Orchard.CRM.Core.ViewModels;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orchard.CRM.Core.Drivers
{
    public class ContentItemPermissionDriver : ContentPartDriver<ContentItemPermissionPart>
    {
        private readonly ICRMContentOwnershipService contentOwnershipService;
        private readonly IContentOwnershipHelper contentOwnershipHelper;
        private readonly IOrchardServices orchardServices;
        public Localizer T { get; set; }

        public ContentItemPermissionDriver(ICRMContentOwnershipService contentOwnershipService, IOrchardServices orchardServices, IContentOwnershipHelper contentOwnershipHelper)
        {
            this.T = NullLocalizer.Instance;
            this.contentOwnershipHelper = contentOwnershipHelper;
            this.orchardServices = orchardServices;
            this.contentOwnershipService = contentOwnershipService;
        }

        protected override DriverResult Editor(ContentItemPermissionPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            return Editor(part, shapeHelper);
        }

        protected override DriverResult Editor(ContentItemPermissionPart part, dynamic shapeHelper)
        {
            var model = this.contentOwnershipHelper.CreateModel();
            model.IsCurrentUserCustomer = this.contentOwnershipService.IsCurrentUserCustomer();
            this.contentOwnershipHelper.FillPermissions(model, new[] { part.ContentItem }, true);

            if (!model.ContentItems[0].CurrentUserHasRightToChangePermissions && part.Record.Items != null && part.Record.Items.Count > 0)
            {
                return null;
            }

            model.Users.Insert(0, new SelectListItem());

            // clear users and teams for customers
            if (this.contentOwnershipService.IsCurrentUserCustomer())
            {
                model.Users.Clear();
                model.Teams.Clear();
                model.BusinessUnits.ToList().ForEach(c => c.Teams.Clear());
            }

            return ContentShape("Parts_ContentItemPermission_Edit",
                 () => shapeHelper.EditorTemplate(
                     TemplateName: "Parts/ContentItemPermission",
                     Model: model,
                     Prefix: Prefix));
        }

        protected override void Importing(ContentItemPermissionPart part, ImportContentContext context)
        {
            base.Importing(part, context);
        }

        protected override void Exporting(ContentItemPermissionPart part, ExportContentContext context)
        {
            base.Exporting(part, context);
        }

        protected override DriverResult Display(ContentItemPermissionPart part, string displayType, dynamic shapeHelper)
        {
            var model = this.contentOwnershipHelper.CreateModel();
            this.contentOwnershipHelper.FillPermissions(model, new[] { part.ContentItem }, false);

            model.IsCurrentUserAdmin = this.orchardServices.Authorizer.Authorize(Permissions.AdvancedOperatorPermission);

            switch (displayType)
            {
                case "TableRow":
                    return ContentShape("Parts_ContentItemPermission_TableRow",
                        () => shapeHelper.Parts_ContentItemPermission_TableRow(
                            Model: model
                            ));
                case "Summary":
                default:
                    return ContentShape("Parts_ContentItemPermission_Summary",
                        () => shapeHelper.Parts_ContentItemPermission_Summary(
                            Model: model
                            ));
            }
        }
    }
}