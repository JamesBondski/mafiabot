using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib.Powers
{
    public class ProtectPower : Power
    {
        public override bool Instant {
            get { return false; }
        }

        public override int Priority {
            get { return 750; }
        }

        bool dieOnSuccess = false;
        public bool DieOnSuccess {
            get { return dieOnSuccess; }
            set { dieOnSuccess = value; }
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
            get { return "protect"; }
        }

        public override string QueuedMessage {
            get {
                return "You are protecting {0}.";
            }
        }

        public ProtectPower() {
        }

        public ProtectPower(bool dieOnSuccess) {
            this.dieOnSuccess = dieOnSuccess;
        }

        public ProtectPower(Dictionary<string, string> args) {
            if (args.ContainsKey("DieOnSuccess")) {
                this.dieOnSuccess = true;
            }
        }

        internal override void Execute(Action action) {
            Effects.CancellingEffect effect = new Effects.CancellingEffect(action.Who, typeof(NightkillPower));
            if(dieOnSuccess) {
                effect.ActionCancelled += new EventHandler<ActionEventArgs>(OnActionCancelled);
            }
            action.Target.ApplyEffect(effect);
        }

        void OnActionCancelled(object sender, ActionEventArgs e) {
            System.Diagnostics.Debug.Assert(this.Owner is VillageMember);

            if(dieOnSuccess) {
                (this.Owner as VillageMember).Kill();
            }
        }
    }
}
