using ConfigurationEditor.Browse;
using ConfigurationEditor.Shell;
using GrinderApp.Configuration;
using GrinderApp.Core.Interface;
using Prism.Ioc;
using Prism.Modularity;


namespace ConfigurationEditor.DependencyInjection
{
    /// <summary>
    /// 配置编辑模块
    /// </summary>
    public class ConfigurationEditorModule : IModule
    {
        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IConfigurationEditorViewLoader, ConfigurationEditorViewLoader>();
            containerRegistry.Register<IModuleViewLoader, ConfigurationEditorViewLoader>(nameof(ConfigurationEditorViewLoader));
            containerRegistry.Register<IConfigDescription, ConfigDescription>();
            containerRegistry.RegisterForNavigation<  ConfigurationShell>(nameof(ConfigurationShell));
            containerRegistry.RegisterForNavigation<ConfigurationBrowse>();
        }

        /// <summary>Notifies the module that it has be initialized.</summary>
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}
