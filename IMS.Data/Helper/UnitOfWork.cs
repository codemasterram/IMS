using IMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Data
{
    public class UnitOfWork : IDisposable
    {
        #region Common
        private IDbContext _context;
        public UnitOfWork()
        {
            _context = new IMSDbContext();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public int SaveChanges(out string error)
        {
            try
            {
                error = null;
                return _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var newException = new FormattedDbEntityValidationException(e);
                error = newException.Message;
                throw newException;
            }
        }
        #endregion
        #region PurchaseOrderDetail
        private IRepository<PurchaseOrderDetail> _PurchaseOrderDetailRepo;
        public IRepository<PurchaseOrderDetail> PurchaseOrderDetailRepo => _PurchaseOrderDetailRepo ?? (_PurchaseOrderDetailRepo = new EfRepository<PurchaseOrderDetail>(_context));
        #endregion
        #region StockTransactionDetail
        private IRepository<StockTransactionDetail> _StockTransactionDetailRepo;
        public IRepository<StockTransactionDetail> StockTransactionDetailRepo => _StockTransactionDetailRepo ?? (_StockTransactionDetailRepo = new EfRepository<StockTransactionDetail>(_context));
        #endregion
        #region Employee
        private IRepository<Employee> _EmployeeRepo;
        public IRepository<Employee> EmployeeRepo => _EmployeeRepo ?? (_EmployeeRepo = new EfRepository<Employee>(_context));
        #endregion
        #region DepreciationRate
        private IRepository<DepreciationRate> _DepreciationRateRepo;
        public IRepository<DepreciationRate> DepreciationRateRepo => _DepreciationRateRepo ?? (_DepreciationRateRepo = new EfRepository<DepreciationRate>(_context));
        #endregion
        #region DocumentNumbering
        private IRepository<DocumentNumbering> _DocumentNumberingRepo;
        public IRepository<DocumentNumbering> DocumentNumberingRepo => _DocumentNumberingRepo ?? (_DocumentNumberingRepo = new EfRepository<DocumentNumbering>(_context));
        #endregion
        #region DocumentSetup
        private IRepository<DocumentSetup> _DocumentSetupRepo;
        public IRepository<DocumentSetup> DocumentSetupRepo => _DocumentSetupRepo ?? (_DocumentSetupRepo = new EfRepository<DocumentSetup>(_context));
        #endregion
        #region Item
        private IRepository<Item> _ItemRepo;
        public IRepository<Item> ItemRepo => _ItemRepo ?? (_ItemRepo = new EfRepository<Item>(_context));
        #endregion
        #region FiscalYear
        private IRepository<FiscalYear> _FiscalYearRepo;
        public IRepository<FiscalYear> FiscalYearRepo => _FiscalYearRepo ?? (_FiscalYearRepo = new EfRepository<FiscalYear>(_context));
        #endregion
        #region PasswordResetLog
        private IRepository<PasswordResetLog> _PasswordResetLogRepo;
        public IRepository<PasswordResetLog> PasswordResetLogRepo => _PasswordResetLogRepo ?? (_PasswordResetLogRepo = new EfRepository<PasswordResetLog>(_context));
        #endregion
        #region ItemReleaseDetail
        private IRepository<ItemReleaseDetail> _ItemReleaseDetailRepo;
        public IRepository<ItemReleaseDetail> ItemReleaseDetailRepo => _ItemReleaseDetailRepo ?? (_ItemReleaseDetailRepo = new EfRepository<ItemReleaseDetail>(_context));
        #endregion
        #region Role
        private IRepository<Role> _RoleRepo;
        public IRepository<Role> RoleRepo => _RoleRepo ?? (_RoleRepo = new EfRepository<Role>(_context));
        #endregion
        #region Vendor
        private IRepository<Vendor> _VendorRepo;
        public IRepository<Vendor> VendorRepo => _VendorRepo ?? (_VendorRepo = new EfRepository<Vendor>(_context));
        #endregion
        #region RolePermission
        private IRepository<RolePermission> _RolePermissionRepo;
        public IRepository<RolePermission> RolePermissionRepo => _RolePermissionRepo ?? (_RolePermissionRepo = new EfRepository<RolePermission>(_context));
        #endregion
        #region UserRole
        private IRepository<UserRole> _UserRoleRepo;
        public IRepository<UserRole> UserRoleRepo => _UserRoleRepo ?? (_UserRoleRepo = new EfRepository<UserRole>(_context));
        #endregion
        #region Permission
        private IRepository<Permission> _PermissionRepo;
        public IRepository<Permission> PermissionRepo => _PermissionRepo ?? (_PermissionRepo = new EfRepository<Permission>(_context));
        #endregion
        #region AppUser
        private IRepository<AppUser> _AppUserRepo;
        public IRepository<AppUser> AppUserRepo => _AppUserRepo ?? (_AppUserRepo = new EfRepository<AppUser>(_context));
        #endregion
        #region ItemRequestDetail
        private IRepository<ItemRequestDetail> _ItemRequestDetailRepo;
        public IRepository<ItemRequestDetail> ItemRequestDetailRepo => _ItemRequestDetailRepo ?? (_ItemRequestDetailRepo = new EfRepository<ItemRequestDetail>(_context));
        #endregion
        #region ItemRequest
        private IRepository<ItemRequest> _ItemRequestRepo;
        public IRepository<ItemRequest> ItemRequestRepo => _ItemRequestRepo ?? (_ItemRequestRepo = new EfRepository<ItemRequest>(_context));
        #endregion
        #region LoginHistory
        private IRepository<LoginHistory> _LoginHistoryRepo;
        public IRepository<LoginHistory> LoginHistoryRepo => _LoginHistoryRepo ?? (_LoginHistoryRepo = new EfRepository<LoginHistory>(_context));
        #endregion
        #region Department
        private IRepository<Department> _DepartmentRepo;
        public IRepository<Department> DepartmentRepo => _DepartmentRepo ?? (_DepartmentRepo = new EfRepository<Department>(_context));
        #endregion
        #region Designation
        private IRepository<Designation> _DesignationRepo;
        public IRepository<Designation> DesignationRepo => _DesignationRepo ?? (_DesignationRepo = new EfRepository<Designation>(_context));
        #endregion
        #region ItemRelease
        private IRepository<ItemRelease> _ItemReleaseRepo;
        public IRepository<ItemRelease> ItemReleaseRepo => _ItemReleaseRepo ?? (_ItemReleaseRepo = new EfRepository<ItemRelease>(_context));
        #endregion
        #region Company
        private IRepository<Company> _CompanyRepo;
        public IRepository<Company> CompanyRepo => _CompanyRepo ?? (_CompanyRepo = new EfRepository<Company>(_context));
        #endregion
        #region PurchaseOrder
        private IRepository<PurchaseOrder> _PurchaseOrderRepo;
        public IRepository<PurchaseOrder> PurchaseOrderRepo => _PurchaseOrderRepo ?? (_PurchaseOrderRepo = new EfRepository<PurchaseOrder>(_context));
        #endregion
        #region SystemConfiguration
        private IRepository<SystemConfiguration> _SystemConfigurationRepo;
        public IRepository<SystemConfiguration> SystemConfigurationRepo => _SystemConfigurationRepo ?? (_SystemConfigurationRepo = new EfRepository<SystemConfiguration>(_context));
        #endregion
        #region StockTransaction
        private IRepository<StockTransaction> _StockTransactionRepo;
        public IRepository<StockTransaction> StockTransactionRepo => _StockTransactionRepo ?? (_StockTransactionRepo = new EfRepository<StockTransaction>(_context));
        #endregion
        #region ItemGroup
        private IRepository<ItemGroup> _ItemGroupRepo;
        public IRepository<ItemGroup> ItemGroupRepo => _ItemGroupRepo ?? (_ItemGroupRepo = new EfRepository<ItemGroup>(_context));
        #endregion
        #region ItemType
        private IRepository<ItemType> _ItemTypeRepo;
        public IRepository<ItemType> ItemTypeRepo => _ItemTypeRepo ?? (_ItemTypeRepo = new EfRepository<ItemType>(_context));
        #endregion
        #region ItemUnit
        private IRepository<ItemUnit> _ItemUnitRepo;
        public IRepository<ItemUnit> ItemUnitRepo => _ItemUnitRepo ?? (_ItemUnitRepo = new EfRepository<ItemUnit>(_context));
        #endregion


        public void Dispose()
        {
            _context.Dispose();
        }

       
    }

    public class FormattedDbEntityValidationException : Exception
    {
        public FormattedDbEntityValidationException(DbEntityValidationException innerException) :
            base(null, innerException)
        {
        }

        public override string Message
        {
            get
            {
                var innerException = InnerException as DbEntityValidationException;
                if (innerException != null)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine();
                    foreach (var eve in innerException.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("<b>Database error... </b> {0}", ve.ErrorMessage));
                        }
                    }
                    sb.AppendLine();

                    return sb.ToString();
                }
                return base.Message;
            }
        }
    }
}
