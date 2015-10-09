using codeathon.connectors.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace codeathon.connectors.Drivers
{
    public class TweetPartDriver : ContentPartDriver<TweetPart>
    {
/*
        protected override DriverResult Display(
            TweetPart part, string displayType, dynamic shapeHelper)
        {

            return ContentShape("Parts_Map", () => shapeHelper.Parts_Map(
                Longitude: part.Longitude,
                Latitude: part.Latitude));
        }

        //GET
        protected override DriverResult Editor(
            TweetPart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_Map_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Map",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(
            TweetPart part, IUpdateModel updater, dynamic shapeHelper)
        {

            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }*/
    }
}