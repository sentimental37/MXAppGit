using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class sp_mob_OpenProdWO
    {
        public int ReferencePullID { get; set; }
        public string Ship_To { get; set; }
        public string Account { get; set; }
        public string Vendor_Name { get; set; }
        public string Author { get; set; }
        public DateTime? Ship_Date { get; set; }
        public DateTime? Cancel_Date { get; set; }
        public string Comments { get; set; }
        public DateTime? PullDate { get; set; }
        public bool? REFNUMNotBilled { get; set; }
        public int? InvoiceNo { get; set; }
    }
    public class ProdOpenOrders : sp_mob_OpenProdWO
    {

    }
}
