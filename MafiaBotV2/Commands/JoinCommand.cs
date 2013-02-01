using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Commands
{
    class JoinCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "join"; }
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

        public JoinCommand(BasicGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            game.AddUser(from);
            from.Commands.Remove(this);
            return "You have been added to the game. To leave again, use the ##leave command.";
        }

        #endregion
    }
}
