using Serilog;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GrinderApp.Configuration
{
    /// <summary>
    /// 强类型命名配置对象基类
    /// 该对象是操作Config的基类
    /// </summary>
    /// <remarks>
    /// 2019-10-16 11:34:51 增加接口 INotifyPropertyChanged TODO 已知问题：1 没有检测变化   2 不是所有的 SetValue 入口都进行了监控
    /// </remarks>
    public abstract class ConfigBase : INotifyPropertyChanged, IDefaultConfigWriter
    {
        private readonly Config _config;

        public Config Config => _config;

        /// <summary>
        /// 仅返回默认值的模式，这个模式给 RestoreDefaultValue 使用，用户调用了 RestoreDefaultValue 后，程序打开这个开关，然后 GetValue....仅返回默认值，而不查询存储的内容。同时 RestoreDefaultValue 函数把读取到的默认值写入磁盘。
        /// </summary>
        private bool _returnDefaultValueMode;

        /// <summary>
        /// 定义作用域(包)
        /// 在多个配置文件合并的时候，用来区分不同的作用域
        /// </summary>
        public abstract string PathName
        {
            get;
        }

        /// <summary>
        /// 构造函数，
        /// </summary>
        /// <param name="config">强命名该分组</param>
        protected ConfigBase(Config config)
        {
            _config = config;
        }




        /// <summary>
        /// 获取配置参数属性的描述信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetPropertyDescriptions()
        {
            var dict = new Dictionary<string, string>();

            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<DescriptionAttribute>(true);
                if (attr == null)
                    continue;

                var path = ConfigPath.CombinePath(PathName, prop.Name);
                dict.Add(path, attr.Description);
            }

            return dict;
        }



        #region Get / Set Value just for Property  :)

        /// <summary>
        /// 获取值, 该函数使用在对属性执行读写操作时
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="defaultValue">如果没有存储该值，返回默认值</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public TValue GetPropertyValue<TValue>(TValue defaultValue, [CallerMemberName] string propertyName = default)
        {
            if (_returnDefaultValueMode)
                return defaultValue;

            // 如果没有指定路径, 返回默认值, 而且不操作存储库
            if (PathName is not { } pathName)
                return defaultValue;

            return _config.GetValue(ConfigPath.CombinePath(pathName, propertyName), defaultValue);
        }


        /// <summary>
        /// 设置属性默认值
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="value">目标值</param>
        /// <param name="propertyName">属性名</param>
        public void SetPropertyValue<TValue>(TValue value, [CallerMemberName] string propertyName = default)
        {
            // 如果没有指定路径, 不操作存储库
            if (PathName is not { } pathName)
                return;

            _config.SetValue(ConfigPath.CombinePath(pathName, propertyName), value);
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// 重命名属性节点
        /// </summary>
        /// <param name="originPropertyName">原属性名</param>
        /// <param name="newPropertyName">属性名</param>
        public void RenameProperty(string originPropertyName, string newPropertyName)
        {
            // 如果没有指定路径, 不操作存储库
            if (PathName is not { } pathName)
                return;

            var originPath = ConfigPath.CombinePath(pathName, originPropertyName);
            var newPath = ConfigPath.CombinePath(pathName, newPropertyName);
            _config.Rename(originPath, newPath);
        }

        #endregion

        /// <summary>
        /// 获取完整路径的值, 如果找不到，返回默认值。
        /// 如果你不需要指定完整路径, 请调用 GetPropertyValue
        /// </summary>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="path">保存配置参数的全路径</param>
        /// <param name="defaultValue">默认值，如果无法读取到内容，返回该值</param>
        /// <returns></returns>
        public TValue GetValue<TValue>(string path, TValue defaultValue)
        {
            if (_returnDefaultValueMode)
                return defaultValue;

            return _config.GetValue(path, defaultValue);
        }

        /// <summary>
        /// 获取指定路径的值，如果无法获取到值，将抛出异常(用于防止路径指定错误)
        /// 如果你不需要指定完整路径, 请调用 GetPropertyValue
        /// </summary>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="path">保存配置参数的全路径</param>
        /// <returns></returns>
        public TValue GetValue<TValue>(string path)
        {
            if (_returnDefaultValueMode)
                return default;

            // todo 考虑抛出异常的合理性，暂时关闭异常抛出的方式
            //return _config.GetValueOrThrow<TValue>(path);
            return _config.GetValue(path, default(TValue));
        }

        /// <summary>
        /// 获取完整路径的值
        /// 如果你不需要指定完整路径, 请调用 SetPropertyValue
        /// </summary>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="path">全路径</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SetValue<TValue>(string path, TValue value)
        {
            return _config.SetValue(path, value);
        }

        /// <summary>
        /// 恢复默认值
        /// </summary>
        public void RestoreDefaultValue()
        {
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var prop in props)
            {
                RestoreDefaultValue(prop);
            }
        }
        public void xr()
        {
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var prop in props)
            {
                try
                {

                    // 仅返回默认值的模式，这个模式给 RestoreDefaultValue 使用，用户调用了 RestoreDefaultValue 后，程序打开这个开关，
                    // 然后 GetValue....仅返回默认值，而不查询存储的内容。同时 RestoreDefaultValue 函数把读取到的默认值写入磁盘。
                    if (prop.CanRead && prop.CanWrite)
                    {
                        var value = prop.GetValue(this);
                        //    prop.SetValue(this, value);
                        Log.Information($"");
                        // Raise property changed
                        OnPropertyChanged(nameof(prop.Name));
                    }
                }
                finally
                {
                    _returnDefaultValueMode = false;
                }
            }
        }

        /// <summary>
        /// 恢复默认值
        /// </summary>
        /// <param name="propertyName"></param>
        public void RestoreDefaultValue(string propertyName)
        {
            var prop = GetType().GetProperty(propertyName);
            if (prop != null)
                RestoreDefaultValue(prop);
        }

        /// <summary>
        /// 恢复默认值
        /// </summary>
        /// <param name="prop"></param>
        public void RestoreDefaultValue(PropertyInfo prop)
        {
            try
            {
                _returnDefaultValueMode = true;

                // 仅返回默认值的模式，这个模式给 RestoreDefaultValue 使用，用户调用了 RestoreDefaultValue 后，程序打开这个开关，
                // 然后 GetValue....仅返回默认值，而不查询存储的内容。同时 RestoreDefaultValue 函数把读取到的默认值写入磁盘。
                if (prop.CanRead && prop.CanWrite)
                {
                    var value = prop.GetValue(this);
                    prop.SetValue(this, value);

                    // Raise property changed
                    OnPropertyChanged(nameof(prop.Name));
                }
            }
            finally
            {
                _returnDefaultValueMode = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 把默认值持久化写入
        /// </summary>
        /// <returns></returns>
        public virtual void WriteDefaultValue()
        {
            // TODO 目前设计不合理，重写该方法，并做相应重构，配置参数读取时不要默认写入. 其中这里应改为 prop.SetValue(prop.GetValue());

            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var prop in props)
            {
                // TODO 判断可读取时去尝试读取加载，并捕获异常，验证这样一来的可靠性
                try
                {
                    // 在读取值的时候，如果发现值不存在，GetValue函数将自动追加一个节点到 ConfigSection 中
                    if (prop.CanRead)
                        prop.GetValue(this);
                }
                catch
                {
                    // no code here
                }
            }
        }
    }
}
