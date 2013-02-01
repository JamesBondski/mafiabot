using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using MafiaBotV2.Util;
using System.Xml;
using System.IO;
using System.Globalization;

namespace MafiaBotV2.MafiaLib.Sources
{
    public class RandomSource : BasicLayoutSource
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        int numPlayers;
        XElement profile;

        public RandomSource(int numPlayers) : this(numPlayers, "Default") {}

        public RandomSource(int numPlayers, string profileName) {
            this.numPlayers = numPlayers;
            XDocument profiles = XDocument.Load(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("MafiaBotV2.MafiaLib.Data.Random.Profiles.xml")));
            try {
                this.profile = profiles.Descendants("Profile").First(p => p.Element("Name").Value.ToLower() == profileName.ToLower());
            }
            catch (Exception ex) {
                throw new ProfileNotFoundException("Profile " + profile + " cannot be found.", ex);
            }
        }

        public override void Load(Village village) {
            base.Load(village);

            village.Rules.MaximumPopulation = numPlayers;

            int numTown = numPlayers - numPlayers * 2 / 7;
            if(numTown == numPlayers) {
                numTown--;
            }

            Faction town = village.Factions.First(f => f.Name == "Town");
            Faction mafia = village.Factions.First(f => f.Name == "Mafia");

            decimal maxBalance = (numPlayers - numTown) / (decimal)numTown * 5; //numMafia/numTown
            decimal minBalance = maxBalance - 2;
            Log.Debug("Balance : " + minBalance + " <---> " + maxBalance);

            decimal copMultiplier = 1;
            decimal nightMultiplier = 1;

            // Generate Mafia roles first
            // Create mafia members
            WeightedRandom<XElement> mafiaRandom = InitRandom("MafiaRoles");
            for (int i = 0; i < (numPlayers - numTown); i++) {
                XElement role = mafiaRandom.Choose();
                string roleName = role.Descendants("Name").First().Value;
                decimal roleCopMulti = Decimal.Parse(role.Descendants("CopReduction").First().Value, CultureInfo.InvariantCulture);
                decimal roleNightMulti = Decimal.Parse(role.Descendants("NightReduction").First().Value, CultureInfo.InvariantCulture);

                copMultiplier *= (1-roleCopMulti);
                nightMultiplier *= (1-roleNightMulti);

                mafia.CreateMember("Mafia", roleName);
                Log.Debug("Mafia-Role: " + roleName);

                if (roleName != "Mafia") {
                    mafiaRandom.Suppress(role);
                }
            }

            WeightedRandom<XElement> townRandom = InitRandom("TownRoles");
            decimal totalBalance = 0;
            for (int i = 0; i < numTown; i++) {
                bool roleFound = false;

                while (!roleFound) {
                    XElement role = townRandom.Choose();
                    string roleName = role.Descendants("Name").First().Value;
                    decimal roleCopValue = Decimal.Parse(role.Descendants("CopValue").First().Value, CultureInfo.InvariantCulture);
                    decimal roleNightValue = Decimal.Parse(role.Descendants("NightValue").First().Value, CultureInfo.InvariantCulture);
                    decimal roleAbsoluteValue = Decimal.Parse(role.Descendants("AbsoluteValue").First().Value, CultureInfo.InvariantCulture);

                    decimal addedBalance = roleAbsoluteValue + roleCopValue * copMultiplier + roleNightValue * nightMultiplier;
                    if (totalBalance + addedBalance < maxBalance) {
                        town.CreateMember(roleName, roleName);

                        if (roleName != "Vanilla Townie") {
                            townRandom.Suppress(role);
                        }
                        roleFound = true;
                        totalBalance += addedBalance;
                        Log.Debug("Town-Role: " + roleName);
                    }
                }
                Log.Debug("Total Balance: " + totalBalance);
            }

            town.WinConditions.Add(new NoEvilLeftCondition());
        }

        private WeightedRandom<XElement> InitRandom(string elementName) {
            WeightedRandom<XElement> random = new WeightedRandom<XElement>();
            foreach(XElement role in profile.Element(elementName).Descendants("Role")) {
                int weight = Int32.Parse(role.Element("Weight").Value);
                random.AddValue(weight, role);
            }

            return random;
        }

        [global::System.Serializable]
        public class ProfileNotFoundException : Exception
        {
            public ProfileNotFoundException() { }
            public ProfileNotFoundException(string message) : base(message) { }
            public ProfileNotFoundException(string message, Exception inner) : base(message, inner) { }
            protected ProfileNotFoundException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }
}
