using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class GetClockedEmployeesList: sp_mob_TimeTrack_GetClockedInEmployees_Result
    {
    }
    public partial class sp_mob_TimeTrack_GetClockedInEmployees_Result
    {
        public int WOEmployeeID { get; set; }
        public Nullable<int> ReferencePullID { get; set; }
        public string EmployeeStatus { get; set; }
        public string EmployeeID { get; set; }
        public Nullable<System.DateTime> EmployeeTime { get; set; }
        public Nullable<bool> NonCompliant { get; set; }
        public Nullable<System.DateTime> InTimeStamp { get; set; }
        public Nullable<System.DateTime> OutTimeStamp { get; set; }
    }
}
