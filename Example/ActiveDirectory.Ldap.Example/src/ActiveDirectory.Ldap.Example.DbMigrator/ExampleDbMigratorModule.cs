using ActiveDirectory.Ldap.Example.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace ActiveDirectory.Ldap.Example.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ExampleEntityFrameworkCoreModule),
    typeof(ExampleApplicationContractsModule)
    )]
public class ExampleDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
