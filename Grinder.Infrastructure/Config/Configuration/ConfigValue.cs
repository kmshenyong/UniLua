using System;
using System.Reflection.Metadata.Ecma335;

namespace grinder.Configuration
{
    /// <summary>
    /// 配置参数的值
    /// </summary>
    public class ConfigValue
    {
        /// <summary>
        /// 存储的值
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Value}";
        }

        /// <summary>
        /// 读取值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public TValue GetValue<TValue>(TValue defaultValue = default)
        {
            return (TValue) GetValue(typeof(TValue), defaultValue);
        }

        /// <summary>
        /// 读取值
        /// </summary>
        /// <param name="expectedType"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object GetValue(Type expectedType, object defaultValue = default)
        {
            if (expectedType.IsInstanceOfType(Value))
                return Value;

            if (Value == null)
                return defaultValue;

            // 尝试转换类型
            try
            {
                // 枚举的单独处理
                if (expectedType.IsEnum)
                {
                    if (Value is string)
                        return Enum.Parse(expectedType, Value.ToString() ?? string.Empty, false);

                    // ReSharper disable once PossibleInvalidCastException
                    return Value;
                }

                // 处理其他情况
                var result = Convert.ChangeType(Value, expectedType);
                return result;
            }
            catch
            {
                // ignored
            }

            // 保存的值无法读取，返回默认值
            return defaultValue;
        }

        /// <summary>
        /// 获取值，如果无法转换，抛出异常
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue GetValueOrThrow<TValue>()
        {
            if (Value is TValue)
                return (TValue) Value;

            if (Value == null)
            {
                if (typeof(TValue).IsClass)
                    return default(TValue);

                throw new InvalidCastException($"Can not cast null to {typeof(TValue).Name}");
            }

            var result = (TValue) Convert.ChangeType(Value, typeof(TValue));
            return result;
        }


        /// <summary>
        /// 写入值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns>return TRUE if value was changed, otherwise return FALSE</returns>
        public bool SetValue<TValue>(TValue value)
        {
            if (!Equals(value, Value))
            {
                Value = value;

                return true;
            }

            return false;
        }
    }
}
