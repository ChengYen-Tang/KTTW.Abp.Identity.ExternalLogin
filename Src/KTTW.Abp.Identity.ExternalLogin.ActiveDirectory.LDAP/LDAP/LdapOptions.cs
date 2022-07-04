namespace KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP.LDAP;
public class LdapOptions
{
    public string ServerHost { get; set; }
    public int ServerPort { get; set; }
    public string SearchBase { get; set; }
    public string DomainName { get; set; }

    public LdapCredentials Credentials { get; set; }

    public LdapOptions()
    {
        Credentials = new LdapCredentials();
    }
}
