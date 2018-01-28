using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.Extensions
{
    public static class ValidationResultExtension
    {
        public static ValidationResult AddModelKey<TModel, TProperty>(this ValidationResult validationResult, Expression<Func<TModel, TProperty>> exp)
        {
            return new ValidationResult(validationResult.ErrorMessage);
        }
    }
}
