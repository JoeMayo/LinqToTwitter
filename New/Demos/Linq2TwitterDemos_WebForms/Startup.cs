using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Linq2TwitterDemos_WebForms.Startup))]
namespace Linq2TwitterDemos_WebForms
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
