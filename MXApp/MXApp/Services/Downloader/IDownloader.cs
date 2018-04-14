using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MXApp.Services.Downloader
{
    public interface IDownloader
    {
        Stream DownloadPdfStream(string URL,string fileName);
        string GetLocalFileUrl(string URL, string fileName);
        Task<string> GetLocalFileUrlUWP(string URL, string fileName);
    }
}
