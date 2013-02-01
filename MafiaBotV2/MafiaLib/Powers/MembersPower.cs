using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib.Powers
{
    public class MembersPower : Power
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
		
		PhaseType[] phases = { PhaseType.Any };
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
            get { return "members"; }
        }

        public MembersPower() {
        }

        internal override void Execute(Action action) {
            StringBuilder members = new StringBuilder();
            members.Append("Your factions members are: ");
            foreach (VillageMember member in action.Initiator.Faction.Members) {
                members.Append(member.Name).Append(", ");
            }
            members.Remove(members.Length - 2, 2).Append(". ");
            action.Initiator.Outside.SendMessage(members.ToString());
        }
    }
}
