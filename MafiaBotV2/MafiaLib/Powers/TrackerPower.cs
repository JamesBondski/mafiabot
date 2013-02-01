using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaLib.Powers
{
    public enum TrackType
    {
        Tracker,
        Watcher
    }

    public class TrackerPower : Power
    {
        TrackType type;

        public override bool Instant {
            get { return false; }
        }

        public override int Priority {
            get { return 1000; }
        }
		
		PhaseType[] phases = { PhaseType.Night };
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
            get {
                if (type == TrackType.Watcher) {
                    return "watch";
                }
                else {
                    return "track";
                }
            }
        }

        public override string QueuedMessage {
            get {
                return "You are " + Name + "ing {0} tonight.";
            }
        }

        public TrackerPower() {
        }

        public TrackerPower(TrackType type) {
            this.type = type;
        }

        public TrackerPower(Dictionary<string, string> args) {
            this.type = (TrackType)Enum.Parse(typeof(TrackType), args["TrackerType"]);
        }

        internal override void Execute(Action action) {
            action.Target.ApplyEffect(new Effects.TrackedEffect(action.Who, type));
        }
    }
}
