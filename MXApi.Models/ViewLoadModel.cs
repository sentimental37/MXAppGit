﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class ViewLoadModel: sp_mob_ship_viewloads_Result
    {

    }
    public partial class sp_mob_ship_viewloads_Result
    {
        public int EDIAPPID { get; set; }
        public string PickUp_LoadNum { get; set; }
        public Nullable<bool> IsLTL { get; set; }
        public string WHS { get; set; }
        public Nullable<System.DateTime> PUDate { get; set; }
        public Nullable<System.DateTime> PUTime { get; set; }
        public string Carrier { get; set; }
        public string SCAC { get; set; }
        public string SealNumber { get; set; }
        public Nullable<bool> CarryOverNextDay { get; set; }
        public Nullable<bool> IsShipped { get; set; }
        public Nullable<bool> IsVoided { get; set; }
        public Nullable<bool> IsPrint { get; set; }
        public Nullable<bool> UPSFedEx { get; set; }
        public string AppIDComments { get; set; }
        public string DepartingDoor { get; set; }
        public Nullable<int> LoadPalletCount { get; set; }
    }
}
