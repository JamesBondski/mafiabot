using MafiaBotV2.MafiaLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using NUnit.Framework;

namespace MafiaBot.Test
{
    /// <summary>
    ///This is a test class for VillageTest and is intended
    ///to contain all VillageTest Unit Tests
    ///</summary>
    [TestFixture]
    public class VillageTest
    {
        Village village;

        [SetUp]
        public void MyTestInitialize() {
            messages.Clear();

            IVariantLoader loader = new MafiaBotV2.MafiaLib.Sources.XmlSource(Assembly.GetExecutingAssembly().GetManifestResourceStream("MafiaBot.Test2.Old.Data.StaticC9.xml"));
            village = new Village(loader);
            village.Outside = new TestOutsideConnector();
        }

        static List<string> messages = new List<string>();
        class TestOutsideConnector : IOutsideConnector {
            public void SendMessage(string message) {
                messages.Add(message);
            }

            public void AllowTalking(bool state) {
                
            }

            public void Highlight(bool state) { }
        }

        /// <summary>
        ///A test for OnPhaseOver
        ///</summary>
        [Test]
        public void PhaseOverTest() {
            village.Start();
            // Initial Phase should be day
            Assert.AreEqual(village.Phase.Type, PhaseType.Day);
            village.Phase.End();
            Assert.AreEqual(village.Phase.Type, PhaseType.Night);
            village.Phase.End();
            Assert.AreEqual(village.Phase.Type, PhaseType.Day);
        }

        [Test]
        public void MembersTest() {
            bool hasCop = false;

            Assert.AreEqual(village.Rules.MaximumPopulation, village.Members.Count);
            
            foreach (VillageMember member in village.Members) {
                bool canVote = false;
                bool canUnvote = false;
                foreach (Power power in member.Powers) {
                    if(power is MafiaBotV2.MafiaLib.Powers.InvestigatePower) {
                        Assert.IsFalse(hasCop);
                        hasCop = true;
                    }
                    else if (power is MafiaBotV2.MafiaLib.Powers.VotePower) {
                        Assert.IsFalse(canVote);
                        canVote = true;
                    }
                    else if (power is MafiaBotV2.MafiaLib.Powers.UnvotePower) {
                        Assert.IsFalse(canUnvote);
                        canUnvote = true;
                    }
                }

                Assert.IsTrue(canVote);
                Assert.IsTrue(canUnvote);
            }

            Assert.IsTrue(hasCop);
        }

        [Test]
        public void FactionsTest() {
            Assert.AreEqual(2, village.Factions.Count);
            bool hasTown = false;
            bool hasMafia = false;
            foreach (Faction faction in village.Factions) {
                switch(faction.Alignment) {
                    case Alignment.Evil:
                        Assert.IsFalse(hasMafia);
                        hasMafia = true;
                        Assert.AreEqual(2, faction.Powers.Count);
                        Assert.IsTrue(faction.Powers[0] is MafiaBotV2.MafiaLib.Powers.NightkillPower);
                        Assert.IsTrue(faction.Powers[1] is MafiaBotV2.MafiaLib.Powers.MembersPower);
                        break;
                    case Alignment.Good:
                        Assert.IsFalse(hasTown);
                        hasTown = true;
                        Assert.AreEqual(0, faction.Powers.Count);
                        break;
                }
            }

            Assert.IsTrue(hasTown);
            Assert.IsTrue(hasMafia);
        }

        /// <summary>
        ///A test for Village Constructor
        ///</summary>
        [Test]
        public void C9Test() {
            IVariantLoader loader = new MafiaBotV2.MafiaLib.Sources.XmlSource(Assembly.GetExecutingAssembly().GetManifestResourceStream("MafiaBot.Test2.Old.Data.C9.xml"));
            Village target = new Village(loader);

            Assert.AreEqual(2, target.Factions.Count);
            Assert.AreEqual(target.Rules.MaximumPopulation, target.Members.Count);

            int count = 0;
            foreach (Faction f in target.Factions) {
                count += f.Members.Count;
            }
            Assert.AreEqual(target.Rules.MaximumPopulation, count);
        }
    }
}
