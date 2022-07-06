using FluentResults;
using KTTW.Abp.Identity.ExternalLogin.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP.LDAP
{
    public class LdapManager : ITransientDependency
    {
        private readonly ILogger logger;
        private readonly LdapOptions ldapOptions;

        public LdapManager(ILogger<LdapManager> logger, IOptions<LdapOptions> ldapSettingsOptions)
            => (this.logger, ldapOptions) = (logger, ldapSettingsOptions.Value);

        public Result<ExternalLoginUserWithRoleInfo> TryAuthenticate(string userName, string password)
        {
            try
            {
                using LdapConnection connection = new();
                connection.Connect(ldapOptions.ServerHost, ldapOptions.ServerPort);
                connection.Bind(ldapOptions.Credentials.AdminDN, ldapOptions.Credentials.Password);

                ILdapSearchResults entities = connection.Search(ldapOptions.SearchBase, LdapConnection.ScopeSub, $"(SAMAccountName={userName})", null, false);
                LdapEntry entry = null;
                LdapAttribute account = null;
                while (entities.HasMore())
                {
                    LdapEntry ldapEntry = entities.Next();
                    account = ldapEntry.GetAttributeSet().ContainsKey("SAMAccountName") ? ldapEntry.GetAttribute("SAMAccountName") : null;
                    if (account != null && account.StringValue == userName)
                    {
                        entry = ldapEntry;
                        break;
                    }
                }
                if (entry is null) return Result.Fail(string.Empty);
                connection.Bind(entry.Dn, password);

                #region After successful login, get user information
                LdapAttribute givenName = entry.GetAttributeSet().ContainsKey("givenName") ? entry.GetAttribute("givenName") : null;
                LdapAttribute surName = entry.GetAttributeSet().ContainsKey("sn") ? entry.GetAttribute("sn") : null;
                LdapAttribute email = entry.GetAttributeSet().ContainsKey("mail") ? entry.GetAttribute("mail") : entry.GetAttributeSet().ContainsKey("userPrincipalName") ? entry.GetAttribute("userPrincipalName") : null;
                LdapAttribute phone = entry.GetAttributeSet().ContainsKey("telephoneNumber") ? entry.GetAttribute("telephoneNumber") : null;
                LdapAttribute groupsDC = entry.GetAttributeSet().ContainsKey("memberOf") ? entry.GetAttribute("memberOf") : null;
                if (email is null)
                {
                    logger.LogError($"User {userName} has no email address");
                    return Result.Fail(string.Empty);
                }
                string[] groups = Array.Empty<string>();
                if (groupsDC != null)
                    groups = groupsDC.StringValueArray.AsParallel().Select(item => GetCN(item)).ToArray();
                ExternalLoginUserWithRoleInfo userInfo = new(email.StringValue)
                {
                    Name = givenName?.StringValue ?? string.Empty,
                    Surname = surName?.StringValue ?? string.Empty,
                    PhoneNumber = phone?.StringValue ?? string.Empty,
                    ProviderKey = $"{entry.Dn}, {account.StringValue}",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    Roles = groups
                };
                #endregion
                return Result.Ok(userInfo);
            }
            catch(Exception ex)
            {
                if (ex.Message != "Invalid Credentials")
                    logger.LogError(ex.Message);
                return Result.Fail(string.Empty);
            }
        }

        public static string GetCN(string str)
        {
            var start = str.ToLower().IndexOf("cn=");
            var end = str.IndexOf(",");
            return str.Substring(start + 3, end - start - 3);
        }
    }
}
