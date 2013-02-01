using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.Network
{
    public abstract class NetChannel : NetObject
    {
        public event EventHandler<UserEventArgs> Joined;
        public event EventHandler<UserEventArgs> Left;

        List<NetUser> users = new List<NetUser>();
        public List<NetUser> Users {
            get { return users; }
        }

        internal NetChannel(NetMaster master, string name) : base(master, name, MessageSource.Channel) {
        }

        internal override void HandleCommand(NetUser from, CommandParser parser) {
            base.HandleCommand(from, parser);
            from.HandleCommand(this, from, parser);
        }

        public abstract void SetMode(string mode);

        public abstract void SetTopic(string topic);

        public abstract void Op(string user);

        public abstract void Halfop(string user);

        public abstract void Voice(string user);

        public abstract void Devoice(string user);

        public abstract void Invite(string user);

        public virtual void Leave() {
            Master.Leave(Name);
        }

        internal void HandleJoined(NetUser user) {
            if (Joined != null) {
                Joined(this, new UserEventArgs(user));
            }
            UpdateUsers();
        }

        internal void HandleLeft(NetUser user) {
            if (Left != null) {
                Left(this, new UserEventArgs(user));
            }
            UpdateUsers();
        }

        protected abstract void UpdateUsers();
    }
}
