using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;

namespace MafiaBotV2
{
    public interface IGame
    {
        string Name {
            get;
        }

        string Description {
            get;
        }

        NetUser Creator {
            get;
            set;
        }

        NetChannel Channel {
            get;
        }

        void Destroy();
    }
}
