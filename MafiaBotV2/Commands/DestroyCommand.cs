using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Commands
{
    class DestroyCommand : ICommand
    {
        #region ICommand Members

        GameManager games;
        IGame game;

        public DestroyCommand(GameManager games, IGame game) {
            this.games = games;
            this.game = game;
        }

        public string Name {
            get { return "destroy"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return game; }
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if (game == null) {
                return "Game does not exist.";
            }
            else if (game.Creator != from && !from.IsOpInChannel(game.Channel.Name)) {
                return "You are not the creator.";
            }

            games.Destroy(game);
            return "Game destroyed.";
        }

        #endregion
    }
}
