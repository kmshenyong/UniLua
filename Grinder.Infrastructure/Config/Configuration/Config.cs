using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using GrinderApp.Configuration.Helper;

namespace GrinderApp.Configuration
{
    /// <summary>
    /// 配置信息的数据全部保存在该对象
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 配置文件持久化接口
        /// </summary>
        private IConfigStore _configStore;

        /// <summary>
        /// 存储数据
        /// </summary>
        private ConfigData _data = new();

        /// <summary>
        /// 允许一个执行者和一个等待着的队列，这个对象在属性更改自动保存的时候防止大批量执行。
        /// </summary>
        private readonly OneWaiterTaskQueue _oneWaiterTaskQueue = new();

        /// <summary>
        /// 在读取节点时, 如果节点不存在，是否把默认值写入节点
        /// </summary>
        public bool WriteDefaultValueEnable
        {
            get;
            set;
        } = true;

        /// <summary>
        /// 构造函数，含有持久化模块
        /// </summary>
        /// <param name="configStore"></param>
        public Config(IConfigStore configStore)
        {
            SetConfigStore(configStore);
            Load();
        }

        public Config()
        {
        }

        #region 数据变更事件

        /// <summary>
        /// 内存内容变化，写值
        /// </summary>
        private void OnConfigSourceChanged()
        {
            if (SaveMethod != SaveMethods.PropertyChanged || _configStore == null)
                return;

            // 如果大量的保存请求到这儿, 只让一个保存正在执行和一个保存模块等待，提高性能
            _oneWaiterTaskQueue.TryEnqueue(async () =>
            {
                // 滞后一秒执行保存，这样可以让保存执行最低间隔一秒，而且间隔一秒自动保存不会有什么影响
                await SaveAsync();
                await Task.Delay(1000);
            });
        }

        /// <summary>
        /// 数据源变更时，重新加载内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task ConfigStore_SourceChanged(object sender, EventArgs e)
        {
            await LoadAsync();
        }

        #endregion

        #region Load & Save

        /// <summary>
        /// 设置配置文件持久化接口
        /// </summary>
        /// <param name="newStore"></param>
        public void SetConfigStore(IConfigStore newStore)
        {
            if (_configStore == newStore)
                return;

            if (_configStore != null)
                _configStore.SourceChanged -= ConfigStore_SourceChanged;

            _configStore               =  newStore;
            _configStore.SourceChanged += ConfigStore_SourceChanged;
        }

        /// <summary>
        /// 持久化保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await SaveCoreAsync(true);
        }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <returns></returns>
        public async Task LoadAsync()
        {
            await LoadCoreAsync(true);
        }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        public void Load()
        {
            LoadCore(true);
        }

        /// <summary>
        /// 持久化保存-内部执行
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private async Task SaveCoreAsync(bool throwWhenStoreNotReady)
        {
            var configStore = _configStore;

            if (configStore == null)
            {
                if (throwWhenStoreNotReady)
                    throw new InvalidOperationException($"{nameof(IConfigStore)} is required");

                return;
            }

            await configStore.SaveAsync(_data);
        }

        /// <summary>
        /// 持久化保存参数另存为, 这个操作不应影响内部默认的 Store
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsAsync(IConfigStore store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            await store.SaveAsync(_data);
        }

        /// <summary>
        /// 从指定的 Store 加载配置, 这个操作不应影响内部默认的 Store
        /// </summary>
        /// <returns></returns>
        public async Task LoadFromAsync(IConfigStore store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            _data = await store.LoadAsync() ?? new ConfigData();
        }

        /// <summary>
        /// 加载配置-内部调用
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private async Task LoadCoreAsync(bool throwWhenStoreNotReady)
        {
            var configStore = _configStore;
            if (configStore == null)
            {
                if (throwWhenStoreNotReady)
                    throw new InvalidOperationException($"{nameof(IConfigStore)} is required");

                return;
            }

            _data = await configStore.LoadAsync() ?? new ConfigData();
        }

        /// <summary>
        /// 加载配置-内部调用
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void LoadCore(bool throwWhenStoreNotReady)
        {
            var configStore = _configStore;
            if (configStore == null)
            {
                if (throwWhenStoreNotReady)
                    throw new InvalidOperationException($"{nameof(IConfigStore)} is required");

                return;
            }

            _data = configStore.Load() ?? new ConfigData();
        }

        /// <summary>
        /// 写入文件的方式，通过手动调用 Flush 函数 or 属性更改后自动刷入
        /// </summary>
        public SaveMethods SaveMethod
        {
            get;
            set;
        } = SaveMethods.PropertyChanged;

        #endregion

        #region Sections

        /// <summary>
        /// 获取当前根节点
        /// </summary>
        /// <returns></returns>
        public ConfigSection GetSection()
        {
            return new(this, "");
        }

        /// <summary>
        /// 获取一个指定的子节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ConfigSection GetSection(string path)
        {
            return new(this, path);
        }

        #endregion

        #region GetValue / SetValue

        /// <summary>
        /// 读取值
        /// </summary>
        /// <param name="path">值的路径</param>
        /// <param name="expectedType">期望的类型</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public object GetValue(string path, Type expectedType, object defaultValue = default)
        {
            var item = _data.OpenConfigValue(path);
            if (item != null)
                return item.GetValue(expectedType, defaultValue);

            if (WriteDefaultValueEnable)
            {
                SetValue(path, defaultValue);
            }

            return defaultValue;
        }

        /// <summary>
        /// 读取值
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="path">值的路径</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public TValue GetValue<TValue>(string path, TValue defaultValue = default)
        {
            var item = _data.OpenConfigValue(path);
            if (item != null)
                return item.GetValue(defaultValue);

            if (WriteDefaultValueEnable)
            {
                SetValue(path, defaultValue);
            }

            return defaultValue;
        }

        /// <summary>
        /// 读取值, 如果无法读取(例如路径不存在，无法转换类型)，抛出异常
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="path">值的路径</param>
        /// <returns></returns>
        public TValue GetValueOrThrow<TValue>(string path)
        {
            var item = _data.OpenConfigValue(path);
            if (item != null)
                return item.GetValue<TValue>();

            throw new ArgumentException("No value item in path");
        }


        /// <summary>
        /// 写入值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public bool SetValue<TValue>(string path, TValue value)
        {
            var cv = _data.OpenOrCreateConfigValue(path);
            if (cv == null)
                throw new ArgumentNullException(nameof(cv));

            if (cv.SetValue(value))
            {
                OnConfigSourceChanged();
                return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// 删除指定路径的所有项
        /// </summary>
        /// <param name="path"></param>
        public void Remove(string path)
        {
            if (_data.Remove(path))
                OnConfigSourceChanged();
        }

        /// <summary>
        /// 清空所有配置
        /// </summary>
        public void Clear()
        {
            if (_data.Clear())
                OnConfigSourceChanged();
        }

        /// <summary>
        /// 更名，把以 <paramref name="originPath"/> 开头的路径更名为 <paramref name="newPath"/>
        /// </summary>
        /// <param name="originPath">原路径</param>
        /// <param name="newPath">新路径</param>
        public void Rename(string originPath, string newPath)
        {
            if (_data.Rename(originPath, newPath))
                OnConfigSourceChanged();
        }

        /// <summary>
        /// 从另一个配置文件合并
        /// </summary>
        /// <param name="config"></param>
        /// <param name="overrideExists">是否覆盖已有数据</param>
        public void MergeWith(Config config, bool overrideExists)
        {
            if (_data.MergeWith(config._data, overrideExists))
                OnConfigSourceChanged();
        }

        /// <summary>
        /// 获取给定路径下的所有的第一层子路径的节点
        /// 例如对于如下路径：Order.Create.Name， GetSubPaths("Order", true) 返回 "Create"
        /// </summary>
        /// <param name="path">指定的路径</param>
        /// <param name="hasMoreSubsequentPath">要获取的路径是否拥有后续节点（是否为一个ConfigSection）</param>
        /// <returns></returns>
        public string[] GetChildrenNodes(string path, bool? hasMoreSubsequentPath = null)
        {
            return _data.GetChildrenNodes(path, hasMoreSubsequentPath);
        }

        #region Convert From / To JObject

        /// <summary>
        /// 转换为JObject
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public JObject ToJObject(string path)
        {
            // 转换为一个Dictionary嵌套，然后写入
            var stack      = new Stack<(string Path, JObject JObject)>();
            var referenced = new HashSet<string>(); // 保存已经读取的节点

            var rootDict = new JObject();
            stack.Push((path, rootDict));

            // 递归转换
            while (stack.Any())
            {
                var cur     = stack.Pop();
                var curPath = cur.Path;

                // 检查循环引用
                if (referenced.Contains(curPath))
                    throw new ArgumentException("Circular reference in config section");
                referenced.Add(curPath);

                // 处理值
                var valuePaths = _data.GetChildrenNodes(curPath, false);
                foreach (var valuePath in valuePaths)
                {
                    var fullValuePath = ConfigPath.CombinePath(curPath, valuePath);
                    var cp            = new ConfigPath(valuePath);
                    var configValue   = _data.OpenConfigValue(fullValuePath);

                    var value = configValue.Value;

                    // 处理枚举
                    if (value is Enum e)
                        value = e.ToString();

                    cur.JObject.Add(cp.Current, new JValue(value));
                }

                // 处理节点
                var sectionPaths = _data.GetChildrenNodes(curPath, true);
                foreach (var sectionPath in sectionPaths)
                {
                    var jObject = new JObject();
                    var cp      = new ConfigPath(sectionPath);
                    cur.JObject.Add(cp.Current, jObject);

                    var fullSectionPath = ConfigPath.CombinePath(curPath, sectionPath);
                    stack.Push((fullSectionPath, jObject));
                }
            }

            return rootDict;
        }

        /// <summary>
        /// 从 JObject 读取值, 然后组合到当前配置中
        /// </summary>
        /// <param name="path"></param>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public void FromJObject(string path, JObject jObject)
        {
            // 执行转换
            var stack = new Stack<(JObject JObject, string Path)>();
            stack.Push((jObject, path));
            var referenced = new HashSet<JObject>();

            // 递归执行JObject至ConfigSection的转换
            while (stack.Any())
            {
                var cur = stack.Pop();

                // 引用检查
                if (referenced.Contains(cur.JObject))
                    continue;
                referenced.Add(cur.JObject);

                // 执行本次转换
                foreach (var prop in cur.JObject)
                {
                    var combinePath = ConfigPath.CombinePath(cur.Path, prop.Key);

                    if (prop.Value == null)
                        continue;

                    // 如果是一个 JObject，递归转换下去
                    if (prop.Value is JObject jObjectSub)
                    {
                        stack.Push((jObjectSub, combinePath));
                        continue;
                    }

                    // 处理值
                    if (prop.Value is JValue jValue)
                    {
                        var cv = _data.OpenOrCreateConfigValue(combinePath);
                        cv.Value = jValue.Value;
                        continue;
                    }

                    throw new NotSupportedException($"Object type {prop.Value.GetType().Name} is not supported");
                }
            }
        }

        #endregion

        #region Convert From / To Dictionary

        /// <summary>
        /// 生成设置参数字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<string, object> ToDictionary(string path)
        {
            // 转换为一个Dictionary嵌套，然后写入
            var stack      = new Stack<(string path, Dictionary<string, object> dict)>();
            var referenced = new HashSet<string>(); // 保存已经读取的节点

            var rootDict = new Dictionary<string, object>();
            stack.Push((path, rootDict));

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
                var valuePaths = _data.GetChildrenNodes(curPath, false);
                foreach (var valuePath in valuePaths)
                {
                    var fullValuePath = ConfigPath.CombinePath(curPath, valuePath);
                    var cp            = new ConfigPath(valuePath);
                    var configValue   = _data.OpenConfigValue(fullValuePath);

                    var value = configValue.Value;

                    // 处理枚举
                    if (value is Enum e)
                        value = e.ToString();

                    curDict.Add(cp.Current, value);
                }

                // 处理节点
                var sectionPaths = _data.GetChildrenNodes(curPath, true);
                foreach (var sectionPath in sectionPaths)
                {
                    var dictionary = new Dictionary<string, object>();
                    var cp         = new ConfigPath(sectionPath);
                    curDict.Add(cp.Current, dictionary);

                    var fullSectionPath = ConfigPath.CombinePath(curPath, sectionPath);
                    stack.Push((fullSectionPath, dictionary));
                }
            }

            return rootDict;
        }

        /// <summary>
        /// 从 JObject 读取值, 然后组合到当前配置中
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public void FromDictionary(string path, Dictionary<string, object> dict)
        {
            // 执行转换
            var stack = new Stack<(string Path, Dictionary<string, object> Dict)>();
            stack.Push((path, dict));
            var referenced = new HashSet<Dictionary<string, object>>();

            // 递归执行JObject至ConfigSection的转换
            while (stack.Any())
            {
                var cur = stack.Pop();

                // 引用检查
                if (referenced.Contains(cur.Dict))
                    continue;
                referenced.Add(cur.Dict);

                // 执行本次转换
                foreach (var keyValue in cur.Dict)
                {
                    var combinePath = ConfigPath.CombinePath(cur.Path, keyValue.Key);

                    if (keyValue.Value == null)
                        continue;

                    // 如果是一个 JObject，递归转换下去
                    if (keyValue.Value is Dictionary<string, object> subDict)
                    {
                        stack.Push((combinePath, subDict));
                        continue;
                    }

                    // 处理值
                    if (keyValue.Value is JValue jValue)
                    {
                        var cv = _data.OpenOrCreateConfigValue(combinePath);
                        cv.Value = jValue.Value;
                        continue;
                    }

                    throw new NotSupportedException($"Object type {keyValue.Value.GetType().Name} is not supported");
                }
            }
        }

        #endregion

        #region Convert From / To Object

        /// <summary>
        /// 从指定的节点装换为一个设置对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="declareType">是否声明子对象类型, (例如含有接口, 抽象类, 则需要传入 true)</param>
        /// <returns></returns>
        public T ToObject<T>(string path, bool declareType = false)
        {
            var jObject = ToJObject(path);

            var setting = new JsonSerializer
            {
                TypeNameHandling = declareType ? TypeNameHandling.Auto : TypeNameHandling.None,
            };

            var obj = jObject.ToObject<T>(setting);
            return obj;
        }

        /// <summary>
        /// 从指定的节点装换为一个设置对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj">要转换的 obj</param>
        /// <param name="declareType">是否声明子对象类型, (例如含有接口, 抽象类, 则需要传入 true)</param>
        /// <returns></returns>
        public void FromObject(string path, object obj, bool declareType = false)
        {
            var setting = new JsonSerializer()
            {
                TypeNameHandling = declareType ? TypeNameHandling.Auto : TypeNameHandling.None
            };
            var jObject = JObject.FromObject(obj, setting);

            FromJObject(path, jObject);
        }

        #endregion
    }
}
