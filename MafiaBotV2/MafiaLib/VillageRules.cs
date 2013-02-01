using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib
{
    public class VillageRules
    {
        int maximumPopulation = 0;
        public int MaximumPopulation {
            get { return maximumPopulation; }
            set { maximumPopulation = value; }
        }

        bool cardFlip = false;
        public bool CardFlip {
            get { return cardFlip; }
            set { cardFlip = value; }
        }

        bool allowNolynch = false;
        public bool AllowNolynch {
            get { return allowNolynch; }
            set { allowNolynch = value; }
        }

        PhaseType initialPhase = PhaseType.Day;
        public MafiaBotV2.MafiaLib.PhaseType InitialPhase {
            get { return initialPhase; }
            set { initialPhase = value; }
        }
    }
}
