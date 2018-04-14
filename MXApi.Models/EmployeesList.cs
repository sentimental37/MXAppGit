using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class EmployeesList: sp_mob_TimeTrack_ListEmployee_Result
    {
        private string fullName;

        public string FullName
        {
            get
            {
                return FirstName + " " + Lastname;
            }
        }
        private int woNumID;

        public int WoNumID
        {
            get { return woNumID; }
            set { woNumID = value; }
        }

    }
    public partial class sp_mob_TimeTrack_ListEmployee_Result
    {
        public int WOEmployeeID { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public double HourlyRate { get; set; }
        public bool Deactivate { get; set; }
    }
}
