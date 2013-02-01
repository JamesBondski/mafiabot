using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Commands
{
    class CreateCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "create"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return null; }
        }

        Bot bot;

        public CreateCommand(Bot bot) {
            this.bot = bot;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(args.Length < 1) {
                return "Syntax: ##create <name>";
            }

            string gameType = "mafia";
            if(args.Length>1) {
                gameType = args[1];
            }

            IGame game = bot.Games.CreateGame(from, args[0], gameType);
            return "Game created! To join the game, go to channel " + game.Channel.Name + " .";
        }

        #endregion
    }
}
