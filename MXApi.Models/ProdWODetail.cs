using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class ProdWODetail : sp_mob_ListProdWODetails_Result
    {
    }
    public partial class sp_mob_ListProdWODetails_Result
    {
        public int CodeID { get; set; }
        public Nullable<int> ReferencePullID { get; set; }
        public string BillCodeDescription { get; set; }
        public Nullable<int> QTY { get; set; }
        public string BillComments { get; set; }
        public Nullable<int> BillCode { get; set; }
        public string Account { get; set; }
        public Nullable<double> StaticOutboundRate { get; set; }
        public Nullable<bool> RefBillReady { get; set; }
    }
}
