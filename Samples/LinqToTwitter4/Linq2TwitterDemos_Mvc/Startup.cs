using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Linq2TwitterDemos_Mvc.Startup))]
namespace Linq2TwitterDemos_Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
