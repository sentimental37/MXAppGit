using MXApp.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MXApp.Views
{
	public partial class ConfirmPopup : PopupPage
	{
        public ConfirmPopup(LoadDetailsPageViewModel vm)
        {
            InitializeComponent();
            ParetnVM = vm;
            imgNo.GestureRecognizers.Add(new TapGestureRecognizer() { NumberOfTapsRequired = 1, TappedCallback = NoTapped });
            imgYes.GestureRecognizers.Add(new TapGestureRecognizer() { NumberOfTapsRequired = 1, TappedCallback = YesTapped });
        }
        private LoadDetailsPageViewModel paretnVM;

        public LoadDetailsPageViewModel ParetnVM
        {
            get { return paretnVM; }
            set { paretnVM = value; OnPropertyChanged(); }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }
        
        private void No_Tapped(object sender,TappedEventArgs e)
        {
            PopupNavigation.PopAsync();
        }
        private async void Yes_Tapped(object sender, TappedEventArgs e)
        {
            await ParetnVM.ShippedYesMethod();
            await PopupNavigation.PopAsync();
        }
        private void noImage_BindingContextChanged(object sender, EventArgs e)
        {

            var leftImage = sender as Image;
       
            (leftImage.Parent as View).GestureRecognizers.Add(new TapGestureRecognizer() { NumberOfTapsRequired = 1, TappedCallback= NoTapped });
        }

        private void NoTapped(View arg1, object arg2)
        {
            PopupNavigation.PopAsync();
        }

        private void yesImage_BindingContextChanged(object sender, EventArgs e)
        {

            var leftImage = sender as Image;
            LoadDetailsPageViewModel viewModel = this.BindingContext as LoadDetailsPageViewModel;
            (leftImage.Parent as View).GestureRecognizers.Add(new TapGestureRecognizer() { NumberOfTapsRequired = 1, TappedCallback = YesTapped });
        }

        private async void YesTapped(View arg1, object arg2)
        {
            await PopupNavigation.PopAsync();
            await ParetnVM.ShippedYesMethod();
        }
    }
}