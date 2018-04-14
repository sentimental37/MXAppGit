using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MXApp.Views
{
    // Custom style class
    public class CustomGridStyle : DataGridStyle
    {
        public CustomGridStyle()
        {
        }
        public override Color GetSelectionBackgroundColor()
        {
            return Color.FromHex("#2196F3");
        }
    }
}
