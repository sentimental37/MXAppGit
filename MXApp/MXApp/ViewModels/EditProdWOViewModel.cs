using MXApp.ViewModels.Base;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MXApp.ViewModels
{
    public class EditProdWOViewModel :ViewModelBase
    {
        #region Members
        private ProductionViewModel productionViewModel;
        private string qty;
        private string billComments;
        private AsyncCommand updateCommand;
        #endregion

        #region Constructor
        public EditProdWOViewModel(ProductionViewModel productionViewModel)
        {
            Parent = productionViewModel;
            SetValues();
        }
        #endregion

        #region Properties
        public ProductionViewModel Parent
        {
            get { return productionViewModel; }
            set { productionViewModel = value; OnPropertyChanged(); }
        }
        public string QTY
        {
            get { return qty; }
            set { qty = value; OnPropertyChanged(); }
        }
        public string BillComments
        {
            get { return billComments; }
            set { billComments = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public AsyncCommand UpdateCommand
        {
            get
            {
                if (updateCommand == null)
                    updateCommand = new AsyncCommand(UpdateCommandMethod);
                return updateCommand;
            }
        }
        #endregion

        #region Methods
        private void SetValues()
        {
            QTY = Parent.ProdWO.QTY.ToString();
            BillComments = Parent.ProdWO.BillComments;
        }
        private async Task UpdateCommandMethod()
        {
            Parent.UpdateItem(this);
            await PopupNavigation.PopAsync();
        }
        #endregion
    }
}
