using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Commands
{
    class StartCommand : ICommand
    {

        #region ICommand Members

        public string Name {
            get { return "start"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        BasicGame game;
        public MafiaBotV2.BasicGame Game {
            get { return game; }
        }

        public object Parent {
            get { return game; }
        }

        public StartCommand(BasicGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(game.MaxPlayers != -1 && game.Players.Count < game.MaxPlayers) {
                return "Need another " + (game.MaxPlayers - game.Players.Count) + " players to start the game.";
            }

            game.Start();
            from.Commands.Remove(this);
            return null;
        }

        #endregion
    }
}
