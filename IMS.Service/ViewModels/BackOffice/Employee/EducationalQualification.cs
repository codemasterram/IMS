using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public partial class EducationalQualificationViewModel : ServiceModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.Degree", DefaultText = "Please enter degree.")]
        [LocalizedDisplayName("Degree", DefaultText = "Degree")]
        public int DegreeId { get; set; }

        [LocalizedDisplayName("MajorSubjects", DefaultText = "Major subjects")]
        public string MajorSubjects { get; set; }

        [LocalizedDisplayName("StartYear", DefaultText = "Start year")]
        public string StartYear { get; set; }

        [LocalizedDisplayName("EndYear", DefaultText = "End year")]
        public string EndYear { get; set; }

        [LocalizedDisplayName("Division", DefaultText = "Passed division")]
        public string Division { get; set; }

        [LocalizedDisplayName("TeachingInstitutionName", DefaultText = "Teaching institution name")]
        public string TeachingInstitutionName { get; set; }

        [LocalizedDisplayName("TeachingInstitutionAddress", DefaultText = "Teaching institution's address")]
        public string TeachingInstitutionAddress { get; set; }

        [LocalizedDisplayName("Remarks", DefaultText = "Remarks")]
        public string Remarks { get; set; }


        public byte[] Certificate { get; set; }
    }

    public partial class EducationalQualificationListViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string DegreeName { get; set; }
        public string MajorSubjects { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public string Division { get; set; }
        public string TeachingInstitutionName { get; set; }
        public string TeachingInstitutionAddress { get; set; }
        public string Remarks { get; set; }
    }
}
