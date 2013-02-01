using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MafiaBotV2.MafiaLib.Powers
{
    class AggregatePower : Power
    {
        Power[] powers;
        public Power[] Powers {
            get { return powers; }
        }

        Actor.ActorType targetType = Actor.ActorType.Person;
        public override Actor.ActorType TargetType {
            get { return targetType; }
        }

        bool instant;
        public override bool Instant {
            get { return instant; }
        }

        int priority;
        public override int Priority {
            get { return priority; }
        }
		
		PhaseType[] phases = { };
		public override PhaseType[] Phases {
			get {
				return phases;
			}
		}

        List<IRestriction> restrictions = new List<IRestriction>();
        public override IRestriction[] Restrictions {
            get { return restrictions.ToArray(); }
        }

        public override bool IsRoleblocking {
            get {
                return powers.Any(p => p.IsRoleblocking);
            }
        }

        string name;
        public override string Name {
            get { return name; }
        }

        public AggregatePower(string name, Power[] powers) {
            this.powers = powers;
            this.name = name;

            InitPower();
        }

        public AggregatePower(Dictionary<string, string> args) {
            this.name = args["Name"];

            string[] powerTypes = args["Powers"].Split(',');
            this.powers = new Power[powerTypes.Length];

            int index = 0;
            foreach(string powerType in powerTypes) {
                object[] cargs = { };
                this.powers[index] = (Power) Type.GetType(powerType).InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, null, cargs);
                index++;
            }
			InitPower();
        }

        private void InitPower() {
            Power first = powers[0];
            if(!powers.All(p => p.TargetType == first.TargetType)) {
                throw new PowersNotAggregatableException("All powers in an aggregate power must have the same target type.");
            }
            targetType = first.TargetType;

            instant = !powers.Any(p => !p.Instant);
            priority = powers.Max(p => p.Priority);
			
			// Merge phases
			List<PhaseType> phases = new List<PhaseType>();
			foreach(Power p in powers) {
				foreach(PhaseType ph in p.Phases) {
					if(!phases.Contains(ph)) {
						phases.Add(ph);
					}
				}
			}
			this.phases = phases.ToArray();

            // Add all restrictions from all powers, avoiding duplicates
            foreach(Power p in powers) {
                foreach(IRestriction r in p.Restrictions) {
                    if(!restrictions.Any(ra => ra.GetType() == r.GetType())) {
                        restrictions.Add(r);
                    }
                }
            }
        }

        internal override void Execute(Action action) {
            foreach(Power p in powers) {
                p.Execute(action);
            }
        }
    }

    [global::System.Serializable]
    public class PowersNotAggregatableException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public PowersNotAggregatableException() { }
        public PowersNotAggregatableException(string message) : base(message) { }
        public PowersNotAggregatableException(string message, Exception inner) : base(message, inner) { }
        protected PowersNotAggregatableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
