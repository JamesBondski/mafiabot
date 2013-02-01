using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.MafiaLib;
using NUnit.Framework;

namespace MafiaBot.Test2.MafiaLib
{
    [TestFixture]
    class VillageRulesTest
    {
        Village village;
        Faction town;
        Faction mafia;

        public VillageRulesTest() {}

        [SetUp]
        public void Init() {
            village = new Village(new MafiaBotV2.MafiaLib.Sources.BasicLayoutSource());
            town = village.Factions.First(v => v.Name == "Town");
            mafia = village.Factions.First(v => v.Name == "Mafia");

            for(int i=0;i<2;i++) {
                town.CreateMember("Town" + i);
            }
            mafia.CreateMember("Mafia");
        }

        [Test]
        public void InitialPhaseDefaultTest() {
            Assert.AreEqual(PhaseType.Day, village.Rules.InitialPhase);
            village.Start();
            Assert.AreEqual(PhaseType.Day, village.Phase.Type);
        }

        [Test]
        public void InitialPhaseChangedTest() {
            village.Rules.InitialPhase = PhaseType.Night;
            village.Start();
            Assert.AreEqual(PhaseType.Night, village.Phase.Type);
        }

        [Test]
        public void NoLynchTrueTest() {
            village.Rules.AllowNolynch = true;
            village.Start();
            MafiaVoteCounter counter = village.Phase.Votes;
            counter.Vote(town.Members[0], counter.NoLynchMember);
            Assert.AreEqual(1, counter.GetVotes().Count);
        }

        [Test]
        [ExpectedException(typeof(NoLynchNotAllowedException))]
        public void NoLynchFalseTest() {
            village.Rules.AllowNolynch = false;
            village.Start();
            MafiaVoteCounter counter = village.Phase.Votes;
            counter.Vote(town.Members[0], counter.NoLynchMember);
        }
    }
}
