using System.Threading.Tasks;
using GrinderApp.Configuration.Helper;

namespace GrinderApp.Configuration
{
    /// <summary>
    /// 配置信息持久化接口
    /// </summary>
    /// <remarks>取消持久化的目标流，持久化方法自己完成实现</remarks>
    public interface IConfigStore
    {
        /// <summary>
        /// 从数据流中加载 配置文件 对象
        /// </summary>
        /// <returns></returns>
        Task<ConfigData> LoadAsync();


        /// <summary>
        /// 加载配置文件对象
        /// </summary>
        /// <returns></returns>
        ConfigData Load();


        /// <summary>
        /// 配置文件保存至数据流
        /// </summary>
        /// <param name="data">配置信息数据</param>
        /// <returns></returns>
        Task SaveAsync(ConfigData data);


        /// <summary>
        /// 配置文件保存至数据流
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        void Save(ConfigData data);


        /// <summary>
        /// 数据源变更事件
        /// </summary>
        event AsyncEventHandler SourceChanged;
    }
}
