using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaLib.Effects
{
    public class HeroEffect : Effect
    {
        public HeroEffect() : base(null) {
        }

        public override void Attach(Actor actor) {
            actor.PreActionTarget += new EventHandler<ActionEventArgs>(OnPreActionTarget);
            base.Attach(actor);
        }

        public override void Detach(Actor actor) {
            actor.PreActionTarget -= new EventHandler<ActionEventArgs>(OnPreActionTarget);
            base.Detach(actor);
        }

        void OnPreActionTarget(object sender, ActionEventArgs e) {
            if(e.Action.Power is Powers.DaykillPower) {
                e.Action.Cancel = true;
                (e.Action.Who as VillageMember).Kill(DeathCauseType.Hero);
                e.Action.Village.Phase.End();
            }
        }
    }
}
