using IMS.Logic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.ViewModels.BackOffice.Employee;
using IMS.Logic.ViewModels.BackOffice;

namespace IMS.Logic.Contract
{
    public interface IExceptionService
    {
        void Execute(Action<ServiceModel> statement, ServiceModel model);
        ServiceModel Log(Exception ex);
    }
}
