using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaLib.Effects
{
    class PowerGrantingEffect : Effect
    {
        Power power = null;

        public PowerGrantingEffect(Actor source, Power power) : base(source) {
            this.power = power;
        }

        public override void Attach(Actor actor) {
            actor.AddPower(power);
            base.Attach(actor);
        }

        public override void Detach(Actor actor) {
            actor.RemovePower(power);
            base.Detach(actor);
        }
    }

    class KingEffect : PowerGrantingEffect
    {
        public KingEffect()
            : base(null, new Powers.DaykillPower(true)) {
            this.Duration = 2;
        }

        public override void Attach(Actor actor) {
            base.Attach(actor);
            actor.Outside.Highlight(true);
            actor.Village.Outside.SendMessage("All hail King " + actor.Name + "!");
        }

        public override void Detach(Actor actor) {
            base.Detach(actor);
            actor.Outside.Highlight(false);
        }
    }

    class KingmakerEffect : PowerGrantingEffect
    {
        string oldRoleName;

        public KingmakerEffect() : base(null, new Powers.KingmakePower()) {
        }

        public override void Attach(Actor actor) {
            base.Attach(actor);

            oldRoleName = (actor as VillageMember).RoleName;
            (actor as VillageMember).RoleName = "Kingmaker";
            actor.Outside.SendMessage("You have become Kingmaker!");
        }

        public override void Detach(Actor actor) {
            base.Detach(actor);
            (actor as VillageMember).RoleName = oldRoleName;
            
            // Find a new kingmaker
            List<VillageMember> goodMembers = actor.Village.AliveMembers.FindAll(vm => vm.Faction.Alignment == Alignment.Good);
            Random rnd = new Random();
            goodMembers[rnd.Next(goodMembers.Count)].ApplyEffect(this);
            actor.Village.Outside.SendMessage("A new kingmaker has been chosen.");
        }
    }
}
