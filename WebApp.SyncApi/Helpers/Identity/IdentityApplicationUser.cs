using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.SyncApi.Helpers.Identity
{
    public class IdentityApplicationUser : IdentityUser
    {
        public int UsuarioId { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class SecurityDbContext : IdentityDbContext<IdentityApplicationUser>
    {
        public SecurityDbContext()
            : base("IdentityConnection", throwIfV1Schema: false)
        {
        }

        public static SecurityDbContext Create()
        {
            return new SecurityDbContext();
        }
    }
}