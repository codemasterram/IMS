//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMS.Data.Infrastructure
{
    using System;
    using System.Collections.Generic;
    
    public partial class StockTransactionDetail
    {
        public int Id { get; set; }
        public int StockTransactionId { get; set; }
        public int ItemId { get; set; }
        public string ItemSubCodeNo { get; set; }
        public string LedgerPageNo { get; set; }
        public int Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Vat { get; set; }
        public Nullable<decimal> BasicAmount { get; set; }
        public Nullable<decimal> NetAmount { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public int StockEffect { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public decimal PurchaseAmount { get; set; }
    
        public virtual Department Department { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Item Item { get; set; }
        public virtual StockTransaction StockTransaction { get; set; }
    }
}
