using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.DataAnnotations;
using System.ComponentModel;

namespace IMS.Logic.ViewModels.BackOffice.User
{
    public class AppUserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public string Name { get; set; }
        public string FullNameNP { get; set; }
        public string Email { get; set; }
        public int? EmployeeId { get; set; }

        public int UserRoleId { get; set; }
        public string UserRoleAlias { get; set; }
        public bool IsFirstLogin { get; set; }
    }

    public class ChangePasswordViewModel : ServiceModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Please enter {0}.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplayName("ConfirmPassword", DefaultText = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirm password not matched.")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }

    }

    public class UserViewModel : ServiceModel
    {
        public int UserId { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.Username", DefaultText = "Please enter username.")]
        [LocalizedDisplayName("Username", DefaultText = "Username")]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed in {0}.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} must be at least 3 characters long.")]
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public string Salt { get; set; }
        public bool IsActive { get; set; } = true;

        [TRequired(ErrorMessageResourceName = "Required.Employee", DefaultText = "Please select employee for this user.")]
        [LocalizedDisplayName("Employee", DefaultText = "Employee")]
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName("Designation", DefaultText = "Designation")]
        public string DesignationName { get; set; }

        [LocalizedDisplayName("Role", DefaultText = "Role")]
        public int?[] Roles { get; set; }
    }

    public class UserRegistrationViewModel : ServiceModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class UserListViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string Active { get; set; }
        public string RoleName { get; set; }
    }

    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter {0}.")]
        [Display(Name = "Username/Email Address")]
        public string UserId { get; set; }
    }
}
