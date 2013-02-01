using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaGame.Commands
{
    class RoleCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "role"; }
        }

        public bool AllowedInPublic {
            get { return false; }
        }

        MafiaGame game;
        public object Parent {
            get { return game; }
        }

        public RoleCommand(MafiaGame game) {
            this.game = game;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            (game.Mapper[from].Outside as MafiaUser).SendRoleInfo();
            return null;
        }

        #endregion
    }
}
