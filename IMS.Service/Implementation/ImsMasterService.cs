using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using IMS.Logic.ViewModels.BackOffice.IMS;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.Entity;
using IMS.Logic.Configuration;
using IMS.Logic.ViewModels;
using IMS.Logic.Utilities;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace IMS.Logic.Implementation
{
    public class ImsMasterService : IImsMasterService
    {
        #region Constructor
        private ILocalizationService localizationService;
        private IExceptionService exceptionService;

        private IRepository<ItemUnit> itemUnitRepository;
        private IRepository<ItemType> itemTypeRepository;
        private IRepository<Item> itemRepository;
        private IRepository<ItemGroup> itemGroupRepository;
        private IRepository<ItemGroupDepreciationRate> itemGroupDepreciationRateRepository;
        private IRepository<Vendor> vendorRepository;

        private IRepository<AdjustmentType> adjustmentTypeRepository;

        public ImsMasterService(
            ILocalizationService localizationSvc,
            IExceptionService exceptionSVC,

            IRepository<ItemUnit> itemUnitRepo,
            IRepository<ItemType> itemTypeRepo,
            IRepository<Item> itemRepo,
            IRepository<ItemGroup> itemGroupRepo,
            IRepository<ItemGroupDepreciationRate> itemGroupDepreciationRepo,
            IRepository<Vendor> vendorRepo,
            IRepository<AdjustmentType> adjustmentTypeRepo
            )
        {
            localizationService = localizationSvc;
            exceptionService = exceptionSVC;

            itemUnitRepository = itemUnitRepo;
            itemTypeRepository = itemTypeRepo;
            itemRepository = itemRepo;
            itemGroupRepository = itemGroupRepo;
            itemGroupDepreciationRateRepository = itemGroupDepreciationRepo;
            vendorRepository = vendorRepo;
            adjustmentTypeRepository = adjustmentTypeRepo;
        }
        #endregion Constructor

        #region ItemUnit
        public ItemUnitViewModel SaveItemUnit(ItemUnitViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (itemUnitRepository.Table.Any(x => x.Name == model.Name && x.Id != model.Id))
                    model.AddModelError(x => x.Name, "Error.DuplicateValue", "Item unit already exists by this name.");

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new ItemUnit();

                if (model.Id > 0)
                {
                    entity = itemUnitRepository.GetById(model.Id);
                    entity.Name = model.Name;
                    entity.Code = model.Code;
                    entity.DisplayOrder = model.DisplayOrder;
                    itemUnitRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<ItemUnit>(model);
                    itemUnitRepository.Insert(entity);
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.ItemUnitSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Item Unit saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.ItemUnitUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Item Unit update succeeded.");
            }, model);

            return model;
        }

        public ItemUnitViewModel GetItemUnit(int itemId)
        {
            var item = itemUnitRepository.GetById(itemId);
            var model = AutomapperConfig.Mapper.Map<ItemUnitViewModel>(item);
            return model;
        }

        public IList<ItemUnitListViewModel> GetItemUnits(string name)
        {
            var data = itemUnitRepository.Table.Where(x => string.IsNullOrEmpty(name) || x.Name.StartsWith(name)).OrderBy(x => x.Name);
            var model = AutomapperConfig.Mapper.Map<IList<ItemUnitListViewModel>>(data);
            return model;
        }

        public ServiceModel DeleteItemUnit(int id)
        {
            var result = new ServiceModel();
            var data = itemUnitRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                itemUnitRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion

        #region Item
        public ItemViewModel SaveItem(ItemViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (itemRepository.Table.Any(x => x.Name == model.Name && x.Id != model.Id))
                    model.AddModelError(x => x.Name, "Error.DuplicateValue", "Item name already exists.");

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new Item();

                if (model.Id > 0)
                {
                    entity = itemRepository.GetById(model.Id);
                    entity.Name = model.Name;
                    entity.Code = model.Code;
                    entity.ItemGroupId = model.ItemGroupId;
                    entity.ItemUnitId = model.ItemUnitId;
                    itemRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<Item>(model);
                    itemRepository.Insert(entity);
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.EmployeeSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Employee saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.EmployeeUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Employee update succeeded.");
            }, model);

            return model;
        }

        public ItemViewModel GetItem(int itemId)
        {
            var item = itemRepository.GetById(itemId);
            var model = AutomapperConfig.Mapper.Map<ItemViewModel>(item);
            return model;
        }

        public IList<ItemListViewModel> GetItems(int? itemGroupId, int? itemUnitId, int? itemTypeId, string name)
        {
            var data = itemRepository.Table.Where(x => (!itemGroupId.HasValue || x.ItemGroupId == itemGroupId) &&
                                (!itemUnitId.HasValue || x.ItemUnitId == itemUnitId) &&
                                (!itemTypeId.HasValue || x.ItemGroup.ItemTypeId == itemTypeId) &&
                                (string.IsNullOrEmpty(name.Trim()) || x.Name.StartsWith(name.Trim()))).OrderBy(x => x.ItemGroup.DisplayOrder ?? 0).ThenBy(x => x.Name);
            var model = AutomapperConfig.Mapper.Map<IList<ItemListViewModel>>(data);
            return model;
        }

        public ServiceModel DeleteItem(int id)
        {
            var result = new ServiceModel();
            var data = itemRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                itemRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion

        #region ItemGroup
        public ItemGroupViewModel SaveItemGroup(ItemGroupViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (itemGroupRepository.Table.Any(x => x.Name == model.Name && x.Id != model.Id))
                    model.AddModelError(x => x.Name, "Error.DuplicateValue", "Item group name already exists.");

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new ItemGroup();

                if (model.Id > 0)
                {
                    entity = itemGroupRepository.GetById(model.Id);
                    entity.Name = model.Name;
                    entity.Code = model.Code;
                    entity.ItemTypeId = model.ItemTypeId;
                    entity.DisplayOrder = model.DisplayOrder;
                    itemGroupRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<ItemGroup>(model);
                    itemGroupRepository.Insert(entity);
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.EmployeeSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Employee saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.EmployeeUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Employee update succeeded.");
            }, model);

            return model;
        }

        public ItemGroupViewModel GetItemGroup(int itemGroupId)
        {
            var item = itemGroupRepository.GetById(itemGroupId);
            var model = AutomapperConfig.Mapper.Map<ItemGroupViewModel>(item);
            return model;
        }

        public IList<ItemGroupListViewModel> GetItemGroups(string name)
        {
            var data = itemGroupRepository.Table.Where(x => string.IsNullOrEmpty(name) || x.Name.StartsWith(name)).OrderBy(x => x.DisplayOrder);
            var model = AutomapperConfig.Mapper.Map<IList<ItemGroupListViewModel>>(data);
            return model;
        }

        public ServiceModel DeleteItemGroup(int id)
        {
            var result = new ServiceModel();
            var data = itemGroupRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                itemGroupRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }

        public ItemGroupDepreciationRateViewModel SaveItemGroupDepreciationRate(ItemGroupDepreciationRateViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (itemGroupDepreciationRateRepository.Table.Any(x => x.FiscalYearId == model.FiscalYearId && x.ItemGroupId == model.ItemGroupId && x.Id != model.Id))
                    model.AddModelError(x => x.FiscalYearId, "Error.ItemGroupDepreciationRate.DuplicateValue", "Rate for given fiscal year already exists.");

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new ItemGroupDepreciationRate();

                if (model.Id > 0)
                {
                    entity = itemGroupDepreciationRateRepository.GetById(model.Id);
                    entity.FiscalYearId = model.FiscalYearId;
                    entity.DepreciationRate = model.DepreciationRate;
                    itemGroupDepreciationRateRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<ItemGroupDepreciationRate>(model);
                    itemGroupDepreciationRateRepository.Insert(entity);
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.ItemGroupDepreciationRateSaveSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Rate saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.ItemGroupDepreciationRateUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Rate update succeeded.");
            }, model);

            return model;
        }

        public IList<ItemGroupDepreciationRateViewModel> GetDepreciationRates(int itemGroupId)
        {
            var data = itemGroupDepreciationRateRepository.Table.Where(x => x.ItemGroupId == itemGroupId).OrderByDescending(x => x.FiscalYear.DisplayOrder);
            var list = AutomapperConfig.Mapper.Map<IList<ItemGroupDepreciationRateViewModel>>(data);
            return list;
        }

        public ItemGroupDepreciationRateViewModel GetDepreciationRate(int id)
        {
            var data = itemGroupDepreciationRateRepository.GetById(id);
            var model = AutomapperConfig.Mapper.Map<ItemGroupDepreciationRateViewModel>(data);
            return model;
        }

        public ServiceModel DeleteItemGroupDepreciationRate(int id)
        {
            var result = new ServiceModel();
            var data = itemGroupDepreciationRateRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                itemGroupDepreciationRateRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion

        #region Vendor
        public VendorViewModel SaveVendor(VendorViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (vendorRepository.Table.Any(x => x.Name == model.Name && x.Id != model.Id))
                    model.AddModelError(x => x.Name, "Error.DuplicateValue", "Vendor name already exists.");

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new Vendor();

                if (model.Id > 0)
                {
                    entity = vendorRepository.GetById(model.Id);
                    entity.Name = model.Name;
                    entity.Address = model.Address;
                    entity.City = model.City;
                    entity.PhoneNo = model.PhoneNo;
                    entity.MobileNo = model.MobileNo;
                    entity.EmailId = model.EmailId;
                    entity.WebSite = model.WebSite;
                    entity.ContactPerson = model.ContactPerson;
                    entity.ContactPersonPhoneNo = model.ContactPersonPhoneNo;
                    entity.ContactPersonMobileNo = model.ContactPersonMobileNo;
                    entity.DisplayOrder = model.DisplayOrder;
                    entity.Remarks = model.Remarks;
                    vendorRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<Vendor>(model);
                    vendorRepository.Insert(entity);
                }
                model.Message = localizationService.GetLocalizedText("Message.SaveSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Data saved succeeded.");
            }, model);

            return model;
        }

        public VendorViewModel GetVendor(int vendorId)
        {
            var item = vendorRepository.GetById(vendorId);
            var model = AutomapperConfig.Mapper.Map<VendorViewModel>(item);
            return model;
        }

        public IList<VendorListViewModel> GetVendors(string name)
        {
            var data = vendorRepository.Table.Where(x => string.IsNullOrEmpty(name) || x.Name.StartsWith(name)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name);
            var model = AutomapperConfig.Mapper.Map<IList<VendorListViewModel>>(data);
            return model;
        }

        public ServiceModel DeleteVendor(int id)
        {
            var result = new ServiceModel();
            var data = vendorRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                vendorRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion

        #region AdjustmentType
        public AdjustmentTypeViewModel SaveAdjustmentType(AdjustmentTypeViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (adjustmentTypeRepository.Table.Any(x => x.Name == model.Name && x.Id != model.Id))
                    model.AddModelError(x => x.Id, "Error.DuplicateValue", "Adjustment type name already exists.");

                if (model.DisplayOrder == 0)
                {
                    int displayOrder = (int)adjustmentTypeRepository.Table.Select(x => x.DisplayOrder).Max();
                    model.DisplayOrder = displayOrder + 1;
                }
                if (model.StockInOutType == 0)
                    model.AddModelError(x => x.StockInOutType, "Message.SelectStockInOutType", string.Format("{0} can not be null.", model.GetDisplayName(x => x.StockInOutType)));
                else
                    model.InOut = ((NTAEnum.eStockInOutType)model.StockInOutType).ToString();


                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new AdjustmentType();

                if (model.Id > 0)
                {
                    entity = adjustmentTypeRepository.GetById(model.Id);
                    entity.Name = model.Name;
                    entity.InOut = model.InOut;
                    entity.DisplayOrder = model.DisplayOrder;
                    adjustmentTypeRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<AdjustmentType>(model);
                    adjustmentTypeRepository.Insert(entity);
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.EmployeeSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Employee saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.EmployeeUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Employee update succeeded.");
            }, model);

            return model;
        }

        public AdjustmentTypeViewModel GetAdjustmentType(int adjustmentTypeId)
        {
            var item = adjustmentTypeRepository.GetById(adjustmentTypeId);
            var model = AutomapperConfig.Mapper.Map<AdjustmentTypeViewModel>(item);
            return model;
        }

        public IList<AdjustmentTypeListViewModel> GetAdjustmentTypes(string name, int? stockInOutType)
        {
            var data = adjustmentTypeRepository.Table.Where(x => (string.IsNullOrEmpty(name) || x.Name.StartsWith(name))).OrderBy(x => x.Name);
            var model = AutomapperConfig.Mapper.Map<IList<AdjustmentTypeListViewModel>>(data);
            model = model.OrderBy(x => x.DisplayOrder).ToList();
            return model;
        }

        public ServiceModel DeleteAdjustmentType(int id)
        {
            var result = new ServiceModel();
            var data = adjustmentTypeRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                adjustmentTypeRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion AdjustmentType
    }
}
