using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;

namespace MafiaBotV2
{
    public interface ICommand
    {
        string Name {
            get;
        }

        bool AllowedInPublic {
            get;
        }

        object Parent {
            get;
        }

        string Execute(NetUser from, NetObject source, string[] args);
    }
}
