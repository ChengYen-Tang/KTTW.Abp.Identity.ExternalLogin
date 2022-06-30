using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Identity.ExternalLogin;

public class VoloAbpIdentityExternalLoginTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        return Task.CompletedTask;
    }
}
