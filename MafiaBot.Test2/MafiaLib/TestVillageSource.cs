using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.MafiaLib.Sources;
using MafiaBotV2.MafiaLib;

namespace MafiaBot.Test2.MafiaLib
{
    class TestVillageSource : BasicLayoutSource
    {
        int numTown;
        int numScum;

        public TestVillageSource(int numTown, int numScum) {
            this.numScum = numScum;
            this.numTown = numTown;
        }

        public override void Load(MafiaBotV2.MafiaLib.Village village) {
            base.Load(village);

            Faction town = village.Factions.First(v => v.Name == "Town");
            Faction mafia = village.Factions.First(v => v.Name == "Mafia");
            for (int i = 0; i < numTown; i++) {
                town.CreateMember("Town" + i);
            }
            for (int i = 0; i < numScum; i++) {
                mafia.CreateMember("Mafia");
            }
        }
    }
}
