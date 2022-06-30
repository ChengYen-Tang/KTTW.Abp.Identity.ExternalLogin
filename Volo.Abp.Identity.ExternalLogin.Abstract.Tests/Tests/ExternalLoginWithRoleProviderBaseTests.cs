using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity.ExternalLogin.Abstract.Tests.Fake;
using Volo.Abp.Uow;

namespace Volo.Abp.Identity.ExternalLogin.Abstract.Tests.Tests
{
    [TestClass]
    public class ExternalLoginWithRoleProviderBaseTests : VoloAbpIdentityExternalLoginAbstractTestBase
    {
        private readonly IIdentityUserRepository identityUserRepository;
        private readonly IIdentityRoleRepository identityRoleRepository;
        private readonly IdentityUserManager identityUserManager;
        private readonly FakeExternalLoginWithRoleProviderBase fakeExternalLoginWithRoleProviderBase;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private ExternalLoginUserWithRoleInfo externalLoginUserWithRoleInfo;

        public ExternalLoginWithRoleProviderBaseTests()
        {
            identityUserRepository = GetRequiredService<IIdentityUserRepository>();
            identityRoleRepository = GetRequiredService<IIdentityRoleRepository>();
            identityUserManager = GetRequiredService<IdentityUserManager>();
            fakeExternalLoginWithRoleProviderBase = GetRequiredService<FakeExternalLoginWithRoleProviderBase>();
            unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        }

        [TestInitialize]
        public void Init()
        {
            externalLoginUserWithRoleInfo = new("test@github.com") { Name = "test", PhoneNumber = "123456789" };
        }

        [TestCleanup]
        public async Task Clean()
        {
            using IUnitOfWork uow = unitOfWorkManager.Begin();
            Guid[] usersId = await (await identityUserRepository.GetDbSetAsync()).Select(item => item.Id).ToArrayAsync();
            await identityUserRepository.DeleteManyAsync(usersId);
            Guid[] roleId = await (await identityRoleRepository.GetDbSetAsync()).Select(item => item.Id).ToArrayAsync();
            await identityRoleRepository.DeleteManyAsync(roleId);
            await uow.CompleteAsync();
        }

        [TestMethod]
        public async Task TestCreateUserAsync()
        {
            using IUnitOfWork uow = unitOfWorkManager.Begin();
            fakeExternalLoginWithRoleProviderBase.ExternalLoginUserWithRoleInfo = externalLoginUserWithRoleInfo;
            await fakeExternalLoginWithRoleProviderBase.CreateUserAsync("test", "fake");

            IdentityUser user = await identityUserManager.FindByNameAsync("test");
            await identityUserRepository.EnsureCollectionLoadedAsync(user, u => u.Roles);
            await uow.CompleteAsync();
            Assert.IsNotNull(user);
            Assert.AreEqual(externalLoginUserWithRoleInfo.Name, user.Name);
            Assert.AreEqual(externalLoginUserWithRoleInfo.Email, user.Email);
            Assert.AreEqual(externalLoginUserWithRoleInfo.PhoneNumber, user.PhoneNumber);
            Assert.IsFalse(user.Roles.Any());
        }

        [TestMethod]
        public async Task TestCreateUserAndRolesAsync()
        {
            using IUnitOfWork uow = unitOfWorkManager.Begin();
            externalLoginUserWithRoleInfo.Roles = new string[] { "role1", "role2" };
            fakeExternalLoginWithRoleProviderBase.ExternalLoginUserWithRoleInfo = externalLoginUserWithRoleInfo;
            await fakeExternalLoginWithRoleProviderBase.CreateUserAsync("test", "fake");

            IdentityUser user = await identityUserManager.FindByNameAsync("test");
            await identityUserRepository.EnsureCollectionLoadedAsync(user, u => u.Roles);
            Assert.IsNotNull(user);
            Assert.AreEqual(externalLoginUserWithRoleInfo.Name, user.Name);
            Assert.AreEqual(externalLoginUserWithRoleInfo.Email, user.Email);
            Assert.AreEqual(externalLoginUserWithRoleInfo.PhoneNumber, user.PhoneNumber);
            Assert.AreEqual(2, user.Roles.Count);
            string[] rolesName = (await identityUserManager.GetRolesAsync(user)).ToArray();
            CollectionAssert.AreEqual(externalLoginUserWithRoleInfo.Roles.ToArray(), rolesName);
            await uow.CompleteAsync();
        }

        [TestMethod]
        public async Task TestUpdateUserAsync()
        {
            await TestCreateUserAsync();

            using IUnitOfWork uow = unitOfWorkManager.Begin();
            IdentityUser user = await identityUserManager.FindByNameAsync("test");
            externalLoginUserWithRoleInfo.PhoneNumber = "987654321";
            await fakeExternalLoginWithRoleProviderBase.UpdateUserAsync(user, "fake");
            
            user = await identityUserManager.FindByNameAsync("test");
            Assert.AreEqual(externalLoginUserWithRoleInfo.PhoneNumber, user.PhoneNumber);
            await uow.CompleteAsync();
        }

        [TestMethod]
        public async Task TestUpdateUserRolesAsync()
        {
            await TestCreateUserAndRolesAsync();
            
            using IUnitOfWork uow = unitOfWorkManager.Begin();
            IdentityUser user = await identityUserManager.FindByNameAsync("test");
            (externalLoginUserWithRoleInfo.Roles as string[])![1] = "test7";
            await fakeExternalLoginWithRoleProviderBase.UpdateUserAsync(user, "fake");

            Assert.AreEqual(2, user.Roles.Count);
            string[] rolesName = (await identityUserManager.GetRolesAsync(user)).ToArray();
            CollectionAssert.AreEqual(externalLoginUserWithRoleInfo.Roles.ToArray(), rolesName);
            await uow.CompleteAsync();
        }

        [TestMethod]
        public async Task TestRemoveUserRolesAsync()
        {
            await TestCreateUserAndRolesAsync();

            using IUnitOfWork uow = unitOfWorkManager.Begin();
            IdentityUser user = await identityUserManager.FindByNameAsync("test");
            externalLoginUserWithRoleInfo.Roles = new string[] { "role1" };
            await fakeExternalLoginWithRoleProviderBase.UpdateUserAsync(user, "fake");

            Assert.AreEqual(1, user.Roles.Count);
            string[] rolesName = (await identityUserManager.GetRolesAsync(user)).ToArray();
            CollectionAssert.AreEqual(externalLoginUserWithRoleInfo.Roles.ToArray(), rolesName);

            Assert.AreEqual(3, await identityRoleRepository.GetCountAsync());
            await uow.CompleteAsync();
        }
    }
}