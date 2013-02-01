using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Network.Irc
{
    class IrcUser : NetUser
    {
        IrcMaster ircMaster;

        public IrcUser(IrcMaster master, string name) : base(master, name) {
            this.master = master;
            this.ircMaster = master as IrcMaster;
        }

        public override bool IsOpInChannel(string channelName) {
            return ircMaster.Client.GetChannelUser(channelName, this.Name).IsOp;
        }

        public override void SendMessage(string text) {
            ircMaster.Client.SendMessage(Meebey.SmartIrc4net.SendType.Message, Name, text);
        }
    }
}
