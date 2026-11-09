using System.IO;
using NUnit.Framework;

namespace TootRaftRush.Tests
{
    public class AssetIntegrityTests
    {
        [Test]
        public void Characters_FourAnimalModels_Exist()
        {
            Assert.IsTrue(File.Exists("Assets/Art/Characters/Hippo/Hippo.obj") || File.Exists("Assets/Art/Characters/Hippo/Hippo.glb"), "Hippo model missing");
            Assert.IsTrue(File.Exists("Assets/Art/Characters/Rhino/Rhino.obj") || File.Exists("Assets/Art/Characters/Rhino/Rhino.glb"), "Rhino model missing");
            Assert.IsTrue(File.Exists("Assets/Art/Characters/Frog/Frog.obj"), "Frog model missing");
            Assert.IsTrue(File.Exists("Assets/Art/Characters/Capybara/Capybara.obj"), "Capybara model missing");
        }

        [Test]
        public void Props_RaftModel_Exists()
        {
            Assert.IsTrue(File.Exists("Assets/Art/Props/Raft/Raft.obj"), "Raft model missing");
        }

        [Test]
        public void Obstacles_Models_Exist()
        {
            Assert.IsTrue(File.Exists("Assets/Art/Obstacles/SwingingBar/SwingingBar.obj"), "SwingingBar missing");
            Assert.IsTrue(File.Exists("Assets/Art/Obstacles/RotatingLog/RotatingLog.obj"), "RotatingLog missing");
            Assert.IsTrue(File.Exists("Assets/Art/Obstacles/MovingGate/MovingGate.obj"), "MovingGate missing");
        }

        [Test]
        public void Pickups_Models_Exist()
        {
            Assert.IsTrue(File.Exists("Assets/Art/Pickups/Beans/Pickup_Bean.obj"), "Bean model missing");
            Assert.IsTrue(File.Exists("Assets/Art/Pickups/Chili/Pickup_Chili.obj"), "Chili model missing");
            Assert.IsTrue(File.Exists("Assets/Art/Pickups/Soda/Pickup_Soda.obj"), "Soda model missing");
        }

        [Test]
        public void Audio_RequiredSoundFiles_Exist()
        {
            string[] audioFiles = new string[]
            {
                "Assets/Art/Audio/toot_hippo.wav",
                "Assets/Art/Audio/toot_rhino.wav",
                "Assets/Art/Audio/toot_frog.wav",
                "Assets/Art/Audio/toot_capybara.wav",
                "Assets/Art/Audio/splash.wav",
                "Assets/Art/Audio/wipeout.wav",
                "Assets/Art/Audio/checkpoint.wav",
                "Assets/Art/Audio/victory.wav",
                "Assets/Art/Audio/music_theme.wav"
            };

            foreach (var f in audioFiles)
            {
                Assert.IsTrue(File.Exists(f), $"Missing audio file: {f}");
            }
        }
    }
}
