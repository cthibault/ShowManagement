using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShowManagement.Web.Startup))]
namespace ShowManagement.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
