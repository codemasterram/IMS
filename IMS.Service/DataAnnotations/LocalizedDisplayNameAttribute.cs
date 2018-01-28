using IMS.Logic.Configuration;
using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Logic.DataAnnotations
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private ILocalizationService localizationService;
        public string DefaultText { get; set; }

        public LocalizedDisplayNameAttribute(string displayName)
            : base(displayName)
        {
            localizationService = IMSAppConfig.Instance.DependencyResolver.GetService<ILocalizationService>();
        }

        public override string DisplayName
        {
            get
            {
                var msg = localizationService.GetLocalizedText(base.DisplayName, IMSAppConfig.Instance.CurrentLanguage, DefaultText);

                return msg;
            }
        }
    }
}
