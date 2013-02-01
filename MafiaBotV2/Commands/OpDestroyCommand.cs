using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Commands
{
    class OpDestroyCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "opdestroy"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return null; ; }
        }

        Bot bot;

        public OpDestroyCommand(Bot bot) {
            this.bot = bot;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(args.Length < 1) {
                return "Syntax: ##opdestroy <name>";
            }
            if(!from.IsOpInChannel(bot.MainChannel)) {
                return "You must be an op to use this command.";
            }
            if(bot.Games.GetGame(args[0]) == null) {
                return "Game doesn't exist.";
            }
            bot.Games.Destroy(args[0]);
            return "Game destroyed.";
        }

        #endregion
    }
}
