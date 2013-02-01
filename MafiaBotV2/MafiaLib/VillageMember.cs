using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib
{
    public enum MemberState
    {
        Alive,
        Dead
    }

    public class VillageMember : Actor
    {
        MemberState state;
        public MemberState State {
            get { return state; }
        }

        DeathCauseType deathCause = DeathCauseType.NotDead;
        public DeathCauseType DeathCause {
            get { return deathCause; }
        }

        Faction faction;
        public MafiaBotV2.MafiaLib.Faction Faction {
            get { return faction; }
            set { faction = value; }
        }

        string roleName;
        public string RoleName {
            get { return roleName; }
            set { roleName = value; }
        }

        public event EventHandler<VillageMemberEventArgs> Killed;

        public VillageMember(Village village, string name)
            : base(village, ActorType.Person) {
            this.RoleName = name;

            if (Village != null) {
                Village.Members.Add(this);
            }
        }

        public Action UsePower(string powerName, Actor target) {
            Power power = GetPower(powerName);
            return UsePower(power, target);
        }

        public Action UsePower(Power power, Actor target) {
            return UsePower(power, this as VillageMember, target);
        }

        public void Kill() {
            Kill(DeathCauseType.Generic);
        }

        public void Kill(DeathCauseType cause) {
            state = MemberState.Dead;
            deathCause = cause;

            string killMessage = Name;

            if (Village.Rules.CardFlip) {
                killMessage += " (" + RoleName + ")";
            }

            switch (cause) {
                case DeathCauseType.Lynch:
                    killMessage += " has been lynched.";
                    break;
                case DeathCauseType.MafiaKill:
                    killMessage += " has been shot.";
                    break;
                case DeathCauseType.Hero:
                    killMessage += " has been slain by the hero.";
                    break;
                case DeathCauseType.King:
                    killMessage += " has been executed by the king.";
                    break;
                default:
                    killMessage += " has been killed.";
                    break;
            }
            Village.Outside.SendMessage(killMessage);
            Outside.AllowTalking(false);

            if(Killed != null) {
                Killed(this, new VillageMemberEventArgs(this));
            }

            Powers.Clear();
        }
    }

    public class VillageMemberEventArgs : EventArgs {
        VillageMember member;
        public MafiaBotV2.MafiaLib.VillageMember Member {
            get { return member; }
            set { member = value; }
        }

        public VillageMemberEventArgs(VillageMember member) {
            this.member = member;
        }
    }

    public enum DeathCauseType
    {
        Lynch,
        MafiaKill,
        Generic,
        NotDead,
        Hero,
        King
    }
}
