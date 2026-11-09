using System.Collections.Generic;
using UnityEngine;
using TootRaftRush.Core;

namespace TootRaftRush.Gameplay
{
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance { get; private set; }

        [Header("Checkpoints")]
        [SerializeField] private List<Transform> checkpointTransforms = new List<Transform>();
        [SerializeField] private int currentCheckpointIndex = 0;

        [Header("Finish Line")]
        [SerializeField] private Transform finishLineTransform;

        public int CurrentCheckpointIndex => currentCheckpointIndex;
        public int TotalCheckpoints => checkpointTransforms.Count;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        public void RegisterCheckpoint(Transform cp)
        {
            if (!checkpointTransforms.Contains(cp))
            {
                checkpointTransforms.Add(cp);
            }
        }

        public void OnCheckpointPassed(int index, Transform cpTransform)
        {
            if (index > currentCheckpointIndex)
            {
                currentCheckpointIndex = index;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.NotifyCheckpoint();
                }
            }
        }

        public void OnFinishPassed()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerFinish();
            }
        }

        public void RespawnRaft(RaftController raft)
        {
            if (raft == null) return;

            Vector3 respawnPos = Vector3.zero;
            Quaternion respawnRot = Quaternion.identity;

            if (checkpointTransforms.Count > 0 && currentCheckpointIndex < checkpointTransforms.Count)
            {
                Transform activeCP = checkpointTransforms[currentCheckpointIndex];
                respawnPos = activeCP.position + Vector3.up * 0.2f;
                respawnRot = activeCP.rotation;
            }
            else
            {
                respawnPos = raft.transform.position;
                respawnRot = raft.transform.rotation;
            }

            raft.ResetToPosition(respawnPos, respawnRot);

            // Reset players on raft
            PlayerController[] players = raft.GetComponentsInChildren<PlayerController>();
            for (int i = 0; i < players.Length; i++)
            {
                Vector3 seatPos = Vector3.zero;
                if (raft.SeatTransforms != null && i < raft.SeatTransforms.Length && raft.SeatTransforms[i] != null)
                {
                    seatPos = raft.SeatTransforms[i].localPosition;
                }
                else
                {
                    float angle = (i * 90f) * Mathf.Deg2Rad;
                    seatPos = new Vector3(Mathf.Sin(angle) * 0.9f, 0.2f, Mathf.Cos(angle) * 0.9f);
                }

                players[i].ResetPosition(seatPos);
                if (players[i].CharacterFartSystem != null)
                {
                    players[i].CharacterFartSystem.ResetMeter();
                }
            }
        }
    }
}
