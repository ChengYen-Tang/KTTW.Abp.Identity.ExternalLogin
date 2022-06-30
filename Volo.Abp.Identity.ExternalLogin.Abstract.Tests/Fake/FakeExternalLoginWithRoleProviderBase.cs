using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Identity.ExternalLogin.Abstract.Tests.Fake
{
    public class FakeExternalLoginWithRoleProviderBase : ExternalLoginWithRoleProviderBase
    {
        public FakeExternalLoginWithRoleProviderBase(IGuidGenerator guidGenerator, ICurrentTenant currentTenant, IdentityUserManager userManager, IdentityRoleManager roleManager, IIdentityUserRepository identityUserRepository, IOptions<IdentityOptions> identityOptions)
            : base(guidGenerator, currentTenant, userManager, roleManager, identityUserRepository, identityOptions)
        {
        }

        public ExternalLoginUserWithRoleInfo ExternalLoginUserWithRoleInfo { get; set; }

        public override Task<bool> IsEnabledAsync()
            => throw new NotImplementedException();

        public override Task<bool> TryAuthenticateAsync(string userName, string plainPassword)
            => throw new NotImplementedException();

        protected override Task<ExternalLoginUserWithRoleInfo> GetUserInfoAsync(string userName)
            => Task.FromResult(ExternalLoginUserWithRoleInfo);
    }
}
