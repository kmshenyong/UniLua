using System;
using System.Linq;

namespace grinder.Configuration
{
    /// <summary>
    /// 该对象负责处理配置路径字符串
    /// </summary>
    public class ConfigPath
    {
        #region Static elements
        /// <summary>
        /// 检查Key 是否合法
        /// </summary>
        /// <param name="newKey"></param>
        /// <returns></returns>
        public static bool IsNodeValidated(string newKey)
        {
            return newKey?.Length > 0 && newKey.Contains(PathSeparator) == false;
        }

        /// <summary>
        /// 确保路径不空
        /// </summary>
        /// <param name="path"></param>
        public static void EnsurePathNotEmpty(string path)
        {
            var nodes = SplitPath(path);
            if (nodes.Any() == false)
                throw new ArgumentException("Path is empty");
        }

        /// <summary>
        /// 确保Path合法，如果有问题，报错
        /// </summary>
        /// <param name="path"></param>
        public static void EnsurePathValidated(string path)
        {
            SplitPath(path);
        }

        /// <summary>
        /// 通过传入的Path，生成路径数组
        /// </summary>
        /// <param name="fullPath">路径字符串</param>
        /// <returns></returns>
        public static string[] SplitPath(string fullPath)
        {
            if (fullPath == null)
                throw new ArgumentNullException(nameof(fullPath));

            if (fullPath == string.Empty)
                return new string[0];

            // 格式化当前的路径
            var paths = fullPath.Split(PathSeparator).Select(c => c.Trim()).ToArray();

            // 组合路径前验证合法性
            if (paths.Any(string.IsNullOrEmpty))
                throw new ArgumentException("Path illegal, some path node is empty or whitespace");

            return paths;
        }

        /// <summary>
        /// 组合路径
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string CombinePath(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            // 过滤无效内容
            paths = paths.Where(c => string.IsNullOrEmpty(c) == false).ToArray();

            // 格式化当前的路径
            paths = paths.SelectMany(c => c.Split(PathSeparator)).Select(c => c.Trim()).ToArray();

            // 组合路径前验证合法性
            if (paths.Any(string.IsNullOrEmpty))
                throw new ArgumentException("Path illegal, some path node is empty or whitespace");

            return string.Join(PathSeparator.ToString(), paths);
        }

        /// <summary>
        /// 路径分隔符
        /// </summary>
        public const char PathSeparator = '.';

        #endregion

        /// <summary>
        /// 路径节点数组
        /// </summary>
        private readonly string[] _paths;

        /// <summary>
        /// 通过路径数组创建对象
        /// </summary>
        /// <param name="path">路径数组，已拆分的路径</param>
        public ConfigPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            _paths = SplitPath(path);
        }

        /// <summary>
        /// 判断路径是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty => _paths.Length == 0;

        /// <summary>
        /// 获取路径头。
        /// 例如 a.b.c 的路径头为 a
        /// </summary>
        /// <returns></returns>
        public string Current
        {
            get
            {
                if (_paths.Length == 0)
                    throw new ArgumentException("Path illegal, this is the empty path");

                return _paths.First();
            }
        }

        /// <summary>
        /// 是否有更多的子节点
        /// </summary>
        public bool HasMoreSubsequentPath => _paths.Length > 1;

        /// <summary>
        /// 获取后续的路径。
        /// 例如 a.b.c 的后续路径为 b.c
        /// </summary>
        /// <returns></returns>
        public string SubsequentPath
        {
            get
            {
                if (_paths.Length == 0)
                    throw new ArgumentException("Path illegal, this is the last node of path");

                var list = _paths.ToList();
                list.RemoveAt(0);

                return CombinePath(list.ToArray());
            }
        }

        /// <summary>
        /// 返回路径
        /// </summary>
        public string[] Paths => _paths;

        
    }
}
