using System.Web.Mvc;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;

namespace SimplyConverse.Framework.Controllers {
    [OrchardFeature("SimplyConverse.Framework")]
    [ValidateInput(false)]
    public class AdminController : Controller {

        public AdminController(
           IOrchardServices orchardServices)
        {
            Services = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult chats()
        {
           return View();
        }


    }
}