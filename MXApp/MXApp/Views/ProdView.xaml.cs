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
	public partial class ProdView : ContentPage
	{
		public ProdView ()
		{
			InitializeComponent();
            this.BindingContext = new ProductionViewModel(this);
        }

       

        public async Task Search_Clicked(object sender, EventArgs e)
        {
            try
            {
                atcBillOrders.Text = "";
                ((ProductionViewModel)this.BindingContext).IsBusy = true;
                SearchWOPopup popup = new SearchWOPopup((ProductionViewModel)this.BindingContext, this);
                await Task.Run(() => ((SearchWOPopupViewModel)popup.BindingContext).LoadRefNumList());
                await PopupNavigation.PushAsync(popup, true);
                ((ProductionViewModel)this.BindingContext).IsBusy = false;
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
                ProductionViewModel viewModel = this.BindingContext as ProductionViewModel;
                if (viewModel != null)
                {
                    viewModel.SwipeDirection = e.SwipeDirection;
                    viewModel.ProdWO = e.RowData as ProdWODetail;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        void DataGrid_SwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            try
            {
                if (e.RowData != null)
                {
                    ProdWODetail ProdWO = e.RowData as ProdWODetail;
                    if (ProdWO.RefBillReady == true)
                        e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }

        private void Edit_Tapped(object sender,TappedEventArgs e)
        {
            try
            {
                ProductionViewModel viewModel = this.BindingContext as ProductionViewModel;
                if (viewModel != null)
                {
                    viewModel.EditWOCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        private void Delete_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                ProductionViewModel viewModel = this.BindingContext as ProductionViewModel;
                if (viewModel != null)
                {
                    viewModel.DeleteWOCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        private void atcRefNum_selectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            try
            {
                var value = (e?.Value as ProdBillCodes)?.BillID;
                if (value != null)
                {
                    ((ProductionViewModel)this.BindingContext).SelectedBillCode = value;
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
                    ((ProductionViewModel)this.BindingContext).SelectedBillCode = null;
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
        }
        private void Folder_Tapped(object sender, TappedEventArgs e)
        {
            SecDataGrid.IsVisible = false;
            FilesView.IsVisible = true;
        }

        private void ListView_SwipeEnded(object sender, Syncfusion.ListView.XForms.SwipeEndedEventArgs e)
        {
            ProductionViewModel ProdViewModel = (ProductionViewModel)this.BindingContext;
            ProdViewModel.ItemIndex = e.ItemIndex;
        }

        private void ListView_SwipeStarted(object sender, Syncfusion.ListView.XForms.SwipeStartedEventArgs e)
        {
            ProductionViewModel ProdViewModel = (ProductionViewModel)this.BindingContext;
            ProdViewModel.ItemIndex = -1;
        }
        private void ViewDocumentTapped(object sender, TappedEventArgs e)
        {
            ProductionViewModel ProdViewModel = (ProductionViewModel)this.BindingContext;
            ProdViewModel.ViewDocumentCommand.Execute(null);
            listView.ResetSwipe(false);
        }
        private void SendMailTapped(object sender, TappedEventArgs e)
        {
            ProductionViewModel ProdViewModel = (ProductionViewModel)this.BindingContext;
            ProdViewModel.SendMailCommand.Execute(null);
            listView.ResetSwipe(false);
        }
    }
}