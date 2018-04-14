using Microsoft.AppCenter.Crashes;
using MXApi.Models;
using MXApp.ViewModels.Base;
using MXApp.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MXApp.ViewModels
{
    public class EmployeeTrackViewModel : ViewModelBase
    {
        public EmployeeTrackViewModel(ProdOpenOrders RefNum)
        {
            Employees = new ObservableCollection<EmployeesList>();
            TempEmployees = new ObservableCollection<EmployeesList>();
            SelectedEmployees = new ObservableCollection<EmployeesList>();
            SelectedRef = RefNum;
        }
        private string title;

        public string Title
        {
            get
            {
                if (title == "")
                    title = "Work Order Time Track";
                return title;
            }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<EmployeesList> employees;

        public ObservableCollection<EmployeesList> Employees
        {
            get
            {
                return employees;
            }
            set
            {
                employees = value;
                OnPropertyChanged();
            }
        }

        private int? selectedEmployeeID;
        private ProdOpenOrders selectedRef;

        public int? SelectedEmployeeID
        {
            get
            {
                return selectedEmployeeID;
            }
            set
            {
                selectedEmployeeID = value;
                OnPropertyChanged();
                var data = Employees.Where(x => x.WOEmployeeID == selectedEmployeeID).FirstOrDefault();
                if (data != null)
                {
                    EmployeeName = data.FirstName + " " + data.Lastname;
                }

            }
        }
        public ProdOpenOrders SelectedRef
        {
            get { return selectedRef; }
            set
            {
                selectedRef = value;
                OnPropertyChanged();
                if (selectedRef != null)
                    Title = "Work Order " + selectedRef.ReferencePullID + " Time Track";
            }
        }

        private string employeeName;

        public string EmployeeName
        {
            get { return employeeName; }
            set { employeeName = value; OnPropertyChanged(); }
        }
        private string employeeIDText;

        public string EmployeeIDText
        {
            get { return employeeIDText; }
            set { employeeIDText = value; OnPropertyChanged(); }
        }


        public async Task LoadEmployeeList()
        {
            try
            {
                if (Employees == null || Employees.Count == 0)
                {
                    this.IsBusy = true;
                    try
                    {
                        string uri = App.BASE_PROD_URL + "GetEmployeesList";
                        var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<EmployeesList>>(uri));
                        if (res != null)
                        {
                            ObservableCollection<EmployeesList> emps = new ObservableCollection<EmployeesList>(res);
                            Employees = emps;
                        }
                    }
                    catch (Exception ex)
                    {
                        await App.DialogService.ShowAlertAsync(ex.Message, "Error", "ok");
                        IsBusy = false;
                    }
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        internal void AddSelectedEmployee(EmployeeListItem item)
        {
            if (item != null)
            {
                EmployeesList itm = item as EmployeesList;
                itm.WoNumID = item.WoNumID;
                if (!SelectedEmployees.Contains(itm))
                {
                    SelectedEmployees.Add(itm);
                    IsMultipleMode = true;
                    EmployeeIDText = "";
                }
            }
        }

        private AsyncCommand checkinCommand;

        public AsyncCommand CheckinCommand
        {
            get
            {

                if (checkinCommand == null)
                    checkinCommand = new AsyncCommand(CheckinCommandMethod);
                return checkinCommand;
            }
        }

        private async Task CheckinCommandMethod()
        {
            try
            {

                IsBusy = true;
                try
                {
                    if (IsMultipleMode)
                    {
                        string uri = App.BASE_PROD_URL + "ClockEmployee";
                        ClockEmployeeModel model = new ClockEmployeeModel();
                        if (SelectedEmployees.Count > 0)
                        {
                            foreach (EmployeesList item in SelectedEmployees)
                            {
                                model = new ClockEmployeeModel();
                                model.RefNum = SelectedRef.ReferencePullID;
                                model.BadgeID = item.WOEmployeeID;
                                model.CheckinType = 1;
                                await Task.Run(() => App.ServiceHelper.PostAsync<ClockEmployeeModel, int>(uri, model));
                            }
                            if (TempEmployees.Count > 0)
                            {
                                if (Device.OS == TargetPlatform.iOS)
                                    TempEmployees.Add(new EmployeesList() { WOEmployeeID = 1002, FirstName = "Temp", Lastname = "Employee" });
                                model = new ClockEmployeeModel();
                                model.RefNum = SelectedRef.ReferencePullID;
                                model.BadgeID = 1001;
                                model.CheckinType = 0;
                                model.Temp = TempEmployees.Count;
                                await Task.Run(() => App.ServiceHelper.PostAsync<ClockEmployeeModel, int>(uri, model));
                            }

                            int count = SelectedEmployees.Count + TempEmployees.Count;
                            string message = string.Format("Total {0} Employees" + Environment.NewLine + "Has been successfully {1}", count, "Checkin");
                            TempEmpCount = 0;
                            TempEmployees = new ObservableCollection<EmployeesList>();
                            IsMultipleMode = false;
                            SelectedEmployeeID = null;
                            EmployeeName = "";
                            EmployeeIDText = "";
                            SelectedEmployees = new ObservableCollection<EmployeesList>();
                            await App.DialogService.ShowAlertAsync(message, "Success", "ok");
                        }
                    }
                    else
                    {
                        if (TempEmployees.Count > 0)
                        {
                            if (Device.OS == TargetPlatform.iOS)
                                TempEmployees.Add(new EmployeesList() { WOEmployeeID = 1002, FirstName = "Temp", Lastname = "Employee" });
                            string uri = App.BASE_PROD_URL + "ClockEmployee";
                            ClockEmployeeModel model = new ClockEmployeeModel();
                            model = new ClockEmployeeModel();
                            model.RefNum = SelectedRef.ReferencePullID;
                            model.BadgeID = 1001;
                            model.CheckinType = 0;
                            model.Temp = TempEmployees.Count;
                            await Task.Run(() => App.ServiceHelper.PostAsync<ClockEmployeeModel, int>(uri, model));
                            int count = TempEmployees.Count;
                            if(SelectedEmployeeID!=null)
                            {
                                model = new ClockEmployeeModel();
                                model.RefNum = SelectedRef.ReferencePullID;
                                model.BadgeID = SelectedEmployeeID.Value;
                                model.CheckinType = 1;
                                await Task.Run(() => App.ServiceHelper.PostAsync<ClockEmployeeModel, int>(uri, model));
                                count = count + 1;
                            }

                            string message = string.Format("Total {0} Employees" + Environment.NewLine + "Has been successfully {1}", count, "Checkin");
                            TempEmpCount = 0;
                            TempEmployees = new ObservableCollection<EmployeesList>();
                            IsMultipleMode = false;
                            SelectedEmployeeID = null;
                            EmployeeName = "";
                            EmployeeIDText = "";
                            SelectedEmployees = new ObservableCollection<EmployeesList>();
                            await App.DialogService.ShowAlertAsync(message, "Success", "ok");

                        }
                        else if (SelectedEmployeeID != null)
                        {
                            string uri = App.BASE_PROD_URL + "ClockEmployee";
                            ClockEmployeeModel model = new ClockEmployeeModel();
                            model.RefNum = SelectedRef.ReferencePullID;
                            model.BadgeID = SelectedEmployeeID.Value;
                            model.CheckinType = 1;
                            var res = await Task.Run(() => App.ServiceHelper.PostAsync<ClockEmployeeModel, int>(uri, model));
                            string message = string.Format("BadgedID:{0}" + Environment.NewLine + "Name {1}" + Environment.NewLine + "Has been successfully {2}", SelectedEmployeeID, EmployeeName, "Checkin");
                            await App.DialogService.ShowAlertAsync(message, "Success", "ok");
                            SelectedEmployeeID = null;
                            EmployeeName = "";
                            EmployeeIDText = "";
                        }
                        else
                        {
                            await App.DialogService.ShowAlertAsync("Select an employee first", "error", "ok");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await App.DialogService.ShowAlertAsync(ex.Message, "Error", "ok");
                    IsBusy = false;
                }
                IsBusy = false;

                //get if we have any active employees for this ref number

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }

        internal async Task ClockoutEmployees()
        {
            IsBusy = true;
            if (SelectedEmployees.Count > 0)
            {
                string uri = App.BASE_PROD_URL + "ClockEmployee";
                ClockEmployeeModel model = new ClockEmployeeModel();
                foreach (EmployeesList item in SelectedEmployees)
                {
                    model.RefNum = SelectedRef.ReferencePullID;
                    model.BadgeID = item.WOEmployeeID;
                    model.CheckinType = 0;
                    model.WoNumID = item.WoNumID;
                    await Task.Run(() => App.ServiceHelper.PostAsync<ClockEmployeeModel, int>(uri, model));
                }
                string message = string.Format("Total {0} Employees" + Environment.NewLine + "Has been successfully {1}", SelectedEmployees.Count, "Checkout");
                await App.DialogService.ShowAlertAsync(message, "Success", "ok");
                IsMultipleMode = false;
                SelectedEmployees = new ObservableCollection<EmployeesList>();
                EmployeeIDText = "";
                EmployeeName = "";
            }
            IsBusy = false;
        }

        private AsyncCommand checkoutCommand;

        public AsyncCommand CheckoutCommand
        {
            get
            {

                if (checkoutCommand == null)
                    checkoutCommand = new AsyncCommand(CheckoutCommandMethod);
                return checkoutCommand;
            }
        }

        private async Task CheckoutCommandMethod()
        {
            try
            {

                IsBusy = true;
                try
                {
                    string uri = App.BASE_PROD_URL + "GetClockedEmployeesList/" + SelectedRef.ReferencePullID;
                    var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<GetClockedEmployeesList>>(uri));
                    if (res != null && res.Count > 0)
                    {
                        SelectedEmployees = new ObservableCollection<EmployeesList>();
                        EmployeeMultiselectPopup employeeMultiselectPopup = new EmployeeMultiselectPopup();
                        EmployeeMultiSelectViewModel vm = new EmployeeMultiSelectViewModel(this, true);
                        await vm.LoadClockedEmployeeList();
                        employeeMultiselectPopup.BindingContext = vm;
                        await PopupNavigation.PushAsync(employeeMultiselectPopup, true);
                    }
                    else
                    {
                        await App.DialogService.ShowAlertAsync("No checkin record found for this reference number", "Error", "ok");
                        IsBusy = false;
                    }
                }
                catch (Exception ex)
                {
                    await App.DialogService.ShowAlertAsync(ex.Message, "Error", "ok");
                    IsBusy = false;
                }
                IsBusy = false;

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

            }
        }

        private AsyncCommand modeMultiSelectCommand;

        public AsyncCommand ModeMultiSelectCommand
        {
            get
            {
                if (modeMultiSelectCommand == null)
                    modeMultiSelectCommand = new AsyncCommand(ModeMultiSelectCommandMethod);
                return modeMultiSelectCommand;
            }
        }

        private async Task ModeMultiSelectCommandMethod()
        {
            SelectedEmployees = new ObservableCollection<EmployeesList>();
            IsMultipleMode = false;
            EmployeeMultiselectPopup employeeMultiselectPopup = new EmployeeMultiselectPopup();
            EmployeeMultiSelectViewModel vm = new EmployeeMultiSelectViewModel(this);
            await vm.LoadEmployeeList();
            employeeMultiselectPopup.BindingContext = vm;
            await PopupNavigation.PushAsync(employeeMultiselectPopup, true);
        }

        private ObservableCollection<EmployeesList> selectedemployees;

        public ObservableCollection<EmployeesList> SelectedEmployees
        {
            get
            {
                return selectedemployees;
            }
            set
            {
                selectedemployees = value;

                OnPropertyChanged();
            }
        }
        private bool isMultipleMode;

        public bool IsMultipleMode
        {
            get { return isMultipleMode; }
            set { isMultipleMode = value; OnPropertyChanged(); }
        }
        private ObservableCollection<EmployeesList> tempEmployees;

        public ObservableCollection<EmployeesList> TempEmployees
        {
            get
            {
                return tempEmployees;
            }
            set
            {
                tempEmployees = value;

                OnPropertyChanged();
            }
        }

        private int tempEmpCount;

        public int TempEmpCount
        {
            get { return tempEmpCount; }
            set { tempEmpCount = value; OnPropertyChanged(); }
        }

    }
}
