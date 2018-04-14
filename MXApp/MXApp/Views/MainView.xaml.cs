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
    public partial class MainView : MasterDetailPage
    {
        public MainView()
        {
            InitializeComponent();
            if (Device.OS == TargetPlatform.iOS)
                IsGestureEnabled = false;
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MainViewMenuItem;
            if (item == null)
                return;

            switch (item.Id)
            {
                case 0:
                    {
                        MainViewDetail details = new MainViewDetail();
                        var page = (Page)details;
                        page.Title = item.Title;
                        Detail = new NavigationPage(page);
                        break;
                    }
                case 3:
                    {
                        if (App.UserLogin != null)
                        {
                            if (App.UserLogin.RF_Production != null)
                            {
                                if (App.UserLogin.RF_Production == true)
                                {
                                    ProdView details = new ProdView();
                                    var page = (Page)details;
                                    page.BindingContext = new ProductionViewModel(page as ProdView);
                                    page.Title = "Production";
                                    Detail = new NavigationPage(page);
                                }
                                else
                                {
                                    App.DialogService.ShowAlertAsync("You don't have permission to access this module", "Access Error", "Ok");
                                }
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        if (App.UserLogin != null)
                        {
                            if (App.UserLogin.RF_Shipping != null)
                            {
                                if (App.UserLogin.RF_Shipping == true)
                                {
                                    ShippingView details = new ShippingView();
                                    var page = (Page)details;
                                    page.BindingContext = new ShippingViewModel(page as ShippingView);
                                    page.Title = "Shipping";
                                    Detail = new NavigationPage(page);
                                }
                                else
                                {
                                    App.DialogService.ShowAlertAsync("You don't have permission to access this module", "Access Error", "Ok");
                                }
                            }
                        }
                        break;
                    }
                case 2:
                case 1:
                    {
                        UnderConstruction construction = new UnderConstruction();
                        var page = (Page)construction;
                        page.Title = item.Title;
                        Detail = new NavigationPage(page);
                        break;
                    }
                case 5:
                    {
                        App.UserName = null;
                        App.Current.MainPage = new LoginView();
                        break;
                    }
                default:
                    break;
            }
            IsPresented = false;
            MasterPage.ListView.SelectedItem = null;
        }
    }
}