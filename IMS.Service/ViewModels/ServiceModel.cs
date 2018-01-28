using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels
{
    public class ServiceModel : ValidationModal
    {
        public ServiceModel()
        {
            Errors = new List<ValidationResult>();
        }

        public IList<ValidationResult> Errors { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public bool HasError
        {
            get
            {
                return Errors.Any();
            }
        }

        public IList<string> ErrorMessages
        {
            get
            {
                if (HasError)
                    return Errors.Select(x => x.ErrorMessage).ToList();
                return null;
            }
        }

        public bool Validate()
        {
            var context = new ValidationContext(this, null, null);
            var result = Validator.TryValidateObject(this, context, this.Errors);
            return result;
        }

        public static ServiceModel Empty
        {
            get
            {
                return new ServiceModel();
            }
        }
    }

    public class ValidationModal
    {
        public List<string> ErrorList = new List<string>();
    }
}
