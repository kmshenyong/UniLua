using grinder.Configuration.Helper;

namespace grinder.Configuration.StreamProvider
{
    /// <summary>
    /// 含有通知消息的接口
    /// </summary>
    public interface IStreamProviderWithNotification : IStreamProvider
    {
        /// <summary>
        /// 流更新事件。
        /// 注意：OpenWrite 写入流不会激发该事件，该事件由程序外部激发调用
        /// </summary>
        event AsyncEventHandler StreamSourceChanged;
    }
}
