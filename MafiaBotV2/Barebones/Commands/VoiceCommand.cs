using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Barebones.Commands
{
    class VoiceCommand : ICommand
    {
        #region ICommand Members

        public string Name {
            get { return "voice"; }
        }

        public bool AllowedInPublic {
            get { return true; }
        }

        BarebonesGame game;
        bool state;

        public object Parent {
            get { return game; }
        }

        public VoiceCommand(BarebonesGame game, bool state) {
            this.game = game;
            this.state = state;
        }

        public string Execute(MafiaBotV2.Network.NetUser from, MafiaBotV2.Network.NetObject source, string[] args) {
            game.VoiceEveryone(state);

            from.Commands.Remove(this);
            from.Commands.Add(new VoiceCommand(game, !state));

            return null;
        }

        #endregion
    }
}
