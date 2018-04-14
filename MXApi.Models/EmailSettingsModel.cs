using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class EmailSettingsModel : WMS_Explorer
    {
    }
    public partial class WMS_Explorer
    {
        public int WMS_ID { get; set; }
        public byte[] WMS_Application_Icon { get; set; }
        public byte[] WMS_Application_Backround { get; set; }
        public string WMS_Application_Welcome { get; set; }
        public string WMS_Warehousemen_TermsConditions { get; set; }
        public string WMS_PODShareDrivePath { get; set; }
        public string WMS_Mail_UserEmail { get; set; }
        public string WMS_Mail_Password { get; set; }
        public string WMS_Mail_HostName { get; set; }
        public Nullable<int> WMS_Mail_Port { get; set; }
        public Nullable<bool> WMS_Mail_EnableSSL { get; set; }
    }
}
