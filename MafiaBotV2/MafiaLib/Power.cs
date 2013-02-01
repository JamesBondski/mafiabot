using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaLib
{
    public abstract class Power : IComparable<Power>
    {
        public virtual Actor.ActorType TargetType {
            get { return Actor.ActorType.Person; }
        }

        public abstract bool Instant {
            get;
        }

        public abstract int Priority {
            get;
        }
		
		public abstract PhaseType[] Phases {
			get;
		}

        public abstract IRestriction[] Restrictions {
            get;
        }

        public abstract string Name {
            get;
        }

        public virtual bool IsRoleblocking {
            get { return false; }
        }

        public virtual string QueuedMessage {
            get { return "Action queued."; }
        }

        Actor owner;
        public MafiaBotV2.MafiaLib.Actor Owner {
            get { return owner; }
            set { owner = value; }
        }

        int charges = 0;
        public int Charges {
            get { return charges; }
            set { charges = value; }
        }

        internal virtual void Execute(Action action) {
            if(charges != 0) {
                charges--;
                if(charges == 0) {
                    owner.RemovePower(this);
                }
            }
        }

        public Power() {

        }

        public bool CanExecute(Village village) {
            if (this.Phases.Any(p => p == PhaseType.Any)) {
                return true;
            }

            if (village.Phase == null) {
                return false;
            }
			return Phases.Contains(village.Phase.Type);
        }

        #region IComparable<Power> Members

        public int CompareTo(Power other) {
            return other.Priority - this.Priority;
        }

        #endregion
    }
}
