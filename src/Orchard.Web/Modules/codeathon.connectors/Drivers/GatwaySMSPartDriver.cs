using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using Orchard.UI.Notify;
using codeathon.connectors.Models;

namespace codeathon.connectors.Drivers
{
	
    public class GatwaySMSPartDriver : ContentPartDriver<GatwaySMSPart>
    {
        private readonly INotifier _notifier;
        private const string TemplateName = "Parts/GatwaySMSPart";

        public Localizer T { get; set; }

        public GatwaySMSPartDriver(INotifier notifier)
        {
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Display(GatwaySMSPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_GatwaySMSPart",
                () => shapeHelper.Parts_GatwaySMSPart(ContentItem: part.ContentItem));
        }

        protected override DriverResult Editor(GatwaySMSPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_GatwaySMSPart",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(GatwaySMSPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(part, Prefix, null, null))
            {
                _notifier.Information(T("GatwaySMSPart edited successfully"));
            }
            else
            {
                _notifier.Error(T("Error during GatwaySMSPart update!"));
            }
            return Editor(part, shapeHelper);
        }

    }
}