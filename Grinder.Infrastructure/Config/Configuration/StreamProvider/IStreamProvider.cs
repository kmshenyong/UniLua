using System.IO;

namespace grinder.Configuration.StreamProvider
{
    /// <summary>
    /// 数据流提供接口，例如内存流，文件流等
    /// </summary>
    public interface IStreamProvider
    {
        /// <summary>
        /// 以读取的方式打开流
        /// </summary>
        /// <returns></returns>
        Stream OpenRead();

        /// <summary>
        /// 为写入打开流
        /// </summary>
        /// <returns></returns>
        Stream OpenWrite();
    }
}
