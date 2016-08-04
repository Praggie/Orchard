using System.Management.Instrumentation;
using codeathon.connectors.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Data.Providers;

namespace codeathon.connectors.Drivers
{
    public class TweetPartDriver : ContentPartDriver<TweetPart>
    {

        protected override DriverResult Display(
            TweetPart part, string displayType, dynamic shapeHelper) {

            return ContentShape("Parts_Tweet", () => shapeHelper.Parts_Tweet(
                Text: part.Text,
                CreatdAt:part.CreatedAt,
                CreatedBy: part.CreatedBy));

        }

        ////GET
        //protected override DriverResult Editor(
        //    TweetPart part, dynamic shapeHelper)
        //{

        //    return ContentShape("Parts_Map_Edit",
        //        () => shapeHelper.EditorTemplate(
        //            TemplateName: "Parts/Map",
        //            Model: part,
        //            Prefix: Prefix));
        //}
        ////POST
        //protected override DriverResult Editor(
        //    TweetPart part, IUpdateModel updater, dynamic shapeHelper)
        //{

        //    updater.TryUpdateModel(part, Prefix, null, null);
        //    return Editor(part, shapeHelper);
        //}
    }
}