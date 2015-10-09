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
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.CRM.Core.ViewModels;

namespace Orchard.CRM.Core.Drivers
{
    public class CRMCommentsDriver : ContentPartDriver<CRMCommentsPart>
    {
        private IRepository<CRMCommentPartRecord> commentPartRecordRepository;
        public CRMCommentsDriver(IRepository<CRMCommentPartRecord> commentPartRecordRepository)
        {
            this.commentPartRecordRepository = commentPartRecordRepository;
        }

        protected override DriverResult Display(CRMCommentsPart part, string displayType, dynamic shapeHelper)
        {
            if (displayType == "TableRow" || displayType == "Summary" || displayType == "SummaryAdmin" || part.Record.Id == default(int))
            {
                return null;
            }

            CommentsViewModel model = new CommentsViewModel();

            var comments = this.commentPartRecordRepository.Table.Where(c => c.CRMCommentsPartRecord.Id == part.Record.Id).ToList();
            model.Id = part.Record.Id;
            model.ContentItemId = part.ContentItem.Id;
            model.CommentsCount = part.Record.CommentsCount;

            foreach (var record in comments)
            {
                model.Comments.Add(record);
            }

            return ContentShape("Parts_CRMComments",
                () => shapeHelper.Parts_CRMComments(
                    Model: model
                    ));
        }

        protected override DriverResult Editor(CRMCommentsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            return this.Editor(part, shapeHelper);
        }

        protected override DriverResult Editor(CRMCommentsPart part, dynamic shapeHelper)
        {
            CommentsViewModel model = new CommentsViewModel();

            var comments = this.commentPartRecordRepository.Table.Where(c => c.CRMCommentsPartRecord.Id == part.Record.Id).ToList();
            model.Id = part.Record.Id;
            model.CommentsCount = part.Record.CommentsCount;

            foreach (var record in comments)
            {
                model.Comments.Add(record);
            }


            return this.Combined(
                    ContentShape("Parts_CRMComments_Edit",
                        () => shapeHelper.EditorTemplate(
                        TemplateName: "Parts/CRMComments",
                        Model: model,
                        Prefix: Prefix)),
                    ContentShape("Parts_CRMComments_OutsideMainForm",
                        () => shapeHelper.Parts_CRMComments(
                            Model: model,
                            Prefix: Prefix))
                        );
        }
    }
}