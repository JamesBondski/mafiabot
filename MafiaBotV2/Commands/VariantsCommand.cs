using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;

namespace MafiaBotV2.Commands
{
    class VariantsCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "variants"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return null; }
        }

        public string Execute(NetUser from, NetObject source, string[] args) {
            if(args.Length == 0) {
                return ListVariants();
            }
            else {
                return GetDescription(args[0]);
            }
        }

        private string GetDescription(string name) {
            string description = MafiaLib.VariantManager.Instance.GetDescription(name);
            if (description == null) {
                return "Variant not found.";
            }
            return description;
        }

        private string ListVariants() {
            IEnumerable<string> variants = MafiaLib.VariantManager.Instance.GetVariants();
            StringBuilder result = new StringBuilder();
            result.Append("Available variants: ");
            foreach (string variant in variants) {
                result.Append(variant).Append(", ");
            }
            result.Remove(result.Length - 2, 2);
            result.Append(" To read the description of a variant, use the \"variants <name>\" command.");
            return result.ToString();
        }

        #endregion
    }
}
