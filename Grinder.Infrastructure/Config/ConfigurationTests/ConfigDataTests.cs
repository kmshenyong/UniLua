using System;
using FirstLineTamping.Configuration;
using Moq;
using NUnit.Framework;

namespace ConfigurationTests
{
    [TestFixture()]
    public class ConfigDataTests
    {
        /// <summary>
        /// 正常读写
        /// </summary>
        [Test()]
        public void PathSetValue()
        {
            var data = new Config();

            string path = "Employee.Age";
            int    age  = 30;

            // 设置员工的年龄
            data.SetValue(path, age);

            // 读取时，第二个参数为默认值，当读取失败时，返回该默认值
            int actual = data.GetValue(path, 0);

            Assert.AreEqual(age, actual);
        }

        [Test]
        public void TestEmployee()
        {
            // 初始化一个含有一个 Employee 节点的配置对象
            var root     = new Config();
            var employee = root.GetSection("Employee");
            employee.SetValue("Name",       "Nepton");
            employee.SetValue("Sex",        "M");
            employee.SetValue("Age",        38);
            employee.SetValue("DateOfJoin", new DateTime(2019, 2, 26));

            // 读取 employee 节点
            employee = root.GetSection("Employee");
            Assert.IsNotNull(employee);

            // 通过使用 employee 读取,注意使用相对路径
            string name = employee.GetValue("Name", ""); // should return "Nepton"
            int    age  = employee.GetValue("Age",  0);  // should return "38"
            string sex  = employee.GetValue("Sex",  ""); // should return "M"

            Assert.AreEqual("Nepton", name);
            Assert.AreEqual(38,       age);
            Assert.AreEqual("M",      sex);
        }


        /// <summary>
        /// 正常读写
        /// </summary>
        [Test()]
        public void PathSetValueThenGetFromBranch()
        {
            var       data     = new Config();
            const int expected = 100;

            // 分两段设置
            data.SetValue("A.B.C.D.E", expected);

            // 另一种方式分两段读取
            var a  = data.GetSection("A");
            var bc = a.GetSection("B.C");
            var de = bc.GetValue<int>("D.E");

            Assert.AreEqual(expected, de);
        }

        /// <summary>
        /// 正常读写
        /// </summary>
        [Test()]
        public void PathSetValueThenGetFromBranch2()
        {
            var       data     = new Config();
            const int expected = 100;

            // 分两段设置
            data.SetValue("A.B.C.D.E", expected);

            // 另一种方式分两段读取
            var actual = data.GetSection("A.B.C").GetValue("D.E", 0);

            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// 测试无效路径
        /// </summary>
        [Test]
        public void PassIncorrectPath_Throw()
        {
            var data = new Config();

            var invalidPaths = new[]
            {
                "A. .B",
                "",
                ".",
                "A..B",
                "A.",
                ".B",
                "..B",
                null,
            };

            foreach (var path in invalidPaths)
            {
                Assert.That(() => data.SetValue(path, 100), Throws.Exception.AssignableTo<ArgumentException>());
                Assert.That(() => data.GetValue(path, 100), Throws.Exception.AssignableTo<ArgumentException>());
            }
        }

        /// <summary>
        /// 测试属性更改的时候正确调用保存
        /// </summary>
        [Test]
        public void SetValueThenSerializeToStore()
        {
            var mock = new Mock<IConfigStore>();
            var data = new Config(mock.Object)
            {
                SaveMethod = SaveMethods.PropertyChanged
            };

            data.SetValue("A", 100);
            mock.Verify(c => c.SaveAsync(It.IsNotNull<ConfigData>()), Times.Once);
        }

        /// <summary>
        /// 把一个Section读出来，单独修改，ConfigData内数据同步变化
        /// </summary>
        [Test]
        public void ValidateSectionReference()
        {
            var configData = new Config();
            configData.SetValue("Name.LastName", "Liu");

            var nameSection = configData.GetSection("Name");

            nameSection.SetValue("LastName", "Fu");
            Assert.AreEqual(nameSection.GetValue<string>("LastName"), configData.GetValue<string>("Name.LastName"));

            var name2Section = configData.GetSection("Name");
            name2Section.SetValue("LastName", "Hang");
            Assert.AreEqual(nameSection.GetValue<string>("LastName"), name2Section.GetValue<string>("LastName"));
        }
    }
}
