using FluentResults;
using KTTW.Abp.Identity.ExternalLogin.Abstract;
using KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP.LDAP;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP
{
    public class ExternalLoginProvider : ExternalLoginWithRoleProviderBase, ITransientDependency
    {
        public const string Name = "ActiveDirectory";
        private readonly LdapManager ldapManager;
        private ExternalLoginUserWithRoleInfo userInfo;

        public ExternalLoginProvider(IGuidGenerator guidGenerator, ICurrentTenant currentTenant, IdentityUserManager userManager, IdentityRoleManager roleManager, IIdentityUserRepository identityUserRepository, IOptions<IdentityOptions> identityOptions, LdapManager ldapManager)
            : base(guidGenerator, currentTenant, userManager, roleManager, identityUserRepository, identityOptions)
            => this.ldapManager = ldapManager;

        public override Task<bool> IsEnabledAsync()
            => Task.FromResult(true);

        public override Task<bool> TryAuthenticateAsync(string userName, string plainPassword)
        {
            Result<ExternalLoginUserWithRoleInfo> result = ldapManager.TryAuthenticate(userName, plainPassword);
            if (result.IsFailed)
                return Task.FromResult(false);
            userInfo = result.Value;
            return Task.FromResult(true);
        }

        protected override Task<ExternalLoginUserWithRoleInfo> GetUserInfoAsync(string userName)
            => Task.FromResult(userInfo);
    }
}
