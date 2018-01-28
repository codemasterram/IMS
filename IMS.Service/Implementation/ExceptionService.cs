using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.ViewModels;

namespace IMS.Logic.Implementation
{
    public class ExceptionService : IExceptionService
    {
        public void Execute(Action<ServiceModel> statement, ServiceModel model)
        {
            try
            {               
                statement(model);
            }
            catch (Exception ex)
            {
                model.Exception = ex;

                if (ex.Message.Contains("Unique"))
                {
                    model.Errors.Add(new System.ComponentModel.DataAnnotations.ValidationResult("Item already exists"));
                }
                else
                {
                    model.Errors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(ex.Message));
                }
            }
        }

        public ServiceModel Log(Exception ex)
        {
            //ServiceModel result = new ServiceModel
            //{
            //    ErrorMessage = ex.Message,
            //    Exception = ex,
            //    InnerException = ex.InnerException != null ? ex.InnerException.ToString() : ""
            //};

            //logging here...


            return new ServiceModel();
        }
    }
}
