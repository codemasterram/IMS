using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using IMS.Logic.ViewModels.BackOffice.IMS;
using System.Text;
using System.Threading.Tasks;
using static IMS.Data.NTAEnum;
using static DateMiti.GetDateMiti;
using System.Globalization;
using IMS.Logic.Configuration;
using System.Web.Mvc;

namespace IMS.Logic.Implementation
{
    public class ImsReportService : IImsReportService
    {
        #region Constructor
        private ILocalizationService localizationService;
        private IExceptionService exceptionService;

        private IRepository<ItemRequest> itemRequestRepository;

        private IRepository<StockTransaction> stockTransactionReporsitory;
        private IRepository<StockTransactionDetail> stockTransanctionDetailRepository;

        private IRepository<ItemRelease> itemReleaseRepository;
        private IRepository<ItemReleaseDetail> itemReleaseDetailRepository;

        private IRepository<Vendor> vendorRepository;
        private IRepository<FiscalYear> fiscalYearRepository;
        private IRepository<AdjustmentType> adjustmentTypeRepository;
        private IRepository<Item> itemRepository;
        private IRepository<ItemLedger> itemLedgerRepository;
        private IRepository<ItemLedgerMaster> itemLedgerMasterRepository;

        public ImsReportService(
            ILocalizationService localizationSvc,
            IExceptionService exceptionSVC,
            IRepository<StockTransaction> stockTransactionRepo,
            IRepository<StockTransactionDetail> stockTransactionDetailRepo,
            IRepository<ItemRelease> itemReleaseRepo,
            IRepository<ItemReleaseDetail> itemReleaseDetailRepo,
            IRepository<Vendor> vendorRepo,
            IRepository<FiscalYear> fiscalYearRepo,
            IRepository<AdjustmentType> adjustmentTypeRepo,
            IRepository<Item> itemRepo,
            IRepository<ItemLedger> itemLedgerRepo,
            IRepository<ItemLedgerMaster> itemLedgerMasterRepo,
            IRepository<ItemRequest> itemRequestRepo
            )
        {
            localizationService = localizationSvc;
            exceptionService = exceptionSVC;
            stockTransactionReporsitory = stockTransactionRepo;
            stockTransanctionDetailRepository = stockTransactionDetailRepo;
            itemReleaseRepository = itemReleaseRepo;
            itemReleaseDetailRepository = itemReleaseDetailRepo;
            vendorRepository = vendorRepo;
            fiscalYearRepository = fiscalYearRepo;
            adjustmentTypeRepository = adjustmentTypeRepo;
            itemRepository = itemRepo;
            itemLedgerRepository = itemLedgerRepo;
            this.itemLedgerMasterRepository = itemLedgerMasterRepo;
            itemRequestRepository = itemRequestRepo;
        }
        #endregion Constructor

        public ItemRecordViewModel GetItemRecord(int? itemId, int fiscalYearId)
        {
            var model = new ItemRecordViewModel();
            if (itemId.HasValue)
            {
                var item = itemRepository.GetById(itemId);
                model.ItemId = item.Id;
                model.ItemName = item.Name;
                model.ItemCode = item.Code;
                model.ItemUnitId = (int)item.ItemUnitId;
                model.ItemUnitName = item.ItemUnit.Name;

                var data = new List<ItemRecordDetailViewModel>();

                //Opening Stock
                var OpeningStock = (from t1 in stockTransactionReporsitory.Table
                                    join t2 in stockTransanctionDetailRepository.Table on t1.Id equals t2.StockTransactionId
                                    where t1.DocumentSetupId == (int)eDocumentSetup.Opening && t1.FiscalYearId == fiscalYearId && t2.ItemId == itemId
                                    select new
                                    {
                                        StockTransactionId = t1.Id,
                                        FiscalYearId = t1.FiscalYearId,
                                        DocumentSetupId = t1.DocumentSetupId,
                                        DocumentNo = t1.DocumentNo,
                                        DisplayDocumentNo = (t1.DisplayDocumentNo != null) ? t1.DisplayDocumentNo : "",
                                        PurchaseOrderId = t1.PurchaseOrderId,
                                        PurchaseOrder = t1.PurchaseOrder,
                                        Date = t1.Date,
                                        //DateBS =DateMiti.GetDateMiti.GetMiti(t1.Date),
                                        Remarks = (t1.Remarks != null) ? t1.Remarks : "",
                                        StockTransactionDetailId = t2.Id,
                                        StockTransactionDetailId_StockTransactionId = t2.StockTransactionId,
                                        ItemId = t2.ItemId,
                                        ItemName = t2.Item.Name,
                                        ItemSubCode = t2.ItemSubCodeNo,
                                        Qty = t2.Qty,
                                        Rate = (t2.Rate != null) ? t2.Rate : 0,
                                        VatPerQty = (t2.Vat != null) ? t2.Vat : 0,
                                        BasicAmount = (t2.BasicAmount != null) ? t2.BasicAmount : 0,
                                        NetAmount = (t2.NetAmount != null) ? t2.NetAmount : 0,
                                        ItemDetailRemarks = t2.ItemSubCodeNo
                                    }).ToList();

                if (OpeningStock != null)
                {
                    foreach (var _openingstock in OpeningStock)
                    {
                        var storeEntryData = new ItemRecordDetailViewModel();
                        storeEntryData.Specification = _openingstock.ItemSubCode;
                        storeEntryData.ManufacturingCompany = "";
                        storeEntryData.ItemSize = "";
                        storeEntryData.EstimatedLifeSpan = "";
                        storeEntryData.PurchasingCompany = (_openingstock.PurchaseOrder == null) ? "" : _openingstock.PurchaseOrder.Vendor.Name;
                        storeEntryData.DisplayDocumentNo = _openingstock.DisplayDocumentNo;
                        storeEntryData.Date = _openingstock.Date;
                        //storeEntryData.DateBS = _openingstock.DateBS;
                        storeEntryData.EarningsQuantity = _openingstock.Qty;
                        storeEntryData.EarningsUnitPrice = (decimal)_openingstock.Rate;
                        storeEntryData.EarningsTotalPrice = (decimal)_openingstock.NetAmount;
                        storeEntryData.ExpenseQuantity = 0;
                        storeEntryData.ExpensesUnitPrice = 0;
                        storeEntryData.ExpensesTotalPrice = 0;
                        string remainingData = GetRemainingItemDetailByDate(_openingstock.ItemId, _openingstock.Date, fiscalYearId);
                        if (!string.IsNullOrEmpty(remainingData))
                        {
                            string[] value = remainingData.Split('-');
                            storeEntryData.RemainingQuantity = Convert.ToInt32(value[0]);
                            storeEntryData.RemainingUnitPrice = Convert.ToDecimal(value[1]);
                            storeEntryData.RemainingTotalPrice = Convert.ToDecimal(value[2]);
                        }
                        else
                        {
                            storeEntryData.RemainingQuantity = 0;
                            storeEntryData.RemainingUnitPrice = 0;
                            storeEntryData.RemainingTotalPrice = 0;
                        }
                        storeEntryData.Remarks = _openingstock.Remarks;
                        data.Add(storeEntryData);
                    }
                }
                //Opening Stock
                //StockEntry
                var stockEntry = (from t1 in stockTransactionReporsitory.Table
                                  join t2 in stockTransanctionDetailRepository.Table on t1.Id equals t2.StockTransactionId
                                  where t1.DocumentSetupId == (int)eDocumentSetup.StoreEntry && t1.FiscalYearId == fiscalYearId && t2.ItemId == itemId
                                  select new
                                  {
                                      StockTransactionId = t1.Id,
                                      FiscalYearId = t1.FiscalYearId,
                                      DocumentSetupId = t1.DocumentSetupId,
                                      DocumentNo = t1.DocumentNo,
                                      DisplayDocumentNo = (t1.DisplayDocumentNo != null) ? t1.DisplayDocumentNo : "",
                                      PurchaseOrderId = t1.PurchaseOrderId,
                                      PurchaseOrder = t1.PurchaseOrder,
                                      Date = t1.Date,
                                      //DateBS = DateMiti.GetDateMiti.GetMiti(t1.Date),
                                      Remarks = (t1.Remarks != null) ? t1.Remarks : "",
                                      StockTransactionDetailId = t2.Id,
                                      StockTransactionDetailId_StockTransactionId = t2.StockTransactionId,
                                      ItemId = t2.ItemId,
                                      ItemName = t2.Item.Name,
                                      SubCode = t2.ItemSubCodeNo,
                                      Qty = t2.Qty,
                                      Rate = (t2.Rate != null) ? t2.Rate : 0,
                                      VatPerQty = (t2.Vat != null) ? t2.Vat : 0,
                                      BasicAmount = (t2.BasicAmount != null) ? t2.BasicAmount : 0,
                                      NetAmount = (t2.NetAmount != null) ? t2.NetAmount : 0,
                                      ItemDetailRemarks = t2.ItemSubCodeNo
                                  }).ToList();

                if (stockEntry != null)
                {
                    foreach (var _storeentry in stockEntry)
                    {
                        var storeEntryData = new ItemRecordDetailViewModel();
                        storeEntryData.Specification = _storeentry.SubCode;
                        storeEntryData.ManufacturingCompany = "";
                        storeEntryData.ItemSize = "";
                        storeEntryData.EstimatedLifeSpan = "";
                        storeEntryData.PurchasingCompany = (_storeentry.PurchaseOrder == null) ? "" : _storeentry.PurchaseOrder.Vendor.Name;
                        storeEntryData.DisplayDocumentNo = _storeentry.DisplayDocumentNo;
                        storeEntryData.Date = _storeentry.Date;
                        //storeEntryData.DateBS = _storeentry.DateBS;
                        storeEntryData.EarningsQuantity = _storeentry.Qty;
                        storeEntryData.EarningsUnitPrice = (decimal)_storeentry.Rate;
                        storeEntryData.EarningsTotalPrice = (decimal)_storeentry.NetAmount;
                        storeEntryData.ExpenseQuantity = 0;
                        storeEntryData.ExpensesUnitPrice = 0;
                        storeEntryData.ExpensesTotalPrice = 0;
                        string remainingData = GetRemainingItemDetailByDate(_storeentry.ItemId, _storeentry.Date, fiscalYearId);
                        if (!string.IsNullOrEmpty(remainingData))
                        {
                            string[] value = remainingData.Split('-');
                            storeEntryData.RemainingQuantity = Convert.ToInt32(value[0]);
                            storeEntryData.RemainingUnitPrice = Convert.ToDecimal(value[1]);
                            storeEntryData.RemainingTotalPrice = Convert.ToDecimal(value[2]);
                        }
                        else
                        {
                            storeEntryData.RemainingQuantity = 0;
                            storeEntryData.RemainingUnitPrice = 0;
                            storeEntryData.RemainingTotalPrice = 0;
                        }
                        storeEntryData.Remarks = _storeentry.ItemDetailRemarks;
                        data.Add(storeEntryData);
                    }
                }
                //StockEntry
                //StockAdjustment
                var stockAdjustment = (from t1 in stockTransactionReporsitory.Table
                                       join t2 in stockTransanctionDetailRepository.Table on t1.Id equals t2.StockTransactionId
                                       where t1.DocumentSetupId == (int)eDocumentSetup.StockAdjustment && t1.FiscalYearId == fiscalYearId && t2.ItemId == itemId
                                       select new
                                       {
                                           StockTransactionId = t1.Id,
                                           FiscalYearId = t1.FiscalYearId,
                                           DocumentSetupId = t1.DocumentSetupId,
                                           DocumentNo = t1.DocumentNo,
                                           DisplayDocumentNo = (t1.DisplayDocumentNo != null) ? t1.DisplayDocumentNo : "",
                                           Date = t1.Date,
                                           //DateBS = DateMiti.GetDateMiti.GetMiti(t1.Date),
                                           AdjustmentInOutStatus = (t1.AdjustmentType.InOut != null) ? t1.AdjustmentType.InOut : "",
                                           Remarks = (t1.Remarks != null) ? t1.Remarks : "",
                                           StockTransactionDetailId = t2.Id,
                                           StockTransactionDetailId_StockTransactionId = t2.StockTransactionId,
                                           ItemId = t2.ItemId,
                                           ItemName = t2.Item.Name,
                                           Qty = t2.Qty,
                                           Rate = (t2.Rate != null) ? t2.Rate : 0,
                                           VatPerQty = (t2.Vat != null) ? t2.Vat : 0,
                                           BasicAmount = (t2.BasicAmount != null) ? t2.BasicAmount : 0,
                                           NetAmount = (t2.NetAmount != null) ? t2.NetAmount : 0,
                                           ItemDetailRemarks = t2.ItemSubCodeNo
                                       }).ToList();

                if (stockAdjustment != null)
                {
                    foreach (var _stockadjustment in stockAdjustment)
                    {
                        var stockAdjustmentData = new ItemRecordDetailViewModel();
                        stockAdjustmentData.Specification = "";
                        stockAdjustmentData.ManufacturingCompany = "";
                        stockAdjustmentData.ItemSize = "";
                        stockAdjustmentData.EstimatedLifeSpan = "";
                        stockAdjustmentData.PurchasingCompany = "";
                        stockAdjustmentData.DisplayDocumentNo = _stockadjustment.DisplayDocumentNo;
                        stockAdjustmentData.Date = _stockadjustment.Date;
                        //stockAdjustmentData.DateBS = _stockadjustment.DateBS;
                        if (!string.IsNullOrEmpty(_stockadjustment.AdjustmentInOutStatus))
                        {
                            if (_stockadjustment.AdjustmentInOutStatus == (eStockInOutType.In).ToString())
                            {
                                stockAdjustmentData.EarningsQuantity = _stockadjustment.Qty;
                                stockAdjustmentData.EarningsUnitPrice = (decimal)_stockadjustment.Rate;
                                stockAdjustmentData.EarningsTotalPrice = (decimal)_stockadjustment.NetAmount;
                                stockAdjustmentData.ExpenseQuantity = 0;
                                stockAdjustmentData.ExpensesUnitPrice = 0;
                                stockAdjustmentData.ExpensesTotalPrice = 0;
                            }
                            if (_stockadjustment.AdjustmentInOutStatus == (eStockInOutType.Out).ToString())
                            {
                                stockAdjustmentData.EarningsQuantity = 0;
                                stockAdjustmentData.EarningsUnitPrice = 0;
                                stockAdjustmentData.EarningsTotalPrice = 0;
                                stockAdjustmentData.ExpenseQuantity = _stockadjustment.Qty;
                                stockAdjustmentData.ExpensesUnitPrice = (decimal)_stockadjustment.Rate;
                                stockAdjustmentData.ExpensesTotalPrice = (decimal)_stockadjustment.NetAmount;
                            }
                        }
                        string remainingData = GetRemainingItemDetailByDate(_stockadjustment.ItemId, _stockadjustment.Date, fiscalYearId);
                        if (!string.IsNullOrEmpty(remainingData))
                        {
                            string[] value = remainingData.Split('-');
                            stockAdjustmentData.RemainingQuantity = Convert.ToInt32(value[0]);
                            stockAdjustmentData.RemainingUnitPrice = Convert.ToDecimal(value[1]);
                            stockAdjustmentData.RemainingTotalPrice = Convert.ToDecimal(value[2]);
                        }
                        else
                        {
                            stockAdjustmentData.RemainingQuantity = 0;
                            stockAdjustmentData.RemainingUnitPrice = 0;
                            stockAdjustmentData.RemainingTotalPrice = 0;
                        }
                        stockAdjustmentData.Remarks = _stockadjustment.ItemDetailRemarks;
                        data.Add(stockAdjustmentData);
                    }
                }
                //StockAdjustment
                //ItemRelease
                var itemRelease = (from t1 in itemReleaseRepository.Table
                                   join t2 in itemReleaseDetailRepository.Table on t1.Id equals t2.ItemReleaseId
                                   where t1.DocumentSetupId == (int)eDocumentSetup.ItemRelease && t1.FiscalYearId == fiscalYearId && t2.ItemId == itemId
                                   select new
                                   {
                                       ItemReleaseId = t1.Id,
                                       FiscalYearId = t1.FiscalYearId,
                                       DocumentSetupId = t1.DocumentSetupId,
                                       ReleaseNo = t1.ReleaseNo,
                                       DisplayDocumentNo = (t1.DisplayReleaseNo != null) ? t1.DisplayReleaseNo : "",
                                       Date = t1.Date,
                                       //DateBS = DateMiti.GetDateMiti.GetMiti(t1.Date),
                                       Remarks = t2.SubCode,
                                       ItemReleaseDetailId = t2.Id,
                                       ItemReleaseDetailId_ItemReleaseId = t2.ItemReleaseId,
                                       ItemId = t2.ItemId,
                                       ItemName = t2.Item.Name,
                                       ItemSubCode = t2.SubCode,
                                       Qty = t2.Qty,
                                       Rate = 0,
                                       VatPerQty = 0,
                                       BasicAmount = 0,
                                       OtherAmount = 0,
                                       NetAmount = 0,
                                       Narration = (t2.Narration != null) ? t2.Narration : "",
                                       ItemDetailRemarks = ""
                                   }).ToList();

                if (itemRelease != null)
                {
                    foreach (var _itemRelease in itemRelease)
                    {
                        var itemReleaseData = new ItemRecordDetailViewModel();
                        itemReleaseData.Specification = _itemRelease.ItemSubCode;
                        itemReleaseData.ManufacturingCompany = "";
                        itemReleaseData.ItemSize = "";
                        itemReleaseData.EstimatedLifeSpan = "";
                        itemReleaseData.PurchasingCompany = "";
                        itemReleaseData.DisplayDocumentNo = _itemRelease.DisplayDocumentNo;
                        itemReleaseData.Date = _itemRelease.Date;
                        //itemReleaseData.DateBS = _itemRelease.DateBS;
                        itemReleaseData.EarningsQuantity = 0;
                        itemReleaseData.EarningsUnitPrice = 0;
                        itemReleaseData.EarningsTotalPrice = 0;
                        itemReleaseData.ExpenseQuantity = _itemRelease.Qty;
                        itemReleaseData.ExpensesUnitPrice = (decimal)_itemRelease.Rate;
                        itemReleaseData.ExpensesTotalPrice = (decimal)_itemRelease.NetAmount;
                        string remainingData = GetRemainingItemDetailByDate(_itemRelease.ItemId, _itemRelease.Date, fiscalYearId);
                        if (!string.IsNullOrEmpty(remainingData))
                        {
                            string[] value = remainingData.Split('-');
                            itemReleaseData.RemainingQuantity = Convert.ToInt32(value[0]);
                            itemReleaseData.RemainingUnitPrice = Convert.ToDecimal(value[1]);
                            itemReleaseData.RemainingTotalPrice = Convert.ToDecimal(value[2]);
                        }
                        else
                        {
                            itemReleaseData.RemainingQuantity = 0;
                            itemReleaseData.RemainingUnitPrice = 0;
                            itemReleaseData.RemainingTotalPrice = 0;
                        }
                        itemReleaseData.Remarks = _itemRelease.ItemSubCode;
                        data.Add(itemReleaseData);
                    }
                }
                //ItemRelease
                model.Detail = data.OrderBy(x => x.Date).ToList();
            }
            return model;
        }

        public ItemRecordViewModel GetItemSubRecord(int? itemId, int fiscalYearId)
        {
            ItemRecordViewModel model = new ItemRecordViewModel();
            if (itemId.HasValue)
            {
                var item = itemRepository.GetById(itemId);
                model.ItemId = item.Id;
                model.ItemName = item.Name;
                model.ItemCode = item.Code;
                model.ItemUnitId = (int)item.ItemUnitId;
                model.ItemUnitName = item.ItemUnit.Name;

                var data = new List<ItemSubRecordDetailViewModel>();

                //Opening Stock
                var OpeningStock = (from t1 in stockTransactionReporsitory.Table
                                    join t2 in stockTransanctionDetailRepository.Table on t1.Id equals t2.StockTransactionId
                                    where t1.DocumentSetupId == (int)eDocumentSetup.Opening && t1.FiscalYearId == fiscalYearId && t2.ItemId == itemId
                                    select new
                                    {
                                        StockTransactionId = t1.Id,
                                        FiscalYearId = t1.FiscalYearId,
                                        DocumentSetupId = t1.DocumentSetupId,
                                        DocumentNo = t1.DocumentNo,
                                        DisplayDocumentNo = (t1.DisplayDocumentNo != null) ? t1.DisplayDocumentNo : "",
                                        PurchaseOrderId = t1.PurchaseOrderId,
                                        PurchaseOrder = t1.PurchaseOrder,
                                        Date = t1.Date,
                                        //DateBS = DateMiti.GetDateMiti.GetMiti(t1.Date),
                                        Remarks = (t1.Remarks != null) ? t1.Remarks : "",
                                        StockTransactionDetailId = t2.Id,
                                        StockTransactionDetailId_StockTransactionId = t2.StockTransactionId,
                                        ItemId = t2.ItemId,
                                        ItemName = t2.Item.Name,
                                        Qty = t2.Qty,
                                        Rate = (t2.Rate != null) ? t2.Rate : 0,
                                        VatPerQty = (t2.Vat != null) ? t2.Vat : 0,
                                        BasicAmount = (t2.BasicAmount != null) ? t2.BasicAmount : 0,
                                        NetAmount = (t2.NetAmount != null) ? t2.NetAmount : 0,
                                        ItemDetailRemarks = (t2.Remarks != null) ? t2.Remarks : "",
                                        EmployeeId = t2.EmployeeId,
                                        EmployeeName = t2.Employee.Name,
                                        SectionId = t2.SectionId,
                                        SectionName = t2.Section.Name
                                    }).ToList();

                if (OpeningStock != null)
                {
                    foreach (var _openingstock in OpeningStock)
                    {
                        var storeEntryData = new ItemSubRecordDetailViewModel();

                        storeEntryData.SectionId = _openingstock.SectionId;
                        storeEntryData.SectionName = _openingstock.SectionName;
                        storeEntryData.EmployeeId = _openingstock.EmployeeId;
                        storeEntryData.EmployeeName = _openingstock.EmployeeName;
                        storeEntryData.DisplayDocumentNo = _openingstock.DisplayDocumentNo;
                        storeEntryData.Date = _openingstock.Date;
                        //storeEntryData.DateBS = _openingstock.DateBS;
                        storeEntryData.EarningsQuantity = _openingstock.Qty;
                        storeEntryData.EarningsUnitPrice = (decimal)_openingstock.Rate;
                        storeEntryData.EarningsTotalPrice = (decimal)_openingstock.NetAmount;
                        storeEntryData.ExpenseQuantity = 0;
                        storeEntryData.ExpensesUnitPrice = 0;
                        storeEntryData.ExpensesTotalPrice = 0;
                        string remainingData = GetRemainingItemDetailByDate(_openingstock.ItemId, _openingstock.Date, fiscalYearId);
                        if (!string.IsNullOrEmpty(remainingData))
                        {
                            string[] value = remainingData.Split('-');
                            storeEntryData.RemainingQuantity = Convert.ToInt32(value[0]);
                        }
                        storeEntryData.Narration = _openingstock.ItemDetailRemarks;
                        data.Add(storeEntryData);
                    }
                }
                //Opening Stock

                //StockEntry
                var stockEntry = (from t1 in stockTransactionReporsitory.Table
                                  join t2 in stockTransanctionDetailRepository.Table on t1.Id equals t2.StockTransactionId
                                  where t1.DocumentSetupId == (int)eDocumentSetup.StoreEntry && t1.FiscalYearId == fiscalYearId && t2.ItemId == itemId
                                  select new
                                  {
                                      StockTransactionId = t1.Id,
                                      FiscalYearId = t1.FiscalYearId,
                                      DocumentSetupId = t1.DocumentSetupId,
                                      DocumentNo = t1.DocumentNo,
                                      DisplayDocumentNo = (t1.DisplayDocumentNo != null) ? t1.DisplayDocumentNo : "",
                                      PurchaseOrderId = t1.PurchaseOrderId,
                                      PurchaseOrder = t1.PurchaseOrder,
                                      Date = t1.Date,
                                      //DateBS = DateMiti.GetDateMiti.GetMiti(t1.Date),
                                      Remarks = (t1.Remarks != null) ? t1.Remarks : "",
                                      StockTransactionDetailId = t2.Id,
                                      StockTransactionDetailId_StockTransactionId = t2.StockTransactionId,
                                      ItemId = t2.ItemId,
                                      ItemName = t2.Item.Name,
                                      Qty = t2.Qty,
                                      Rate = (t2.Rate != null) ? t2.Rate : 0,
                                      VatPerQty = (t2.Vat != null) ? t2.Vat : 0,
                                      BasicAmount = (t2.BasicAmount != null) ? t2.BasicAmount : 0,
                                      NetAmount = (t2.NetAmount != null) ? t2.NetAmount : 0,
                                      ItemDetailRemarks = (t2.Remarks != null) ? t2.Remarks : "",
                                      EmployeeId = t2.EmployeeId,
                                      EmployeeName = t2.Employee.Name,
                                      SectionId = t2.SectionId,
                                      SectionName = t2.Section.Name
                                  }).ToList();

                if (stockEntry != null)
                {
                    foreach (var _storeentry in stockEntry)
                    {
                        var storeEntryData = new ItemSubRecordDetailViewModel();
                        storeEntryData.SectionId = _storeentry.SectionId;
                        storeEntryData.SectionName = _storeentry.SectionName;
                        storeEntryData.EmployeeId = _storeentry.EmployeeId;
                        storeEntryData.EmployeeName = _storeentry.EmployeeName;
                        storeEntryData.DisplayDocumentNo = _storeentry.DisplayDocumentNo;
                        storeEntryData.Date = _storeentry.Date;
                        //storeEntryData.DateBS = _storeentry.DateBS;
                        storeEntryData.EarningsQuantity = _storeentry.Qty;
                        storeEntryData.EarningsUnitPrice = (decimal)_storeentry.Rate;
                        storeEntryData.EarningsTotalPrice = (decimal)_storeentry.NetAmount;
                        storeEntryData.ExpenseQuantity = 0;
                        storeEntryData.ExpensesUnitPrice = 0;
                        storeEntryData.ExpensesTotalPrice = 0;
                        string remainingData = GetRemainingItemDetailByDate(_storeentry.ItemId, _storeentry.Date, fiscalYearId);
                        if (!string.IsNullOrEmpty(remainingData))
                        {
                            string[] value = remainingData.Split('-');
                            storeEntryData.RemainingQuantity = Convert.ToInt32(value[0]);
                        }
                        storeEntryData.Narration = _storeentry.ItemDetailRemarks;
                        data.Add(storeEntryData);
                    }
                }
                //StockEntry
                //StockAdjustment
                var stockAdjustment = (from t1 in stockTransactionReporsitory.Table
                                       join t2 in stockTransanctionDetailRepository.Table on t1.Id equals t2.StockTransactionId
                                       where t1.DocumentSetupId == (int)eDocumentSetup.StockAdjustment && t1.FiscalYearId == fiscalYearId && t2.ItemId == itemId
                                       select new
                                       {
                                           StockTransactionId = t1.Id,
                                           FiscalYearId = t1.FiscalYearId,
                                           DocumentSetupId = t1.DocumentSetupId,
                                           DocumentNo = t1.DocumentNo,
                                           DisplayDocumentNo = (t1.DisplayDocumentNo != null) ? t1.DisplayDocumentNo : "",
                                           Date = t1.Date,
                                           //DateBS = DateMiti.GetDateMiti.GetMiti(t1.Date),
                                           AdjustmentInOutStatus = (t1.AdjustmentType.InOut != null) ? t1.AdjustmentType.InOut : "",
                                           Remarks = (t1.Remarks != null) ? t1.Remarks : "",
                                           StockTransactionDetailId = t2.Id,
                                           StockTransactionDetailId_StockTransactionId = t2.StockTransactionId,
                                           ItemId = t2.ItemId,
                                           ItemName = t2.Item.Name,
                                           Qty = t2.Qty,
                                           Rate = (t2.Rate != null) ? t2.Rate : 0,
                                           VatPerQty = (t2.Vat != null) ? t2.Vat : 0,
                                           BasicAmount = (t2.BasicAmount != null) ? t2.BasicAmount : 0,
                                           NetAmount = (t2.NetAmount != null) ? t2.NetAmount : 0,
                                           ItemDetailRemarks = (t2.Remarks != null) ? t2.Remarks : "",
                                           EmployeeId = t2.EmployeeId,
                                           EmployeeName = t2.Employee.Name,
                                           SectionId = t2.SectionId,
                                           SectionName = t2.Section.Name
                                       }).ToList();

                if (stockAdjustment != null)
                {
                    foreach (var _stockadjustment in stockAdjustment)
                    {
                        var stockAdjustmentData = new ItemSubRecordDetailViewModel();
                        stockAdjustmentData.SectionId = _stockadjustment.SectionId;
                        stockAdjustmentData.SectionName = _stockadjustment.SectionName;
                        stockAdjustmentData.EmployeeId = _stockadjustment.EmployeeId;
                        stockAdjustmentData.EmployeeName = _stockadjustment.EmployeeName;
                        stockAdjustmentData.DisplayDocumentNo = _stockadjustment.DisplayDocumentNo;
                        stockAdjustmentData.Date = _stockadjustment.Date;
                        //stockAdjustmentData.DateBS = _stockadjustment.DateBS;
                        if (!string.IsNullOrEmpty(_stockadjustment.AdjustmentInOutStatus))
                        {
                            if (_stockadjustment.AdjustmentInOutStatus == (eStockInOutType.In).ToString())
                            {
                                stockAdjustmentData.EarningsQuantity = _stockadjustment.Qty;
                                stockAdjustmentData.EarningsUnitPrice = (decimal)_stockadjustment.Rate;
                                stockAdjustmentData.EarningsTotalPrice = (decimal)_stockadjustment.NetAmount;
                                stockAdjustmentData.ExpenseQuantity = 0;
                                stockAdjustmentData.ExpensesUnitPrice = 0;
                                stockAdjustmentData.ExpensesTotalPrice = 0;
                            }
                            if (_stockadjustment.AdjustmentInOutStatus == (eStockInOutType.Out).ToString())
                            {
                                stockAdjustmentData.EarningsQuantity = 0;
                                stockAdjustmentData.EarningsUnitPrice = 0;
                                stockAdjustmentData.EarningsTotalPrice = 0;
                                stockAdjustmentData.ExpenseQuantity = _stockadjustment.Qty;
                                stockAdjustmentData.ExpensesUnitPrice = (decimal)_stockadjustment.Rate;
                                stockAdjustmentData.ExpensesTotalPrice = (decimal)_stockadjustment.NetAmount;
                            }
                        }
                        string remainingData = GetRemainingItemDetailByDate(_stockadjustment.ItemId, _stockadjustment.Date, fiscalYearId);
                        if (!string.IsNullOrEmpty(remainingData))
                        {
                            string[] value = remainingData.Split('-');
                            stockAdjustmentData.RemainingQuantity = Convert.ToInt32(value[0]);
                        }
                        stockAdjustmentData.Narration = _stockadjustment.ItemDetailRemarks;
                        data.Add(stockAdjustmentData);
                    }
                }
                //StockAdjustment
                //ItemRelease
                var itemRelease = (from t1 in itemReleaseRepository.Table
                                   join t2 in itemReleaseDetailRepository.Table on t1.Id equals t2.ItemReleaseId
                                   where t1.DocumentSetupId == (int)eDocumentSetup.ItemRelease && t1.FiscalYearId == fiscalYearId && t2.ItemId == itemId
                                   select new
                                   {
                                       ItemReleaseId = t1.Id,
                                       FiscalYearId = t1.FiscalYearId,
                                       DocumentSetupId = t1.DocumentSetupId,
                                       ReleaseNo = t1.ReleaseNo,
                                       DisplayDocumentNo = (t1.DisplayReleaseNo != null) ? t1.DisplayReleaseNo : "",
                                       Date = t1.Date,
                                       //DateBS = DateMiti.GetDateMiti.GetMiti(t1.Date),
                                       Remarks = (t1.Remarks != null) ? t1.Remarks : "",
                                       ItemReleaseDetailId = t2.Id,
                                       ItemReleaseDetailId_ItemReleaseId = t2.ItemReleaseId,
                                       ItemId = t2.ItemId,
                                       ItemName = t2.Item.Name,
                                       Qty = t2.Qty,
                                       Rate = 0,
                                       VatPerQty = 0,
                                       BasicAmount = 0,
                                       OtherAmount = 0,
                                       NetAmount = 0,
                                       Narration = (t2.Narration != null) ? t2.Narration : "",
                                       ItemDetailRemarks = "",
                                       EmployeeId = t1.Employee.Id,
                                       EmployeeName = t1.Employee.Name,
                                       SectionId = t1.SectionId,
                                       SectionName = t1.Section.Name
                                   }).ToList();

                if (itemRelease != null)
                {
                    foreach (var _itemRelease in itemRelease)
                    {
                        var itemReleaseData = new ItemSubRecordDetailViewModel();
                        itemReleaseData.SectionId = _itemRelease.SectionId;
                        itemReleaseData.SectionName = _itemRelease.SectionName;
                        itemReleaseData.EmployeeId = _itemRelease.EmployeeId;
                        itemReleaseData.EmployeeName = _itemRelease.EmployeeName;
                        itemReleaseData.DisplayDocumentNo = _itemRelease.DisplayDocumentNo;
                        itemReleaseData.Date = _itemRelease.Date;
                        //itemReleaseData.DateBS = _itemRelease.DateBS;
                        itemReleaseData.EarningsQuantity = 0;
                        itemReleaseData.EarningsUnitPrice = 0;
                        itemReleaseData.EarningsTotalPrice = 0;
                        itemReleaseData.ExpenseQuantity = _itemRelease.Qty;
                        itemReleaseData.ExpensesUnitPrice = (decimal)_itemRelease.Rate;
                        itemReleaseData.ExpensesTotalPrice = (decimal)_itemRelease.NetAmount;
                        itemReleaseData.Narration = _itemRelease.Narration;
                        string remainingData = GetRemainingItemDetailByDate(_itemRelease.ItemId, _itemRelease.Date, fiscalYearId);
                        if (!string.IsNullOrEmpty(remainingData))
                        {
                            string[] value = remainingData.Split('-');
                            itemReleaseData.RemainingQuantity = Convert.ToInt32(value[0]);
                        }
                        data.Add(itemReleaseData);
                    }
                }
                //ItemRelease
                model.SubDetail = data.OrderBy(x => x.Date).ToList();
            }
            return model;
        }

        public string GetRemainingItemDetailByDate(int itemId, DateTime date, int fiscalYearId)
        {
            string result = "";
            var data = new List<ItemRecordDetailViewModel>();
            //Stock Entry & Opening Stock
            var stockEntry = (from t1 in stockTransactionReporsitory.Table
                              join t2 in stockTransanctionDetailRepository.Table on t1.Id equals t2.StockTransactionId
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
            var stockAdjustment = (from t1 in stockTransactionReporsitory.Table
                                   join t2 in stockTransanctionDetailRepository.Table on t1.Id equals t2.StockTransactionId
                                   where t1.DocumentSetupId == (int)eDocumentSetup.StockAdjustment && t1.FiscalYearId == fiscalYearId && t1.Date <= date && t2.ItemId == itemId
                                   select new
                                   {
                                       Date = t1.Date,
                                       //Miti = GetMiti(t1.Date),
                                       AdjustmentInOutStatus = (t1.AdjustmentType.InOut != null) ? t1.AdjustmentType.InOut : "",
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
            if (stockAdjustment != null)
            {
                foreach (var _stockadjustment in stockAdjustment)
                {
                    var stockAdjustmentData = new ItemRecordDetailViewModel();
                    stockAdjustmentData.Date = _stockadjustment.Date;
                    stockAdjustmentData.DateBS = _stockadjustment.Date.GetMiti();
                    if (!string.IsNullOrEmpty(_stockadjustment.AdjustmentInOutStatus))
                    {
                        if (_stockadjustment.AdjustmentInOutStatus == (eStockInOutType.In).ToString())
                        {
                            stockAdjustmentData.EarningsQuantity = _stockadjustment.Qty;
                            stockAdjustmentData.EarningsUnitPrice = (decimal)_stockadjustment.Rate;
                            stockAdjustmentData.EarningsTotalPrice = (decimal)_stockadjustment.NetAmount;
                            stockAdjustmentData.ExpenseQuantity = 0;
                            stockAdjustmentData.ExpensesUnitPrice = 0;
                            stockAdjustmentData.ExpensesTotalPrice = 0;
                        }
                        if (_stockadjustment.AdjustmentInOutStatus == (eStockInOutType.Out).ToString())
                        {
                            stockAdjustmentData.EarningsQuantity = 0;
                            stockAdjustmentData.EarningsUnitPrice = 0;
                            stockAdjustmentData.EarningsTotalPrice = 0;
                            stockAdjustmentData.ExpenseQuantity = _stockadjustment.Qty;
                            stockAdjustmentData.ExpensesUnitPrice = (decimal)_stockadjustment.Rate;
                            stockAdjustmentData.ExpensesTotalPrice = (decimal)_stockadjustment.NetAmount;
                        }
                    }
                    data.Add(stockAdjustmentData);
                }
            }
            //Item Release
            var itemRelease = (from t1 in itemReleaseRepository.Table
                               join t2 in itemReleaseDetailRepository.Table on t1.Id equals t2.ItemReleaseId
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

        public List<ItemLedgerViewModel> ItemLedgerDetail(string fiscalYear, string itemCode)
        {
            var data = itemLedgerRepository.Table.Where(x => x.ItemLedgerMaster.Code == itemCode);
            var model = new List<ItemLedgerViewModel>();
            if (string.IsNullOrEmpty(fiscalYear))
                return model;

            foreach (var item in data)
            {
                var m = new ItemLedgerViewModel();
                m.DataId = item.DataId;
                m.ItemName = item.ItemLedgerMaster.Name;
                m.CategoryName = item.ItemLedgerMaster.Category;
                m.Code = item.ItemLedgerMaster.Code;
                m.SubCode = item.SubCode;
                m.PurchasedDate = item.PurchasedDate;
                m.Description = item.Description;
                m.Qty = item.Qty;
                m.Rate = item.Rate;
                m.PurchaseAmt = item.PurchaseAmt;
                m.Unit = item.ItemLedgerMaster.Unit;
                switch (fiscalYear)
                {
                    case "2054/55":
                        m.Dep = item.Dep_5455;
                        m.Sale = item.Sale_5455;
                        m.Rem = item.Rem_5455;
                        break;
                    case "2055/56":
                        m.Dep = item.Dep_5556;
                        m.Sale = item.Sale_5556;
                        m.Rem = item.Rem_5556;
                        break;
                    case "2056/57":
                        m.Dep = item.Dep_5657;
                        m.Sale = item.Sale_5657;
                        m.Rem = item.Rem_5657;
                        break;
                    case "2057/58":
                        m.Dep = item.Dep_5758;
                        m.Sale = item.Sale_5758;
                        m.Rem = item.Rem_5758;
                        break;
                    case "2058/59":
                        m.Dep = item.Dep_5859;
                        m.Sale = item.Sale_5859;
                        m.Rem = item.Rem_5859;
                        break;
                    case "2059/60":
                        m.Dep = item.Dep_5960;
                        m.Sale = item.Sale_5960;
                        m.Rem = item.Rem_5960;
                        break;
                    case "2060/61":
                        m.Dep = item.Dep_6061;
                        m.Sale = item.Sale_6061;
                        m.Rem = item.Rem_6061;
                        break;
                    case "2061/62":
                        m.Dep = item.Dep_6162;
                        m.Sale = item.Sale_6162;
                        m.Rem = item.Rem_6162;
                        break;
                    case "2062/63":
                        m.Dep = item.Dep_6263;
                        m.Sale = item.Sale_6263;
                        m.Rem = item.Rem_6263;
                        break;
                    case "2063/64":
                        m.Dep = item.Dep_6364;
                        m.Sale = item.Sale_6364;
                        m.Rem = item.Rem_6364;
                        break;
                    case "2064/65":
                        m.Dep = item.Dep_6465;
                        m.Sale = item.Sale_6465;
                        m.Rem = item.Rem_6465;
                        break;
                    case "2065/66":
                        m.Dep = item.Dep_6566;
                        m.Sale = item.Sale_6566;
                        m.Rem = item.Rem_6566;
                        break;
                    case "2066/67":
                        m.Dep = item.Dep_6667;
                        m.Sale = item.Sale_6667;
                        m.Rem = item.Rem_6667;
                        break;
                    case "2067/68":
                        m.Dep = item.Dep_6768;
                        m.Sale = item.Sale_6768;
                        m.Rem = item.Rem_6768;
                        break;
                    case "2068/69":
                        m.Dep = item.Dep_6869;
                        m.Sale = item.Sale_6869;
                        m.Rem = item.Rem_6869;
                        break;
                    case "2069/70":
                        m.Dep = item.Dep_6970;
                        m.Sale = item.Sale_6970;
                        m.Rem = item.Rem_6970;
                        break;
                    case "2070/71":
                        m.Dep = item.Dep_7071;
                        m.Sale = item.Sale_7071;
                        m.Rem = item.Rem_7071;
                        break;
                    case "2071/72":
                        m.Dep = item.Dep_7172;
                        m.Sale = item.Sale_7172;
                        m.Rem = item.Rem_7172;
                        break;
                    case "2072/73":
                        m.Dep = item.Dep_7273;
                        m.Sale = item.Sale_7273;
                        m.Rem = item.Rem_7273;
                        break;
                }
                model.Add(m);
            }

            return model;
        }

        public SelectList ItemCategories()
        {
            var data = itemLedgerMasterRepository.Table.Select(x => new { Category = x.Category }).Distinct().ToList();
            return new SelectList(data, "Category", "Category", null);
        }

        public SelectList Items(string category)
        {
            var data = itemLedgerMasterRepository.Table.Where(x => x.Category == category).Select(x => new { Name = x.Name, Code = x.Code }).Distinct();
            return new SelectList(data, "Code", "Name");
        }

        public string GetItem(string itemCode)
        {
            return itemLedgerMasterRepository.Table.Where(x => x.Code == itemCode).FirstOrDefault().Name;
        }

        public List<DashboardItemRequestViewModel> GetPendingItemRequests(int employeeId)
        {
            var list = itemRequestRepository.Table.Where(x => x.EmployeeId == employeeId && !x.ItemReceivedBy.HasValue).OrderByDescending(x => x.Date).Take(20);
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
    }
}
