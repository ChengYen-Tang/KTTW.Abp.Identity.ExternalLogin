using JetBrains.Annotations;
using System.Collections.Generic;

namespace Volo.Abp.Identity.ExternalLogin.Abstract;
public class ExternalLoginUserWithRoleInfo : ExternalLoginUserInfo
{
    [NotNull]
    public ICollection<string> Roles { get; set; }

    public ExternalLoginUserWithRoleInfo([NotNull] string email)
        : base(email) { }
}
