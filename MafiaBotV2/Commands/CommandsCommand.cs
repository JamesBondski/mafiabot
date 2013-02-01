using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Util;
using MafiaBotV2.Network;

namespace MafiaBotV2.Commands
{
    class CommandsCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "commands"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return null; ; }
        }

        public string Execute(MafiaBotV2.Network.NetUser fromUser, MafiaBotV2.Network.NetObject source, string[] args) {
            string result = null;

            IEnumerable<string> commands;
            if (source is NetUser) {
                commands = from ICommand c in fromUser.Commands where c != this select c.Name;
            }
            else {
                //HACK: Need a better way to handle shoot
                commands = from ICommand c in fromUser.Commands 
                           where c != this && c.AllowedInPublic && c.Name != "shoot"
                           select c.Name;
            }

            if (commands.Count() > 0) {
                result = "Your commands: " + new ListFormatter<string>(commands).Format() + ".";
            }
            else {
                result = "You have no commands right now.";
            }

            if(source is NetChannel) {
                IEnumerable<string> channelCommands = from ICommand c in source.Commands select c.Name;
                if (channelCommands.Count() > 0) {
                    result += " Channel commands: " + new ListFormatter<string>(channelCommands).Format() + ".";
                }
            }

            return result;
        }

        #endregion
    }
}
