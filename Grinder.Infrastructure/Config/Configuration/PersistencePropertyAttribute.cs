using System;

namespace grinder.Configuration
{
    /// <summary>
    /// 需要持久化的设置参数特性头声明
    /// </summary>
    public class PersistencePropertyAttribute : Attribute
    {
        public object DefaultValue { get; }

        public PersistencePropertyAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
