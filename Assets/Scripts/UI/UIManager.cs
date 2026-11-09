using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TootRaftRush.Core;
using TootRaftRush.Gameplay;

namespace TootRaftRush.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Elements")]
        [SerializeField] private Text timerText;
        [SerializeField] private Text deathCountText;
        [SerializeField] private Text levelTitleText;
        [SerializeField] private GameObject checkpointBanner;

        [Header("Player Fart Meters (4 Animals)")]
        [SerializeField] private Image meterHippo;
        [SerializeField] private Image meterRhino;
        [SerializeField] private Image meterFrog;
        [SerializeField] private Image meterCapybara;

        [Header("Touch Controls")]
        [SerializeField] private FartButton fartButton;
        [SerializeField] private Button jumpButton;

        [Header("Screens & Overlays")]
        [SerializeField] private GameObject wipedOutOverlay;
        [SerializeField] private GameObject winScreenOverlay;
        [SerializeField] private Text winTimeText;
        [SerializeField] private Text winDeathsText;
        [SerializeField] private GameObject pauseMenuOverlay;
        [SerializeField] private GameObject settingsOverlay;

        private bool fartReleasedFlag = false;
        private bool jumpPressedFlag = false;

        public bool IsFartButtonHeld => fartButton != null && fartButton.IsHeld;

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

        private void Start()
        {
            if (jumpButton != null)
            {
                jumpButton.onClick.AddListener(SignalJumpPressed);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                GameManager.Instance.OnDeathCountChanged += UpdateDeathCount;
                GameManager.Instance.OnCheckpointReached += ShowCheckpointBanner;
                if (levelTitleText != null)
                {
                    levelTitleText.text = GameManager.Instance.LevelName;
                }
            }

            // Hide initial overlays
            if (wipedOutOverlay != null) wipedOutOverlay.SetActive(false);
            if (winScreenOverlay != null) winScreenOverlay.SetActive(false);
            if (checkpointBanner != null) checkpointBanner.SetActive(false);
            if (pauseMenuOverlay != null) pauseMenuOverlay.SetActive(false);
            if (settingsOverlay != null) settingsOverlay.SetActive(false);

            UpdateDeathCount(GameManager.Instance != null ? GameManager.Instance.DeathCount : 0);
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                UpdateTimer(GameManager.Instance.ElapsedTime);
            }

            UpdateAnimalMeters();
        }

        private void UpdateTimer(float time)
        {
            if (timerText != null)
            {
                int minutes = (int)(time / 60);
                int seconds = (int)(time % 60);
                int fraction = (int)((time * 100) % 100);
                timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
            }
        }

        private void UpdateDeathCount(int count)
        {
            if (deathCountText != null)
            {
                deathCountText.text = count.ToString();
            }
        }

        private void UpdateAnimalMeters()
        {
            // Dynamically query PlayerControllers if not manually wired
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            foreach (var p in players)
            {
                if (p.CharacterFartSystem == null) continue;
                float pct = p.CharacterFartSystem.MeterPercentage;

                switch (p.CharacterAnimalType)
                {
                    case AnimalType.Hippo:
                        if (meterHippo != null) meterHippo.fillAmount = pct;
                        break;
                    case AnimalType.Rhino:
                        if (meterRhino != null) meterRhino.fillAmount = pct;
                        break;
                    case AnimalType.Frog:
                        if (meterFrog != null) meterFrog.fillAmount = pct;
                        break;
                    case AnimalType.Capybara:
                        if (meterCapybara != null) meterCapybara.fillAmount = pct;
                        break;
                }
            }
        }

        private void HandleStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.WipedOut:
                    if (wipedOutOverlay != null) wipedOutOverlay.SetActive(true);
                    break;
                case GameState.Playing:
                    if (wipedOutOverlay != null) wipedOutOverlay.SetActive(false);
                    if (pauseMenuOverlay != null) pauseMenuOverlay.SetActive(false);
                    break;
                case GameState.Paused:
                    if (pauseMenuOverlay != null) pauseMenuOverlay.SetActive(true);
                    break;
                case GameState.Finished:
                    ShowWinScreen();
                    break;
            }
        }

        private void ShowWinScreen()
        {
            if (winScreenOverlay != null)
            {
                winScreenOverlay.SetActive(true);
                if (winTimeText != null && GameManager.Instance != null)
                {
                    float t = GameManager.Instance.ElapsedTime;
                    int m = (int)(t / 60);
                    int s = (int)(t % 60);
                    int ms = (int)((t * 100) % 100);
                    winTimeText.text = string.Format("{0:00}:{1:00}.{2:00}", m, s, ms);
                }
                if (winDeathsText != null && GameManager.Instance != null)
                {
                    winDeathsText.text = GameManager.Instance.DeathCount.ToString();
                }
            }
        }

        public void ShowCheckpointBanner()
        {
            StartCoroutine(CheckpointBannerRoutine());
        }

        private IEnumerator CheckpointBannerRoutine()
        {
            if (checkpointBanner != null)
            {
                checkpointBanner.SetActive(true);
                yield return new WaitForSeconds(1.8f);
                checkpointBanner.SetActive(false);
            }
        }

        public void SignalFartButtonReleased()
        {
            fartReleasedFlag = true;
        }

        public bool ConsumeFartButtonReleased()
        {
            if (fartReleasedFlag)
            {
                fartReleasedFlag = false;
                return true;
            }
            return false;
        }

        public void SignalJumpPressed()
        {
            jumpPressedFlag = true;
        }

        public bool ConsumeJumpPressed()
        {
            if (jumpPressedFlag)
            {
                jumpPressedFlag = false;
                return true;
            }
            return false;
        }

        // Button Click Handlers
        public void OnPauseClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
            }
        }

        public void OnResumeClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }

        public void OnRestartClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartLevel();
            }
        }

        public void OnNextLevelClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextLevel();
            }
        }

        public void OnMainMenuClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadMainMenu();
            }
        }

        public void OnSettingsClicked()
        {
            if (settingsOverlay != null)
            {
                settingsOverlay.SetActive(true);
            }
        }

        public void OnCloseSettingsClicked()
        {
            if (settingsOverlay != null)
            {
                settingsOverlay.SetActive(false);
            }
        }
    }
}
