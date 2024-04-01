using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MystiqueMcApi.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]

namespace MystiqueMcApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            #if DEBUG
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.DisableTelemetry = true;
            #endif
            ConfigureAuth(app);
            Hangfire.GlobalConfiguration.Configuration
                .UseSqlServerStorage(@"HangfireConnection");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}