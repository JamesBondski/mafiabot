using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;
using System.Xml.Linq;
using MafiaBotV2.MafiaLib;
using MafiaBotV2.Util;

namespace MafiaBotV2.Barebones
{
    class BarebonesGame : BasicGame
    {
        bool voiceState = false;
        public bool VoiceState {
            get { return voiceState; }
        }

        VoteCounter<NetUser> votes;

        public BarebonesGame(Bot bot, NetUser creator, string name)
            : base(bot, creator, name, bot.Config.Descendants("Games").Descendants("Mafia").Descendants("Prefix").First().Value) {

            Channel.Commands.Add(new Commands.VoteCommand(this));
            Channel.Commands.Add(new Commands.UnvoteCommand(this));
            Channel.Commands.Add(new Commands.VotecountCommand(this));

            votes = new VoteCounter<NetUser>(Players, bot.Master.GetUser("Nolynch"));

            votes.MajorityReached += new EventHandler<MajorityReachedEventArgs<NetUser>>(OnMajorityReached);

            IgnoredPlayers.Add(creator);
        }

        void OnMajorityReached(object sender, MajorityReachedEventArgs<NetUser> e) {
            Channel.SendMessage(Kill(e.Winner));
            VoiceEveryone(false);
        }

        public override void Start() {
            base.Start();
            Channel.SendMessage("The game has started. You can talk as soon as the mod uses the ##voice command.");
            AddCommands(Creator);
            votes.Voters = new List<NetUser>(Players);
        }

        public string Kill(NetUser player) {
            if(!Players.Contains(player)) {
                return player.Name + " is not a player.";
            }
            if(IgnoredPlayers.Contains(player)) {
                return player.Name + " is already dead.";
            }

            IgnoredPlayers.Add(player);
            Channel.Devoice(player.Name);
            votes.Voters.Remove(player);

            return player.Name + " has been killed.";
        }

        public string Vote(NetUser source, NetUser target) {
            if(!Players.Contains(target)) {
                return target.Name + " is not in the game.";
            }

            if(votes.Vote(source, target)) {
                return votes.GetVoteCount(true);
            }
            else {
                return "You are already voting for someone.";
            }
        }

        public string Unvote(NetUser source) {
            votes.Unvote(source);
            return votes.GetVoteCount(true);
        }

        public void ResetVotes() {
            votes.Clear();
        }

        public string GetVoteCount() {
            return votes.GetVoteCount(true);
        }

        protected override void AddCommands(NetUser user) {
            base.AddCommands(user);

            switch(State) {
                case GameState.Running:
                    if(user == Creator) {
                        user.Commands.Add(new Commands.VoiceCommand(this, !voiceState));
                        user.Commands.Add(new Commands.KillCommand(this));
                        user.Commands.Add(new Commands.ResetCommand(this));
                        user.Commands.Add(new Commands.AddCommand(this));
                    }
                    break;
            }
        }

        public override void VoiceEveryone(bool state) {
            voiceState = state;
            base.VoiceEveryone(state);
            Creator.Commands.RemoveAll(c => c is Commands.VoiceCommand);
            Creator.Commands.Add(new Commands.VoiceCommand(this, !state));
        }
    }
}
