using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrinderApp.Core.Interface
{
    /// <summary>
    /// 视图加载接口
    /// </summary>
    public interface IViewLoader
    {
        /// <summary>
        /// 显示视图
        /// </summary>
        /// <param name="regionName"></param>
        void Show(string regionName);
    }
}
