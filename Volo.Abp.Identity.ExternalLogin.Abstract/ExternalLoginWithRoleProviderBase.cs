using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Data.HashFunction.MurmurHash;
using System.Data.HashFunction.xxHash;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Identity.ExternalLogin.Abstract;

public abstract class ExternalLoginWithRoleProviderBase : IExternalLoginProvider
{
    protected IGuidGenerator GuidGenerator { get; }
    protected ICurrentTenant CurrentTenant { get; }
    protected IIdentityUserRepository IdentityUserRepository { get; }
    protected IOptions<IdentityOptions> IdentityOptions { get; }
    protected IdentityUserManager UserManager { get; }
    protected IdentityRoleManager RoleManager { get; }
    protected ExternalLoginWithRoleProviderBase(
    IGuidGenerator guidGenerator,
    ICurrentTenant currentTenant,
    IdentityUserManager userManager,
    IdentityRoleManager roleManager,
    IIdentityUserRepository identityUserRepository,
    IOptions<IdentityOptions> identityOptions)
        => (GuidGenerator, CurrentTenant, IdentityUserRepository, IdentityOptions, UserManager, RoleManager)
        = (guidGenerator, currentTenant, identityUserRepository, identityOptions, userManager, roleManager);

    protected virtual async Task<IdentityUser> CreateUserAsync(ExternalLoginUserWithRoleInfo externalUser, string userName, string providerName)
    {
        NormalizeExternalLoginUserInfo(externalUser, userName);

        var user = new IdentityUser(
            GuidGenerator.Create(),
            userName,
            externalUser.Email,
            tenantId: CurrentTenant.Id
        )
        {
            Name = externalUser.Name,
            Surname = externalUser.Surname,

            IsExternal = true
        };

        user.SetEmailConfirmed(externalUser.EmailConfirmed ?? false);
        user.SetPhoneNumber(externalUser.PhoneNumber, externalUser.PhoneNumberConfirmed ?? false);

        (await UserManager.CreateAsync(user)).CheckErrors();

        if (externalUser.TwoFactorEnabled != null)
            (await UserManager.SetTwoFactorEnabledAsync(user, externalUser.TwoFactorEnabled.Value)).CheckErrors();

        (await UserManager.AddDefaultRolesAsync(user)).CheckErrors();
        (await UserManager.AddLoginAsync(
                    user,
                    new UserLoginInfo(
                        providerName,
                        externalUser.ProviderKey,
                        providerName
                    )
                )
            ).CheckErrors();

        await AddToRoles(user, externalUser.Roles);

        return user;
    }

    protected virtual async Task UpdateUserAsync(IdentityUser user, ExternalLoginUserWithRoleInfo externalUser, string providerName)
    {
        NormalizeExternalLoginUserInfo(externalUser, user.UserName);

        if (!externalUser.Name.IsNullOrWhiteSpace())
            user.Name = externalUser.Name;

        if (!externalUser.Surname.IsNullOrWhiteSpace())
            user.Surname = externalUser.Surname;

        if (user.PhoneNumber != externalUser.PhoneNumber)
        {
            if (!externalUser.PhoneNumber.IsNullOrWhiteSpace())
            {
                await UserManager.SetPhoneNumberAsync(user, externalUser.PhoneNumber);
                user.SetPhoneNumberConfirmed(externalUser.PhoneNumberConfirmed == true);
            }
        }
        else
        {
            if (!user.PhoneNumber.IsNullOrWhiteSpace() &&
                user.PhoneNumberConfirmed == false &&
                externalUser.PhoneNumberConfirmed == true)
            {
                user.SetPhoneNumberConfirmed(true);
            }
        }

        if (!string.Equals(user.Email, externalUser.Email, StringComparison.OrdinalIgnoreCase))
        {
            (await UserManager.SetEmailAsync(user, externalUser.Email)).CheckErrors();
            user.SetEmailConfirmed(externalUser.EmailConfirmed ?? false);
        }

        if (externalUser.TwoFactorEnabled != null)
            (await UserManager.SetTwoFactorEnabledAsync(user, externalUser.TwoFactorEnabled.Value)).CheckErrors();

        await IdentityUserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins);

        var userLogin = user.Logins.FirstOrDefault(l => l.LoginProvider == providerName);
        if (userLogin != null)
        {
            if (userLogin.ProviderKey != externalUser.ProviderKey)
            {
                (await UserManager.RemoveLoginAsync(user, providerName, userLogin.ProviderKey)).CheckErrors();
                (await UserManager.AddLoginAsync(user, new UserLoginInfo(providerName, externalUser.ProviderKey, providerName))).CheckErrors();
            }
        }
        else
            (await UserManager.AddLoginAsync(user, new UserLoginInfo(providerName, externalUser.ProviderKey, providerName))).CheckErrors();

        user.IsExternal = true;

        (await UserManager.UpdateAsync(user)).CheckErrors();

        // Update user roles
        IList<string> currentRoleNames = await UserManager.GetRolesAsync(user);
        string[] removeRoles = currentRoleNames.Except(externalUser.Roles).Distinct().ToArray();
        if (removeRoles.Any())
            await UserManager.RemoveFromRolesAsync(user, removeRoles);

        string[] addRoles = externalUser.Roles.Except(currentRoleNames).Distinct().ToArray();
        if (addRoles.Any())
            await AddToRoles(user, addRoles);
    }

    public abstract Task<bool> TryAuthenticateAsync(string userName, string plainPassword);

    public abstract Task<bool> IsEnabledAsync();

    public virtual async Task<IdentityUser> CreateUserAsync(string userName, string providerName)
    {
        await IdentityOptions.SetAsync();

        var externalUser = await GetUserInfoAsync(userName);

        return await CreateUserAsync(externalUser, userName, providerName);
    }

    public virtual async Task UpdateUserAsync(IdentityUser user, string providerName)
    {
        await IdentityOptions.SetAsync();

        var externalUser = await GetUserInfoAsync(user);

        await UpdateUserAsync(user, externalUser, providerName);
    }

    protected abstract Task<ExternalLoginUserWithRoleInfo> GetUserInfoAsync(string userName);

    protected virtual Task<ExternalLoginUserWithRoleInfo> GetUserInfoAsync(IdentityUser user)
    {
        return GetUserInfoAsync(user.UserName);
    }

    private async Task AddToRoles(IdentityUser user, ICollection<string> roles)
    {
        await CreateRolesAsync(roles);
        await UserManager.AddToRolesAsync(user, roles);
    }

    private async Task CreateRolesAsync(ICollection<string> roles)
    {
        foreach (string roleName in roles)
            await RoleManager.CreateAsync(new IdentityRole(HashStringToGuidAsync(roleName).Result, roleName));
    }

    private static async Task<Guid> HashStringToGuidAsync(string input)
    {
        Task<byte[]> murmurHashResult = ComputeMurmurHash(input);
        Task<byte[]> xxHashResult = ComputexxHash(input);
        byte[][] hashResult = await Task.WhenAll(murmurHashResult, xxHashResult);
        return Double64BitToGuid(hashResult);
    }

    private static Guid Double64BitToGuid(byte[][] bytes)
    {
        byte[] combined = new byte[16];
        Buffer.BlockCopy(bytes[0], 0, combined, 0, 8);
        Buffer.BlockCopy(bytes[1], 0, combined, 8, 8);
        return new Guid(combined);
    }

    private static async Task<byte[]> ComputeMurmurHash(string input)
    {
        return await Task.Run(() => {
            IMurmurHash2Config hashConfig = new MurmurHash2Config() { HashSizeInBits = 64, Seed = 0 };
            IMurmurHash2 murmurHash = MurmurHash2Factory.Instance.Create(hashConfig);
            IHashValue hashValue = murmurHash.ComputeHash(input);
            return hashValue.Hash;
        });
    }

    private static async Task<byte[]> ComputexxHash(string input)
    {
        return await Task.Run(() => {
            IxxHashConfig hashConfig = new xxHashConfig() { HashSizeInBits = 64, Seed = 0 };
            IxxHash xxHash = xxHashFactory.Instance.Create(hashConfig);
            IHashValue hashValue = xxHash.ComputeHash(input);
            return hashValue.Hash;
        });
    }

    private static void NormalizeExternalLoginUserInfo(
        ExternalLoginUserInfo externalUser,
        string userName)
    {
        if (externalUser.ProviderKey.IsNullOrWhiteSpace())
            externalUser.ProviderKey = userName;
    }
}
