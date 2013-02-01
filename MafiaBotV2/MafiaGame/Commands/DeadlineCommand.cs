using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Util;

namespace MafiaBotV2.MafiaGame.Commands
{
    class DeadlineCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "deadline"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        MafiaGame game;

        public object Parent {
            get { return game; }
        }

        public DeadlineCommand(MafiaGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(args.Length == 0) {
                return "Syntax: ##deadline <seconds>";
            }

            if(from != game.Creator) {
                return "Only the creator can set deadlines.";
            }

            DateTime executionTime = DateTime.Now.AddSeconds(Int32.Parse(args[0]));
            game.SetDeadline(executionTime);
            return null;
        }

        #endregion
    }
}
