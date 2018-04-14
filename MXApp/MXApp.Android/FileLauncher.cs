using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MXApp.Droid;
using MXApp.Services.FileLaunch;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileLaunch))]
namespace MXApp.Droid
{
    public class FileLaunch : IFileLaunch
    {
        public void LaunchFile(string path, string type)
        {
            string application = "";
            if(type=="excel")
                application= "application/vnd.ms-excel";
            else
                application = application = "application/msword";
            Android.Net.Uri uri = Android.Net.Uri.FromFile(new Java.IO.File(path));
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, application);
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            Forms.Context.StartActivity(intent);
        }
    }
}