using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.Network;
using MafiaBotV2.MafiaLib;

namespace MafiaBotV2.MafiaGame
{
    public class VillageConnector : IOutsideConnector
    {
        MafiaGame game;

        public VillageConnector(MafiaGame game) {
            this.game = game;
        }

        public void SendMessage(string message) {
            game.Channel.SendMessage(message);
        }

        public void AllowTalking(bool state) {
            game.VoiceEveryone(state);
        }

        public void Highlight(bool state) {
        }
    }

}