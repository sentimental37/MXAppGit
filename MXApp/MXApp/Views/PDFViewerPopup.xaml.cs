using MXApp.Services.Downloader;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfPdfViewer.XForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MXApp.Views
{
	public partial class PDFViewerPopup : PopupPage
	{
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public PDFViewerPopup ()
		{
			InitializeComponent ();
            goToPreviousButton.Clicked += OnGoToPreviousPageClicked;
            goToNextButton.Clicked += OnGoToNextPageClicked;
            pageNumberEntry.Completed += CurrentPageEntry_Completed;
            pdfViewerControl.PageChanged += PdfViewerControl_PageChanged;
            pageNumberEntry.Text = "1";
            pdfViewerControl.Toolbar.Enabled = false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Stream documenStream = null;
            //Provide the PDF document URL in the below overload.
            documenStream = DependencyService.Get<IDownloader>().DownloadPdfStream(FileUrl, FileName);
            //Loads the PDF document as Stream to PDF viewer control
            pdfViewerControl.LoadDocument(documenStream);
            pageCountLabel.Text = pdfViewerControl.PageCount.ToString();
        }
        /// <summary>
        /// Triggers whenever the PDF document page gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"> which contains the current page number</param>
        private void PdfViewerControl_PageChanged(object sender, PageChangedEventArgs args)
        {
            pageNumberEntry.Text = args.PageNumber.ToString();
        }

        private void CurrentPageEntry_Completed(object sender, EventArgs e)
        {
            //int pageNumber = 1;
            //if (int.TryParse(((sender as Entry).Text), NumberStyles.Integer, CultureInfo.InvariantCulture, out pageNumber))
            //{
            //    if ((sender as Entry) != null && pageNumber > 0 && pageNumber <= pdfViewerControl.PageCount)
            //        pdfViewerControl.GoToPage(int.Parse((sender as Entry).Text));
            //    else
            //    {
            //        DisplayAlert("Alert", "Please enter the valid page number.", "OK");
            //        (sender as Entry).Text = pdfViewerControl.PageNumber.ToString();
            //    }
            //}
            //else
            //{
            //    DisplayAlert("Alert", "Please enter the valid page number.", "OK");
            //    (sender as Entry).Text = pdfViewerControl.PageNumber.ToString();
            //}
        }

        void OnGoToPreviousPageClicked(object sender, EventArgs e)
        {
            //Navigates to previous page of the PDF document.
            pdfViewerControl.GoToPreviousPage();
        }

        void OnGoToNextPageClicked(object sender, EventArgs e)
        {
            //Navigates to the next page of the PDF document.
            pdfViewerControl.GoToNextPage();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        public void Close_Tapped(object sender,TappedEventArgs e)
        {
            PopupNavigation.PopAsync(true);
        }
        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }
        private void Close_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.PopAsync(true);
        }
    }
}