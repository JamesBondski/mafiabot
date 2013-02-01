using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.MafiaLib;
using NUnit.Framework;

namespace MafiaBot.Test2.MafiaLib
{
    [TestFixture]
    class WinConditionsTest
    {
        Village village;
        Faction town;
        Faction mafia;

        public WinConditionsTest() {}

        [SetUp]
        public void Init() {
            village = new Village(new MafiaBotV2.MafiaLib.Sources.BasicLayoutSource());
            town = village.Factions.First(v => v.Name == "Town");
            mafia = village.Factions.First(v => v.Name == "Mafia");
        }

        [Test]
        public void NoEvilLeftTest() {
            NoEvilLeftCondition cond = new NoEvilLeftCondition();
            
            Assert.IsTrue(cond.Check(town));

            town.CreateMember("DummyT");
            Assert.IsTrue(cond.Check(town));

            VillageMember mm = mafia.CreateMember("DummyM");
            Assert.IsTrue(cond.Check(mafia));
            Assert.IsFalse(cond.Check(town));

            mm.Kill();
            Assert.IsTrue(cond.Check(town));
        }

        [Test]
        public void MajorityOrEqualTest() {
            MajorityOrEqualCondition cond = new MajorityOrEqualCondition();

            Assert.IsTrue(cond.Check(town));
            Assert.IsTrue(cond.Check(mafia));

            VillageMember tm = town.CreateMember("DummyT");
            Assert.IsTrue(cond.Check(town));
            Assert.IsFalse(cond.Check(mafia));

            mafia.CreateMember("DummyM");
            Assert.IsTrue(cond.Check(town));
            Assert.IsTrue(cond.Check(mafia));

            tm.Kill();
            Assert.IsFalse(cond.Check(town));
            Assert.IsTrue(cond.Check(mafia));
        }
    }
}
