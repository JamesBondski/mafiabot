using System;

using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib
{
    public interface IWinCondition
    {
        bool Check(Faction faction);
    }

    public class NoEvilLeftCondition : IWinCondition
    {
        #region IWinCondition Members

        public bool Check(Faction faction) {
            Village village = faction.Village;
            foreach (Faction villageFaction in village.Factions) {
                if (villageFaction != faction && villageFaction.Alignment == Alignment.Evil && villageFaction.AliveMembers.Count > 0) {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }

    public class MajorityOrEqualCondition : IWinCondition
    {
        #region IWinCondition Members

        public bool Check(Faction faction) {
            int count = 0;
            foreach (Faction villageFaction in faction.Village.Factions) {
                if (villageFaction != faction) {
                    count += villageFaction.AliveMembers.Count;
                }
            }
            return faction.AliveMembers.Count >= count;
        }

        #endregion
    }
}
