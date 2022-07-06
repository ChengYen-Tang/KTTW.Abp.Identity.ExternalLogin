using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ActiveDirectory.Ldap.Example;

[Dependency(ReplaceServices = true)]
public class ExampleBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Example";
}
