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
    /// �༭����������ģ��
    /// </summary>
    public class ValueEditWidowViewModel : BindableBaseWithDataError
    {
        /// <summary>e
        /// �����ڵ�
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
        /// �޸Ľڵ�
        /// </summary>
        /// <param name="section">�������Section��parent</param>
        /// <param name="key">��Ҫ�༭�� SectionName</param>
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
        /// ����
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
        /// ��֤���ƺϷ���
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
        /// ��������
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
        /// ֵ
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
        /// ��ֵ֤
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
        /// ִ�йرհ�ť 
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

            // �����Key��ɾ��ԭ����Key
            if (_originKey != null && _originKey != Key)
                _section.Remove(_originKey);

            dialogWindow.Ok();
        }

        /// <summary>
        /// �ж��Ƿ����ִ��
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
