using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Util;
using System.Xml.Linq;

namespace MafiaBotV2
{
    public static class DonationMessage
    {
        static WeightedRandom<string> names = null;

        public static string GetMessage() {
            if(names == null) {
                if(System.IO.File.Exists("donations.xml")) {
                    ReadNames();
                }
                else {
                    return null;
                }
            }
            return names.Choose();
        }

        private static void ReadNames() {
            names = new WeightedRandom<string>();

            XDocument doc = XDocument.Load("donations.xml");
            foreach(XElement element in doc.Descendants("Donation")) {
                string name = element.Element("Name").Value;
                int weight = Int32.Parse(element.Element("Amount").Value);
                names.AddValue(weight, name);
            }
        }
    }
}
