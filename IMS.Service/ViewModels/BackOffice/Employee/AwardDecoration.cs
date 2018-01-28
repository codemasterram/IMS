using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class AwardDecorationViewModel : ServiceModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.AwardDecorationTypeId", DefaultText = "Please select award/decoration type.")]
        [LocalizedDisplayName("Employee.AwardDecorationTypeId", DefaultText = "Award/decoration type")]
        public int AwardDecorationTypeId { get; set; }

        [LocalizedDisplayName("Description", DefaultText = "Description")]
        public string Description { get; set; }

        public Nullable<System.DateTime> ReceivedDate { get; set; }
        [LocalizedDisplayName("ReceivedDateBS", DefaultText = "Received date (BS)")]
        public string ReceivedDateBS { get; set; }

        [LocalizedDisplayName("ReasonToReceiveAward", DefaultText = "Reason")]
        public string ReasonToReceiveAward { get; set; }

        [LocalizedDisplayName("Benefit", DefaultText = "Benefit")]
        public string Benefit { get; set; }

        [LocalizedDisplayName("AwardedBy", DefaultText = "Awarded by")]
        public string AwardedBy { get; set; }

        [LocalizedDisplayName("Remarks", DefaultText = "Remarks")]
        public string Remarks { get; set; }
    }

    public class AwardDecorationListViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string AwardDecorationTypeName { get; set; }
        public string Description { get; set; }
        public string ReceivedDateBS { get; set; }
        public string ReasonToReceiveAward { get; set; }
        public string Benefit { get; set; }
        public string AwardedBy { get; set; }
        public string Remarks { get; set; }
    }
}
