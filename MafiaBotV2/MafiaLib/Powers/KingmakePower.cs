using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaLib.Powers
{
    class KingmakePower : Power
    {
        public override bool Instant {
            get { return false; }
        }

        public override int Priority {
            get { return 0; }
        }

        PhaseType[] phases = { PhaseType.Dawn };
        public override PhaseType[] Phases {
            get { return phases; }
        }

        IRestriction[] restrictions = { new NotSelfRestriction() };
        public override IRestriction[] Restrictions {
            get { return restrictions; }
        }

        public override string Name {
            get { return "kingmake"; }
        }

        internal override void Execute(Action action) {
            base.Execute(action);

            action.Target.ApplyEffect(new Effects.KingEffect());
        }
    }
}
