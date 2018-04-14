using Microsoft.AppCenter.Crashes;
using MXApi.Models;
using MXApp.ViewModels.Base;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MXApp.ViewModels
{
    public class SearchWarehousePopupViewModel : ViewModelBase
    {

        public SearchWarehousePopupViewModel(ShippingViewModel shippingViewModel)
        {
            ParentVM = shippingViewModel;
        }
        private ShippingViewModel parent;

        public ShippingViewModel ParentVM
        {
            get { return parent; }
            set { parent = value; OnPropertyChanged(); }
        }

        private ObservableCollection<WHS> whsList;

        public ObservableCollection<WHS> WHSList
        {
            get { return whsList; }
            set { whsList = value; OnPropertyChanged(); }
        }
        private string selectedWHSText;

        public string SelectedWHSText
        {
            get { return selectedWHSText; }
            set
            {
                selectedWHSText = value;
                OnPropertyChanged();
                SelectedWarehouse = WHSList?.Where(x => x.WHS == SelectedWHS).FirstOrDefault();
            }
        }

        private string selectedWHS;

        public string SelectedWHS
        {
            get { return selectedWHS; }
            set
            {
                selectedWHS = value;
                OnPropertyChanged();
                if (value != null)
                    SelectedWarehouse = WHSList?.Where(x => x.WHS == SelectedWHS).FirstOrDefault();
            }
        }

        private WHS selectedWarehouse;

        public WHS SelectedWarehouse
        {
            get { return selectedWarehouse; }
            set { selectedWarehouse = value; OnPropertyChanged(); }
        }

        private AsyncCommand selectWHSCommand;

        public AsyncCommand SelectWHSCommand
        {
            get
            {
                if (selectWHSCommand == null)
                    selectWHSCommand = new AsyncCommand(SelectWHSCommandMethod);
                return selectWHSCommand;
            }
        }

        private async Task SelectWHSCommandMethod()
        {
            if (SelectedWarehouse != null)
            {
                try
                {
                    await PopupNavigation.PopAsync(true);
                    ParentVM.SelectedWareHouse = SelectedWarehouse;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
            else
            {
                await App.DialogService.ShowAlertAsync("Please provide a valid warehouse", "Error", "ok");
            }
        }

        public async Task LoadWHSList()
        {
            try
            {
                if (WHSList == null || WHSList.Count == 0)
                {
                    this.IsBusy = true;
                    try
                    {
                        string uri = App.BASE_SHIPPING_URL + "LoadWHSList";
                        var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<WHS>>(uri));
                        if (res != null)
                        {
                            ObservableCollection<WHS> whsLists = new ObservableCollection<WHS>(res);
                            WHSList = whsLists;
                        }
                    }
                    catch (Exception ex)
                    {
                        await App.DialogService.ShowAlertAsync(ex.Message, "Error", "ok");
                    }
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
