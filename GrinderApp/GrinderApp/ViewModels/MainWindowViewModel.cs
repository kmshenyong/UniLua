using ControlzEx.Theming;
using GrinderApp.Core;
using GrinderApp.Core.Mvvm;
using GrinderApp.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace GrinderApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        IRegionManager regionManager;
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private bool _IsLightTheme = false;
        public bool IsLightTheme
        {
            get => _IsLightTheme;
            set
            {
                if (SetProperty(ref _IsLightTheme, value))
                {
                    ModifyTheme(value);
                }
            }
        }
        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(isDarkTheme ? BaseTheme.Dark : BaseTheme.Light);
            paletteHelper.SetTheme(theme);
        }

        //  public override void on
        public MainWindowViewModel(IRegionManager regionManager)
        {
           this.regionManager = regionManager;
            //  ChangeAccentCommand = new ICommand<string?>(o => true, this.DoChangeTheme);
        }

        public ICommand HomeCommand => new DelegateCommand(() =>
        {
            regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(HomeMenu));
        });

        public ICommand ChangeAccentCommand => new DelegateCommand<string>((o) =>
        {
            DoChangeTheme(o);
        });

        protected virtual void DoChangeTheme(string? name)
        {
            if (name is not null)
            {
                ThemeManager.Current.ChangeThemeColorScheme(Application.Current, name);
            }
        }
    }
}
