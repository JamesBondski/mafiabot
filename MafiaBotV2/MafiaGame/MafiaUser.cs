using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.MafiaLib;
using MafiaBotV2.MafiaGame.Commands;
using MafiaBotV2.Network;

namespace MafiaBotV2.MafiaGame
{
    class MafiaUser : IOutsideConnector
    {
        NetUser user;
        MafiaGame game;
        VillageMember member;

        public MafiaUser(MafiaGame game, NetUser user, VillageMember member) {
            this.user = user;
            this.game = game;
            this.member = member;

            AddMafiaCommands();

            member.PowerGained += new EventHandler<PowerEventArgs>(OnPowerGained);
            member.PowerLost += new EventHandler<PowerEventArgs>(OnPowerLost);

            member.Faction.PowerGained += new EventHandler<PowerEventArgs>(OnFactionPowerGained);
            member.Faction.PowerLost += new EventHandler<PowerEventArgs>(OnFactionPowerLost);
        }

        void OnFactionPowerLost(object sender, PowerEventArgs e) {
            user.Commands.RemoveAll(c => c is FactionCommand && (c as FactionCommand).Power == e.Power);
            user.SendMessage("Your faction has lost a power: " + e.Power.Name);
        }

        void OnFactionPowerGained(object sender, PowerEventArgs e) {
            user.Commands.Add(new FactionCommand(game, member, e.Power));
            user.SendMessage("Your faction has gained a new power: " + e.Power.Name);
        }

        void OnPowerLost(object sender, PowerEventArgs e) {
            user.Commands.RemoveAll(c => c is PowerCommand && (c as PowerCommand).Power == e.Power);
            user.SendMessage("You have lost a power: " + e.Power.Name);
        }

        void OnPowerGained(object sender, PowerEventArgs e) {
            user.Commands.Add(new PowerCommand(game, member, e.Power));
            user.SendMessage("You have gained a new power: " + e.Power.Name);
        }

        public void SendMessage(string message) {
            user.SendMessage(message);
        }

        public void AllowTalking(bool state) {
            if (state) {
                game.Channel.Voice(user.Name);
            }
            else {
                game.Channel.Devoice(user.Name);
            }
        }

        public void Highlight(bool state) {
            if (state) {
                game.Channel.SetMode("+h " + user.Name);
            }
            else {
                game.Channel.SetMode("-h " + user.Name);
            }
        }

        public void AddMafiaCommands() {
            foreach (Power power in member.Powers) {
                user.Commands.Add(new PowerCommand(game, member, power));
            }

            // Add faction commands until a better solution exists
            foreach (Power power in member.Faction.Powers) {
                user.Commands.Add(new FactionCommand(game, member, power));
            }

            user.Commands.Add(new SkipCommand(game));
        }

        public void SendRoleInfo() {
            StringBuilder builder = new StringBuilder();
            builder.Append("You are a ").Append(member.RoleName).Append(". ");

            IEnumerable<Power> powers = member.Powers.FindAll(p => p.Name != "vote" && p.Name != "unvote");
            if (powers.Count() == 0) {
                builder.Append("You do not have any special powers. ");
            }
            else {
                builder.Append("You have the following powers: ");
                foreach (Power power in powers) {
                    if (power.Name != "vote" && power.Name != "unvote") {
                        builder.Append(power.Name);
                        if(power.Charges != 0) {
                            builder.Append(" (" + power.Charges + " charges)");
                        }
                        builder.Append(", ");
                    }
                }
            }
            builder.Remove(builder.Length - 2, 2).Append(". ");
            builder.Append("You belong to faction ").Append(member.Faction.Name).Append(". ");
            if (member.Faction.Powers.Count > 0) {
                builder.Append("Your faction has the following powers: ");
                foreach (Power power in member.Faction.Powers) {
                    builder.Append(power.Name).Append(", ");
                }
                builder.Remove(builder.Length - 2, 2);
            }
            user.SendMessage(builder.ToString());

            Power membersPower = member.Faction.GetPower("members");
            if (membersPower != null) {
                member.Faction.UsePower("members", member, null);
            }
        }
    }
}
