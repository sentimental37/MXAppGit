using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class ClockEmployeeModel
    {
        public int RefNum { get; set; }
        public int BadgeID { get; set; }
        public int CheckinType { get; set; }

        public int Temp { get; set; } = 0;

        public int WoNumID { get; set; } = 0;
    }
}
