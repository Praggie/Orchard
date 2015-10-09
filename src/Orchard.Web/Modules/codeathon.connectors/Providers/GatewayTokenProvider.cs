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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using codeathon.connectors.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Tokens;

namespace codeathon.connectors.Providers
{
    public class GatewayTokenProvider : ITokenProvider
    {
        private Localizer T { get; set; }
        private readonly IContentManager contentManager;

        /// <summary>
        /// The key of CRMCommentPart in the EvaluateContext.Data dictionary
        /// </summary>
        public const string TweetKey = "Tweet";

 

        public GatewayTokenProvider(
            IContentManager contentManager
        )
        {
            this.contentManager = contentManager;
            this.T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context)
        {

            context.For(GatewayTokenProvider.TweetKey, T("Tweet"), T("Tweet"))
                 .Token("Text", T("Text"), T("Text"))
                 .Token("TweetId", T("TweetId"), T("TweetId"))
                 .Token("CreatedBy", T("CreatedBy"), T("CreatedBy"));

        }

        public void Evaluate(EvaluateContext context)
        {
            this.SubstituteTweetProperties(context);

        }

        private TPart GetPart<TPart>(EvaluateContext context)
            where TPart : ContentPart
        {
            ContentItem contentItem = (ContentItem)context.Data["Content"];
            if (contentItem == null)
            {
                return null;
            }

            return contentItem.As<TPart>();
        }

        
        private void SubstituteTweetProperties(EvaluateContext context)
        {
            var part = this.GetPart<TweetPart>(context);
            if (part == null)
            {
                return;
            }

            context.For(GatewayTokenProvider.TweetKey, () => this.GetPart<TweetPart>(context))
                .Token("Text", contextParameter =>
                {
                    return part.Text;
                }).Token("CreatedBy", contextParameter =>
                {
                    return part.CreatedBy;
                })
               .Token("TweetId", contextParameter =>
               {
                   return part.TweetId;
               });
        }

    }
}