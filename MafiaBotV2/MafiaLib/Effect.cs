using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib
{
    public abstract class Effect
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        int duration = 0;
        public int Duration {
            get { return duration; }
            set { duration = value; }
        }
        Actor source;
        public Actor Source {
            get { return source; }
        }
        Actor target;
        public Actor Target {
            get { return target; }
        }
        bool isCancellable = true;
        public bool IsCancellable {
            get { return isCancellable; }
            set { isCancellable = value; }
        }

        public virtual void Attach(Actor actor) {
            Log.Debug("Effect " + this.GetType().Name + " attached to " + actor.Name);
            this.target = actor;
        }

        public virtual void Detach(Actor actor) {
            Log.Debug("Effect " + this.GetType().Name + " detached from " + actor.Name);
            this.target = null;
        }

        public Effect(Actor source) {
            this.source = source;
        }
    }
}
