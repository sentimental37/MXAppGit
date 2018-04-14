using MXApp.ViewModels.Base;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MXApp.ViewModels
{
    public class EditLoadDetailsPopupViewModel : ViewModelBase
    {
        public EditLoadDetailsPopupViewModel(LoadDetailsPageViewModel detailsPageViewModel)
        {
            ParentVM = detailsPageViewModel;
            SetValues();
        }

        private void SetValues()
        {
            if (ParentVM.ViewLoadDetail.MBOLPalletCount != null)
                MBOLPalletCount = ParentVM.ViewLoadDetail.MBOLPalletCount.Value;
            if (ParentVM.ViewLoadDetail.MBOLShipComments != null)
                MBOLShipComments = ParentVM.ViewLoadDetail.MBOLShipComments;
        }

        private LoadDetailsPageViewModel parentVM;

        public LoadDetailsPageViewModel ParentVM
        {
            get { return parentVM; }
            set { parentVM = value; OnPropertyChanged(); }
        }

        private int mBOLPalletCount;

        public int MBOLPalletCount
        {
            get { return mBOLPalletCount; }
            set { mBOLPalletCount = value; OnPropertyChanged(); }
        }

        private string mBOLShipComments;

        public string MBOLShipComments
        {
            get { return mBOLShipComments; }
            set { mBOLShipComments = value; OnPropertyChanged(); }
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

        private async Task UpdateCommandMethod()
        {
            ParentVM.UpdateItem(this);
            await PopupNavigation.PopAsync();
        }
    }
}
