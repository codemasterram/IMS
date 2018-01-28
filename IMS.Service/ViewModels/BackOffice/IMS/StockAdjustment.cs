using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class StockAdjustmentViewModel : ServiceModel
    {
        public StockAdjustmentViewModel()
        {
            this.Details= new List<StockAdjustmentDetailViewModel>();
        }

        public int Id { get; set; }
        public int FiscalYearId { get; set; }
        public int DocumentSetupId { get; set; }
        public Nullable<int> DocumentNo { get; set; }
        [TRequired(ErrorMessageResourceName = "AdjustmentTypeId.Required.Name", DefaultText = "Please select adjustment type.")]
        [LocalizedDisplayName("AdjustmentType", DefaultText = "Adjustment Type")]
        public int AdjustmentTypeId { get; set; }
        [LocalizedDisplayName("AdjustmentNo", DefaultText = "Adjustment no")]
        public string DisplayDocumentNo { get; set; }
        public System.DateTime Date { get; set; } = DateTime.Today;
        public string DateBS { get; set; } = DateMiti.GetDateMiti.GetMiti(DateTime.Today);
        public string Remarks { get; set; }
        
        public List<StockAdjustmentDetailViewModel> Details { get; set; }
    }

    public class StockAdjustmentDetailViewModel
    {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public int Id { get; set; }
        public int StockTransactionId { get; set; }
        [LocalizedDisplayName("Item", DefaultText = "Item")]
        [TRequired(ErrorMessageResourceName = "Required.Item", DefaultText = "Please select item.")]
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        [LocalizedDisplayName("Unit", DefaultText = "Unit")]
        public string UnitName { get; set; }
        [LocalizedDisplayName("Qty", DefaultText = "Qty")]
        public int Qty { get; set; }
        [LocalizedDisplayName("Rate", DefaultText = "Rate")]
        public decimal Rate { get; set; }
        [LocalizedDisplayName("NetAmout", DefaultText = "Net amount")]
        public decimal NetAmount { get; set; }
        public string Narration { get; set; }
        [TRequired(ErrorMessageResourceName = "Required.Section", DefaultText = "Please select section/department.")]
        [LocalizedDisplayName("Section/Department", DefaultText = "Section/Department")]
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        [LocalizedDisplayName("Employee", DefaultText = "Employee")]
        public Nullable<int> EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int StockEffect { get; set; } = (int)Data.NTAEnum.StockEffect.Add;
    }

    public class StockAdjustmentListViewModel
    {
        public int Id { get; set; }
        public string DisplayDocumentNo { get; set; }
        public int AdjustmentTypeId { get; set; }
        public string AdjustmentTypeName { get; set; }
        public string DateBS { get; set; }
        public string Remarks { get; set; }
        public int NoOfItems { get; set; }
    }

}
