using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ActiveDirectory.Ldap.Example.Data;
using Volo.Abp.DependencyInjection;

namespace ActiveDirectory.Ldap.Example.EntityFrameworkCore;

public class EntityFrameworkCoreExampleDbSchemaMigrator
    : IExampleDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreExampleDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the ExampleDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<ExampleDbContext>()
            .Database
            .MigrateAsync();
    }
}
