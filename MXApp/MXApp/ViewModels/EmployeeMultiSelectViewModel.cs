using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using MXApi.Models;
using MXApp.ViewModels.Base;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace MXApp.ViewModels
{
    public class EmployeeMultiSelectViewModel : ViewModelBase
    {
        public EmployeeMultiSelectViewModel()
        {

        }
        bool FromCheckout = false;
        bool TempEmployeeMode = false;
        public EmployeeMultiSelectViewModel(EmployeeTrackViewModel parentVM, bool fromCheckout = false, bool _TempEmployeeMode = false)
        {
            EmployeeTrackViewModel = parentVM;
            FromCheckout = fromCheckout;
            TempEmployeeMode = _TempEmployeeMode;
            Employees = new ObservableCollection<EmployeeListItem>();
        }
        private ObservableCollection<EmployeeListItem> employees;

        public ObservableCollection<EmployeeListItem> Employees
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
        private EmployeeTrackViewModel employeeTrackViewModel;

        public EmployeeTrackViewModel EmployeeTrackViewModel
        {
            get { return employeeTrackViewModel; }
            set { employeeTrackViewModel = value; OnPropertyChanged(); }
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
                        var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<EmployeeListItem>>(uri));
                        if (res != null)
                        {
                            ObservableCollection<EmployeeListItem> emps = new ObservableCollection<EmployeeListItem>(res);
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

        public async Task LoadClockedEmployeeList()
        {
            try
            {
                if (Employees == null || Employees.Count == 0)
                {
                    this.IsBusy = true;
                    try
                    {
                        string uri = App.BASE_PROD_URL + "GetClockedEmployeesList/" + EmployeeTrackViewModel.SelectedRef.ReferencePullID;
                        var res = await Task.Run(() => App.ServiceHelper.GetAsync<List<GetClockedEmployeesList>>(uri));
                        if (res != null)
                        {
                            var MasterEmployees = EmployeeTrackViewModel.Employees;
                            foreach (var item in res)
                            {
                                if(item.EmployeeID=="1002")
                                {
                                    Employees.Add(new EmployeeListItem()
                                    {
                                        WoNumID=item.WOEmployeeID,
                                        WOEmployeeID = 1002,
                                        FirstName = "Temp",
                                        Lastname = "Employee"
                                    });
                                }
                                else
                                {
                                    var emp = MasterEmployees.Where(x => x.WOEmployeeID == Convert.ToInt32(item.EmployeeID)).FirstOrDefault();
                                    if (emp != null)
                                    {
                                        Employees.Add(new EmployeeListItem()
                                        {
                                            WoNumID = item.WOEmployeeID,
                                            FirstName = emp.FirstName,
                                            Lastname = emp.Lastname,
                                            WOEmployeeID = emp.WOEmployeeID,
                                        });
                                    }
                                }
                                
                            }
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

        private AsyncCommand submitCommand;

        public AsyncCommand SubmitCommand
        {
            get
            {
                if (submitCommand == null)
                    submitCommand = new AsyncCommand(SubmitCommandMethod);
                return submitCommand;
            }
        }
        private ObservableCollection<object> selectedEmployees;

        public ObservableCollection<object> SelectedEmployees
        {
            get { return selectedEmployees; }
            set { selectedEmployees = value; OnPropertyChanged(); }
        }
        private async Task SubmitCommandMethod()
        {
            IsBusy = true;
            await PopupNavigation.PopAsync();

            if (SelectedEmployees.Count > 0)
            {
                foreach (var item in SelectedEmployees)
                {
                    EmployeeTrackViewModel.AddSelectedEmployee((EmployeeListItem)item);
                }
            }
            else
            {
                EmployeeTrackViewModel.IsMultipleMode = false;
            }
            if (FromCheckout)
            {
                await EmployeeTrackViewModel.ClockoutEmployees();
            }

            IsBusy = false;
        }
    }

    public class EmployeeListItem : EmployeesList, INotifyPropertyChanged
    {
        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        private int woNumID;

        public int WoNumID
        {
            get { return woNumID; }
            set { woNumID = value; RaisePropertyChanged("WoNumID"); }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
