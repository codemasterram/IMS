using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IMS.Logic.ViewModels.BackOffice.Master;
using IMS.Data.Infrastructure;
using IMS.Logic.ViewModels.BackOffice.Employee;
using IMS.Logic.ViewModels.BackOffice.User;
using IMS.Data;
using static IMS.Data.NTAEnum;
using IMS.Logic.ViewModels.BackOffice.Common;
using static IMS.Logic.Extensions.ViewModelExtensions;
using IMS.Logic.ViewModels.BackOffice;
using IMS.Logic.ViewModels.BackOffice.Home;
using System.Web.Mvc;
using IMS.Logic.ViewModels.BackOffice.IMS;
using DateMiti;
using IMS.Logic.ViewModels;

namespace IMS.Logic.Configuration
{
    public partial class AutomapperConfig
    {
        #region Constructor
        private static IMapper mapper;
        public static IMapper Mapper
        {
            get
            {
                return mapper ?? (mapper = new AutomapperConfig().ConfigureMapper());
            }
        }

        private AutomapperConfig()
        {
        }
        #endregion

        private IMapper ConfigureMapper()
        {
            EncryptionService encSvc = new EncryptionService();

            AutoMapper.Configuration.MapperConfigurationExpression mapConfig = new AutoMapper.Configuration.MapperConfigurationExpression();
            MappingConfigurationBackOffice(mapConfig);
            var config = new MapperConfiguration(mapConfig);
            AutoMapper.Mapper.Initialize(mapConfig);
            return AutoMapper.Mapper.Instance;
        }

        private void MappingConfigurationBackOffice(IMapperConfigurationExpression cfg)
        {
            #region SystemConfiguration
            cfg.CreateMap<SystemConfiguration, SystemConfigurationViewModel>();
            #endregion


            #region FiscalYear
            cfg.CreateMap<FiscalYear, FiscalYearViewModel>()
            .ForMember(d => d.FiscalYearId, opts => opts.MapFrom(src => src.Id));
            #endregion


            #region LoginEmployee
            cfg.CreateMap<Employee, LoginEmployee>()
            .ForMember(d => d.UserId, opts => opts.MapFrom(src => src.AppUsers.Any() ? src.AppUsers.FirstOrDefault().Id : 0))
            .ForMember(d => d.Id, opts => opts.MapFrom(src => src.Id));
            #endregion

            #region RoleModule
            cfg.CreateMap<Role, RoleModel>();
            cfg.CreateMap<Permission, PermissionModel>()
            .AfterMap((src, dest) =>
            {
                dest.PermissionName = src.Name;
                dest.PermissionDescription = src.Description;
            });
            #endregion RoleModule

            #region Role
            cfg.CreateMap<Role, RoleListViewModel>()
            .AfterMap((s, d) =>
            {
                RoleListViewModel role = new RoleListViewModel();
                d.Id = s.Id;
                d.Role = s.Name.ConvertToSentence();

            });
            #endregion

            #region EmployeeProfile
            cfg.CreateMap<Employee, EmployeeProfileViewModel>()
            .ForMember(d => d.SectionName, opts => opts.MapFrom(src => src.Department.Name))
            .ForMember(d => d.DesignationName, opts => opts.MapFrom(src => src.Designation.Name))
            .ForMember(d => d.DateOfBirthBS, opts => opts.MapFrom(src => DateMiti.GetDateMiti.GetMiti(src.DateOfBirth)));
            cfg.CreateMap<EmployeeProfileViewModel, Employee>();

            #endregion EmployeeProfile

            #region Employee
            cfg.CreateMap<Employee, EmployeeViewModel>()
            .ForMember(d => d.DateOfBirthBS, opts => opts.MapFrom(src => DateMiti.GetDateMiti.GetMiti(src.DateOfBirth)));
            cfg.CreateMap<EmployeeViewModel, Employee>();
            cfg.CreateMap<Employee, EmployeeListViewModel>()
            .ForMember(d => d.DesignationName, opts => opts.MapFrom(src => src.Designation.Name))
            .ForMember(d => d.SectionName, opts => opts.MapFrom(src => src.Department.Name))
            .ForMember(d => d.UserName, opts => opts.MapFrom(src => src.AppUsers.Any() ? src.AppUsers.FirstOrDefault().UserName : null));

            #endregion Employee

            #region AppUser
            cfg.CreateMap<AppUser, AppUserViewModel>()
            .AfterMap((s, dest) =>
            {
                dest.Name = s.EmployeeId.HasValue ? s.Employee.Name : null;
                dest.FullNameNP = s.EmployeeId.HasValue ? s.Employee.Name : null;
                dest.Email = s.EmployeeId.HasValue ? s.Employee.Email : null;
            });

            cfg.CreateMap<AppUser, UserViewModel>()

            .AfterMap((s, d) =>
            {
                if (s.EmployeeId.HasValue)
                {
                    d.EmployeeId = s.EmployeeId;
                    d.EmployeeName = s.Employee.Name;
                    d.Email = s.Employee.Email;
                    d.DesignationName = s.Employee.Designation.Name;
                }
                if (s.UserRoles.Any())
                {
                    d.Roles = s.UserRoles.Select(x => (int?)x.RoleId).ToArray();
                }

            });

            cfg.CreateMap<UserViewModel, AppUser>();

            cfg.CreateMap<AppUser, UserListViewModel>()
            .ForMember(d => d.Active, opts => opts.MapFrom(src => src.IsActive ? "Active" : "Passive"))
            .AfterMap((s, d) =>
            {
                if (s.EmployeeId.HasValue)
                {
                    d.EmployeeId = s.EmployeeId;
                    d.EmployeeName = s.Employee.Name;
                    d.Email = s.Employee.Email;
                }
                if (s.UserRoles.Any())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var role in s.UserRoles)
                    {
                        sb.AppendFormat("{0}, ", role.Role.Name.ConvertToSentence());
                    }
                    d.RoleName = sb.ToString().Trim().Trim(',');
                }
            });
            #endregion AppUser

            #region LoginHostory
            cfg.CreateMap<LoginHistory, LoginHistoryViewModel>();
            cfg.CreateMap<LoginHistoryViewModel, LoginHistory>();
            #endregion


            #region Ims

            #region Ims Master

            #region Document Numbering
            cfg.CreateMap<DocumentNumbering, DocumentNumberingViewModel>();
            cfg.CreateMap<DocumentNumberingViewModel, DocumentNumbering>();

            cfg.CreateMap<DocumentNumbering, DocumentNumberingListViewModel>()
            .ForMember(d => d.DocumentSetupName, opts => opts.MapFrom(src => src.DocumentSetup.Name))
            .ForMember(d => d.FiscalYearName, opts => opts.MapFrom(src => src.FiscalYear.Name));
            #endregion Document Numbering

            #region ItemUnit
            cfg.CreateMap<ItemUnit, ItemUnitViewModel>();
            cfg.CreateMap<ItemUnitViewModel, ItemUnit>();

            cfg.CreateMap<ItemUnit, ItemUnitListViewModel>();
            #endregion ItemUnit

            #region Item
            cfg.CreateMap<Item, ItemViewModel>()
            .ForMember(d => d.ItemUnitName, opts => opts.MapFrom(src => src.ItemUnit.Name));
            cfg.CreateMap<ItemViewModel, Item>();

            cfg.CreateMap<Item, ItemListViewModel>()
            .ForMember(d => d.ItemGroupName, opts => opts.MapFrom(src => src.ItemGroup.Name))
            .ForMember(d => d.ItemUnitName, opts => opts.MapFrom(src => src.ItemUnit.Name))
            .ForMember(d => d.ItemTypeName, opts => opts.MapFrom(src => src.ItemGroup.ItemType.Name));
            #endregion Item

            #region Item Group
            cfg.CreateMap<ItemGroup, ItemGroupViewModel>().ForMember(d => d.ItemTypeAlias, opts => opts.MapFrom(src => src.ItemType.Alias));
            cfg.CreateMap<ItemGroupViewModel, ItemGroup>();

            cfg.CreateMap<ItemGroup, ItemGroupListViewModel>()
            .ForMember(d => d.ItemTypeName, opts => opts.MapFrom(src => src.ItemType.Name))
            .ForMember(d => d.IsFixed, opts => opts.MapFrom(src => src.ItemType.Alias == "Non-Consumable"));


            cfg.CreateMap<DepreciationRate, ItemGroupDepreciationRateViewModel>()
                .ForMember(d => d.FiscalYearName, opts => opts.MapFrom(src => src.FiscalYear.Name))
                .ForMember(d => d.ItemGroupName, opts => opts.MapFrom(src => src.ItemGroup.Name));

            cfg.CreateMap<ItemGroupDepreciationRateViewModel, DepreciationRate>();
            #endregion Item Group

            #region Vendor
            cfg.CreateMap<Vendor, VendorViewModel>();
            cfg.CreateMap<VendorViewModel, Vendor>();

            cfg.CreateMap<Vendor, VendorListViewModel>();
            #endregion Vendor


            #endregion Ims Master


            #region OpeningStock
            cfg.CreateMap<ItemReleaseDetail, TransactionItemViewModel>()
            .AfterMap((s, d) =>
            {
                d.ItemId = s.ItemId;
                d.ItemCode = s.Item.Code;
                d.ItemName = s.Item.Name;
                d.Qty = s.Qty;
            });

            cfg.CreateMap<ItemRequestDetail, TransactionItemViewModel>()
            .AfterMap((s, d) =>
            {
                d.ItemId = s.ItemId;
                d.ItemCode = s.Item.Code;
                d.ItemName = s.Item.Name;
                d.Qty = s.Qty;
            });


            cfg.CreateMap<StockTransaction, OpeningStockViewModel>()
            .ForMember(d => d.DateBS, opts => opts.MapFrom(src => src.Date.GetMiti()));
            cfg.CreateMap<OpeningStockViewModel, StockTransaction>()
            .ForMember(d => d.Date, opts => opts.MapFrom(src => src.DateBS.GetDate()));
            cfg.CreateMap<StockTransactionDetail, OpeningStockDetailViewModel>()
            .ForMember(d => d.PurchaseDateBS, opts => opts.MapFrom(src => src.PurchaseDate.GetMiti()))
            .ForMember(d => d.ItemName, opts => opts.MapFrom(src => src.Item.Name))
            .ForMember(d => d.UnitName, opts => opts.MapFrom(src => src.Item.ItemUnit.Name))
            .ForMember(d => d.SectionName, opts => opts.MapFrom(src => src.DepartmentId.HasValue ? src.Department.Name : null))
            .ForMember(d => d.EmployeeName, opts => opts.MapFrom(src => src.EmployeeId.HasValue ? src.Employee.Name : null))
            .ForMember(d => d.Amount, opts => opts.MapFrom(src => src.NetAmount))
            .ForMember(d => d.Narration, opts => opts.MapFrom(src => src.Remarks));

            cfg.CreateMap<OpeningStockDetailViewModel, StockTransactionDetail>()
            .ForMember(d => d.PurchaseDate, opts => opts.MapFrom(src => src.PurchaseDateBS.GetDate() == DateTime.MinValue ? (DateTime?)null : src.PurchaseDateBS.GetDate()))
            .ForMember(d => d.NetAmount, opts => opts.MapFrom(src => src.Amount))
            .ForMember(d => d.Remarks, opts => opts.MapFrom(src => src.Narration));

            cfg.CreateMap<StockTransaction, OpeningStockListViewModel>()
            .ForMember(d => d.NoOfItems, opts => opts.MapFrom(src => src.StockTransactionDetails.Count))
            .ForMember(d => d.DateBS, opts => opts.MapFrom(src => src.Date.GetMiti()))

             //new
             .ForMember(d => d.EmployeeDesignation, opts => opts.MapFrom(src => src.Employee.Designation.Name))
             .ForMember(d => d.EmployeeSection, opts => opts.MapFrom(src => src.Employee.Department.Name))
             .AfterMap((s, d) =>
             {
                 d.ItemList = $"{s.StockTransactionDetails.FirstOrDefault().Item.Name}";
                 d.ItemList = s.StockTransactionDetails.GroupBy(x => x.ItemId).Count() > 1 ? $"{d.ItemList} And other..." : d.ItemList;




                 foreach (var item in s.StockTransactionDetails)
                 {
                     d.Details.Add(Mapper.Map<ItemRequestDetailViewModel>(item));
                 }

             });

            cfg.CreateMap<OpeningStockListViewModel, StockTransaction>();
            cfg.CreateMap<StockTransactionDetail, ItemRequestDetailViewModel>()
            .ForMember(d => d.ItemId, opts => opts.MapFrom(src => src.ItemId))
            .ForMember(d => d.ItemCode, opts => opts.MapFrom(src => src.Item.Code))
            .ForMember(d => d.ItemName, opts => opts.MapFrom(src => src.Item.Name))
            .ForMember(d => d.UnitName, opts => opts.MapFrom(src => src.Item.ItemUnit.Name))
            .ForMember(d => d.Remarks, opts => opts.MapFrom(src => src.Remarks));
            cfg.CreateMap<ItemRequestDetailViewModel, StockTransactionDetail>()
            .ForMember(d => d.Remarks, opts => opts.MapFrom(src => src.Remarks));


            #endregion OpeningStock

            #region Item Request
            cfg.CreateMap<ItemRequest, ItemRequestViewModel>()
            .ForMember(d => d.EmployeeDesignation, opts => opts.MapFrom(src => src.Employee.Designation.Name))
            .ForMember(d => d.EmployeeSection, opts => opts.MapFrom(src => src.Employee.Department.Name))
            .ForMember(d => d.DateBS, opts => opts.MapFrom(src => src.Date.GetMiti()))
            //.ForMember(d => d.AcceptedBy, opts => opts.MapFrom(src => src.Employee1 != null ? src.Employee1.Name : ""))
            //.ForMember(d => d.ItemReceivedBy, opts => opts.MapFrom(src => src.Employee2 != null ? src.Employee2.Name : ""))
            .AfterMap((s, d) =>
            {
                d.ItemList = $"{s.ItemRequestDetails.FirstOrDefault().Item.Name} ";
                d.ItemList = s.ItemRequestDetails.GroupBy(x => x.ItemId).Count() > 1 ? $"{d.ItemList } And other..." : d.ItemList;

                d.SectionName = s.Department.Name;
                d.EmployeeName = s.Employee.Name;
                foreach (var item in s.ItemRequestDetails)
                {
                    d.Details.Add(Mapper.Map<ItemRequestDetailViewModel>(item));
                }
                //d.AnyItemsLeftForPurchaseOrder = s.ItemRequestDetails.Any(x => !x.PurchaseOrderId.HasValue && !x.ItemReleaseId.HasValue);
                //d.AnyItemsLeftToReleased = s.ItemRequestDetails.Any(x => !x.ItemReleaseId.HasValue);

                d.ItemRequestStatus = (int)eItemRequestStatus.Pending;
                if (s.ItemReleases.Any())
                {
                    var demand = s.ItemRequestDetails.Sum(x => x.Qty);
                    var released = s.ItemReleases.Sum(x => x.ItemReleaseDetails.Sum(y => y.Qty));
                    if (demand > released)
                    {
                        d.ItemRequestStatus = (int)eItemRequestStatus.PartialReleased;
                    }
                    else
                    {
                        d.ItemRequestStatus = (int)eItemRequestStatus.Released;
                    }
                }
                else if (s.AcceptedBy.HasValue)
                {
                    d.ItemRequestStatus = (int)eItemRequestStatus.OnProcess;
                }
            });
            cfg.CreateMap<ItemRequestViewModel, ItemRequest>();
            cfg.CreateMap<ItemRequestDetail, ItemRequestDetailViewModel>()
            .ForMember(d => d.ItemCode, opts => opts.MapFrom(src => src.Item.Code))
            .ForMember(d => d.ItemName, opts => opts.MapFrom(src => src.Item.Name))
            .ForMember(d => d.UnitName, opts => opts.MapFrom(src => src.Item.ItemUnit.Name))
            .ForMember(d => d.Remarks, opts => opts.MapFrom(src => src.Remarks));
            cfg.CreateMap<ItemRequestDetailViewModel, ItemRequestDetail>()
            .ForMember(d => d.Remarks, opts => opts.MapFrom(src => src.Remarks));

            cfg.CreateMap<ItemRequestDetail, DashboardItemRequestViewModel>()
                .ForMember(d => d.RequestedDate, opts => opts.MapFrom(src => src.ItemRequest.Date))
                .ForMember(d => d.RequestedMiti, opts => opts.MapFrom(src => DateMiti.GetDateMiti.GetMiti(src.ItemRequest.Date)))
                .ForMember(d => d.RequestNo, opts => opts.MapFrom(src => src.ItemRequest.DisplayDocumentNo))
                .ForMember(d => d.Item, opts => opts.MapFrom(src => src.Item.Name))
                .ForMember(d => d.Status, opts => opts.MapFrom(src => Enum.GetName(typeof(eApplicationStatus), src.ItemRequest.ApplicationStatus)));
            #endregion

            #region PurchaseOrder
            cfg.CreateMap<PurchaseOrder, PurchaseOrderViewModel>()
            .ForMember(d => d.DateBS, opts => opts.MapFrom(src => src.Date.GetMiti()))
            .ForMember(d => d.DueDateBS, opts => opts.MapFrom(src => src.DueDate.GetMiti()));
            cfg.CreateMap<PurchaseOrderViewModel, PurchaseOrder>()
            .ForMember(d => d.Date, opts => opts.MapFrom(src => src.DateBS.GetDate()))
            .ForMember(d => d.DueDate, opts => opts.MapFrom(src => src.DueDateBS.GetDate()));
            cfg.CreateMap<PurchaseOrderDetail, PurchaseOrderDetailViewModel>()
            .AfterMap((src, d) =>
            {
                d.ItemId = src.ItemId;
                d.ItemCode = src.Item.Code;
                d.ItemName = src.Item.Name;
                d.UnitName = src.Item.ItemUnit.Name;
                d.ItemRequestIdCSV = src.PurchaseOrder.ItemRequestDetails != null ? string.Join(",", src.PurchaseOrder.ItemRequestDetails.Select(x => x.ItemRequestId)) : "";
                d.ItemRequestDetailIdCSV = src.PurchaseOrder.ItemRequestDetails != null ? string.Join(",", src.PurchaseOrder.ItemRequestDetails.Select(x => x.Id)) : "";
                d.Specification = src.Remarks;
            });
            cfg.CreateMap<PurchaseOrderDetailViewModel, PurchaseOrderDetail>()
            .ForMember(d => d.Remarks, opts => opts.MapFrom(src => src.Specification));
            cfg.CreateMap<PurchaseOrder, PurchaseOrderListViewModel>()
            .AfterMap((s, d) =>
            {
                d.Vendor = s.Vendor.Name;
                d.DateBS = s.Date.GetMiti();
                d.NoOfItems = s.PurchaseOrderDetails.Sum(x => x.Qty);
                d.IsPurchased = s.StockTransactions.Any();
                d.ItemList = $"{s.PurchaseOrderDetails.FirstOrDefault().Item.Name} ";
                d.ItemList = s.PurchaseOrderDetails.Count > 1 ? $"{d.ItemList } And other..." : d.ItemList;
                d.FullEntry = s.PurchaseOrderDetails.Sum(y => y.Qty) == s.StockTransactions.Sum(y => y.StockTransactionDetails.Sum(z => z.Qty));
                d.AcceptedEmployeeName = s.AcceptedBy.HasValue ? s.Employee.Name : null;
                using (var db = new UnitOfWork())
                {
                    var creater = db.EmployeeRepo.Table.Where(x => x.Id == s.EmployeeId).FirstOrDefault();
                    if (creater != null)
                        d.CreatedEmployeeName = creater.Name;
                }
                d.AcceptedDateBs = s.AcceptedDate.GetMiti();
                if (s.PurchaseOrderDetails.Any())
                {
                    foreach (var item in s.PurchaseOrderDetails)
                    {
                        d.Details.Add(new PurchaseOrderDetailViewModel
                        {
                            ItemId = item.ItemId,
                            ItemName = item.Item.Name,
                            UnitName = item.Item.ItemUnit.Name,
                            Qty = item.Qty,
                            Specification = item.Remarks
                        });
                    }
                }

            });
            cfg.CreateMap<ItemRequestDetailViewModel, PurchaseOrderDetailViewModel>()
            .ForMember(d => d.Specification, opts => opts.MapFrom(src => src.Remarks));
            #endregion

            #region ItemRelease
            cfg.CreateMap<ItemRelease, ItemReleaseViewModel>()
            .ForMember(d => d.DateBS, opts => opts.MapFrom(src => src.Date.GetMiti()));
            cfg.CreateMap<ItemReleaseViewModel, ItemRelease>()
            .ForMember(d => d.Date, opts => opts.MapFrom(src => src.DateBS.GetDate()));
            cfg.CreateMap<ItemReleaseDetail, ItemReleaseDetailViewModel>()
            .AfterMap((src, d) =>
            {
                d.ItemId = src.ItemId;
                d.ItemCode = src.Item.Code;
                d.ItemName = src.Item.Name;
                d.UnitName = src.Item.ItemUnit.Name;
                d.ItemRequestIdCSV = src.ItemRelease.ItemRequestDetails != null ? string.Join(",", src.ItemRelease.ItemRequestDetails.Select(x => x.ItemRequestId)) : "";
                d.ItemRequestDetailIdCSV = src.ItemRelease.ItemRequestDetails != null ? string.Join(",", src.ItemRelease.ItemRequestDetails.Select(x => x.Id)) : "";
            });
            cfg.CreateMap<ItemReleaseDetailViewModel, ItemReleaseDetail>();
            cfg.CreateMap<ItemRelease, ItemReleaseListViewModel>()
            .AfterMap((src, d) =>
            {
                d.DisplayRequestNo = src.ItemRequest.DisplayDocumentNo;
                d.DateBS = src.Date.GetMiti();
                d.SectionName = src.Department.Name;
                d.EmployeeName = src.Employee.Name;
                d.ItemList = src.ItemReleaseDetails.FirstOrDefault().Item.Name;
                d.ItemList = src.ItemReleaseDetails.GroupBy(x => x.ItemId).Count() > 1 ? $"{d.ItemList } And other..." : d.ItemList;
            })
            .AfterMap((s, d) =>
            {
                d.IsAccepted = s.ItemRequest.ItemReceivedBy.HasValue;
            });
            cfg.CreateMap<ItemRequestDetailViewModel, ItemReleaseDetailViewModel>();
            #endregion

            #region PurchaseEntry
            cfg.CreateMap<StockTransaction, PurchaseEntryViewModel>()
            .ForMember(d => d.PurchaseOrderId, opts => opts.MapFrom(src => src.PurchaseOrderId))
            .ForMember(d => d.PurchaseOrderDisplayOrderNo, opts => opts.MapFrom(src => src.PurchaseOrderId.HasValue ? src.PurchaseOrder.DisplayDocumentNo : ""))
            .ForMember(d => d.PurchaseOrderDateBS, opts => opts.MapFrom(src => src.PurchaseOrderId.HasValue ? GetDateMiti.GetMiti(src.PurchaseOrder.Date) : ""))
            .ForMember(d => d.DateBS, opts => opts.MapFrom(src => src.Date.GetMiti()));
            cfg.CreateMap<PurchaseEntryViewModel, StockTransaction>()
            .ForMember(d => d.Date, opts => opts.MapFrom(src => src.DateBS.GetDate()));
            cfg.CreateMap<StockTransactionDetail, PurchaseEntryDetailViewModel>()
            .ForMember(d => d.ItemName, opts => opts.MapFrom(src => src.Item.Name))
            .ForMember(d => d.ItemCode, opts => opts.MapFrom(src => src.Item.Code))
            .ForMember(d => d.UnitName, opts => opts.MapFrom(src => src.Item.ItemUnit.Name))
            .ForMember(d => d.VatPerQty, opts => opts.MapFrom(src => src.Vat));
            cfg.CreateMap<PurchaseEntryDetailViewModel, StockTransactionDetail>()
            .ForMember(d => d.Vat, opts => opts.MapFrom(src => src.VatPerQty));

            cfg.CreateMap<StockTransaction, PurchaseEntryListViewModel>()
            .ForMember(d => d.NoOfItems, opts => opts.MapFrom(src => src.StockTransactionDetails.Count))
            .ForMember(d => d.DateBS, opts => opts.MapFrom(src => src.Date.GetMiti()))
            .AfterMap((s, d) =>
            {
                d.VendorName = s.Vendor != null ? s.Vendor.Name : null;
                if (s.StockTransactionDetails.Any())
                {
                    d.ItemList = $"{s.StockTransactionDetails.FirstOrDefault().Item.Name} ";
                    d.ItemList = s.StockTransactionDetails.GroupBy(x => x.ItemId).Count() > 1 ? $"{d.ItemList } And other..." : d.ItemList;
                }
            });
            #endregion PurchaseEntry

            #region StockAdjustment
            cfg.CreateMap<StockTransaction, StockAdjustmentViewModel>()
            .ForMember(d => d.DateBS, opts => opts.MapFrom(src => src.Date.GetMiti()));
            cfg.CreateMap<StockAdjustmentViewModel, StockTransaction>()
            .ForMember(d => d.Date, opts => opts.MapFrom(src => src.DateBS.GetDate()));

            cfg.CreateMap<StockTransactionDetail, StockAdjustmentDetailViewModel>().AfterMap((src, d) =>
            {
                d.ItemId = src.ItemId;
                d.ItemName = src.Item.Name;
                d.UnitName = src.Item.ItemUnit.Name;
                d.SectionName = src.Department.Name;
                d.EmployeeName = (src.EmployeeId.HasValue) ? src.Employee.Name : "";
                d.Narration = src.Remarks;
            });
            cfg.CreateMap<StockAdjustmentDetailViewModel, StockTransactionDetail>()
            .ForMember(d => d.Remarks, opts => opts.MapFrom(src => src.Narration));

            cfg.CreateMap<StockTransaction, StockAdjustmentListViewModel>()
            //.ForMember(d => d.AdjustmentTypeName, opts => opts.MapFrom(src => src.AdjustmentType.Name))
            .ForMember(d => d.NoOfItems, opts => opts.MapFrom(src => src.StockTransactionDetails.Count));
            #endregion StockAdjustment
            #endregion Ims

            
        }


    }
}