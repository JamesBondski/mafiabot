using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MafiaBotV2.MafiaLib.Powers
{
    public class DaykillPower : Power
    {
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

        IRestriction[] restrictions = { new NotSelfRestriction() };
        public override IRestriction[] Restrictions {
            get { return restrictions; }
        }

        public override string Name {
            get { return "shoot"; }
        }

        bool endsDay = false;

        public DaykillPower() : this(false) {

        }

        public DaykillPower(bool endsDay) {
            this.endsDay = endsDay;
        }

        public DaykillPower(Dictionary<string, string> args) {
            if(args.ContainsKey("EndsDay")) {
                endsDay = Boolean.Parse(args["EndsDay"]);
            }
        }

        internal override void Execute(Action action) {
            Debug.Assert(action.Target is VillageMember);

            VillageMember target = action.Target as VillageMember;
            target.Kill(DeathCauseType.Generic);

            if (endsDay) {
                action.Target.Village.Phase.End();
            }
        }
    }
}
