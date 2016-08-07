using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
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
    public class WinXinRespForms : IFormProvider
    {
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        readonly IContentManager _contentManager;

        public WinXinRespForms(IShapeFactory shapeFactory, IContentManager contentManager)
        {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;

            _contentManager = contentManager;
        }
        public void Describe(DescribeContext context)
        {
            context.Form("ActivityWinXinResp", shape =>
            {
                var f = Shape.Form(
                Id: "ActivityWinXinResp",
                _ContentList: Shape.SelectList(
                    Id: "contentId", Name: "contentId",
                    Title: T("Response to user message"),
                    Description: T("Please select a user in response to the micro-letter message.")));

                var list = _contentManager.Query().ForType(new[] { "WXTextMsg", "WXImageMsg", "WXVoiceMsg", "WXVideoMsg", "WXMusicMsg"
                ,"WXNewsMsg","WXDKFMsg"}).List().Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = string.Format("[{0}]{1}", item.TypeDefinition.DisplayName, item.As<TitlePart>().Title)
                });

                foreach (var item in list)
                    f._ContentList.Add(item);

                return f;
            });

            context.Form("ActivityWeiXinDispatch", shape =>
            {
                var f = Shape.Form(
                    Id: "ActivityWeiXinDispatch",
                    _ApiUrl: Shape.Textbox(
                        Id: "api_url", Name: "api_url",
                        Title: T("Third-party micro-channel api address"),
                        Classes: new[] { "medium" }),
                    _Token: Shape.Textbox(
                        Id: "api_token", Name: "api_token",
                        Title: T("Third-party micro-channel token"),
                        Classes: new[] { "medium" })
                    );
                return f;
            });
        }
    }
}