using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TootRaftRush.Core
{
    public enum GameState
    {
        Ready,
        Playing,
        Paused,
        WipedOut,
        Finished
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Ready;
        public GameState CurrentState => currentState;

        [Header("Level Info")]
        [SerializeField] private string levelName = "Sunny Slide";
        [SerializeField] private int levelIndex = 1;
        public string LevelName => levelName;
        public int LevelIndex => levelIndex;

        [Header("Stats")]
        [SerializeField] private float elapsedTime = 0f;
        [SerializeField] private int deathCount = 0;
        public float ElapsedTime => elapsedTime;
        public int DeathCount => deathCount;

        [Header("References")]
        [SerializeField] private Gameplay.RaftController raft;
        [SerializeField] private Gameplay.CheckpointManager checkpointManager;

        public event Action<GameState> OnStateChanged;
        public event Action<int> OnDeathCountChanged;
        public event Action OnCheckpointReached;
        public event Action OnLevelCompleted;

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

            Time.timeScale = 1f;
        }

        private void Start()
        {
            if (raft == null)
            {
                raft = FindObjectOfType<Gameplay.RaftController>();
            }
            if (checkpointManager == null)
            {
                checkpointManager = FindObjectOfType<Gameplay.CheckpointManager>();
            }

            StartGame();
        }

        private void Update()
        {
            if (currentState == GameState.Playing)
            {
                elapsedTime += Time.deltaTime;
            }
        }

        public void StartGame()
        {
            elapsedTime = 0f;
            deathCount = 0;
            SetState(GameState.Playing);
        }

        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                SetState(GameState.Paused);
                Time.timeScale = 0f;
            }
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                Time.timeScale = 1f;
                SetState(GameState.Playing);
            }
        }

        public void TriggerWipeout(string hazardCause = "Obstacle")
        {
            if (currentState != GameState.Playing) return;

            deathCount++;
            OnDeathCountChanged?.Invoke(deathCount);
            StartCoroutine(WipeoutRoutine(hazardCause));
        }

        private IEnumerator WipeoutRoutine(string hazardCause)
        {
            SetState(GameState.WipedOut);

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlayWipeout();
            }

            if (raft != null)
            {
                raft.FreezeRaft(true);
            }

            // Show WIPED OUT screen for two seconds as per design requirement
            yield return new WaitForSeconds(2.0f);

            // Respawn raft and players at latest checkpoint
            if (checkpointManager != null && raft != null)
            {
                checkpointManager.RespawnRaft(raft);
            }

            if (raft != null)
            {
                raft.FreezeRaft(false);
            }

            SetState(GameState.Playing);
        }

        public void TriggerFinish()
        {
            if (currentState != GameState.Playing) return;

            SetState(GameState.Finished);

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlayVictory();
            }

            OnLevelCompleted?.Invoke();
        }

        public void SetState(GameState newState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadNextLevel()
        {
            Time.timeScale = 1f;
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
            }
            else
            {
                // Return to main menu if completed all levels
                SceneManager.LoadScene(0);
            }
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0); // MainMenu scene
        }

        public void NotifyCheckpoint()
        {
            OnCheckpointReached?.Invoke();
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlayCheckpoint();
            }
        }
    }
}
