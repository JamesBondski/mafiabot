using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MafiaBotV2.MafiaLib
{
    public enum Alignment {
        Good,
        Evil
    }

    public class Faction : Actor
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        List<VillageMember> members = new List<VillageMember>();
        public List<VillageMember> Members {
            get { return members; }
        }

        Alignment alignment = Alignment.Good;
        public Alignment Alignment {
            get { return alignment; }
            set { alignment = value; }
        }

        public List<VillageMember> AliveMembers {
            get {
                return (from m in members where m.State == MemberState.Alive select m).ToList();
            }
        }

        public List<IWinCondition> winConditions = new List<IWinCondition>();
        public List<IWinCondition> WinConditions {
            get { return winConditions; }
        }

        internal Faction(Village village, string name) : base(village, ActorType.Faction) {
            this.Name = name;
            this.Outside = new DefaultFactionConnector(this);
        }

        public VillageMember CreateMember(string name) {
            return CreateMember(name, "Vanilla Townie");
        }

        public VillageMember CreateMember(string name, string role) {
            Log.Debug("Creating member of faction " + this.Name + " (" + name + "," + role + ")");

            VillageMember member = new VillageMember(Village, role);
            member.Name = name;
            member.Faction = this;
            RoleManager.Instance.InitRole(member, role);
            this.Members.Add(member);

            Village.Rules.MaximumPopulation = Village.Members.Count;

            return member;
        }

        public void RemoveMember(string name) {
            RemoveMember(members.First(m => m.Name.ToLower() == name.ToLower()));
        }

        public void RemoveMember(VillageMember member) {
            members.Remove(member);
            if (Village != null) {
                Village.Members.Remove(member);
            }

            Village.Rules.MaximumPopulation = Village.Members.Count;
        }

        public override void RaisePreActionSource(Action action) {
            base.RaisePreActionSource(action);
            action.Initiator.RaisePreActionSource(action);
        }

        public override void RaisePostActionSource(Action action) {
            base.RaisePostActionSource(action);
            action.Initiator.RaisePostActionSource(action);
        }
    }
}
