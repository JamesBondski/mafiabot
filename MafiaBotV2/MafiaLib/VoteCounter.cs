using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MafiaBotV2.Util;

namespace MafiaBotV2.MafiaLib
{
    public class MafiaVoteCounter : VoteCounter<VillageMember>
    {
        public MafiaVoteCounter(List<VillageMember> population)
            : base(population, new VillageMember(null, "Nolynch")) {
        }

        public override bool Vote(VillageMember from, VillageMember to) {
            if(to == this.NoLynchMember && !from.Village.Rules.AllowNolynch) {
                throw new NoLynchNotAllowedException("No lynch is not enabled for this game.");
            }

            bool success = base.Vote(from, to);
            if (!success) {
                return false;
            }

            if (from is VillageMember) {
                (from as VillageMember).Killed += new EventHandler<VillageMemberEventArgs>(OnActorKilled);
            }
            if (to is VillageMember) {
                (to as VillageMember).Killed += new EventHandler<VillageMemberEventArgs>(OnActorKilled);
            }

            return true;
        }

        void OnActorKilled(object sender, VillageMemberEventArgs e) {
            this.ClearVoter(e.Member);
        }
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
