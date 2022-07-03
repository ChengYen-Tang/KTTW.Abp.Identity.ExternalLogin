# KTTW.Abp.Identity.ExternalLogin.Abstract
[![NuGet (with prereleases)](https://img.shields.io/nuget/v/KTTW.Abp.Identity.ExternalLogin.Abstract)](https://www.nuget.org/packages/KTTW.Abp.Identity.ExternalLogin.Abstract/)

This package provides some methods to synchronize user information to the local database when we use external login.

# Introduce
|Class|Description|
|---|---|
|[ExternalLoginWithRoleProviderBase](./ExternalLoginWithRoleProviderBase.cs)|After successful verification, add or update user information and user roles|

# How to use
Example: [FakeExternalLoginWithRoleProviderBase.cs](../../Tests/KTTW.Abp.Identity.ExternalLogin.Abstract.Tests/Fake/FakeExternalLoginWithRoleProviderBase.cs)
1. Inherit the desired class
1. Implement the necessary methods, you can also override virtual functions
