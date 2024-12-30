using System.Threading.Tasks;
using System.Windows;
using System;

namespace ConfigurationEditor.Helper
{
    /// <summary>
    /// 消息框接口
    /// </summary>
    public interface IMessageBox
    {
        /// <summary>
        /// 显示消息对话框
        /// </summary>
        /// <returns></returns>
        Task<MessageBoxResult> ShowAsync(string text, MessageBoxSetting settingAction = null);
        /// <summary>
        /// 显示一个问题让用户选择
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
      //  bool ShowQuestion(string text, string title);

        /// <summary>
        /// 显示一个等待用户确认询问
        /// </summary>
        public async Task<bool?> ShowQuestionAsync(string text, Action<MessageBoxSetting> settingAction = null)
        {
            var option = new MessageBoxSetting
            {
                Title = "请选择",
                Button = MessageBoxButton.YesNo,
                Icon = MessageBoxImage.Question,
            };
            settingAction?.Invoke(option);

            if (option.Button == MessageBoxButton.OK)
                throw new NotSupportedException();

            var result = await ShowAsync(text, option);
            return result switch
            {
                MessageBoxResult.None => null,
                MessageBoxResult.OK => true,
                MessageBoxResult.Cancel => null,
                MessageBoxResult.Yes => true,
                MessageBoxResult.No => false,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

    };
}
