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

using Orchard.CRM.Core.Models;
using Orchard.CRM.Core.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Mvc;

namespace Orchard.CRM.Core.Services
{
    public static class Converter
    {
        public static void Fill(Collection<SelectListItem> collection, int? selectedValue, IEnumerable<IBasicDataRecord> items)
        {
            foreach (var item in items)
            {
                bool isSelected = selectedValue.HasValue && selectedValue.Value == item.Id;
                collection.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString(),
                    Selected = isSelected
                });
            }
        }

        public static TargetContentItemPermissionViewModel DecodeGroupId(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return null;
            }

            string[] parts = groupId.Split(new[] { ':' });
            if (parts.Length > 0)
            {
                int id = int.Parse(parts[1]);
                TargetContentItemPermissionViewModel targetContentItemPermissionViewModel = new TargetContentItemPermissionViewModel { Checked = true };
                if (parts[0] == "Team")
                {
                    targetContentItemPermissionViewModel.TeamId = id;
                }
                else
                {
                    targetContentItemPermissionViewModel.BusinessUnitId = id;
                }

                return targetContentItemPermissionViewModel;
            }
            else
            {
                return null;
            }
        }
    }
}