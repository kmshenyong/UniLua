using System.Collections.ObjectModel;
using System.Linq;
using GrinderApp.Configuration;
using Prism.Mvvm;

using ConfigurationEditor.Helper;


namespace ConfigurationEditor.Browse
{
    /// <summary>
    /// 配置节点视图模型
    /// </summary>
    public class ConfigSectionItem : BindableBase
    {
        private readonly IMessageBox _messageBox;
        private readonly IConfigDescription _configDescription;

        #region 选中和展开属性

        private bool _isSelected;

        /// <summary>
        /// 选中属性
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }


        private bool _isExpanded;

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        #endregion

        /// <summary>
        /// 构造函数，创建一个根
        /// </summary>
        /// <param name="rootName">根节点名称</param>
        /// <param name="rootSection">节点树</param>
        /// <param name="messageBox"></param>
        public ConfigSectionItem(string rootName, ConfigSection rootSection, IMessageBox messageBox, IConfigDescription configDescription)
        {
            _messageBox = messageBox;
            Parent      = null;
            Section     = rootSection;
            Key         = rootName;
            _configDescription = configDescription ?? throw new System.ArgumentNullException(nameof(configDescription));

            UpdateSections();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parentSectionItem">配置父节点对象</param>
        /// <param name="name">配置节点名</param>
        /// <param name="messageBox"></param>
        public ConfigSectionItem(ConfigSectionItem parentSectionItem, string name, IMessageBox messageBox, IConfigDescription configDescription)
        {
            _messageBox = messageBox;

            // 获取当前节点
            Parent  = parentSectionItem;
            Section = parentSectionItem.Section.GetSection(name);
            Key     = name;
            _configDescription = configDescription ?? throw new System.ArgumentNullException(nameof(configDescription));

            UpdateSections();
        }

        public void UpdateSections()
        {
            RaisePropertyChanged(nameof(Children));
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public ConfigSectionItem Parent
        {
            get;
        }

        /// <summary>
        /// 当前节点
        /// </summary>
        public ConfigSection Section
        {
            get;
        }

        private string _key;

        /// <summary>
        /// 配置节点名
        /// </summary>
        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }

        private readonly ObservableCollection<ConfigSectionItem> _sections = new ObservableCollection<ConfigSectionItem>();

        /// <summary>
        /// 子节点
        /// </summary>
        public ObservableCollection<ConfigSectionItem> Children
        {
            get
            {
                var pathNodes = Section.GetChildrenNodes(true);
                foreach (var key in pathNodes)
                {
                    if (_sections.Any(c => c.Key == key) == false)
                        _sections.Add(new ConfigSectionItem(this, key, _messageBox, _configDescription));
                }

                foreach (var item in _sections.ToArray())
                {
                    if (pathNodes.Contains(item.Key) == false)
                        _sections.Remove(item);
                }

                return _sections;
            }
        }

        private readonly ObservableCollection<ConfigValueItem> _values = new ObservableCollection<ConfigValueItem>();

        /// <summary>
        /// 值
        /// </summary>
        public ObservableCollection<ConfigValueItem> Values
        {
            get
            {
                var nodes = Section.GetChildrenNodes(false);
                nodes = nodes.OrderBy(c => c).ToArray();

                foreach (var key in nodes)
                {
                    if (_values.Any(c => c.Key == key) == false)
                        _values.Add(new ConfigValueItem(this, key, _messageBox, _configDescription));
                }

                foreach (var item in _values.ToArray())
                {
                    if (nodes.Contains(item.Key) == false)
                        _values.Remove(item);
                }

                return _values;
            }
        }

        public void UpdateValues()
        {
            RaisePropertyChanged(nameof(Values));
        }
    }
}
