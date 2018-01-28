using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.Contract
{
    public interface ILocalizationService
    {
        string GetLocalizedText(string key, string language = null, string defaultText = "");
    }
}
