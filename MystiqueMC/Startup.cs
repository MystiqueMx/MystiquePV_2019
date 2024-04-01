using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using MystiqueMC.Helpers.Permissions;
using MystiqueMC.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(MystiqueMC.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace MystiqueMC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureRoles(RolesConfiguration.RolesBase);
        }


        private void ConfigureRoles(string[] rolesBase)
        {
            using (var _context = new ApplicationDbContext())
            {
                var roleManager
                    = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
                foreach (var rol in rolesBase)
                    if (!roleManager.RoleExists(rol))
                        roleManager.Create(new IdentityRole { Name = rol });

            }
        }
    }
}
