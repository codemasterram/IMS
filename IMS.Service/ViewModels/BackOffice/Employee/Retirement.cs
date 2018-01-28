using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.DataAnnotations;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class RetirementViewModel : ServiceModel
    {
        public int Id { get; set; }

        [TRequired(ErrorMessageResourceName = "Retirement.Required.EmployeeId", DefaultText = "Please select employee.")]
        [LocalizedDisplayName("Retirement.Employee", DefaultText = "Employee")]
        public int EmployeeId { get; set; }

        [TRequired(ErrorMessageResourceName = "Retirement.Required.RetirementReasonId", DefaultText = "Please select retirement reason.")]
        [LocalizedDisplayName("Retirement.RetirementReason", DefaultText = "Retirement Reason")]
        public int RetirementReasonId { get; set; }

        [LocalizedDisplayName("Retirement.RetirementDate", DefaultText = "Retirement Date")]
        public System.DateTime RetirementDate { get; set; }

        [TRequired(ErrorMessageResourceName = "Retirement.Required.RetirementDate", DefaultText = "Please select retirement date.")]
        [LocalizedDisplayName("Retirement.RetirementDateBS", DefaultText = "Retirement Date")]
        public string RetirementDateBS { get; set; }

        [LocalizedDisplayName("Retirement.IsHandOverCompleted", DefaultText = "handover completed")]
        public bool IsHandoverCompleted { get; set; }

        [LocalizedDisplayName("Retirement.HasPFLetterIssued", DefaultText = "PF letter issued")]
        public bool HasPFLetterIssued { get; set; }

        [LocalizedDisplayName("Retirement.HasCITLetterIssued", DefaultText = "CIT Letter Issued")]
        public bool HasCITLetterIssued { get; set; }
    }

    public class RetirementListViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int RetirementReasonId { get; set; }
        public string RetirementReasonName { get; set; }
        public System.DateTime RetirementDate { get; set; }
        public string RetirementDateBS { get; set; }
        public bool IsHandoverCompleted { get; set; }
        public bool HasPFLetterIssued { get; set; }
        public bool HasCITLetterIssued { get; set; }
    }
}
