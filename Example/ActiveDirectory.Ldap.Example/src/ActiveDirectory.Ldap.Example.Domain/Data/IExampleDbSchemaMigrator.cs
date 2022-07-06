using System.Threading.Tasks;

namespace ActiveDirectory.Ldap.Example.Data;

public interface IExampleDbSchemaMigrator
{
    Task MigrateAsync();
}
