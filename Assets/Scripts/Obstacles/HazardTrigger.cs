using UnityEngine;
using TootRaftRush.Core;
using TootRaftRush.Gameplay;

namespace TootRaftRush.Obstacles
{
    public class HazardTrigger : MonoBehaviour
    {
        [Header("Hazard Info")]
        [SerializeField] private string hazardName = "Pink Spikes";
        [SerializeField] private ParticleSystem impactParticles;

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision.gameObject);
        }

        private void HandleCollision(GameObject target)
        {
            // Check if player or raft hit the hazard
            RaftController raft = target.GetComponentInParent<RaftController>();
            PlayerController player = target.GetComponentInParent<PlayerController>();

            if (raft != null || player != null)
            {
                if (impactParticles != null)
                {
                    impactParticles.Play();
                }

                if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
                {
                    GameManager.Instance.TriggerWipeout(hazardName);
                }
            }
        }
    }
}
