using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class PurchaseOrderViewModel : ServiceModel
    {
        public PurchaseOrderViewModel()
        {
            Details = new List<PurchaseOrderDetailViewModel>();
        }

        public int Id { get; set; }
        public int FiscalYearId { get; set; }
        public int DocumentSetupId { get; set; }
        [Display(Name = "बिक्रेता")]
        public int VendorId { get; set; }
        public int? OrderNo { get; set; }
        [Display(Name = "खरिद आदेश नं.")]
        public string DisplayOrderNo { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        [Display(Name = "मिति")]
        public string DateBS { get; set; } = DateMiti.GetDateMiti.GetMiti(DateTime.Today);
        public DateTime? DueDate { get; set; }
        [Display(Name = "दाखिला गर्नु पर्ने मिति")]
        public string DueDateBS { get; set; }
        [Display(Name = "कैफियत")]
        public string Remarks { get; set; }
        public int CurrentUserId { get; set; }

        [Display(Name = "स्वीकृत गर्ने व्यक्ति")]
        [Required]
        public int AcceptedBy { get; set; }

        public VendorViewModel VendorDetails { get; set; }
        public IList<PurchaseOrderDetailViewModel> Details { get; set; }

        public int EmployeeId { get; set; }
    }

    public class PurchaseOrderDetailViewModel
    {
        public string ItemRequestDetailIdCSV { get; set; }

        public string ItemRequestIdCSV { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }

        [LocalizedDisplayName("Item", DefaultText = "Item")]
        [TRequired(ErrorMessageResourceName = "Required.Item", DefaultText = "Please select item.")]
        public int ItemId { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        [LocalizedDisplayName("Unit", DefaultText = "Unit")]
        public string UnitName { get; set; }

        [LocalizedDisplayName("Qty", DefaultText = "Qty")]
        public int Qty { get; set; }

        [LocalizedDisplayName("Rate", DefaultText = "Rate")]
        public decimal Rate { get; set; }

        [LocalizedDisplayName("BasicAmount", DefaultText = "Basic Amount")]
        public decimal BasicAmount { get; set; }

        [LocalizedDisplayName("TotalAmount", DefaultText = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public string Specification { get; set; }

        public int InStockQty { get; set; }
    }

    public class PurchaseOrderListViewModel
    {
        public PurchaseOrderListViewModel()
        {
            this.Details = new List<PurchaseOrderDetailViewModel>();
        }
        public int Id { get; set; }
        public int DocumentSetupId { get; set; }
        public string Vendor { get; set; }
        public string DisplayOrderNo { get; set; }
        public string DateBS { get; set; }
        public DateTime Date { get; set; }
        public string Remarks { get; set; }
        public int NoOfItems { get; set; }
        public bool IsPurchased { get; set; }
        public string ItemList { get; set; }
        public bool FullEntry { get; set; }
        public string AcceptedEmployeeName { get; set; }
        public int ApplicationStatus { get; set; }

        public string CreatedEmployeeName { get; set; }
        public string AcceptedDateBs { get; set; }
        public List<PurchaseOrderDetailViewModel> Details { get; set; }

    }

    public class UnprocessedItemRequestViewModel
    {
        public int Id { get; set; }
        public string DisplayRequestNo { get; set; }
    }
}
