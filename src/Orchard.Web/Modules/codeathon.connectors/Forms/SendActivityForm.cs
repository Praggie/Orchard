using System;
using System.Linq;
using Orchard;
using Orchard.Environment.Features;
using Orchard.Forms.Services;

namespace codeathon.connectors.Forms {
    public class SendActivityForm : Component, IFormProvider, IFormEventHandler
    {
        private readonly IFeatureManager _featureManager;
        public SendActivityForm(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }
        void IFormProvider.Describe(DescribeContext context)
        {
            var jobsQueueEnabled = _featureManager.GetEnabledFeatures().Any(x => x.Id == "Orchard.JobsQueue");

            context.Form("SendActivity", factory => {
                var shape = (dynamic)factory;
                var form = shape.Form(
                    Id: "SendActivity",
                    _TwitterUser: shape.Textbox(
                        Id: "TwitterUser",
                        Name: "TwitterUser",
                        Title: T("Twitter User"),
                        Description: T("Reply to this user."),
                        Classes: new[] { "text", "large", "tokenized" }), 
                    _InReplyToActivity: shape.Textbox(
                        Id: "InReplyToActivity",
                        Name: "InReplyToActivity",
                        Title: T("In Reply To Activity"),
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
                        Description: T("Check to send Activity as PM."),
                        Value: true));

                if (jobsQueueEnabled)
                {
                    form._Queued(shape.Checkbox(
                            Id: "Queued", Name: "Queued",
                            Title: T("Queued"),
                            Checked: false, Value: "true",
                            Description: T("Check send it as a queued job.")));

                    form._Priority(shape.Textbox(
                            Id: "priority",
                            Name: "Priority",
                            Title: T("Priority"),
                            Classes: new[] { "text", "large", "tokenized" },
                            Description: ("The priority of this message.")
                        ));

                }
                return form;
            });
        }

        void IFormEventHandler.Validating(ValidatingContext context)
        {
            if (context.FormName != "SendActivity") return;

            var twitterUser = context.ValueProvider.GetValue("TwitterUser").AttemptedValue;
            var textToSend = context.ValueProvider.GetValue("TextToSend").AttemptedValue;
            var inReplyToActivity = context.ValueProvider.GetValue("InReplyToActivity").AttemptedValue;

            if (String.IsNullOrWhiteSpace(twitterUser))
            {
                context.ModelState.AddModelError("TwitterUser", T("You must specify a TwitterUser or a token that evaluates to a TwitterUser.").Text);
            }

            if (String.IsNullOrWhiteSpace(textToSend))
            {
                context.ModelState.AddModelError("TextToSend", T("You must specify an TextToSend or a token that evaluates to an TextToSend address.").Text);
            }

            if (String.IsNullOrWhiteSpace(inReplyToActivity))
            {
                context.ModelState.AddModelError("TextToSend", T("You must specify an inReplyToActivity or a token that evaluates to an inReplyToActivity address.").Text);
            }
        }

        void IFormEventHandler.Building(BuildingContext context) { }
        void IFormEventHandler.Built(BuildingContext context) { }
        void IFormEventHandler.Validated(ValidatingContext context) { }
    }
}