using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib.Powers
{
    public enum SanityType {
        Sane,
        Insane,
        Naive,
        Paranoid
    }

    public class InvestigationResult : IActionResult {

        bool evil;
        public bool Evil {
            get { return evil; }
            set { evil = value; }
        }

        Actor who;
        public MafiaBotV2.MafiaLib.Actor Who {
            get { return who; }
            set { who = value; }
        }

        Actor target;
        public MafiaBotV2.MafiaLib.Actor Target {
            get { return target; }
            set { target = value; }
        }

        public InvestigationResult(Actor who, Actor target, bool evil) {
            this.evil = evil;
            this.who = who;
            this.target = target;
        }

        public void Apply() {
            string message = null;
            switch (evil) {
                case true:
                    message = target.Name + " appears to be evil.";
                    break;
                case false:
                    message = target.Name + " appears to be town.";
                    break;
            }
            who.Outside.SendMessage(message);
        }
    }

    public class InvestigatePower : Power
    {
        public override bool Instant {
            get { return false; }
        }

        public override int Priority {
            get { return 0; }
        }
		
		PhaseType[] phases = { PhaseType.Night, PhaseType.Night0 };
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
            get { return "investigate";  }
        }

        public override string QueuedMessage {
            get {
                return "You are investigating {0}.";
            }
        }

        SanityType sanity = SanityType.Sane;
        public MafiaBotV2.MafiaLib.Powers.SanityType Sanity {
            get { return sanity; }
            set { sanity = value; }
        }

        public InvestigatePower() {
        }

        public InvestigatePower(SanityType sanity) {
            this.sanity = sanity;
        }

        public InvestigatePower(Dictionary<string, string> args) {
            if (args.ContainsKey("Sanity")) {
                this.sanity = (SanityType)Enum.Parse(typeof(SanityType), args["Sanity"]);
            }
        }

        internal override void Execute(Action action) {
            VillageMember target = (VillageMember)action.Target;
            action.Result = new InvestigationResult(action.Who, action.Target, GetResult(target));
        }

        private bool GetResult(VillageMember target) {
            if(this.Sanity == SanityType.Naive) {
                return false;
            }
            if(this.Sanity == SanityType.Paranoid) {
                return true;
            }
            bool saneResult = (target.Faction.Alignment == Alignment.Evil);
            if(this.Sanity == SanityType.Sane) {
                return saneResult;
            }
            else {
                return !saneResult;
            }
        }
    }
}
