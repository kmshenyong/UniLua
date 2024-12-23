using System.Linq;
using Serilog;
using System.Collections.Generic;
using Unity;
using GrinderApp.Configuration;

namespace ConfigurationEditor.Browse
{
    public class ConfigDescription : IConfigDescription
    {
        private readonly Dictionary<string, string> _propertyDescriptions = new Dictionary<string, string>();

        public ConfigDescription(IUnityContainer unityContainer)
        {
            // 解析系统的配置参数的属性的描述, 在 ConfigBase 中, 通常都配置一个 DescriptionAttribute, 这个方法把这些 Attribute 解析出来, 显示在UI上
            ResolveConfigPropertyDescriptionAttributes(unityContainer);
        }

        public string GetDescription(string key)
        {
            return _propertyDescriptions.TryGetValue(key, out var value) ? value : "";
        }

        /// <summary>
        /// 解析系统的配置参数的属性的描述, 在 ConfigBase 中, 通常都配置一个 DescriptionAttribute, 这个方法把这些 Attribute 解析出来, 显示在UI上
        /// </summary>
        /// <param name="unityContainer"></param>
        private void ResolveConfigPropertyDescriptionAttributes(IUnityContainer unityContainer)
        {
            var q = (from r in unityContainer.Registrations
                     where typeof(ConfigBase).IsAssignableFrom(r.MappedToType) || typeof(ConfigBase).IsAssignableFrom(r.RegisteredType)
                     select r).ToArray();
            foreach (var item in q)
            {
                try
                {
                    var configBase = unityContainer.Resolve(item.RegisteredType) as ConfigBase;
                    if (configBase != null)
                    {
                        var dict = configBase.GetPropertyDescriptions();
                        foreach (var kv in dict)
                        {
                            _propertyDescriptions[kv.Key] = kv.Value;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Warning(ex, "无法加载配置文件 {Config} 的描述信息", item.MappedToType.Name);
                }
            }
        }

    }
    public interface IConfigDescription
    {
        string GetDescription(string key);
    }
}
