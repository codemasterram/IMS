using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.DataAnnotations
{
    public static class ValidationAttributeHelper
    {
        public static ValidationContext LocalizeDisplayName(this ValidationContext context)
        {
            context.DisplayName = context.DisplayName;
            return context;
        }
    }
}
