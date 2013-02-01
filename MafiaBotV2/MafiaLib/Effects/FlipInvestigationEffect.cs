using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaLib.Effects
{
    public class FlipInvestigationEffect : Effect
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public FlipInvestigationEffect(Actor source) : base(source) {
            IsCancellable = false;
        }

        public override void Attach(Actor actor) {
            actor.PostActionTarget += new EventHandler<ActionEventArgs>(OnPostActionTarget);
            base.Attach(actor);
        }

        public override void Detach(Actor actor) {
            actor.PostActionTarget -= new EventHandler<ActionEventArgs>(OnPostActionTarget);
        }
        void OnPostActionTarget(object sender, ActionEventArgs e) {
            if(e.Action.Power is Powers.InvestigatePower) {
                Powers.InvestigationResult result = e.Action.Result as Powers.InvestigationResult;
                if(result != null) {
                    result.Evil = !result.Evil;

                    Log.Debug("Flipped investigation result on " + e.Action.Target.Name + " for " + e.Action.Who.Name);
                }
            }
        }
    }
}
