using System;
using Orchard;
using Orchard.Forms.Services;

namespace codeathon.connectors.Folder {
    public class ConvertTextToSpeechForm : Component, IFormProvider, IFormEventHandler
    {
        void IFormProvider.Describe(DescribeContext context)
        {
            context.Form("ConvertTextToSpeech", factory => {
                var shape = (dynamic)factory;
                var form = shape.Form(
                    Id: "ConvertTextToSpeech",
                    _TextBlock: shape.Textbox(
                        Id: "textBlock",
                        Name: "TextBlock",
                        Title: T("Text Block"),
                        Description: T("The text block that needs to be searched."),
                        Classes: new[] { "text", "large", "tokenized" }),
                    _FileName: shape.Textbox(
                        Id: "fileName",
                        Name: "FileName",
                        Title: T("FileName"),
                        Description: T("The Text To Verify in textblock ."),
                        Classes: new[] { "text", "large", "tokenized" }));

                return form;
            });
        }

        void IFormEventHandler.Validating(ValidatingContext context)
        {
            if (context.FormName != "ConvertTextToSpeech") return;

            var textBlock = context.ValueProvider.GetValue("TextBlock").AttemptedValue;
            var fileName = context.ValueProvider.GetValue("FileName").AttemptedValue;

            if (String.IsNullOrWhiteSpace(textBlock))
            {
                context.ModelState.AddModelError("TextBlock", T("You must specify a TextBlock or a token that evaluates to a TextBlock.").Text);
            }

            if (String.IsNullOrWhiteSpace(fileName))
            {
                context.ModelState.AddModelError("FileName", T("You must specify an FileName or a token that evaluates to an FileName address.").Text);
            }
        }

        void IFormEventHandler.Building(BuildingContext context) { }
        void IFormEventHandler.Built(BuildingContext context) { }
        void IFormEventHandler.Validated(ValidatingContext context) { }
    }
}