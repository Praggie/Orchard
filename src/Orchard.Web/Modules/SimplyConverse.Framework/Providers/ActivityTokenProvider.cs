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
using Microsoft.Bot.Connector;
using SimplyConverse.Framework.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Tokens;

namespace SimplyConverse.Framework.Providers
{
    public class ActivityTokenProvider : ITokenProvider
    {
        private Localizer T { get; set; }
        private readonly IContentManager contentManager;

        /// <summary>
        /// The key of CRMCommentPart in the EvaluateContext.Data dictionary
        /// </summary>
        public const string ActivityKey = "Activity"; 

        public ActivityTokenProvider(
            IContentManager contentManager
        )
        {
            this.contentManager = contentManager;
            this.T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context)
        {
             context.For(ActivityTokenProvider.ActivityKey, T("Activity"), T("Activity"))
                 .Token("Text", T("Text"), T("Text"))
                 .Token("ActivityId", T("ActivityId"), T("ActivityId"))
                 .Token("ReplyToId", T("ReplyToId"), T("ReplyToId"))
                 .Token("ChannelId", T("ChannelId"), T("ChannelId"))
                 .Token("ServiceUrl", T("ServiceUrl"), T("ServiceUrl"))
                 .Token("TextFormat", T("TextFormat"), T("TextFormat"))
                 .Token("AttachmentLayout", T("AttachmentLayout"), T("AttachmentLayout"))
                 .Token("ConversationId", T("ConversationId"), T("ConversationId"))
                 .Token("ConversationName", T("ConversationName"), T("ConversationName"))
                 .Token("Type", T("Type"), T("Type"));

            context.For(ActivityKey, T("Activity"), T("Activity")).Token("originalBBActivity", T("originalBBActivity"), T("originalBBActivity"));
        }

        public void Evaluate(EvaluateContext context)
        {
            this.SubstituteActivityProperties(context);

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

        
        private void SubstituteActivityProperties(EvaluateContext context)
        {
            var part = this.GetPart<BBActivityPart>(context);
            if (part == null)
            {
                return;
            }

            context.For(ActivityTokenProvider.ActivityKey, () => this.GetPart<BBActivityPart>(context))
                .Token("Text", contextParameter => part.Text).Token("Type", contextParameter => part.Type)
                .Token("ReplyToId", contextParameter => part.ReplyToId)
                 .Token("ChannelId", contextParameter => part.ChannelId)
                  .Token("ServiceUrl", contextParameter => part.ServiceUrl)
                   .Token("TextFormat", contextParameter => part.TextFormat)
                   .Token("AttachmentLayout", contextParameter => part.AttachmentLayout)
                   .Token("ConversationId", contextParameter => part.Conversation.Id)
                   .Token("ConversationName", contextParameter => part.Conversation.Name)
               .Token("ActivityId", contextParameter => part.ActivityId);

            context.For<Activity>(ActivityKey).Token("originalBBActivity", contextParameter => context.Data["originalBBActivity"]);
        }

    }
}