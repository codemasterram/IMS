using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using IMS.Logic.DataAnnotations;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class FamilyViewModel : ServiceModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.Name", DefaultText = "Please enter full name.")]
        [LocalizedDisplayName("Name", DefaultText = "Full name")]
        public string Name { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.RelationshipId", DefaultText = "Please select relationship.")]
        [LocalizedDisplayName("Employee.Family.RelationshipId", DefaultText = "Relationship")]
        public int RelationshipId { get; set; }

        [LocalizedDisplayName("Age", DefaultText = "Age")]
        [Range(minimum: 1, maximum: 120, ErrorMessage = "The {0} must between {1} to {2}.")]
        public int? Age { get; set; }

        [LocalizedDisplayName("Employee.Family.ProfessionId", DefaultText = "Profession")]
        public Nullable<int> ProfessionId { get; set; }

        [LocalizedDisplayName("Employee.Family.OfficeName", DefaultText = "Office name")]
        public string OfficeName { get; set; }

        [LocalizedDisplayName("Employee.Family.DesignationId", DefaultText = "Designation")]
        public Nullable<int> DesignationId { get; set; }

        public Nullable<System.DateTime> JoinedDate { get; set; }
        [LocalizedDisplayName("Employee.Family.JoindeDateBS", DefaultText = "Joined date (BS)")]
        public string JoinedDateBS { get; set; }

        public Nullable<System.DateTime> RetirementDate { get; set; }
        [LocalizedDisplayName("Employee.Family.RetirementDateBS", DefaultText = "Retirement date (BS)")]
        public string RetirementDateBS { get; set; }

    }

    public class FamilyListViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string RelationshipName { get; set; }
        public string ProfessionName { get; set; }
        public string DesignationName { get; set; }
        public string OfficeName { get; set; }
        public string JoinedDateBS { get; set; }
        public string RetirementDateBS { get; set; }
        public int? Age { get; set; }
    }
}
