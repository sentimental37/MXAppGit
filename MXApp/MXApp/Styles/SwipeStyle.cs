using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MXApp.Styles
{
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class SwipeStyle : DataGridStyle
    {
        public SwipeStyle()
        {
        }

        public override Color GetHeaderBackgroundColor()
        {
            return Color.FromRgb(15, 15, 15);
        }

        public override Color GetHeaderForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetRecordBackgroundColor()
        {
            return Color.FromRgb(25, 25, 25);
        }

        public override Color GetRecordForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetSelectionBackgroundColor()
        {
            return Color.FromHex("#cce5fa");
        }

        public override Color GetSelectionForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.FromRgb(02, 02, 02);
        }

        public override Color GetCaptionSummaryRowForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetBorderColor()
        {
            return Color.FromRgb(81, 83, 82);
        }

        public override GridLinesVisibility GetGridLinesVisibility()
        {
            return GridLinesVisibility.Horizontal;
        }

    }
}
