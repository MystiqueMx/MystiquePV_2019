using MystiqueMC.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebApp.SyncApi.Helpers.Identity
{
    public class AuthRepository : IDisposable
    {
        private readonly SecurityDbContext _ctx;

        private readonly UserManager<IdentityApplicationUser> _userManager;

        public AuthRepository()
        {
            _ctx = new SecurityDbContext();
            _userManager
                       = new UserManager<IdentityApplicationUser>(new UserStore<IdentityApplicationUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(IdentityApplicationUser userModel, string password)
        {
            var result = await _userManager.CreateAsync(userModel, password);
            if (!result.Succeeded) return result;

            var identityUser = await _userManager.FindByNameAsync(userModel.UserName);
            var resultRole = await _userManager.AddToRoleAsync(identityUser.Id, "Beneficiario");

            return result;
        }

        public async Task<IdentityApplicationUser> FindUser(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public async Task<string> FindRole(string id)
        {
            var roles = await _userManager.GetRolesAsync(id);
            return roles.FirstOrDefault();
        }
        public async Task<IdentityResult> ChangePassword(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await _userManager.ResetPasswordAsync(user.Id, token, password);
            return result;
        }
        public void Dispose()
        {
            _ctx?.Dispose();
            _userManager?.Dispose();

        }
    }
}