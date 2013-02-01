using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;

namespace MafiaBotV2
{
    public enum GameState
    {
        Forming,
        Running,
        Over
    }

    public abstract class BasicGame : IGame
    {
        string name;
        public string Name {
            get { return name; }
        }

        string description;
        public string Description {
            get { return state.ToString() + description; }
            set { description = value; }
        }

        NetChannel channel;
        public MafiaBotV2.Network.NetChannel Channel {
            get { return channel; }
        }

        List<NetUser> players = new List<NetUser>();
        public List<NetUser> Players {
            get { return players; }
            set { players = value; }
        }

        List<NetUser> ignoredPlayers = new List<NetUser>();
        protected List<NetUser> IgnoredPlayers {
            get { return ignoredPlayers; }
        }

        GameState state = GameState.Forming;
        public GameState State {
            get { return state; }
        }

        NetUser creator;
        public MafiaBotV2.Network.NetUser Creator {
            get { return creator; }
            set { creator = value; }
        }

        int maxPlayers = -1;
        public int MaxPlayers {
            get { return maxPlayers; }
            set { maxPlayers = value; }
        }

        bool initalized = false;

        Bot bot;
        public MafiaBotV2.Bot Bot {
            get { return bot; }
        }

        public BasicGame(Bot bot, NetUser creator, string name, string channelPrefix) {
            this.name = name;
            this.creator = creator;

            this.bot = bot;
            this.channel = Bot.Master.GetChannel("#" + channelPrefix + name);

            this.channel.Joined += new EventHandler<UserEventArgs>(OnJoin);
            this.channel.Left += new EventHandler<UserEventArgs>(OnLeft);
        }

        public virtual void Start() {
            if (state != GameState.Forming) {
                throw new InvalidStateException("Game must be in Forming-state to be started");
            }

            if(maxPlayers != -1 && players.Count < maxPlayers) {
                throw new NotEnoughPlayersException("Need "+maxPlayers+" players, have only "+players.Count);
            }

            foreach(NetUser player in players) {
                player.Commands.RemoveAll(c => c is Commands.LeaveCommand && (c as Commands.LeaveCommand).Game == this);
                player.Commands.RemoveAll(c => c is Commands.JoinCommand && (c as Commands.JoinCommand).Game == this);
            }

            Channel.SetMode("+m");

            string donator = DonationMessage.GetMessage();
            if(donator != null) {
                Channel.SendMessage("This game is sponsored by " + donator);
            }

            state = GameState.Running;

            foreach (NetUser player in Players) {
                AddCommands(player);
            }
        }

        public void AddUser(NetUser user) {
            players.Add(user);
            channel.SendMessage(user.Name + " has joined the game (" + GetPlayerCount() + " now). To leave the game, leave the channel or use the ##leave command.");
        }

        public void RemoveUser(NetUser user) {
            players.Remove(user);
            channel.SendMessage(user.Name + " has left the game (" + GetPlayerCount() + " players now).");

            user.Commands.RemoveAll(c => c.Parent == this);
        }

        public virtual void VoiceEveryone(bool state) {
            string ModeCommand;
            if (state) {
                ModeCommand = "+";
            }
            else {
                ModeCommand = "-";
            }

            for (int i = 0; i < players.Count; i++) {
                ModeCommand += "v";
            }

            foreach (NetUser U in players) {
                if (state == false || !IgnoredPlayers.Contains(U)) {
                    ModeCommand += " " + U.Name;
                }
            }

            Channel.SetMode(ModeCommand);
        }

        protected virtual void AddCommands(NetUser user) {
            user.Commands.RemoveAll(c => c.Parent == this);
            switch(State) {
                case GameState.Forming:
                    user.Commands.Add(new Commands.LeaveCommand(this));
                    if(user == creator) {
                        user.Commands.Add(new Commands.StartCommand(this));
                    }
                    break;
            }

            user.Commands.Add(new Commands.HelpCommand());

            if(user == creator) {
                user.Commands.Add(new Commands.DestroyCommand(Bot.Games, this));
            }
        }

        void OnLeft(object sender, UserEventArgs e) {
            if (state == GameState.Forming) {
                RemoveUser(e.User);
                e.User.Commands.RemoveAll(c => c.Parent == this);
            }
        }

        void OnJoin(object sender, UserEventArgs e) {
            if(!initalized) {
                Initialize();
            }

            if(e.User == Creator) {
                Channel.Op(e.User.Name);
            }

            if (state == GameState.Forming && !IgnoredPlayers.Contains(e.User)) {
                AddUser(e.User);
            }

            if(players.Contains(e.User) || e.User == Creator) {
                AddCommands(e.User);
            }
        }

        private void Initialize() {
            Channel.SetMode("+NQ");

            initalized = true;
        }

        private string GetPlayerCount() {
            if (maxPlayers == -1) {
                return players.Count + " players";
            }
            else {
                return players.Count + "/" + maxPlayers + " players";
            }
        }

        public virtual void Destroy() {
            foreach(NetUser player in players) {
                player.Commands.RemoveAll(c => c.Parent == this);
            }

            Channel.SetMode("-m");
            players.Clear();
            Channel.Leave();
        }

        public override string ToString() {
            return Channel.Name + " (" + Description + ")";
        }

        [global::System.Serializable]
        public class InvalidStateException : MafiaException
        {
            public InvalidStateException() { }
            public InvalidStateException(string message) : base(message) { }
            public InvalidStateException(string message, Exception inner) : base(message, inner) { }
            protected InvalidStateException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }

        [global::System.Serializable]
        public class NotEnoughPlayersException : MafiaException
        {
            public NotEnoughPlayersException() { }
            public NotEnoughPlayersException(string message) : base(message) { }
            public NotEnoughPlayersException(string message, Exception inner) : base(message, inner) { }
            protected NotEnoughPlayersException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }
}
