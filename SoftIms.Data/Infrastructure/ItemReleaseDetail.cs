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
    
    public partial class ItemReleaseDetail
    {
        public int Id { get; set; }
        public int ItemReleaseId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public string Narration { get; set; }
        public string SubCode { get; set; }
    
    	//RelationshipName: FK_ItemReleaseDetail_Item
        public virtual Item Item { get; set; }
    	//RelationshipName: FK_ItemReleaseDetail_ItemRelease
        public virtual ItemRelease ItemRelease { get; set; }
    }
}
