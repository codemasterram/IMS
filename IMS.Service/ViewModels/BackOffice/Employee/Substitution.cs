using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class SubstitutionViewModel : ServiceModel
    {
        public int Id { get; set; }

        [TRequired(ErrorMessageResourceName = "Substitution.Required.SubstitutionTypeId", DefaultText = "Please select substitution type.")]
        [LocalizedDisplayName("Substitution.SubstitutionType", DefaultText = "Substitution type")]
        public int SubstitutionTypeId { get; set; }

        [TRequired(ErrorMessageResourceName = "Substitution.Required.EmployeeToSubstitute", DefaultText = "Please select employee to substitute.")]
        [LocalizedDisplayName("Substitution.EmployeeToSubstitute", DefaultText = "Employee to substitute")]
        public int EmployeeToSubstitute { get; set; }

        [TRequired(ErrorMessageResourceName = "Substitution.Required.SubstitutedEmployee", DefaultText = "Please select Substituted employee.")]
        [LocalizedDisplayName("Substitution.SubstitutedEmployee", DefaultText = "Substituted employee")]
        public int SubstitutedEmployee { get; set; }

        [LocalizedDisplayName("Substitution.EffectiveFromDate", DefaultText = "Effective from date")]
        public System.DateTime EffectiveFromDate { get; set; }

        [TRequired(ErrorMessageResourceName = "Substitution.Required.EffectiveFromDateBS", DefaultText = "Please enter effective from date (BS).")]
        [LocalizedDisplayName("Substitution.EffectiveFromDate", DefaultText = "Effective from date (BS)")]
        public string EffectiveFromDateBS { get; set; }

        [LocalizedDisplayName("Substitution.EffectiveToDate", DefaultText = "Effective to date")]
        public System.DateTime EffectiveToDate { get; set; }

        [TRequired(ErrorMessageResourceName = "Substitution.Required.EffectiveToDateBS", DefaultText = "Please enter effective to date (BS).")]
        [LocalizedDisplayName("Substitution.EffectiveToDate", DefaultText = "Effective to date (BS)")]
        public string EffectiveToDateBS { get; set; }

        [LocalizedDisplayName("Substitution.Remarks", DefaultText = "Remarks")]
        public string Remarks { get; set; }
    }

    public class SubstitutionListViewModel
    {
        public int Id { get; set; }
        public string SubstitutionTypeName { get; set; }
        public string EmployeeToSubstituteName { get; set; }
        public string SubstitutedEmployeeName { get; set; }
        public string EffectiveFromDateBS { get; set; }
        public string EffectiveToDateBS { get; set; }
        public string Remarks { get; set; }
    }
}
