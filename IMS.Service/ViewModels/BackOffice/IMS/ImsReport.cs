using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class ItemRecordViewModel
    {
        public ItemRecordViewModel()
        {
            this.Detail = new List<ItemRecordDetailViewModel>();
            this.SubDetail = new List<ItemSubRecordDetailViewModel>();
        }

        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemUnitId { get; set; }
        public string ItemUnitName { get; set; }
        public string ItemCode { get; set; }
        public string Specification { get; set; }
        
        public List<ItemRecordDetailViewModel> Detail { get; set; }
        public List<ItemSubRecordDetailViewModel> SubDetail { get; set; }
    }

    public class ItemRecordDetailViewModel
    {
        public string Specification { get; set; }
        public string ManufacturingCompany { get; set; }
        public string ItemSize { get; set; }
        public string EstimatedLifeSpan { get; set; }
        public string PurchasingCompany { get; set; }
        public string DisplayDocumentNo { get; set; }
        public DateTime Date { get; set; }
        public string DateBS { get; set; }
        public int EarningsQuantity { get; set; }
        public decimal EarningsUnitPrice { get; set; }
        public decimal EarningsTotalPrice { get; set; }
        public int ExpenseQuantity { get; set; }
        public decimal ExpensesUnitPrice { get; set; }
        public decimal ExpensesTotalPrice { get; set; }
        public int RemainingQuantity { get; set; }
        public decimal RemainingUnitPrice { get; set; }
        public decimal RemainingTotalPrice { get; set; }
        public string Remarks { get; set; }
    }

    public class ItemSubRecordDetailViewModel
    {
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DisplayDocumentNo { get; set; }
        public DateTime Date { get; set; }
        public string DateBS { get; set; }
        public int EarningsQuantity { get; set; }
        public decimal EarningsUnitPrice { get; set; }
        public decimal EarningsTotalPrice { get; set; }
        public int ExpenseQuantity { get; set; }
        public decimal ExpensesUnitPrice { get; set; }
        public decimal ExpensesTotalPrice { get; set; }
        public int RemainingQuantity { get; set; }
        public string Narration { get; set; }
    }
}
