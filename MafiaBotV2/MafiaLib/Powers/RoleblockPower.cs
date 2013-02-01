using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaLib.Powers
{
    class RoleblockPower : Power
    {
        public override bool Instant {
            get { return false; }
        }

        public override int Priority {
            get { return 2000; }
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
            get { return "block"; }
        }

        public override string QueuedMessage {
            get {
                return "You are blocking {0} tonight.";
            }
        }

        public override bool IsRoleblocking {
            get {
                return true;
            }
        }

        public RoleblockPower() {
        }

        internal override void Execute(Action action) {
            Effects.CancellingEffect effect = new Effects.CancellingEffect(action.Who, typeof(Power), Effects.CancelWhichType.Source);
            effect.ActionCancelled += new EventHandler<ActionEventArgs>(OnBlocked);
            action.Target.ApplyEffect(effect);

            //Remove cancellable Effects caused by target
            foreach(VillageMember member in action.Village.Members) {
                IEnumerable<Effect> causedEffects = member.Effects.Where(e => e.IsCancellable && e.Source == action.Target);
                foreach(Effect causedEffect in causedEffects) {
                    member.RemoveEffect(causedEffect);
                }
            }
        }

        void OnBlocked(object sender, ActionEventArgs e) {
            e.Action.Initiator.Outside.SendMessage("Your action (" + e.Action.Power.Name + ") was roleblocked.");
        }
    }
}
