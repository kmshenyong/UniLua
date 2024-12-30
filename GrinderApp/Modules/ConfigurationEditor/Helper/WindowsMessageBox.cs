using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ConfigurationEditor.Helper
{
    /// <summary>
    /// Windows 标准的消息框
    /// </summary>
    public class WindowsMessageBox : IMessageBox
    {
        /// <summary>
        /// 显示消息对话框
        /// </summary>
        /// <returns></returns>
        public Task<MessageBoxResult> ShowAsync(string text, MessageBoxSetting setting = null)
        {
            setting ??= new MessageBoxSetting
            {
                Title = "提示",
                Button = MessageBoxButton.OK,
                Icon = MessageBoxImage.None,
            };

            MessageBoxResult ShowAction() =>
                System.Windows.MessageBox.Show(
                    text,
                    setting.Title,
                    setting.Button,
                    setting.Icon);

            return Task.Factory.StartNew(ShowAction,
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
