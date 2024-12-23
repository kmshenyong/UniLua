using System;
using System.Diagnostics;
using System.Linq;
using ConfigurationEditor.Helper;
using GrinderApp.Configuration;
using Prism.Commands; //using FirstLineTamping.Infrastructure.UI.ViewModels;

namespace ConfigurationEditor.SectionEdit
{
    /// <summary>
    /// 设置配置节点名
    /// </summary>
    public class SectionEditWindowViewModel : BindableBaseWithDataError
    {
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="parentSection"></param>
        public void CreateSection(ConfigSection parentSection)
        {
            if (parentSection == null)
                throw new ArgumentNullException(nameof(parentSection));

            _parentSection = parentSection;

            Name = "";
        }

        /// <summary>
        /// 设置配置节点
        /// </summary>
        /// <param name="parentSection">持有这个Section的parent</param>
        /// <param name="name">需要编辑的 SectionName</param>
        public void EditConfigSection(ConfigSection parentSection, string name = null)
        {
            if (parentSection == null)
                throw new ArgumentNullException(nameof(parentSection));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (parentSection.GetChildrenNodes().Contains(name) == false)
                throw new ArgumentException(I18n.The_name_is_not_in_section);
            
            _parentSection = parentSection;
            _originName    = name;
            Name           = name;
        }

        private ConfigSection _parentSection;

        #region Name

        /// <summary>
        /// 修改前的原值
        /// </summary>
        private string _originName;

        /// <summary>
        /// 新名称
        /// </summary>
        private string _name;

        /// <summary>
        /// 新名称
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                _confirmCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 验证SectionName
        /// </summary>
        /// <returns></returns>
        [ValidateFor(nameof(Name))]
        private string NameValidation()
        {
            var stringData = Name;

            if (ConfigPath.IsNodeValidated(stringData) == false)
                return I18n.Section_name_is_not_validated;

            if (Name != _originName && _parentSection?.GetChildrenNodes().Contains(Name) == true)
                return I18n.Section_name_was_be_used;

            return null;
        }

        #endregion

        #region Ok Button Command

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
            Debug.Assert(_parentSection != null);

            // create new or edit current
            if (_originName == null)
            {
                var newPath = ConfigPath.CombinePath(Name, "(Default)");
                _parentSection.SetValue(newPath, "");
            }
            else if (_originName != Name)
            {
                var section = _parentSection.GetSection(_originName);
                if (section == null)
                    throw new Exception(I18n.Section_is_not_exist);

                _parentSection.Rename(_originName, Name);
            }

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
