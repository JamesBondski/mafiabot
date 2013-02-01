using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib
{
    public interface IRestriction
    {
        bool Check(Action power);

        string FailMessage {
            get;
        }
    }

    public class OrRestriction : IRestriction
    {
        IRestriction[] restrictions;
        public IRestriction[] Restrictions {
            get { return restrictions; }
        }

        public OrRestriction(params IRestriction[] restrictions) {
            this.restrictions = restrictions;
        }

        #region IRestriction Members

        public bool Check(Action power) {
            foreach(IRestriction restriction in restrictions) {
                if(restriction.Check(power)) {
                    failMessage = restriction.FailMessage;
                    return true;
                }
            }
            return false;
        }

        string failMessage = "";
        public string FailMessage {
            get { return failMessage; }
        }

        #endregion
    }

    public class NotSelfRestriction : IRestriction
    {

        #region IRestriction Members

        public bool Check(Action power) {
            return (power.Who != power.Target);
        }

        public string FailMessage {
            get { return "You cannot use this power on yourself."; }
        }

        #endregion
    }

    public class TargetNotOwnFactionRestriction : IRestriction
    {

        #region IRestriction Members

        public bool Check(Action power) {
            Faction who;
            Faction target;

            if(power.Who.Type == Actor.ActorType.Person) {
                who = ((VillageMember)power.Who).Faction;
            }
            else {
                who = (Faction)power.Who;
            }

            if (power.Target.Type == Actor.ActorType.Person) {
                target = ((VillageMember)power.Target).Faction;
            }
            else {
                target = (Faction)power.Target;
            }

            return (who != target);
        }

        public string FailMessage {
            get { return "You cannot use this power on someone in your own faction."; }
        }
        #endregion
    }

}
