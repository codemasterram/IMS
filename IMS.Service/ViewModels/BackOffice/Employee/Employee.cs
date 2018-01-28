using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.DataAnnotations;
using IMS.Data;

using static IMS.Data.NTAEnum;
using System.Web;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class EmployeeViewModel : ServiceModel
    {
        public EmployeeViewModel()
        {
            this.Addresses = new List<EmployeeAddressViewModel>();
            this.Nominees = new List<NomineeViewModel>();
        }

        public int Id { get; set; }
        public string EmployeeId { get; set; } = Guid.NewGuid().ToString();

        //[TRequired(ErrorMessageResourceName = "Employee.Required.StaffCode", DefaultText = "Please enter Staff Code.")]
        [LocalizedDisplayName("Employee.StaffCode", DefaultText = "Staff code no")]
        public string StaffCode { get; set; }

        [Display(Name = "Employee Id")]
        [Required]
        public int EmployeeNo { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.Name", DefaultText = "Full name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.FullNameNP", DefaultText = "Full name nepali")]
        public string FullNameNP { get; set; }

        [LocalizedDisplayName("Employee.DateOfBirth", DefaultText = "Dob")]
        public System.DateTime DateOfBirth { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.DateOfBirth", DefaultText = "Dob")]
        public string DateOfBirthBS { get; set; }

        [Required]
        [LocalizedDisplayName("Common.Gender", DefaultText = "Gender")]
        public string Gender { get; set; }

        [LocalizedDisplayName("Employee.AppointmentDate", DefaultText = "Appointment date")]
        public System.DateTime AppointmentDate { get; set; }
        [Required]
        [LocalizedDisplayName("Employee.AppointmentDate", DefaultText = "Appointment date (BS)")]
        public string AppointmentDateBS { get; set; }

        [LocalizedDisplayName("Employee.DateTo20YrsofServiceDuration", DefaultText = "Date to 20 year. of service dur.")]
        public System.DateTime DateTo20YrsofServiceDuration { get; set; }
        [LocalizedDisplayName("Employee.DateTo20YrsofServiceDuration", DefaultText = "Date to 20 year. of service dur. (BS)")]
        public string DateTo20YrsofServiceDurationBS { get; set; }

        [LocalizedDisplayName("Employee.DateTo58YrsOld", DefaultText = "Date to 58 yrs. old")]
        public System.DateTime DateTo58YrsOld { get; set; }
        //[TRequired(ErrorMessageResourceName = "Employee.Required.DateTo58YrsOld", DefaultText = "Date to 58 yrs. old (BS).")]
        [LocalizedDisplayName("Employee.DateTo58YrsOld", DefaultText = "Date to 58 yrs. old (BS)")]
        public string DateTo58YrsOldBS { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.FatherName", DefaultText = "Father's name")]
        public string FatherName { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.MotherName", DefaultText = "Mother's name")]
        public string MotherName { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.GrandFatherName", DefaultText = "Grand father's name")]
        public string GrandFatherName { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.SectionId", DefaultText = "Section")]
        public int SectionId { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.DesignationId", DefaultText = "Designation")]
        public int DesignationId { get; set; }

        [LocalizedDisplayName("Employee.HomeTelephone", DefaultText = "Telephone (H)")]
        public string HomeTelephone { get; set; }

        [LocalizedDisplayName("Employee.OfficeTelephone", DefaultText = "Telephone (O)")]
        public string OfficeTelephone { get; set; }

        [LocalizedDisplayName("Employee.TelExtenstionNumber", DefaultText = "Telephone ext. no.")]
        public string TelExtenstionNumber { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.Mobile", DefaultText = "Mobile no")]
        public string Mobile { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.Email", DefaultText = "Email address")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Not a valid email address.")]
        public string Email { get; set; }

        [LocalizedDisplayName("Employee.PANNumber", DefaultText = "PAN no")]
        public string PANNumber { get; set; }

        [LocalizedDisplayName("Employee.CitizenshipNumber", DefaultText = "Citizenship no")]
        public string CitizenshipNumber { get; set; }

        public Nullable<System.DateTime> CitizenshipIssuedDate { get; set; }
        [LocalizedDisplayName("Employee.CitizenshipIssuedDateBS", DefaultText = "Citizenship issued date (BS)")]
        public string CitizenshipIssuedDateBS { get; set; }

        [LocalizedDisplayName("Employee.CitizenshipIssuedDistrictId", DefaultText = "Citizenship issued district")]
        public Nullable<int> CitizenshipIssuedDistrictId { get; set; }

        [LocalizedDisplayName("Employee.PassportNumber", DefaultText = "Passport no")]
        public string PassportNumber { get; set; }

        public Nullable<System.DateTime> PassportIssuedDate { get; set; }
        [LocalizedDisplayName("Employee.PassportIssuedDateBS", DefaultText = "Passport issued date (BS)")]
        public string PassportIssuedDateBS { get; set; }

        [LocalizedDisplayName("Employee.DriverLicenseNumber", DefaultText = "Driving license no")]
        public string DriverLicenseNumber { get; set; }

        public Nullable<System.DateTime> DriverLicenseIssuedDate { get; set; }
        [LocalizedDisplayName("Employee.DriverLicenseIssuedDateBS", DefaultText = "Driving license issued date (BS)")]
        public string DriverLicenseIssuedDateBS { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.MaritalStatus", DefaultText = "Marital status")]
        public string MaritalStatus { get; set; }

        [LocalizedDisplayName("Employee.SupervisorId", DefaultText = "Supervisor")]
        public Nullable<int> SupervisorId { get; set; }

        [Required]
        [LocalizedDisplayName("Employee.EmployeeType", DefaultText = "Employee type")]
        public string EmployeeType { get; set; }

        [LocalizedDisplayName("Employee.EmployeeStatus", DefaultText = "Status")]
        public string EmployeeStatus { get; set; }

        [LocalizedDisplayName("Employee.UserId", DefaultText = "User name")]
        public Nullable<int> UserId { get; set; }

        public HttpPostedFileBase ProfilePicture { get; set; }

        public List<EmployeeAddressViewModel> Addresses { get; set; }

        public List<NomineeViewModel> Nominees { get; set; }
    }

    public class EmployeeListViewModel
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public int EmployeeNo { get; set; }
        public string FullNameNP { get; set; }
        public string EmployeeType { get; set; }
        public string DesignationName { get; set; }
        public string SectionName { get; set; }
        public string Mobile { get; set; }
        public string UserName { get; set; }
        public int? UserId { get; set; }
        public string Email { get; set; }
        public string EmployeeStatus { get; set; }
    }
}
