//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MXApp.MobileService
{
    using System;
    using System.Collections.Generic;
    
    public partial class EDIAppTable_Child
    {
        public int ChildAPPID { get; set; }
        public int EDIAPPID { get; set; }
        public string MBOL { get; set; }
        public string MBOLVICS { get; set; }
        public Nullable<decimal> CTNCount { get; set; }
        public string Account { get; set; }
        public Nullable<double> TotalWeight { get; set; }
        public Nullable<double> TotalCube { get; set; }
        public Nullable<bool> FullPickUp { get; set; }
        public Nullable<bool> PartialPickUp { get; set; }
        public Nullable<bool> FailToPickUp { get; set; }
        public Nullable<bool> Void { get; set; }
        public string AccountDivName { get; set; }
        public string ShipTo { get; set; }
        public string BOLDocumentPath { get; set; }
        public string POD_DOC_Vics { get; set; }
        public Nullable<int> MBOLPalletCount { get; set; }
        public string MBOLShipComments { get; set; }
    
        public virtual EDIAppTable EDIAppTable { get; set; }
    }
}
