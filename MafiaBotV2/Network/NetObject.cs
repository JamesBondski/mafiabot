using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MafiaBotV2.Network
{
    public abstract class NetObject
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler<MessageEventArgs> MessageReceived;

        protected MessageSource source;
        protected NetMaster master;

        List<ICommand> commands = new List<ICommand>();
        public List<ICommand> Commands {
            get { return commands; }
        }

        private string name;
        public string Name {
            get { return name; }
        }

        public NetMaster Master {
            get { return master; }
        }

        protected virtual string CommandPrefix {
            get { return "##"; }
        }

        public abstract void SendMessage(string text);

        protected NetObject(NetMaster master, string name, MessageSource source) {
            this.master = master;
            this.name = name;
            this.source = source;
        }

        internal virtual void HandleMessage(NetUser from, string message) {
            if (MessageReceived != null) {
                MessageReceived(this, new MessageEventArgs(from, message, source));
            }

            CommandParser parser = new CommandParser(CommandPrefix);
            parser.Parse(message);
            if(parser.IsValid) {
                HandleCommand(from, parser);
            }
        }

        internal virtual void HandleCommand(NetUser from, CommandParser parser) {
            HandleCommand(this, from, parser);
        }

        internal virtual void HandleCommand(NetObject source, NetUser from, CommandParser parser) {
            foreach(ICommand command in this.commands.FindAll(c => c.Name.ToLower() == parser.Command.ToLower())) {
                Log.Debug("Command ##" + parser.Command + " from " + from.Name + " >> " + command.GetType().FullName);
                if (command.AllowedInPublic || !(this is NetChannel)) {
                    string result = null;
                    try {
                        result = command.Execute(from, source, parser.Args);
                    }
                    catch(MafiaException ex) {
                        result = ex.Message;
                    }
                    catch(Exception ex) {
                        result = "Error: " + ex.ToString();
                        Log.Warn("Error executing command: " + ex.ToString() + " (" + ex.StackTrace + ")");
                    }
                    if (!String.IsNullOrEmpty(result)) {
                        source.SendMessage(result);
                    }

                    break;
                }
            }
        }
        
    }
}
