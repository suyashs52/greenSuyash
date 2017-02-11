using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GreenOxPOS.Startup))]
namespace GreenOxPOS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
