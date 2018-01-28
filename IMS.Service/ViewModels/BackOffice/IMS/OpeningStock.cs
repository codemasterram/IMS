using IMS.Data;
using IMS.Logic.DataAnnotations;
using IMS.Logic.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class OpeningStockViewModel : ServiceModel
    {
        public OpeningStockViewModel()
        {
            this.Details = new List<OpeningStockDetailViewModel>();
        }

        public int Id { get; set; }

        [LocalizedDisplayName("FiscalYear", DefaultText = "Fiscal Year")]
        public int FiscalYearId { get; set; }
        public int DocumentSetupId { get; set; }
        public Nullable<int> DocumentNo { get; set; }
        [LocalizedDisplayName("OpeningNo", DefaultText = "Opening No")]
        public string DisplayDocumentNo { get; set; }
        [Required]
        [Display(Name = "मिति")]
        public string DateBS { get; set; }
        public System.DateTime Date { get; set; } = DateTime.Today;
        [Display(Name = "कैफियत")]
        public string Remarks { get; set; }

        public List<OpeningStockDetailViewModel> Details { get; set; }
    }

    public class OpeningStockDetailViewModel
    {
        public int Id { get; set; }
        public int StockTransactionId { get; set; }
        [LocalizedDisplayName("Item", DefaultText = "Item")]
        [TRequired(ErrorMessageResourceName = "Required.Item", DefaultText = "Please select item.")]
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemSubCodeNo { get; set; }
        [LocalizedDisplayName("Unit", DefaultText = "Unit")]
        public string UnitName { get; set; }
        [LocalizedDisplayName("Qty", DefaultText = "मात्रा")]
        [Range(1, 9999, ErrorMessage = "The {0} must greater than {1}.")]
        public int Qty { get; set; }
        [Display(Name = "Cur. Value")]
        public decimal? Amount { get; set; }
        public string Narration { get; set; }
        [TRequired(ErrorMessageResourceName = "Required.Section", DefaultText = "Please select section/department.")]
        [LocalizedDisplayName("Section/Department", DefaultText = "Section/Department")]
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        [LocalizedDisplayName("Employee", DefaultText = "Employee")]
        public Nullable<int> EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Guid { get; set; }

        public string PurchaseDateBS { get; set; }
        public decimal? PurchaseAmount { get; set; }

        public int StockEffect { get; set; } = (int)NTAEnum.StockEffect.Add;
    }

    public class OpeningStockListViewModel : ServiceModel
    {
        public OpeningStockListViewModel()
        {
            this.Details = new List<ItemRequestDetailViewModel>();
        }
        public int Id { get; set; }
        public string DisplayDocumentNo { get; set; }
        public string DateBS { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int NoOfItems { get; set; }
        public int CreatedBy { get; set; }

        public string ItemList { get; set; }
        public string EmployeeDesignation { get; set; }
        public string EmployeeSection { get; set; }

        public List<ItemRequestDetailViewModel> Details { get; set; }

        public string SectionName { get; set; }
        public string EmployeeName { get; set; }


    }
}
