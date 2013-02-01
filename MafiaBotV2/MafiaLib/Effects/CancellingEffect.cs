using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib.Effects
{
    enum CancelWhichType {
        Source,
        Target
    }

    class CancellingEffect : Effect
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        Actor protector;
        Type cancelType;
        CancelWhichType which;

        public event EventHandler<ActionEventArgs> ActionCancelled;

        public CancellingEffect(Actor source, Dictionary<string, string> args)
            : this(source, Type.GetType(args["CancelType"])) {
        }

        public CancellingEffect(Actor source, Type cancelType)
            : this(source, cancelType, CancelWhichType.Target) {
        }

        public CancellingEffect(Actor source, Type cancelType, CancelWhichType which) : base(source) {
            this.protector = source;
            this.cancelType = cancelType;
            this.which = which;
            this.Duration = 1;
        }

        public override void Attach(Actor actor) {
            if (which == CancelWhichType.Target) {
                actor.PreActionTarget += new EventHandler<ActionEventArgs>(OnPreAction);
            }
            else {
                actor.PreActionSource += new EventHandler<ActionEventArgs>(OnPreAction);
            }
            base.Attach(actor);
        }

        public override void Detach(Actor actor) {
            if (which == CancelWhichType.Target) {
                actor.PreActionTarget -= new EventHandler<ActionEventArgs>(OnPreAction);
            }
            else {
                actor.PreActionSource -= new EventHandler<ActionEventArgs>(OnPreAction);
            }
            base.Detach(actor);
        }

        void OnPreAction(object sender, ActionEventArgs e) {
            if ((e.Action.Power.GetType() == cancelType || e.Action.Power.GetType().IsSubclassOf(cancelType)) 
                && !e.Action.Power.Instant) {

                Log.Debug("Cancelled Power " + e.Action.Power.GetType().FullName + " by " + e.Action.Who.Name + (e.Action.Target != null ? " on " + e.Action.Target.Name : ""));

                e.Action.Cancel = true;
                if(ActionCancelled != null) {
                    ActionCancelled(this, e);
                }
            }
        }
    }
}
