using MXApp.ViewModels;
using Rg.Plugins.Popup.Pages;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MXApp.Views
{
    public partial class EmployeeMultiselectPopup : PopupPage
    {
        public EmployeeMultiselectPopup()
        {
            InitializeComponent();
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

        private void lstEmployees_SelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            for (int i = 0; i < e.AddedItems.Count; i++)
            {
                var item = e.AddedItems[i];
                (item as EmployeeListItem).IsSelected = true;
            }
            for (int i = 0; i < e.RemovedItems.Count; i++)
            {
                var item = e.RemovedItems[i];
                (item as EmployeeListItem).IsSelected = false;
            }
            EmployeeMultiSelectViewModel viewModel = this.BindingContext as EmployeeMultiSelectViewModel;
            viewModel.SelectedEmployees = lstEmployees.SelectedItems;

        }
    }
}