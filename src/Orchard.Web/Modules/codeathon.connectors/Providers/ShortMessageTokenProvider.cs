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
    public class ShortMessageTokenProvider : ITokenProvider
    {
        private Localizer T { get; set; }
        private readonly IContentManager contentManager;

        /// <summary>
        /// The key of CRMCommentPart in the EvaluateContext.Data dictionary
        /// </summary>
        public const string ShortMessageKey = "ShortMessage";

        public ShortMessageTokenProvider(
            IContentManager contentManager
        )
        {
            this.contentManager = contentManager;
            this.T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context)
        {
            context.For(ShortMessageTokenProvider.ShortMessageKey, T("ShortMessage"), T("ShortMessage"))
           .Token("Message", T("Message"), T("Message"))
           .Token("MessagePriority", T("MessagePriority"), T("MessagePriority"))
           .Token("NotificationRequired", T("NotificationRequired"), T("NotificationRequired"))
           .Token("SMSMessageSendTo", T("SMSMessageSendTo"), T("SMSMessageSendTo"))
           .Token("EmailMessageSendTo", T("EmailMessageSendTo"), T("EmailMessageSendTo"))
           .Token("TwitterMessageSendTo", T("TwitterMessageSendTo"), T("TwitterMessageSendTo"))
           .Token("TargetQueue", T("TwitterMessageSendTo"), T("TwitterMessageSendTo"))
           .Token("MessageId", T("TwitterMessageSendTo"), T("TwitterMessageSendTo"))          
           ;
        }

        public void Evaluate(EvaluateContext context)
        {
            this.SubstituteShortMessageProperties(context);
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


        private void SubstituteShortMessageProperties(EvaluateContext context)
        {
            var part = this.GetPart<ShortMessagePart>(context);
            if (part == null)
            {
                return;
            }

            context.For(ShortMessageTokenProvider.ShortMessageKey, () => this.GetPart<TweetPart>(context))
                 .Token("Message", contextParameter => { return part.Message; })
                 .Token("MessagePriority", contextParameter => { return part.MessagePriority; })
                 .Token("NotificationRequired", contextParameter => { return part.NotificationRequired; })
                 .Token("SMSMessageSendTo", contextParameter => { return part.SMSMessageSendTo; })
                 .Token("EmailMessageSendTo", contextParameter => { return part.EmailMessageSendTo; })
                 .Token("TwitterMessageSendTo", contextParameter => { return part.TwitterMessageSendTo; })
                 .Token("TargetQueue", contextParameter => { return part.TargetQueue; })
                 .Token("MessageId", contextParameter => { return part.MessageId; });
        }
    }
}