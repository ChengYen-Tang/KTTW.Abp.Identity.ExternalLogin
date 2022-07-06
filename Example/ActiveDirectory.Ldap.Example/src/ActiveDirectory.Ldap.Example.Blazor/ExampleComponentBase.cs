using ActiveDirectory.Ldap.Example.Localization;
using Volo.Abp.AspNetCore.Components;

namespace ActiveDirectory.Ldap.Example.Blazor;

public abstract class ExampleComponentBase : AbpComponentBase
{
    protected ExampleComponentBase()
    {
        LocalizationResource = typeof(ExampleResource);
    }
}
