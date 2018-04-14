using Microsoft.AppCenter.Analytics;
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
    public class ImageViewerViewModel:ViewModelBase
    {
        public object Parent { get; set; }
        public ImageViewerViewModel(ProductionViewModel productionViewModel)
        {
            Parent = productionViewModel;
        }
        public ImageViewerViewModel(ShippingViewModel productionViewModel)
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
                Analytics.TrackEvent("Going to Save the Image, Image Source:" + ImageSource);
            
                
                Analytics.TrackEvent("Starting SaveImageMethod");

                if (MediaFile != null)
                {

                    Analytics.TrackEvent("Media File is not null");

                    IsBusy = true;
                    var content = new MultipartFormDataContent();
                    Analytics.TrackEvent("Going to call save method, ImgFileName: "+ImgFileName);
                    content.Add(new StreamContent(MediaFile.GetStream()),
                        "\"file\"",
                        $"\"{ImgFileName}\"");

                    var httpClient = new HttpClient();

                    var uploadServiceBaseAddress = App.BASE_PROD_URL + "UploadImage";

                    var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);
                    IsBusy = false;
                    Analytics.TrackEvent(httpResponseMessage.StatusCode.ToString());
                    Analytics.TrackEvent(httpResponseMessage.ToString());
                   
                    if (Parent.GetType()==typeof(ProductionViewModel))
                    {
                        Analytics.TrackEvent("Going to refresh the files list");
                      
                        ((ProductionViewModel)Parent).RefreshFilesList();
                    }
                    else
                    {
                        ((ShippingViewModel)Parent).RefreshFilesList();
                    }
                    await PopupNavigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Analytics.TrackEvent("Some Exception occured " + ex.Message);
              
                IsBusy = false;
            }
        }
    }
}
