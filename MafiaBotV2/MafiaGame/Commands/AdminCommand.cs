using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.MafiaGame.Http;

namespace MafiaBotV2.MafiaGame.Commands
{
    class AdminCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "admin"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return game; }
        }

        public string Execute(Network.NetUser from, Network.NetObject source, string[] args) {
            game.Http = new HttpInterface(game);
            game.Creator.SendMessage("Admin interface: " + game.Http.Url + " . If you stay in the game, it will automatically be an open setup. Use the ##leave command to leave the game.");
            return "Entered admin mode.";
        }

        MafiaGame game;

        public AdminCommand(MafiaGame game) {
            this.game = game;
        }

        #endregion
    }
}
