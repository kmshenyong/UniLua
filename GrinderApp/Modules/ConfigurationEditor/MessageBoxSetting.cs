using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConfigurationEditor
{

    /// <summary>
    /// 消息框选项
    /// </summary>
    public class MessageBoxSetting
    {
        public string Title { get; set; }

        public MessageBoxButton Button { get; set; }

        public MessageBoxImage Icon { get; set; }

        public string[] ButtonTexts { get; set; }
    }
}
