using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.User
{
    public class CreateUserModel : ServiceModel
    {
        [TRequired(ErrorMessageResourceName = "ErrorMessage.UsernameIsRequired", DefaultText = "Please enter {0}.")]
        [LocalizedDisplayName("User.Username", DefaultText = "Username")]
        public string Username { get; set; }

        [TRequired(ErrorMessageResourceName = "ErrorMessage.PasswordIsRequired", DefaultText = "Please enter {0}.")]
        [LocalizedDisplayName("User.Password", DefaultText = "Password")]
        public string Password { get; set; }

        [Compare("Password")]
        [LocalizedDisplayName("User.ConfirmPassword", DefaultText = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [TRequired(ErrorMessageResourceName = "ErrorMessage.UserCreate.EmployeeIsRequired", DefaultText = "Please select the employee.")]
        [LocalizedDisplayName("User.Employee", DefaultText = "Employee")]
        public int EmployeeId { get; set; }
        public int[] Roles { get; set; }
    }
}
