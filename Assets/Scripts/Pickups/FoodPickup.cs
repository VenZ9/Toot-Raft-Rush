using System.Collections;
using UnityEngine;
using TootRaftRush.Gameplay;

namespace TootRaftRush.Pickups
{
    public enum PickupType
    {
        Beans,
        Chili,
        Soda
    }

    public class FoodPickup : MonoBehaviour
    {
        [Header("Pickup Settings")]
        [SerializeField] private PickupType pickupType = PickupType.Beans;
        [SerializeField] private float meterReward = 35f;
        [SerializeField] private float respawnTime = 6.0f;

        [Header("Visual Animation")]
        [SerializeField] private float bobHeight = 0.25f;
        [SerializeField] private float bobSpeed = 3.0f;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private Transform modelMesh;

        [Header("VFX")]
        [SerializeField] private ParticleSystem collectParticles;

        private Vector3 initialLocalPos;
        private bool isCollected = false;
        private Collider triggerCollider;

        private void Start()
        {
            if (modelMesh == null)
            {
                modelMesh = transform;
            }
            initialLocalPos = modelMesh.localPosition;
            triggerCollider = GetComponent<Collider>();

            // Setup reward amounts according to design
            switch (pickupType)
            {
                case PickupType.Beans:
                    meterReward = 35f;
                    break;
                case PickupType.Chili:
                    meterReward = 60f;
                    break;
                case PickupType.Soda:
                    meterReward = 100f;
                    break;
            }
        }

        private void Update()
        {
            if (isCollected) return;

            // Bobbing & spinning
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            modelMesh.localPosition = initialLocalPos + Vector3.up * bob;
            modelMesh.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime), Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            RaftController raft = other.GetComponentInParent<RaftController>();
            PlayerController player = other.GetComponentInParent<PlayerController>();

            if (raft != null || player != null)
            {
                Collect(raft);
            }
        }

        private void Collect(RaftController raft)
        {
            isCollected = true;

            // Play pickup sound
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlayPickup(pickupType);
            }

            // Play particles
            if (collectParticles != null)
            {
                collectParticles.Play();
            }

            // Reward players on raft
            if (raft != null)
            {
                PlayerController[] players = raft.GetComponentsInChildren<PlayerController>();
                foreach (var p in players)
                {
                    if (p.CharacterFartSystem != null)
                    {
                        p.CharacterFartSystem.AddFartMeter(meterReward);

                        if (pickupType == PickupType.Chili)
                        {
                            p.CharacterFartSystem.ActivateSupercharge(2.0f, 6.0f);
                        }
                    }
                }

                if (pickupType == PickupType.Soda)
                {
                    raft.ApplySpeedBoost(1.3f, 3.0f);
                }
            }

            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            if (modelMesh != null)
            {
                modelMesh.gameObject.SetActive(false);
            }
            if (triggerCollider != null)
            {
                triggerCollider.enabled = false;
            }

            yield return new WaitForSeconds(respawnTime);

            if (modelMesh != null)
            {
                modelMesh.gameObject.SetActive(true);
            }
            if (triggerCollider != null)
            {
                triggerCollider.enabled = true;
            }
            isCollected = false;
        }
    }
}
