using IMS.Logic.ViewModels.BackOffice.IMS;
using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.Configuration;
using IMS.Logic.Contract;
using IMS.Logic.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using IMS.Logic.Utilities;
using System.Text;
using System.Threading.Tasks;
using static IMS.Data.NTAEnum;
using static DateMiti.GetDateMiti;
using PagedList;

namespace IMS.Logic.Implementation
{
    public class ImsEntryService : IImsEntryService
    {
        #region Constructor
        private ILocalizationService localizationService;
        private ICommonService commonService;
        private IImsReportService reportService;
        private IExceptionService exceptionService;
        private INotificationService notificationService;
        private IWorkFlowService workflowService;

        private IRepository<StockTransaction> stockTransactionReporsitory;
        private IRepository<StockTransactionDetail> stockTransanctionDetailRepository;

        private IRepository<PurchaseOrder> purchaseOrderRepository;
        private IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository;

        private IRepository<ItemRequest> itemRequestRepository;
        private IRepository<ItemRequestDetail> itemRequestDetailRepository;

        private IRepository<ItemRelease> itemReleaseRepository;
        private IRepository<ItemReleaseDetail> itemReleaseDetailRepository;

        private IRepository<Vendor> vendorRepository;
        private IRepository<FiscalYear> fiscalYearRepository;
        private IRepository<AdjustmentType> adjustmentTypeRepository;
        private IRepository<Employee> employeeRepository;
        private UnitOfWork db;

        public ImsEntryService(
            ILocalizationService localizationSvc,
            ICommonService commonSVC,
            IImsReportService reportSVC,
            IExceptionService exceptionSVC,
            INotificationService notificationSVC,
            IWorkFlowService workflowSVC,

            IRepository<StockTransaction> stockTransactionRepo,
            IRepository<StockTransactionDetail> stockTransactionDetailRepo,
            IRepository<PurchaseOrder> purchaseOrderRepo,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepo,
            IRepository<ItemRequest> itemRequestRepo,
            IRepository<ItemRequestDetail> itemRequestDetailRepo,
            IRepository<ItemRelease> itemReleaseRepo,
            IRepository<ItemReleaseDetail> itemReleaseDetailRepo,
            IRepository<Vendor> vendorRepo,
            IRepository<FiscalYear> fiscalYearRepo,
            IRepository<AdjustmentType> adjustmentTypeRepo,
            IRepository<Employee> employeeRepo
            )
        {
            localizationService = localizationSvc;
            exceptionService = exceptionSVC;
            reportService = reportSVC;
            this.notificationService = notificationSVC;
            workflowService = workflowSVC;
            db = new UnitOfWork();

            stockTransactionReporsitory = stockTransactionRepo;
            stockTransanctionDetailRepository = stockTransactionDetailRepo;
            purchaseOrderRepository = purchaseOrderRepo;
            purchaseOrderDetailRepository = purchaseOrderDetailRepo;
            itemRequestRepository = itemRequestRepo;
            itemRequestDetailRepository = itemRequestDetailRepo;
            itemReleaseRepository = itemReleaseRepo;
            itemReleaseDetailRepository = itemReleaseDetailRepo;
            vendorRepository = vendorRepo;
            fiscalYearRepository = fiscalYearRepo;
            adjustmentTypeRepository = adjustmentTypeRepo;
            commonService = commonSVC;
            employeeRepository = employeeRepo;
        }
        #endregion Constructor

        #region Common

        public ServiceModel DeleteStockTransaction(int stockTransactionId)
        {
            var result = new ServiceModel();

            var data = stockTransactionReporsitory.GetById(stockTransactionId);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                stockTransactionReporsitory.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion

        #region Opening Stock
        public OpeningStockViewModel GetOpeningStock(int openingStockId)
        {
            var data = stockTransactionReporsitory.GetById(openingStockId);
            var model = AutomapperConfig.Mapper.Map<OpeningStockViewModel>(data);
            if (model == null)
                return new OpeningStockViewModel();

            foreach (var item in data.StockTransactionDetails)
            {
                model.Details.Add(AutomapperConfig.Mapper.Map<OpeningStockDetailViewModel>(item));
            }
            return model;
        }

        public IList<OpeningStockListViewModel> GetOpeningStocks(int fiscalYearId, string displayDocumentNo, DateTime? dateFrom, DateTime? dateTo, int? sectionId, int? employeeId)
        {
            var data = stockTransactionReporsitory.Table.Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.Opening
                                && (string.IsNullOrEmpty(displayDocumentNo) || x.DisplayDocumentNo == displayDocumentNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!dateTo.HasValue || x.Date <= dateTo)
                                && (!sectionId.HasValue || x.StockTransactionDetails.Any(y => y.SectionId == sectionId))
                                && (!employeeId.HasValue || x.StockTransactionDetails.Any(y => y.EmployeeId == employeeId))).OrderBy(x => x.Date);
            var model = AutomapperConfig.Mapper.Map<IList<OpeningStockListViewModel>>(data);
            return model;
        }

        public OpeningStockViewModel SaveOpeningStock(OpeningStockViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (model.Details.Count == 0)
                {
                    model.Errors.Add(new ValidationResult("Please enter detail.", new string[] { "" }));
                }

                if (model.Errors.Any())
                    return;

                if (!model.Validate())
                    return;

                var entity = new StockTransaction();

                if (model.Id > 0)
                {
                    entity = stockTransactionReporsitory.GetById(model.Id);
                    entity.Date = model.DateBS.GetDate();
                    entity.Remarks = model.Remarks;
                    stockTransanctionDetailRepository.DeleteRange(entity.StockTransactionDetails);
                }
                else
                {
                    model.DocumentSetupId = (int)eDocumentSetup.Opening;
                    model.Date = model.DateBS.GetDate();
                    entity = AutomapperConfig.Mapper.Map<StockTransaction>(model);
                    stockTransactionReporsitory.Insert(entity);
                }

                foreach (var item in model.Details)
                {
                    var detail = AutomapperConfig.Mapper.Map<StockTransactionDetail>(item);
                    detail.StockTransactionId = entity.Id;
                    stockTransanctionDetailRepository.Insert(detail);
                }

                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Data saved successfully.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Save changes successfully.");
            }, model);

            return model;
        }
        #endregion Opening Stock

        #region Item Request
        public ItemRequestItemStatusViewModel GetItemRequestItemStatus(int itemRequestId, DateTime date, int fiscalYearId)
        {
            var model = new ItemRequestItemStatusViewModel();
            var data = itemRequestRepository.GetById(itemRequestId);
            if (data != null && data.ItemRequestDetails != null)
            {
                model.Remarks = data.Remarks;
                foreach (var item in data.ItemRequestDetails)
                {
                    var _item = new ItemRequestItemStatusListViewModel();
                    var _data = commonService.GetItemStatusByItemId(item.ItemId, date, fiscalYearId);
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

        public IList<ItemRequestViewModel> GetItemRequests(int fiscalYearId, int employeeId, string displayRequestNo, DateTime? dateFrom, DateTime? dateTo, bool unProcessedOnly = false, bool unReleasedOnly = false, bool acceptedOnly = false, string itemRequestIds = "")
        {
            var data = itemRequestRepository.Table.Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.ItemRequest
                                && (employeeId == 0 || x.EmployeeId == employeeId)
                                && (string.IsNullOrEmpty(displayRequestNo) || x.DisplayRequestNo == displayRequestNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!dateTo.HasValue || x.Date <= dateTo)
                                && (unProcessedOnly || !acceptedOnly || x.AcceptedBy.HasValue)
                                && (!unProcessedOnly || (unProcessedOnly && x.ItemRequestDetails.Any(x1 => !x1.PurchaseOrderId.HasValue)))
                                && (!unReleasedOnly || (unReleasedOnly && x.ItemRequestDetails.Any(x1 => !x1.ItemReleaseId.HasValue)))).OrderByDescending(x => x.Date).ThenByDescending(x => x.DisplayRequestNo);

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
                        var data1 = itemRequestRepository.GetById(requestId);
                        var selectedRequest = AutomapperConfig.Mapper.Map<ItemRequestViewModel>(data1);
                        model.Add(selectedRequest);
                    }
                }
            }
            var modelResult = AutomapperConfig.Mapper.Map<IList<ItemRequestViewModel>>(data);
            return modelResult;
        }

        public ItemRequestViewModel SaveItemRequest(ItemRequestViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                model.DocumentSetupId = (int)eDocumentSetup.ItemRequest;

                if (model.Details.Count == 0)
                {
                    model.Errors.Add(new ValidationResult("Please enter detail.", new string[] { "" }));
                }

                if (model.Errors.Any())
                    return;

                if (!model.Validate())
                    return;

                model.Date = model.DateBS.GetDate();

                var entity = new ItemRequest();

                if (model.Id > 0)
                {
                    entity = itemRequestRepository.GetById(model.Id);
                    entity.Date = model.Date;
                    entity.Remarks = model.Remarks;
                    itemRequestDetailRepository.DeleteRange(entity.ItemRequestDetails);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<ItemRequest>(model);
                    entity.ApplicationStatus = (int)eApplicationStatus.Applied;
                    itemRequestRepository.Insert(entity);
                }

                foreach (var item in model.Details)
                {
                    var detail = AutomapperConfig.Mapper.Map<ItemRequestDetail>(item);
                    detail.ItemRequestId = entity.Id;
                    itemRequestDetailRepository.Insert(detail);
                }

                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Data saved successfully.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Save changes successfully.");

                var receiver = employeeRepository.GetById(model.RequestedEmployeeId);
                if (receiver != null)
                {
                    //intialize workflow
                    var workflowResponse = workflowService.InitiliazeWorkflow(NTAEnum.eWorkFlowType.ItemRequest, entity.Id, model.EmployeeId, model.RequestedEmployeeId, new Dictionary<string, string>());
                    if (workflowResponse.Status != ViewModels.BackOffice.Workflow.eWorkflowReponseStatus.Success)
                    {
                        model.Errors.Add(new ValidationResult("An error has occured. " + workflowResponse.Message));
                        var itemReqDetail = itemRequestDetailRepository.Table.Where(x => x.ItemRequestId == entity.Id);
                        itemRequestDetailRepository.DeleteRange(itemReqDetail);
                        itemRequestRepository.Delete(entity.Id);
                        return;
                    }
                }

            }, model);
            return model;
        }

        public ItemRequestViewModel GetItemRequest(int itemRequestId)
        {
            var data = itemRequestRepository.GetById(itemRequestId);
            var model = AutomapperConfig.Mapper.Map<ItemRequestViewModel>(data);

            if (model == null)
                return new ItemRequestViewModel();

            model.FiscalYearDetail = (model.FiscalYearId > 0) ? fiscalYearRepository.GetById(model.FiscalYearId) : new FiscalYear();
            return model;
        }

        public ServiceModel DeleteItemRequest(int itemRequestId, int deletedBy)
        {
            var result = new ServiceModel();
            var data = itemRequestRepository.GetById(itemRequestId);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                itemRequestRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Item Request

        #region Purchase Order
        public PurchaseOrderViewModel SavePurchaseOrder(PurchaseOrderViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                model.DocumentSetupId = (int)eDocumentSetup.PurchaseOrder;

                if (model.Details.Count == 0)
                {
                    model.Errors.Add(new ValidationResult("Please enter detail.", new string[] { "" }));
                }

                if (model.Errors.Any())
                    return;

                if (!model.Validate())
                    return;

                var entity = new PurchaseOrder();

                if (model.Id > 0)
                {
                    entity = purchaseOrderRepository.GetById(model.Id);
                    entity.Date = model.DateBS.GetDate();
                    entity.DueDate = string.IsNullOrEmpty(model.DueDateBS) ? (DateTime?)null : model.DueDateBS.GetDate();
                    entity.Remarks = model.Remarks;
                    purchaseOrderDetailRepository.DeleteRange(entity.PurchaseOrderDetails);
                }
                else
                {
                    model.Date = model.DateBS.GetDate();
                    model.DueDate = string.IsNullOrEmpty(model.DueDateBS) ? (DateTime?)null : model.DueDateBS.GetDate();
                    entity = AutomapperConfig.Mapper.Map<PurchaseOrder>(model);
                    purchaseOrderRepository.Insert(entity);
                }

                List<int> selectedItemRequestId = new List<int>();
                foreach (var item in model.Details)
                {
                    var detail = AutomapperConfig.Mapper.Map<PurchaseOrderDetail>(item);
                    detail.PurchaseOrderId = entity.Id;
                    purchaseOrderDetailRepository.Insert(detail);

                    if (!string.IsNullOrEmpty(item.ItemRequestDetailIdCSV))
                    {
                        selectedItemRequestId.AddRange(item.ItemRequestDetailIdCSV.Split(',').Select(x => Convert.ToInt32(x)));
                    }
                }

                var selectedItemRequest = itemRequestDetailRepository.Table.Where(x => selectedItemRequestId.Contains(x.Id));
                foreach (var req in selectedItemRequest)
                {
                    if (purchaseOrderDetailRepository.Table.Any(x => x.PurchaseOrderId == entity.Id && x.ItemId == req.ItemId))
                        req.PurchaseOrderId = entity.Id;
                    else
                        req.PurchaseOrderId = null;
                }
                itemRequestRepository.CommitChanges();

                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Data saved successfully.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Save changes successfully.");

                var receiver = employeeRepository.GetById(model.AcceptedBy);
                if (receiver != null)
                {
                    //intialize workflow
                    var workflowResponse = workflowService.InitiliazeWorkflow(NTAEnum.eWorkFlowType.PurchaseOrder, entity.Id, model.EmployeeId, model.AcceptedBy, new Dictionary<string, string>());
                    if (workflowResponse.Status != ViewModels.BackOffice.Workflow.eWorkflowReponseStatus.Success)
                    {
                        model.Errors.Add(new ValidationResult("An error has occured, Please try again " + workflowResponse.Message));
                        var detail = db.PurchaseOrderDetailRepo.Table.Where(x => x.PurchaseOrderId == entity.Id);
                        db.PurchaseOrderDetailRepo.DeleteRange(detail);
                        db.PurchaseOrderRepo.Delete(entity.Id);
                        return;
                    }
                }

            }, model);
            return model;
        }

        public PurchaseOrderViewModel GetPurchaseOrder(int purchaseOrderId)
        {
            var data = purchaseOrderRepository.GetById(purchaseOrderId);
            var model = AutomapperConfig.Mapper.Map<PurchaseOrderViewModel>(data);
            if (model == null)
                return new PurchaseOrderViewModel();
            var vendor = (model.VendorId > 0) ? vendorRepository.GetById(model.VendorId) : new Vendor();
            model.VendorDetails = AutomapperConfig.Mapper.Map<VendorViewModel>(vendor);

            foreach (var item in data.PurchaseOrderDetails)
            {
                model.Details.Add(AutomapperConfig.Mapper.Map<PurchaseOrderDetailViewModel>(item));
            }
            return model;
        }

        public PurchaseOrderListViewModel GetPurchaseOrderList(int purchaseOrderId)
        {
            var data = db.PurchaseOrderRepo.GetById(purchaseOrderId);
            return AutomapperConfig.Mapper.Map<PurchaseOrderListViewModel>(data);
        }

        public IList<PurchaseOrderListViewModel> GetPurchaseOrders(int fiscalYearId, string displayOrderNo, DateTime? dateFrom, DateTime? dateTo, int? stockTransactionId, bool noLinkedToPurchaseEntry = false)
        {
            var data = purchaseOrderRepository.Table.Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.PurchaseOrder
                                && (string.IsNullOrEmpty(displayOrderNo) || x.DisplayOrderNo == displayOrderNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!noLinkedToPurchaseEntry || (!x.StockTransactions.Any() || x.PurchaseOrderDetails.Sum(x1 => x1.Qty) > x.StockTransactions.Sum(x1 => x1.StockTransactionDetails.Sum(x2 => x2.Qty)) || x.StockTransactions.Any(y => y.Id == stockTransactionId)))
                                && (!dateTo.HasValue || x.Date <= dateTo)).OrderByDescending(x => x.DisplayOrderNo);
            return AutomapperConfig.Mapper.Map<IList<PurchaseOrderListViewModel>>(data);
        }

        public ServiceModel DeletePurchaseOrder(int purchaseOrderId, int deletedBy)
        {
            var result = new ServiceModel();
            var data = purchaseOrderRepository.GetById(purchaseOrderId);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                if (data.PurchaseOrderDetails != null)
                    purchaseOrderDetailRepository.DeleteRange(data.PurchaseOrderDetails);
                purchaseOrderRepository.Delete(data);

                var itemRequestDetails = itemRequestDetailRepository.Table.Where(x => x.PurchaseOrderId == data.Id);
                foreach (var det in itemRequestDetails)
                {
                    det.PurchaseOrderId = null;
                }
                itemRequestDetailRepository.CommitChanges();
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }

        #endregion Purchase Order

        #region Item Release
        public ItemReleaseViewModel SaveItemRelease(ItemReleaseViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                //bool newRecord = false;
                model.DocumentSetupId = (int)eDocumentSetup.ItemRelease;

                if (model.Details.Count == 0)
                {
                    model.Errors.Add(new ValidationResult("Please enter detail.", new string[] { "" }));
                }

                if (model.Errors.Any())
                    return;

                if (!model.Validate())
                    return;

                var entity = new ItemRelease();

                if (model.Id > 0)
                {
                    entity = itemReleaseRepository.GetById(model.Id);
                    entity.Date = model.DateBS.GetDate();
                    entity.Remarks = model.Remarks;
                    itemReleaseDetailRepository.DeleteRange(entity.ItemReleaseDetails);
                }
                else
                {
                    model.Date = model.DateBS.GetDate();
                    entity = AutomapperConfig.Mapper.Map<ItemRelease>(model);
                    itemReleaseRepository.Insert(entity);
                    //newRecord = true;
                }

                List<int> selectedItemRequestId = new List<int>(); //Item Request Detail Id --> Child table of item request table
                foreach (var item in model.Details)
                {
                    var detail = AutomapperConfig.Mapper.Map<ItemReleaseDetail>(item);
                    detail.ItemReleaseId = entity.Id;
                    itemReleaseDetailRepository.Insert(detail);

                    if (!string.IsNullOrEmpty(item.ItemRequestDetailIdCSV))
                    {
                        selectedItemRequestId.AddRange(item.ItemRequestDetailIdCSV.Split(',').Select(x => Convert.ToInt32(x)));
                    }
                }

                var selectedItemRequest = itemRequestDetailRepository.Table.Where(x => selectedItemRequestId.Contains(x.Id)); //ItemRequestDetail data table datas.
                foreach (var req in selectedItemRequest)
                {
                    if (itemReleaseDetailRepository.Table.Any(x => x.ItemReleaseId == entity.Id && x.ItemId == req.ItemId))
                        req.ItemReleaseId = entity.Id;
                    else
                        req.ItemReleaseId = null;
                }
                itemRequestRepository.CommitChanges();

                if (model.Id == 0)
                    model.Message = "Item Released Successfully.";
                else
                    model.Message = model.Message = "Item Released Updated Successfully.";
            }, model);

            return model;
        }

        public ItemReleaseViewModel GetItemRelease(int id)
        {
            var data = itemReleaseRepository.GetById(id);
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
            var data = itemReleaseRepository.GetById(itemReleaseId);
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
            var data = itemReleaseRepository.Table.Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.ItemRelease
                                && (string.IsNullOrEmpty(displayReleaseNo) || x.DisplayReleaseNo == displayReleaseNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!dateTo.HasValue || x.Date <= dateTo)).OrderByDescending(x => x.Date).ThenByDescending(x => x.DisplayReleaseNo);

            return AutomapperConfig.Mapper.Map<IList<ItemReleaseListViewModel>>(data);
        }

        public ServiceModel DeleteItemRelease(int itemReleaseId, int deletedBy)
        {
            var result = new ServiceModel();
            var data = itemReleaseRepository.GetById(itemReleaseId);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                if (data.ItemReleaseDetails != null)
                    itemReleaseDetailRepository.DeleteRange(data.ItemReleaseDetails);
                itemReleaseRepository.Delete(data);

                var itemRequestDetails = itemRequestDetailRepository.Table.Where(x => x.ItemReleaseId == data.Id);
                foreach (var det in itemRequestDetails)
                {
                    det.ItemReleaseId = null;
                }
                itemRequestDetailRepository.CommitChanges();
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Item Release

        #region Stock Adjustment
        public StockAdjustmentViewModel GetStockAdjustment(int stockTransactionId)
        {
            var data = stockTransactionReporsitory.GetById(stockTransactionId);
            var model = AutomapperConfig.Mapper.Map<StockAdjustmentViewModel>(data);
            if (model == null)
                return new StockAdjustmentViewModel();

            foreach (var item in data.StockTransactionDetails)
            {
                model.Details.Add(AutomapperConfig.Mapper.Map<StockAdjustmentDetailViewModel>(item));
            }

            return model;
        }

        public IPagedList<StockAdjustmentListViewModel> GetStockAdjustments(int fiscalYearId, string displayDocumentNo, DateTime? dateFrom, DateTime? dateTo, int? page = default(int?), int? pageSize = default(int?))
        {
            var data = stockTransactionReporsitory.Table.Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.StockAdjustment
                                && (string.IsNullOrEmpty(displayDocumentNo) || x.DisplayDocumentNo == displayDocumentNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!dateTo.HasValue || x.Date <= dateTo)).OrderBy(x => x.Date);

            var ipageList = data.ToPagedList(page ?? 1, pageSize ?? IMSAppConfig.Instance.DefaultPageSize);
            var viewModel = AutomapperConfig.Mapper.Map<IEnumerable<StockTransaction>, IEnumerable<StockAdjustmentListViewModel>>(ipageList.ToArray());
            IPagedList<StockAdjustmentListViewModel> model = new StaticPagedList<StockAdjustmentListViewModel>(viewModel, ipageList.GetMetaData());
            return model;
        }

        public StockAdjustmentViewModel SaveStockAdjustment(StockAdjustmentViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                model.DocumentSetupId = (int)eDocumentSetup.StockAdjustment;

                if (model.Details.Count == 0)
                {
                    model.Errors.Add(new ValidationResult("Please enter detail.", new string[] { "" }));
                }
                else
                {
                    if (model.Details.Any(x => x.SectionId == 0))
                        model.Errors.Add(new ValidationResult("Please select dept./section in item list.", new string[] { "" }));
                    if (model.Details.Any(x => x.Rate < 1))
                        model.Errors.Add(new ValidationResult("Please enter rate in item list.", new string[] { "" }));
                    if (model.Details.Any(x => x.NetAmount < 1))
                        model.Errors.Add(new ValidationResult("Net amount in item list is missing.", new string[] { "" }));
                }

                if (model.AdjustmentTypeId == 0)
                    model.Errors.Add(new ValidationResult("Please select adjustment type.", new string[] { "" }));

                if (model.Errors.Any())
                    return;

                if (!model.Validate())
                    return;

                model.Date = model.DateBS.GetDate();

                var entity = new StockTransaction();

                if (model.Id > 0)
                {
                    entity = stockTransactionReporsitory.GetById(model.Id);
                    entity.AdjustmentTypeId = model.AdjustmentTypeId;
                    entity.Date = model.Date;
                    entity.Remarks = model.Remarks;
                    stockTransanctionDetailRepository.DeleteRange(entity.StockTransactionDetails);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<StockTransaction>(model);

                    stockTransactionReporsitory.Insert(entity);
                }

                foreach (var item in model.Details)
                {
                    var adjustmentType = adjustmentTypeRepository.GetById(model.AdjustmentTypeId);
                    var detail = AutomapperConfig.Mapper.Map<StockTransactionDetail>(item);
                    detail.StockTransactionId = entity.Id;
                    detail.StockEffect = (adjustmentType.InOut == eStockInOutType.In.ToString()) ? (int)StockEffect.Add : (int)StockEffect.Deduct;
                    detail.NetAmount = item.NetAmount;
                    stockTransanctionDetailRepository.Insert(detail);
                }

                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Data saved successfully.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Save changes successfully.");
            }, model);
            return model;
        }
        #endregion Stock Adjustment

        #region Purchase Entry
        public PurchaseEntryViewModel GetPurchaseEntry(int PurchaseEntryId)
        {
            var data = stockTransactionReporsitory.GetById(PurchaseEntryId);
            var model = AutomapperConfig.Mapper.Map<PurchaseEntryViewModel>(data);
            if (model == null)
                return new PurchaseEntryViewModel();

            foreach (var item in data.StockTransactionDetails)
            {
                model.Details.Add(AutomapperConfig.Mapper.Map<PurchaseEntryDetailViewModel>(item));
            }

            if (model.PurchaseOrderId.HasValue)
            {
                var _purchaseOrder = purchaseOrderRepository.GetById(Convert.ToInt32(model.PurchaseOrderId));
                if (_purchaseOrder.VendorId.HasValue)
                {
                    var vendorDetail = vendorRepository.GetById(_purchaseOrder.VendorId);
                    model.VendorDetail = AutomapperConfig.Mapper.Map<VendorViewModel>(vendorDetail);
                }
            }
            return model;
        }

        public IList<PurchaseEntryListViewModel> GetPurchaseEntrys(int fiscalYearId, string displayDocumentNo, DateTime? dateFrom, DateTime? dateTo, int? vendorId)
        {
            var data = stockTransactionReporsitory.Table.Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.StoreEntry
                                && (string.IsNullOrEmpty(displayDocumentNo) || x.DisplayDocumentNo == displayDocumentNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!vendorId.HasValue || x.VendorId == vendorId)
                                && (!dateTo.HasValue || x.Date <= dateTo)).OrderByDescending(x => x.Date).ThenBy(x => x.DisplayDocumentNo);

            return AutomapperConfig.Mapper.Map<IList<PurchaseEntryListViewModel>>(data);
        }

        public PurchaseEntryViewModel SavePurchaseEntry(PurchaseEntryViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                model.DocumentSetupId = (int)eDocumentSetup.StoreEntry;

                if (model.Details.Count == 0)
                {
                    model.Errors.Add(new ValidationResult("Please enter detail.", new string[] { "" }));
                }
                else
                {
                    if (model.Details.Any(x => x.SectionId == 0))
                        model.Errors.Add(new ValidationResult("Please select dept./section in item list.", new string[] { "" }));
                }

                if (model.Errors.Any())
                    return;

                if (!model.Validate())
                    return;

                model.Date = model.DateBS.GetDate();

                var entity = new StockTransaction();

                if (model.Id > 0)
                {
                    entity = stockTransactionReporsitory.GetById(model.Id);
                    entity.PurchaseOrderId = model.PurchaseOrderId;
                    entity.Remarks = model.Remarks;
                    entity.InvoiceNo = model.InvoiceNo;
                    stockTransanctionDetailRepository.DeleteRange(entity.StockTransactionDetails);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<StockTransaction>(model);
                    stockTransactionReporsitory.Insert(entity);
                }

                foreach (var item in model.Details)
                {
                    var detail = AutomapperConfig.Mapper.Map<StockTransactionDetail>(item);
                    detail.StockTransactionId = entity.Id;
                    detail.LedgerPageNo = string.IsNullOrEmpty(detail.LedgerPageNo) ? null : detail.LedgerPageNo;
                    stockTransanctionDetailRepository.Insert(detail);
                }

                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Data saved successfully.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.DataSavedSuccessfully", "Save changes successfully.");
            }, model);
            return model;
        }
        #endregion Purchase Entry

        private List<ItemRequestReleasedNotificationViewModel> GetItemRequestReleasedData(List<int> selectedItemRequestId)
        {
            List<ItemRequestReleasedNotificationViewModel> model = new List<ItemRequestReleasedNotificationViewModel>();
            var selectedItemRequestDetail = itemRequestDetailRepository.Table.Where(x => selectedItemRequestId.Contains(x.Id)).ToList(); //ItemRequestDetail data table datas.
            List<int> ItemRequestIds = selectedItemRequestDetail.Select(x => x.ItemRequestId).Distinct().ToList();

            foreach (int itemRequestId in ItemRequestIds)
            {
                ItemRequestReleasedNotificationViewModel data = new ItemRequestReleasedNotificationViewModel();
                var itemRequest = itemRequestRepository.GetById(itemRequestId);

                data.EmployeeId = itemRequest.EmployeeId;
                data.EmployeeName = itemRequest.Employee.Name;
                data.SectionId = itemRequest.Employee.SectionId;
                data.SectionName = itemRequest.Employee.Section.Name;
                data.DesignationId = itemRequest.Employee.DesignationId;
                data.DesignationName = itemRequest.Employee.Designation.Name;

                data.ItemRequestId = itemRequest.Id;
                data.DocumentSetupId = itemRequest.DocumentSetupId;
                data.Date = itemRequest.Date;
                data.DateBS = itemRequest.Date.GetMiti();
                data.DisplayRequestNo = itemRequest.DisplayRequestNo;
                data.Remarks = itemRequest.Remarks;

                List<ItemRequestDetailNotificationViewModel> itemRequestDetailData = new List<ItemRequestDetailNotificationViewModel>();
                var itemRequestDetials = selectedItemRequestDetail.Where(x => x.ItemRequestId == itemRequestId).ToList();

                foreach (var item in itemRequestDetials)
                {
                    ItemRequestDetailNotificationViewModel _data1 = new ItemRequestDetailNotificationViewModel();
                    _data1.ItemRequestDetailId = item.Id;
                    _data1.ItemRequestId = item.ItemRequestId;
                    _data1.ItemId = item.ItemId;
                    _data1.ItemName = item.Item.Name;
                    _data1.ItemUnitName = item.Item.ItemUnit.Name;
                    _data1.Specification = item.Remarks;
                    int releaseQuantity = itemReleaseDetailRepository.Table.Where(x => x.ItemReleaseId == item.ItemReleaseId && x.ItemId == item.ItemId).Select(x => x.Qty).FirstOrDefault();
                    _data1.Quantity = releaseQuantity;
                    itemRequestDetailData.Add(_data1);
                }
                data.Details = itemRequestDetailData;
                model.Add(data);
            }

            return model;
        }
    }
}
