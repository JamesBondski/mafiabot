using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;
using System.Xml.Linq;
using MafiaBotV2.MafiaLib;
using MafiaBotV2.Util;
using MafiaBotV2.MafiaGame.Http;

namespace MafiaBotV2.MafiaGame
{
    public class MafiaGame : BasicGame
    {
        HttpInterface http = null;
        public HttpInterface Http {
            get { return http; }
            set { http = value; }
        }

        string currentVariant;
        public string CurrentVariant {
            get { return currentVariant; }
            set { currentVariant = value; }
        }

        Mapper<NetUser, VillageMember> mapper = new Mapper<NetUser, VillageMember>();
        public Mapper<NetUser, VillageMember> Mapper {
            get { return mapper; }
        }

        string variantName = null;
        Village village;
        public MafiaBotV2.MafiaLib.Village Village {
            get { return village; }
        }

        public MafiaGame(Bot bot, NetUser creator, string name)
            : base(bot, creator, name, bot.Config.Descendants("Games").Descendants("Mafia").Descendants("Prefix").First().Value) {

            SetVariant("random", new List<string>());
        }

        public void SetVariant(string variant, List<string> args) {
            variantName = variant;
            if (variantName.ToLower() == "random") {
                if (args.Count > 0) {
                    village = new Village(new MafiaLib.Sources.RandomSource(Math.Max(3, Players.Count), args[0]));
                }
                else {
                    village = new Village(new MafiaLib.Sources.RandomSource(Math.Max(3, Players.Count)));
                }
                this.MaxPlayers = -1;
            }
            else {
                village = VariantManager.Instance.CreateVillage(variant);
                this.MaxPlayers = village.Members.Count;
            }

            if (village == null) {
                variantName = null;
                throw new UnknownVariantException("Variant " + variant + " not found.");
            }

            Channel.SendMessage("Variant set to " + village.Name + ". Players: " + Players.Count + "/" + village.Members.Count);
        }

        void OnPhaseOver(object sender, PhaseOverEventArgs e) {
            int toLynch = (int)Math.Floor(village.AliveMembers.Count / 2.0) + 1;
            string topic = Name + " (Variant " + variantName + "), " + e.NewPhase.Type.ToString() + ". ";
            switch (e.NewPhase.Type) {
                case PhaseType.Day:
                    topic += village.AliveMembers.Count + " alive, " + toLynch + " to lynch.";
                    break;
            }

            Channel.SetTopic(topic);

            Scheduler.Instance.RemoveAll(this);
            if (e.NewPhase.Type == PhaseType.Night) {
                SetDeadline(DateTime.Now.AddSeconds(180));
            }
            if (e.NewPhase.Type == PhaseType.Dawn || e.NewPhase.Type == PhaseType.Twilight) {
                SetDeadline(DateTime.Now.AddSeconds(60));
            }
        }

        void OnGameOver(object sender, GameOverEventArgs e) {
            Bot.Games.Destroy(this);

            Bot.MainChannel.SendMessage("Game " + Name + ": " + e.WinMessage);
            if (http != null) {
                http.Close();
            }
        }

        public void SetDeadline(DateTime executionTime) {
            Scheduler.Instance.QueueTask(this, executionTime, new ScheduledTaskHandler(ExecuteTask));

            TimeSpan duration = executionTime - DateTime.Now;
            if (duration.TotalSeconds > 10) {
                Scheduler.Instance.QueueTask(this, executionTime.Subtract(new TimeSpan(0, 0, 10)), new ScheduledTaskHandler(Warn), "10");
                if (duration.TotalSeconds > 60) {
                    Scheduler.Instance.QueueTask(this, executionTime.Subtract(new TimeSpan(0, 1, 0)), new ScheduledTaskHandler(Warn), "60");
                }
            }

            Channel.SendMessage("Deadline set in " + duration.Minutes + ":" + duration.Seconds.ToString("00"));
        }

        private void Warn(Scheduler scheduler, object[] args) {
            this.Channel.SendMessage("Deadline in " + args[0] + " seconds.");

            foreach (VillageMember member in village.AliveMembers) {
                if (member.HasUnusedPowers) {
                    member.Outside.SendMessage("You have not submitted all your actions.");
                }
            }
        }

        private void ExecuteTask(Scheduler scheduler, object[] args) {
            this.Village.Phase.End();
        }

        public override void Start() {
            if (Players.Any(p => p.Name.ToLower() == "nolynch")) {
                this.RemoveUser(Players.Find(p => p.Name.ToLower() == "nolynch"));
                throw new Exception("Users may not have the name NoLynch. The game has not been started.");
            }

            if (variantName == null) {
                throw new UnknownVariantException("You need to set a variant before starting the game.");
            }
            else if (variantName == "random" && http == null) {
                village = new Village(new MafiaLib.Sources.RandomSource(Players.Count));
                this.MaxPlayers = Players.Count;
                Channel.SendMessage("Generated random variant for " + Players.Count + " players.");
            }

            village.GameOver += new EventHandler<GameOverEventArgs>(OnGameOver);
            village.PhaseOver += new EventHandler<PhaseOverEventArgs>(OnPhaseOver);

            village.Outside = new VillageConnector(this);

            if (MaxPlayers == -1) {
                MaxPlayers = Village.Rules.MaximumPopulation;
            }
            Players.RemoveRange(MaxPlayers, Players.Count - MaxPlayers);
            //HACK
            if (!(Creator is MafiaBotV2.Network.File.FileUser)) {
                Players = Players.Shuffle();
            }

            // Give players their roles
            int index = 0;
            foreach (VillageMember member in village.Members) {
                NetUser player = Players[index];
                member.Name = player.Name;
                mapper.Add(player, member);
                member.Outside = new MafiaUser(this, player, member);
                member.Killed += new EventHandler<VillageMemberEventArgs>(OnKilled);
                index++;
            }

            foreach (VillageMember member in village.Members) {
                (member.Outside as MafiaUser).SendRoleInfo();
            }

            base.Start();
            village.Start();

            Channel.Commands.Add(new Commands.VoteCountCommand(this));

            if (http != null && Players.Contains(Creator)) {
                Bot.MainChannel.SendMessage("Keep in mind " + Creator.Name + " knows the setup.");
            }
            Bot.MainChannel.SendMessage("A game has started in " + Channel.Name + ". You can join the channel to watch it!");
        }

        public void Replace(NetUser oldUser, NetUser newUser) {
            oldUser.Commands.RemoveAll(c => c.Parent == this);
            Players.Remove(oldUser);
            Players.Add(newUser);
            VillageMember member = mapper.Remove(oldUser);
            member.Name = newUser.Name;
            mapper.Add(newUser, member);
            member.Outside = new MafiaUser(this, newUser, member);
            AddCommands(newUser);
        }

        void OnKilled(object sender, VillageMemberEventArgs e) {
            IgnoredPlayers.Add(mapper[e.Member]);
            mapper[e.Member].Commands.RemoveAll(c => c.Parent == this);
        }

        public override void Destroy() {
            if (State != GameState.Forming) {
                ShowRoles();
            }
            Scheduler.Instance.RemoveAll(this);

            base.Destroy();
        }

        private void ShowRoles() {
            Channel.SendMessage("Role list: ");
            foreach (VillageMember member in village.Members) {
                Channel.SendMessage(member.Name + ": " + member.RoleName);
            }
        }

        protected override void AddCommands(NetUser user) {
            base.AddCommands(user);
            switch (State) {
                case GameState.Forming:
                    if (user == Creator) {
                        this.Creator.Commands.Add(new Commands.VariantCommand(this));
                        this.Creator.Commands.Add(new Commands.AdminCommand(this));
                    }
                    break;
                case GameState.Running:
                    if (Players.Contains(user) && !IgnoredPlayers.Contains(user)) {
                        (Mapper[user].Outside as MafiaUser).AddMafiaCommands();
                        user.Commands.Add(new Commands.RoleCommand(this));
                    }
                    if (user == Creator) {
                        this.Creator.Commands.Add(new Commands.ReplaceCommand(this));
                        this.Creator.Commands.Add(new Commands.ModkillCommand(this));
                        this.Creator.Commands.Add(new Commands.DeadlineCommand(this));
                    }
                    break;
            }
        }

        [global::System.Serializable]
        public class UnknownVariantException : MafiaException
        {
            public UnknownVariantException() { }
            public UnknownVariantException(string message) : base(message) { }
            public UnknownVariantException(string message, Exception inner) : base(message, inner) { }
            protected UnknownVariantException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }
}
