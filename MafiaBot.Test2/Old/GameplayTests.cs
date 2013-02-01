using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MafiaBotV2.MafiaLib;
using System.Reflection;
using NUnit.Framework;

namespace MafiaBot.Test
{
    /// <summary>
    /// Summary description for GameplayTests
    /// </summary>
    [TestFixture]
    public class GameplayTests
    {
        public GameplayTests() {
            //
            // TODO: Add constructor logic here
            //
        }

        Village village;
        static List<string> messages = new List<string>();
        class TestOutsideConnector : IOutsideConnector
        {
            public void SendMessage(string message) {
                messages.Add(message);
            }

            public void AllowTalking(bool state) {
                messages.Add("AllowTalking " + state);
            }

            public void Highlight(bool state) {}
        }

        [SetUp]
        public void GameplayTestInitialize() {
            messages.Clear();

            IVariantLoader loader = new MafiaBotV2.MafiaLib.Sources.XmlSource(Assembly.GetExecutingAssembly().GetManifestResourceStream("MafiaBot.Test2.Old.Data.StaticC9.xml"));
            village = new Village(loader);
            village.Outside = new TestOutsideConnector();
        }

        [Test]
        public void Nolynch() {
            village.Start();

            village.Members[0].UsePower("vote", village.Phase.Votes.NoLynchMember);
            village.Members[1].UsePower("vote", village.Phase.Votes.NoLynchMember);
            village.Members[2].UsePower("vote", village.Phase.Votes.NoLynchMember);
            village.Members[3].UsePower("vote", village.Phase.Votes.NoLynchMember);
            Assert.AreEqual(village.Phase.Type, PhaseType.Night);
            foreach(VillageMember member in village.Members) {
                Assert.AreEqual(MemberState.Alive, member.State);
            }
        }
        [Test]
        public void Night0Test() {
            village.Rules.InitialPhase = PhaseType.Night0;
            village.Start();
            Assert.AreEqual(PhaseType.Night0, village.Phase.Type);
            village.Members[4].UsePower("investigate", village.Members[5]);
            Assert.AreEqual(PhaseType.Day, village.Phase.Type);
        }

        [Test]
        public void TownWin() {
            // 0-2 Vanilla, 3 Doc, 4 Cop, 5-6 Mafia
            village.Start();

            Assert.AreEqual(village.Phase.Type, PhaseType.Day);
            village.Members[0].UsePower("vote", village.Members[5]);
            village.Members[1].UsePower("vote", village.Members[5]);
            village.Members[2].UsePower("vote", village.Members[5]);
            village.Members[3].UsePower("vote", village.Members[5]);
            Assert.AreEqual(village.Members[5].State, MemberState.Dead);
            Assert.AreEqual(village.Phase.Type, PhaseType.Night);

            village.Factions[1].UsePower("kill", village.Members[6], village.Members[0]);
            village.Members[3].UsePower("protect", village.Members[1]);
            village.Members[4].UsePower("investigate", village.Members[6]);
            Assert.AreEqual(village.Members[0].State, MemberState.Dead);
            Assert.AreEqual(village.Phase.Type, PhaseType.Day);

            village.Members[1].UsePower("vote", village.Members[1]);
            village.Members[2].UsePower("vote", village.Members[1]);
            village.Members[3].UsePower("vote", village.Members[1]);
            Assert.AreEqual(village.Members[1].State, MemberState.Dead);
            Assert.AreEqual(village.Phase.Type, PhaseType.Night);

            village.Factions[1].UsePower("kill", village.Members[6], village.Members[2]);
            village.Members[3].UsePower("protect", village.Members[2]);
            village.Members[4].UsePower("investigate", village.Members[2]);
            Assert.AreEqual(village.Members[2].State, MemberState.Alive);
            Assert.AreEqual(village.Phase.Type, PhaseType.Day);

            village.Members[2].UsePower("vote", village.Members[6]);
            village.Members[3].UsePower("vote", village.Members[6]);
            village.Members[4].UsePower("vote", village.Members[6]);
            Assert.AreEqual(MemberState.Dead, village.Members[6].State);
            Assert.AreEqual(PhaseType.Over, village.Phase.Type);

            Assert.AreEqual(false, village.Factions[1].WinConditions[0].Check(village.Factions[1]));
            Assert.AreEqual(true, village.Factions[0].WinConditions[0].Check(village.Factions[0]));
        }
    }
}
