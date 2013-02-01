using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Util;
using NUnit.Framework;

namespace MafiaBot.Test2.Util
{
    class Voter : IVoter
    {
        #region IVoter Members

        string name;

        public Voter(int id) {
            name = "Voter" + id;
        }

        public string Name {
            get { return name; }
        }

        #endregion
    }

    [TestFixture]
    class VoteCounterTest
    {
        List<Voter> voters = new List<Voter>();
        Voter nolynch;
        VoteCounter<Voter> counter;
        bool majorityReached;

        public VoteCounterTest() {
        }

        [SetUp]
        public void Init() {
            voters.Clear();
            for (int i = 0; i < 5; i++) {
                voters.Add(new Voter(i));
            }
            majorityReached = false;

            nolynch = new Voter(99);
            counter = new VoteCounter<Voter>(voters, nolynch);
            counter.MajorityReached += new EventHandler<MajorityReachedEventArgs<Voter>>(OnMajorityReached);
        }

        void OnMajorityReached(object sender, MajorityReachedEventArgs<Voter> e) {
            majorityReached = true;
        }

        [Test]
        public void VoteOk() {
            counter.Vote(voters[0], voters[1]);

            Dictionary<Voter, List<Voter>> votes = counter.GetVotes();
            Assert.AreEqual(1, votes.Count);
            Assert.IsTrue(votes.ContainsKey(voters[1]));
            Assert.AreEqual(1, votes[voters[1]].Count);
            Assert.AreEqual(voters[0], votes[voters[1]][0]);

            Assert.IsFalse(majorityReached);
        }

        [Test]
        [ExpectedException(typeof(InvalidVoterException))]
        public void VoteVoterNotExisting() {
            Voter dummy = new Voter(98);
            counter.Vote(dummy, voters[1]);
        }

        [Test]
        [ExpectedException(typeof(InvalidVoterException))]
        public void VoteVoteeNotExisting() {
            Voter dummy = new Voter(98);
            counter.Vote(voters[0], dummy);
        }

        [Test]
        public void MajorityReachedUneven() {
            for (int i = 0; i < 2; i++) {
                counter.Vote(voters[i], voters[4]);
            }
            Assert.IsFalse(majorityReached);
            counter.Vote(voters[2], voters[4]);
            Assert.IsTrue(majorityReached);
        }

        [Test]
        public void MajorityReachedEven() {
            voters.Add(new Voter(5));
            for (int i = 0; i < 3; i++) {
                counter.Vote(voters[i], voters[4]);
            }
            Assert.IsFalse(majorityReached);
            counter.Vote(voters[3], voters[4]);
            Assert.AreEqual(4, counter.GetVotes()[voters[4]].Count);
            Assert.IsTrue(majorityReached);
        }

        [Test]
        public void ClearVotes() {
            for (int i = 0; i < 3; i++) {
                counter.Vote(voters[i], voters[4]);
            }
            Assert.AreEqual(3, counter.GetVotes()[voters[4]].Count);
            counter.ClearVoter(voters[1]);
            Assert.AreEqual(2, counter.GetVotes()[voters[4]].Count);
            counter.ClearVoter(voters[4]);
            Assert.AreEqual(0, counter.GetVotes().Count);
        }

        [Test]
        [ExpectedException(typeof(InvalidVoterException))]
        public void ClearVotesInvalid() {
            counter.ClearVoter(new Voter(999));
        }
    }
}
