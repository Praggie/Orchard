using System;
using Orchard;
using Orchard.Forms.Services;

namespace codeathon.connectors.Folder {
    public class TextFilterContainsForm : Component, IFormProvider, IFormEventHandler
    {
        void IFormProvider.Describe(DescribeContext context)
        {
            context.Form("TextFilterContains", factory => {
                var shape = (dynamic)factory;
                var form = shape.Form(
                    Id: "TextFilterContains",
                    _TextBlock: shape.Textbox(
                        Id: "textBlock",
                        Name: "TextBlock",
                        Title: T("Text Block"),
                        Description: T("The text block that needs to be searched."),
                        Classes: new[] { "text", "large", "tokenized" }),
                    _TextToVerify: shape.Textbox(
                        Id: "textToVerify",
                        Name: "TextToVerify",
                        Title: T("TextToVerify"),
                        Description: T("The Text To Verify in textblock ."),
                        Classes: new[] { "text", "large", "tokenized" }));

                return form;
            });
        }

        void IFormEventHandler.Validating(ValidatingContext context)
        {
            if (context.FormName != "TextFilterContains") return;

            var textBlock = context.ValueProvider.GetValue("TextBlock").AttemptedValue;
            var textToVerify = context.ValueProvider.GetValue("TextToVerify").AttemptedValue;

            if (String.IsNullOrWhiteSpace(textBlock))
            {
                context.ModelState.AddModelError("TextBlock", T("You must specify a TextBlock or a token that evaluates to a TextBlock.").Text);
            }

            if (String.IsNullOrWhiteSpace(textToVerify))
            {
                context.ModelState.AddModelError("TextToVerify", T("You must specify an TextToVerify or a token that evaluates to an TextToVerify address.").Text);
            }
        }

        void IFormEventHandler.Building(BuildingContext context) { }
        void IFormEventHandler.Built(BuildingContext context) { }
        void IFormEventHandler.Validated(ValidatingContext context) { }
    }
}