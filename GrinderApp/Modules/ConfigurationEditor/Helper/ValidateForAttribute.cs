using System;

namespace ConfigurationEditor.Helper
{
    /// <summary>
    /// 描述该方法为哪一个属性验证
    /// </summary>
    internal class ValidateForAttribute : Attribute
    {
        public string PropertyName
        {
            get;
        }

        public ValidateForAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}