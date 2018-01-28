using IMS.Data;
using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static DateMiti.GetDateMiti;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class PurchaseEntryViewModel : ServiceModel
    {
        public PurchaseEntryViewModel()
        {
            this.Details = new List<PurchaseEntryDetailViewModel>();
            VendorDetail = new VendorViewModel();
        }

        public int Id { get; set; }
        public int FiscalYearId { get; set; }
        public int DocumentSetupId { get; set; }
        public Nullable<int> DocumentNo { get; set; }
        [Display(Name = "दाखिला नं.")]
        public string DisplayDocumentNo { get; set; }
        [Display(Name = "विल नं.")]
        [Required]
        public string InvoiceNo { get; set; }
        public System.DateTime Date { get; set; } = DateTime.Today;
        [Display(Name = "मिति")]
        public string DateBS { get; set; } = DateTime.Today.GetMiti();
        [Display(Name = "कैफियत")]
        public string Remarks { get; set; }

        [Display(Name = "खरिद आदेश नं.")]
        public int? PurchaseOrderId { get; set; }
        public string PurchaseOrderDisplayOrderNo { get; set; }
        public string PurchaseOrderDateBS { get; set; }

        [Display(Name = "बिक्रेता")]
        public int VendorId { get; set; }

        public List<PurchaseEntryDetailViewModel> Details { get; set; }
        public VendorViewModel VendorDetail { get; set; }
    }

    public class PurchaseEntryDetailViewModel
    {
        public int Id { get; set; }
        public int StockTransactionId { get; set; }
        [LocalizedDisplayName("Item", DefaultText = "Item")]
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemSubCodeNo { get; set; }
        [LocalizedDisplayName("LedgerPageNo", DefaultText = "Ledger Page No")]
        public string LedgerPageNo { get; set; }
        [LocalizedDisplayName("Unit", DefaultText = "Unit")]
        public string UnitName { get; set; }
        [LocalizedDisplayName("Qty", DefaultText = "Qty")]
        public int Qty { get; set; }
        [LocalizedDisplayName("Rate", DefaultText = "Rate")]
        public decimal Rate { get; set; }
        [LocalizedDisplayName("VatPerQty", DefaultText = "Vat")]
        public decimal? VatPerQty { get; set; }
        [LocalizedDisplayName("NetAmount", DefaultText = "NetAmount")]
        public decimal NetAmount { get; set; }
        public string Narration { get; set; }
        [LocalizedDisplayName("Section/Department", DefaultText = "Section/Department")]
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public int StockEffect { get; set; } = (int)NTAEnum.StockEffect.Add;

        public decimal BasicAmount { get; set; }
    }

    public class PurchaseEntryListViewModel
    {
        public int Id { get; set; }
        public string DisplayDocumentNo { get; set; }
        public int DocumentSetupId { get; set; }
        public string LedgerPageNo { get; set; }
        public string DateBS { get; set; }
        public string Remarks { get; set; }
        public int NoOfItems { get; set; }
        public string ItemList { get; set; }
        public string VendorName { get; set; }
        public string InvoiceNo { get; set; }
    }
}
