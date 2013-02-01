using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Barebones.Commands
{
    class AddCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "add"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        BarebonesGame game;

        public object Parent {
            get { return game; }
        }

        public AddCommand(BarebonesGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(args.Length == 0) {
                return "Syntax: ##add <name>";
            }

            game.AddUser(from.Master.GetUser(args[0]));
            return args[0] + " has been added to the game.";
        }

        #endregion
    }
}
