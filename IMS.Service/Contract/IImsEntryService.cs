using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Data;
using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.IMS;
using PagedList;

namespace IMS.Logic.Contract
{
    public interface IImsEntryService
    {
        ServiceModel DeleteStockTransaction(int stockTransactionId);

        OpeningStockViewModel GetOpeningStock(int openingStockId);
        IList<OpeningStockListViewModel> GetOpeningStocks(int fiscalYearId, string displayDocumentNo, DateTime? dateFrom, DateTime? dateTo, int? sectionId, int? employeeId);
        OpeningStockViewModel SaveOpeningStock(OpeningStockViewModel model);

        ItemRequestItemStatusViewModel GetItemRequestItemStatus(int itemRequestId, DateTime date, int fiscalYearId);
        IList<ItemRequestViewModel> GetItemRequests(int fiscalYearId, int employeeId, string displayRequestNo, DateTime? dateFrom, DateTime? dateTo, bool unProcessedOnly = false, bool unReleasedOnly = false, bool acceptedOnly = false, string itemRequestIds = "");
        ItemRequestViewModel SaveItemRequest(ItemRequestViewModel model);
        ItemRequestViewModel GetItemRequest(int itemRequestId);
        ServiceModel DeleteItemRequest(int itemRequestId, int deletedBy);

        PurchaseOrderViewModel SavePurchaseOrder(PurchaseOrderViewModel model);
        PurchaseOrderViewModel GetPurchaseOrder(int purchaseOrderId);
        PurchaseOrderListViewModel GetPurchaseOrderList(int purchaseOrderId);

        IList<PurchaseOrderListViewModel> GetPurchaseOrders(int fiscalYearId, string displayOrderNo, DateTime? dateFrom, DateTime? dateTo, int? stockTransactionId, bool noLinkedToPurchaseEntry = false);
        ServiceModel DeletePurchaseOrder(int purchaseOrderId, int deletedBy);

        ItemReleaseViewModel SaveItemRelease(ItemReleaseViewModel model);
        ItemReleaseViewModel GetItemRelease(int id);
        ItemReleaseViewModel GetItemRelease(int itemReleaseId, bool isEnableButton);
        IList<ItemReleaseListViewModel> GetItemReleases(int fiscalYearId, string displayReleaseNo, DateTime? dateFrom, DateTime? dateTo);
        ServiceModel DeleteItemRelease(int itemReleaseId, int deletedBy);

        PurchaseEntryViewModel GetPurchaseEntry(int PurchaseEntryId);
        IList<PurchaseEntryListViewModel> GetPurchaseEntrys(int fiscalYearId, string displayDocumentNo, DateTime? dateFrom, DateTime? dateTo, int? vendorId);
        PurchaseEntryViewModel SavePurchaseEntry(PurchaseEntryViewModel model);

        StockAdjustmentViewModel GetStockAdjustment(int stockTransactionId);
        IPagedList<StockAdjustmentListViewModel> GetStockAdjustments(int fiscalYearId, string displayDocumentNo, DateTime? dateFrom, DateTime? dateTo, int? page = default(int?), int? pageSize = default(int?));
        StockAdjustmentViewModel SaveStockAdjustment(StockAdjustmentViewModel model);
    }
}
