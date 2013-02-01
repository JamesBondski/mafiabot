using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.MafiaLib
{
    public interface IOutsideConnector
    {
        void SendMessage(string message);
        void AllowTalking(bool state);

        void Highlight(bool state);
    }

    public class DummyOutsideConnector : IOutsideConnector {

        #region IOutsideConnector Members

        public void SendMessage(string message) {
        }

        public void AllowTalking(bool state) {
        }

        public void Highlight(bool state) {
        }

        #endregion
    }

    public class DefaultFactionConnector : IOutsideConnector {

        Faction faction;

        public DefaultFactionConnector(Faction faction) {
            this.faction = faction;
        }

        #region IOutsideConnector Members

        public void SendMessage(string message) {
            foreach(VillageMember member in faction.Members) {
                member.Outside.SendMessage(message);
            }
        }

        public void AllowTalking(bool state) {
            foreach (VillageMember member in faction.Members) {
                member.Outside.AllowTalking(state);
            }
        }

        public void Highlight(bool state) {
            foreach(VillageMember member in faction.Members) {
                member.Outside.Highlight(state);
            }
        }

        #endregion
    }
}
