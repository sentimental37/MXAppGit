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
    public partial class EmployeeTrackPopup : PopupPage
    {
        public EmployeeTrackPopup()
        {
            InitializeComponent();
            if (Device.OS == TargetPlatform.iOS)
            {
                imgConfirm.IsVisible = true;
                stNumPanel.Margin = new Thickness(100, 0, 100, 0);
            }
            else
            {
                stNumPanel.Margin = new Thickness(10, 0, 10, 0);

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
        private void atcRefNum_selectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            try
            {
                var value = (e?.Value as EmployeesList)?.WOEmployeeID;
                if (value != null)
                {
                    ((EmployeeTrackViewModel)this.BindingContext).SelectedEmployeeID = value;
                    ((EmployeeTrackViewModel)this.BindingContext).IsMultipleMode = false;
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
                    ((EmployeeTrackViewModel)this.BindingContext).SelectedEmployeeID = null;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }
        private void Close_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.PopAsync(true);
        }
        void Handle_FocusChanged(object sender, Syncfusion.SfAutoComplete.XForms.FocusChangedEventArgs e)
        {
            if (actEmployees.Text != "")
            {
                EmployeeTrackViewModel vm = this.BindingContext as EmployeeTrackViewModel;
                if (vm.Employees.Where(x => x.WOEmployeeID == Convert.ToInt32(actEmployees.Text)).Count() > 0)
                {
                    vm.SelectedEmployeeID = Convert.ToInt32(actEmployees.Text);
                    vm.IsMultipleMode = false;
                }
                else
                    vm.SelectedEmployeeID = null;
            }
        }
        private void Done_Tapped(object sender, EventArgs e)
        {
            if (actEmployees.SelectedValue != null && actEmployees.SelectedValue.ToString() != "")
            {
                EmployeeTrackViewModel vm = this.BindingContext as EmployeeTrackViewModel;
                if (vm.Employees.Where(x => x.WOEmployeeID == Convert.ToInt32(actEmployees.SelectedValue.ToString())).Count() > 0)
                {
                    vm.SelectedEmployeeID = Convert.ToInt32(actEmployees.SelectedValue.ToString());
                    vm.IsMultipleMode = false;
                }
                else
                    vm.SelectedEmployeeID = null;
            }
        }
        private void MultiSelect_Tapped(object sender, EventArgs e)
        {
            EmployeeTrackViewModel vm = this.BindingContext as EmployeeTrackViewModel;
            vm.ModeMultiSelectCommand.Execute(null);
        }

        void TempForce_Changed(object sender, Syncfusion.SfNumericUpDown.XForms.ValueEventArgs e)
        {
            EmployeeTrackViewModel vm = this.BindingContext as EmployeeTrackViewModel;
            vm.TempEmployees = new System.Collections.ObjectModel.ObservableCollection<EmployeesList>();
            if(TempForceCount.Value!=null)
            {
                if(Convert.ToInt32(TempForceCount.Value.ToString())>0)
                {
                    for (int i = 0; i < Convert.ToInt32(TempForceCount.Value.ToString()); i++)
                    {
                        vm.TempEmployees.Add(new EmployeesList() { FirstName = "Temp", Lastname = "Force", WOEmployeeID = 1001 });
                    }
                }
            }
        }
    }
}