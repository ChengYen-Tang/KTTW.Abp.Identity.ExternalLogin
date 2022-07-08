using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Services;
using KTTW.Abp.Identity.ExternalLogin.Abstract;
using PrivateObjectExtension;
using System.Runtime.InteropServices;

namespace KTTW.Abp.Identity.ExternalLogin.ActiveDirectory.LDAP.Tests.Tests
{
#if IgnoreTest
    [Ignore]
#endif
    [TestClass]
    public class ExternalLoginProviderTests : KTTWAbpIdentityExternalLoginActiveDirectoryLDAPTestBase
    {
        private readonly string configsPath;
        private readonly string dockerVolume;
        private readonly string virtualAdConfigPath;
        private readonly IHostService docker;
        private readonly ContainerCreateParams containerParams;
        private readonly ExternalLoginProvider externalLoginProvider;
        private readonly PrivateObject privateObject;

        public ExternalLoginProviderTests()
        {
            configsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ADConfig");
            dockerVolume = Path.Combine(configsPath, "config");
            if (!Directory.Exists(dockerVolume))
                Directory.CreateDirectory(dockerVolume);
            virtualAdConfigPath = Path.Combine(dockerVolume, "users.ldif");
            IList<IHostService> hosts = new Hosts().Discover();
            docker = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");
            string containerConfigPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:/ldap/config" : "/ldap/config";
            containerParams = new()
            {
                PortMappings = new string[] { "389:10389" },
                Name = "virtual-ad",
                Volumes = new string[] { $"{dockerVolume}:{containerConfigPath}" },
                AutoRemoveContainer = true
            };
            externalLoginProvider = GetRequiredService<ExternalLoginProvider>();
            privateObject = new(externalLoginProvider);
        }

        [TestCleanup]
        public void Cleanup()
            => docker.Host.Stop("virtual-ad");

        public void RunDocker(string configName)
        {
            if (File.Exists(virtualAdConfigPath))
                File.Delete(virtualAdConfigPath);
            File.Copy(Path.Combine(configsPath, configName), Path.Combine(dockerVolume, "users.ldif"));
            docker.Host.Run("kenneth850511/ldap-ad-it", containerParams);
            Thread.Sleep(5000);
        }

        [TestMethod]
        public async Task TestLoginSuccessAsync()
        {
            RunDocker("users.ldif");
            Assert.IsTrue(await externalLoginProvider.TryAuthenticateAsync("test", "secret"));
        }

        [TestMethod]
        public async Task TestLoginFailedAsync()
        {
            RunDocker("users.ldif");
            Assert.IsFalse(await externalLoginProvider.TryAuthenticateAsync("test", "secrett"));
        }

        [TestMethod]
        public async Task TestGetUserInfoAsync()
        {
            RunDocker("users.ldif");
            Assert.IsTrue(await externalLoginProvider.TryAuthenticateAsync("test", "secret"));
            ExternalLoginUserWithRoleInfo userInfo = await (privateObject.Invoke("GetUserInfoAsync", "test") as Task<ExternalLoginUserWithRoleInfo>);
            Assert.AreEqual("test@asdf.zxcv", userInfo.Email);
            Assert.AreEqual("gtest", userInfo.Name);
            Assert.AreEqual("stest", userInfo.Surname);
            Assert.AreEqual("123456789", userInfo.PhoneNumber);
            CollectionAssert.AreEqual(new string[] { "testadmin" }, userInfo.Roles.ToArray());
            Assert.IsTrue(userInfo.EmailConfirmed);
            Assert.IsFalse(userInfo.PhoneNumberConfirmed);
            Assert.IsFalse(userInfo.TwoFactorEnabled);
        }

        [TestMethod]
        public async Task TestGetUserInfoNoEmailAsync()
        {
            RunDocker("usersNoEmail.ldif");
            Assert.IsTrue(await externalLoginProvider.TryAuthenticateAsync("test", "secret"));
            ExternalLoginUserWithRoleInfo userInfo = await (privateObject.Invoke("GetUserInfoAsync", "test") as Task<ExternalLoginUserWithRoleInfo>);
            Assert.AreEqual("test@wimpi.net", userInfo.Email);
            Assert.AreEqual("gtest", userInfo.Name);
            Assert.AreEqual("stest", userInfo.Surname);
            Assert.AreEqual("123456789", userInfo.PhoneNumber);
            CollectionAssert.AreEqual(new string[] { "testadmin" }, userInfo.Roles.ToArray());
            Assert.IsTrue(userInfo.EmailConfirmed);
            Assert.IsFalse(userInfo.PhoneNumberConfirmed);
            Assert.IsFalse(userInfo.TwoFactorEnabled);
        }

        [TestMethod]
        public async Task TestGetUserInfoNoNameAsync()
        {
            RunDocker("usersNoName.ldif");
            Assert.IsTrue(await externalLoginProvider.TryAuthenticateAsync("test", "secret"));
            ExternalLoginUserWithRoleInfo userInfo = await (privateObject.Invoke("GetUserInfoAsync", "test") as Task<ExternalLoginUserWithRoleInfo>);
            Assert.AreEqual("test@asdf.zxcv", userInfo.Email);
            Assert.AreEqual(string.Empty, userInfo.Name);
            Assert.AreEqual("stest", userInfo.Surname);
            Assert.AreEqual("123456789", userInfo.PhoneNumber);
            CollectionAssert.AreEqual(new string[] { "testadmin" }, userInfo.Roles.ToArray());
            Assert.IsTrue(userInfo.EmailConfirmed);
            Assert.IsFalse(userInfo.PhoneNumberConfirmed);
            Assert.IsFalse(userInfo.TwoFactorEnabled);
        }

        [TestMethod]
        public async Task TestGetUserInfoNoPhoneNumberAsync()
        {
            RunDocker("usersNoPhoneNumber.ldif");
            Assert.IsTrue(await externalLoginProvider.TryAuthenticateAsync("test", "secret"));
            ExternalLoginUserWithRoleInfo userInfo = await (privateObject.Invoke("GetUserInfoAsync", "test") as Task<ExternalLoginUserWithRoleInfo>);
            Assert.AreEqual("test@asdf.zxcv", userInfo.Email);
            Assert.AreEqual("gtest", userInfo.Name);
            Assert.AreEqual("stest", userInfo.Surname);
            Assert.AreEqual(string.Empty, userInfo.PhoneNumber);
            CollectionAssert.AreEqual(new string[] { "testadmin" }, userInfo.Roles.ToArray());
            Assert.IsTrue(userInfo.EmailConfirmed);
            Assert.IsFalse(userInfo.PhoneNumberConfirmed);
            Assert.IsFalse(userInfo.TwoFactorEnabled);
        }

        [TestMethod]
        public async Task TestGetUserInfoNoRolesAsync()
        {
            RunDocker("usersNoRoles.ldif");
            Assert.IsTrue(await externalLoginProvider.TryAuthenticateAsync("test", "secret"));
            ExternalLoginUserWithRoleInfo userInfo = await (privateObject.Invoke("GetUserInfoAsync", "test") as Task<ExternalLoginUserWithRoleInfo>);
            Assert.AreEqual("test@asdf.zxcv", userInfo.Email);
            Assert.AreEqual("gtest", userInfo.Name);
            Assert.AreEqual("stest", userInfo.Surname);
            Assert.AreEqual("123456789", userInfo.PhoneNumber);
            CollectionAssert.AreEqual(Array.Empty<string>(), userInfo.Roles.ToArray());
            Assert.IsTrue(userInfo.EmailConfirmed);
            Assert.IsFalse(userInfo.PhoneNumberConfirmed);
            Assert.IsFalse(userInfo.TwoFactorEnabled);
        }
    }
}
