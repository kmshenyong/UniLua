using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Prism.Mvvm;

namespace ConfigurationEditor.Helper
{
    /// <summary>
    /// 含有错误处理的绑定基类
    /// </summary>
    public class BindableBaseWithDataError : BindableBase, IDataErrorInfo
    {
        /// <summary>
        /// 验证方法
        /// </summary>
        private readonly Dictionary<string, MethodInfo> _validateMethods = new Dictionary<string, MethodInfo>();

        public BindableBaseWithDataError()
        {
            var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttributes(typeof(ValidateForAttribute), true).FirstOrDefault() as ValidateForAttribute;
                if (attr == null)
                    continue;

                if (method.ReturnType != typeof(string))
                    throw new InvalidProgramException("You must declare return as string in method that special ValidateForAttribute");

                _validateMethods.Add(attr.PropertyName, method);
            }
        }

        /// <summary>Gets the error message for the property with the given name.</summary>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        /// <param name="columnName">The name of the property whose error message to get. </param>
        public string this[string columnName]
        {
            get
            {
                MethodInfo method;
                if (_validateMethods.TryGetValue(columnName, out method) == false)
                    return null;

                return method.Invoke(this, null) as string;
            }
        }

        /// <summary>
        /// 判断是否仍有错误
        /// </summary>
        public virtual bool HasInFault
        {
            get
            {
                foreach (var methodsValue in _validateMethods.Values)
                {
                    var result = methodsValue.Invoke(this, null) as string;
                    if (string.IsNullOrEmpty(result) == false)
                        return true;
                }

                return false;
            }
        }

        /// <summary>Gets an error message indicating what is wrong with this object.</summary>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        public virtual string Error
        {
            get
            {
                var sb = new StringBuilder();

                foreach (var methodsValue in _validateMethods.Values)
                {
                    var result = methodsValue.Invoke(this, null) as string;
                    if (string.IsNullOrEmpty(result) == false)
                        sb.AppendLine(result);

                }

                return sb.ToString();
            }
        }
    }
}