using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Network.Irc
{
    class IrcChannel : NetChannel
    {
		static readonly NLog.Logger Log=NLog.LogManager.GetCurrentClassLogger();
		
        IrcMaster ircMaster;

        public IrcChannel(IrcMaster master, string name) : base(master, name) {
            this.ircMaster = master;
            if (!ircMaster.Client.JoinedChannels.Contains(Name)) {
                ircMaster.Client.RfcJoin(Name);
            }
        }

        public override void SetMode(string mode) {
            ircMaster.Client.RfcMode(Name, mode);
        }

        public override void SetTopic(string topic) {
            ircMaster.Client.RfcTopic(Name, topic);
        }

        public override void Op(string user) {
            ircMaster.Client.Op(Name, user);
        }

        public override void Halfop(string user) {
            ircMaster.Client.Halfop(Name, user, Meebey.SmartIrc4net.Priority.Medium);
        }

        public override void Voice(string user) {
            ircMaster.Client.Voice(Name, user);
        }

        public override void Devoice(string user) {
            ircMaster.Client.Devoice(Name, user);
        }

        public override void Invite(string user) {
            ircMaster.Client.RfcInvite(user, Name);
        }

        public override void Leave() {
            base.Leave();
            ircMaster.Client.RfcPart(Name);
        }

        public override void SendMessage(string text) {
            ircMaster.Client.SendMessage(Meebey.SmartIrc4net.SendType.Message, Name, text);
        }

        protected override void UpdateUsers() {
            Users.Clear();
			try {
	            System.Collections.Hashtable ircUsers = ircMaster.Client.GetChannel(Name).Users;
	            foreach(System.Collections.DictionaryEntry user in ircUsers) {
	                string name = user.Key as string;
	                Users.Add(ircMaster.GetUser(name));
	            }
			}
			catch(Exception ex) {
				Log.Warn("Error updating user list: " + ex.ToString());
			}
        }
    }
}
