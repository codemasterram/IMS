using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.User;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.Contract
{
    public interface IUserService
    {
        UserViewModel SaveUser(UserViewModel model);
        UserViewModel GetUser(int UserId);
        IList<UserListViewModel> GetUsers(string userName);
        ServiceModel DeleteUser(int id);

        ChangePasswordViewModel ChangePassword(int userId, ChangePasswordViewModel model);
    }
}
