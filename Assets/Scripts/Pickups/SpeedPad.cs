using UnityEngine;
using TootRaftRush.Gameplay;

namespace TootRaftRush.Pickups
{
    public class SpeedPad : MonoBehaviour
    {
        [Header("Boost Settings")]
        [SerializeField] private float speedMultiplier = 1.6f;
        [SerializeField] private float duration = 2.5f;

        [Header("VFX")]
        [SerializeField] private ParticleSystem boostParticles;

        private void OnTriggerEnter(Collider other)
        {
            RaftController raft = other.GetComponentInParent<RaftController>();
            if (raft != null)
            {
                raft.ApplySpeedBoost(speedMultiplier, duration);

                if (boostParticles != null)
                {
                    boostParticles.Play();
                }

                if (Audio.AudioManager.Instance != null)
                {
                    Audio.AudioManager.Instance.PlaySplash();
                }

                if (CameraController.Instance != null)
                {
                    CameraController.Instance.TriggerFovKick(6f);
                }
            }
        }
    }
}
