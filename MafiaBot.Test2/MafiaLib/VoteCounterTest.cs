using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MafiaBotV2.MafiaLib;

namespace MafiaBot.Test2.MafiaLib
{
    [TestFixture]
    class VoteCounterTest
    {
        MafiaVoteCounter counter;
        Village village;

        public VoteCounterTest() {}

        [SetUp]
        public void Init() {
            village = new Village();
            Faction f = village.CreateFaction("Faction");
            for (int i = 0; i < 5; i++) {
                f.CreateMember("Voter" + i);
            }
            counter = new MafiaVoteCounter(village.Members);
        }

        private void KilledSetup() {
            counter.Vote(village.Members[0], village.Members[1]);
            //Need exactly one vote
            Assert.AreEqual(1, counter.GetVotes().Count);
            Assert.AreEqual(1, counter.GetVotes()[village.Members[1]].Count);
            Assert.AreEqual(village.Members[0], counter.GetVotes()[village.Members[1]][0]);
        }

        [Test]
        public void KilledVoterTest() {
            KilledSetup();

            village.Members[0].Kill();
            Assert.AreEqual(0, counter.GetVotes().Count);
        }

        [Test]
        public void KilledVoteeTest() {
            KilledSetup();
            village.Members[1].Kill();
            Assert.AreEqual(0, counter.GetVotes().Count);
        }

        [Test]
        public void NoLynchVoteTest() {
            village.Rules.AllowNolynch = true;
            counter.Vote(village.Members[0], counter.NoLynchMember);
            Assert.AreEqual(1, counter.GetVotes().Count);
            Assert.IsTrue(counter.GetVotes().ContainsKey(counter.NoLynchMember));
            Assert.AreEqual(1, counter.GetVotes()[counter.NoLynchMember].Count);
            Assert.AreEqual(village.Members[0], counter.GetVotes()[counter.NoLynchMember][0]);
        }
    }
}
