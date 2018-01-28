using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class TrainingSeminarConferenceViewModel : ServiceModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [TRequired(ErrorMessageResourceName = "Required.Name", DefaultText = "Please enter name.")]
        [LocalizedDisplayName("Name", DefaultText = "Name")]
        public string Name { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        [LocalizedDisplayName("StartDateBS", DefaultText = "Start date (BS)")]
        public string StartDateBS { get; set; }

        public Nullable<System.DateTime> EndDate { get; set; }
        [LocalizedDisplayName("EndDateBS", DefaultText = "End date (BS)")]
        public string EndDateBS { get; set; }

        [LocalizedDisplayName("OrganizationName", DefaultText = "Organized by")]
        public string OrganizationName { get; set; }

        [LocalizedDisplayName("OrganizationAddress", DefaultText = "Organization address")]
        public string OrganizationAddress { get; set; }

        [LocalizedDisplayName("Remarks", DefaultText = "Remarks")]
        public string Remarks { get; set; }
    }

    public class TrainingSeminarConferenceListViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string StartDateBS { get; set; }
        public string EndDateBS { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationAddress { get; set; }
        public string Remarks { get; set; }
    }


}
