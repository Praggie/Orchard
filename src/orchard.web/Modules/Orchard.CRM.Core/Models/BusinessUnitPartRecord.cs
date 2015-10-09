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

using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Orchard.CRM.Core.Models
{
    public class BusinessUnitPartRecord : ContentPartRecord, IBasicDataRecord
    {
        public virtual BusinessUnitPartRecord Parent { get; set; }
        public virtual string Description { get; set; }
        public virtual string Name { get; set; }

        [Aggregate]
        [XmlArray("Teams")]
        public virtual IList<TeamPartRecord> Teams { get; set; }

        [Aggregate]
        [XmlArray("Childs")]
        public virtual IList<BusinessUnitPartRecord> Childs { get; set; }

        [Aggregate]
        [XmlArray("BusinessUnitMembers")]
        public virtual IList<BusinessUnitMemberPartRecord> Members { get; set; }
    }
}