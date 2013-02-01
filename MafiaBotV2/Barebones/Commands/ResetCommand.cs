using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Barebones.Commands
{
    class ResetCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "reset"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        BarebonesGame game;

        public object Parent {
            get { return game; }
        }

        public ResetCommand(BarebonesGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            game.ResetVotes();
            return "Votes have been reset.";
        }

        #endregion
    }
}
