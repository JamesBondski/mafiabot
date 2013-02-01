using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Commands
{
    class LeaveCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "leave"; }
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

        public LeaveCommand(BasicGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            game.RemoveUser(from);
            from.Commands.Add(new JoinCommand(game));
            from.Commands.Remove(this);

            return "You have been removed from the game. To rejoin, use the ##join command.";
        }

        #endregion
    }
}
