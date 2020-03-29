using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcDemo.Startup))]
namespace MvcDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
