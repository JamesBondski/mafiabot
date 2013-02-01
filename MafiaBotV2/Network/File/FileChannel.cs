using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Network.File
{
    class FileChannel : NetChannel
    {
        public FileChannel(FileMaster master, string name) : base(master, name) {
            (master as FileMaster).Output.WriteLine("Joined-"+Name+":" + name);
        }

        public override void SetMode(string mode) {
            (master as FileMaster).Output.WriteLine("Mode-" + Name + ":" + mode);
        }

        public override void SetTopic(string topic) {
            (master as FileMaster).Output.WriteLine("Topic-" + Name + ":" + topic);
        }

        public override void Op(string user) {
            (master as FileMaster).Output.WriteLine("Op-" + Name + ":" + user);
        }

        public override void Halfop(string user) {
            (master as FileMaster).Output.WriteLine("Halfop-" + Name + ":" + user);
        }

        public override void Voice(string user) {
            (master as FileMaster).Output.WriteLine("Voice-" + Name + ":" + user);
        }

        public override void Devoice(string user) {
            (master as FileMaster).Output.WriteLine("Devoice-" + Name + ":" + user);
        }

        public override void Invite(string user) {
            (master as FileMaster).Output.WriteLine("Invite-" + Name + ":" + user);
        }

        public override void Leave() {
            base.Leave();
            (master as FileMaster).Output.WriteLine("Left-" + Name + ":" + this.Name);
        }

        protected override void UpdateUsers() {
        }

        public override void SendMessage(string text) {
            (master as FileMaster).Output.WriteLine("C-" + this.Name + ":" + text);
        }
    }
}
