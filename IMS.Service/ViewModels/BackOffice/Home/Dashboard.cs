using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Home
{
    class DashboardViewModel
    {
    }

    public class DashboardAttendanceSummaryViewModel
    {
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLeaveAndTravel { get; set; }
        public int TotalEmployee { get; set; }
    }
    public class DashboardEmployeeTravelDetailListViewModel
    {
        public int TravelApplicationId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Objectives { get; set; }
        public string Organizer { get; set; }
        public string TravelTypeName { get; set; }
        public string TravelCategoryName { get; set; }
        public string DateFromBS { get; set; }
        public string DateToBS { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalDay { get; set; }
        public string TotalDayString { get; set; }
    }
    public class DashboardEmployeeLeaveDetailListViewModel
    {
        public int LeaveApplicationId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveTypeName { get; set; }
        public string LeaveCategotyName { get; set; }
        public string Cause { get; set; }
        public string DateFromBS { get; set; }
        public string DateToBS { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalDay { get; set; }
        public string TotalDayString { get; set; }
    }
}
