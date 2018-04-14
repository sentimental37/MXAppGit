using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{

    public class ProdBillCodes: sp_mob_AddProdWODetails_Result
    {
    }
    public partial class sp_mob_AddProdWODetails_Result
    {
        public string Description { get; set; }
        public int CODE_ { get; set; }
        public string Account_ { get; set; }
        public int BillID { get; set; }
        public double Price { get; set; }
    }
}
