using System.IO;
using System.Threading.Tasks;
using FirstLineTamping.Configuration;
using FirstLineTamping.Configuration.Store.Json;
using FirstLineTamping.Configuration.StreamProvider;
using NUnit.Framework;

namespace ConfigStream.JsonTests
{
    [TestFixture()]
    public class JsonConfigStreamTests
    {
        /// <summary>
        /// 先写入，再读出，验证一致
        /// </summary>
        /// <returns></returns>
        [Test()]
        public async Task SaveThenLoadTest()
        {
            var config = new Config();

            var person = config.GetSection();
            person.SetValue("Name", "Nepton");
            person.SetValue("Age",  38);
            person.SetValue("Sex",  "M");
            person.SetValue("City", "Kunming");

            var child = person.GetSection("Child");
            child.SetValue("Name", "doudou");
            child.SetValue("Age",  "6");

            var stream = new MemoryStreamProvider();
            config.SetConfigStore(new JsonConfigStore(stream));

            // 写入
            await config.SaveAsync();

            // 读出
            var newConfig = new Config(new JsonConfigStore(stream));
            await newConfig.LoadAsync();

            // 验证
            Assert.AreEqual(person.GetValue<string>("Name"),   newConfig.GetValue<string>("Name"));
            Assert.AreEqual(person.GetValue<int>("Age"),       newConfig.GetValue<int>("Age"));
            Assert.AreEqual(person.GetValue<char>("Sex"),      newConfig.GetValue<char>("Sex"));
            Assert.AreEqual(person.GetValue<int>("Child.Age"), newConfig.GetValue<int>("Child.Age"));
            Assert.AreEqual(person.GetValue<int>("City"),      newConfig.GetValue<int>("City"));
        }
    }
}
