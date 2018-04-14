using Microsoft.AppCenter.Crashes;
using MXApi.Models;
using MXApp.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MXApp.Views
{
    public partial class LoadDetailsPage : PopupPage
    {
        public LoadDetailsPage()
        {
            InitializeComponent();
            this.dgLoadDetails.QueryRowStyle += dgLoadDetails_QueryRowStyle;
            btnGoBack.GestureRecognizers.Add(new TapGestureRecognizer() { NumberOfTapsRequired = 1, TappedCallback =GoBackTapped });
            
        }


        private async void GoBackTapped(View arg1, object arg2)
        {
            LoadDetailsPageViewModel viewModel = this.BindingContext as LoadDetailsPageViewModel;
            await PopupNavigation.PopAsync();
            await viewModel.Parent.RefreshGrids();
           
        }

        private void dgLoadDetails_QueryRowStyle(object sender, QueryRowStyleEventArgs e)
        {
            if (e.RowData != null)
            {
                ViewLoadDetailsModel data = e.RowData as ViewLoadDetailsModel;
                if (data.FullPickUp == true)
                {
                    e.Style.BackgroundColor = Color.Green;
                    e.Style.ForegroundColor = Color.White;
                }
                e.Style.ConditionalStylingPreference = StylePreference.RowStyleAndSelection;
                e.Handled = true;
            }
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

        void DG_SwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            try
            {
                LoadDetailsPageViewModel viewModel = this.BindingContext as LoadDetailsPageViewModel;
                if (viewModel != null)
                {
                    viewModel.ViewLoadDetail = e.RowData as ViewLoadDetailsModel;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        void DG_SwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            try
            {
                if (e.RowData != null)
                {
                    ViewLoadDetailsModel ProdWO = e.RowData as ViewLoadDetailsModel;
                    if (ProdWO.FullPickUp == true)
                        e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        private void Edit_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                LoadDetailsPageViewModel viewModel = this.BindingContext as LoadDetailsPageViewModel;
                if (viewModel != null)
                {
                    viewModel.EditLoadDetailsCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        private void dgLoadDetails_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            LoadDetailsPageViewModel viewModel = this.BindingContext as LoadDetailsPageViewModel;
            viewModel.SelectedViewLoadDetail = dgLoadDetails.SelectedItems;
        }
        private void leftImage_BindingContextChanged(object sender, EventArgs e)
        {
            var leftImage = sender as Grid;
            LoadDetailsPageViewModel viewModel = this.BindingContext as LoadDetailsPageViewModel;
            leftImage.GestureRecognizers.Add(new TapGestureRecognizer() { NumberOfTapsRequired = 1, Command = viewModel.EditLoadDetailsCommand });
        }
        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            LoadDetailsPageViewModel ProdViewModel = (LoadDetailsPageViewModel)this.BindingContext;
            if (ProdViewModel.filterTextChanged == null)
                ProdViewModel.filterTextChanged = OnFilterChanged;
            if (e.NewTextValue == null)
                ProdViewModel.FilterText = "";
            else
                ProdViewModel.FilterText = e.NewTextValue;
        }
        public void OnFilterChanged()
        {
            if (dgLoadDetails.View != null)
            {
                LoadDetailsPageViewModel ProdViewModel = (LoadDetailsPageViewModel)this.BindingContext;
                dgLoadDetails.View.Filter = ProdViewModel.FilerRecords;
                dgLoadDetails.View.RefreshFilter();
            }
        }
        private void GoBack_Tapped(object sender,TappedEventArgs e)
        {
            PopupNavigation.PopAsync();
        }
    }
}