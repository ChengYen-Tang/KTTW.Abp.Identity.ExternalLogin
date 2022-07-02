using Volo.Abp.DependencyInjection;

namespace KTTW.Abp.Identity.ExternalLogin;

public class KTTWAbpIdentityExternalLoginTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        return Task.CompletedTask;
    }
}
