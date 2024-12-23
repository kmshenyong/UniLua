using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GrinderApp.Configuration.Helper;
using GrinderApp.Configuration.StreamProvider;

namespace GrinderApp.Configuration.Store.Json
{
    /// <summary>
    /// JSON 配置文件转换流
    /// </summary>
    public class JsonConfigStore : IConfigStore
    {
        /// <summary>
        /// 配置文件读写流（这个设计到技术资源，例如磁盘, 单独抽出来为了更好的单元测试）
        /// </summary>
        private readonly IStreamProvider _configStream;

        public JsonConfigStore(IStreamProvider configStream)
        {
            if (configStream == null)
                throw new ArgumentNullException(nameof(configStream));

            _configStream = configStream;
        }

        /// <summary>
        /// 数据源由外部更新事件
        /// </summary>
        public event AsyncEventHandler SourceChanged
        {
            add
            {
                if (_configStream is IStreamProviderWithNotification streamProviderWithNotification)
                    streamProviderWithNotification.StreamSourceChanged += value;
            }
            remove
            {
                if (_configStream is IStreamProviderWithNotification streamProviderWithNotification)
                    streamProviderWithNotification.StreamSourceChanged -= value;
            }
        }

        #region Implementation of IConfigStream

        /// <summary>
        /// 从数据流中加载 配置文件 对象
        /// </summary>
        /// <returns></returns>
        public async Task<ConfigData> LoadAsync()
        {
            await using (var stream = _configStream.OpenRead())
            {
                using var reader = new StreamReader(stream);
                // 读取配置信息
                var settingString = await reader.ReadToEndAsync();
                if (string.IsNullOrEmpty(settingString))
                    return new ConfigData();

                return ExtraSettingFromString(settingString);
            }
        }

        /// <summary>
        /// 加载配置文件对象
        /// </summary>
        /// <returns></returns>
        public ConfigData Load()
        {
            using (var stream = _configStream.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                // 读取配置信息
                var settingString = reader.ReadToEnd();
                if (string.IsNullOrEmpty(settingString))
                    return new ConfigData();

                return ExtraSettingFromString(settingString);
            }
        }

        /// <summary>
        /// 配置文件保存至数据流
        /// </summary>
        /// <param name="section">配置信息数据</param>
        /// <returns></returns>
        public async Task SaveAsync(ConfigData section)
        {
            string settingString = GenerateSettingString(section);
            await using (var stream = _configStream.OpenWrite())
            await using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(settingString);
                await writer.FlushAsync();
            }
        }


        /// <summary>
        /// 配置文件保存至数据流
        /// </summary>
        /// <param name="section">配置信息数据</param>
        /// <returns></returns>
        public void Save(ConfigData section)
        {
            string settingString = GenerateSettingString(section);
            using (var stream = _configStream.OpenWrite())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(settingString);
                writer.Flush();
            }
        }

        /// <summary>
        /// 生成设置参数字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GenerateSettingString(ConfigData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // 转换为一个Dictionary嵌套，然后写入

            var stack      = new Stack<Tuple<string, Dictionary<string, object>>>();
            var referenced = new HashSet<string>(); // 保存已经读取的节点

            var rootDict = new Dictionary<string, object>();
            stack.Push(new Tuple<string, Dictionary<string, object>>("", rootDict));

            // 递归转换
            while (stack.Any())
            {
                var cur     = stack.Pop();
                var curPath = cur.Item1;
                var curDict = cur.Item2;

                // 检查循环引用
                if (referenced.Contains(curPath))
                    throw new ArgumentException("Circular reference in config section");
                referenced.Add(curPath);

                // 处理值
                var valuePaths = data.GetChildrenNodes(curPath, false);
                foreach (var valuePath in valuePaths)
                {
                    var path        = ConfigPath.CombinePath(curPath, valuePath);
                    var cp          = new ConfigPath(valuePath);
                    var configValue = data.OpenConfigValue(path);

                    var value = configValue.Value;

                    // 处理枚举
                    if (value is Enum e)
                        value = e.ToString();

                    curDict.Add(cp.Current, value);
                }

                // 处理节点
                var sectionPaths = data.GetChildrenNodes(curPath, true);
                foreach (var sectionPath in sectionPaths)
                {
                    var path = ConfigPath.CombinePath(curPath, sectionPath);

                    var dictionary = new Dictionary<string, object>();
                    var cp         = new ConfigPath(sectionPath);
                    curDict.Add(cp.Current, dictionary);

                    stack.Push(new Tuple<string, Dictionary<string, object>>(path, dictionary));
                }
            }

            // 执行序列化，并写到流中
            var settingString = JsonConvert.SerializeObject(rootDict, Formatting.Indented);
            return settingString;
        }

        #endregion

        /// <summary>
        /// 解析处理设置参数
        /// </summary>
        /// <param name="settingString"></param>
        /// <returns></returns>
        private static ConfigData ExtraSettingFromString(string settingString)
        {
            // 执行转换
            var root = JsonConvert.DeserializeObject(settingString) as JObject;
            var data = new ConfigData();
            var stack = new Stack<Tuple<JObject, string>>(new[]
            {
                new Tuple<JObject, string>(root, "")
            });
            var referenced = new HashSet<JObject>();

            // 递归执行JObject至ConfigSection的转换
            while (stack.Any())
            {
                var cur     = stack.Pop();
                var curItem = cur.Item1;
                var curPath = cur.Item2;

                // 引用检查
                if (referenced.Contains(curItem))
                    continue;
                referenced.Add(curItem);

                // 执行本次转换
                foreach (var item in curItem)
                {
                    var combinePath = ConfigPath.CombinePath(curPath, item.Key);

                    if (item.Value == null)
                        continue;

                    // 如果是一个 JObject，递归转换下去
                    if (item.Value is JObject jObject)
                    {
                        stack.Push(new Tuple<JObject, string>(jObject, combinePath));
                        continue;
                    }

                    // 处理值
                    if (item.Value is JValue jValue)
                    {
                        var cv = data.OpenOrCreateConfigValue(combinePath);
                        cv.Value = jValue.Value;
                        continue;
                    }

                    throw new NotSupportedException($"Object type {item.Value.GetType().Name} is not supported");
                }
            }

            return data;
        }
    }
}
