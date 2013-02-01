using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MafiaBotV2.MafiaLib.Sources
{
    public class XmlSource : IVariantLoader
    {
        XmlDocument source;
        Village village;

        public XmlSource(string filename) {
            source = new XmlDocument();
            source.Load(filename);
        }

        public XmlSource(System.IO.Stream stream) {
            source = new XmlDocument();
            source.Load(stream);
        }

        public void Load(Village village) {
            this.village = village;

            InitVillage();
            ResolveCounts();

            foreach(XmlNode factionNode in source.SelectNodes("Game/Teams/Team")) {
                Faction faction = InitFaction(factionNode);
                village.Factions.Add(faction);
                foreach(XmlNode roleNode in factionNode.SelectNodes("Roles/Role")) {
                    int count = Int32.Parse(roleNode.SelectSingleNode("Count").InnerText);
                    for (int i = 0; i < count; i++) {
                        VillageMember role = faction.CreateMember(roleNode.SelectSingleNode("Name").InnerText);
                        InitRole(roleNode, role);
                        role.Faction = faction;
                    }
                }
            }
        }

        private Faction InitFaction(XmlNode factionNode) {
            Faction faction = new Faction(village, factionNode.SelectSingleNode("Name").InnerText);
            if (factionNode.SelectSingleNode("Evil") != null) {
                faction.Alignment = Alignment.Evil;
            }
            if(factionNode.SelectSingleNode("Nightkill") != null ) {
                faction.AddPower(new Powers.NightkillPower());
            }
            if(factionNode.SelectSingleNode("KnowsGroup") != null) {
                faction.AddPower(new Powers.MembersPower());
            }

            string[] conditions = factionNode.SelectSingleNode("WinsWhen").InnerText.Split('|');
            foreach (string condition in conditions) {
                switch (condition) {
                    case "MajorityOrEqual":
                        faction.WinConditions.Add(new MajorityOrEqualCondition());
                        break;
                    case "NoEvilLeft":
                        faction.WinConditions.Add(new NoEvilLeftCondition());
                        break;
                }
            }
            return faction;
        }

        private VillageMember InitRole(XmlNode roleNode, VillageMember member) {
            member.RoleName = roleNode.SelectSingleNode("Name").InnerText;
            if(roleNode.SelectSingleNode("Investigate") != null) {
                member.AddPower(new Powers.InvestigatePower());
            }
            if(roleNode.SelectSingleNode("Protect") != null) {
                member.AddPower(new Powers.ProtectPower());
            }
            if(roleNode.SelectSingleNode("Roleblock") != null) {
                member.AddPower(new Powers.RoleblockPower());
            }
            if(roleNode.SelectSingleNode("Daykill") != null) {
                bool endsDay = !String.IsNullOrEmpty(roleNode.SelectSingleNode("Daykill").InnerText);
                member.AddPower(new Powers.DaykillPower(endsDay));
            }
            if (roleNode.SelectSingleNode("Kingmake") != null) {
                member.ApplyEffect(new Effects.KingmakerEffect());
            }
            if(roleNode.SelectSingleNode("Hero") != null) {
                member.ApplyEffect(new Effects.HeroEffect());
            }
            return member;
        }

        private void InitVillage() {
            village.Name = source.SelectSingleNode("Game/Name").InnerText;
            village.Rules.MaximumPopulation = Int32.Parse(source.SelectSingleNode("Game/Players").InnerText);
            if (source.SelectSingleNode("Game/NoLynch") != null) {
                village.Rules.AllowNolynch = true;
            }
            if (source.SelectSingleNode("Game/Cardflip") != null) {
                village.Rules.CardFlip = true;
            }
            if (source.SelectSingleNode("Game/Night0") != null) {
                village.Rules.InitialPhase = PhaseType.Night0;
            }
            if(source.SelectSingleNode("Game/InitialPhase") != null) {
                string phaseName = source.SelectSingleNode("Game/InitialPhase").InnerText;
                village.Rules.InitialPhase = (PhaseType)Enum.Parse(typeof(PhaseType), phaseName);
            }
        }

        private void ResolveCounts() {
            int players = village.Rules.MaximumPopulation;
            Random rnd = new Random();
            XmlNode flexRole = null;

            foreach(XmlNode node in source.SelectNodes("Game/Teams/Team/Roles/Role/Count")) {
                int roleCount;
                if(Int32.TryParse(node.InnerText, out roleCount)) {
                    players -= roleCount;
                }
                else {
                    if(node.InnerText == "*") {
                        flexRole = node;
                    }
                    else {
                        try {
                            int index = node.InnerText.IndexOf('-');
                            int min = Int32.Parse(node.InnerText.Substring(0, index));
                            int max = Int32.Parse(node.InnerText.Substring(index + 1));

                            int count = rnd.Next(min, max + 1);
                            node.InnerText = count.ToString();
                            players -= count;
                        }
                        catch(Exception ex) {
                            throw new InvalidVariantException("Variant has at least one invalid role count.", ex);
                        }
                    }
                }
            }
            if(flexRole != null) {
                flexRole.InnerText = players.ToString();
            }
        }

        public class InvalidVariantException : Exception {
            public InvalidVariantException(string message) : base(message) {
            }

            public InvalidVariantException(string message, Exception inner)
                : base(message, inner) {
            }
        }
    }
}
