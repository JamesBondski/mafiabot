using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.Network
{
    public class UserEventArgs : EventArgs
    {
        private NetUser user;
        public NetUser User {
            get { return user; }
        }

        public UserEventArgs(NetUser user) {
            this.user = user;
        }
    }
}
