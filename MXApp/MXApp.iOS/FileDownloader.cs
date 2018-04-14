using System;
using System.IO;
using System.Net;
using Xamarin.Forms;
using System.Threading.Tasks;
using MXApp.Services.Downloader;
using MXApp.iOS;

[assembly: Dependency(typeof(FileDownloader))]
namespace MXApp.iOS
{
    public class FileDownloader : IDownloader
    {
        
        Stream documentStream;
        public FileDownloader()
        {
        }

        public Stream DownloadPdfStream(string URL,string fileName)
        {
            var uri = new System.Uri(URL);
            WebClient m_webClient = new WebClient();
            //Returns the PDF document stream from the given URL
            documentStream = m_webClient.OpenRead(uri);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(path, string.Format("{0}{1}", DateTime.Now.ToShortTimeString(), fileName));
            try
            {
                FileStream fileStream = File.Open(filePath, FileMode.Create);
                documentStream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            catch (Exception e)
            {

            }

            if (File.Exists(filePath))
            {
                return new MemoryStream(File.ReadAllBytes(filePath));
            }
            return documentStream;
        }
        public string GetLocalFileUrl(string URL, string fileName)
        {
            var uri = new System.Uri(URL);
            WebClient m_webClient = new WebClient();
            //Returns the PDF document stream from the given URL
            documentStream = m_webClient.OpenRead(uri);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            string filePath = Path.Combine(path, string.Format("{0}{1}", DateTime.Now.ToShortTimeString(), fileName));
            try
            {
                FileStream fileStream = File.Open(filePath, FileMode.Create);
                //docstream.Position = 0;
                documentStream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            catch (Exception e)
            {

            }
            if (File.Exists(filePath))
            {
                return filePath;
            }
            return string.Empty;
        }

        public Task<string> GetLocalFileUrlUWP(string URL, string fileName)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
