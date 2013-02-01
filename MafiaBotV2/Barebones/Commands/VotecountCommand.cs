using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Barebones.Commands
{
    class VotecountCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "votecount"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        BarebonesGame game;

        public object Parent {
            get { return game; }
        }

        public VotecountCommand(BarebonesGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            return game.GetVoteCount();
        }

        #endregion
    }
}
