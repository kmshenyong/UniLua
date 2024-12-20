using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

namespace grinder.Configuration
{
    /// <summary>
    /// 这个类负责存储所有的数据，新版本改为扁平化存储，取消树状存储形式
    /// </summary>
    public class ConfigData
    {
        /// <summary>
        /// 存储所有的设置参数
        /// </summary>
        private readonly ConcurrentDictionary<string, ConfigValue> _dict = new();

        /// <summary>
        /// 打开子节点，如果不存在则创建一个
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ConfigValue OpenOrCreateConfigValue(string path)
        {
            ConfigPath.EnsurePathNotEmpty(path);
            return _dict.GetOrAdd(path, _ => new ConfigValue());
        }

        /// <summary>
        /// 打开子节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ConfigValue OpenConfigValue(string path)
        {
            ConfigPath.EnsurePathNotEmpty(path);

            _dict.TryGetValue(path, out var value);
            return value;
        }

        /// <summary>
        /// 移除指定路径的一个参数项，或者一个Section下的所有参数项
        /// </summary>
        /// <param name="path">需要删除的参数的全路径</param>
        /// <returns>删除成功</returns>
        public bool Remove(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            ConfigPath.EnsurePathNotEmpty(path);

            // 找到所有的路径
            var r = new Regex($@"^{path}(\.|$)", RegexOptions.Compiled);
            var q = from key in _dict.Keys
                    where r.IsMatch(key)
                    select key;

            // 删除这些Key对应的项目
            return q.Aggregate(false, (current, key) => current | _dict.TryRemove(key, out _));
        }

        /// <summary>
        /// 更名，把以 <paramref name="originPath"/> 开头的路径更名为 <paramref name="newPath"/>
        /// </summary>
        /// <param name="originPath">原路径</param>
        /// <param name="newPath">新路径</param>
        /// <returns>如果数据发生了更改，返回True，否则返回False</returns>
        public bool Rename(string originPath, string newPath)
        {
            if (originPath == null)
                throw new ArgumentNullException(nameof(originPath));
            if (newPath == null)
                throw new ArgumentNullException(nameof(newPath));

            ConfigPath.EnsurePathNotEmpty(originPath);
            ConfigPath.EnsurePathNotEmpty(newPath);

            // 找到所有的路径
            var r = new Regex($@"^{originPath}(\.|$)", RegexOptions.Compiled);
            var q = from key in _dict.Keys
                    where r.IsMatch(key)
                    select key;

            var hasChanged = false;
            // 执行更名
            foreach (var path in q)
            {
                var newFullPath = Regex.Replace(path, $@"^{originPath}", newPath);

                if (_dict.TryRemove(path, out var value))
                {
                    _dict[newFullPath] = value;

                    hasChanged = true;
                }
            }

            return hasChanged;
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
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            ConfigPath.EnsurePathValidated(path);

            if (string.IsNullOrEmpty(path) == false)
                path += ConfigPath.PathSeparator;

            // 找到所有的路径
            var q = from key in _dict.Keys
                    where key.StartsWith(path)
                    let cp = new ConfigPath(key.Substring(path.Length))
                    where hasMoreSubsequentPath.HasValue == false || cp.HasMoreSubsequentPath == hasMoreSubsequentPath
                    orderby cp.Current
                    select cp.Current;

            return q.Distinct().ToArray();
        }

        /// <summary>
        /// 清空所有保存的配置信息
        /// </summary>
        public bool Clear()
        {
            if (_dict.Any())
            {
                _dict.Clear();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 从另一个配置存储合并配置参数
        /// </summary>
        /// <param name="second">另一个配置存储</param>
        /// <param name="overrideExists">如果出现路径重复,是否覆盖当前对象的数据</param>
        public bool MergeWith(ConfigData second, bool overrideExists)
        {
            var changed = false;

            foreach (var keyValue in second._dict)
            {
                if (!overrideExists && _dict.ContainsKey(keyValue.Key))
                    continue;

                var node = OpenOrCreateConfigValue(keyValue.Key);
                if (!Equals(node.Value, keyValue.Value.Value))
                {
                    node.Value = keyValue.Value.Value;
                    changed    = true;
                }
            }

            return changed;
        }
    }
}
