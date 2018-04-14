using Microsoft.AppCenter.Crashes;
using MXApi.Models;
using MXApp.ViewModels.Base;
using MXApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MXApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Members
        private string _UserName;
        private string _Password;
        private AsyncCommand _LoginCommand;
        #endregion

        #region Constructor
        public LoginViewModel()
        {

        }
        #endregion

        #region Properties
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public AsyncCommand LoginCommand
        {
            get
            {
                if (_LoginCommand == null)
                    _LoginCommand = new AsyncCommand(LoginCommandMethod);
                return _LoginCommand;
            }
        }
        #endregion

        #region Methods
        private bool Validate()
        {
            return (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password));
        }
        private async Task LoginCommandMethod()
        {
            try
            {
                IsBusy = true;
                var valid = Validate();
                if (valid)
                {
                    LoginModel model = new LoginModel();
                    model.UserName = UserName;
                    model.Password = Password;
                    string uri = App.BASE_AUTH_URL + "Login";
                    var res = await Task.Run(() => App.ServiceHelper.PostAsync<LoginModel, LoginResModel>(uri, model));
                    if (res != null)
                    {
                        App.UserName = UserName;
                        App.UserLogin = res;
                        App.Current.MainPage = App.MainView;
                    }
                    else
                    {
                        await Task.Run(() => App.DialogService.ShowAlertAsync("Login failed.Try again", "Error", "Ok"));
                    }
                }
                else
                {
                    await Task.Run(() => App.DialogService.ShowAlertAsync("Username or Password cannot be empty", "Error", "Ok"));
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                IsBusy = false;
            }
        }
        #endregion
    }
}
