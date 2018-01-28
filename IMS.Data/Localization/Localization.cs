using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IMS.Data.Localization
{
    public class Localization
    {
        private static List<Localization> data;
        public static List<Localization> Data
        {
            get
            {
                if (data == null)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    XDocument xDoc = new XDocument();
                    using (Stream stream = assembly.GetManifestResourceStream("IMS.Data.Localization.Localization.xml"))
                    {
                        if (stream == null)
                            throw new Exception("Localization Resource not found.");

                        StreamReader reader = new StreamReader(stream);
                        string xml = reader.ReadToEnd();
                        xDoc = XDocument.Parse(xml);
                        data = xDoc.Descendants("locale").Select(x => new Localization { Language = x.Attribute("lang").Value, Key = x.Element("key").Value, Value = x.Element("value").Value }).ToList();
                    }
                }

                return data;
            }
            set
            {
                data = value;
            }
        }

        private Localization()
        {

        }

        public string Key { get; set; }
        public string Language { get; set; }
        public string Value { get; set; }
    }
}
