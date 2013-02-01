using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;

namespace MafiaBotV2.MafiaGame.Commands
{
    class ReplaceCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "replace"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        MafiaGame game;

        public object Parent {
            get { return game; }
        }

        public ReplaceCommand(MafiaGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(args.Length < 2) {
                return "Syntax: ##replace <old> <new>";
            }

            NetUser oldUser = game.Players.Find(p => p.Name.ToLower() == args[0].ToLower());
            if(oldUser == null) {
                return "Player " + args[0] + " is not in this game.";
            }
            NetUser newUser = game.Channel.Users.Find(p => p.Name.ToLower() == args[1].ToLower());
            if(newUser == null) {
                return "User " + args[1] + " is not in the game channel.";
            }

            game.Replace(oldUser, newUser);

            return args[0] + " has been replaced by " + args[1];
        }

        #endregion
    }
}
