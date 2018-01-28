using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
    public class AppUserViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(400, MinimumLength = 1)]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed in {0}.")]
        [Display(Name = "User Name")]
        [Remote("UserNameNotExist", "Master", HttpMethod = "Post", AdditionalFields = "Id", ErrorMessage = "{0} already exists.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter {0}.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }
        public string EmployeeName { get; set; }
        [Display(Name ="Assign Role")]
        public int RoleId {get;set;}
        public string Photo { get; set; }
        public bool IsActive { get; set; }
    }
    public class AppUserListViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Department { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Photo { get; set; }
        public bool IsActive { get; set; }
    }
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage ="Pelase enter {0}")]
        [Display(Name ="Username or Email Address")]
        public string userId { get; set; }
    }
}
