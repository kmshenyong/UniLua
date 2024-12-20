using System.Windows;
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
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
        }
    }
}
