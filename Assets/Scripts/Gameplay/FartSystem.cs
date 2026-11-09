using System;
using UnityEngine;

namespace TootRaftRush.Gameplay
{
    public class FartSystem : MonoBehaviour
    {
        [Header("Meter Configuration")]
        [SerializeField] private float currentFartMeter = 100f;
        [SerializeField] private float maxFartMeter = 100f;
        [SerializeField] private float passiveRegenRate = 8.5f; // points per sec
        [SerializeField] private float baseCost = 25f;

        [Header("Fart Physics")]
        [SerializeField] private float baseForce = 12f;
        [SerializeField] private float maxChargeForceMultiplier = 2.4f;
        [SerializeField] private float maxChargeDuration = 1.4f;

        [Header("VFX & Particles")]
        [SerializeField] private ParticleSystem gasCloudParticles;
        [SerializeField] private ParticleSystem gasRingParticles;
        [SerializeField] private ParticleSystem superchargeFlameParticles;

        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private RaftController raft;

        private bool isCharging = false;
        private float chargeTimer = 0f;
        private float superchargeTimer = 0f;
        private float superchargeMultiplier = 1.0f;

        public float CurrentMeter => currentFartMeter;
        public float MaxMeter => maxFartMeter;
        public float MeterPercentage => Mathf.Clamp01(currentFartMeter / maxFartMeter);
        public bool IsCharging => isCharging;
        public float ChargeRatio => Mathf.Clamp01(chargeTimer / maxChargeDuration);
        public bool CanFart => currentFartMeter >= baseCost;
        public bool IsSupercharged => superchargeTimer > 0f;

        public event Action<float> OnMeterChanged;
        public event Action<float> OnFartExecuted; // force

        private void Awake()
        {
            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();
            }
        }

        private void Start()
        {
            if (raft == null)
            {
                raft = GetComponentInParent<RaftController>() ?? FindObjectOfType<RaftController>();
            }

            currentFartMeter = maxFartMeter;
            OnMeterChanged?.Invoke(MeterPercentage);
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // Passive meter regeneration
            if (!isCharging && currentFartMeter < maxFartMeter)
            {
                currentFartMeter = Mathf.Min(maxFartMeter, currentFartMeter + passiveRegenRate * dt);
                OnMeterChanged?.Invoke(MeterPercentage);
            }

            // Supercharge timer countdown
            if (superchargeTimer > 0f)
            {
                superchargeTimer -= dt;
                if (superchargeTimer <= 0f)
                {
                    superchargeMultiplier = 1.0f;
                    if (superchargeFlameParticles != null && superchargeFlameParticles.isPlaying)
                    {
                        superchargeFlameParticles.Stop();
                    }
                }
            }

            // Charge accumulation
            if (isCharging)
            {
                chargeTimer += dt;
                if (chargeTimer > maxChargeDuration)
                {
                    chargeTimer = maxChargeDuration;
                }
            }
        }

        public void StartCharging()
        {
            if (!CanFart || isCharging) return;

            isCharging = true;
            chargeTimer = 0f;

            if (Audio.AudioManager.Instance != null && playerController.IsHumanPlayer)
            {
                Audio.AudioManager.Instance.PlayFartCharge();
            }
        }

        public void ReleaseChargeAndFart()
        {
            if (!isCharging) return;

            float ratio = ChargeRatio;
            isCharging = false;

            if (currentFartMeter < baseCost)
            {
                chargeTimer = 0f;
                return;
            }

            // Scaled cost from 25 to 50
            float cost = baseCost + ratio * 25f;
            currentFartMeter = Mathf.Max(0f, currentFartMeter - cost);
            OnMeterChanged?.Invoke(MeterPercentage);

            // Compute fart impulse force
            float forceMultiplier = Mathf.Lerp(1.0f, maxChargeForceMultiplier, ratio);
            if (IsSupercharged)
            {
                forceMultiplier *= superchargeMultiplier;
            }
            float finalForce = baseForce * forceMultiplier;

            // Direction: player forward pushes raft forward
            Vector3 fartDirection = transform.forward;
            Vector3 contactPoint = transform.position;

            if (raft != null)
            {
                raft.ApplyFartImpulse(contactPoint, fartDirection, finalForce);
            }

            // Trigger VFX
            TriggerFartVFX(ratio);

            // Trigger character audio
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlayCharacterFart(playerController.CharacterAnimalType, ratio);
            }

            // Trigger camera shake & FOV kick for human player
            if (playerController.IsHumanPlayer && CameraController.Instance != null)
            {
                float shakeIntensity = 0.3f + ratio * 0.7f;
                CameraController.Instance.TriggerShake(shakeIntensity, 0.25f + ratio * 0.25f);
                CameraController.Instance.TriggerFovKick(3.0f + ratio * 8.0f);
            }

            // Trigger character animation
            var visual = GetComponentInChildren<Utility.CharacterVisualController>();
            if (visual != null)
            {
                visual.TriggerFartBurst(ratio);
            }

            OnFartExecuted?.Invoke(finalForce);
            chargeTimer = 0f;
        }

        private void TriggerFartVFX(float chargeRatio)
        {
            Vector3 spawnPos = playerController.WorldFartPosition;
            Quaternion spawnRot = Quaternion.LookRotation(-transform.forward, Vector3.up);

            if (gasCloudParticles != null)
            {
                gasCloudParticles.transform.position = spawnPos;
                gasCloudParticles.transform.rotation = spawnRot;
                var main = gasCloudParticles.main;
                main.startSize = Mathf.Lerp(1.2f, 3.2f, chargeRatio);
                gasCloudParticles.Play();
            }

            if (chargeRatio > 0.4f && gasRingParticles != null)
            {
                gasRingParticles.transform.position = spawnPos;
                gasRingParticles.transform.rotation = spawnRot;
                gasRingParticles.Play();
            }
        }

        public void AddFartMeter(float amount)
        {
            currentFartMeter = Mathf.Min(maxFartMeter, currentFartMeter + amount);
            OnMeterChanged?.Invoke(MeterPercentage);
        }

        public void ActivateSupercharge(float multiplier, float duration)
        {
            superchargeMultiplier = multiplier;
            superchargeTimer = duration;

            if (superchargeFlameParticles != null)
            {
                superchargeFlameParticles.Play();
            }
        }

        public void ResetMeter()
        {
            currentFartMeter = maxFartMeter;
            isCharging = false;
            chargeTimer = 0f;
            superchargeTimer = 0f;
            superchargeMultiplier = 1.0f;
            OnMeterChanged?.Invoke(MeterPercentage);
        }
    }
}
