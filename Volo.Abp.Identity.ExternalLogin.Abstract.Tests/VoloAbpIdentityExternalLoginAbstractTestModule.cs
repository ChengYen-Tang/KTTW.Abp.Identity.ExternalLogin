using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Identity.ExternalLogin.Abstract.Tests.Fake;
using Volo.Abp.Identity.ExternalLogin.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Volo.Abp.Identity.ExternalLogin.Abstract;

[DependsOn(
    typeof(VoloAbpIdentityExternalLoginEntityFrameworkCoreTestModule)
    )]
public class VoloAbpIdentityExternalLoginAbstractTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<FakeExternalLoginWithRoleProviderBase>();
    }
}
