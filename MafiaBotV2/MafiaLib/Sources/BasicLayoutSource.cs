using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaLib.Sources
{
    public class BasicLayoutSource : IVariantLoader
    {
        #region IVariantLoader Members

        public virtual void Load(Village village) {
            Faction town = village.CreateFaction("Town");
            Faction mafia = village.CreateFaction("Mafia");

            village.Rules.CardFlip = true;
            village.Rules.AllowNolynch = true;

            town.WinConditions.Add(new NoEvilLeftCondition());

            mafia.Alignment = Alignment.Evil;
            mafia.AddPower(new Powers.NightkillPower());
            mafia.AddPower(new Powers.MembersPower());
            mafia.WinConditions.Add(new MajorityOrEqualCondition());
        }

        #endregion
    }
}
