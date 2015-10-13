using System;
using Orchard;
using Orchard.Forms.Services;

namespace codeathon.connectors.Folder {
    public class SendTweetForm : Component, IFormProvider, IFormEventHandler
    {
        void IFormProvider.Describe(DescribeContext context)
        {
            context.Form("SendTweet", factory => {
                var shape = (dynamic)factory;
                var form = shape.Form(
                    Id: "SendTweet",
                    _TwitterUser: shape.Textbox(
                        Id: "TwitterUser",
                        Name: "TwitterUser",
                        Title: T("Twitter User"),
                        Description: T("Reply to this user."),
                        Classes: new[] { "text", "large", "tokenized" }), 
                    _InReplyToTweet: shape.Textbox(
                        Id: "InReplyToTweet",
                        Name: "InReplyToTweet",
                        Title: T("In Reply To Tweet"),
                        Description: T("The text block that needs to be searched."),
                        Classes: new[] { "text", "large", "tokenized" }),
                    _TextToSend: shape.Textbox(
                        Id: "TextToSend",
                        Name: "TextToSend",
                        Title: T("Text To Send"),
                        Description: T("The Text To Verify in TwitterUser ."),
                        Classes: new[] { "text", "large", "tokenized" }),
                    _SendAsPM: shape.Checkbox(
                        Id: "SendAsPM",
                        Name: "SendAsPM",
                        Title: T("SendAsPM"),
                        Description: T("Check to send tweet as PM."),
                        Value: true));

            return form;
            });
        }

        void IFormEventHandler.Validating(ValidatingContext context)
        {
            if (context.FormName != "SendTweet") return;

            var twitterUser = context.ValueProvider.GetValue("TwitterUser").AttemptedValue;
            var textToSend = context.ValueProvider.GetValue("TextToSend").AttemptedValue;
            var inReplyToTweet = context.ValueProvider.GetValue("InReplyToTweet").AttemptedValue;

            if (String.IsNullOrWhiteSpace(twitterUser))
            {
                context.ModelState.AddModelError("TwitterUser", T("You must specify a TwitterUser or a token that evaluates to a TwitterUser.").Text);
            }

            if (String.IsNullOrWhiteSpace(textToSend))
            {
                context.ModelState.AddModelError("TextToSend", T("You must specify an TextToSend or a token that evaluates to an TextToSend address.").Text);
            }

            if (String.IsNullOrWhiteSpace(inReplyToTweet))
            {
                context.ModelState.AddModelError("TextToSend", T("You must specify an inReplyToTweet or a token that evaluates to an inReplyToTweet address.").Text);
            }
        }

        void IFormEventHandler.Building(BuildingContext context) { }
        void IFormEventHandler.Built(BuildingContext context) { }
        void IFormEventHandler.Validated(ValidatingContext context) { }
    }
}