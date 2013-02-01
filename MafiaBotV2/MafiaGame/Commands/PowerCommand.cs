using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;
using MafiaBotV2.MafiaLib;
using System.Diagnostics;

namespace MafiaBotV2.MafiaGame.Commands
{
    class PowerCommand : ICommand
    {
        string name;
        public string Name {
            get { return name; }
        }

        bool allowedInPublic;
        public bool AllowedInPublic {
            get { return allowedInPublic; }
        }

        public object Parent {
            get { return game; }
        }

        MafiaGame game;
        VillageMember member;
        Power power;
        public MafiaBotV2.MafiaLib.Power Power {
            get { return power; }
        }

        static readonly string[] publicCommands = { "vote", "unvote", "shoot" };

        public PowerCommand(MafiaGame game, VillageMember member, Power power) {
            this.game = game;
            this.member = member;
            this.power = power;

            this.name = power.Name;
            this.allowedInPublic = publicCommands.Contains(power.Name.ToLower());
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            Debug.Assert(game.Mapper[from] == member);

            if(this.AllowedInPublic && !(source is NetChannel)) {
                return "You have to issue this command in the channel.";
            }
            if(!this.AllowedInPublic && source is NetChannel) {
                return null;
            }

            VillageMember target = FindTarget(args);
            if (target != null && target.State == MemberState.Dead) {
                return args[0] + " is dead.";
            }

            string message = null;
            try {
                message = member.UsePower(power, target).ResultMessage;
            }
            catch (MafiaException ex) {
                return ex.Message;
            }

            if (source is NetChannel) {
                return null;
            }
            else {
                return message;
            }
        }

        public VillageMember FindTarget(string[] args)
        {
            VillageMember target = null;
            if (args.Length > 0)
            {
                NetUser targetUser = game.Players.Find(p => p.Name.ToLower() == args[0].ToLower());
                if (targetUser != null)
                {
                    target = game.Mapper[targetUser];
                }
            }
            if (target == null && power is MafiaLib.Powers.VotePower && args[0].ToLower() == "nolynch")
            {
                target = game.Village.Phase.Votes.NoLynchMember as MafiaLib.VillageMember;
            }
            return target;
        }
    }
}
