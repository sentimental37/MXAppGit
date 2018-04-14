using System;
using System.Collections.Generic;
using System.Text;

namespace MXApp.Services.FileLaunch
{
    public interface IFileLaunch
    {
        void LaunchFile(string path, string type);
    }
}
