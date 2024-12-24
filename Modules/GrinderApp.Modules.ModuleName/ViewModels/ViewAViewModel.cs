using GrinderApp.Core.Mvvm;
using GrinderApp.Services.Interfaces;
using Prism.Navigation.Regions;


namespace GrinderApp.Modules.ModuleName.ViewModels
{
    public class ViewAViewModel : GrinderApp.Core.Mvvm.RegionViewModelBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public ViewAViewModel(IRegionManager regionManager, IMessageService messageService) :
            base(regionManager)
        {
            Message = messageService.GetMessage();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }
    }
}
