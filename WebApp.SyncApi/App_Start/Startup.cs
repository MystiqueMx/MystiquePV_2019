using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebApp.SyncApi.Helpers.Identity;
using Owin;
using System;
using static WebApp.SyncApi.App_Start.IdentityConfig;

namespace WebApp.SyncApi
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(SecurityDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            var OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth2/token"),
#if DEBUG
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(12),
#else
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
#endif
                Provider = new ApplicationAuthorizationServerProvider(),

            };
            app.UseOAuthBearerTokens(OAuthServerOptions);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}