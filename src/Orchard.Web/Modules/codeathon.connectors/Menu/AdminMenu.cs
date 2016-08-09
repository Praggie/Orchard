using Orchard.Localization;
using Orchard.UI.Navigation;

namespace codeathon.connectors.Menu {
    public class AdminMenu : INavigationProvider
    {

        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .AddImageSet("Bots")
                .Add(T("Bots"), "7.5", layouts => layouts
                    .Action("List", "Admin", new { id = "Layout", area = "Contents" })
                    .LinkToFirstChild(false)
                    .Add(T("Chat"), "1", elements => elements.Action("Chats", "Admin", new { area = "codeathon.connectors" })));
        }
    }
}