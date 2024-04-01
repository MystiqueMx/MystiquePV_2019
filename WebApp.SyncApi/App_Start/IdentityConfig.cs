using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Threading.Tasks;
using WebApp.SyncApi.Helpers.Identity;

namespace WebApp.SyncApi.App_Start
{
    public class IdentityConfig
    {
        public class EmailService : IIdentityMessageService
        {
            public Task SendAsync(IdentityMessage message)
            {
                // Plug in your email service here to send an email.
                return Task.FromResult(0);
            }
        }

        public class SmsService : IIdentityMessageService
        {
            public Task SendAsync(IdentityMessage message)
            {
                // Plug in your SMS service here to send a text message.
                return Task.FromResult(0);
            }
        }

        // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
        public class ApplicationUserManager : UserManager<Helpers.Identity.IdentityApplicationUser>
        {
            public ApplicationUserManager(IUserStore<Helpers.Identity.IdentityApplicationUser> store)
                : base(store)
            {
            }

            public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
            {
                var manager = new ApplicationUserManager(new UserStore<IdentityApplicationUser>(context.Get<SecurityDbContext>()));
                // Configure validation logic for usernames
                manager.UserValidator = new UserValidator<IdentityApplicationUser>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = false
                };

                // Configure validation logic for passwords
                manager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6,
                };

                // Configure user lockout defaults
                manager.UserLockoutEnabledByDefault = true;
                manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                manager.MaxFailedAccessAttemptsBeforeLockout = 5;

                // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
                // You can write your own provider and plug it in here.
                manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<Helpers.Identity.IdentityApplicationUser>
                {
                    MessageFormat = "Your security code is {0}"
                });
                manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<Helpers.Identity.IdentityApplicationUser>
                {
                    Subject = "Security Code",
                    BodyFormat = "Your security code is {0}"
                });
                manager.EmailService = new EmailService();
                manager.SmsService = new SmsService();
                var dataProtectionProvider = options.DataProtectionProvider;
                if (dataProtectionProvider != null)
                {
                    manager.UserTokenProvider =
                        new DataProtectorTokenProvider<Helpers.Identity.IdentityApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
                }
                return manager;
            }
        }

        // Configure the application sign-in manager which is used in this application.
        //public class ApplicationSignInManager : SignInManager<Helpers.Identity.IdentityApplicationUser, string>
        //{
        //    public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
        //        : base(userManager, authenticationManager)
        //    {
        //    }

        //    public override Task<ClaimsIdentity> CreateUserIdentityAsync(Helpers.Identity.IdentityApplicationUser user)
        //    {
        //        return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        //    }

        //    public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        //    {
        //        return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        //    }
        //}
    }
}