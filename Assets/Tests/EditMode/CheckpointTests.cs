using NUnit.Framework;
using UnityEngine;
using TootRaftRush.Gameplay;

namespace TootRaftRush.Tests
{
    public class CheckpointTests
    {
        private GameObject managerObject;
        private CheckpointManager checkpointManager;

        [SetUp]
        public void Setup()
        {
            managerObject = new GameObject("TestCheckpointManager");
            checkpointManager = managerObject.AddComponent<CheckpointManager>();
        }

        [TearDown]
        public void Teardown()
        {
            if (managerObject != null)
            {
                Object.DestroyImmediate(managerObject);
            }
        }

        [Test]
        public void CheckpointManager_InitialCheckpoint_IsZero()
        {
            Assert.AreEqual(0, checkpointManager.CurrentCheckpointIndex);
        }

        [Test]
        public void CheckpointManager_OnCheckpointPassed_AdvancesIndex()
        {
            GameObject cp1 = new GameObject("CP1");
            checkpointManager.RegisterCheckpoint(cp1.transform);

            checkpointManager.OnCheckpointPassed(1, cp1.transform);
            Assert.AreEqual(1, checkpointManager.CurrentCheckpointIndex);

            // Re-passing lower checkpoint does not revert
            checkpointManager.OnCheckpointPassed(0, cp1.transform);
            Assert.AreEqual(1, checkpointManager.CurrentCheckpointIndex);

            Object.DestroyImmediate(cp1);
        }
    }
}
