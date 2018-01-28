using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.Configuration;
using IMS.Logic.Contract;
using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.User;
using IMS.Logic.ViewModels.BackOffice.Common;

namespace IMS.Logic.Implementation
{
    public class AuthService : IAuthService
    {
        #region Constructor
        private ILocalizationService localizationService;
        private IRepository<AppUser> appUserRepository;
        private IRepository<UserRole> userRoleRepository;
        private IRepository<Role> roleRepository;
        private IRepository<Permission> permissionRepository;
        private IRepository<RolePermission> rolepermissionRepository;
        private IRepository<LoginHistory> loginHistoryRepository;
        private IRepository<Employee> employeeRepository;
        public AuthService(ILocalizationService localizationSvc,
            IRepository<Permission> permissionRepo,
            IRepository<AppUser> appUserRepo,
            IRepository<UserRole> userRoleRepo,
            IRepository<Role> roleRepo,
            IRepository<RolePermission> rolepermissionRepo,
            IRepository<LoginHistory> loginHistoryRepo,
            IRepository<Employee> employeeRepo
            )
        {
            this.localizationService = localizationSvc;

            this.appUserRepository = appUserRepo;
            this.userRoleRepository = userRoleRepo;
            this.roleRepository = roleRepo;
            this.permissionRepository = permissionRepo;
            this.rolepermissionRepository = rolepermissionRepo;
            this.loginHistoryRepository = loginHistoryRepo;
            this.employeeRepository = employeeRepo;
        }
        #endregion

        public string ChangeLanguage(int userId, string lang)
        {
            //var entity = appUserRepository.GetById(userId);
            //entity.SelectedLanguage = lang;
            //appUserRepository.Update(entity);
            return "success";
        }

        public AppUserViewModel GetById(int id)
        {
            var user = appUserRepository.GetById(id);
            return user == null ? null : AutomapperConfig.Mapper.Map<AppUserViewModel>(user);
        }

        public string[] GetRoles(string username = null)
        {
            return username != null ? userRoleRepository.Table.Where(x => x.AppUser.UserName == username).Select(x => x.Role.Name).ToArray() : roleRepository.Table.Select(x => x.Name).ToArray();
        }

        public AppUserViewModel GetUser(string username, string role = null)
        {
            var user = appUserRepository.Table.Where(x => x.UserName == username).FirstOrDefault();
            if (user != null && !string.IsNullOrEmpty(role) && !user.UserRoles.Any(x => x.Role.Name == role))
                return null;

            if (user == null)
                return null;

            return AutomapperConfig.Mapper.Map<AppUserViewModel>(user);
        }

        public IEnumerable<AppUserViewModel> GetUsers(string role = null)
        {
            IQueryable<AppUser> users;
            if (string.IsNullOrEmpty(role))
            {
                users = appUserRepository.Table;
            }

            users = userRoleRepository.Table.Where(x => x.Role.Name == role).Select(x => x.AppUser);
            return AutomapperConfig.Mapper.Map<IEnumerable<AppUserViewModel>>(users);
        }


        public bool IsAuthorized(string username, string module, string permission = null)
        {
            var userRoles = userRoleRepository.Table.Where(x => x.AppUser.UserName == username);

            if (!string.IsNullOrEmpty(permission))
                userRoles = userRoles.Where(x => x.Role.RolePermissions.Any(x1=>x1.Permission.Name == permission));

            return userRoles.Any();
        }

        public bool IsAuthorized(string username, int[] permissions)
        {
            var userRoles = from r in userRoleRepository.Table.Where(x => x.AppUser.UserName == username)
                            join rp in rolepermissionRepository.Table on r.RoleId equals rp.RoleId
                            join p in permissions on rp.PermissionId equals p
                            select r;

            return userRoles.Any();
        }


        public AppUserViewModel Login(string username, string password)
        {
            EncryptionService encSvc = new EncryptionService();
            var user = appUserRepository.Table.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                var entityUnsuccessLog = AutomapperConfig.Mapper.Map<LoginHistory>(new LoginHistoryViewModel() { UserName = username, IsLoginSuccessful = false });
                loginHistoryRepository.Insert(entityUnsuccessLog);
                return null;
            }

            //check password
            string pass = encSvc.CreatePasswordHash(password, user.Salt);
            if (user.Password != pass)
            {
                var entityUnsuccessLog = AutomapperConfig.Mapper.Map<LoginHistory>(new LoginHistoryViewModel() { UserName = username, IsLoginSuccessful = false });
                loginHistoryRepository.Insert(entityUnsuccessLog);
                return null;
            }
            var entitySuccessLog = AutomapperConfig.Mapper.Map<LoginHistory>(new LoginHistoryViewModel() { UserName = username, IsLoginSuccessful = true });
            loginHistoryRepository.Insert(entitySuccessLog);
            return AutomapperConfig.Mapper.Map<AppUserViewModel>(user);
        }

        public string UserPhoto(int userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PermissionModel> GetPermissions(int roleId)
        {
            var list = new List<PermissionModel>();

            var permissions = permissionRepository.Table;
            var roleModulePermissions = roleModulePermissionRepository.Table.Where(x => x.RoleId == roleId).Select(x => x.PermissionId).ToList();

            foreach (var permission in permissions)
            {
                var p = AutomapperConfig.Mapper.Map<PermissionModel>(permission);
                p.RoleId = roleId;
                p.IsChecked = roleModulePermissions.Any(x => x == permission.Id);

                list.Add(p);
            }

            return list;
        }

        public IEnumerable<RoleModel> GetAllRoles()
        {
            var query = roleRepository.Table.OrderBy(x => x.DisplayOrder);
            var list = AutomapperConfig.Mapper.Map<IEnumerable<RoleModel>>(query);
            return list;
        }

        public ServiceModel SaveRolePermission(int roleId, string permissionIdCSV)
        {
            var result = new ServiceModel();
            var query = roleModulePermissionRepository.Table.Where(x => x.RoleId == roleId);
            roleModulePermissionRepository.DeleteRange(query);

            if (string.IsNullOrEmpty(permissionIdCSV))
                return result;

            string[] s = permissionIdCSV.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (s.Any())
            {
                List<RoleModulePermission> list = new List<RoleModulePermission>();
                foreach (string pid in s)
                {
                    list.Add(new RoleModulePermission
                    {
                        RoleId = roleId,
                        PermissionId = Convert.ToInt32(pid)
                    });
                }

                roleModulePermissionRepository.InsertRange(list, 1000);
            }

            return result;
        }

        public List<RoleListViewModel> RoleList()
        {
            var query = roleRepository.Table;
            List<RoleListViewModel> model = new List<RoleListViewModel>();
            foreach (var item in query)
            {
                model.Add(AutomapperConfig.Mapper.Map<RoleListViewModel>(item));
            }
            return model;
        }

        public LoginEmployee LoginEmployee(int userId)
        {
            var data = employeeRepository.Table.Where(x => x.UserId == userId).FirstOrDefault();
            return AutomapperConfig.Mapper.Map<LoginEmployee>(data);
        }

        public LoginCustomer LoginCustomer(int userId)
        {
            var data = customerRepository.Table.Where(x => x.Id == userId).FirstOrDefault();
            return AutomapperConfig.Mapper.Map<LoginCustomer>(data);
        }

        public IEnumerable<ModuleViewModel> GetModules(bool forClient)
        {
            var data = moduleRepository.Table.Where(x => x.IsForClient == forClient);
            return AutomapperConfig.Mapper.Map<List<ModuleViewModel>>(data);
        }
    }
}
