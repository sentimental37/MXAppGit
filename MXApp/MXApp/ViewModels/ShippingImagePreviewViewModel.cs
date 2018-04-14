using Microsoft.AppCenter.Crashes;
using MXApp.ViewModels.Base;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MXApp.ViewModels
{
    public class ShippingImagePreviewViewModel:ViewModelBase
    {
        public ShippingViewModel Parent { get; set; }
        public ShippingImagePreviewViewModel(ShippingViewModel productionViewModel)
        {
            Parent = productionViewModel;
        }
        private string imageSource;

        public string ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                imageSource = value;
                OnPropertyChanged();

            }
        }
        private string imgFileName;

        public string ImgFileName
        {
            get { return imgFileName; }
            set { imgFileName = value; OnPropertyChanged(); }
        }

        private AsyncCommand saveImage;

        public AsyncCommand SaveImageCommand
        {
            get
            {
                if (saveImage == null)
                    saveImage = new AsyncCommand(SaveImageMethod);
                return saveImage;
            }
        }
        private MediaFile _MediaFile;

        public MediaFile MediaFile
        {
            get { return _MediaFile; }
            set { _MediaFile = value; OnPropertyChanged(); }
        }

        private async Task SaveImageMethod()
        {
            try
            {
                if (MediaFile != null)
                {
                    IsBusy = true;
                    var content = new MultipartFormDataContent();

                    content.Add(new StreamContent(MediaFile.GetStream()),
                        "\"file\"",
                        $"\"{ImgFileName}\"");

                    var httpClient = new HttpClient();

                    var uploadServiceBaseAddress = App.BASE_SHIPPING_URL + "UploadImage";

                    var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);
                    IsBusy = false;
                    Parent.RefreshFilesList();
                    await PopupNavigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsBusy = false;
            }
        }
    }
}
