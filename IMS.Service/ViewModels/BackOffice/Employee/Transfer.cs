using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class TransferViewModel : ServiceModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.PreviousSection", DefaultText = "Please select source section.")]
        [LocalizedDisplayName("PreviousSection", DefaultText = "Previous section")]
        public int PreviousSection { get; set; }

        [TRequired(ErrorMessageResourceName = "Required.TransferredSection", DefaultText = "Please select destination section.")]
        [LocalizedDisplayName("TransferredSection", DefaultText = "Destination section")]
        public int TransferredSection { get; set; }

        public System.DateTime StartDate { get; set; }
        [TRequired(ErrorMessageResourceName = "Required.StartDateBS", DefaultText = "Please enter start date (BS).")]
        [LocalizedDisplayName("StartDateBS", DefaultText = "Start date (BS)")]
        public string StartDateBS { get; set; }

        public Nullable<System.DateTime> EndDate { get; set; }
        [LocalizedDisplayName("EndDateBS", DefaultText = "End date (BS)")]
        public string EndDateBS { get; set; }

        public Nullable<System.DateTime> NextTransferDate { get; set; }
        [LocalizedDisplayName("NextTransferDateBS", DefaultText = "Next transfer date (BS)")]
        public string NextTransferDateBS { get; set; }

        public Nullable<System.DateTime> AttendedDate { get; set; }
        [LocalizedDisplayName("AttendedDateBS", DefaultText = "Attended date (BS)")]
        public string AttendedDateBS { get; set; }

        [LocalizedDisplayName("TransferReasonId", DefaultText = "Transfer reason")]
        public Nullable<int> TransferReasonId { get; set; }

        [LocalizedDisplayName("PreparedBy", DefaultText = "Prepared by")]
        public Nullable<int> PreparedBy { get; set; }

        public Nullable<System.DateTime> PreparedDate { get; set; }
        [LocalizedDisplayName("PreparedDateBS", DefaultText = "Prepared date (BS)")]
        public string PreparedDateBS { get; set; }

        [LocalizedDisplayName("AuthorisedBy", DefaultText = "Authorised by")]
        public Nullable<int> AuthorisedBy { get; set; }

        public Nullable<System.DateTime> AuthorisedDate { get; set; }
        [LocalizedDisplayName("AuthorisedDateBS", DefaultText = "Authorised date (BS)")]
        public string AuthorisedDateBS { get; set; }

        [LocalizedDisplayName("Remarks", DefaultText = "Remarks")]
        public string Remarks { get; set; }
    }

    public class TransferListViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string PreviousSectionName { get; set; }
        public string TransferredSectionName { get; set; }
        public string StartDateBS { get; set; }
        public string EndDateBS { get; set; }
        public string NextTransferDateBS { get; set; }
        public string AttendedDateBS { get; set; }
        public string TransferReasonName { get; set; }
        public string PreparedByName { get; set; }
        public string PreparedDateBS { get; set; }
        public string AuthorisedByName { get; set; }
        public string AuthorisedDateBS { get; set; }
        public string Remarks { get; set; }
    }
}
