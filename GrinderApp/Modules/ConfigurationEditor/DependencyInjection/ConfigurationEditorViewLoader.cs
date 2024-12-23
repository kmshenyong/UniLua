using ConfigurationEditor.Browse;
using ConfigurationEditor.Shell;
using MaterialDesignThemes.Wpf;
using GrinderApp.Configuration;
using Prism.Navigation.Regions;

using Prism.Navigation;
using GrinderApp.Core.Interface;

namespace ConfigurationEditor.DependencyInjection
{
    public class ConfigurationEditorViewLoader : IConfigurationEditorViewLoader, IModuleViewLoader
    {
        private readonly IRegionManager _regionManager;
        private readonly Config _config;

        public ConfigurationEditorViewLoader(IRegionManager regionManager, Config config)
        {
            _regionManager = regionManager;
            _config = config;
        }

        public  void Show(string regionName, params ConfigSectionView[] configSections)
        {
            //  _regionManager.RequestNavigate(regionName,nameof ( ConfigurationShell));
            _regionManager.RequestNavigate(regionName, nameof(ConfigurationBrowse), new NavigationParameters
            {
                {
                    "ConfigSections", configSections
                }
            });
        }

        /// <summary>
        /// 在呈现区显示
        /// </summary>
        /// <param name="regionName">呈现区</param>
        public void Show(string regionName)
        {
            var configSection = _config.GetSection();
         //   Show(regionName, new ConfigSectionView("Default", configSection));
              Show(regionName, new ConfigSectionView("Root", configSection));
            //  _regionManager.RequestNavigate(regionName, nameof(ConfigurationShell));
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name => "参数设置";

        /// <summary>
        /// 图标 todo 没有完全想好支持的格式, 目前支持 MaterialDesign 图标的枚举
        /// </summary>
        public string Icon => PackIconKind.Cog.ToString();

        /// <summary>
        /// 默认显示顺序
        /// </summary>
        public int DefaultIndex => 1000;
    }
}
