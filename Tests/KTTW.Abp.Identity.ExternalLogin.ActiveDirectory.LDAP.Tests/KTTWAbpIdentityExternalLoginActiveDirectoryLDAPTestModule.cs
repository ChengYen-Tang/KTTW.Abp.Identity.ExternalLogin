using KTTW.Abp.Identity.ExternalLogin.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP.Tests;

[DependsOn(
    typeof(KTTWAbpIdentityExternalLoginEntityFrameworkCoreTestModule),
    typeof(ExternalLoginActiveDirectoryModule)
    )]
public class KTTWAbpIdentityExternalLoginActiveDirectoryLDAPTestModule : AbpModule
{
}
