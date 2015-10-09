﻿/// Orchard Collaboration is a plugin for Orchard CMS that provides an integrated ticketing system for it.
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
using Orchard.CRM.Core.Models;

namespace Orchard.CRM.Core.Drivers
{
    public class BusinessUnitDriver : CRMContentPartDriver<BusinessUnitPart>
    {
        public BusinessUnitDriver(IOrchardServices services)
            : base(services)
        {
             
        }

        protected override DriverResult Display(
             BusinessUnitPart part, string displayType, dynamic shapeHelper)
        {
            string viewName = BusinessUnitDriver.GetViewName(displayType, "BusinessUnit");
            return ContentShape(viewName,
                () => shapeHelper.Parts_BusinessUnit(
                    Model: part));
        }

        //GET
        protected override DriverResult Editor(BusinessUnitPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_BusinessUnit_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/BusinessUnit",
                    Model: part,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(
            BusinessUnitPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}