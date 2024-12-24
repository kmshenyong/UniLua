using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrinderApp.Core.Mvvm
{

    /// <summary>
    /// 激发该属性 Changed 的观察目标，如果目标Changed ，声明该特性的属性同时激发 Changed
    /// 通常, 我们使用 PropertyChanged.Fody 实现变化跟踪, 但是如果要跟踪的属性在基类中的话, PropertyChanged.Fody 就无能为力了.
    /// 因此, 我们使用 ObservesProperty 特性头, 作为补充, 实现功能
    ///
    /// How to use
    /// 在属性头上声明 ObservesProperty 要跟踪的属性即可
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ObservesPropertyAttribute : Attribute
    {
        public string PropertyName
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName">观察目标，如果目标更改，那么该属性也以激发更改</param>
        public ObservesPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }

}
