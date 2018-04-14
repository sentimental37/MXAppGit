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
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
            if (App.IPAddress.StartsWith("100"))
                toggleWLAN.IsToggled = false;
            else
                toggleWLAN.IsToggled = true;
        }
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {

            if (App.IPAddress == "100.100.100.74" && toggleWLAN.IsToggled == true)
            {
                App.IPAddress = "216.2.236.45";
                App.BASE_WEB_URL = string.Format("http://{0}/mxapp/", App.IPAddress);
                App.DialogService.ShowToast("App is using WLAN", 1000);
            }
            else if (App.IPAddress == "216.2.236.45" && toggleWLAN.IsToggled == false)
            {
                App.IPAddress = "100.100.100.74";
                App.BASE_WEB_URL = string.Format("http://{0}/mxapp/", App.IPAddress);
                App.DialogService.ShowToast("App is using VLAN", 1000);
            }
            else if(toggleWLAN.IsToggled==false)
            {
                App.IPAddress = "100.100.100.74";
                App.BASE_WEB_URL = string.Format("http://{0}/mxapp/", App.IPAddress);
                App.DialogService.ShowToast("App is using VLAN", 1000);
            }

        }
    }
}