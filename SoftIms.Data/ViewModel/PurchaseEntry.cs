using DateMiti;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
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
        public string PurchaseOrderDisplayDocumentNo { get; set; }
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
        [Display(Name = "Item")]
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemSubCodeNo { get; set; }
        [Display(Name = "Ledger Page No")]
        public string LedgerPageNo { get; set; }
        [Display(Name = "Unit")]
        public string UnitName { get; set; }
        [Display(Name = "Qty")]
        public int Qty { get; set; }
        [Display(Name = "Rate")]
        public decimal Rate { get; set; }
        [Display(Name = "Vat")]
        public decimal? VatPerQty { get; set; }
        [Display(Name = "NetAmount")]
        public decimal NetAmount { get; set; }
        public string Narration { get; set; }
        [Display(Name = "Section/Department")]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public int StockEffect { get; set; } = (int)Enums.StockEffect.Add;

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

