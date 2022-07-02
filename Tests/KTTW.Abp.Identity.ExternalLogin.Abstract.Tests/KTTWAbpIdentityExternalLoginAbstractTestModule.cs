using Microsoft.Extensions.DependencyInjection;
using KTTW.Abp.Identity.ExternalLogin.Abstract.Tests.Fake;
using KTTW.Abp.Identity.ExternalLogin.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace KTTW.Abp.Identity.ExternalLogin.Abstract;

[DependsOn(
    typeof(KTTWAbpIdentityExternalLoginEntityFrameworkCoreTestModule)
    )]
public class KTTWAbpIdentityExternalLoginAbstractTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<FakeExternalLoginWithRoleProviderBase>();
    }
}
