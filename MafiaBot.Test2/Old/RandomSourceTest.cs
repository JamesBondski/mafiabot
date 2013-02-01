using MafiaBotV2.MafiaLib.Sources;
using MafiaBotV2.MafiaLib;
using NUnit.Framework;

namespace MafiaBot.Test
{
    
    
    /// <summary>
    ///This is a test class for RandomSourceTest and is intended
    ///to contain all RandomSourceTest Unit Tests
    ///</summary>
    [TestFixture]
    public class RandomSourceTest
    {
        /// <summary>
        ///A test for Load
        ///</summary>
        [Test]
        public void LoadTest() {
            int numPlayers = 7; 
            RandomSource target = new RandomSource(numPlayers); 
            Village village = new Village(target); 
            
            // Every faction should have a win condition
            bool hasEvil = false;
            foreach(Faction faction in village.Factions) {
                Assert.AreNotEqual(0, faction.WinConditions.Count);
                foreach(IWinCondition condition in faction.WinConditions) {
                    Assert.AreEqual(false, condition.Check(faction));
                }

                if(faction.Alignment == Alignment.Evil) {
                    hasEvil = true;
                }
            }
            Assert.AreEqual(true, hasEvil);

            // Number of players should match
            Assert.AreEqual(numPlayers, village.Members.Count);

            // Power roles should have the appropriate powers
            foreach(VillageMember member in village.Members) {
                if(member.Name == "Cop") {
                    Assert.IsTrue(member.GetPower("investigate") != null);
                }
                if(member.Name == "Doc") {
                    Assert.IsTrue(member.GetPower("protect") != null);
                }
            }
        }

    }
}
