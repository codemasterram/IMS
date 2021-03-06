//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftIms.Data.Infrastructure
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseOrderDetail
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> BasicAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string Remarks { get; set; }
    
    	//RelationshipName: FK_PurchaseOrderDetail_Item
        public virtual Item Item { get; set; }
    	//RelationshipName: FK_PurchaseOrderDetail_PurchaseOrder
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
