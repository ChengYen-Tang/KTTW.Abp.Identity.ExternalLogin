using System;
using System.Collections.Generic;
using System.Text;
using ActiveDirectory.Ldap.Example.Localization;
using Volo.Abp.Application.Services;

namespace ActiveDirectory.Ldap.Example;

/* Inherit your application services from this class.
 */
public abstract class ExampleAppService : ApplicationService
{
    protected ExampleAppService()
    {
        LocalizationResource = typeof(ExampleResource);
    }
}
