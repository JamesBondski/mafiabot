using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using MafiaBotV2.Util;

namespace MafiaBotV2.MafiaLib
{
    public class Actor : IVoter
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        string name;
        public string Name {
            get { return name; }
            set { name = value; }
        }

        Village village;
        public Village Village {
            get { return village; }
        }
        
        List<Power> powers = new List<Power>();
        public List<Power> Powers {
            get { return powers; }
        }

        List<Effect> effects = new List<Effect>();
        public List<Effect> Effects {
            get { return effects; }
        }

        ActorType type;
        public MafiaBotV2.MafiaLib.Actor.ActorType Type {
            get { return type; }
        }

        IOutsideConnector outside = new DummyOutsideConnector();
        public IOutsideConnector Outside {
            get { return outside; }
            set { outside = value; }
        }

        public bool HasUnusedPowers {
            get { return GetUnusedPowers().Count > 0; }
        }

        public event EventHandler<ActionEventArgs> PreActionSource;
        public event EventHandler<ActionEventArgs> PreActionTarget;
        public event EventHandler<ActionEventArgs> PostActionSource;
        public event EventHandler<ActionEventArgs> PostActionTarget;

        public event EventHandler<PowerEventArgs> PowerGained;
        public event EventHandler<PowerEventArgs> PowerLost;

        public enum ActorType {
            Faction,
            Person,
            None
        }

        public Actor(Village village, ActorType type) {
            this.village = village;
            this.type = type;
        }

        public Power GetPower(string name) {
            IEnumerable<Power> powersFound = from p in powers
                                       where p.Name.ToLower() == name.ToLower()
                                       select p;
            if(powersFound.Count() > 0) {
                return powersFound.ElementAt(0);
            }
            else {
                return null;
            }
        }

        public void ApplyEffect(Effect effect) {
            Log.Debug(this.Name + " gained effect " + effect.GetType().Name);

            effects.Add(effect);
            effect.Attach(this);
        }

        public void RemoveEffect(Effect effect) {
            Debug.Assert(effect.Target == this);
            Debug.Assert(effects.Contains(effect));
            Log.Debug(this.Name + " lost effect " + effect.GetType().Name);

            effects.Remove(effect);
            effect.Detach(this);
        }

        public void Skip(string powerName) {
            Power power = GetPower(powerName);
            if (power == null) {
                throw new NoSuchPowerException("You do not have the power " + powerName);
            }
            Skip(power);
        }

        public void Skip(Power power) {
            village.Phase.SkipPower(this, power);
        }

        public Action UsePower(string powerName, VillageMember initiator, Actor target) {
            Power power = GetPower(powerName);
            if(power == null) {
                throw new NoSuchPowerException("You do not have the power " + powerName);
            }
            return UsePower(power, initiator, target);
        }

        public Action UsePower(Power power, VillageMember initiator, Actor target) {
            if (target != null) {
                Log.Debug(this.Name + " uses power " + power.GetType().Name + " on " + target.Name);
            }
            else {
                Log.Debug(this.Name + " uses power " + power.GetType().Name);
            }

            if (target is VillageMember) {
                if (((VillageMember)target).State == MemberState.Dead) {
                    throw new TargetIsDeadException(target.Name + " is dead.");
                }
            }

            Action action = new Action(power, this, initiator, target);
            try {
                action.Prepare();
                village.RaiseActionPrepared(action);
                if (action.Power.Instant) {
                    action.Who.RaisePreActionSource(action);
                    if (action.Target != null) {
                        action.Target.RaisePreActionTarget(action);
                    }
                    if (action.Cancel) {
                        Log.Debug("Action cancelled.");
                    } else {
                        Log.Debug("Executing action...");
                        action.Execute();

                        action.Who.RaisePostActionSource(action);
                        if (action.Target != null) {
                            action.Target.RaisePostActionTarget(action);
                        }
                        if (action.Result != null) {
                            action.Result.Apply();
                        }
                    }
                }
                else {
                    Log.Debug("Queueing " + power.GetType().FullName + " on " + target.Name);
                    village.Phase.QueueAction(action);
                    action.ResultMessage = String.Format(power.QueuedMessage, target.Name);
                }
                village.RaiseActionExecuted(action);
                return action;
            }
            catch (Action.ActionException ex) {
                action.ResultMessage = ex.Message;
                action.Cancel = true;
                return action;
            }
        }

        public virtual void Turn() {
            foreach(Effect e in Effects.FindAll(ef => ef.Duration == 1)) {
                RemoveEffect(e);
            }

            foreach(Effect e in Effects) {
                e.Duration--;
            }     
        }

        public virtual void RaisePreActionSource(Action action) {
            if(PreActionSource != null) {
                PreActionSource(this, new ActionEventArgs(action));
            }
        }

        public virtual void RaisePreActionTarget(Action action) {
            if(PreActionTarget != null) {
                PreActionTarget(this, new ActionEventArgs(action));
            }
        }

        public virtual void RaisePostActionSource(Action action) {
            if (PostActionSource != null) {
                PostActionSource(this, new ActionEventArgs(action));
            }
        }

        public virtual void RaisePostActionTarget(Action action) {
            if (PostActionTarget != null) {
                PostActionTarget(this, new ActionEventArgs(action));
            }
        }

        public List<Power> GetUnusedPowers() {
            return (from p in powers
                   where !p.Instant 
                        && p.CanExecute(village) 
                        && !village.Phase.HasAction(this, p)
                   select p).ToList();
        }

        public void AddPower(Power power) {
            Log.Debug(this.Name + " gained power " + power.GetType().Name);
            power.Owner = this;
            powers.Add(power);

            if(PowerGained != null) {
                PowerGained(this, new PowerEventArgs(this, power));
            }
        }

        public void RemovePower(Power power) {
            Log.Debug(this.Name + " lost power " + power.GetType().Name);
            powers.Remove(power);

            if (PowerLost != null) {
                PowerLost(this, new PowerEventArgs(this, power));
            }
        }

        [global::System.Serializable]
        public class NoSuchPowerException : MafiaException
        {
            public NoSuchPowerException() { }
            public NoSuchPowerException(string message) : base(message) { }
            public NoSuchPowerException(string message, Exception inner) : base(message, inner) { }
            protected NoSuchPowerException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }

    public class ActionEventArgs : EventArgs {
        Action action;
        public MafiaBotV2.MafiaLib.Action Action {
            get { return action; }
        }

        public ActionEventArgs(Action action) {
            this.action = action;
        }
    }

    public class PowerEventArgs : EventArgs {
        Actor actor;
        public MafiaBotV2.MafiaLib.Actor Actor {
            get { return actor; }
        }

        Power power;
        public MafiaBotV2.MafiaLib.Power Power {
            get { return power; }
        }

        public PowerEventArgs(Actor actor, Power power) {
            this.actor = actor;
            this.power = power;
        }
    }

    [Serializable]
    public class TargetIsDeadException : MafiaException
    {
        public TargetIsDeadException() { }
        public TargetIsDeadException(string message) : base(message) { }
        public TargetIsDeadException(string message, Exception inner) : base(message, inner) { }
        protected TargetIsDeadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
