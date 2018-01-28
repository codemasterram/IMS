using IMS.Data.Infrastructure;
using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class ItemRequestViewModel : ServiceModel
    {
        public ItemRequestViewModel()
        {
            this.Details = new List<ItemRequestDetailViewModel>();
        }
        public int Id { get; set; }
        [Required]
        public int DocumentSetupId { get; set; }

        [Display(Name = "माग गर्नेको बिभाग शाखा")]
        [Required]
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        [Display(Name = "माग गर्नेको नाम")]
        [Required]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDesignation { get; set; }
        public string EmployeeSection { get; set; }
        public int FiscalYearId { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        [Display(Name = "मिति")]
        public string DateBS { get; set; } = DateMiti.GetDateMiti.GetMiti(DateTime.Today);
        public Nullable<int> RequestNo { get; set; }
        [Display(Name = "मा.फा.नं.")]
        public string DisplayRequestNo { get; set; }
        [Display(Name = "कैफियत")]
        public string Remarks { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public int ApproveStatus { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public string ApprovedRemarks { get; set; }
        public int ReleasedType { get; set; }
        public List<ItemRequestDetailViewModel> Details { get; set; }
        public FiscalYear FiscalYearDetail { get; set; }
        public bool AnyItemsLeftForPurchaseOrder { get; set; }
        public bool AnyItemsLeftToReleased { get; set; }
        public int ItemRequestStatus { get; set; }
        public int ApplicationStatus { get; set; }
        public int NoOfItems
        {
            get
            {
                return Details.Sum(x => x.Qty);
            }
        }

        [Display(Name = "सिफारिस गर्ने व्यक्ति")]
        [Required]
        public int RequestedEmployeeId { get; set; }
        public string AcceptedBy { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public string ItemReceivedBy { get; set; }
        public DateTime? ItemReceivedDate { get; set; }

        public string ItemList { get; set; }
    }

    public class ItemRequestDetailViewModel
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public int ItemRequestId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public int Qty { get; set; }
        public int? PurchaseOrderId { get; set; }
        public int? ItemReleaseId { get; set; }
        public string Remarks { get; set; }
        public string Specification { get; set; }
    }

    public class ItemRequestListViewModel
    {
        public int Id { get; set; }
        public int DocumentSetupId { get; set; }
        public int SectionId { get; set; }
        public string Section { get; set; }
        public int EmployeeId { get; set; }
        public string Employee { get; set; }
        public int FiscalYearId { get; set; }
        public System.DateTime Date { get; set; }
        public string DateBS { get; set; }
        public Nullable<int> RequestNo { get; set; }
        public string DisplayRequestNo { get; set; }
        public int NoOfItems { get; set; }
        public string Remarks { get; set; }
        public int ReleasedType { get; set; }
    }

    public class ItemRequestItemStatusViewModel
    {
        public ItemRequestItemStatusViewModel()
        {
            this.Details = new List<ItemRequestItemStatusListViewModel>();
        }
        public string Usages { get; set; }
        public string Remarks { get; set; }
        public List<ItemRequestItemStatusListViewModel> Details { get; set; }
    }

    public class ItemRequestItemStatusListViewModel
    {
        public int ItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }
        public int ItemUnitId { get; set; }
        public string UnitName { get; set; }
        public int OnDemandQuantity { get; set; }
        public int InStockQuantity { get; set; }
    }

    public class ItemRequestReleasedNotificationViewModel
    {
        public ItemRequestReleasedNotificationViewModel()
        {
            this.Details = new List<ItemRequestDetailNotificationViewModel>();
        }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int ItemRequestId { get; set; }
        public int DocumentSetupId { get; set; }
        public DateTime Date { get; set; }
        public string DateBS { get; set; }
        public string DisplayRequestNo { get; set; }
        public string Remarks { get; set; }

        public List<ItemRequestDetailNotificationViewModel> Details { get; set; }
    }

    public class ItemRequestDetailNotificationViewModel
    {
        public int ItemRequestDetailId { get; set; }
        public int ItemRequestId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemUnitName { get; set; }
        public string Specification { get; set; }
        public int Quantity { get; set; }
    }
}
