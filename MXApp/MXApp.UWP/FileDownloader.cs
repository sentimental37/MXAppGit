
using MXApp.Services.Downloader;
using MXApp.UWP;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileDownloader))]
namespace MXApp.UWP
{
    public class FileDownloader:IDownloader
    {
        HttpClient client = new HttpClient();
        string filename;
        Stream documentStream;
        public FileDownloader()
        {

        }
        public Stream DownloadPdfStream(string URL)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = client.GetAsync(URL).Result;
                responseMessage.EnsureSuccessStatusCode();
                Stream str = responseMessage.Content.ReadAsStreamAsync().Result;
                return str;
            }
        }

        public Stream DownloadPdfStream(string URL, string fileName)
        {
            throw new NotImplementedException();
        }

        public string GetLocalFileUrl(string URL, string fileName)
        {
            return string.Empty;
        }

        public async Task<string> GetLocalFileUrlUWP(string URL, string fileName)
        {
            var uriBing = new Uri(URL);
            // Create sample file; replace if exists.
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.CreateFileAsync(filename,
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
            var cli = new HttpClient();
            Byte[] bytes = await cli.GetByteArrayAsync(uriBing);
            IBuffer buffer = bytes.AsBuffer();
            await Windows.Storage.FileIO.WriteBufferAsync(sampleFile, buffer);
            return sampleFile.Path;
        }
    }
}
