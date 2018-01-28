using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static IMS.Data.NTAEnum;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class EmployeeProfileViewModel : ServiceModel
    {
        public EmployeeProfileViewModel()
        {
            this.Addresses = new List<EmployeeAddressProfileViewModel>();
            this.Nominees = new List<NomineeProfileViewModel>();
        }

        public int Id { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.Name", DefaultText = "Please enter full name.")]
        [LocalizedDisplayName("Employee.Name", DefaultText = "Full name")]
        public string Name { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.FullNameNP", DefaultText = "Please enter full name in nepali.")]
        [LocalizedDisplayName("Employee.FullNameNP", DefaultText = "Full name nepali")]
        public string FullNameNP { get; set; }

        [LocalizedDisplayName("Employee.DateOfBirth", DefaultText = "Dob")]
        public System.DateTime DateOfBirth { get; set; }

        [TRequired(ErrorMessageResourceName = "Employee.Required.DateOfBirth", DefaultText = "Enter date of birth (BS).")]
        [LocalizedDisplayName("Employee.DateOfBirth", DefaultText = "Dob")]
        public string DateOfBirthBS { get; set; }

        [TRequired(ErrorMessageResourceName = "Employee.Required.Gender", DefaultText = "Please select gender")]
        [LocalizedDisplayName("Common.Gender", DefaultText = "Gender")]
        public string Gender { get; set; }

        [LocalizedDisplayName("Employee.AppointmentDate", DefaultText = "Appointment date")]
        public System.DateTime AppointmentDate { get; set; }
        [TRequired(ErrorMessageResourceName = "Employee.Required.AppointmentDate", DefaultText = "Please enter appointment date (BS).")]
        [LocalizedDisplayName("Employee.AppointmentDate", DefaultText = "Appointment date (BS)")]
        public string AppointmentDateBS { get; set; }

        [LocalizedDisplayName("Employee.DateTo20YrsofServiceDuration", DefaultText = "Date to 20 year. of service dur.")]
        public System.DateTime DateTo20YrsofServiceDuration { get; set; }
        [TRequired(ErrorMessageResourceName = "Employee.Required.DateTo20YrsofServiceDuration", DefaultText = "Please enter Date to 20 year. of service dur. (BS).")]
        [LocalizedDisplayName("Employee.DateTo20YrsofServiceDuration", DefaultText = "Date to 20 year. of service dur. (BS)")]
        public string DateTo20YrsofServiceDurationBS { get; set; }

        [LocalizedDisplayName("Employee.DateTo58YrsOld", DefaultText = "Date to 58 yrs. old")]
        public System.DateTime DateTo58YrsOld { get; set; }
        [TRequired(ErrorMessageResourceName = "Employee.Required.DateTo58YrsOld", DefaultText = "Date to 58 yrs. old (BS).")]
        [LocalizedDisplayName("Employee.DateTo58YrsOld", DefaultText = "Date to 58 yrs. old (BS)")]
        public string DateTo58YrsOldBS { get; set; }

        [TRequired(ErrorMessageResourceName = "Employee.Required.FatherName", DefaultText = "Please enter father name")]
        [LocalizedDisplayName("Employee.FatherName", DefaultText = "Father name")]
        public string FatherName { get; set; }

        [TRequired(ErrorMessageResourceName = "Employee.Required.MotherName", DefaultText = "Please enter mother name")]
        [LocalizedDisplayName("Employee.MotherName", DefaultText = "Mother name")]
        public string MotherName { get; set; }

        [TRequired(ErrorMessageResourceName = "Employee.Required.GrandFatherName", DefaultText = "Please enter grandfather name")]
        [LocalizedDisplayName("Employee.GrandFatherName", DefaultText = "Grand father name")]
        public string GrandFatherName { get; set; }

        //[TRequired(ErrorMessageResourceName = "Employee.Required.SectionId", DefaultText = "Please select section")]
        [LocalizedDisplayName("Employee.SectionId", DefaultText = "Section")]
        public int SectionId { get; set; }

        [LocalizedDisplayName("Employee.SectionId", DefaultText = "Section")]
        public string SectionName { get; set; }

        //[TRequired(ErrorMessageResourceName = "Employee.Required.DesignationId", DefaultText = "Please select designation")]
        [LocalizedDisplayName("Employee.DesignationId", DefaultText = "Designation")]
        public int DesignationId { get; set; }

        [LocalizedDisplayName("Employee.DesignationId", DefaultText = "Designation")]
        public string DesignationName { get; set; }

        [LocalizedDisplayName("Employee.HomeTelephone", DefaultText = "Telephone (H)")]
        public string HomeTelephone { get; set; }

        [LocalizedDisplayName("Employee.OfficeTelephone", DefaultText = "Telephone (O)")]
        public string OfficeTelephone { get; set; }

        [LocalizedDisplayName("Employee.TelExtenstionNumber", DefaultText = "Telephone ext. no.")]
        public string TelExtenstionNumber { get; set; }

        [TRequired(ErrorMessageResourceName = "Employee.Required.Mobile", DefaultText = "Please enter mobile no")]
        [LocalizedDisplayName("Employee.Mobile", DefaultText = "Mobile no")]
        public string Mobile { get; set; }

        [TRequired(ErrorMessageResourceName = "Employee.Required.Email", DefaultText = "Please enter email address.")]
        [LocalizedDisplayName("Employee.Email", DefaultText = "Email address")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Not a valid email address.")]
        public string Email { get; set; }

        [LocalizedDisplayName("Employee.PANNumber", DefaultText = "PAN no")]
        public string PANNumber { get; set; }

        [LocalizedDisplayName("Employee.CitizenshipNumber", DefaultText = "Citizenship no")]
        public string CitizenshipNumber { get; set; }

        public Nullable<System.DateTime> CitizenshipIssuedDate { get; set; }
        [LocalizedDisplayName("Employee.CitizenshipIssuedDateBS", DefaultText = "Citizenship issue date (BS)")]
        public string CitizenshipIssuedDateBS { get; set; }

        [LocalizedDisplayName("Employee.CitizenshipIssuedDistrictId", DefaultText = "Citizenship issued district")]
        public string CitizenshipIssuedDistrictName { get; set; }

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

        [TRequired(ErrorMessageResourceName = "Employee.Required.MaritalStatus", DefaultText = "Please select marital status")]
        [LocalizedDisplayName("Employee.MaritalStatus", DefaultText = "Marital status")]
        public string MaritalStatus { get; set; }

        [LocalizedDisplayName("Employee.SupervisorId", DefaultText = "Supervisor")]
        public Nullable<int> SupervisorId { get; set; }

        [LocalizedDisplayName("Employee.SupervisorId", DefaultText = "Supervisor")]
        public string SupervisorName { get; set; }

        //[TRequired(ErrorMessageResourceName = "Employee.Required.EmployeeType", DefaultText = "Please select employee type.")]
        [LocalizedDisplayName("Employee.EmployeeType", DefaultText = "Employee type")]
        public string EmployeeType { get; set; }

        [LocalizedDisplayName("Employee.EmployeeStatus", DefaultText = "Status")]
        public string EmployeeStatus { get; set; }

        [LocalizedDisplayName("Employee.UserId", DefaultText = "User name")]
        public Nullable<int> UserId { get; set; }

        public HttpPostedFileBase ProfilePicture { get; set; }

        public List<EmployeeAddressProfileViewModel> Addresses { get; set; }

        public List<NomineeProfileViewModel> Nominees { get; set; }
    }

    public class EmployeeAddressProfileViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public eAddressType AddressType { get; set; }

        [LocalizedDisplayName("Employee.ZoneId", DefaultText = "Zone")]
        public string ZoneName { get; set; }

        [LocalizedDisplayName("Employee.DistrictId", DefaultText = "District")]
        public string DistrictName { get; set; }

        [LocalizedDisplayName("Employee.VdcId", DefaultText = "Vdc")]
        public string VdcName { get; set; }

        [LocalizedDisplayName("Employee.WardNo", DefaultText = "Ward no")]
        public string WardNo { get; set; }

        [LocalizedDisplayName("Employee.StreetAddress", DefaultText = "Street")]
        public string StreetAddress { get; set; }

        [LocalizedDisplayName("Employee.HouseNumber", DefaultText = "House no")]
        public string HouseNumber { get; set; }
    }

    public class NomineeProfileViewModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [LocalizedDisplayName("Employee.Nominee.Name", DefaultText = "Full name")]
        public string Name { get; set; }

        [LocalizedDisplayName("Employee.Nominee.RelationshipId", DefaultText = "Relationship")]
        public string RelationshipName { get; set; }

        [LocalizedDisplayName("Employee.Nominee.AuthorizedBy", DefaultText = "Authorized by")]
        public string AuthorizedByName { get; set; }

        //public HttpPostedFileBase NomineeProfilePhoto { get; set; }
        //public HttpPostedFileBase NomineeAuthorizationForm { get; set; }
    }
}
