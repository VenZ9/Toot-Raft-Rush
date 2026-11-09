using NUnit.Framework;
using UnityEngine;
using TootRaftRush.Gameplay;

namespace TootRaftRush.Tests
{
    public class FartSystemTests
    {
        private GameObject testObject;
        private FartSystem fartSystem;
        private PlayerController playerController;

        [SetUp]
        public void Setup()
        {
            testObject = new GameObject("TestPlayer");
            playerController = testObject.AddComponent<PlayerController>();
            fartSystem = testObject.GetComponent<FartSystem>();
        }

        [TearDown]
        public void Teardown()
        {
            if (testObject != null)
            {
                Object.DestroyImmediate(testObject);
            }
        }

        [Test]
        public void FartSystem_InitialMeter_IsFull()
        {
            Assert.AreEqual(100f, fartSystem.CurrentMeter);
            Assert.AreEqual(1.0f, fartSystem.MeterPercentage);
            Assert.IsTrue(fartSystem.CanFart);
        }

        [Test]
        public void FartSystem_QuickFart_ConsumesBaseCost()
        {
            fartSystem.StartCharging();
            fartSystem.ReleaseChargeAndFart();

            // Quick fart consumes ~25 points
            Assert.LessOrEqual(fartSystem.CurrentMeter, 75f);
        }

        [Test]
        public void FartSystem_AddFartMeter_IncreasesAndClampsToMax()
        {
            fartSystem.StartCharging();
            fartSystem.ReleaseChargeAndFart();
            float before = fartSystem.CurrentMeter;

            fartSystem.AddFartMeter(15f);
            Assert.AreEqual(before + 15f, fartSystem.CurrentMeter);

            // Adding beyond 100 clamps to 100
            fartSystem.AddFartMeter(500f);
            Assert.AreEqual(100f, fartSystem.CurrentMeter);
        }

        [Test]
        public void FartSystem_Supercharge_ActivatesAndMultiplies()
        {
            fartSystem.ActivateSupercharge(2.0f, 5.0f);
            Assert.IsTrue(fartSystem.IsSupercharged);
        }
    }
}
