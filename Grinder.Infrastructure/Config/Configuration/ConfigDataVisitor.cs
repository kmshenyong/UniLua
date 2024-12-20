namespace grinder.Configuration
{
    /// <summary>
    /// 用来遍历 ConfigTree 的算法
    /// // TODO 有时间重构 Config 里的三个遍历算法, 剥离遍历和
    /// </summary>
    internal class ConfigDataVisitor
    {
        private readonly ConfigData _data;

        public ConfigDataVisitor(ConfigData data)
        {
            _data = data;
        }

        public void Visit(string path)
        {
        }
    }
}
