
using ConfigurationEditor.SectionEdit;
using ConfigurationEditor.ValueEdit;
using GrinderApp.Configuration;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ConfigurationEditor.Browse
{
    /// <summary>
    /// 配置参数浏览视图模型
    /// </summary>
    public class ConfigurationBrowseViewModel : BindableBase, INavigationAware
    {
        /// <summary>
        /// </summary>
        private readonly ConfigurationEditor.Helper.IMessageBox _messageBox;
        private readonly IConfigDescription _configDescription;
        Config config;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="messageBox"></param>
        public ConfigurationBrowseViewModel(ConfigurationEditor.Helper.IMessageBox messageBox, IConfigDescription configDescription)
        {
            _messageBox = messageBox;
            _configDescription = configDescription;
            // 初始化提供的节点
            config = new Config();
            Roots = new ObservableCollection<ConfigSectionItem>(new[]
            {
                new ConfigSectionItem("ROOT", new ConfigSection(config, ""), messageBox,configDescription),
            });
        }

        #region Section Collection & Value Collection

        private ObservableCollection<ConfigSectionItem> _roots;

        /// <summary>
        /// 根节点（为了绑定，弄成数组）
        /// </summary>
        public ObservableCollection<ConfigSectionItem> Roots
        {
            get => _roots;
            set => SetProperty(ref _roots, value);
        }

        private ConfigSectionItem _selectedSection;

        /// <summary>
        /// 当前选择的节点
        /// </summary>
        public ConfigSectionItem SelectedSection
        {
            get => _selectedSection;
            set
            {
                SetProperty(ref _selectedSection, value);

                CreateSectionCommand.RaiseCanExecuteChanged();
                RenameSectionCommand.RaiseCanExecuteChanged();
                DeleteSectionCommand.RaiseCanExecuteChanged();
                CreateValueCommand.RaiseCanExecuteChanged();
                EditValueCommand.RaiseCanExecuteChanged();
                DeleteValueCommand.RaiseCanExecuteChanged();
            }
        }

        private ConfigValueItem _selectedValue;

        /// <summary>
        /// 当前选择的值
        /// </summary>
        public ConfigValueItem SelectedValue
        {
            get => _selectedValue;
            set
            {
                SetProperty(ref _selectedValue, value);

                EditValueCommand.RaiseCanExecuteChanged();
                DeleteValueCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Selection Changed Command

        /// <summary>
        /// </summary>
        private DelegateCommand<ConfigSectionItem> _sectionChangedCommand;

        /// <summary>
        /// 选择的节点变更事件
        /// </summary>
        public DelegateCommand<ConfigSectionItem> SectionChangedCommand =>
            _sectionChangedCommand ??= new DelegateCommand<ConfigSectionItem>(ExecuteSectionChangedCommand);

        /// <summary>
        /// 选择节点改变，更新右侧值信息
        /// </summary>
        /// <param name="section"></param>
        void ExecuteSectionChangedCommand(ConfigSectionItem section)
        {
            SelectedSection = section;
        }

        #endregion

        #region Section Create

        private DelegateCommand _createSectionCommand;

        /// <summary>
        /// 创建一个节点
        /// </summary>
        public DelegateCommand CreateSectionCommand =>
            _createSectionCommand ??= new DelegateCommand(ExecuteCreateSectionCommand, CanExecuteCreateSectionCommand);

        void ExecuteCreateSectionCommand()
        {
            var edit = new SectionEditWindow();
            edit.ViewModel.CreateSection(SelectedSection.Section);
            if (edit.ShowDialog() == true)
            {
                SelectedSection.IsExpanded = true;
                var newItem = SelectedSection.Children.FirstOrDefault(c => c.Key == edit.ViewModel.Name);
                if (newItem != null)
                    newItem.IsSelected = true;
            }
        }

        bool CanExecuteCreateSectionCommand()
        {
            return SelectedSection != null;
        }

        #endregion

        #region Section Rename

        private DelegateCommand _renameSectionCommand;

        /// <summary>
        /// Rename command
        /// </summary>
        public DelegateCommand RenameSectionCommand =>
            _renameSectionCommand ??= new DelegateCommand(ExecuteRenameSectionCommand, CanExecuteRenameSectionCommand);

        /// <summary>
        /// rename section
        /// </summary>
        void ExecuteRenameSectionCommand()
        {
            var view = new SectionEditWindow
            {
                Owner = Application.Current.MainWindow
            };
            view.ViewModel.EditConfigSection(SelectedSection.Parent.Section, SelectedSection.Key);

            if (view.ShowDialog() ?? false)
            {
                SelectedSection.Key = view.ViewModel.Name;
            }
        }

        bool CanExecuteRenameSectionCommand()
        {
            return SelectedSection?.Parent != null;
        }

        #endregion

        #region Section Delete

        private DelegateCommand _deleteSectionCommand;

        public DelegateCommand DeleteSectionCommand =>
            _deleteSectionCommand ??= new DelegateCommand(ExecuteDeleteSectionCommand, CanExecuteDeleteSectionCommand);

        /// <summary>
        /// 删除 Section
        /// </summary>
        async void ExecuteDeleteSectionCommand()
        {
            if (await _messageBox.ShowQuestionAsync(I18n.Are_you_sure_delete_this_section) == true)
            {
                var key = SelectedSection.Key; // hold the key that needs to be deleted

                var parent = SelectedSection.Parent;
                parent.IsSelected = true;

                // remove section then update ui
                parent.Section.Remove(key);
                parent.UpdateSections();
            }
        }

        bool CanExecuteDeleteSectionCommand()
        {
            return SelectedSection?.Parent != null;
        }

        #endregion

        #region Value Create

        /// <summary>
        /// </summary>
        private DelegateCommand _createValueCommand;

        /// <summary>
        /// 创建新的值
        /// </summary>
        public DelegateCommand CreateValueCommand =>
            _createValueCommand ??= new DelegateCommand(ExecuteCreateValueCommand, CanExecuteCreateValueCommand);


        /// <summary>
        /// 执行创建新值命令
        /// </summary>
        void ExecuteCreateValueCommand()
        {
            var valueEdit = new ValueEditWidow
            {
                Owner = Application.Current.MainWindow
            };
            valueEdit.ViewModel.CreateSection(SelectedSection.Section);
            if (valueEdit.ShowDialog() == true)
            {
                SelectedSection.UpdateValues();
                SelectedValue = SelectedSection.Values.FirstOrDefault(c => c.Key == valueEdit.ViewModel.Key);
            }
        }

        /// <summary>
        /// 判断是否可以创建新值
        /// </summary>
        /// <returns></returns>
        bool CanExecuteCreateValueCommand()
        {
            return SelectedSection != null;
        }

        #endregion

        #region Value Edit

        private DelegateCommand _editValueCommand;

        public DelegateCommand EditValueCommand =>
            _editValueCommand ??= new DelegateCommand(ExecuteEditValueCommand, CanExecuteEditValueCommand);

        void ExecuteEditValueCommand()
        {
            var valueEdit = new ValueEditWidow()
            {
                Owner = Application.Current.MainWindow
            };

            valueEdit.ViewModel.EditConfigSection(SelectedSection.Section, SelectedValue.Key);

            if (valueEdit.ShowDialog() == true)
            {
                // Key 不在范畴内, 因此特别添加
                SelectedValue.Key = valueEdit.ViewModel.Key;
                SelectedValue.Update();
            }
        }

        bool CanExecuteEditValueCommand()
        {
            return SelectedSection != null && SelectedValue != null;
        }

        #endregion

        #region Value Delete

        private DelegateCommand _deleteValueCommand;

        public DelegateCommand DeleteValueCommand =>
            _deleteValueCommand ??= new DelegateCommand(ExecuteDeleteValueCommand, CanExecuteDeleteValueCommand);

        async void ExecuteDeleteValueCommand()
        {
            if (await _messageBox.ShowQuestionAsync(I18n.Are_you_sure_you_want_to_delete_this_item) == true)
            {
                var selectedValueItem = SelectedValue;

                // when value removed, ensure next value be selected.
                ConfigValueItem nextValue = null;
                foreach (var value in SelectedSection.Values.Reverse())
                {
                    if (value.Selected)
                        break;

                    nextValue = value;
                }

                foreach (var value in SelectedSection.Values)
                {
                    if (value.Selected)
                        break;

                    nextValue = value;
                }

                // remove value
                SelectedSection.Section.Remove(selectedValueItem.Key);
                SelectedSection.UpdateValues();

                // select next value
                nextValue ??= SelectedSection.Values.FirstOrDefault();
                if (nextValue != null)
                    nextValue.Selected = true;
            }
        }

        bool CanExecuteDeleteValueCommand()
        {
            return SelectedSection != null && SelectedValue != null;
        }

        #endregion

        #region Navigator

        /// <summary>Called when the implementer has been navigated to.</summary>
        /// <param name="navigationContext">The navigation context.</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var param = navigationContext.Parameters["ConfigSections"] as ConfigSectionView[] ?? new ConfigSectionView[0];
            var roots = from p in param
                        let first = p == param.FirstOrDefault()
                        select new ConfigSectionItem(p.Name, p.Section, _messageBox, _configDescription)
                        {
                            IsSelected = first,
                            IsExpanded = first
                        };

            Roots = new ObservableCollection<ConfigSectionItem>(roots);
        }

        /// <summary>
        /// Called to determine if this instance can handle the navigation request.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <returns>
        /// <see langword="true" /> if this instance accepts the navigation request; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// Called when the implementer is being navigated away from.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion
    }
}
