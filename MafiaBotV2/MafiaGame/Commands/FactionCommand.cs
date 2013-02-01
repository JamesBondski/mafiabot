using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;
using MafiaBotV2.MafiaLib;
using System.Diagnostics;

namespace MafiaBotV2.MafiaGame.Commands
{
    class FactionCommand : ICommand
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

        public FactionCommand(MafiaGame game, VillageMember member, Power power) {
            this.game = game;
            this.member = member;
            this.power = power;

            this.name = power.Name;
            this.allowedInPublic = false;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            Debug.Assert(game.Mapper[from] == member);

            VillageMember target = null;
            if(args.Length > 0) {
                NetUser targetUser = game.Players.Find(p => p.Name.ToLower() == args[0].ToLower());
                if(targetUser != null) {
                    target = game.Mapper[targetUser];
                }
            }

            member.Faction.UsePower(power, member, target);

            return "Action submitted";
        }
    }
}
