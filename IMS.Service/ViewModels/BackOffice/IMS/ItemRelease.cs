using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
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
        [LocalizedDisplayName("ItemReleaseNo", DefaultText = "Release No")]
        public string DisplayReleaseNo { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        [Display(Name = "Date")]
        public string DateBS { get; set; } = DateMiti.GetDateMiti.GetMiti(DateTime.Today);
        [LocalizedDisplayName("Employee", DefaultText = "Employee")]
        public int EmployeeId { get; set; }
        [LocalizedDisplayName("Section", DefaultText = "Section")]
        public int SectionId { get; set; }
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
        [LocalizedDisplayName("Item", DefaultText = "Item")]
        [TRequired(ErrorMessageResourceName = "Required.Item", DefaultText = "Please select item.")]
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string SubCode { get; set; }
        [LocalizedDisplayName("Unit", DefaultText = "Unit")]
        public string UnitName { get; set; }
        [LocalizedDisplayName("Qty", DefaultText = "Qty")]
        public int Qty { get; set; }
        public string Narration { get; set; }
        public int InStockQty { get; set; }
    }

    public class ItemReleaseListViewModel
    {
        public int Id { get; set; }
        public int DocumentSetupId { get; set; }
        public string EmployeeName { get; set; }
        public string SectionName { get; set; }
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
