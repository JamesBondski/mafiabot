using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.MafiaGame.Commands
{
    class VariantCommand : ICommand
    {
        #region ICommand Members

        MafiaGame game;

        public VariantCommand(MafiaGame game) {
            this.game = game;
        }

        public string Name {
            get { return "variant"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        public object Parent {
            get { return game; }
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            if(args.Length<1) {
                return "Syntax: ##variant <name>";
            }

            List<string> argList = args.ToList();
            argList.RemoveAt(0);

            game.SetVariant(args[0], argList);
            return null;
        }

        #endregion
    }
}
