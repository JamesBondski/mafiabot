using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib.Powers
{
    public class VotePower : Power
    {
        public override bool Instant {
            get { return true; }
        }

        public override int Priority {
            get { return 0; }
        }
		
		PhaseType[] phases = { PhaseType.Day };
		public override PhaseType[] Phases {
			get {
				return phases;
			}
		}

        IRestriction[] restrictions = { };
        public override IRestriction[] Restrictions {
            get { return restrictions; }
        }

        public override string Name {
            get { return "vote"; }
        }

        public VotePower() {
        }

        internal override void Execute(Action action) {
            if(action.Target == null) {
                throw new MafiaException("You need to specify a valid target for your vote.");
            }

            MafiaVoteCounter counter = action.Village.Phase.Votes;
            if (action.Target == counter.NoLynchMember && !action.Village.Rules.AllowNolynch) {
                throw new NoLynchNotAllowedException("Nolynch is not allowed in this game.");
            }
            if (!counter.Vote(action.Who as VillageMember, action.Target as VillageMember)) {
                throw new AlreadyVotedException("You have already voted for someone.");
            }
            action.Village.Outside.SendMessage(counter.GetVoteCount(true));
        }

        [global::System.Serializable]
        public class AlreadyVotedException : MafiaException
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
        public class NoLynchNotAllowedException : MafiaException
        {
            public NoLynchNotAllowedException() { }
            public NoLynchNotAllowedException(string message) : base(message) { }
            public NoLynchNotAllowedException(string message, Exception inner) : base(message, inner) { }
            protected NoLynchNotAllowedException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }
}
