using Volo.Abp.Settings;

namespace ActiveDirectory.Ldap.Example.Settings;

public class ExampleSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(ExampleSettings.MySetting1));
    }
}
