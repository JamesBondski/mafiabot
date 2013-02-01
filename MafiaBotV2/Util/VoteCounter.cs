using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MafiaBotV2.Util
{
    public interface IVoter
    {
        string Name {
            get;
        }
    }

    public class MajorityReachedEventArgs<T> : EventArgs where T : class, IVoter
    {
        T winner;
        public T Winner {
            get { return winner; }
        }

        public MajorityReachedEventArgs(T winner) {
            this.winner = winner;
        }
    }

    public class VoteCounter<T> where T : class, IVoter
    {
        List<T> voters = new List<T>();
        public List<T> Voters {
            get { return voters; }
            set { voters = value; }
        }

        List<VoteStruct> votes = new List<VoteStruct>();
        public event EventHandler<MajorityReachedEventArgs<T>> MajorityReached;

        T nolynch;
        public T NoLynchMember {
            get { return nolynch; }
        }

        public VoteCounter(List<T> voters, T nolynchMember) {
            this.voters = voters;
            this.nolynch = nolynchMember;
        }

        public virtual bool Vote(T from, T to) {
            if (!IsValidVoter(from)) {
                throw new InvalidVoterException("Voter " + from.Name + " not counted.");
            }
            if (!IsValidVoter(to)) {
                throw new InvalidVoterException("Votee "+to.Name+" not counted.");
            }

            if (votes.Any(v => (v.From == from))) {
                return false;
            }

            votes.Add(new VoteStruct(from, to));

            CheckForMajority(to);
            return true;
        }

        public void Unvote(T from) {
            if (!IsValidVoter(from)) {
                throw new InvalidVoterException("Voter " + from.Name + " not counted.");
            }
            if (!votes.Any(v => (v.From.Equals(from)))) {
                throw new NotVotedException();
            }

            votes.RemoveAll(v => v.From == from);
        }

        private bool IsValidVoter(T voter) {
            return voters.Contains(voter) || voter == nolynch;
        }

        public void Clear() {
            votes.Clear();
        }

        public void ClearVoter(T who) {
            if (!IsValidVoter(who)) {
                throw new InvalidVoterException("Voter "+who.Name+" is not counted.");
            }

            votes.RemoveAll(v => v.From.Equals(who) || v.To.Equals(who));
        }

        public Dictionary<T, List<T>> GetVotes() {
            Debug.Assert(NoLynchMember != default(T));

            Dictionary<T, List<T>> voteList = new Dictionary<T, List<T>>();
            foreach (VoteStruct vote in votes) {
                if (!voteList.ContainsKey(vote.To)) {
                    voteList.Add(vote.To, new List<T>());
                }
                voteList[vote.To].Add(vote.From);
            }
            return voteList;
        }

        public string GetVoteCount(bool verbose) {
            if (votes.Count == 0) {
                return "No votes cast.";
            }

            string result = "Count: ";
            foreach (KeyValuePair<T, List<T>> Count in GetVotes()) {
                if (verbose) {
                    result += Count.Key.Name + "(";
                    foreach (T U in Count.Value) {
                        result += U.Name + ", ";
                    }
                    result = result.Substring(0, result.Length - 2);
                    result += "), ";
                }
                else {
                    result += Count.Key.Name + " (" + Count.Value.Count + "), ";
                }
            }
            result = result.Substring(0, result.Length - 2) + ".";
            return result;
        }

        private void CheckForMajority(T who) {
            int voteCount = votes.Count(v => v.To == who);
            if (voteCount > Math.Floor(voters.Count / 2.0) && MajorityReached != null) {
                MajorityReached(this, new MajorityReachedEventArgs<T>(who));
            }
        }

        struct VoteStruct
        {
            T from;
            public T From {
                get { return from; }
            }

            T to;
            public T To {
                get { return to; }
            }

            public VoteStruct(T from, T to) {
                this.from = from;
                this.to = to;
            }
        }
    }

    [global::System.Serializable]
    public class AlreadyVotedException : VoteException
    {
        public AlreadyVotedException() { }
        public AlreadyVotedException(string message) : base(message) { }
        public AlreadyVotedException(string message, Exception inner) : base(message, inner) { }
        protected AlreadyVotedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [global::System.Serializable]
    public class NotVotedException : VoteException
    {
        public NotVotedException() { }
        public NotVotedException(string message) : base(message) { }
        public NotVotedException(string message, Exception inner) : base(message, inner) { }
        protected NotVotedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [global::System.Serializable]
    public class InvalidVoterException : VoteException
    {
        public InvalidVoterException() { }
        public InvalidVoterException(string message) : base(message) { }
        public InvalidVoterException(string message, Exception inner) : base(message, inner) { }
        protected InvalidVoterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [global::System.Serializable]
    public abstract class VoteException : MafiaException
    {
        public VoteException() { }
        public VoteException(string message) : base(message) { }
        public VoteException(string message, Exception inner) : base(message, inner) { }
        protected VoteException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
