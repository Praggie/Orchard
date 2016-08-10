using System;
using System.Linq;
using Orchard;
using Orchard.Environment.Features;
using Orchard.Forms.Services;

namespace SimplyConverse.Framework.Forms {
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

                    _TextToSend: shape.Textbox(
                        Id: "TextToSend",
                        Name: "TextToSend",
                        Title: T("Text To Send"),
                        Description: T("The Text To Verify in TwitterUser ."),
                        Classes: new[] {"text", "large", "tokenized"}));
               
                return form;
            });
        }

        void IFormEventHandler.Validating(ValidatingContext context)
        {
            if (context.FormName != "SendActivity") return;

            var textToSend = context.ValueProvider.GetValue("TextToSend").AttemptedValue;

            if (String.IsNullOrWhiteSpace(textToSend))
            {
                context.ModelState.AddModelError("TextToSend", T("You must specify an TextToSend or a token that evaluates to an TextToSend address.").Text);
            }
        }

        void IFormEventHandler.Building(BuildingContext context) { }
        void IFormEventHandler.Built(BuildingContext context) { }
        void IFormEventHandler.Validated(ValidatingContext context) { }
    }
}