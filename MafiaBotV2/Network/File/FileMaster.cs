using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MafiaBotV2.Network.File
{
    public class FileMaster : NetMaster
    {
        StreamWriter output;
        public System.IO.StreamWriter Output {
            get { return output; }
        }

        StreamReader input;

        List<FileUser> bots = new List<FileUser>();

        public FileMaster(string inputFile, string outputFile) 
            : this(new StreamReader(inputFile), new StreamWriter(outputFile)) {
        }

        public FileMaster(StreamReader input, StreamWriter output) {
            this.output = output;
            this.output.AutoFlush = true;
            this.input = input;
        }

        public override bool ProcessMessage() {
            string line = input.ReadLine();
            try {
                ExecuteLine(line);
            }
            catch(Exception ex) {
                output.WriteLine(ex.ToString());
            }
            return !input.EndOfStream;
        }

        public override void Close() {
            input.Dispose();
            output.Dispose();
        }

        protected override NetChannel CreateChannel(string name) {
            return new FileChannel(this, name);
        }

        protected override NetUser CreateUser(string name) {
            return new FileUser(this, name);
        }

        private void ExecuteLine(string line) {
            char type = line[0];
            line = line.Substring(2);

            string command = line.Substring(0, line.IndexOf(' '));
            string args = line.Substring(command.Length + 1);
            switch(type) {
                case 'G':
                    ExecuteGeneral(command, args);
                    break;
                case 'A':
                    foreach(FileUser user in bots) {
                        ExecuteUser(user, command, args);
                    }
                    break;
                default:
                    ExecuteUser(bots[Int32.Parse(type.ToString())], command, args);
                    break;
            }
        }

        private void ExecuteUser(FileUser user, string command, string args) {
            switch(command) {
                case "say":
                    string channel = args.Substring(0, args.IndexOf(' '));
                    string message = args.Substring(channel.Length + 1);
                    OnChannelMessage(user, channel, message);
                    break;
                case "whisper":
                    OnQueryMessage(user, args);
                    break;
                case "join":
                    GetChannel(args).HandleJoined(user);
                    GetChannel(args).Users.Add(user);
                    break;
                case "leave":
                    GetChannel(args).HandleLeft(user);
                    GetChannel(args).Users.Remove(user);
                    break;
            }
        }

        private void ExecuteGeneral(string command, string args) {
            switch(command) {
                case "bots":
                    bots.Clear();
                    for (int i = 0; i < Int32.Parse(args); i++) {
                        bots.Add(GetUser("Bot" + i) as FileUser);
                    }
                    break;
            }
        }
    }
}
