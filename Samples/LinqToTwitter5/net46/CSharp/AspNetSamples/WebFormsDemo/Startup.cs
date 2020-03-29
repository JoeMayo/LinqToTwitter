using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebFormsDemo.Startup))]
namespace WebFormsDemo
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
