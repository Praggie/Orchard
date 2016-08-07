using Juqian.Winxin.Models;
using Juqian.Winxin.Services;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Juqian.Winxin.Activities
{
    public abstract class WinXinActivity : Event
    {
        public Localizer T { get; set; }

        public override bool CanStartWorkflow
        {
            get { return true; }
        }

        public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return true;
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] { T("Done") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            yield return T("Done");
        }

        public override LocalizedString Category
        {
            get { return T("MSG"); }
        }
    }

    public class WX_TextActivity : WinXinActivity
    {
        public override string Name
        {
            get { return "TextMessage"; }
        }

        public override LocalizedString Description
        {
            get { return T("TextMessage activity"); }
        }

        public override string Form
        {
            get
            {
                return "ActivityWeiXinText";
            }
        }
        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] { T("true"), T("false") };
        }

        public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var part = workflowContext.Content.As<WXMsgPart>();
            return part.MsgType == "text";
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var part = workflowContext.Content.As<WXMsgPart>();
            var content = part.Content;
            var op = activityContext.GetState<string>("operator");
            var value = activityContext.GetState<string>("textValue");
            switch (op)
            {
                case "Equals":
                    if (content == value)
                    {
                        yield return T("true");
                        yield break;
                    }
                    break;
                case "NotEquals":
                    if (content != value)
                    {
                        yield return T("true");
                        yield break;
                    }
                    break;
                case "Contains":
                    if (content.Contains(value))
                    {
                        yield return T("true");
                        yield break;
                    }
                    break;
                case "NotContains":
                    if (!content.Contains(value))
                    {
                        yield return T("true");
                        yield break;
                    }
                    break;
                case "Starts":
                    if (content.StartsWith(value))
                    {
                        yield return T("true");
                        yield break;
                    }
                    break;
                case "NotStarts":
                    if (!content.StartsWith(value))
                    {
                        yield return T("true");
                        yield break;
                    }
                    break;
                case "Ends":
                    if (content.EndsWith(value))
                    {
                        yield return T("true");
                        yield break;
                    }
                    break;
                case "NotEnds":
                    if (!content.EndsWith(value))
                    {
                        yield return T("true");
                        yield break;
                    }
                    break;
            }


            yield return T("false");
        }
    }
    public class WX_ImageActivity : WinXinActivity
    {
        public override string Name
        {
            get { return "ImageMessage"; }
        }

        public override LocalizedString Description
        {
            get { return T("When User sends an Image as Message."); }
        }
    }
    public class WX_VoiceActivity : WinXinActivity
    {
        public override string Name
        {
            get { return "AudioMessage"; }
        }

        public override LocalizedString Description
        {
            get { return T("When user sends and Audio message."); }
        }
    }
    public class WX_VideoActivity : WinXinActivity
    {
        public override string Name
        {
            get { return "VideoMesage"; }
        }

        public override LocalizedString Description
        {
            get { return T("When User sends and video message."); }
        }
    }

    public class WX_LinkActivity : WinXinActivity
    {
        public override string Name
        {
            get { return "LinkMessage"; }
        }

        public override LocalizedString Description
        {
            get { return T("When user sends and link."); }
        }
    }
    public class WX_EventActivity : WinXinActivity
    {
        readonly IWinXinService _winXinService;
        public WX_EventActivity(IWinXinService winXinService)
        {
            _winXinService = winXinService;
        }
        public override string Name
        {
            get { return "EventMessage"; }
        }

        public override string Form
        {
            get
            {
                return "ActivityWeiXinEventKey";
            }
        }

        public override LocalizedString Description
        {
            get { return T("When user sends an event."); }
        }

        public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var part = workflowContext.Content.As<WXMsgPart>();
            if (part.MsgType != "event")
                return false;

            var type = activityContext.GetState<string>("EventType");
            if (type != part.Event)
                return false;

            var eventKey = activityContext.GetState<string>("EventKey");
            if (!string.IsNullOrWhiteSpace(eventKey) && eventKey != part.EventKey)
                return false;

            return true;
        }
    }

    public class WX_LocationEventActivity : WinXinActivity
    {
        readonly IWinXinService _winXinService;
        public WX_LocationEventActivity(IWinXinService winXinService)
        {
            _winXinService = winXinService;
        }
        public override string Name
        {
            get { return "LocationEventMessage"; }
        }
        public override string Form
        {
            get
            {
                return "ActivityWeiXinLocation";
            }
        }

        public override LocalizedString Description
        {
            get { return T("Users agree to report the location, the number of each entry into the public session, will be reported to the location at the time of entry, or after entering the session reported a location every 5 seconds."); }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] { T("outsideRange"), T("inRange"), T("default") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            double lat1 = 0, lng1 = 0, distance = 0, distance1 = 0;
            bool flag = true;
            var part = workflowContext.Content.As<WXMsgPart>();

            try
            {
                lat1 = activityContext.GetState<double>("lat1");
                lng1 = activityContext.GetState<double>("lng1");
                distance = activityContext.GetState<double>("distance");
                distance1 = _winXinService.GetDistance(lat1, lng1, part.Latitude, part.Longitude);
            }
            catch { flag = false; }

            if (lat1 + lat1 + distance == 0 || !flag)
            {
                yield return T("default");
                yield break;
            }

            if (distance1 <= distance)
            {
                yield return T("outsideRange");
                yield break;
            }
            else
            {
                yield return T("inRange");
                yield break;
            }
        }

        public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var part = workflowContext.Content.As<WXMsgPart>();
            return part.MsgType == "event" && part.Event == "LOCATION";
        }
    }

    public class WX_LocationActivity : WinXinActivity
    {
        readonly IWinXinService _winXinService;

        public WX_LocationActivity(IWinXinService winXinService)
        {
            _winXinService = winXinService;
        }

        public override string Name
        {
            get { return "LocationMessage"; }
        }

        public override string Form
        {
            get
            {
                return "ActivityWeiXinLocation";
            }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] { T("outsideRange"), T("inRange"), T("default") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            double lat1 = 0, lng1 = 0, distance = 0, distance1 = 0;
            bool flag = true;
            var part = workflowContext.Content.As<WXMsgPart>();

            try
            {
                lat1 = activityContext.GetState<double>("lat1");
                lng1 = activityContext.GetState<double>("lng1");
                distance = activityContext.GetState<double>("distance");
                distance1 = _winXinService.GetDistance(lat1, lng1, part.Location_X, part.Location_Y);
            }
            catch { flag = false; }

            if (lat1 + lat1 + distance == 0 || !flag)
            {
                yield return T("default");
                yield break;
            }

            if (distance1 <= distance)
            {
                yield return T("outsideRange");
                yield break;
            }
            else
            {
                yield return T("inRange");
                yield break;
            }
        }

        public override LocalizedString Description
        {
            get { return T("When user sends his location."); }
        }

        public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var part = workflowContext.Content.As<WXMsgPart>();
            return part.MsgType == "location";
        }
    }
}