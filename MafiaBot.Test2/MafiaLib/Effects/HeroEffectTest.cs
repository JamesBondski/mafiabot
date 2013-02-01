using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MafiaBotV2.MafiaLib;
using MafiaBotV2.MafiaLib.Effects;
using MafiaBotV2.MafiaLib.Powers;

namespace MafiaBot.Test2.MafiaLib.Effects
{
    [TestFixture]
    class HeroEffectTest
    {
        Village village;
        Faction town;
        Faction mafia;

        public HeroEffectTest() {}

        [SetUp]
        public void Init() { }

        private void Init(int numTown, int numScum) {
            village = new Village(new TestVillageSource(numTown, numScum));
            town = village.Factions.First(v => v.Name == "Town");
            mafia = village.Factions.First(v => v.Name == "Mafia");

            village.Start();
            town.Members[0].ApplyEffect(new HeroEffect());
        }

        [Test]
        public void TownieSlain() {
            Init(4, 1);

            town.Members[1].AddPower(new DaykillPower(true));

            Assert.AreEqual(MemberState.Alive, town.Members[0].State);
            Assert.AreEqual(MemberState.Alive, town.Members[1].State);
            Assert.AreEqual(PhaseType.Day, village.Phase.Type);

            town.Members[1].UsePower("shoot", town.Members[0]);

            Assert.AreEqual(MemberState.Alive, town.Members[0].State);
            Assert.AreEqual(MemberState.Dead, town.Members[1].State);
            Assert.AreEqual(PhaseType.Night, village.Phase.Type);
        }

        [Test]
        public void MafiaSlain() {
            Init(4, 1);

            mafia.Members[0].AddPower(new DaykillPower(true));

            Assert.AreEqual(MemberState.Alive, town.Members[0].State);
            Assert.AreEqual(MemberState.Alive, town.Members[1].State);
            Assert.AreEqual(PhaseType.Day, village.Phase.Type);

            mafia.Members[0].UsePower("shoot", town.Members[0]);

            Assert.AreEqual(MemberState.Alive, town.Members[0].State);
            Assert.AreEqual(MemberState.Dead, mafia.Members[0].State);
            Assert.AreEqual(PhaseType.Over, village.Phase.Type);
        }
    }
}
