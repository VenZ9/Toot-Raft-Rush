using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TootRaftRush.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject howToPlayPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Level Select Panel")]
        [SerializeField] private GameObject levelSelectPanel;

        private void Start()
        {
            ShowMainPanel();
        }

        public void ShowMainPanel()
        {
            if (mainPanel != null) mainPanel.SetActive(true);
            if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        }

        public void OnPlayClicked()
        {
            // Open Level Select or go to Character Selection / Sunny Slide
            if (levelSelectPanel != null)
            {
                levelSelectPanel.SetActive(true);
                if (mainPanel != null) mainPanel.SetActive(false);
            }
            else
            {
                SceneManager.LoadScene("SunnySlide");
            }
        }

        public void OnSelectLevel(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void OnCharacterSelectClicked()
        {
            SceneManager.LoadScene("CharacterSelection");
        }

        public void OnLobbyClicked()
        {
            SceneManager.LoadScene("Lobby");
        }

        public void OnHowToPlayClicked()
        {
            if (howToPlayPanel != null) howToPlayPanel.SetActive(true);
            if (mainPanel != null) mainPanel.SetActive(false);
        }

        public void OnSettingsClicked()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
            if (mainPanel != null) mainPanel.SetActive(false);
        }

        public void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
