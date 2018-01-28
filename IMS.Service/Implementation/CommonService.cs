using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Data;
using IMS.Logic.ViewModels.BackOffice.Master;
using IMS.Logic.Configuration;
using System.ComponentModel.DataAnnotations;
using IMS.Data.Infrastructure;
using IMS.Logic.ViewModels;
using PagedList;
using System.Web.Mvc;

using static IMS.Data.NTAEnum;
using IMS.Logic.ViewModels.BackOffice.Common;
using static DateMiti.GetDateMiti;
using IMS.Logic.ViewModels.BackOffice.IMS;

namespace IMS.Logic.Implementation
{
    public class CommonService : ICommonService
    {
        private ILocalizationService localizationService;
        private IExceptionService exceptionService;
        private IImsReportService reportService;
        private IRepository<FiscalYear> fiscalYearRepository;
        private IRepository<TestLog> testLogRepository;
        private IRepository<Section> sectionRepository;
        private IRepository<Item> itemRepository;
        private IRepository<SystemConfiguration> systemConfigRepository;
        public CommonService(
            ILocalizationService localizationSVC,
            IExceptionService exceptionSVC,
            IImsReportService reportSVC,
            IRepository<FiscalYear> fiscalYearRepo,
            IRepository<TestLog> testLogRepo,
            IRepository<Section> sectionRepo,
            IRepository<Item> itemRepo,
            IRepository<SystemConfiguration> systemConfigRepo)
        {
            localizationService = localizationSVC;
            exceptionService = exceptionSVC;
            reportService = reportSVC;
            fiscalYearRepository = fiscalYearRepo;
            testLogRepository = testLogRepo;
            sectionRepository = sectionRepo;
            itemRepository = itemRepo;
            systemConfigRepository = systemConfigRepo;
        }

        public IList<SystemConfigurationViewModel> GetSystemConfiguration(string key = null)
        {
            var data = systemConfigRepository.Table.Where(x => key == null || x.Key == key);
            return AutomapperConfig.Mapper.Map<IList<SystemConfigurationViewModel>>(data);

        }

        public SelectList GetDropDownList<T>(IEnumerable<T> list, string dataValueField, string dataTextField, object selectedValue = null) where T : class
        {
            return new SelectList(list, dataValueField, dataTextField, selectedValue);
        }

        public SelectList GetDropDownList<T>(string dataValueField, string dataTextField, object selectedValue = null) where T : class
        {
            IRepository<T> repo = IMSAppConfig.Instance.DependencyResolver.GetService<IRepository<T>>();
            return new SelectList(repo.Table, dataValueField, dataTextField, selectedValue);
        }

        public SelectList GetDropDownList(NTAEnum.eSelectListType listType, object selectedValue = null, object pushItemBefore = null)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (pushItemBefore != null)
            {
                if (pushItemBefore is SelectListItem)
                {
                    items.Add((SelectListItem)pushItemBefore);
                }
            }
            switch (listType)
            {
                case eSelectListType.Gender:
                    items.AddRange(new SelectListItem[]
                    {
                        new SelectListItem { Value = "Male", Text = "Male"}, new SelectListItem { Value = "Female", Text = "Female" }, new SelectListItem { Value = "Other", Text = "Other" }
                    });
                    break;

                case eSelectListType.MaritalStatus:
                    items.AddRange(new SelectListItem[]
                    {
                        new SelectListItem { Value = "Married", Text = "Married"}, new SelectListItem { Value = "Unmarried", Text = "Unmarried" }
                    });
                    break;

                case eSelectListType.EmployeeStatus:
                    items.AddRange(new SelectListItem[]
                    {
                        new SelectListItem { Value = "Active", Text = "Active"}, new SelectListItem { Value = "Resigned", Text = "Resigned" }, new SelectListItem { Value = "Retired", Text = "Retired" }
                    });
                    break;

                case eSelectListType.EmployeeType:
                    items.AddRange(new SelectListItem[]
                    {
                        new SelectListItem { Value = "Permanent", Text = "Permanent"}, new SelectListItem { Value = "Temporary", Text = "Temporary" }, new SelectListItem { Value = "Contract", Text = "Contract" }
                    });
                    break;
                case eSelectListType.ApplicableFor:
                    items.AddRange(new SelectListItem[]
                    {
                        new SelectListItem { Value="All",Text="All"} ,new SelectListItem { Value="Male",Text="Male" }, new SelectListItem {Value="Female",Text="Female" }
                    });
                    break;
                case eSelectListType.CheckType:
                    items.AddRange(new SelectListItem[]
                    {
                        new SelectListItem { Value="1",Text="In"},new SelectListItem { Value="2",Text="Out"}
                    });
                    break;
                case eSelectListType.ApprovedStatus:
                    items.AddRange(new SelectListItem[]
                        {
                            new SelectListItem {Value="1",Text="Pending" },new SelectListItem {Value="2",Text="Accept" },new SelectListItem {Value="3",Text="Reject" }
                        });
                    break;
                case eSelectListType.AttendanceReportCategory:
                    items.AddRange(new SelectListItem[]
                        {
                            new SelectListItem{ Value ="1", Text ="All" },
                            new SelectListItem { Value="2", Text="Present" },
                            new SelectListItem { Value="3", Text="Absent" },
                            new SelectListItem { Value="4", Text="Late" },
                            new SelectListItem { Value="5", Text="Punctual" }
                        });
                    break;

                case eSelectListType.FrequencyUnit:
                    items.AddRange(new SelectListItem[]
                    {
                        new SelectListItem  { Value = "Gigahertz", Text = "Gigahertz"}, new SelectListItem { Value = "Megahertz", Text = "Megahertz"}, new SelectListItem { Value = "Kilohertz", Text = "Kilohertz" }, new SelectListItem { Value = "Hertz", Text = "Hertz" }
                    });
                    break;
                case eSelectListType.StockInOutType:
                    items.AddRange(new SelectListItem[]
                         {
                            new SelectListItem {Value="1",Text="In" },new SelectListItem {Value="2",Text="Out" }
                         });
                    break;
            }

            return new SelectList(items, "Value", "Text", selectedValue);
        }

        public SelectList GetDistrictList(int? zoneId, string dataValueField, string dataTextField, object selectedValue = null)
        {
            IRepository<District> repo = IMSAppConfig.Instance.DependencyResolver.GetService<IRepository<District>>();
            return new SelectList(repo.Table.Where(x => x.ZoneId == zoneId).OrderBy(x => x.DisplayOrder), dataValueField, dataTextField, selectedValue);
        }

        public SelectList GetVdcList(int? districtId, string dataValueField, string dataTextField, object selectedValue = null)
        {
            IRepository<Vdc> repo = IMSAppConfig.Instance.DependencyResolver.GetService<IRepository<Vdc>>();
            return new SelectList(repo.Table.Where(x => x.DistrictId == districtId).OrderBy(x => x.DisplayOrder), dataValueField, dataTextField, selectedValue);
        }

        public FiscalYearViewModel GetCurrentFiscalYear()
        {
            var item = fiscalYearRepository.Table.FirstOrDefault(x => x.IsActive);
            return AutomapperConfig.Mapper.Map<FiscalYearViewModel>(item);
        }

        public IList<FiscalYearViewModel> GetFiscalYearList()
        {
            var list = fiscalYearRepository.Table;
            return AutomapperConfig.Mapper.Map<IList<FiscalYearViewModel>>(list);
        }

        public Department GetSection(int id)
        {
            var model = sectionRepository.GetById(id);
            return model;
        }

        public SelectList GetItemList(int? itemGroupId, object selectedValue = null)
        {
            IRepository<Item> repo = IMSAppConfig.Instance.DependencyResolver.GetService<IRepository<Item>>();
            var data = repo.Table.Where(x => x.ItemGroupId == itemGroupId).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name);
            List<Item> list = new List<Item>();
            foreach (var item in data)
            {
                list.Add(new Item
                {
                    Id = item.Id,
                    Name = string.Format("{2} {0} - {1}", item.Name, item.ItemUnit.Name, item.Code)
                });
            }
            return new SelectList(list, "Id", "Name", selectedValue);
        }

        public ItemStatusViewModel GetItemStatusByItemId(int itemId, DateTime date, int fiscalYearId)
        {
            ItemStatusViewModel model = new ItemStatusViewModel();
            string data = reportService.GetRemainingItemDetailByDate(itemId, date, fiscalYearId);
            var item = itemRepository.GetById(itemId);
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

        public SelectList GetEmployeeList(int? sectionId, object selectedValue = null)
        {
            IRepository<Employee> repo = IMSAppConfig.Instance.DependencyResolver.GetService<IRepository<Employee>>();
            var data = repo.Table.Where(x => x.DepartmentId == sectionId).OrderBy(x => x.Designation.DisplayOrder).ThenBy(x => x.Name);
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

        public SelectList GetConsumableItems(object selectedValue = null)
        {
            IRepository<Item> repo = IMSAppConfig.Instance.DependencyResolver.GetService<IRepository<Item>>();
            var data = repo.Table.Where(x => x.ItemGroup.ItemTypeId == (int)ItemTypes.Consumable).OrderBy(x => x.DisplayOrder);
            List<Item> list = new List<Item>();
            foreach (var item in data)
            {
                list.Add(new Item
                {
                    Id = item.Id,
                    Name = string.Format("{0} - {1}", item.Name, item.ItemUnit.Name)
                });
            }
            return new SelectList(list, "Id", "Name", selectedValue);
        }

        public SelectList GetNonConsumableItems(object selectedValue = null)
        {
            IRepository<Item> repo = IMSAppConfig.Instance.DependencyResolver.GetService<IRepository<Item>>();
            var data = repo.Table.Where(x => x.ItemGroup.ItemTypeId == (int)ItemTypes.NonConsumable).OrderBy(x => x.DisplayOrder);
            List<Item> list = new List<Item>();
            foreach (var item in data)
            {
                list.Add(new Item
                {
                    Id = item.Id,
                    Name = string.Format("{0} - {1}", item.Name, item.ItemUnit.Name)
                });
            }
            return new SelectList(list, "Id", "Name", selectedValue);
        }
        public SelectList GetTelecomServiceType(object selectedValue = null)
        {
            IRepository<TelecomServiceType> repo = IMSAppConfig.Instance.DependencyResolver.GetService<IRepository<TelecomServiceType>>();
            var data = repo.Table.OrderBy(x => x.DisplayOrder);
            List<Item> list = new List<Item>();
            foreach (var item in data)
            {
                list.Add(new Item
                {
                    Id = item.Id,
                    Name = item.Name
                });
            }
            return new SelectList(list, "Id", "Name", selectedValue);
        }

        #region TestLog
        public TestLogViewModel SaveTestLog(TestLogViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (!model.Validate())
                    return;
                var entity = new TestLog();

                if (model.Id > 0)
                {
                    entity = testLogRepository.GetById(model.Id);
                    entity.IsResolved = model.IsResolved;
                    entity.ResolvedBy = model.ResolvedBy;
                    entity.ResolvedDate = model.ResolvedDate;
                    entity.ResolvedRemarks = model.ResolvedRemarks;
                    testLogRepository.Update(entity);
                }
                else
                {
                    model.Date = DateTime.Now;
                    model.ResolvedDate = null;
                    entity = AutomapperConfig.Mapper.Map<TestLog>(model);
                    testLogRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);
            return model;
        }

        public TestLogViewModel MakeAsResolve(TestLogViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (!model.Validate())
                    return;
                var entity = new TestLog();

                entity = testLogRepository.GetById(model.Id);
                entity.IsResolved = true;
                entity.ResolvedBy = model.ResolvedBy;
                entity.ResolvedDate = DateTime.Now;
                entity.ResolvedRemarks = model.ResolvedRemarks;
                testLogRepository.Update(entity);
                model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);
            return model;
        }

        public TestLogViewModel GetTestLog(int TestLogId)
        {
            var item = testLogRepository.GetById(TestLogId);
            var model = AutomapperConfig.Mapper.Map<TestLogViewModel>(item);
            return model;
        }

        public IPagedList<TestLogListViewModel> GetTestLogs(string module = null, string dateFromBs = null, string dateToBS = null, bool pending = true, int? page = null, int? pageSize = null)
        {
            DateTime dateFrom = dateFromBs.GetDate();
            DateTime dateTo = dateToBS.GetDate();

            var data = testLogRepository.Table.Where(x =>
                                ((string.IsNullOrEmpty(module)) || x.Module.StartsWith(module))
                             && (dateFrom == DateTime.MinValue || x.Date >= dateFrom)
                             && (dateTo == DateTime.MinValue || x.Date <= dateTo)
                             && (!pending || !x.IsResolved)).OrderBy(x => x.Date);
            var ipageList = data.ToPagedList(page ?? 1, pageSize ?? IMSAppConfig.Instance.DefaultPageSize);
            var viewModel = AutomapperConfig.Mapper.Map<IEnumerable<TestLog>, IEnumerable<TestLogListViewModel>>(ipageList.ToArray());
            IPagedList<TestLogListViewModel> model = new StaticPagedList<TestLogListViewModel>(viewModel, ipageList.GetMetaData());
            return model;
        }

        public ServiceModel DeleteTestLog(int id)
        {
            var result = new ServiceModel();
            var data = testLogRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                testLogRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion TestLog

    }
}
