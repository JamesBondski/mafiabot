using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2
{
    class CommandParser
    {
        private string prefix;
        public string Prefix {
            get { return prefix; }
            set { prefix = value; }
        }

        private string command;
        public string Command {
            get { return command; }
        }

        private string[] args;
        public string[] Args {
            get { return args; }
        }

        private bool isValid;
        public bool IsValid {
            get { return isValid; }
        }

        public CommandParser()
            : this("") {
        }

        public CommandParser(string prefix) {
            this.prefix = prefix;
        }

        public void Parse(string message) {
            if (String.IsNullOrEmpty(message)) {
                return;
            }

            isValid = false;
            args = new string[] { };
            command = "";

            if (!String.IsNullOrEmpty(Prefix) && !message.StartsWith(Prefix)) {
                return;
            }
            if (String.IsNullOrEmpty(message)) {
                return;
            }

            isValid = true;

            message = message.Substring(Prefix.Length);
            int index = message.IndexOf(' ');
            if (index == -1) {
                command = message.ToLower();
                return;
            }

            command = message.Substring(0, index).ToLower();
            message = message.Substring(index + 1);
            args = message.Split(' ');
        }
    }
}
