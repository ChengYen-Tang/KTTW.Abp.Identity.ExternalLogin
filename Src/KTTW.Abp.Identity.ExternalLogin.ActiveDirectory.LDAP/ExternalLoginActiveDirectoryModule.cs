using KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP.LDAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP
{
    public class ExternalLoginActiveDirectoryModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            ConfigureBundles();
            ConfigureUrls(configuration);
        }

        private void ConfigureBundles()
            => Configure<AbpIdentityOptions>(options =>
            {
                options.ExternalLoginProviders.Add<ExternalLoginProvider>(ExternalLoginProvider.Name);
            });
        
        private void ConfigureUrls(IConfiguration configuration)
            => Configure<LdapOptions>(configuration.GetSection(ExternalLoginProvider.Name));
    }
}
