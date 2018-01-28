using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
{
    public class ItemReleaseViewModel : ServiceModel
    {
        public ItemReleaseViewModel()
        {
            Details = new List<ItemReleaseDetailViewModel>();
        }

        public int Id { get; set; }
        public int FiscalYearId { get; set; }
        public int DocumentSetupId { get; set; }
        public int? ReleaseNo { get; set; }
        [Display(Name = "Release No")]
        public string DisplayReleaseNo { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        [Display(Name = "Date")]
        public string DateBS { get; set; } = DateMiti.GetDateMiti.GetMiti(DateTime.Today);
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Display(Name = "Request No")]
        public int ItemRequestId { get; set; }
        public string Remarks { get; set; }
        public int CurrentEmployeeId { get; set; }
        public bool IsEnableButton { get; set; }

        public IList<ItemReleaseDetailViewModel> Details { get; set; }
    }

    public class ItemReleaseDetailViewModel
    {
        public string ItemRequestIdCSV { get; set; }
        public string ItemRequestDetailIdCSV { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public int Id { get; set; }
        public int ItemReleaseId { get; set; }
        [Display(Name = "Item")]
        [Required(ErrorMessage = "Please select item.")]
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string SubCode { get; set; }
        [Display(Name = "Unit")]
        public string UnitName { get; set; }
        [Display(Name = "Qty")]
        public int Qty { get; set; }
        public string Narration { get; set; }
        public int InStockQty { get; set; }
    }

    public class ItemReleaseListViewModel
    {
        public int Id { get; set; }
        public int DocumentSetupId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string DisplayReleaseNo { get; set; }
        public string DisplayRequestNo { get; set; }
        public string DateBS { get; set; }
        public string Remarks { get; set; }
        public string ItemList { get; set; }
        public bool IsAccepted { get; set; }
    }

    public class TransactionItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int? Qty { get; set; }
    }
}
