using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TootRaftRush.Gameplay;

namespace TootRaftRush.UI
{
    public class CharacterSelectController : MonoBehaviour
    {
        [System.Serializable]
        public struct CharacterInfo
        {
            public AnimalType animalType;
            public string characterName;
            public string description;
            public GameObject previewModel;
        }

        [Header("Characters Data")]
        [SerializeField] private CharacterInfo[] characters = new CharacterInfo[]
        {
            new CharacterInfo { animalType = AnimalType.Hippo, characterName = "Blue Hippo", description = "Heavyweight blaster! High fart inertia and wide thrust cone." },
            new CharacterInfo { animalType = AnimalType.Rhino, characterName = "Grey Rhino", description = "Raw horsepower! Deep bass rumble delivers maximum forward impulse." },
            new CharacterInfo { animalType = AnimalType.Frog, characterName = "Green Frog", description = "Agile amphibian! Rapid meter recharge and nimble steering." },
            new CharacterInfo { animalType = AnimalType.Capybara, characterName = "Orange Capybara", description = "Zen party captain! High stability and explosive charged blasts." }
        };

        [Header("UI References")]
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button selectButton;

        private int currentIndex = 0;
        public static AnimalType SelectedAnimal = AnimalType.Hippo;

        private void Start()
        {
            UpdateDisplay();
        }

        public void OnNextCharacter()
        {
            currentIndex = (currentIndex + 1) % characters.Length;
            UpdateDisplay();
        }

        public void OnPrevCharacter()
        {
            currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (currentIndex < 0 || currentIndex >= characters.Length) return;

            CharacterInfo info = characters[currentIndex];
            if (characterNameText != null) characterNameText.text = info.characterName;
            if (descriptionText != null) descriptionText.text = info.description;

            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].previewModel != null)
                {
                    characters[i].previewModel.SetActive(i == currentIndex);
                }
            }
        }

        public void OnSelectAndPlay()
        {
            SelectedAnimal = characters[currentIndex].animalType;
            PlayerPrefs.SetInt("SelectedAnimal", (int)SelectedAnimal);
            PlayerPrefs.Save();
            SceneManager.LoadScene("SunnySlide");
        }

        public void OnBackClicked()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
