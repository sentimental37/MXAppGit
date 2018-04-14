using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace MXApp.Views
{
    public class CustomScanPage : ContentPage
    {
        public ZXingScannerView zxing;
        ZXingDefaultOverlay overlay;

        public CustomScanPage() : base()
        {
            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            zxing.OnScanResult += (result) =>
                Device.BeginInvokeOnMainThread(async () => {
                    await Navigation.PopModalAsync();
                });

            overlay = new ZXingDefaultOverlay
            {
                TopText = "Hold your phone up to the barcode",
                BottomText = "Scanning will happen automatically",
                ShowFlashButton = true,
            };
            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            overlay.FlashButtonClicked += (sender, e) => {
                zxing.IsTorchOn = !zxing.IsTorchOn;
            };
            
            zxing.BackgroundColor = Color.OrangeRed;
            grid.Children.Add(zxing);
            grid.Children.Add(overlay);
            Button btn = new Button();
            btn.Text = "Cancel";
            btn.VerticalOptions = LayoutOptions.End;
            btn.HorizontalOptions = LayoutOptions.End;
            btn.WidthRequest = 100;
            btn.Clicked += Btn_Clicked;
            btn.BackgroundColor = Color.Red;
            btn.TextColor = Color.White;
            grid.Children.Add(btn);
            Content = grid;
        }

        private async void Btn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            zxing.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;

            base.OnDisappearing();
        }
    }
}
