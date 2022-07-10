# KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP
[![NuGet (with prereleases)](https://img.shields.io/nuget/v/KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP)](https://www.nuget.org/packages/KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP/)

This package provides some methods to synchronize user information to the local database when we use active directory account login.

## How to use
1. Install this package into the HttpApi.Host project. (created by Abp framework)
1. Add the following to [*.HttpApi.Host/appsettings.json](../../Example/ActiveDirectory.Ldap.Example/src/ActiveDirectory.Ldap.Example.HttpApi.Host/appsettings.json)
    ```
    "ActiveDirectory": {
        "ServerHost": "localhost",
        "ServerPort": 389,
        "Credentials": {
            "AdminDN": "uid=test,ou=users,dc=wimpi,dc=net",
            "Password": "secret"
        },
        "SearchBase": "DC=wimpi,DC=net",
        "DomainName": "wimpi.net"
    }
    ```
1. Add `typeof(ExternalLoginActiveDirectoryModule)` in DependsOn of [*.HttpApi.Host/ExampleHttpApiHostModule.cs](../../Example/ActiveDirectory.Ldap.Example/src/ActiveDirectory.Ldap.Example.HttpApi.Host/ExampleHttpApiHostModule.cs)

## Example
[ActiveDirectory.Ldap.Example](../../Example/ActiveDirectory.Ldap.Example/)
