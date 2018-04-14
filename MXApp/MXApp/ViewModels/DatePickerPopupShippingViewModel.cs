
using MXApp.ViewModels.Base;
using MXApp.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MXApp.ViewModels
{
    public class DatePickerPopupShippingViewModel:ViewModelBase
    {
        private ShippingViewModel parent;

        public ShippingViewModel Parent
        {
            get { return parent; }
            set { parent = value; OnPropertyChanged(); }
        }
        private ShippingView view;

        public ShippingView ParentView
        {
            get { return view; }
            set { view = value; OnPropertyChanged(); }
        }


        public DatePickerPopupShippingViewModel(ShippingViewModel parent,ShippingView view)
        {
            Parent = parent;
            ParentView = view;
            SelectedDate = DateTime.Now;
        }
        private DateTime selectedDate;

        public DateTime SelectedDate
        {
            get
            {
                if (selectedDate == null)
                    selectedDate = DateTime.Now;
                return selectedDate;
            }
            set { selectedDate = value; OnPropertyChanged(); }
        }

        private AsyncCommand saveDateCommand;

        public AsyncCommand SaveDateCommand
        {
            get
            {
                if (saveDateCommand == null)
                    saveDateCommand = new AsyncCommand(SaveDateCommandMethod);
                return saveDateCommand;
            }
        }

        private async Task SaveDateCommandMethod()
        {
            await PopupNavigation.PopAsync();
            ParentView.Title= string.Format("Shipping {0} - {1}", Parent.SelectedWareHouse.WHS, SelectedDate.ToString("dd/MM/yyyy"));
            Parent.SelectedDate = SelectedDate;
            await Parent.RefreshGrids();
            
        }
    }
}
