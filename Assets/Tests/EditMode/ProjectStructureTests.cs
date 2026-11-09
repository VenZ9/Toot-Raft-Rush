using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace TootRaftRush.Tests
{
    public class ProjectStructureTests
    {
        [Test]
        public void Project_RequiredScenes_ExistOnDisk()
        {
            string[] requiredScenes = new string[]
            {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/CharacterSelection.unity",
                "Assets/Scenes/Lobby.unity",
                "Assets/Scenes/SunnySlide.unity",
                "Assets/Scenes/SpikyCanyon.unity",
                "Assets/Scenes/TurboRapids.unity"
            };

            foreach (var scenePath in requiredScenes)
            {
                Assert.IsTrue(File.Exists(scenePath), $"Missing required scene: {scenePath}");
            }
        }

        [Test]
        public void Project_RequiredScripts_ExistOnDisk()
        {
            string[] requiredScripts = new string[]
            {
                "Assets/Scripts/Gameplay/RaftController.cs",
                "Assets/Scripts/Gameplay/PlayerController.cs",
                "Assets/Scripts/Gameplay/FartSystem.cs",
                "Assets/Scripts/Obstacles/ObstacleSwing.cs",
                "Assets/Scripts/Gameplay/CheckpointManager.cs",
                "Assets/Scripts/Core/GameManager.cs",
                "Assets/Scripts/UI/UIManager.cs",
                "Assets/Scripts/Multiplayer/NetworkManager.cs",
                "Assets/Scripts/Audio/AudioManager.cs"
            };

            foreach (var scriptPath in requiredScripts)
            {
                Assert.IsTrue(File.Exists(scriptPath), $"Missing required script: {scriptPath}");
            }
        }

        [Test]
        public void Project_PackagesManifest_ContainsURP()
        {
            string manifestPath = "Packages/manifest.json";
            Assert.IsTrue(File.Exists(manifestPath), "Packages/manifest.json missing");
            string content = File.ReadAllText(manifestPath);
            Assert.IsTrue(content.Contains("com.unity.render-pipelines.universal"), "URP missing from manifest");
        }
    }
}
