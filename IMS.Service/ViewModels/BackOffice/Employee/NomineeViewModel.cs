using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class NomineeViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Nominee name")]
        public string Name { get; set; }

        [LocalizedDisplayName("Employee.Nominee.RelationshipId", DefaultText = "Relationship")]
        public int? RelationshipId { get; set; }

        [LocalizedDisplayName("Employee.Nominee.AuthorizedBy", DefaultText = "Authorized by")]
        public int? AuthorizedBy { get; set; }

        public HttpPostedFileBase NomineeProfilePhoto { get; set; }
        public HttpPostedFileBase NomineeAuthorizationForm { get; set; }
    }
}
