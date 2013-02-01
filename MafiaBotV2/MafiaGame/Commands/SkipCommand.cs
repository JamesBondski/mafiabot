using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.MafiaLib;

namespace MafiaBotV2.MafiaGame.Commands
{
    class SkipCommand : ICommand
    {
        public string Name {
            get { return "skip"; }
        }

        public bool AllowedInPublic {
            get { return false; }
        }

        MafiaGame game;
        public object Parent {
            get { return game; }
        }

        public SkipCommand(MafiaGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if (args.Length == 0) {
                return "Syntax: ##skip <powername>";
            }

            VillageMember member = game.Mapper[from];
            member.Skip(args[0]);
            return "Action " + args[0] + " skipped.";
        }
    }
}
