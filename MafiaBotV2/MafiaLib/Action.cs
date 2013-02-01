using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib
{
    public interface IActionResult {
        void Apply();
    }

    public class Action : IComparable<Action>
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        Power power;
        public Power Power {
            get { return power; }
            set { power = value; }
        }

        Village village;
        public Village Village {
            get { return village; }
        }

        Actor who;
        public Actor Who {
            get { return who; }
        }

        Actor target;
        public Actor Target {
            get { return target; }
        }

        VillageMember initiator = null;
        public MafiaBotV2.MafiaLib.VillageMember Initiator {
            get { return initiator; }
            set { initiator = value; }
        }

        bool cancel = false;
        public bool Cancel {
            get { return cancel; }
            set { cancel = value; }
        }

        IActionResult result = null;
        public IActionResult Result {
            get { return result; }
            set { result = value; }
        }

        string resultMessage = string.Empty;
        public string ResultMessage {
            get { return resultMessage; }
            set { resultMessage = value; }
        }

        public Action(Power power, Actor who, VillageMember initiator, Actor target) {
            this.power = power;
            this.who = who;
            this.target = target;
            this.village = who.Village;
            this.initiator = initiator;
        }

        public void Prepare() {
            if (target != null && target.Type != power.TargetType) {
                throw new InvalidTargetException("Target " + target.Name + " is not a " + power.TargetType.ToString());
            }
            //HACK
            if(target == null && power.TargetType != Actor.ActorType.None && !(power is Powers.VotePower)) {
                throw new InvalidTargetException("You need to specify a valid target.");
            }
			
			if(!power.CanExecute(village)) {
				throw new FailedRestrictionException("Power "+power.Name+" can not be executed during the current phase.");
			}

            foreach (IRestriction restriction in power.Restrictions) {
                if (!restriction.Check(this)) {
                    throw new FailedRestrictionException(restriction.FailMessage);
                }
            }
        }

        public void Execute() {
            Log.Debug(who.Name + " executes " + power.GetType().FullName + ( target != null ? " on " + target.Name : ""));
            power.Execute(this);
        }

        [global::System.Serializable]
        public class ActionException : MafiaException
        {
            public ActionException() { }
            public ActionException(string message) : base(message) { }
            public ActionException(string message, Exception inner) : base(message, inner) { }
            protected ActionException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }

        public class InvalidTargetException : ActionException
        {
            public InvalidTargetException(string message) : base(message) { }
        }

        public class FailedRestrictionException : ActionException
        {
            public FailedRestrictionException(string message) : base(message) { }
        }

        #region IComparable<Action> Members

        public int CompareTo(Action other) {
            return power.CompareTo(other.Power);
        }

        #endregion
    }
}
