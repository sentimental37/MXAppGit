using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Microsoft.AppCenter.Crashes;
using MXApp.ViewModels;
using MXApi.Models;

namespace MXApp.Views
{
    public partial class SearchWarehousePopup : PopupPage
    {
        public SearchWarehousePopup(ShippingViewModel shippingViewModel, ShippingView sv)
        {
            InitializeComponent();
            this.BindingContext = new SearchWarehousePopupViewModel(shippingViewModel);
            ShippingView = sv;
            ParentVM = shippingViewModel;
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

        private ShippingView shippingView;

        public ShippingView ShippingView
        {
            get { return shippingView; }
            set { shippingView = value; OnPropertyChanged(); }
        }
        private ShippingViewModel parentVM;

        public ShippingViewModel ParentVM
        {
            get { return parentVM; }
            set { parentVM = value; OnPropertyChanged(); }
        }

        private void atcRefNum_selectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            try
            {
                var value = (e?.Value as WHS)?.WHS;
                if (value != null)
                {
                    ((SearchWarehousePopupViewModel)this.BindingContext).SelectedWHS = value;
                    if (ParentVM.SelectedDate != null)
                    {
                        string date = "";
                        string month = "";
                        string year = "";
                        date = ParentVM.SelectedDate.Day.ToString();
                        month = ParentVM.SelectedDate.Month.ToString();
                        year = ParentVM.SelectedDate.Year.ToString();
                        ShippingView.Title = string.Format("Shipping {0} - {1}", value, date + "/" + month + "/" + year);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        private void atcRefNum_valueChanged(object sender, Syncfusion.SfAutoComplete.XForms.ValueChangedEventArgs e)
        {
            try
            {
                var text = e?.Value;
                if (string.IsNullOrEmpty(text))
                {
                    ((SearchWarehousePopupViewModel)this.BindingContext).SelectedWHS = null;
                }
                else
                {
                    ShippingView.Title = "Shipping";
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        private void Close_Clicked(object sender, EventArgs e)
        {
            ((SearchWarehousePopupViewModel)this.BindingContext).SelectedWHS = null;
            ShippingView.Title = "Shipping";
            PopupNavigation.PopAsync(true);
        }
    }
}