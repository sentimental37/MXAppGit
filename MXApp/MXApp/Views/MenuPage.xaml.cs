using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MXApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public ListView ListView;

        public MenuPage()
        {
            InitializeComponent();
            Icon = "ic_menu.png";
            Title = "menu"; // The Title property must be set.
            BindingContext = new MainViewMasterViewModel();
            ListView = MenuItemsListView;
            
        }

        class MainViewMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainViewMenuItem> MenuItems { get; set; }

            public MainViewMasterViewModel()
            {
                MenuItems = new ObservableCollection<MainViewMenuItem>(new[]
                {
                    new MainViewMenuItem { Id = 0, Title = "Home" ,MenuType=MenuItemType.Home},
                     new MainViewMenuItem { Id = 1, Title = "Receiving",MenuType=MenuItemType.Receiving },
                     new MainViewMenuItem { Id = 2, Title = "Inventory",MenuType=MenuItemType.Inventory },
                    new MainViewMenuItem { Id = 3, Title = "Production",MenuType=MenuItemType.Production },
                    new MainViewMenuItem { Id = 4, Title = "Shipping" ,MenuType=MenuItemType.Shipping},
                });
            }

            private AsyncCommand logoutCommand;

            public AsyncCommand LogoutCommand
            {
                get
                {
                    if (logoutCommand == null)
                        logoutCommand = new AsyncCommand(LogoutCommandMethod);
                    return logoutCommand;
                }
            }

            private async Task LogoutCommandMethod()
            {
                App.Current.MainPage = new LoginView();
            }

            public string UserName
            {
                get
                {
                    return App.UserName;
                }

            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}