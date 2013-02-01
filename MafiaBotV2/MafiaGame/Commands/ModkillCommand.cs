using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;

namespace MafiaBotV2.MafiaGame.Commands
{
    class ModkillCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "modkill"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        MafiaGame game;

        public object Parent {
            get { return game; }
        }

        public ModkillCommand(MafiaGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(args.Length == 0) {
                return "Syntax: ##modkill <target>";
            }
            
            NetUser target = game.Players.Find(p => p.Name.ToLower() == args[0].ToLower());
            if(target != null) {
                game.Mapper[target].Kill();
                return args[0] + " has been modkilled.";
            }
            else {
                return "Unknown user " + args[0];
            }
        }

        #endregion
    }
}
