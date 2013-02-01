using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Network.File
{
    class FileUser : NetUser
    {
        FileMaster fileMaster;

        public FileUser(FileMaster master, string name) : base(master, name) {
            this.fileMaster = master;
        }

        public override bool IsOpInChannel(string channelName) {
            return false;
        }

        public override void SendMessage(string text) {
            fileMaster.Output.WriteLine("W-" + this.Name + ": " + text);
        }
    }
}
