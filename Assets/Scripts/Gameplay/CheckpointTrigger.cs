using UnityEngine;

namespace TootRaftRush.Gameplay
{
    public class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private int checkpointIndex = 0;
        [SerializeField] private bool isFinishLine = false;

        private bool triggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (triggered && isFinishLine) return;

            RaftController raft = other.GetComponentInParent<RaftController>();
            if (raft != null)
            {
                if (isFinishLine)
                {
                    triggered = true;
                    if (CheckpointManager.Instance != null)
                    {
                        CheckpointManager.Instance.OnFinishPassed();
                    }
                }
                else
                {
                    if (CheckpointManager.Instance != null)
                    {
                        CheckpointManager.Instance.OnCheckpointPassed(checkpointIndex, transform);
                    }
                }
            }
        }
    }
}
