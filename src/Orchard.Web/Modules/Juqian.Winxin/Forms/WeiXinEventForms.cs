using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Juqian.Winxin.Forms
{
    public class WeiXinEventForms : IFormProvider
    {
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public WeiXinEventForms(IShapeFactory shapeFactory)
        {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }
        public void Describe(DescribeContext context)
        {
            context.Form("ActivityWeiXinEventKey", shape => Shape.Form(
                Id: "ActivityWeiXinEventKey",
                _Type: Shape.SelectList(
                    Id: "EventType", Name: "EventType",
                    Title: T("Event type of user message"),
                    Description: T("Please select an event message type."))
                    .Add(new SelectListItem { Value = "VIEW", Text = T("View Menu").Text })
                    .Add(new SelectListItem { Value = "CLICK", Text = T("Clicj Menu").Text })
                    .Add(new SelectListItem { Value = "SCAN", Text = T("Scan QR Code").Text })
                    .Add(new SelectListItem { Value = "subscribe", Text = T("Scan QR code [subscribe]").Text })
                    .Add(new SelectListItem { Value = "unsubscribe", Text = T("unsubscribe(Stop Following)").Text }),

                _EventKey: Shape.Textbox(
                Id: "eventKey", Name: "EventKey",
                Title: T("EventKey"),
                Description: T(@"View Menu<br/>
Click on menu :customEventKey<br/>
scan:QRCode<br/>
unfollow:unsubscribe"),
                Classes: new[] { "medium" })
                ));

            context.Form("ActivityWeiXinLocation", shape => Shape.Form(
                Id: "ActivityWeiXinLocation",
                _lat1: Shape.Textbox(
                Id: "lat1", Name: "lat1",
                Title: T("lattitude"),
                Description: T("Reference point position of latitude, you can use <a href='http://api.map.baidu.com/lbsapi/getpoint/index.html' target='_blank'> Baidu map tool </a> pickup latitude and longitude."),
                Classes: new[] { "medium" }),

                _lng1: Shape.Textbox(
                Id: "lng1", Name: "lng1",
                Title: T("longitude"),
                Description: T("longitude"),
                Classes: new[] { "medium" }),

                _Distance: Shape.Textbox(
                Id: "distance", Name: "distance",
                Title: T("distance"),
                Description: T("Distance from the user point of reference, the unit kilometers"),
                Classes: new[] { "medium" })
                ));

            context.Form("ActivityWeiXinText", shape => Shape.Form(
                Id: "ActivityWeiXinText",
                _Operator: Shape.SelectList(
                    Id: "operator", Name: "operator",
                    Title: T("Opertators"),
                    Description: T("Please select an operator to test a text message sent by the user."))
                    .Add(new SelectListItem { Value = "Equals", Text = T("Equals").ToString() })
                    .Add(new SelectListItem { Value = "NotEquals", Text = T("Not Equals").ToString() })
                    .Add(new SelectListItem { Value = "Contains", Text = T("Contains").ToString() })
                    .Add(new SelectListItem { Value = "NotContains", Text = T("NotContains").ToString() })
                    .Add(new SelectListItem { Value = "Starts", Text = T("Starts").ToString() })
                    .Add(new SelectListItem { Value = "NotStarts", Text = T("NotStarts").ToString() })
                    .Add(new SelectListItem { Value = "Ends", Text = T("Ends").ToString() })
                    .Add(new SelectListItem { Value = "NotEnds", Text = T("NotEnds").ToString() }),
                _Text: Shape.TextArea(
                        Id: "textValue", Name: "textValue",
                        Title: T("value"),
                        Description: T("string value."),
                        Classes: new[] { "large", "text", "tokenized" }
                        )));
        }
    }
}