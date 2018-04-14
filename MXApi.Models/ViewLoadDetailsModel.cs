using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class ViewLoadDetailsModel: sp_mob_ship_viewloaddetails_Result
    {

        public string IsFullPickup
        {
            get
            {
                if (FullPickUp == null)
                    return "false";
                else
                {
                    if (FullPickUp == true)
                        return "true";
                    else
                        return "false";
                }
            }
        }

        public string IsFailToPickUp
        {
            get
            {
                if (FailToPickUp == null)
                    return "false";
                else
                {
                    if (FailToPickUp == true)
                        return "true";
                    else
                        return "false";
                }
            }
        }

        public string IsPartialPickUp
        {
            get
            {
                if (PartialPickUp == null)
                    return "false";
                else
                {
                    if (PartialPickUp == true)
                        return "true";
                    else
                        return "false";
                }
            }
        }

    }
    public partial class sp_mob_ship_viewloaddetails_Result
    {
        public int EDIAPPID { get; set; }
        public string MBOLVICS { get; set; }
        public Nullable<decimal> CTNCount { get; set; }
        public string Account { get; set; }
        public Nullable<double> TotalWeight { get; set; }
        public Nullable<double> TotalCube { get; set; }
        public Nullable<bool> FullPickUp { get; set; }
        public Nullable<bool> FailToPickUp { get; set; }
        public Nullable<bool> PartialPickUp { get; set; }
        public string AccountDivName { get; set; }
        public string ShipTo { get; set; }
        public Nullable<int> MBOLPalletCount { get; set; }
        public string MBOLShipComments { get; set; }

        public int ChildAPPID { get; set; }
    }
}
