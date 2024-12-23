using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GrinderApp.Configuration
{
    /// <summary>
    /// 这个类负责保存配置信息数据，意味着一个配置信息分组
    /// </summary>
    public class ConfigSection
    {
        private readonly Config _config;

        private readonly string _pathPrefix;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public ConfigSection(Config config, string pathPrefix)
        {
            _config     = config;
            _pathPrefix = pathPrefix;
        }
        public string GetFullPath(string path)
        {
            return ConfigPath.CombinePath(_pathPrefix, path);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="expectedType">配置参数值的数据类型</param>
        /// <param name="defaultValue">默认值，如果值不存在返回默认值</param>
        /// <returns></returns>
        public object GetValue(string path, Type expectedType, object defaultValue = default)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));

            path = ConfigPath.CombinePath(_pathPrefix, path);

            return _config.GetValue(path, expectedType, defaultValue);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TValue">要获取的值的类型</typeparam>
        /// <param name="path">路径</param>
        /// <param name="defaultValue">默认值，如果值不存在返回默认值</param>
        /// <returns></returns>
        public TValue GetValue<TValue>(string path, TValue defaultValue = default)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));

            path = ConfigPath.CombinePath(_pathPrefix, path);

            return _config.GetValue(path, defaultValue);
        }

        /// <summary>
        /// 获取指定Key对应的子Section对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ConfigSection GetSection(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));

            path = ConfigPath.CombinePath(_pathPrefix, path);

            return _config.GetSection(path);
        }

        /// <summary>
        /// 获取所有子 Section
        /// </summary>
        /// <returns></returns>
        public string[] GetChildrenNodes(bool? hasMoreSubsequentPath = null)
        {
            return _config.GetChildrenNodes(_pathPrefix, hasMoreSubsequentPath);
        }

        /// <summary>
        /// 设置配置值
        /// </summary>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="path">路径</param>
        /// <param name="value">默认值</param>
        public void SetValue<TValue>(string path, TValue value)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));

            path = ConfigPath.CombinePath(_pathPrefix, path);

            _config.SetValue(path, value);
        }


        /// <summary>
        /// 删除指定路径的所有项
        /// </summary>
        /// <param name="path"></param>
        public void Remove(string path)
        {
            path = ConfigPath.CombinePath(_pathPrefix, path);

            _config.Remove(path);
        }


        /// <summary>
        /// 执行更名
        /// </summary>
        /// <param name="originName"></param>
        /// <param name="newName"></param>
        public void Rename(string originName, string newName)
        {
            var originPath = ConfigPath.CombinePath(_pathPrefix, originName);
            var newPath    = ConfigPath.CombinePath(_pathPrefix, newName);

            _config.Rename(originPath, newPath);
        }

        #region Convert From / To JObject

        /// <summary>
        /// 转换为JObject
        /// </summary>
        /// <returns></returns>
        public JObject ToJObject()
        {
            return _config.ToJObject(_pathPrefix);
        }

        /// <summary>
        /// 从 JObject 读取值, 然后组合到当前配置中
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public void FromJObject(JObject jObject)
        {
            _config.FromJObject(_pathPrefix, jObject);
        }

        #endregion

        #region Convert From / To Dictionary

        /// <summary>
        /// 生成设置参数字符串
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToDictionary()
        {
            return _config.ToDictionary(_pathPrefix);
        }

        /// <summary>
        /// 从 JObject 读取值, 然后组合到当前配置中
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public void FromDictionary(Dictionary<string, object> dict)
        {
            _config.FromDictionary(_pathPrefix, dict);
        }

        #endregion

        #region Convert From / To Object

        /// <summary>
        /// 从指定的节点装换为一个设置对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="declareType">是否声明子对象类型, (例如含有接口, 抽象类, 则需要传入 true)</param>
        /// <returns></returns>
        public T ToObject<T>(bool declareType = false)
        {
            return _config.ToObject<T>(_pathPrefix, declareType);
        }

        /// <summary>
        /// 从指定的节点装换为一个设置对象
        /// </summary>
        /// <param name="obj">要转换的 obj</param>
        /// <param name="declareType">是否声明子对象类型, (例如含有接口, 抽象类, 则需要传入 true)</param>
        /// <returns></returns>
        public void FromObject(object obj, bool declareType = false)
        {
            _config.FromObject(_pathPrefix, obj, declareType);
        }

        #endregion
    }
}
