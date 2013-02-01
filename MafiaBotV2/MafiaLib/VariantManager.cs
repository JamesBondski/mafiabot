using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace MafiaBotV2.MafiaLib
{
    class VariantManager
    {
        XDocument doc = new XDocument();

        private VariantManager() {
            Stream source = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MafiaBotV2.MafiaLib.Data.Variants.xml");
            doc = XDocument.Load(new System.IO.StreamReader(source));
        }

        public IEnumerable<string> GetVariants() {
            var results = from v in doc.Descendants("Variant")
                          select v.Element("Name").Value;
            return results.ToList();
        }

        public string GetDescription(string name) {
            var results = from v in doc.Descendants("Variant")
                          where name.Equals(v.Element("Name").Value, StringComparison.OrdinalIgnoreCase)
                          select v.Element("Description").Value;
            return results.First();
        }

        public Village CreateVillage(string variantName) {
            var variantResults = from v in doc.Descendants("Variant")
                          where variantName.Equals(v.Element("Name").Value, StringComparison.OrdinalIgnoreCase)
                          select v;

            foreach(XElement variant in variantResults) {
                IVariantLoader loader = null;
                if (variant.Element("File") != null) {
                    using (Stream source = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(variant.Element("File").Value)) {
                        loader = new Sources.XmlSource(source);
                    }
                    return new Village(loader);
                }
            }

            return null;
        }

        #region VariantManager Singleton
        public static VariantManager Instance {
            get {
                return NestedVariantManager.instance;
            }
        }

        class NestedVariantManager
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static NestedVariantManager() {
            }

            internal static readonly VariantManager instance = new VariantManager();
        }
        #endregion
        
    }
}
