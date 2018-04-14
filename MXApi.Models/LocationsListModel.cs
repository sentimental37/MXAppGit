using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class LocationsListModel: sp_mob_ship_viewloads_locations_Result
    {
    }
    public partial class sp_mob_ship_viewloads_locations_Result
    {
        public string PickUp_Load_Num { get; set; }
        public string TATTLETALE { get; set; }
        public string WAVELocation { get; set; }
        public string MBOLVics { get; set; }
    }
}
