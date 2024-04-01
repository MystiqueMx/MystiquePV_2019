using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApp.SyncApi.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace WebApp.SyncApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}