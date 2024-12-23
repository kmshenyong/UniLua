using ConfigurationEditor.Helper;
using ConfigurationEditor.Value;
using Prism.Mvvm;
 

namespace ConfigurationEditor.Browse
{
    /// <summary>
    /// 配置参数
    /// </summary>
    public class ConfigValueItem : BindableBase
    {
        public ConfigValueItem(ConfigSectionItem section, string key, IMessageBox messageBox, IConfigDescription configDescription )
        {
            Key         = key;
            _section    = section;
            _messageBox = messageBox;
            _configDescription = configDescription ?? throw new System.ArgumentNullException(nameof(configDescription));

        }

        public void Update()
        {
            RaisePropertyChanged(null);
        }


        /// <summary>
        /// 保存该值的Section
        /// </summary>
        private readonly ConfigSectionItem _section;

        private readonly IMessageBox _messageBox;
        private readonly IConfigDescription _configDescription;

        /// <summary>
        /// 参数 Key
        /// </summary>
        public string Key
        {
            get;
            set;
        }


        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType? Type
        {
            get
            {
                var value = _section.Section.GetValue<object>(Key);
                var type  = ValueHelper.GetDataType(value);

                return type;
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        public string ValueText
        {
            get
            {
                var value     = _section.Section.GetValue<object>(Key);
                var valueText = ValueHelper.GetValueText(value);
                return valueText;
            }
        }
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description => _configDescription.GetDescription(_section.Section.GetFullPath(Key));


        /// <summary>
        /// 当前是否选中
        /// </summary>
        private bool _bSelected;
        public bool Selected
        {
            get { return _bSelected; }
            set { SetProperty(ref _bSelected, value); }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}, {nameof(Type)}: {Type}, {nameof(ValueText)}: {ValueText}, {nameof(Selected)}: {Selected}";
        }
    }
}
