using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class PunishmentViewModel : ServiceModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.PunishmentTypeId", DefaultText = "Please select punishment type.")]
        [LocalizedDisplayName("Employee.Punishment.PunishmentTypeId", DefaultText = "Punishment type")]
        public int PunishmentTypeId { get; set; }

        public Nullable<System.DateTime> PunishmentOrderDate { get; set; }
        [LocalizedDisplayName("Employee.Punishment.PunishmentOrderDateBS", DefaultText = "Punishment order date (BS)")]
        public string PunishmentOrderDateBS { get; set; }

        [LocalizedDisplayName("Employee.Punishment.CourtDecision", DefaultText = "Court decision")]
        public string CourtDecision { get; set; }

        public Nullable<System.DateTime> CourtDecisionDate { get; set; }
        [LocalizedDisplayName("Employee.Punishment.CourtDecisionDate", DefaultText = "Court decision date (BS)")]
        public string CourtDecisionDateBS { get; set; }

        [LocalizedDisplayName("Remarks", DefaultText = "Remarks")]
        public string Remarks { get; set; }
    }

    public class PunishmentListViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string PunishmentTypeName { get; set; }
        public string PunishmentOrderDateBS { get; set; }
        public string CourtDecision { get; set; }
        public string CourtDecisionDateBS { get; set; }
        public string Remarks { get; set; }
    }
}
