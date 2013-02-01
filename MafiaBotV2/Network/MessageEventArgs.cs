using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.Network
{
    public enum MessageSource
    {
        Channel,
        Query,
        Unknown
    }

    public class MessageEventArgs : EventArgs
    {
        private string message;
        public string Message {
            get { return message; }
        }

        private NetUser from;
        public NetUser From {
            get { return from; }
        }

        private MessageSource source;
        public MessageSource Source {
            get { return source; }
        }

        private string sourceInfo;
        public string SourceInfo {
            get { return sourceInfo; }
        }

        public MessageEventArgs(NetUser from, string message, MessageSource source, string sourceInfo) {
            this.message = message;
            this.from = from;
            this.source = source;
            this.sourceInfo = sourceInfo;
        }

        public MessageEventArgs(NetUser from, string message, MessageSource source) 
            : this(from,message,source,null) {
        }

        public MessageEventArgs(NetUser from, string message) 
            : this(from, message, MessageSource.Unknown) {
        }

        public void Reply(string message) {
            if (source == MessageSource.Query) {
                From.SendMessage(message);
            }
            else {
                From.Master.GetChannel(SourceInfo).SendMessage(message);
            }
        }
    }
}
