using MXApp.ViewModels.Base;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MXApp.ViewModels
{
    public class EditViewLoadViewModel : ViewModelBase
    {
        private ShippingViewModel shippingViewModel;

        public ShippingViewModel ShippingViewModel
        {
            get { return shippingViewModel; }
            set { shippingViewModel = value; OnPropertyChanged(); }
        }

        public EditViewLoadViewModel(ShippingViewModel shippingViewModel)
        {
            ShippingViewModel = shippingViewModel;
            SetValues();
        }
        private string departingDoor;

        public string DepartingDoor
        {
            get { return departingDoor; }
            set { departingDoor = value; OnPropertyChanged(); }
        }
        private string appIDComments;

        public string AppIDComments
        {
            get { return appIDComments; }
            set { appIDComments = value; OnPropertyChanged(); }
        }

        private AsyncCommand updateCommand;

        public AsyncCommand UpdateCommand
        {
            get
            {
                if (updateCommand == null)
                    updateCommand = new AsyncCommand(UpdateCommandMethod);
                return updateCommand;
            }
           
        }
        private void SetValues()
        {
            DepartingDoor = ShippingViewModel.ViewLoad.DepartingDoor;
            AppIDComments = ShippingViewModel.ViewLoad.AppIDComments;
        }
        private async Task UpdateCommandMethod()
        {
            ShippingViewModel.UpdateItem(this);
            await PopupNavigation.PopAsync();
        }
    }
}
