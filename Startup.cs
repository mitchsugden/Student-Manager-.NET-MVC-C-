using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BYOD_manager.Startup))]
namespace BYOD_manager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
