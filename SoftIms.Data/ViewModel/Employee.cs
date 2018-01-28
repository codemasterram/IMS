using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 1)]
        [Display(Name = "Employee Name")]
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        [StringLength(50, MinimumLength = 1)]
        public string Gender { get; set; }
        public int DepartmentId { get; set; }
        [Display(Name = "Designation")]
        public int DesignationId { get; set; }
        [StringLength(400, MinimumLength = 5)]
        public string ContactNo { get; set; }
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Not a valid email address.")]
        public string Email { get; set; }
        [Display(Name = "Employee Type")]
        public string EmployeeType { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class EmployeeListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string EmployeeType { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByEmployeeName { get; set; }
        public string CreatedMiti { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}
