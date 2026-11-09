using NUnit.Framework;
using UnityEngine;
using TootRaftRush.Gameplay;

namespace TootRaftRush.Tests
{
    public class RaftPhysicsTests
    {
        private GameObject raftObject;
        private RaftController raft;
        private Rigidbody rb;

        [SetUp]
        public void Setup()
        {
            raftObject = new GameObject("TestRaft");
            rb = raftObject.AddComponent<Rigidbody>();
            raft = raftObject.AddComponent<RaftController>();
        }

        [TearDown]
        public void Teardown()
        {
            if (raftObject != null)
            {
                Object.DestroyImmediate(raftObject);
            }
        }

        [Test]
        public void RaftController_Initialization_ConfiguresRigidbody()
        {
            Assert.IsNotNull(rb);
            Assert.AreEqual(1.45f, raft.RaftRadius);
        }

        [Test]
        public void RaftController_ApplyFartImpulse_AppliesVelocity()
        {
            Vector3 contact = raftObject.transform.position;
            Vector3 forwardImpulse = Vector3.forward;

            raft.ApplyFartImpulse(contact, forwardImpulse, 10f);

            // Rigidbody has received velocity impulse
            Assert.Greater(rb.velocity.z, 0f);
        }

        [Test]
        public void RaftController_OffCenterFart_GeneratesTorque()
        {
            // Apply impulse on the right edge of raft
            Vector3 rightEdgeContact = raftObject.transform.position + Vector3.right * 1.0f;
            Vector3 forwardImpulse = Vector3.forward;

            raft.ApplyFartImpulse(rightEdgeContact, forwardImpulse, 10f);

            // Torque should rotate the raft
            Assert.AreNotEqual(0f, rb.angularVelocity.y);
        }

        [Test]
        public void RaftController_FreezeRaft_StopsAllVelocity()
        {
            rb.velocity = new Vector3(5, 0, 5);
            rb.angularVelocity = new Vector3(0, 2, 0);

            raft.FreezeRaft(true);

            Assert.AreEqual(Vector3.zero, rb.velocity);
            Assert.AreEqual(Vector3.zero, rb.angularVelocity);
        }
    }
}
