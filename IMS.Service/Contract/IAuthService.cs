using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.ViewModels.BackOffice.User;
using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.Common;
using IMS.Logic.ViewModels.Public;

namespace IMS.Logic.Contract
{
    public interface IAuthService
    {
        AppUserViewModel GetUser(string username, string role = null);
        string[] GetRoles(string username = null);
        IEnumerable<AppUserViewModel> GetUsers(string role = null);
        AppUserViewModel Login(string username, string password);
        string ChangeLanguage(int userId, string lang);
        AppUserViewModel GetById(int id);
        string UserPhoto(int userId);

        bool IsAuthorized(string username, string module, string permission);
        bool IsAuthorized(string username, int[] permissions);


        LoginEmployee LoginEmployee(int userId);
        LoginCustomer LoginCustomer(int userId);
        IEnumerable<ModuleViewModel> GetModules(bool forClient = false);
        IEnumerable<RoleModel> GetAllRoles();
        List<RoleListViewModel> RoleList();
        IEnumerable<PermissionModel> GetPermissions(int roleId);
        ServiceModel SaveRolePermission(int roleId, string permissionIdCSV);
    }
}
