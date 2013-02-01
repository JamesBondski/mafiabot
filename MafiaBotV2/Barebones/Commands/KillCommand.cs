using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;

namespace MafiaBotV2.Barebones.Commands
{
    class KillCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "kill"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        BarebonesGame game;

        public object Parent {
            get { return game; }
        }

        public KillCommand(BarebonesGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(args.Length == 0) {
                return "Syntax: ##kill <name>";
            }
            
            NetUser user = from.Master.GetUser(args[0]);
            game.Kill(user);
            return null;
        }

        #endregion
    }
}
