using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Commands
{
    class ListCommand : ICommand
    {
        Bot bot;

        public ListCommand(Bot bot) {
            this.bot = bot;
        }

        public string Name {
            get { return "list"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return null; }
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if (bot.Games.Games.Count == 0) {
                return "There are no games going on right now.";
            }
            else {
                return "Current games: " + new Util.ListFormatter<IGame>(bot.Games.Games);
            }
        }
    }
}
