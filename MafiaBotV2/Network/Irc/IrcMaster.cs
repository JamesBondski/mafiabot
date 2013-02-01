using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meebey.SmartIrc4net;

namespace MafiaBotV2.Network.Irc
{
    class IrcMaster : NetMaster
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        IrcClient client;
        internal IrcClient Client {
            get {
                return client;
            }
        }

        public IrcMaster(string host, int port) {
            // Init IRC client
            client = new IrcClient();
            client.Encoding = Encoding.UTF8;
            client.ActiveChannelSyncing = true;
            client.SendDelay = 1;

            Log.Info("Connecting to "+host+":"+port+".");
            client.Connect(host, port);

            if(!client.IsConnected) {
                throw new Exception("Could not connect to the server.");
            }

            // Hook up events
            client.OnChannelMessage += new IrcEventHandler(OnChannelMessage);
            client.OnQueryMessage += new IrcEventHandler(OnQueryMessage);
            client.OnKick += new KickEventHandler(OnKick);
            client.OnDisconnected += new EventHandler(OnDisconnect);
            client.OnJoin += new JoinEventHandler(OnJoin);
            client.OnPart += new PartEventHandler(OnPart);
            client.OnQuit += new QuitEventHandler(OnQuit);
        }

        public override bool ProcessMessage() {
            client.ListenOnce();
            return true;
        }

        public override void Close() {
            client.Disconnect();
        }

        void OnQueryMessage(object sender, IrcEventArgs e) {
            this.OnQueryMessage(GetUser(e.Data.Nick), e.Data.Message);
        }

        void OnChannelMessage(object sender, IrcEventArgs e) {
            this.OnChannelMessage(GetUser(e.Data.Nick), e.Data.Channel, e.Data.Message);
        }

        protected override NetChannel CreateChannel(string name) {
            return new IrcChannel(this, name);
        }

        protected override NetUser CreateUser(string name) {
            return new IrcUser(this, name);
        }

        void OnQuit(object sender, QuitEventArgs e) {
            if(Client.IsMe(e.Who)) {
                return;
            }

            foreach (KeyValuePair<string, NetChannel> channel in Channels) {
                if (channel.Value.Users.Contains(GetUser(e.Who))) {
                    this.UserLeft(channel.Value, GetUser(e.Who));
                }
            }
        }

        void OnPart(object sender, PartEventArgs e) {
            if(Client.IsMe(e.Who)) {
                return;
            }
            this.UserLeft(this.GetChannel(e.Channel), this.GetUser(e.Who));
        }

        void OnJoin(object sender, JoinEventArgs e) {
            if (Client.IsMe(e.Who)) {
                return;
            }
            this.UserJoined(this.GetChannel(e.Channel), this.GetUser(e.Who));
        }

        void OnDisconnect(object sender, EventArgs e) {
            throw new Exception("Got disconnected!");
        }

        void OnKick(object sender, KickEventArgs e) {
            if (Client.IsMe(e.Whom)) {
                throw new Exception("Got kicked!");
            }
            else {
                this.UserLeft(this.GetChannel(e.Channel), this.GetUser(e.Whom));
            }
        }

        public void Login(string nick, string realname, string password) {
            Log.Info("Logging on as " + nick + " (" + realname + "),");
            client.Login(nick, realname);
            if (!String.IsNullOrEmpty(password)) {
                Log.Info("Identifying to NickServ.");
                client.SendMessage(SendType.Message, "NickServ", "identify " + password);
            }
        }
    }
}
