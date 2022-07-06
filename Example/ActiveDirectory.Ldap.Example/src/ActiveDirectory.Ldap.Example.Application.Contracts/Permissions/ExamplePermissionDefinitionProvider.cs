using ActiveDirectory.Ldap.Example.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ActiveDirectory.Ldap.Example.Permissions;

public class ExamplePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ExamplePermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(ExamplePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ExampleResource>(name);
    }
}
