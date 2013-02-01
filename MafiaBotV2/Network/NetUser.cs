using System;
using System.Collections.Generic;
using System.Text;
using MafiaBotV2.Util;

namespace MafiaBotV2.Network
{
    public abstract class NetUser : NetObject, IVoter
    {
        protected override string CommandPrefix {
            get {
                return "";
            }
        }

        internal NetUser(NetMaster master, string nick) 
            : base(master, nick, MessageSource.Query) {
            
            this.Commands.Add(new Commands.CommandsCommand());
        }

        public abstract bool IsOpInChannel(string channelName);

        public bool IsOpInChannel(NetChannel channel) {
            return IsOpInChannel(channel.Name);
        }
    }
}
