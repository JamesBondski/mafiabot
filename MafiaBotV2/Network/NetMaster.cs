using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.Network
{
    public abstract class NetMaster
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler<MessageEventArgs> ChannelMessage;
        public event EventHandler<MessageEventArgs> UserQuery;

        Dictionary<string, NetChannel> channels = new Dictionary<string,NetChannel>();
        public Dictionary<string, NetChannel> Channels {
            get { return channels; }
        }
        Dictionary<string, NetUser> users = new Dictionary<string, NetUser>();
        public Dictionary<string, NetUser> Users {
            get { return users; }
        }
        
        public NetMaster() {
        }

        public NetChannel GetChannel(string name) {
            if (!channels.ContainsKey(name)) {
                Log.Info("Joining channel " + name + ".");
                channels.Add(name, CreateChannel(name));
            }
            return channels[name];
        }

        internal void Leave(string name) {
            if (channels.ContainsKey(name)) {
                Log.Info("Leaving channel " + name + ".");
                channels.Remove(name);
            }
        }

        public NetUser GetUser(string nickname) {
            if (users.ContainsKey(nickname)) {
                return users[nickname];
            }

            Log.Info("Creating user " + nickname);
            NetUser user = CreateUser(nickname);
            users.Add(nickname, user);
            return user;
        }

        public abstract bool ProcessMessage();

        public abstract void Close();

        protected abstract NetChannel CreateChannel(string name);

        protected abstract NetUser CreateUser(string name);

        protected void OnChannelMessage(NetUser user, string channel, string message) {
            if (channels.ContainsKey(channel)) {
                channels[channel].HandleMessage(user, message);
            }
            if (ChannelMessage != null) {
                ChannelMessage(this, new MessageEventArgs(user, message, MessageSource.Channel, channel));
            }
        }

        protected void OnQueryMessage(NetUser user, string message) {
            user.HandleMessage(user, message);
            if (UserQuery != null) {
                UserQuery(this, new MessageEventArgs(user, message, MessageSource.Query));
            }
        } 

        protected void UserJoined(NetChannel channel, NetUser user) {
            if (channel != null) { 
                channel.HandleJoined(user);
            }
        }

        protected void UserLeft(NetChannel channel, NetUser user) {
            if (channel != null) {
                channel.HandleLeft(user);
            }
        }
    }
}
