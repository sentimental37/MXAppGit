using Microsoft.AppCenter.Crashes;
using MXApi.Models;
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
    public partial class SearchWOPopup : PopupPage
    {
        ProdView prodView = null;
        public SearchWOPopup(ProductionViewModel productionViewModel,ProdView pv)
        {
            InitializeComponent();
            this.BindingContext = new SearchWOPopupViewModel(productionViewModel);
            prodView = pv;
        }
        public SearchWOPopup(ProductionViewModel productionViewModel, ProdView pv,SearchWOPopupViewModel vm)
        {
            InitializeComponent();
            this.BindingContext = vm;
            prodView = pv;
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

        private void atcRefNum_selectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            try
            {
                var value = (e?.Value as ProdOpenOrders)?.ReferencePullID;
                if (value != null)
                {
                    ((SearchWOPopupViewModel)this.BindingContext).SelectedRefNum = value;
                    prodView.Title = "Production " + value;
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
                    ((SearchWOPopupViewModel)this.BindingContext).SelectedRefNum = null;
                }
                else
                {
                    prodView.Title = "Production " + text;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        private void Close_Clicked(object sender, EventArgs e)
        {
            ((SearchWOPopupViewModel)this.BindingContext).SelectedRefNum = null;
            prodView.Title = "Production";
            PopupNavigation.PopAsync(true);
        }
    }
}