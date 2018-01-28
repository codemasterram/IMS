using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.Home;
using IMS.Logic.ViewModels.BackOffice.IMS;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.Contract
{
    public interface IDashboardServicecs
    {
        #region MyRegion
        DashboardAttendanceSummaryViewModel GetDashboardAttendanceSummary(DateTime date);
        List<DashboardEmployeeTravelDetailListViewModel> GetDashboardEmployeeTravelDetailList(DateTime date);
        List<DashboardEmployeeLeaveDetailListViewModel> GetDashboardEmployeeLeaveDetailList(DateTime date);
        #endregion

        #region Inventory
        ItemRequestItemStatusViewModel GetPersonalItemRequestItemStatus(int itemRequestId, DateTime date, int fiscalYearId);
        IPagedList<ItemRequestViewModel> GetPersonalItemRequests(int fiscalYearId, int employeeId, string displayRequestNo, DateTime? dateFrom, DateTime? dateTo, bool unProcessedOnly = false, bool unReleasedOnly = false, string itemRequestIds = "", int? page = default(int?), int? pageSize = default(int?));
        ItemRequestViewModel SavePersonalItemRequest(ItemRequestViewModel model);
        ItemRequestViewModel GetPersonalItemRequest(int itemRequestId);
        ServiceModel DeletePersonalItemRequest(int itemRequestId, int deletedBy);
        #endregion
    }
}
