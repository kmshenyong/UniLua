using System;
using System.Diagnostics;
using System.Linq;
using ConfigurationEditor.Helper;
using ConfigurationEditor.Value;
using GrinderApp.Configuration;
using Prism.Commands; //using FirstLineTamping.Infrastructure.UI.ViewModels;

namespace ConfigurationEditor.ValueEdit
{
    /// <summary>
    /// 编辑的配置数据模型
    /// </summary>
    public class ValueEditWidowViewModel : BindableBaseWithDataError
    {
        /// <summary>e
        /// 创建节点
        /// </summary>
        /// <param name="section"></param>
        public void CreateSection(ConfigSection section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            _section = section;
            Key      = "";

            NameFocused = true;
        }

        private bool _nameFocused;

        public bool NameFocused
        {
            get => _nameFocused;
            set => SetProperty(ref _nameFocused, value);
        }

        private bool _valueFocused;

        public bool ValueFocused
        {
            get => _valueFocused;
            set => SetProperty(ref _valueFocused, value);
        }

        /// <summary>
        /// 修改节点
        /// </summary>
        /// <param name="section">持有这个Section的parent</param>
        /// <param name="key">需要编辑的 SectionName</param>
        public void EditConfigSection(ConfigSection section, string key)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (section.GetChildrenNodes().Contains(key) == false)
                throw new ArgumentException(I18n.The_key_is_not_exist);

            _section   = section;
            _originKey = key;
            Key        = key;

            var value = _section.GetValue<object>(Key);
            DataType  = ValueHelper.GetDataType(value) ?? DataType.String;
            ValueText = ValueHelper.GetValueText(value);

            ValueFocused = true;
        }

        private ConfigSection _section;
        private string _originKey;

        private string _key;

        /// <summary>
        /// 名称
        /// </summary>
        public string Key
        {
            get => _key;
            set
            {
                SetProperty(ref _key, value);
                _confirmCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 验证名称合法性
        /// </summary>
        /// <returns></returns>
        [ValidateFor(nameof(Key))]
        private string ValidateKey()
        {
            var stringData = Key;

            if (ConfigPath.IsNodeValidated(stringData) == false)
                return I18n.Key_is_not_validated;

            if (Key != _originKey && _section?.GetChildrenNodes().Contains(Key) == true)
                return I18n.Key_was_be_used;

            return null;
        }

        /// <summary>
        /// </summary>
        private DataType _dataType;

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType DataType
        {
            get => _dataType;
            set
            {
                SetProperty(ref _dataType, value);
                _confirmCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string _valueText;

        /// <summary>
        /// 值
        /// </summary>
        public string ValueText
        {
            get => _valueText;
            set
            {
                SetProperty(ref _valueText, value);
                _confirmCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 验证值
        /// </summary>
        /// <returns></returns>
        [ValidateFor(nameof(ValueText))]
        private string ValueValidation()
        {
            object value;
            if (ValueHelper.TryGetValue(ValueText, DataType, out value) == false)
                return I18n.Can_not_convert_value_to_specific_type;

            return null;
        }


        #region Ok Button Command

        /// <summary>
        /// </summary>
        private DelegateCommand<IDialogWindow> _confirmCommand;

        /// <summary>
        /// OK button
        /// </summary>
        public DelegateCommand<IDialogWindow> ConfirmCommand =>
            _confirmCommand ??= new DelegateCommand<IDialogWindow>(ExecuteConfirmCommand, CanExecuteConfirmCommand);

        /// <summary>
        /// 执行关闭按钮 
        /// </summary>
        /// <param name="dialogWindow"></param>
        void ExecuteConfirmCommand(IDialogWindow dialogWindow)
        {
            Debug.Assert(_section != null);

            object value;
            if (ValueHelper.TryGetValue(ValueText, DataType, out value) == false)
                throw new InvalidOperationException(I18n.Can_not_convert_value_to_specific_type);

            // create new or edit current
            _section.SetValue(Key, value);

            // 如果换Key，删除原来的Key
            if (_originKey != null && _originKey != Key)
                _section.Remove(_originKey);

            dialogWindow.Ok();
        }

        /// <summary>
        /// 判断是否可以执行
        /// </summary>
        /// <param name="closeableWindow"></param>
        /// <returns></returns>
        bool CanExecuteConfirmCommand(IDialogWindow closeableWindow)
        {
            // name equal _originName is not fault, but we do not allow this 
            return !base.HasInFault;
        }

        #endregion
    }
}
