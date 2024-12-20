namespace grinder.Configuration
{
    /// <summary>
    /// 接口负责把默认的配置文件参数持久化（这样一来用户可以直接修改被持久化的文件，提高效率）
    /// </summary>
    public interface IDefaultConfigWriter
    {
        /// <summary>
        /// 把默认值持久化写入
        /// </summary>
        /// <returns></returns>
        void WriteDefaultValue();
    }
}
