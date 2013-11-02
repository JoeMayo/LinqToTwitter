using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Linq2TwitterDemos_MVC.Startup))]
namespace Linq2TwitterDemos_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
