using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MXApp.Helpers;
using MXApp.Views;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MXApi.Models;

namespace MXApp
{
    public partial class App : Application
    {
        public static Application CurrentApplication = App.Current;
        private static DialogService _dialogService;
        public static string UserName { get; set; }


        private static string baseWebURL;

        public static string BASE_WEB_URL
        {
            get { return baseWebURL; }
            set { baseWebURL = value; }
        }

        private static string bASE_AUTH_URL;

        public static string BASE_AUTH_URL
        {
            get
            {
                return BASE_WEB_URL + "api/Auth/";
            }
        }

        private static string bASE_PROD_URL;

        public static string BASE_PROD_URL
        {
            get
            {
                return BASE_WEB_URL + "api/Production/";
            }
        }

        private static string bASE_SHIPPING_URL;

        public static string BASE_SHIPPING_URL
        {
            get
            {
                return BASE_WEB_URL + "api/Shipping/";
            }
        }


        public static DialogService DialogService
        {
            get
            {
                if (_dialogService == null)
                    _dialogService = new DialogService();
                return _dialogService;
            }

        }

        private static RequestService _ServiceHelper;

        public static RequestService ServiceHelper
        {
            get
            {
                if (_ServiceHelper == null)
                    _ServiceHelper = new RequestService();
                return _ServiceHelper;
            }
        }

        private static MainView mainView;


        public static MainView MainView
        {
            get
            {
                if (mainView == null)
                    mainView = new MainView();
                return mainView;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new LoginView();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("android=8cf4473a-8573-4261-b804-3e913cf84ac3;ios=866bd2b0-d2a6-4a01-8d77-02bc00b14a29;", typeof(Analytics), typeof(Crashes));
            BASE_WEB_URL = string.Format("http://{0}/mxapp/", IPAddress);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        private static LoginResModel userLogin;

        public static LoginResModel UserLogin
        {
            get { return userLogin; }
            set { userLogin = value; }
        }

        private static string ipAddr;

        public static string IPAddress
        {
            get
            {
                if (string.IsNullOrEmpty(ipAddr))
                    ipAddr = "100.100.100.74";
                return ipAddr;
            }
            set { ipAddr = value; }
        }

    }

    public enum NetworkType
    {
        WLAN = 1,
        VLAN = 0
    }
}
