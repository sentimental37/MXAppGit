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
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace MXApp.ViewModels
{
    public class SearchWOPopupViewModel : ViewModelBase
    {
        #region Members
        private ProductionViewModel production;
        private ObservableCollection<ProdOpenOrders> openOrders;
        private int? selectedRefNum;
        private ProdOpenOrders selectedRef;
        private bool isDetailVisible;
        private AsyncCommand selectRefNumCommand;
        private AsyncCommand barcodeCommand;
        private string selectedRefText;
        #endregion

        #region Constructors
        public SearchWOPopupViewModel(ProductionViewModel productionViewModel)
        {
            Production = productionViewModel;
        }
        #endregion

        #region Properties
        public ObservableCollection<ProdOpenOrders> OpenProdOrders
        {
            get
            {
                return openOrders;
            }
            set
            {
                openOrders = value;
                OnPropertyChanged();
            }
        }
        public ProductionViewModel Production
        {
            get
            {
                return production;
            }
            set
            {
                production = value;
                OnPropertyChanged();
            }
        }
        public int? SelectedRefNum
        {
            get { return selectedRefNum; }
            set
            {
                selectedRefNum = value;
                OnPropertyChanged();
                SelectedRef = OpenProdOrders?.Where(x => x.ReferencePullID == selectedRefNum).FirstOrDefault();
            }
        }
        public ProdOpenOrders SelectedRef
        {
            get { return selectedRef; }
            set
            {
                selectedRef = value;
                OnPropertyChanged();
                if (value != null)
                {
                    IsDetailVisible = true;
                }
                else
                {
                    IsDetailVisible = false;
                }
            }
        }
        public bool IsDetailVisible
        {
            get { return isDetailVisible; }
            set { isDetailVisible = value; OnPropertyChanged(); }
        }
        public string SelectedRefText
        {
            get { return selectedRefText; }
            set { selectedRefText = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public AsyncCommand SelectRefNumCommand
        {
            get
            {
                if (selectRefNumCommand == null)
                    selectRefNumCommand = new AsyncCommand(SelectRefNumCommandMethod);
                return selectRefNumCommand;
            }
        }
        public AsyncCommand BarcodeCommand
        {
            get
            {
                if (barcodeCommand == null)
                    barcodeCommand = new AsyncCommand(BarcodeCommandMethod);
                return barcodeCommand;
            }
        }
        #endregion

        #region Methods
        public async Task LoadRefNumList()
        {
            try
            {
                if (OpenProdOrders == null || OpenProdOrders.Count == 0)
                {
                    this.IsBusy = true;
                    try
                    {
                        string uri = App.BASE_PROD_URL + "GetOpenOrders";
                        var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<ProdOpenOrders>>(uri));
                        if (res != null)
                        {
                            ObservableCollection<ProdOpenOrders> orders = new ObservableCollection<ProdOpenOrders>(res);
                            OpenProdOrders = orders;
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
        private async Task SelectRefNumCommandMethod()
        {
            if(SelectedRef!=null)
            {
                try
                {
                    Production.SelectedRef = SelectedRef;
                    //Production.TitleString = SelectedRef.ReferencePullID.ToString();
                    await PopupNavigation.PopAsync(true);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);

                }
            }
            else
            {
                await App.DialogService.ShowAlertAsync("Please provide a valid work order ref#", "Error", "ok");
            }
        }
        private async Task BarcodeCommandMethod()
        {
            try
            {
                await Production.LoadBarcodeScanner(this);
                PopupNavigation.PopAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        #endregion
    }
}
