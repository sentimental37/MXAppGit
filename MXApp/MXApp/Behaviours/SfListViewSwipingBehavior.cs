using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using MXApp.ViewModels;

namespace MXApp.Behaviours
{
    #region SwipingBehavior
    [Preserve(AllMembers = true)]
    public class SfListViewSwipingBehavior : Behavior<Syncfusion.ListView.XForms.SfListView>
    {
        #region Fields

        private ProductionViewModel ProdViewModel;
        private Syncfusion.ListView.XForms.SfListView ListView;

        #endregion


        #region Overrides
        protected override void OnAttachedTo(Syncfusion.ListView.XForms.SfListView bindable)
        {
            ListView = bindable;
            ListView.SwipeStarted += ListView_SwipeStarted;
            ListView.SwipeEnded += ListView_SwipeEnded;

            ProdViewModel = new ProductionViewModel();
            ProdViewModel.sfListView = ListView;
            bindable.BindingContext = ProdViewModel;
            ListView.ItemsSource = ProdViewModel.FilesList;
            base.OnAttachedTo(bindable);
        }
       
        protected override void OnDetachingFrom(Syncfusion.ListView.XForms.SfListView bindable)
        {
            ListView.SwipeStarted -= ListView_SwipeStarted;
            ListView.SwipeEnded -= ListView_SwipeEnded;
            ListView = null;
            ProdViewModel = null;
            base.OnDetachingFrom(bindable);
        }
        #endregion

        #region Events
        private void ListView_SwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            ProdViewModel.ItemIndex = e.ItemIndex;
        }

        private void ListView_SwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            ProdViewModel.ItemIndex = -1;
        }
        #endregion
    }
    #endregion
}
