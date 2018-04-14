using MXApi.Models;
using MXApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MXApp.Views;
using Microsoft.AppCenter.Crashes;
using System.Linq;
using System.Globalization;
using Rg.Plugins.Popup.Services;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Media.Abstractions;
using System.IO;
using MXApp.Services.Downloader;
using MXApp.Services.Save;

namespace MXApp.ViewModels
{
    public class ShippingViewModel : ViewModelBase
    {
        private AsyncCommand viewDocumentCommand;
        private AsyncCommand sendMailCommand;
        private ObservableCollection<ProdFileItem> fileList;
        private ObservableCollection<LocationsListModel> locationList;
        private ShippingView spView;

        public ShippingView SpView
        {
            get { return spView; }
            set { spView = value; OnPropertyChanged(); }
        }

        public ShippingViewModel()
        {
        }
        public ShippingViewModel(ShippingView shippingView)
        {
            SpView = shippingView;
            SelectedDate = DateTime.Now;
        }
        private ObservableCollection<ViewLoadModel> viewLoadsList;

        public ObservableCollection<ViewLoadModel> ViewLoadsList
        {
            get
            {
                return viewLoadsList;
            }
            set
            {
                viewLoadsList = value;
                OnPropertyChanged();
            }
        }

        private ViewLoadModel viewLoad;

        public ViewLoadModel ViewLoad
        {
            get { return viewLoad; }
            set { viewLoad = value; OnPropertyChanged(); }
        }

        private bool isGridBusy;

        public bool IsGridBusy
        {
            get { return isGridBusy; }
            set { isGridBusy = value; OnPropertyChanged(); }
        }
        private DateTime selectedDate;

        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                selectedDate = value;
                OnPropertyChanged();
            }
        }
        private WHS selectedWareHouse;

        public WHS SelectedWareHouse
        {
            get
            {
                return selectedWareHouse;
            }
            set
            {
                selectedWareHouse = value;
                OnPropertyChanged();
                RefreshGrids();
            }
        }
        private string titleString;

        public string TitleString
        {
            get { return titleString; }
            set { titleString = value; OnPropertyChanged(); }
        }

        public async Task RefreshGrids()
        {
            IsBusy = true;
            await Task.Run(() => LoadViewLoadItems(SelectedDate));

        }

        private async void LoadViewLoadItems(DateTime dt)
        {
            try
            {
                string uri = App.BASE_SHIPPING_URL + "GetShippingViewLoads";
                GetShippingViewLoadsModel model = new GetShippingViewLoadsModel();
                model.WHS = SelectedWareHouse.WHS;
                model.Date = dt;
                var res = await Task.Run(() => App.ServiceHelper.PostAsync<GetShippingViewLoadsModel, List<ViewLoadModel>>(uri, model));
                if (res != null)
                {
                    ViewLoadsList = new ObservableCollection<ViewLoadModel>(res);
                }
                IsBusy = false;
                if (FilterText != "")
                    await ReApplyFilter();

            }
            catch (Exception ex)
            {
                await App.DialogService.ShowAlertAsync(ex.Message, "Error", "ok");
                IsBusy = false;
                Crashes.TrackError(ex);
            }
        }
        public async Task ReApplyFilter()
        {
            //await Task.Delay(500);
            if (FilterText != "")
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    OnFilterTextChanged();
                });
               
            }
        }
        public async void RefreshFilesList()
        {
            if (ViewLoad != null)
                await Task.Run(() => LoadFilesList(ViewLoad.PickUp_LoadNum));
        }
        public async void RefreshLocationList()
        {
            if (ViewLoad != null)
                await Task.Run(() => LoadLocationsList(ViewLoad.PickUp_LoadNum));
        }
        private async Task LoadFilesList(string PickUp_LoadNum)
        {
            try
            {
                IsBusy = true;
                string uri = App.BASE_SHIPPING_URL + "GetFilesList/" + PickUp_LoadNum;
                var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<ProdFileItem>>(uri));
                if (res != null)
                {
                    FilesList = new ObservableCollection<ProdFileItem>(res.OrderByDescending(x => x.PODBorn).ToList());
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

        private async Task LoadLocationsList(string PickUp_LoadNum)
        {

            try
            {
                IsBusy = true;
                string uri = App.BASE_SHIPPING_URL + "GetLocationsList/" + PickUp_LoadNum;
                var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<LocationsListModel>>(uri));
                if (res != null)
                {
                    LocationsList = new ObservableCollection<LocationsListModel>(res);
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
        private AsyncCommand openCameraCommand;
        private AsyncCommand pullToRefreshCommand;
        private AsyncCommand editWOCommand;

        public AsyncCommand OpenCameraCommand
        {
            get
            {
                if (openCameraCommand == null)
                    openCameraCommand = new AsyncCommand(OpenCameraCommandMethod);
                return openCameraCommand;
            }
        }

        private async Task OpenCameraCommandMethod()
        {
            try
            {
                if (ViewLoad != null)
                {
                    if (CrossMedia.Current.IsCameraAvailable)
                    {
                        var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                        var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

                        if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
                        {
                            var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                            cameraStatus = results[Permission.Camera];
                            storageStatus = results[Permission.Storage];
                        }

                        if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
                        {
                            Random random = new Random();
                            string rand = random.Next(10000, 999999).ToString();
                            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                            {
                                SaveToAlbum = false,
                                CompressionQuality = 30,
                                PhotoSize = PhotoSize.Small,
                                Name = ViewLoad.PickUp_LoadNum + "^" + App.UserName + "^" + rand + "MOXIE.jpeg",
                                AllowCropping = true,
                                DefaultCamera = CameraDevice.Rear,
                                SaveMetaData = true
                            });
                            var path = file.Path;
                            PreviewBeforeUpload img = new PreviewBeforeUpload();
                            ShippingImagePreviewViewModel vm = new ShippingImagePreviewViewModel(this);
                            vm.ImageSource = path;
                            vm.MediaFile = file;
                            vm.ImgFileName = new FileInfo(file.Path).Name;
                            FileInfo info = new FileInfo(file.Path);
                            if (info.Name.Contains("^"))
                                vm.ImgFileName = info.Name;
                            else
                                vm.ImgFileName = ViewLoad.PickUp_LoadNum + "^" + App.UserName + "^" + rand + "MOXIE.jpeg";

                            img.BindingContext = vm;
                            img.BindingContext = vm;
                            await PopupNavigation.PushAsync(img);
                        }
                        else
                        {
                            await App.DialogService.ShowAlertAsync("Permissions Denied", "Unable to take photos.", "OK");
                        }
                    }
                }
                else
                {
                    await App.DialogService.ShowAlertAsync("Please select a valid Pickup Load Number", "Error", "ok");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        public ObservableCollection<ProdFileItem> FilesList
        {
            get { return fileList; }
            set { this.fileList = value; OnPropertyChanged(); }
        }
        public ObservableCollection<LocationsListModel> LocationsList
        {
            get { return locationList; }
            set { this.locationList = value; OnPropertyChanged(); }
        }
        private LocationsListModel locations;

        public LocationsListModel Location
        {
            get { return locations; }
            set { locations = value; OnPropertyChanged(); }
        }

        internal int ItemIndex
        {
            get;
            set;
        }
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
            IsGridBusy = true;
            await Task.Run(() => LoadViewLoadItems(SelectedDate));
            IsGridBusy = false;
        }

        public AsyncCommand EditViewLoadCommand
        {
            get
            {
                if (editWOCommand == null)
                    editWOCommand = new AsyncCommand(EditViewLoadCommandMethod);
                return editWOCommand;
            }
        }

        private async Task EditViewLoadCommandMethod()
        {
            EditViewModePopup editViewModePopup = new EditViewModePopup();
            editViewModePopup.BindingContext = new EditViewLoadViewModel(this);
            await PopupNavigation.PushAsync(editViewModePopup, true);
        }

        public async void UpdateItem(EditViewLoadViewModel vm)
        {
            try
            {
                IsBusy = true;
                ViewLoadModel model = new ViewLoadModel();
                model.EDIAPPID = ViewLoad.EDIAPPID;
                model.DepartingDoor = vm.DepartingDoor;
                model.AppIDComments = vm.AppIDComments;
                string uri = App.BASE_SHIPPING_URL + "UpdateViewLoadItem";
                var res = await Task.Run(() => App.ServiceHelper.PostAsync<ViewLoadModel, int>(uri, model));
                if (res > 0)
                {
                    await App.DialogService.ShowAlertAsync("Item Updated Successfully", "Updated", "Ok");
                    RefreshGrids();
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsBusy = false;
            }
        }

        private AsyncCommand calendarCommand;

        public AsyncCommand CalendarCommand
        {
            get
            {
                if (calendarCommand == null)
                    calendarCommand = new AsyncCommand(CalendarCommandMethod);
                return calendarCommand;
            }
        }

        private async Task CalendarCommandMethod()
        {
            DatePickerPopup popup = new DatePickerPopup();
            DatePickerPopupShippingViewModel dtpViewModel = new DatePickerPopupShippingViewModel(this, SpView);
            popup.BindingContext = dtpViewModel;
            await PopupNavigation.PushAsync(popup, true);
        }

        private AsyncCommand selectionModeCommand;

        public AsyncCommand SelectionModeCommand
        {
            get
            {
                if (selectionModeCommand == null)
                    selectionModeCommand = new AsyncCommand(SelectionModeCommandMethod);
                return selectionModeCommand;
            }
        }

        private async Task SelectionModeCommandMethod()
        {
            if (ViewLoad != null)
            {

                IsBusy = true;
                LoadDetailsPage loadDetailsPage = new LoadDetailsPage();
                LoadDetailsPageViewModel viewModel = new LoadDetailsPageViewModel(ViewLoad.EDIAPPID, ViewLoad.PickUp_LoadNum, this);
                await viewModel.LoadDetails();
                loadDetailsPage.BindingContext = viewModel;
                await PopupNavigation.PushAsync(loadDetailsPage, true);
                IsBusy = false;
            }
            else
            {
                await App.DialogService.ShowAlertAsync("Please select a valid Pickup Load Number", "Error", "OK");
            }
        }

        internal Syncfusion.ListView.XForms.SfListView sfListView;

        public AsyncCommand ViewDocumentCommand
        {
            get
            {
                if (viewDocumentCommand == null)
                    viewDocumentCommand = new AsyncCommand(ViewDocumentCommandMethod);
                return viewDocumentCommand;
            }
        }

        public AsyncCommand SendMailCommand
        {
            get
            {
                if (sendMailCommand == null)
                    sendMailCommand = new AsyncCommand(SendMailCommandMethod);
                return sendMailCommand;
            }
        }
        private async Task SendMailCommandMethod()
        {
            var file = FilesList[ItemIndex];
            switch (file.FileType)
            {
                case PODFileTypes.PDF:
                    {
                        await SendDocumentMail(file.PODID);
                    }
                    break;
                case PODFileTypes.Excel:
                    {
                        await SendDocumentMail(file.PODID);
                    }
                    break;
                case PODFileTypes.Docx:
                    {
                        await SendDocumentMail(file.PODID);
                    }
                    break;
                case PODFileTypes.Image:
                    {
                        await SendDocumentMail(file.PODID);
                    }
                    break;
                case PODFileTypes.Other:
                    {
                        await App.DialogService.ShowAlertAsync("Other file types not supported", "Error", "Ok");
                    }
                    break;
                default:
                    break;
            }
        }
        private async Task ViewDocumentCommandMethod()
        {
            var file = FilesList[ItemIndex];
            switch (file.FileType)
            {
                case PODFileTypes.PDF:
                    {
                        await LoadAndShowPDFViewer(file.PODID);
                    }
                    break;
                case PODFileTypes.Excel:
                    {
                        await LoadAndDownloadExcel(file.PODID);
                    }
                    break;
                case PODFileTypes.Docx:
                    {
                        await LoadAndDownloadWord(file.PODID);
                    }
                    break;
                case PODFileTypes.Image:
                    {
                        await LoadAndShowImageViewer(file.PODID);
                    }
                    break;
                case PODFileTypes.Other:
                    {
                        await App.DialogService.ShowAlertAsync("Other file types not supported", "Error", "Ok");
                    }
                    break;
                default:
                    break;
            }
        }

        private async Task LoadAndShowImageViewer(int pODID)
        {
            try
            {
                IsBusy = true;
                string uri = App.BASE_PROD_URL + "GetImageFile/" + pODID;
                var res = await Task.Run(() => App.ServiceHelper.GetAsync<string>(uri));
                if (!string.IsNullOrEmpty(res))
                {
                    var FileURL = App.BASE_WEB_URL + "MobileFile\\" + res;
                    ImageViewer popup = new ImageViewer();
                    string url = DependencyService.Get<IDownloader>().GetLocalFileUrl(FileURL, res);
                    ImageViewerViewModel vm = new ImageViewerViewModel(this);
                    vm.ImageSource = url;
                    popup.BindingContext = vm;
                    await PopupNavigation.PushAsync(popup, true);
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }

        private async Task LoadAndDownloadWord(int pODID)
        {
            try
            {
                IsBusy = true;
                string uri = App.BASE_PROD_URL + "GetWordFile/" + pODID;
                var res = await Task.Run(() => App.ServiceHelper.GetAsync<string>(uri));
                if (!string.IsNullOrEmpty(res))
                {
                    var FileURL = App.BASE_WEB_URL + "MobileFile\\" + res;
                    Stream stream = DependencyService.Get<IDownloader>().DownloadPdfStream(FileURL, res);
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    if (Device.OS == TargetPlatform.WinPhone || Device.OS == TargetPlatform.Windows)
                        DependencyService.Get<ISaveWindowsPhone>().Save(res, "application/msword", memoryStream);
                    else
                        DependencyService.Get<ISave>().Save(res, "application/msword", memoryStream);
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }

        private async Task LoadAndDownloadExcel(int pODID)
        {
            try
            {
                IsBusy = true;
                string uri = App.BASE_PROD_URL + "GetExcelFile/" + pODID;
                var res = await Task.Run(() => App.ServiceHelper.GetAsync<string>(uri));
                if (!string.IsNullOrEmpty(res))
                {
                    var FileURL = App.BASE_WEB_URL + "MobileFile\\" + res;
                    Stream stream = DependencyService.Get<IDownloader>().DownloadPdfStream(FileURL, res);
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    if (Device.OS == TargetPlatform.WinPhone || Device.OS == TargetPlatform.Windows)
                        DependencyService.Get<ISaveWindowsPhone>().Save(res, "application/msexcel", memoryStream);
                    else
                        DependencyService.Get<ISave>().Save(res, "application/msexcel", memoryStream);
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }

        private async Task LoadAndShowPDFViewer(int pODID)
        {
            try
            {
                IsBusy = true;
                string uri = App.BASE_PROD_URL + "GetPDFFile/" + pODID;
                var res = await Task.Run(() => App.ServiceHelper.GetAsync<string>(uri));
                if (!string.IsNullOrEmpty(res))
                {
                    var FileURL = App.BASE_WEB_URL + "MobileFile\\" + res;
                    PDFViewerPopup popup = new PDFViewerPopup();
                    popup.FileUrl = FileURL;
                    popup.FileName = res;
                    await PopupNavigation.PushAsync(popup, true);
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }

        private async Task SendDocumentMail(int pODID)
        {
            try
            {
                IsBusy = true;
                string uri = App.BASE_SHIPPING_URL + "SendDocumentMail";
                SendDocumentMailModel model = new SendDocumentMailModel();
                model.PODID = pODID;
                model.VendorID = App.UserName;
                var res = await Task.Run(() => App.ServiceHelper.PostAsync<SendDocumentMailModel, string>(uri, model));
                if (res.StartsWith("Exception"))
                {
                    await App.DialogService.ShowAlertAsync("Error Occured" + res, "Error", "ok");
                }
                else
                {
                    await App.DialogService.ShowAlertAsync("Mail Sent Successfully", "Success", "ok");
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        private AsyncCommand pullToRefreshLocations;

        public AsyncCommand PullToRefreshLocations
        {
            get
            {
                if (pullToRefreshCommand == null)
                    pullToRefreshCommand = new AsyncCommand(PullToRefreshLocationsMethod);
                return pullToRefreshLocations;
            }
        }

        private async Task PullToRefreshLocationsMethod()
        {
            await Task.Run(() => LoadLocationsList(ViewLoad.PickUp_LoadNum));
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

        private bool MakeStringFilter(ViewLoadModel o, string option, string condition)
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

        private bool MakeNumericFilter(ViewLoadModel o, string option, string condition)
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
            var item = o as ViewLoadModel;
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
                            if (item.AppIDComments != null)
                                if (item.AppIDComments.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.Carrier != null)
                                if (item.Carrier.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.DepartingDoor != null)
                                if (item.DepartingDoor.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.PickUp_LoadNum != null)
                                if (item.PickUp_LoadNum.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.SCAC != null)
                                if (item.SCAC.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.SealNumber != null)
                                if (item.SealNumber.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.WHS != null)
                                if (item.WHS.ToLower().Contains(FilterText.ToLower()))
                                    return true;
                            if (item.EDIAPPID != null)
                                if (item.EDIAPPID.ToString().ToLower().Contains(FilterText.ToLower()))
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
