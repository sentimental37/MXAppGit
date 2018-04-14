using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class AddBillingCodeModel
    {
        public string Account { get; set; }
        public int RefNum { get; set; }
        public int BillCode { get; set; }

        public string UserName { get; set; }

        public string Vendor { get; set; }
    }
}
