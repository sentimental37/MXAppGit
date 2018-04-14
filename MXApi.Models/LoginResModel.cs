﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MXApi.Models
{
    public class LoginResModel:login
    {
    }
    public partial class login
    {
        public int LoginID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string ModuleAccess { get; set; }
        public Nullable<bool> ECOM { get; set; }
        public Nullable<bool> SUPERVISOR { get; set; }
        public Nullable<bool> ReadWrite { get; set; }
        public Nullable<bool> INVADJUSTMENTS { get; set; }
        public string FullName { get; set; }
        public string WHS { get; set; }
        public Nullable<bool> RF { get; set; }
        public Nullable<bool> DESKTOP { get; set; }
        public Nullable<bool> DESKTOP_Receiving { get; set; }
        public Nullable<bool> DESKTOP_Inventory { get; set; }
        public Nullable<bool> DESKTOP_Production { get; set; }
        public Nullable<bool> DESKTOP_Shipping { get; set; }
        public Nullable<bool> DESKTOP_Billing { get; set; }
        public Nullable<bool> DESKTOP_Management { get; set; }
        public Nullable<bool> RF_Receiving { get; set; }
        public Nullable<bool> RF_Inventory { get; set; }
        public Nullable<bool> RF_Production { get; set; }
        public Nullable<bool> RF_Shipping { get; set; }
        public Nullable<bool> RF_AddedValue { get; set; }
        public Nullable<bool> RF_Management { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<bool> DESKTOP_Allocation { get; set; }
        public string login_email { get; set; }
    }
}
