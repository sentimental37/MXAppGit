using Microsoft.AppCenter.Crashes;
using MXApi.Models;
using MXApp.ViewModels;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfDataGrid.XForms;
using Syncfusion.XForms.TabView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MXApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShippingView : ContentPage
    {
        public ShippingView()
        {
            InitializeComponent();
            this.BindingContext = new ShippingViewModel(this);
            this.dg1.QueryRowStyle += dg1_QueryRowStyle;
        }
      
        private void dg1_QueryRowStyle(object sender, QueryRowStyleEventArgs e)
        {
            if (e.RowData != null)
            {
                ViewLoadModel data = e.RowData as ViewLoadModel;
                if (data.IsShipped == true)
                {
                    e.Style.BackgroundColor = Color.Green;
                    e.Style.ForegroundColor = Color.White;
                }
                e.Style.ConditionalStylingPreference = StylePreference.RowStyleAndSelection;
                e.Handled = true;
            }
        }

        public async Task Search_Clicked(object sender, EventArgs e)
        {
            try
            {
                ((ShippingViewModel)this.BindingContext).IsBusy = true;
                SearchWarehousePopup popup = new SearchWarehousePopup((ShippingViewModel)this.BindingContext, this);
                await Task.Run(() => ((SearchWarehousePopupViewModel)popup.BindingContext).LoadWHSList());
                await PopupNavigation.PushAsync(popup, true);
                ((ShippingViewModel)this.BindingContext).IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        void DataGrid_SwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            try
            {
                ShippingViewModel viewModel = this.BindingContext as ShippingViewModel;
                if (viewModel != null)
                {
                    viewModel.ViewLoad = e.RowData as ViewLoadModel;
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
                ShippingViewModel viewModel = this.BindingContext as ShippingViewModel;
                if (viewModel != null)
                {
                    viewModel.EditViewLoadCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        private void MainGrid_Tapped(object sender, TappedEventArgs e)
        {
            SecDataGrid.IsVisible = true;
            FilesView.IsVisible = false;
            LocationView.IsVisible = false;
        }
        private void Folder_Tapped(object sender, TappedEventArgs e)
        {
            SecDataGrid.IsVisible = false;
            FilesView.IsVisible = true;
            LocationView.IsVisible = false;
            ((ShippingViewModel)this.BindingContext).RefreshFilesList();
        }

        private void Location_Tapped(object sender, TappedEventArgs e)
        {
            LocationView.IsVisible = true;
            SecDataGrid.IsVisible = false;
            FilesView.IsVisible = false;
            ((ShippingViewModel)this.BindingContext).RefreshLocationList();
        }
        private void ListView_SwipeEnded(object sender, Syncfusion.ListView.XForms.SwipeEndedEventArgs e)
        {
            ShippingViewModel ProdViewModel = (ShippingViewModel)this.BindingContext;
            ProdViewModel.ItemIndex = e.ItemIndex;
        }

        private void ListView_SwipeStarted(object sender, Syncfusion.ListView.XForms.SwipeStartedEventArgs e)
        {
            ShippingViewModel ProdViewModel = (ShippingViewModel)this.BindingContext;
            ProdViewModel.ItemIndex = -1;
        }
        private void ViewDocumentTapped(object sender, TappedEventArgs e)
        {
            ShippingViewModel ProdViewModel = (ShippingViewModel)this.BindingContext;
            ProdViewModel.ViewDocumentCommand.Execute(null);
            listView.ResetSwipe(false);
        }
        private void SendMailTapped(object sender, TappedEventArgs e)
        {
            ShippingViewModel ProdViewModel = (ShippingViewModel)this.BindingContext;
            ProdViewModel.SendMailCommand.Execute(null);
            listView.ResetSwipe(false);
        }
        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            //this.dg1.View.LiveDataUpdateMode = Syncfusion.Data.LiveDataUpdateMode.AllowDataShaping;

            ShippingViewModel ProdViewModel = (ShippingViewModel)this.BindingContext;
            if (ProdViewModel.filterTextChanged == null)
                ProdViewModel.filterTextChanged = OnFilterChanged;
            if (e.NewTextValue == null)
                ProdViewModel.FilterText = "";
            else
                ProdViewModel.FilterText = e.NewTextValue;
        }
        public void OnFilterChanged()
        {
            if (dg1.View != null)
            {
                ShippingViewModel ProdViewModel = (ShippingViewModel)this.BindingContext;
                dg1.View.Filter = ProdViewModel.FilerRecords;
                dg1.View.RefreshFilter();
            }
        }
    }
}