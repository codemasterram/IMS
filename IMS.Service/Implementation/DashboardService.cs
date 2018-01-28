using DateMiti;
using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.Configuration;
using IMS.Logic.Contract;
using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.Home;
using IMS.Logic.ViewModels.BackOffice.IMS;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IMS.Data.NTAEnum;

namespace IMS.Logic.Implementation
{
    public class DashboardService : IDashboardServicecs
    {
        private ILocalizationService localizationService;
        private INotificationService notificationService;
        private IExceptionService exceptionService;
        private IWorkFlowService workflowService;
        private ICommonService commonService;

        private IRepository<Employee> employeeRepository;
        private IRepository<AttendanceLog> attendanceLogRepository;
        private IRepository<FiscalYear> fiscalYearRepository;
        private IRepository<LeaveApplication> leaveApplicationRepository;
        private IRepository<LeaveType> leaveTypeRepository;
        private IRepository<LeaveCategory> leaveCategoryRepository;
        private IRepository<TravelApplication> travelApplicationRepository;
        private IRepository<TravelCategory> travelCategoryRepository;
        private IRepository<TravelType> travelTypeRepository;
        private IRepository<ItemRequest> itemRequestRepository;
        private IRepository<ItemRequestDetail> itemRequestDetailRepository;
        public DashboardService(ILocalizationService localizationSVC,
            INotificationService notificationSVC,
            IExceptionService exceptionSVC,
            IWorkFlowService workflowSVC,
            ICommonService commonSVC,
            IRepository<Employee> employeeRepo,
            IRepository<AttendanceLog> attendanceLogRepo,
            IRepository<FiscalYear> fiscalYearRepo,
            IRepository<LeaveApplication> leaveApplicationRepo,
            IRepository<LeaveType> leaveTypeRepo,
            IRepository<LeaveCategory> leaveCategoryRepo,
            IRepository<TravelApplication> travelApplicationRepo,
            IRepository<TravelCategory> travelCategoryRepo,
            IRepository<TravelType> travelTypeRepo,
            IRepository<ItemRequest> itemRequestRepo,
            IRepository<ItemRequestDetail> itemRequestDetailRepo)
        {
            localizationService = localizationSVC;
            this.notificationService = notificationSVC;
            exceptionService = exceptionSVC;
            workflowService = workflowSVC;
            commonService = commonSVC;
            employeeRepository = employeeRepo;
            attendanceLogRepository = attendanceLogRepo;
            fiscalYearRepository = fiscalYearRepo;
            leaveApplicationRepository = leaveApplicationRepo;
            leaveTypeRepository = leaveTypeRepo;
            leaveCategoryRepository = leaveCategoryRepo;
            travelApplicationRepository = travelApplicationRepo;
            travelCategoryRepository = travelCategoryRepo;
            travelTypeRepository = travelTypeRepo;
            itemRequestRepository = itemRequestRepo;
            itemRequestDetailRepository = itemRequestDetailRepo;
        }

        public DashboardAttendanceSummaryViewModel GetDashboardAttendanceSummary(DateTime date)
        {
            int leaveAndTravel = 0;
            var employee = employeeRepository.Table.OrderBy(x => x.Designation.DisplayOrder);
            date = (date == DateTime.MinValue) ? DateTime.Now : date;
            var attendanceLogData = attendanceLogRepository.Table.Where(x => DbFunctions.TruncateTime(x.Date) == date.Date && (!x.IsManual || (eApproveStatus)x.ApprovedStatus == eApproveStatus.Accept));
            var leaveApplicationData = leaveApplicationRepository.Table.Where(x => DbFunctions.TruncateTime(date) >= DbFunctions.TruncateTime(x.DateFrom) && DbFunctions.TruncateTime(date) <= DbFunctions.TruncateTime(x.DateTo) && x.ApproveStatus == (int)eApproveStatus.Accept).ToList();
            var travelApplicationData = travelApplicationRepository.Table.Where(x => DbFunctions.TruncateTime(date) >= DbFunctions.TruncateTime(x.DateFrom) && DbFunctions.TruncateTime(date) <= DbFunctions.TruncateTime(x.DateTo) && x.ApproveStatus == (int)eApproveStatus.Accept).ToList();

            leaveAndTravel = (leaveApplicationData != null) ? leaveApplicationData.Where(x => !travelApplicationData.Any(y => y.EmployeeId == x.EmployeeId)).Select(x => x.EmployeeId).Distinct().Count() : 0;
            leaveAndTravel += (travelApplicationData != null) ? travelApplicationData.Select(x => x.EmployeeId).Distinct().Count() : 0;

            var model = new DashboardAttendanceSummaryViewModel();
            model.TotalEmployee = employee.Select(x => x.Id).Distinct().Count();
            model.TotalPresent = (attendanceLogData != null && attendanceLogData.Count() > 0) ? attendanceLogData.Select(x => x.EmployeeId).Distinct().Count() : 0;
            model.TotalLeaveAndTravel = leaveAndTravel;
            model.TotalAbsent = model.TotalEmployee - (model.TotalPresent + model.TotalLeaveAndTravel);
            return model;
        }

        public List<DashboardEmployeeTravelDetailListViewModel> GetDashboardEmployeeTravelDetailList(DateTime date)
        {
            date = (date == DateTime.MinValue) ? DateTime.Now : date;
            var item = travelApplicationRepository.Table.Where(x => DbFunctions.TruncateTime(date) >= DbFunctions.TruncateTime(x.DateFrom) && DbFunctions.TruncateTime(date) <= DbFunctions.TruncateTime(x.DateTo) && x.ApproveStatus == (int)eApproveStatus.Accept).OrderBy(x => x.Employee.Designation.DisplayOrder).ToList();
            var model = new List<DashboardEmployeeTravelDetailListViewModel>();

            if (item != null)
                model = AutomapperConfig.Mapper.Map<List<DashboardEmployeeTravelDetailListViewModel>>(item);
            return model;
        }

        public List<DashboardEmployeeLeaveDetailListViewModel> GetDashboardEmployeeLeaveDetailList(DateTime date)
        {
            date = (date == DateTime.MinValue) ? DateTime.Now : date;
            var item = leaveApplicationRepository.Table.Where(x => DbFunctions.TruncateTime(date) >= DbFunctions.TruncateTime(x.DateFrom) && DbFunctions.TruncateTime(date) <= DbFunctions.TruncateTime(x.DateTo) && x.ApproveStatus == (int)eApproveStatus.Accept).OrderBy(x => x.Employee.Designation.DisplayOrder).ToList();
            var model = new List<DashboardEmployeeLeaveDetailListViewModel>();

            if (item != null)
                model = AutomapperConfig.Mapper.Map<List<DashboardEmployeeLeaveDetailListViewModel>>(item);
            return model;
        }

        #region Item Request
        public ItemRequestItemStatusViewModel GetPersonalItemRequestItemStatus(int itemRequestId, DateTime date, int fiscalYearId)
        {
            var model = new ItemRequestItemStatusViewModel();
            var data = itemRequestRepository.GetById(itemRequestId);
            if (data != null && data.ItemRequestDetails != null)
            {
                model.Remarks = data.Remarks;
                foreach (var item in data.ItemRequestDetails)
                {
                    var _item = new ItemRequestItemStatusListViewModel();
                    //var _data = commonService.GetItemStatusByItemId(item.ItemId, date, fiscalYearId);
                    _item.ItemId = item.ItemId;
                    _item.Code = item.Item.Code;
                    _item.Name = item.Item.Name;
                    _item.ItemGroupId = item.Item.ItemGroupId;
                    _item.ItemGroupName = item.Item.ItemGroup.Name;
                    _item.ItemUnitId = item.Item.ItemUnitId;
                    _item.UnitName = item.Item.ItemUnit.Name;
                    _item.OnDemandQuantity = item.Qty;
                    _item.InStockQuantity = 0;
                    model.Details.Add(_item);
                }
            }
            return model;
        }

        public IPagedList<ItemRequestViewModel> GetPersonalItemRequests(int fiscalYearId, int employeeId, string displayRequestNo, DateTime? dateFrom, DateTime? dateTo, bool unProcessedOnly = false, bool unReleasedOnly = false, string itemRequestIds = "", int? page = default(int?), int? pageSize = default(int?))
        {
            var data = itemRequestRepository.Table.Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.ItemRequest
                                && (employeeId == 0 || x.EmployeeId == employeeId)
                                && (string.IsNullOrEmpty(displayRequestNo) || x.DisplayRequestNo == displayRequestNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!dateTo.HasValue || x.Date <= dateTo)
                                && (!unProcessedOnly || (unProcessedOnly && x.ItemRequestDetails.Any(x1 => !x1.PurchaseOrderId.HasValue)))
                                && (!unReleasedOnly || (unReleasedOnly && x.ItemRequestDetails.Any(x1 => !x1.ItemReleaseId.HasValue)))).OrderBy(x => x.Date);

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
            var ipageList = data.ToPagedList(page ?? 1, pageSize ?? IMSAppConfig.Instance.DefaultPageSize);
            var viewModel = AutomapperConfig.Mapper.Map<IEnumerable<ItemRequest>, IEnumerable<ItemRequestViewModel>>(ipageList.ToArray());
            IPagedList<ItemRequestViewModel> modelResult = new StaticPagedList<ItemRequestViewModel>(viewModel, ipageList.GetMetaData());
            return modelResult;
        }

        public ItemRequestViewModel SavePersonalItemRequest(ItemRequestViewModel model)
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

            }, model);
            return model;
        }

        public ItemRequestViewModel GetPersonalItemRequest(int itemRequestId)
        {
            var data = itemRequestRepository.GetById(itemRequestId);
            var model = AutomapperConfig.Mapper.Map<ItemRequestViewModel>(data);

            if (model == null)
                return new ItemRequestViewModel();

            model.FiscalYearDetail = (model.FiscalYearId > 0) ? fiscalYearRepository.GetById(model.FiscalYearId) : new FiscalYear();
            return model;
        }

        public ServiceModel DeletePersonalItemRequest(int itemRequestId, int deletedBy)
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
                itemRequestDetailRepository.DeleteRange(data.ItemRequestDetails);
                itemRequestRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Item Request
    }
}
