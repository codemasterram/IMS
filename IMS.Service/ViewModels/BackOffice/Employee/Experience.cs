using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class ExperienceViewModel : ServiceModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.OfficeName", DefaultText = "Please enter office name.")]
        [LocalizedDisplayName("OfficeName", DefaultText = "Office name")]
        public string OfficeName { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.DesignationId", DefaultText = "Please select designation.")]
        [LocalizedDisplayName("Employee.Experience.DesignationId", DefaultText = "Designation")]
        public int DesignationId { get; set; }

        [LocalizedDisplayName("Employee.Experience.ServiceClassId", DefaultText = "Service class")]
        public Nullable<int> ServiceClassId { get; set; }

        public Nullable<System.DateTime> AppointedDate { get; set; }
        [LocalizedDisplayName("Employee.Experience.AppointedDateBS", DefaultText = "Appointed date (BS)")]
        public string AppointedDateBS { get; set; }

        public Nullable<System.DateTime> SeparationDate { get; set; }
        [LocalizedDisplayName("Employee.Experience.SeparationDateBS", DefaultText = "Separation date (BS)")]
        public string SeparationDateBS { get; set; }

        [LocalizedDisplayName("Employee.Experience.AuthorizedBy", DefaultText = "Authorized by")]
        public Nullable<int> AuthorizedBy { get; set; }

        public byte[] ExperienceVerificationForm { get; set; }
    }

    public class ExperienceListViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string OfficeName { get; set; }
        public string DesignationName { get; set; }
        public string ServiceClassName { get; set; }
        public string AppointedDateBS { get; set; }
        public string SeparationDateBS { get; set; }
        public string AuthorizedByName { get; set; }
    }
}
