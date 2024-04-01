using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace WebApp.SyncApi.Helpers.Identity
{
    public class ApplicationAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            IdentityApplicationUser identityApplicationUser;
            string role;
            using (var repo = new AuthRepository())
            {
                identityApplicationUser = await repo.FindUser(context.UserName, context.Password);
                if (identityApplicationUser == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

                role = await repo.FindRole(identityApplicationUser.Id);
                if (string.IsNullOrEmpty(role))
                {
                    context.SetError("invalid_grant", "The role is incorrect.");
                    return;
                }

                if (role != "WebService")
                {
                    context.SetError("invalid_grant", "The role is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", role));
            identity.AddClaim(new Claim("user", identityApplicationUser.UsuarioId.ToString()));

            context.Validated(identity);

        }
    }
}