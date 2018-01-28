using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.Employee;
using PagedList;

namespace IMS.Logic.Contract
{
    public interface IEmployeeProfileService
    {
        EmployeeProfileViewModel SaveProfile(EmployeeProfileViewModel model);
        EmployeeProfileViewModel GetProfile(int employeeId);
    }
}
