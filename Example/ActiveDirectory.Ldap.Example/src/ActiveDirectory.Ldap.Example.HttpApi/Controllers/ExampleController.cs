using ActiveDirectory.Ldap.Example.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ActiveDirectory.Ldap.Example.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ExampleController : AbpControllerBase
{
    protected ExampleController()
    {
        LocalizationResource = typeof(ExampleResource);
    }
}
