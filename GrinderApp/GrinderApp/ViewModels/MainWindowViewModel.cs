using GrinderApp.Core.Mvvm;
using MaterialDesignThemes.Wpf;
using Prism.Mvvm;

namespace GrinderApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private bool _IsLightTheme = false ;
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
        public MainWindowViewModel()
        {

        }
    }
}
