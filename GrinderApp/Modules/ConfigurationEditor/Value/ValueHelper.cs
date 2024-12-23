using System;

namespace ConfigurationEditor.Value
{
    public class ValueHelper
    {
        /// <summary>
        /// 获取数据类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataType? GetDataType(object value)
        {
            if (value == null)
                return null;

            switch (value.GetType().Name)
            {
                case nameof(Boolean):
                    return DataType.Boolean;
                case nameof(DateTime):
                    return DataType.DateTime;
                case nameof(Single):
                case nameof(Double):
                case nameof(Decimal):
                    return DataType.Float;
                case nameof(Guid):
                    return DataType.Guid;
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                case nameof(UInt16):
                case nameof(UInt32):
                case nameof(UInt64):
                    return DataType.Int;
                case nameof(String):
                    return DataType.String;
            }

            return DataType.String;
        }

        /// <summary>
        /// 判断输入值是否有效
        /// </summary>
        /// <returns></returns>
        public static bool TryGetValue(string valueText, DataType dataType, out object value)
        {
            try
            {
                switch (dataType)
                {
                    case DataType.String:
                        value = valueText;
                        break;
                    case DataType.DateTime:
                        value = Convert.ToDateTime(valueText);
                        break;
                    case DataType.Boolean:
                        value = Convert.ToBoolean(valueText);
                        break;
                    case DataType.Int:
                        value = Convert.ToInt32(valueText);
                        break;
                    case DataType.Float:
                        value = Convert.ToDouble(valueText);
                        break;
                    case DataType.Guid:
                        value = Guid.Parse(valueText);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }


        /// <summary>
        /// 把值转换为字符串用来显示 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetValueText(object value)
        {
            return value?.ToString();
        }
    }
}
