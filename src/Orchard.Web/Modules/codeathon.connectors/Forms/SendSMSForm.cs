using System;
using Orchard;
using Orchard.Forms.Services;

namespace codeathon.connectors.Folder {
    public class SendSMSForm : Component, IFormProvider, IFormEventHandler
    {
        void IFormProvider.Describe(DescribeContext context)
        {
            context.Form("SendSMS", factory => {
                var shape = (dynamic)factory;
                var form = shape.Form(
                    Id: "SendSMS",
                    _SMSUser: shape.Textbox(
                        Id: "SMSUser",
                        Name: "SMSUser",
                        Title: T("SMS User"),
                        Description: T("Reply to this user."),
                        Classes: new[] { "text", "large", "tokenized" }), 
                    _InReplyToSMS: shape.Textbox(
                        Id: "InReplyToSMS",
                        Name: "InReplyToSMS",
                        Title: T("In Reply To SMS"),
                        Description: T("The text block that needs to be searched."),
                        Classes: new[] { "text", "large", "tokenized" }),
                    _TextToSend: shape.Textbox(
                        Id: "TextToSend",
                        Name: "TextToSend",
                        Title: T("Text To Send"),
                        Description: T("The Text To Verify in SMSUser ."),
                        Classes: new[] { "text", "large", "tokenized" }));

                return form;
            });
        }

        void IFormEventHandler.Validating(ValidatingContext context)
        {
            if (context.FormName != "SendSMS") return;

            var SMSUser = context.ValueProvider.GetValue("SMSUser").AttemptedValue;
            var textToSend = context.ValueProvider.GetValue("TextToSend").AttemptedValue;
            var inReplyToSMS = context.ValueProvider.GetValue("InReplyToSMS").AttemptedValue;

            if (String.IsNullOrWhiteSpace(SMSUser))
            {
                context.ModelState.AddModelError("SMSUser", T("You must specify a SMSUser or a token that evaluates to a SMSUser.").Text);
            }

            if (String.IsNullOrWhiteSpace(textToSend))
            {
                context.ModelState.AddModelError("TextToSend", T("You must specify an TextToSend or a token that evaluates to an TextToSend address.").Text);
            }

            //if (String.IsNullOrWhiteSpace(inReplyToSMS))
            //{
            //    context.ModelState.AddModelError("inReplyToSMS", T("You must specify an inReplyToSMS or a token that evaluates to an inReplyToSMS address.").Text);
            //}
        }

        void IFormEventHandler.Building(BuildingContext context) { }
        void IFormEventHandler.Built(BuildingContext context) { }
        void IFormEventHandler.Validated(ValidatingContext context) { }
    }
}