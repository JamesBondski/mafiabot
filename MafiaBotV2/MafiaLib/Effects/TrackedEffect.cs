using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.MafiaLib.Powers;

namespace MafiaBotV2.MafiaLib.Effects
{
    public class TrackedEffect : Effect
    {
        TrackType type;
        bool triggered = false;

        public TrackedEffect(Actor tracker,TrackType type) : base(tracker) {
            this.type = type;
            this.Duration = 1;
        }

        public override void Attach(Actor actor) {
            if (type == TrackType.Tracker) {
                actor.PostActionSource += new EventHandler<ActionEventArgs>(OnPostAction);
            }
            else {
                actor.PostActionTarget += new EventHandler<ActionEventArgs>(OnPostAction);
            }
        }

        public override void Detach(Actor actor) {
            if (type == TrackType.Tracker) {
                actor.PostActionSource -= new EventHandler<ActionEventArgs>(OnPostAction);
            }
            else {
                actor.PostActionTarget -= new EventHandler<ActionEventArgs>(OnPostAction);
            }
            if (!triggered) {
                Source.Outside.SendMessage("You did not see anything.");
            }
        }

        void OnPostAction(object sender, ActionEventArgs e) {
            if (!e.Action.Cancel && e.Action.Initiator != Source) {
                Source.Outside.SendMessage(e.Action.Initiator.Name + " targeted " + e.Action.Target.Name);
                triggered = true;
            }
        }
    }
}
