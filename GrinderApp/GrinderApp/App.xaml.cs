using System.Windows;
using ConfigurationEditor.DependencyInjection;
using GrinderApp.Configuration;
using GrinderApp.Modules.ModuleName;
using GrinderApp.Services;
using GrinderApp.Services.Interfaces;
using GrinderApp.Views;

using Prism.Ioc;
using Prism.Modularity;

namespace GrinderApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(ConfigFactory.Config);
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            containerRegistry.RegisterSingleton<IAppConfig, AppConfig > ();
            containerRegistry.RegisterForNavigation<HomeMenu>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
            moduleCatalog.AddModule<ConfigurationEditorModule>();
        }
        //protected override async void OnStartup(StartupEventArgs e)
        //{


        //}
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }
    }
}
