using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MXApi.Models;
using MXApp.Services.Downloader;
using MXApp.Services.FileLaunch;
using MXApp.Services.Save;
using MXApp.ViewModels.Base;
using MXApp.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Messaging;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace MXApp.ViewModels
{
    public class ProductionViewModel : ViewModelBase
    {

        #region Members
        private SearchWOPopupViewModel searchWOPopupViewModel;
        private ProdView prodView;
        private bool isGridBusy;
        private ObservableCollection<ProdBillCodes> billingCodesList;
        private ObservableCollection<ProdWODetail> prodWOList;
        private string title;
        private ProdOpenOrders selectedRef;
        private AsyncCommand pullToRefreshCommand;
        private int? selectedBillCode;
        private ProdWODetail prodWO;
        private SwipeDirection swipeDirection;
        private AsyncCommand editWOCommand;
        private AsyncCommand deleteWOCommand;
        private AsyncCommand addBillingCodeCommand;
        private AsyncCommand confirmWOCommand;
        private ObservableCollection<ProdFileItem> fileList;
        private AsyncCommand viewDocumentCommand;
        private AsyncCommand sendMailCommand;
        internal Syncfusion.ListView.XForms.SfListView sfListView;
        private AsyncCommand openCameraCommand;

        #endregion

        #region Constructor
        public ProductionViewModel(ProdView pv)
        {
            ProdView = pv;
        }
        public ProductionViewModel()
        {

        }
        #endregion

        #region Properties
        public SearchWOPopupViewModel SearchWOPopupViewModel
        {
            get
            {
                return searchWOPopupViewModel;
            }
            set
            {
                searchWOPopupViewModel = value;
                OnPropertyChanged();
            }
        }
        public ProdView ProdView
        {
            get
            {
                return prodView;
            }
            set
            {
                prodView = value;
                OnPropertyChanged();
            }
        }
        public bool IsGridBusy
        {
            get
            {
                return isGridBusy;
            }
            set
            {
                isGridBusy = value;
                OnPropertyChanged();
            }
        }
        public string TitleString
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProdBillCodes> BillingCodesList
        {
            get
            {
                return billingCodesList;
            }
            set
            {
                billingCodesList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ProdWODetail> ProdWOList
        {
            get
            {
                return prodWOList;
            }
            set
            {
                prodWOList = value;
                OnPropertyChanged();
            }
        }
        public ProdOpenOrders SelectedRef
        {
            get { return selectedRef; }
            set
            {
                selectedRef = value;
                OnPropertyChanged();
                RefreshGrid();
            }
        }
        public int? SelectedBillCode
        {
            get { return selectedBillCode; }
            set { selectedBillCode = value; OnPropertyChanged(); }
        }
        public ProdWODetail ProdWO
        {
            get { return prodWO; }
            set { prodWO = value; OnPropertyChanged(); }
        }
        public SwipeDirection SwipeDirection
        {
            get { return swipeDirection; }
            set { swipeDirection = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ProdFileItem> FilesList
        {
            get { return fileList; }
            set { this.fileList = value; OnPropertyChanged(); }
        }
        internal int ItemIndex
        {
            get;
            set;
        }
        #endregion

        #region Commands
        public AsyncCommand PullToRefreshCommand
        {
            get
            {
                if (pullToRefreshCommand == null)
                    pullToRefreshCommand = new AsyncCommand(PullToRefreshCommandMethod);
                return pullToRefreshCommand;
            }
        }

        public AsyncCommand EditWOCommand
        {
            get
            {
                if (editWOCommand == null)
                    editWOCommand = new AsyncCommand(EditWOCommandMethod);
                return editWOCommand;
            }
        }
        public AsyncCommand DeleteWOCommand
        {
            get
            {
                if (deleteWOCommand == null)
                    deleteWOCommand = new AsyncCommand(DeleteWOCommandMethod);
                return deleteWOCommand;
            }
        }
        public AsyncCommand ConfirmWOCommand
        {
            get
            {
                if (confirmWOCommand == null)
                    confirmWOCommand = new AsyncCommand(ConfirmWOCommandMethod);
                return confirmWOCommand;
            }
        }
        public AsyncCommand AddBillingCodeCommand
        {
            get
            {
                if (addBillingCodeCommand == null)
                    addBillingCodeCommand = new AsyncCommand(AddBillingCodeCommandMethod);
                return addBillingCodeCommand;
            }
        }
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
                Analytics.TrackEvent("Starting of Open Camera Method");
                Analytics.TrackEvent("Selected Ref Value=" + SelectedRef.ReferencePullID);

                if (SelectedRef != null)
                {
                    Analytics.TrackEvent("IS Camera Available=" + CrossMedia.Current.IsCameraAvailable);
                    if (CrossMedia.Current.IsCameraAvailable)
                    {
                        var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                        var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                        Analytics.TrackEvent("Camera Status=" + cameraStatus.ToString());
                        Analytics.TrackEvent("Storage Status=" + storageStatus.ToString());

                        if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
                        {
                            Analytics.TrackEvent("Permission Not granted so granting it now");
                            var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                            cameraStatus = results[Permission.Camera];
                            storageStatus = results[Permission.Storage];
                            Analytics.TrackEvent("Obtaining Camera Status=" + cameraStatus.ToString());
                            Analytics.TrackEvent("Obtaining Storage Status=" + storageStatus.ToString());
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
                                Name = SelectedRef.ReferencePullID + "^" + App.UserName + "^" + rand + "MOXIE.jpeg",
                                AllowCropping = true,
                                DefaultCamera = CameraDevice.Rear,
                                SaveMetaData = true
                            });
                            if (file != null)
                            {
                                Analytics.TrackEvent("Image Captured, File Path" + file.Path);

                                var path = file.Path;
                                PreviewBeforeUpload img = new PreviewBeforeUpload();
                                ImageViewerViewModel vm = new ImageViewerViewModel(this);
                                vm.ImageSource = path;
                                vm.MediaFile = file;
                                FileInfo info = new FileInfo(file.Path);
                                if (info.Name.Contains("^"))
                                    vm.ImgFileName = info.Name;
                                else
                                    vm.ImgFileName = SelectedRef.ReferencePullID + "^" + App.UserName + "^" + rand + "MOXIE.jpeg";

                                img.BindingContext = vm;
                                await PopupNavigation.PushAsync(img);
                            }
                            else
                            {
                                Analytics.TrackEvent("Image Not Captured");
                            }
                        }
                        else
                        {
                            await App.DialogService.ShowAlertAsync("Permissions Denied", "Unable to take photos.", "OK");
                        }
                    }
                }
                else
                {
                    await App.DialogService.ShowAlertAsync("Please select a work order first", "Error", "ok");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        #endregion

        #region Methods
        private async Task LoadBillingCodes(string account)
        {
            this.IsBusy = true;
            try
            {
                string uri = App.BASE_PROD_URL + "GetBillOrdersList/" + account;
                var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<ProdBillCodes>>(uri));
                if (res != null)
                {
                    BillingCodesList = new ObservableCollection<ProdBillCodes>(res);
                }
                await Task.Run(() => LoadWOList(SelectedRef.ReferencePullID));
            }
            catch (Exception ex)
            {
                await App.DialogService.ShowAlertAsync(ex.Message, "Error", "ok");
                IsBusy = false;

                Crashes.TrackError(ex);

            }
            IsBusy = false;
        }
        private async Task LoadWOList(int RefNum)
        {
            try
            {
                string uri = App.BASE_PROD_URL + "ListWOProds/" + RefNum;
                var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<ProdWODetail>>(uri));
                if (res != null)
                {
                    ProdWOList = new ObservableCollection<ProdWODetail>(res);
                }
            }
            catch (Exception ex)
            {
                await App.DialogService.ShowAlertAsync(ex.Message, "Error", "ok");
                IsBusy = false;
                Crashes.TrackError(ex);
            }
        }
        private async Task LoadFilesList(string RefNum)
        {
            try
            {
                IsBusy = true;
                string uri = App.BASE_PROD_URL + "GetFilesList/" + RefNum;
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
        private async void RefreshGrid()
        {
            TitleString = "Production " + SelectedRef.ReferencePullID;
            OnPropertyChanged("TitleString");
            IsBusy = true;
            await Task.Run(() => LoadBillingCodes(SelectedRef.Account));
            await Task.Run(() => RefreshFilesList());
            await Task.Run(() => UpdateWO(SelectedRef.ReferencePullID));
            IsBusy = false;
        }

        private async Task UpdateWO(int RefNum)
        {
            try
            {
                IsBusy = true;
                string uri = App.BASE_PROD_URL + "UpdateWO/" + RefNum;
                var res = await Task.Run(() => App.ServiceHelper.GetAsync<int>(uri));
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsBusy = false;
            }
        }

        private async void RefreshDataGrid()
        {
            BillCodeText = "";
            await Task.Run(() => LoadWOList(SelectedRef.ReferencePullID));
        }
        public async void RefreshFilesList()
        {
            await Task.Run(() => LoadFilesList(SelectedRef.ReferencePullID.ToString()));
        }
        private async Task PullToRefreshCommandMethod()
        {
            IsGridBusy = true;
            await Task.Run(() => LoadWOList(SelectedRef.ReferencePullID));
            IsGridBusy = false;
        }
        private async Task EditWOCommandMethod()
        {
            try
            {
                if (this.ProdWO != null)
                {
                    EditProdWO editProdWO = new EditProdWO();
                    editProdWO.BindingContext = new EditProdWOViewModel(this);
                    await PopupNavigation.PushAsync(editProdWO, true);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        private async Task DeleteWOCommandMethod()
        {
            try
            {
                if (this.ProdWO != null)
                {
                    await DeleteItem(this.ProdWO.CodeID);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        public async void UpdateItem(EditProdWOViewModel vm)
        {
            try
            {
                IsBusy = true;
                ProdWODetail model = new ProdWODetail();
                model.ReferencePullID = ProdWO.ReferencePullID;
                model.CodeID = ProdWO.CodeID;
                model.QTY = string.IsNullOrEmpty(vm.QTY) == false ? Convert.ToInt32(vm.QTY) : 0;
                model.BillComments = vm.BillComments;
                string uri = App.BASE_PROD_URL + "UpdateProdWOItem";
                var res = await Task.Run(() => App.ServiceHelper.PostAsync<ProdWODetail, int>(uri, model));
                if (res > 0)
                {
                    await App.DialogService.ShowAlertAsync("Item Updated Successfully", "Updated", "Ok");
                    RefreshDataGrid();
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsBusy = false;
            }
        }
        public async Task DeleteItem(int? codeID)
        {
            try
            {
                IsBusy = true;
                string uri = App.BASE_PROD_URL + "DeleteProdItem";
                DeleteBillingCodeModel model = new DeleteBillingCodeModel();
                model.Account = SelectedRef.Account;
                model.CodeID = codeID.Value;
                model.RefNum = SelectedRef.ReferencePullID;
                model.UserName = App.UserName;
                model.Vendor = SelectedRef.Account;
                var res = await Task.Run(() => App.ServiceHelper.PostAsync<DeleteBillingCodeModel, int>(uri, model));

                await App.DialogService.ShowAlertAsync("Item Deleted Successfully", "Delete", "Ok");
                RefreshDataGrid();

                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsBusy = false;
            }
        }
        private async Task AddBillingCodeCommandMethod()
        {
            try
            {
                if (SelectedRef != null && BillingCodesList != null && SelectedBillCode != null)
                {
                    IsBusy = true;
                    AddBillingCodeModel model = new AddBillingCodeModel();
                    model.Account = SelectedRef.Account;
                    model.RefNum = SelectedRef.ReferencePullID;
                    model.BillCode = SelectedBillCode.Value;
                    model.UserName = App.UserName;
                    model.Vendor = SelectedRef.Account;
                    string uri = App.BASE_PROD_URL + "InsertBillingCode";
                    var res = await Task.Run(() => App.ServiceHelper.PostAsync<AddBillingCodeModel, int>(uri, model));

                    //await App.DialogService.ShowAlertAsync("Bill Code Added Successfully", "Updated", "Ok");

                    RefreshDataGrid();
                    BillCodeText = "";
                    IsBusy = false;
                }
                else
                {
                    await App.DialogService.ShowAlertAsync("No Ref Num or Billing Code Selected", "Error", "Ok");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsBusy = false;
            }
        }
        private async Task ConfirmWOCommandMethod()
        {
            try
            {
                if (SelectedRef != null)
                {
                    IsBusy = true;
                    try
                    {
                        string clockoutURI = App.BASE_PROD_URL + "ClockOutAll/" + SelectedRef.ReferencePullID;
                        await Task.Run(() => App.ServiceHelper.GetAsync<object>(clockoutURI));
                    }
                    catch (Exception ex)
                    {
                        await App.DialogService.ShowAlertAsync(ex.Message, "Error", "ok");
                        IsBusy = false;
                    }
                    string uri = App.BASE_PROD_URL + "ConfirmWO";
                    ConfirmWOModel model = new ConfirmWOModel();
                    model.Account = SelectedRef.Account;
                    model.RefNum = SelectedRef.ReferencePullID;
                    model.BillCode = 0;
                    model.UserName = App.UserName;
                    model.Vendor = SelectedRef.Account;
                    var res = await Task.Run(() => App.ServiceHelper.PostAsync<ConfirmWOModel, int>(uri, model));
                    await App.DialogService.ShowAlertAsync("Work Item Confirmed Successfully", "Confirmed", "Ok");
                    RefreshFilesList();
                    IsBusy = false;
                }
                else
                {
                    await App.DialogService.ShowAlertAsync("No Ref Num Selected", "Error", "Ok");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsBusy = false;
            }
        }
        public async Task LoadBarcodeScanner(SearchWOPopupViewModel searchWOPopupViewModel)
        {
            try
            {
                CustomScanPage customScanPage = new CustomScanPage();
                customScanPage.zxing.OnScanResult += Zxing_OnScanResult;
                SearchWOPopupViewModel = searchWOPopupViewModel;
                await App.MainView.Navigation.PushModalAsync(customScanPage);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        private void Zxing_OnScanResult(ZXing.Result result)
        {
            try
            {

                if (!string.IsNullOrEmpty(result.Text))
                {
                    SearchWOPopupViewModel.SelectedRefNum = Convert.ToInt32(result.Text);
                    SearchWOPopup popup = new SearchWOPopup(this, ProdView, SearchWOPopupViewModel);
                    SearchWOPopupViewModel.SelectedRefText = result.Text;

                    Thread.Sleep(500);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        prodView.Title = "Production " + result.Text;
                        await PopupNavigation.PushAsync(popup, false);
                    });

                }
                else
                {
                    App.DialogService.ShowAlertAsync("Not able to scan barcode, or this barcode value is not a proper Reference Number(Code 128)", "Error", "ok");
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
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
                string uri = App.BASE_PROD_URL + "SendDocumentMail";
                SendDocumentMailModel model = new SendDocumentMailModel();
                model.PODID = pODID;
                model.VendorID = SelectedRef.Account;
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
        #endregion


        private AsyncCommand openEmployeeCheckin;

        public AsyncCommand OpenEmployeeCheckin
        {
            get
            {
                if (openEmployeeCheckin == null)
                    openEmployeeCheckin = new AsyncCommand(OpenEmployeeCheckinMethod);
                return openEmployeeCheckin;
            }
        }

        private async Task OpenEmployeeCheckinMethod()
        {
            try
            {
                if (SelectedRef != null)
                {
                    IsBusy = true;
                    EmployeeTrackPopup popup = new EmployeeTrackPopup();
                    EmployeeTrackViewModel vm = new EmployeeTrackViewModel(SelectedRef);
                    await vm.LoadEmployeeList();
                    popup.BindingContext = vm;
                    await PopupNavigation.PushAsync(popup, true);
                    IsBusy = false;
                }
                else
                {
                    await App.DialogService.ShowAlertAsync("Please select an work order first", "Error", "ok");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        private AsyncCommand readDocumentCommand;

        public AsyncCommand ReadDocumentCommand
        {
            get
            {
                if (readDocumentCommand == null)
                    readDocumentCommand = new AsyncCommand(x => ReadDocumentCommandMethod(x));
                return readDocumentCommand;
            }
        }

        private async Task ReadDocumentCommandMethod(object x)
        {

        }

        private string billCodeText;

        public string BillCodeText
        {
            get { return billCodeText; }
            set { billCodeText = value; OnPropertyChanged(); }
        }

    }
}
