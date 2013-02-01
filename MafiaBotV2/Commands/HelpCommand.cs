using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Commands
{
    class HelpCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "help"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return null; }
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            return "You can find help on how to use this bot on http://mafiawiki.four-horsemen.com/index.php/MafiaBot";
        }

        #endregion
    }
}
