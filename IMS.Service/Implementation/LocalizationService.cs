using IMS.Data.Localization;
using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IMS.Logic.Implementation
{
    public class LocalizationService : ILocalizationService
    {
        public string GetLocalizedText(string key, string language = null, string defaultText = "")
        {
            language = string.IsNullOrEmpty(language) ? IMS.Logic.Configuration.IMSAppConfig.Instance.CurrentLanguage : language;
            var localization = Localization.Data.FirstOrDefault(x => x.Key == key && x.Language == language);

            if (localization == null)

                return (language == "np" || string.IsNullOrEmpty(defaultText)) ? key : defaultText;

            string localizedText = localization.Value;
            return localizedText;
        }
    }
}
