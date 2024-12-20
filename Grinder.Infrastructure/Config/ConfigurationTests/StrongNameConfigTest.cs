using CommonServiceLocator;
using FirstLineTamping.Contract.Authorities;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TampingDbService.Entities;
using TampingDbService.Entities.Users;

namespace ConfigurationTests
{
    [TestFixture]
    public class StrongNameConfigTest
    {
        /// <summary>
        /// 强命名配置测试
        /// </summary>
        [Test]
        public void StrongNameAuthorityConfigTest()
        {
            var loginUserSessionMock = new Mock<ILoginUserSession>();
            loginUserSessionMock.Setup(c => c.GetLoginUser()).Returns(new User("Operate", "", new UserAuthority(Authority.Operator, Authority.None)));

            // register service locator
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(c => c.GetInstance<ILoginUserSession>()).Returns(loginUserSessionMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            var config = new StrongNameConfigTestSample();

            var age = 38;
            Assert.Throws<NotAuthorizedException>(() => config.Age = age);
            Assert.AreEqual(0, config.Age);
        }
    }
}
