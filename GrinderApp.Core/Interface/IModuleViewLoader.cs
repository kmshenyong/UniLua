using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrinderApp.Core.Interface
{
    /// <summary>
    /// 模块视图
    /// </summary>
    public interface IModuleViewLoader : IViewLoader
    {
        /// <summary>
        /// 显示视图
        /// </summary>
        /// <param name="regionName"></param>
     //   void Show(string regionName);

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 图标 todo 没有完全想好支持的格式, 目前支持 MaterialDesign 图标的枚举
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// 默认显示顺序
        /// </summary>
        int DefaultIndex { get; }
    }
}
