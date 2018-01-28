using IMS.Logic.ViewModels.BackOffice.IMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Logic.Contract
{
    public interface IImsReportService
    {
        ItemRecordViewModel GetItemRecord(int? itemId, int fiscalYearId);
        ItemRecordViewModel GetItemSubRecord(int? itemId, int fiscalYearIds);
        string GetRemainingItemDetailByDate(int itemId, DateTime date, int fiscalYearId);
        List<ItemLedgerViewModel> ItemLedgerDetail(string fiscalYear, string itemCode);
        SelectList ItemCategories();
        SelectList Items(string itemCategory);
        string GetItem(string code);
        List<DashboardItemRequestViewModel> GetPendingItemRequests(int employeeId);
    }
}
