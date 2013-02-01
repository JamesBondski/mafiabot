using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Barebones.Commands
{
    class VoteCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "vote"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        BarebonesGame game;

        public object Parent {
            get { return game; }
        }

        public VoteCommand(BarebonesGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(args.Length == 0) {
                return "Syntax: ##vote <name>";
            }
            return game.Vote(from, from.Master.GetUser(args[0]));
        }

        #endregion
    }
}
