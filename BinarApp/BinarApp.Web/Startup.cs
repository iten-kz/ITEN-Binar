using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BinarApp.Web.Startup))]
namespace BinarApp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
