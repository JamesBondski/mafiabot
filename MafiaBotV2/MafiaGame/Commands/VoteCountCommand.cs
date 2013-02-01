using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaGame.Commands
{
    class VoteCountCommand : ICommand
    {
        MafiaGame game;

        public VoteCountCommand(MafiaGame game) {
            this.game = game;
        }

        public string Name {
            get { return "votecount"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return game; }
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            return game.Village.Phase.Votes.GetVoteCount(true);
        }
    }
}
