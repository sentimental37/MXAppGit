using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MXApp.ViewModels.Base
{
    public abstract class ViewModelBase : BindableObject
    {
        #region Members
        private bool _isBusy;
        #endregion

        #region Constructor
        public ViewModelBase()
        {

        }
        #endregion

        #region Properties
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        } 
        #endregion
    }
}
