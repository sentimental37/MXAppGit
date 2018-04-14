using MXApp.iOS;
using MXApp.Services.Save;
using QuickLook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveIOS))]
namespace MXApp.iOS
{
    class SaveIOS : ISave
    {
        public void Save(string filename, string contentType, MemoryStream stream)
        {
            string exception = string.Empty;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(path, filename);
            try
            {
                FileStream fileStream = File.Open(filePath, FileMode.Create);
                stream.Position = 0;
                stream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            catch (Exception e)
            {
                exception = e.ToString();
            }
            finally
            {
                if (contentType != "application/html")
                    stream.Dispose();
            }
            if (contentType == "application/html" || exception != string.Empty)
                return;
            UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (currentController.PresentedViewController != null)
                currentController = currentController.PresentedViewController;
            UIView currentView = currentController.View;

            QLPreviewController qlPreview = new QLPreviewController();
            QLPreviewItem item = new QLPreviewItemBundle(filename, filePath);
            qlPreview.DataSource = new PreviewControllerDS(item);

            //UIViewController uiView = currentView as UIViewController;

            currentController.PresentViewController((UIViewController)qlPreview, true, (Action)null);
        }
    }
}
