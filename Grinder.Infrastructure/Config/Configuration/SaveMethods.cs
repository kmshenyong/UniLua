namespace grinder.Configuration
{
    /// <summary>
    /// 刷新至数据库的方法
    /// </summary>
    public enum SaveMethods
    {
        /// <summary>
        /// 当属性更新时，自动更新
        /// </summary>
        PropertyChanged,
        
        /// <summary>
        /// 手动刷新
        /// </summary>
        Manual,
    }
}