using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using SoftIms.Data.Infrastructure;
using SoftIms.Data.ViewModel;
using static SoftIms.Data.Enums;
using System.Globalization;
using DateMiti;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace SoftIms.Data.Helper
{
   public  class CommonHelper
    {
        private UnitOfWork db;
        public CommonHelper()
        {
            db = new UnitOfWork();
            reportService = new ImsReportService();
        }
        public ImsReportService reportService;
        public string GetDocumentNo(Enums.eDocumentSetup documentSetup)
        {
            SqlParameter[] p = new SqlParameter[1];
            p[0] = new SqlParameter("@DocumentSetupId", (int)documentSetup);
            var newDocumentNo = db.SqlQuery<string>("Select dbo.fn_GetDocumentNo(@documentSetupId, 'N')", p);
            return newDocumentNo.FirstOrDefault();
        }
        public ItemStatusViewModel GetItemStatusByItemId(int itemId, DateTime date, int fiscalYearId)
        {
            ItemStatusViewModel model = new ItemStatusViewModel();
            string data = reportService.GetRemainingItemDetailByDate(itemId, date, fiscalYearId);
            var item = db.ItemRepo.GetById(itemId);
            if (item != null)
            {
                model.Id = item.Id;
                model.Code = item.Code;
                model.Name = item.Name;
                model.ItemGroupId = (int)item.ItemGroupId;
                model.ItemGroupName = item.ItemGroup.Name;
                model.ItemUnitId = (int)item.ItemUnitId;
                model.UnitName = item.ItemUnit.Name;
                if (!string.IsNullOrEmpty(data))
                {
                    string[] value = data.Split('-');//zero index :- Remaining Quantity
                    model.InStockQuantity = Convert.ToInt32(value[0]);
                }
            }
            return model;
        }
        public List<DashboardItemRequestViewModel> GetPendingItemRequests(int employeeId)
        {
            var list = db.ItemRequestRepo.Table().Where(x => x.EmployeeId == employeeId && !x.ItemReceivedBy.HasValue).OrderByDescending(x => x.Date).Take(20);
            var data = new List<DashboardItemRequestViewModel>();
            foreach (var req in list)
            {
                foreach (var item in req.ItemRequestDetails)
                {
                    data.Add(AutomapperConfig.Mapper.Map<DashboardItemRequestViewModel>(item));
                }
            }
            return data;
        }

        #region Item Release      
        public ItemReleaseViewModel SaveItemRelease(ItemReleaseViewModel model)
        {
            //bool newRecord = false;
            model.DocumentSetupId = (int)eDocumentSetup.ItemRelease;

            if (model.Details.Count == 0)
            {
                model.Errors.Add(new ValidationResult("Please enter detail.", new string[] { "" }));
            }

            if (model.Errors.Any())
                return model;

            if (!model.Validate())
                return model;

            var entity = new ItemRelease();

            if (model.Id > 0)
            {
                entity = db.ItemReleaseRepo.GetById(model.Id);
                entity.Date = model.DateBS.GetDate();
                entity.Remarks = model.Remarks;
                db.ItemReleaseDetailRepo.DeleteRange(entity.ItemReleaseDetails);
            }
            else
            {
                model.Date = model.DateBS.GetDate();
                
                entity = AutomapperConfig.Mapper.Map<ItemRelease>(model);
                entity.CreatedDate = DateTime.Now;
                db.ItemReleaseRepo.Create(entity);
                //newRecord = true;
            }

            List<int> selectedItemRequestId = new List<int>(); //Item Request Detail Id --> Child table of item request table
            foreach (var item in model.Details)
            {
                var detail = AutomapperConfig.Mapper.Map<ItemReleaseDetail>(item);
                detail.ItemReleaseId = entity.Id;
                db.ItemReleaseDetailRepo.Create(detail);

                if (!string.IsNullOrEmpty(item.ItemRequestDetailIdCSV))
                {
                    selectedItemRequestId.AddRange(item.ItemRequestDetailIdCSV.Split(',').Select(x => Convert.ToInt32(x)));
                }
            }

            //get requested items
            var releasedItemIds = model.Details.Select(x => x.ItemId).ToList();
            var requestedItems = db.ItemRequestDetailRepo.Table().Where(x => x.ItemRequestId == model.ItemRequestId && releasedItemIds.Contains(x.ItemId));
            foreach (var item in requestedItems)
            {
                item.ItemReleaseId = entity.Id;
            }

            db.SaveChanges();

            if (model.Id == 0)
                model.Message = "Item Released Successfully.";
            else
                model.Message = model.Message = "Item Released Updated Successfully.";

            return model;
        }

        public SelectList GetEmployeeList(int? sectionId, object selectedValue = null)
        {
            var data = db.EmployeeRepo.Table().Where(x => x.DepartmentId == sectionId).OrderBy(x => x.Department.DisplayOrder).ThenBy(x => x.Designation.DisplayOrder).ThenBy(x => x.Name);
            List<Employee> list = new List<Employee>();
            foreach (var item in data)
            {
                list.Add(new Employee
                {
                    Id = item.Id,
                    Name = item.DesignationId > 0 ? string.Format("{0} - {1}", item.Name, item.Designation.Name) : item.Name
                });
            }
            return new SelectList(list, "Id", "Name", selectedValue);
        }


        public ItemReleaseViewModel GetItemRelease(int id)
        {
            var data = db.ItemReleaseRepo.GetById(id);
            var model = AutomapperConfig.Mapper.Map<ItemReleaseViewModel>(data);
            if (model == null)
                return new ItemReleaseViewModel();

            foreach (var item in data.ItemReleaseDetails)
            {
                model.Details.Add(AutomapperConfig.Mapper.Map<ItemReleaseDetailViewModel>(item));
            }
            return model;
        }

        public ItemReleaseViewModel GetItemRelease(int itemReleaseId, bool isEnableButton)
        {
            var data = db.ItemReleaseRepo.GetById(itemReleaseId);
            var model = AutomapperConfig.Mapper.Map<ItemReleaseViewModel>(data);
            model.IsEnableButton = isEnableButton;
            if (model == null)
                return new ItemReleaseViewModel();

            foreach (var item in data.ItemReleaseDetails)
            {

                model.Details.Add(AutomapperConfig.Mapper.Map<ItemReleaseDetailViewModel>(item));
                foreach (var detailItems in model.Details)
                {
                    //detailItems.IsEnableButton = model.IsEnableButton;
                }
            }
            return model;
        }

        public IList<ItemReleaseListViewModel> GetItemReleases(int fiscalYearId, string displayReleaseNo, DateTime? dateFrom, DateTime? dateTo)
        {
            var data = db.ItemReleaseRepo.Table().Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.ItemRelease
                                && (string.IsNullOrEmpty(displayReleaseNo) || x.DisplayDocumentNo == displayReleaseNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!dateTo.HasValue || x.Date <= dateTo)).OrderByDescending(x => x.Date).ThenByDescending(x => x.DisplayDocumentNo);

            return AutomapperConfig.Mapper.Map<IList<ItemReleaseListViewModel>>(data);
        }

        public ServiceModel DeleteItemRelease(int itemReleaseId, int deletedBy)
        {
            var result = new ServiceModel();
            var data = db.ItemReleaseRepo.GetById(itemReleaseId);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult( "Invalid Request!!!"));
                return result;
            }

         
      
                if (data.ItemReleaseDetails != null)
                    db.ItemReleaseDetailRepo.DeleteRange(data.ItemReleaseDetails);
                db.ItemReleaseRepo.Delete(data);

                var itemRequestDetails = db.ItemRequestDetailRepo.Table().Where(x => x.ItemReleaseId == data.Id);
                foreach (var det in itemRequestDetails)
                {
                    det.ItemReleaseId = null;
                }
                db.SaveChanges();
      

            if (result.HasError)
                return result;

            result.Message =  "Deleted successfully.";
            return result;
        }
        #endregion Item Release

        public string GetRemainingItemDetailByDate(int itemId, DateTime date, int fiscalYearId)
        {
            string result = "";
            var data = new List<ItemRecordDetailViewModel>();
            //Stock Entry & Opening Stock
            var stockEntry = (from t1 in db.StockTransactionRepo.Table()
                              join t2 in db.StockTransactionDetailRepo.Table() on t1.Id equals t2.StockTransactionId
                              where (t1.DocumentSetupId == (int)eDocumentSetup.StoreEntry || t1.DocumentSetupId == (int)eDocumentSetup.Opening) && t1.FiscalYearId == fiscalYearId && t1.Date <= date && t2.ItemId == itemId
                              select new
                              {
                                  StockTransactionId = t1.Id,
                                  FiscalYearId = t1.FiscalYearId,
                                  Date = t1.Date,
                                  //DateBs = GetMiti(t1.Date),
                                  StockTransactionDetailId = t2.Id,
                                  StockTransactionDetailId_StockTransactionId = t2.StockTransactionId,
                                  ItemId = t2.ItemId,
                                  ItemName = t2.Item.Name,
                                  Qty = t2.Qty,
                                  Rate = (t2.Rate != null) ? t2.Rate : 0,
                                  VAT = (t2.Vat != null) ? t2.Vat : 0,
                                  BasicAmount = (t2.BasicAmount != null) ? t2.BasicAmount : 0,
                                  NetAmount = (t2.NetAmount != null) ? t2.NetAmount : 0
                              }).ToList();
            if (stockEntry != null)
            {
                foreach (var _storeentry in stockEntry)
                {
                    var storeEntryData = new ItemRecordDetailViewModel();
                    storeEntryData.Date = _storeentry.Date;
                    storeEntryData.DateBS = _storeentry.Date.GetMiti();
                    storeEntryData.EarningsQuantity = _storeentry.Qty;
                    storeEntryData.EarningsUnitPrice = (decimal)_storeentry.Rate;
                    storeEntryData.EarningsTotalPrice = (decimal)_storeentry.NetAmount;
                    storeEntryData.ExpenseQuantity = 0;
                    storeEntryData.ExpensesUnitPrice = 0;
                    storeEntryData.ExpensesTotalPrice = 0;
                    data.Add(storeEntryData);
                }
            }
            //Stock Adjustment
            var stockAdjustment = (from t1 in db.StockTransactionRepo.Table()
                                   join t2 in db.StockTransactionDetailRepo.Table() on t1.Id equals t2.StockTransactionId
                                   where t1.DocumentSetupId == (int)eDocumentSetup.StockAdjustment && t1.FiscalYearId == fiscalYearId && t1.Date <= date && t2.ItemId == itemId
                                   select new
                                   {
                                       Date = t1.Date,
                                       //Miti = GetMiti(t1.Date),
                                       //AdjustmentInOutStatus = (t1.AdjustmentType.InOut != null) ? t1.AdjustmentType.InOut : "",
                                       StockTransactionDetailId = t2.Id,
                                       StockTransactionDetailId_StockTransactionId = t2.StockTransactionId,
                                       ItemId = t2.ItemId,
                                       ItemName = t2.Item.Name,
                                       Qty = t2.Qty,
                                       Rate = (t2.Rate != null) ? t2.Rate : 0,
                                       VAT = (t2.Vat != null) ? t2.Vat : 0,
                                       BasicAmount = (t2.BasicAmount != null) ? t2.BasicAmount : 0,
                                       NetAmount = (t2.NetAmount != null) ? t2.NetAmount : 0
                                   }).ToList();
            //if (stockAdjustment != null)
            //{
            //    foreach (var _stockadjustment in stockAdjustment)
            //    {
            //        var stockAdjustmentData = new ItemRecordDetailViewModel();
            //        stockAdjustmentData.Date = _stockadjustment.Date;
            //        stockAdjustmentData.DateBS = _stockadjustment.Date.GetMiti();
            //        if (!string.IsNullOrEmpty(_stockadjustment.AdjustmentInOutStatus))
            //        {
            //            if (_stockadjustment.AdjustmentInOutStatus == (eStockInOutType.In).ToString())
            //            {
            //                stockAdjustmentData.EarningsQuantity = _stockadjustment.Qty;
            //                stockAdjustmentData.EarningsUnitPrice = (decimal)_stockadjustment.Rate;
            //                stockAdjustmentData.EarningsTotalPrice = (decimal)_stockadjustment.NetAmount;
            //                stockAdjustmentData.ExpenseQuantity = 0;
            //                stockAdjustmentData.ExpensesUnitPrice = 0;
            //                stockAdjustmentData.ExpensesTotalPrice = 0;
            //            }
            //            if (_stockadjustment.AdjustmentInOutStatus == (eStockInOutType.Out).ToString())
            //            {
            //                stockAdjustmentData.EarningsQuantity = 0;
            //                stockAdjustmentData.EarningsUnitPrice = 0;
            //                stockAdjustmentData.EarningsTotalPrice = 0;
            //                stockAdjustmentData.ExpenseQuantity = _stockadjustment.Qty;
            //                stockAdjustmentData.ExpensesUnitPrice = (decimal)_stockadjustment.Rate;
            //                stockAdjustmentData.ExpensesTotalPrice = (decimal)_stockadjustment.NetAmount;
            //            }
            //        }
            //        data.Add(stockAdjustmentData);
            //    }
            //}
            //Item Release
            var itemRelease = (from t1 in db.ItemReleaseRepo.Table()
                               join t2 in db.ItemReleaseDetailRepo.Table() on t1.Id equals t2.ItemReleaseId
                               where t1.DocumentSetupId == (int)eDocumentSetup.ItemRelease && t1.FiscalYearId == fiscalYearId && t1.Date <= date && t2.ItemId == itemId
                               select new
                               {
                                   Date = t1.Date,
                                   //Miti = GetMiti(t1.Date),
                                   ItemReleaseDetailId = t2.Id,
                                   ItemReleaseDetailId_ItemReleaseId = t2.ItemReleaseId,
                                   ItemId = t2.ItemId,
                                   ItemName = t2.Item.Name,
                                   Qty = t2.Qty,
                                   Rate = 0,
                                   VAT = 0,
                                   BasicAmount = 0,
                                   NetAmount = 0,
                               }).ToList();

            if (itemRelease != null)
            {
                foreach (var _itemRelease in itemRelease)
                {
                    var itemReleaseData = new ItemRecordDetailViewModel();
                    itemReleaseData.Date = _itemRelease.Date;
                    itemReleaseData.DateBS = _itemRelease.Date.GetMiti();
                    itemReleaseData.EarningsQuantity = 0;
                    itemReleaseData.EarningsUnitPrice = 0;
                    itemReleaseData.EarningsTotalPrice = 0;
                    itemReleaseData.ExpenseQuantity = _itemRelease.Qty;
                    itemReleaseData.ExpensesUnitPrice = (decimal)_itemRelease.Rate;
                    itemReleaseData.ExpensesTotalPrice = (decimal)_itemRelease.NetAmount;
                    data.Add(itemReleaseData);
                }
            }

            if (data != null && data.Count > 0)
            {
                int totalEarningQuantity = data.Sum(x => x.EarningsQuantity);
                decimal totalEarningTotalAmount = data.Sum(x => x.EarningsTotalPrice);
                int totalExpensesQuantity = data.Sum(x => x.ExpenseQuantity);
                decimal totalExpensesTotalAmount = data.Sum(x => x.ExpensesTotalPrice);
                int totalRemainigsQuantity = ((totalEarningQuantity - totalExpensesQuantity) < 0) ? 0 : totalEarningQuantity - totalExpensesQuantity;
                decimal totalRemainigsTotalAmount = 0;//((totalEarningTotalAmount - totalExpensesTotalAmount) < 0) ? 0 : totalEarningTotalAmount - totalExpensesTotalAmount;
                decimal Rate = 0;//data.Select(x=> x.EarningsUnitPrice).Max();
                result = totalRemainigsQuantity.ToString() + "-" + Rate.ToString("N2", CultureInfo.InvariantCulture) + "-" + totalRemainigsTotalAmount.ToString("N2", CultureInfo.InvariantCulture);
            }
            return result;
        }
        public ItemRequestItemStatusViewModel GetItemRequestItemStatus(int itemRequestId, DateTime date, int fiscalYearId)
        {
            var model = new ItemRequestItemStatusViewModel();
            var data = db.ItemRequestRepo.GetById(itemRequestId);
            if (data != null && data.ItemRequestDetails != null)
            {
                model.Remarks = data.Remarks;
                foreach (var item in data.ItemRequestDetails)
                {
                    var _item = new ItemRequestItemStatusListViewModel();
                    var _data = GetItemStatusByItemId(item.ItemId, date, fiscalYearId);
                    _item.ItemId = _data.Id;
                    _item.Code = _data.Code;
                    _item.Name = _data.Name;
                    _item.ItemGroupId = _data.ItemGroupId;
                    _item.ItemGroupName = _data.ItemGroupName;
                    _item.ItemUnitId = _data.ItemUnitId;
                    _item.UnitName = _data.UnitName;
                    _item.OnDemandQuantity = item.Qty;
                    _item.InStockQuantity = _data.InStockQuantity;
                    model.Details.Add(_item);
                }
            }
            return model;
        }
        public ItemRequestViewModel GetItemRequest(int itemRequestId)
        {
            var data = db.ItemRequestRepo.GetById(itemRequestId);
            var model = AutomapperConfig.Mapper.Map<ItemRequestViewModel>(data);

            if (model == null)
                return new ItemRequestViewModel();

            model.FiscalYearDetail = (model.FiscalYearId > 0) ? db.FiscalYearRepo.GetById(model.FiscalYearId) : new FiscalYear();
            return model;
        }
        public IList<ItemRequestViewModel> GetItemRequests(int fiscalYearId, int employeeId, string displayRequestNo, DateTime? dateFrom, DateTime? dateTo, bool unProcessedOnly = false, bool unReleasedOnly = false, bool acceptedOnly = false, string itemRequestIds = "")
        {
            var data = db.ItemRequestRepo.Table().Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.ItemRequest
                                && (employeeId == 0 || x.EmployeeId == employeeId)
                                && (string.IsNullOrEmpty(displayRequestNo) || x.DisplayDocumentNo == displayRequestNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!dateTo.HasValue || x.Date <= dateTo)
                                && (unProcessedOnly || !acceptedOnly || x.AcceptedBy.HasValue)
                                && (!unProcessedOnly || (unProcessedOnly && x.ItemRequestDetails.Any(x1 => !x1.PurchaseOrderId.HasValue)))
                                && (!unReleasedOnly || (unReleasedOnly && x.ItemRequestDetails.Any(x1 => !x1.ItemReleaseId.HasValue)))).OrderByDescending(x => x.Date).ThenByDescending(x => x.DisplayDocumentNo);

            var model = AutomapperConfig.Mapper.Map<List<ItemRequestViewModel>>(data);
            foreach (var req in data)
            {
                foreach (var item in req.ItemRequestDetails)
                {
                    if (unProcessedOnly && item.PurchaseOrderId.HasValue)
                        model.FirstOrDefault(x => x.Id == req.Id).Details.RemoveAll(x => x.Id == item.Id);

                    if (unReleasedOnly && item.ItemReleaseId.HasValue)
                        model.FirstOrDefault(x => x.Id == req.Id).Details.RemoveAll(x => x.Id == item.Id);
                }
            }
            if (!string.IsNullOrEmpty(itemRequestIds))
            {
                string[] _itemRequestIds = itemRequestIds.Split(',');
                foreach (var item in _itemRequestIds.Distinct())
                {
                    if (!model.Any(x => x.Id == Convert.ToInt32(item)))
                    {
                        int requestId = Convert.ToInt32(item);
                        var data1 = db.ItemRepo.GetById(requestId);
                        var selectedRequest = AutomapperConfig.Mapper.Map<ItemRequestViewModel>(data1);
                        model.Add(selectedRequest);
                    }
                }
            }
            var modelResult = AutomapperConfig.Mapper.Map<IList<ItemRequestViewModel>>(data);
            return modelResult;
        }
        #region PurchaseEntry
        public PurchaseOrderViewModel GetPurchaseOrder(int purchaseOrderId)
        {
            var data = db.PurchaseOrderRepo.GetById(purchaseOrderId);
            var model = AutomapperConfig.Mapper.Map<PurchaseOrderViewModel>(data);
            if (model == null)
                return new PurchaseOrderViewModel();
            var vendor = (model.VendorId > 0) ? db.VendorRepo.GetById(model.VendorId) : new Vendor();
            model.VendorDetails = AutomapperConfig.Mapper.Map<VendorViewModel>(vendor);

            foreach (var item in data.PurchaseOrderDetails)
            {
                model.Details.Add(AutomapperConfig.Mapper.Map<PurchaseOrderDetailViewModel>(item));
            }
            return model;
        }
        public PurchaseEntryViewModel GetPurchaseEntry(int PurchaseEntryId)
        {
            var data = db.StockTransactionRepo.GetById(PurchaseEntryId);
            var model = AutomapperConfig.Mapper.Map<PurchaseEntryViewModel>(data);
            if (model == null)
                return new PurchaseEntryViewModel();

            foreach (var item in data.StockTransactionDetails)
            {
                model.Details.Add(AutomapperConfig.Mapper.Map<PurchaseEntryDetailViewModel>(item));
            }

            if (model.PurchaseOrderId.HasValue)
            {
                var _purchaseOrder = db.PurchaseOrderRepo.GetById(Convert.ToInt32(model.PurchaseOrderId));
                if (_purchaseOrder.VendorId.HasValue)
                {
                    var vendorDetail = db.VendorRepo.GetById(_purchaseOrder.VendorId.Value);
                    model.VendorDetail = AutomapperConfig.Mapper.Map<VendorViewModel>(vendorDetail);
                }
            }
            return model;
        }
        public IList<PurchaseOrderListViewModel> GetPurchaseOrders(int fiscalYearId, string DisplayDocumentNo, DateTime? dateFrom, DateTime? dateTo, int? stockTransactionId, bool noLinkedToPurchaseEntry = false)
        {
            var data = db.PurchaseOrderRepo.Table().Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.PurchaseOrder
                                && (string.IsNullOrEmpty(DisplayDocumentNo) || x.DisplayDocumentNo == DisplayDocumentNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!noLinkedToPurchaseEntry || (!x.StockTransactions.Any() || x.PurchaseOrderDetails.Sum(x1 => x1.Qty) > x.StockTransactions.Sum(x1 => x1.StockTransactionDetails.Sum(x2 => x2.Qty)) || x.StockTransactions.Any(y => y.Id == stockTransactionId)))
                                && (!dateTo.HasValue || x.Date <= dateTo)).OrderByDescending(x => x.DisplayDocumentNo);
            return AutomapperConfig.Mapper.Map<IList<PurchaseOrderListViewModel>>(data);
        }
        public PurchaseEntryViewModel SavePurchaseEntry(PurchaseEntryViewModel model)
        {
            model.DocumentSetupId = (int)eDocumentSetup.StoreEntry;

            if (model.Details.Count == 0)
            {
                model.Errors.Add(new ValidationResult("Please enter detail.", new string[] { "" }));
            }
            else
            {
                if (model.Details.Any(x => x.DepartmentId == 0))
                    model.Errors.Add(new ValidationResult("Please select dept./section in item list.", new string[] { "" }));
            }

            if (model.Errors.Any())
                return model;

            if (!model.Validate())
                return model;

            model.Date = model.DateBS.GetDate();

            var entity = new StockTransaction();

            if (model.Id > 0)
            {
                entity = db.StockTransactionRepo.GetById(model.Id);
                entity.PurchaseOrderId = model.PurchaseOrderId;
                entity.Remarks = model.Remarks;
                entity.InvoiceNo = model.InvoiceNo;
                db.StockTransactionDetailRepo.DeleteRange(entity.StockTransactionDetails);
            }
            else
            {
                entity = AutomapperConfig.Mapper.Map<StockTransaction>(model);
                entity.CreatedDate = DateTime.Now;
                db.StockTransactionRepo.Create(entity);
            }

            foreach (var item in model.Details)
            {
                var detail = AutomapperConfig.Mapper.Map<StockTransactionDetail>(item);
                detail.StockTransactionId = entity.Id;
                detail.LedgerPageNo = string.IsNullOrEmpty(detail.LedgerPageNo) ? null : detail.LedgerPageNo;
                db.StockTransactionDetailRepo.Create(detail);
            }

            if (model.Id == 0)
                model.Message = "Data saved successfully.";
            else
                model.Message =  "Save changes successfully.";
            return model;
        }

        #endregion


        #region Random string
        public char GenerateChar(System.Random random)
        {
            char randomChar;
            int next = random.Next(0, 62);
            if (next < 10)
            {
                randomChar = (char)(next + '0'); //0, 1, 2 ... 9
            }
            else if (next < 36)
            {
                randomChar = (char)(next - 10 + 'A'); //A, B, C ... Z
            }
            else
            {
                randomChar = (char)(next - 36 + 'a'); //a, b, c ... z
            }
            return randomChar;
        }

        public string GenerateString(System.Random random, int length)
        {
            char[] randomStr = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomStr[i] = GenerateChar(random);
            }
            return new string(randomStr);
        } 
        #endregion


    }
}
