namespace ConfigurationEditor.Helper
{
    /// <summary>
    /// 可关闭的窗口, 提供给视图模型调用关闭窗口使用
    /// </summary>
    public interface IDialogWindow
    {
        /// <summary>
        /// 对话框确定关闭
        /// </summary>
        void Ok();

        /// <summary>
        /// 对话框取消关闭
        /// </summary>
        void Cancel();
    }
}
