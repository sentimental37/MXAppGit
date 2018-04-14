using Microsoft.AppCenter.Crashes;
using MXApi.Models;
using MXApp.ViewModels.Base;
using MXApp.Views;
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
    public class LoadDetailsPageViewModel : ViewModelBase
    {

        public LoadDetailsPageViewModel()
        {

        }
        private ShippingViewModel _parent;

        public ShippingViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; OnPropertyChanged(); }
        }

        public LoadDetailsPageViewModel(int _EDIAppID, string pickupLoadNum, ShippingViewModel parent)
        {
            EDIAppID = _EDIAppID;
            PickupLoadNum = pickupLoadNum;
            Parent = parent;
        }
        private int ediAppID;

        public int EDIAppID
        {
            get { return ediAppID; }
            set { ediAppID = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ViewLoadDetailsModel> viewLoadDetailsList;

        public ObservableCollection<ViewLoadDetailsModel> ViewLoadDetailsList
        {
            get { return viewLoadDetailsList; }
            set { viewLoadDetailsList = value; OnPropertyChanged(); }
        }

        private ViewLoadDetailsModel viewLoadDetail;

        public ViewLoadDetailsModel ViewLoadDetail
        {
            get { return viewLoadDetail; }
            set { viewLoadDetail = value; OnPropertyChanged(); }
        }

        private ObservableCollection<object> selectedViewLoadDetail;

        public ObservableCollection<object> SelectedViewLoadDetail
        {
            get { return selectedViewLoadDetail; }
            set { selectedViewLoadDetail = value; OnPropertyChanged(); }
        }
        private string pickupLoadNum;

        public string PickupLoadNum
        {
            get { return pickupLoadNum; }
            set { pickupLoadNum = value; }
        }

        public async Task LoadDetails()
        {
            if (EDIAppID > 0)
            {
                try
                {
                    IsBusy = true;
                    string uri = App.BASE_SHIPPING_URL + "GetShippingViewLoadDetails/" + EDIAppID;
                    var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<ViewLoadDetailsModel>>(uri));
                    if (res != null)
                    {
                        ViewLoadDetailsList = new ObservableCollection<ViewLoadDetailsModel>(res);
                    }
                    IsBusy = false;
                }
                catch (Exception ex)
                {
                    await App.DialogService.ShowAlertAsync(ex.Message, "Error", "ok");
                    IsBusy = false;
                    Crashes.TrackError(ex);
                }
            }
        }
        private AsyncCommand editLoadDetailsCommand;

        public AsyncCommand EditLoadDetailsCommand
        {
            get
            {
                if (editLoadDetailsCommand == null)
                    editLoadDetailsCommand = new AsyncCommand(EditLoadDetailsCommandMethod);
                return editLoadDetailsCommand;
            }
        }

        private async Task EditLoadDetailsCommandMethod()
        {
            if (!PopupNavigation.PopupStack.Any(x => x.GetType() == typeof(EditLoadDetailsPopup)))
            {
                EditLoadDetailsPopup popup = new EditLoadDetailsPopup();
                EditLoadDetailsPopupViewModel vm = new EditLoadDetailsPopupViewModel(this);
                popup.BindingContext = vm;
                await PopupNavigation.PushAsync(popup, true);
            }
        }
        public async Task UpdateItem(EditLoadDetailsPopupViewModel vm)
        {
            try
            {
                IsBusy = true;
                ViewLoadDetailsModel model = new ViewLoadDetailsModel();
                model.ChildAPPID = ViewLoadDetail.ChildAPPID;
                model.MBOLPalletCount = vm.MBOLPalletCount;
                model.MBOLShipComments = vm.MBOLShipComments;
                string uri = App.BASE_SHIPPING_URL + "UpdateViewLoadDetailsItem";
                var res = await Task.Run(() => App.ServiceHelper.PostAsync<ViewLoadDetailsModel, int>(uri, model));
                if (res > 0)
                {
                    await App.DialogService.ShowAlertAsync("Item Updated Successfully", "Updated", "Ok");
                    await LoadDetails();
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsBusy = false;
            }
        }

        private AsyncCommand shippedNoCommand;

        public AsyncCommand ShippedNoCommand
        {
            get
            {
                if (shippedNoCommand == null)
                    shippedNoCommand = new AsyncCommand(ShippedNoCommandMethod);
                return shippedNoCommand;
            }
        }

        private async Task ShippedNoCommandMethod()
        {
            if (SelectedViewLoadDetail != null && SelectedViewLoadDetail.Count > 0)
            {
                IsBusy = true;
                foreach (var item in SelectedViewLoadDetail)
                {
                    var obj = item as ViewLoadDetailsModel;
                    ConfirmDetailsModel model = new ConfirmDetailsModel();
                    model.mBOLVICS = obj.MBOLVICS;
                    model.type = 0;
                    string uri = App.BASE_SHIPPING_URL + "ConfirmDetail";
                    await Task.Run(() => App.ServiceHelper.PostAsync<ConfirmDetailsModel, int>(uri, model));
                }
                await App.DialogService.ShowAlertAsync("Selected Bill Of Ladings have been marked as fail to pick up.", "Success", "OK");
                await Task.Run(() => LoadDetails());
                await Task.Run(() => MarkMasterNo());
            }
            else
            {
                await App.DialogService.ShowAlertAsync("Please select at least one MBOL before proceeding.", "Error", "OK");
            }
        }

        private async Task MarkMaster()
        {
            if (ViewLoadDetailsList != null && ViewLoadDetailsList.Count > 0)
            {
                if (ViewLoadDetailsList.ToList().All(x => x.FullPickUp == true))
                {
                    ConfirmMasterModel model = new ConfirmMasterModel();
                    model.eDIAPPID = EDIAppID;
                    model.type = 1;
                    string uri = App.BASE_SHIPPING_URL + "ConfirmMaster";
                    await Task.Run(() => App.ServiceHelper.PostAsync<ConfirmMasterModel, int>(uri, model));
                }
                else
                {
                    ConfirmMasterModel model = new ConfirmMasterModel();
                    model.eDIAPPID = EDIAppID;
                    model.type = 0;
                    string uri = App.BASE_SHIPPING_URL + "ConfirmMaster";
                    await Task.Run(() => App.ServiceHelper.PostAsync<ConfirmMasterModel, int>(uri, model));
                }
            }
        }
        private async Task MarkMasterNo()
        {
            if (ViewLoadDetailsList != null && ViewLoadDetailsList.Count > 0)
            {

                ConfirmMasterModel model = new ConfirmMasterModel();
                model.eDIAPPID = EDIAppID;
                model.type = 0;
                string uri = App.BASE_SHIPPING_URL + "ConfirmMaster";
                await Task.Run(() => App.ServiceHelper.PostAsync<ConfirmMasterModel, int>(uri, model));

            }
        }

        private AsyncCommand shippedYesCommand;

        public AsyncCommand ShippedYesCommand
        {
            get
            {
                if (shippedYesCommand == null)
                    shippedYesCommand = new AsyncCommand(ShippedYesCommandMethod);
                return shippedYesCommand;
            }
        }

        private async Task ShippedYesCommandMethod()
        {
            if (SelectedViewLoadDetail != null && SelectedViewLoadDetail.Count > 0)
            {
                ConfirmPopup popup = new ConfirmPopup(this);
                await PopupNavigation.PushAsync(popup);
            }
            else
            {
                await App.DialogService.ShowAlertAsync("Please select at least one MBOL before proceeding.", "Error", "OK");
            }
        }
        public async Task ShippedYesMethod()
        {
            if (SelectedViewLoadDetail != null && SelectedViewLoadDetail.Count > 0)
            {
                IsBusy = true;
                foreach (var item in SelectedViewLoadDetail)
                {
                    var obj = item as ViewLoadDetailsModel;
                    ConfirmDetailsModel model = new ConfirmDetailsModel();
                    model.mBOLVICS = obj.MBOLVICS;
                    model.type = 1;
                    string uri = App.BASE_SHIPPING_URL + "ConfirmDetail";
                    await Task.Run(() => App.ServiceHelper.PostAsync<ConfirmDetailsModel, int>(uri, model));
                }
                await App.DialogService.ShowAlertAsync("Selected Bill Of Ladings have been marked as shipped.", "Success", "OK");
                await Task.Run(() => LoadDetails());
                await Task.Run(() => MarkMaster());
            }
            else
            {
                await App.DialogService.ShowAlertAsync("Please select at least one MBOL before proceeding.", "Error", "OK");
            }
        }
        private bool isGridBusy;

        public bool IsGridBusy
        {
            get { return isGridBusy; }
            set { isGridBusy = value; OnPropertyChanged(); }
        }

        private AsyncCommand pullToRefreshCommand;

        public AsyncCommand PullToRefreshCommand
        {
            get
            {
                if (pullToRefreshCommand == null)
                    pullToRefreshCommand = new AsyncCommand(PullToRefreshCommandMethod);
                return pullToRefreshCommand;
            }
        }

        private async Task PullToRefreshCommandMethod()
        {
            await LoadDetails();
        }

        #region Filtering

        #region Fields

        private string filterText = "";
        private string selectedColumn = "All Columns";
        private string selectedCondition = "Contains";
        internal delegate void FilterChanged();
        internal FilterChanged filterTextChanged;

        #endregion

        #region Property

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                OnFilterTextChanged();
                OnPropertyChanged();
            }
        }

        public string SelectedCondition
        {
            get { return selectedCondition; }
            set { selectedCondition = value; OnPropertyChanged(); }
        }

        public string SelectedColumn
        {
            get { return selectedColumn; }
            set { selectedColumn = value; OnPropertyChanged(); }
        }

        #endregion

        #region Private Methods

        private void OnFilterTextChanged()
        {
            if (filterTextChanged != null)
                filterTextChanged();
        }

        private bool MakeStringFilter(ViewLoadDetailsModel o, string option, string condition)
        {
            var value = o.GetType().GetProperty(option);
            var exactValue = value.GetValue(o, null);
            exactValue = exactValue.ToString().ToLower();
            string text = FilterText.ToLower();
            var methods = typeof(string).GetMethods();
            if (methods.Count() != 0)
            {
                if (condition == "Contains")
                {
                    var methodInfo = methods.FirstOrDefault(method => method.Name == condition);
                    bool result1 = (bool)methodInfo.Invoke(exactValue, new object[] { text });
                    return result1;
                }
                else if (exactValue.ToString() == text.ToString())
                {
                    bool result1 = String.Equals(exactValue.ToString(), text.ToString());
                    if (condition == "Equals")
                        return result1;
                    else if (condition == "NotEquals")
                        return false;
                }
                else if (condition == "NotEquals")
                {
                    return true;
                }
                return false;
            }
            else
                return false;
        }

        private bool MakeNumericFilter(ViewLoadDetailsModel o, string option, string condition)
        {
            var value = o.GetType().GetProperty(option);
            var exactValue = value.GetValue(o, null);
            double res;
            bool checkNumeric = double.TryParse(exactValue.ToString(), out res);
            if (checkNumeric)
            {
                switch (condition)
                {
                    case "Equals":
                        try
                        {
                            if (exactValue.ToString() == FilterText)
                            {
                                if (Convert.ToDouble(exactValue) == (Convert.ToDouble(FilterText)))
                                    return true;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;
                    case "NotEquals":
                        try
                        {
                            if (Convert.ToDouble(FilterText) != Convert.ToDouble(exactValue))
                                return true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        #endregion

        #region Public Methods

        public bool FilerRecords(object o)
        {
            double res;
            bool checkNumeric = double.TryParse(FilterText, out res);
            var item = o as ViewLoadDetailsModel;
            if (item != null && FilterText.Equals(""))
            {
                return true;
            }
            else
            {
                if (item != null)
                {
                    if (checkNumeric && !SelectedColumn.Equals("All Columns"))
                    {
                        bool result = MakeNumericFilter(item, SelectedColumn, SelectedCondition);
                        return result;
                    }
                    else if (SelectedColumn.Equals("All Columns"))
                    {
                        if (item != null)
                        {
                            if (item.Account != null)
                                if (item.Account.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.AccountDivName != null)
                                if (item.AccountDivName.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.ChildAPPID != null)
                                if (item.ChildAPPID.ToString().ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.CTNCount != null)
                                if (item.CTNCount.ToString().ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.EDIAPPID != null)
                                if (item.EDIAPPID.ToString().ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.MBOLPalletCount != null)
                                if (item.MBOLPalletCount.ToString().ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.MBOLShipComments != null)
                                if (item.MBOLShipComments.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.MBOLVICS != null)
                                if (item.MBOLVICS.ToString().ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.ShipTo != null)
                                if (item.ShipTo.ToString().ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.TotalCube != null)
                                if (item.TotalCube.ToString().ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.TotalWeight != null)
                                if (item.TotalWeight.ToString().ToLower().Contains(FilterText.ToLower()))
                                    return true;
                        }
                        return false;
                    }
                    else
                    {
                        bool result = MakeStringFilter(item, SelectedColumn, SelectedCondition);
                        return result;
                    }
                }
            }
            return false;
        }

        #endregion

        #endregion
    }
}
