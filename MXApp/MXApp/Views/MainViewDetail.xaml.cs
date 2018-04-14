using MXApp.ViewModels;
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
    public partial class MainViewDetail : ContentPage
    {
        public MainViewDetail()
        {
            InitializeComponent();
        }
        void Handle_ProdTapped(object sender, Syncfusion.SfRadialMenu.XForms.ItemTappedEventArgs e)
        {
            if (App.UserLogin != null)
            {
                if (App.UserLogin.RF_Production != null)
                {
                    if (App.UserLogin.RF_Production == true)
                    {
                        var page = (Page)Activator.CreateInstance(typeof(ProdView));
                        page.BindingContext = new ProductionViewModel(page as ProdView);
                        page.Title = "Production";

                        App.MainView.Detail = new NavigationPage(page);
                        App.MainView.IsPresented = false;
                    }
                    else
                    {
                        App.DialogService.ShowAlertAsync("You don't have permission to access this module", "Access Error", "Ok");
                    }
                }
            }
        }

        void Handle_ShipTapped(object sender, Syncfusion.SfRadialMenu.XForms.ItemTappedEventArgs e)
        {
            if (App.UserLogin != null)
            {
                if (App.UserLogin.RF_Shipping != null)
                {
                    if (App.UserLogin.RF_Shipping == true)
                    {
                        var page = (Page)Activator.CreateInstance(typeof(ShippingView));
                        page.BindingContext = new ShippingViewModel(page as ShippingView);
                        page.Title = "Shipping";

                        App.MainView.Detail = new NavigationPage(page);
                        App.MainView.IsPresented = false;
                    }
                    else
                    {
                        App.DialogService.ShowAlertAsync("You don't have permission to access this module", "Access Error", "Ok");
                    }
                }
            }
        }
        void Handle_LogoutTapped(object sender, Syncfusion.SfRadialMenu.XForms.ItemTappedEventArgs e)
        {
            App.Current.MainPage = new LoginView();
        }
    }
}