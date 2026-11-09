using System;
using System.Collections.Generic;
using UnityEngine;

namespace TootRaftRush.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class RaftController : MonoBehaviour
    {
        [Header("Physics Configuration")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float waterLinearDrag = 1.2f;
        [SerializeField] private float waterAngularDrag = 2.5f;
        [SerializeField] private float maxSpeed = 22f;
        [SerializeField] private float steeringTorqueFactor = 1.8f;
        [SerializeField] private float gravityCorrection = 18f;

        [Header("Buoyancy and Bobbing")]
        [SerializeField] private float bobbingAmplitude = 0.08f;
        [SerializeField] private float bobbingFrequency = 2.4f;
        [SerializeField] private float tiltFollowAngle = 14f;

        [Header("Deck Boundaries")]
        [SerializeField] private float raftRadius = 1.45f;
        public float RaftRadius => raftRadius;

        [Header("Seats / Positions")]
        [SerializeField] private Transform[] seatTransforms;
        public Transform[] SeatTransforms => seatTransforms;

        [Header("VFX & Feedback")]
        [SerializeField] private ParticleSystem waterWakeParticles;
        [SerializeField] private ParticleSystem waterSplashParticles;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private bool isFrozen = false;
        private float bobTimer = 0f;

        // Multi-player combo tracking
        private float lastFartTime = -1f;
        private int simultaneousFarts = 0;

        public Vector3 Velocity => rb != null ? rb.velocity : Vector3.zero;
        public float CurrentSpeed => rb != null ? rb.velocity.magnitude : 0f;

        private void Awake()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            rb.drag = waterLinearDrag;
            rb.angularDrag = waterAngularDrag;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }

        private void FixedUpdate()
        {
            if (isFrozen)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                return;
            }

            // Downward slope gravity along water channel
            rb.AddForce(Vector3.down * gravityCorrection, ForceMode.Acceleration);

            // Water bobbing effect
            bobTimer += Time.fixedDeltaTime * bobbingFrequency;
            float bobOffset = Mathf.Sin(bobTimer) * bobbingAmplitude * Time.fixedDeltaTime;
            transform.position += Vector3.up * bobOffset;

            // Speed clamping
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }

            // Align upright while allowing subtle tilting
            Vector3 predictedUp = Vector3.up;
            if (rb.velocity.sqrMagnitude > 0.1f)
            {
                // Banking into turns
                float angularYaw = rb.angularVelocity.y;
                predictedUp = Quaternion.AngleAxis(-angularYaw * tiltFollowAngle, rb.velocity.normalized) * Vector3.up;
            }
            Quaternion targetUpRot = Quaternion.FromToRotation(transform.up, predictedUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetUpRot, Time.fixedDeltaTime * 4.0f);

            // Water wake particle intensity
            if (waterWakeParticles != null)
            {
                var emission = waterWakeParticles.emission;
                emission.rateOverTime = Mathf.Clamp(CurrentSpeed * 4f, 0f, 60f);
            }
        }

        /// <summary>
        /// Applies fart impulse to the raft.
        /// Fart direction points backwards from the animal (pushing the raft opposite).
        /// If the player is on the edge, applies steering torque!
        /// </summary>
        public void ApplyFartImpulse(Vector3 contactPosition, Vector3 impulseDirection, float forceMagnitude)
        {
            if (isFrozen || rb == null) return;

            // Track co-op combo (farts happening within 0.4s window)
            float now = Time.time;
            if (now - lastFartTime < 0.4f)
            {
                simultaneousFarts++;
            }
            else
            {
                simultaneousFarts = 1;
            }
            lastFartTime = now;

            // Co-op combo bonus multiplier
            float comboBonus = 1.0f + (simultaneousFarts - 1) * 0.25f;
            float finalForce = forceMagnitude * comboBonus;

            // Force applied at contact position creates natural linear acceleration + torque
            rb.AddForceAtPosition(impulseDirection * finalForce, contactPosition, ForceMode.Impulse);

            // Steering evaluation relative to raft center
            Vector3 localContact = transform.InverseTransformPoint(contactPosition);
            // If contact is on right side (localContact.x > 0.3), farts push left -> turn right
            // If contact is on left side (localContact.x < -0.3), farts push right -> turn left
            float torqueDir = -localContact.x;
            rb.AddTorque(transform.up * (torqueDir * finalForce * steeringTorqueFactor), ForceMode.Impulse);

            // Water splash effect at contact point
            if (waterSplashParticles != null)
            {
                waterSplashParticles.transform.position = contactPosition;
                waterSplashParticles.Play();
            }
        }

        public void ApplySpeedBoost(float multiplier, float duration)
        {
            if (rb != null)
            {
                rb.AddForce(transform.forward * (maxSpeed * multiplier * 0.8f), ForceMode.VelocityChange);
            }
        }

        public void FreezeRaft(bool freeze)
        {
            isFrozen = freeze;
            if (freeze && rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        public void ResetToPosition(Vector3 newPosition, Quaternion newRotation)
        {
            transform.position = newPosition;
            transform.rotation = newRotation;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
