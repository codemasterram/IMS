using IMS.Logic.Configuration;
using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Logic.DataAnnotations
{
    public class TRequiredAttribute : RequiredAttribute
    {
        private string displayName;
        private ILocalizationService localizationService;

        public string DefaultText { get; set; }

        public TRequiredAttribute()
        {
            ErrorMessageResourceName = "Validation_Required";
            localizationService = IMSAppConfig.Instance.DependencyResolver.GetService<ILocalizationService>();
        }

        protected override ValidationResult IsValid
        (object value, ValidationContext validationContext)
        {
            displayName = validationContext.DisplayName;
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            var msg = localizationService.GetLocalizedText(ErrorMessageResourceName, IMSAppConfig.Instance.CurrentLanguage, DefaultText);
            return msg;
        }
    }

    public class TRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        public TRequiredAttributeAdapter(ModelMetadata metadata, ControllerContext context, TRequiredAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return base.GetClientValidationRules();
        }
    }
}
