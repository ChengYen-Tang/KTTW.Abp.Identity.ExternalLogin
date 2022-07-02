using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp.Identity;

namespace KTTW.Abp.Identity.ExternalLogin.Abstract;
public class ExternalLoginUserWithRoleInfo : ExternalLoginUserInfo
{
    [NotNull]
    public ICollection<string> Roles { get; set; } = Array.Empty<string>();

    public ExternalLoginUserWithRoleInfo([NotNull] string email)
        : base(email) { }
}
