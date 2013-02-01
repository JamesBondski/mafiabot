using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MafiaBotV2.MafiaLib;
using NUnit.Framework;

namespace MafiaBot.Test2.MafiaLib
{
    [TestFixture]
    public class BugsTest
    {
        Village village;
        Faction town;
        Faction mafia;

        public BugsTest() {}

        [SetUp]
        public void Init() {
            village = new Village(new MafiaBotV2.MafiaLib.Sources.BasicLayoutSource());
            town = village.Factions.First(v => v.Name == "Town");
            mafia = village.Factions.First(v => v.Name == "Mafia");

            NLog.Config.SimpleConfigurator.ConfigureForFileLogging("test.log", NLog.LogLevel.Debug);
        }

        [Test]
        public void JailerBlocked() {
            VillageMember jailer = town.CreateMember("J", "Jailer");
            VillageMember doc = town.CreateMember("D", "Doctor");
            VillageMember scum = mafia.CreateMember("M", "Roleblocker");

            village.Start();
            village.Phase.End();

            jailer.UsePower("jail", doc);
            doc.UsePower("protect", jailer);
            scum.UsePower("block", jailer);
            scum.Faction.UsePower("kill", scum, jailer);

            Assert.AreEqual(MemberState.Alive, jailer.State);
            Assert.AreEqual(MemberState.Alive, doc.State);
        }
    }
}
