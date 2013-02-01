using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MafiaBotV2.MafiaLib.Powers
{
    public class NightkillPower : Power
    {
        public override bool Instant {
            get { return false; }
        }

        public override int Priority {
            get { return 500; }
        }
		
		PhaseType[] phases = { PhaseType.Night };
		public override PhaseType[] Phases {
			get {
				return phases;
			}
		}

        IRestriction[] restrictions = { new TargetNotOwnFactionRestriction(), 
                                          new NotSelfRestriction() };
        public override IRestriction[] Restrictions {
            get { return restrictions; }
        }

        public override string Name {
            get { return "kill"; }
        }

        public override string QueuedMessage {
            get {
                return "{0} is going to be killed tonight.";
            }
        }

        public NightkillPower() {
        }

        internal override void Execute(Action action) {
            Debug.Assert(action.Target is VillageMember);

            VillageMember target = (VillageMember)action.Target;
            target.Kill();
        }
    }
}
