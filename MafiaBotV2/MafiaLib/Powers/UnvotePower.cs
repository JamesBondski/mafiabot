using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib.Powers
{
    public class UnvotePower : Power
    {
        public override Actor.ActorType TargetType {
            get { return Actor.ActorType.None; }
        }

        public override bool Instant {
            get { return true; }
        }

        public override int Priority {
            get { return 0; }
        }
		
		PhaseType[] phases = { PhaseType.Day };
		public override PhaseType[] Phases {
			get {
				return phases;
			}
		}

        IRestriction[] restrictions = { };
        public override IRestriction[] Restrictions {
            get { return restrictions; }
        }

        public override string Name {
            get { return "unvote"; }
        }

        public UnvotePower() {
        }
        
        internal override void Execute(Action action) {
            action.Village.Phase.Votes.Unvote((VillageMember)action.Who);
            action.Village.Outside.SendMessage(action.Village.Phase.Votes.GetVoteCount(true));
        }
    }
}
