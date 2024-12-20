using CommonServiceLocator;
using FirstLineTamping.Configuration;
using Moq;
using FirstLineTamping.Contract.Authorities;
using NUnit.Framework;
using TampingDbService.Entities;
using TampingDbService.Entities.Users;
using Unity;

namespace ConfigurationTests
{
    [TestFixture]
    public class PermissionTest
    {
        /// <summary>
        /// 含有权限的读写, 允许操作
        /// </summary>
        [Test]
        public void AuthorityPermitTest()
        {
            var admin = new User("admin", "", new UserAuthority(Authority.All));

            var loginUserSessionMock = new Mock<ILoginUserSession>();
            loginUserSessionMock.Setup(c => c.GetLoginUser()).Returns(admin);

            // register service locator
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(c => c.GetInstance<ILoginUserSession>()).Returns(loginUserSessionMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            var cs   = new Config();
            var path = "Employee.Name";
            cs.SetAuthority(path, new ObjectAuthority(Authority.Anonymous), new ObjectAuthority(Authority.Anonymous));

            var nepton = "Nepton";
            cs.SetValue(path, nepton);
            var actual = cs.GetValue(path, "");

            Assert.AreEqual(nepton, actual);
        }


        /// <summary>
        /// 含有权限的读写，拒绝，并抛异常
        /// </summary>
        [Test]
        public void AuthorityDenyTest()
        {
            var baby = new User("admin", "", new UserAuthority(Authority.None));

            var loginUserSessionMock = new Mock<ILoginUserSession>();
            loginUserSessionMock.Setup(c => c.GetLoginUser()).Returns(baby);

            // register service locator
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(c => c.GetInstance<ILoginUserSession>()).Returns(loginUserSessionMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            var cs   = new Config();
            var path = "Employee.Name";
            cs.SetAuthority(path, new ObjectAuthority(Authority.Anonymous), new ObjectAuthority(Authority.Anonymous));

            var nepton = "Nepton";
            Assert.Throws<NotAuthorizedException>(() => cs.SetValue(path, nepton));
            Assert.Throws<NotAuthorizedException>(() => cs.GetValue(path, ""));
        }


        /// <summary>
        /// 含有权限的读写，拒绝，并抛异常
        /// </summary>
        [Test]
        public void AuthorityReadOnlyTest()
        {
            var user = new User("test", "", new UserAuthority(Authority.Anonymous));

            var loginUserSessionMock = new Mock<ILoginUserSession>();
            loginUserSessionMock.Setup(c => c.GetLoginUser()).Returns(user);

            // register service locator
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(c => c.GetInstance<ILoginUserSession>()).Returns(loginUserSessionMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            var cs     = new Config();
            var path   = "Employee.Name";
            var nepton = "Nepton";
            cs.SetValue(path, nepton);

            cs.SetAuthority(path, new ObjectAuthority(Authority.Anonymous), new ObjectAuthority(Authority.Engineer));
            Assert.Throws<NotAuthorizedException>(() => cs.SetValue(path, "Test123"));

            var actual = cs.GetValue(path, "");
            Assert.AreEqual(nepton, actual);
        }
    }
}
