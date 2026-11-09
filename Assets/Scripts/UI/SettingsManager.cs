using UnityEngine;
using UnityEngine.UI;
using TootRaftRush.Audio;

namespace TootRaftRush.UI
{
    public class SettingsManager : MonoBehaviour
    {
        [Header("Audio UI Elements")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle muteToggle;
        [SerializeField] private Toggle vibrationToggle;

        [Header("Graphics UI")]
        [SerializeField] private Dropdown qualityDropdown;

        private void Start()
        {
            LoadSettings();

            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

            if (muteToggle != null)
                muteToggle.onValueChanged.AddListener(OnMuteToggled);

            if (vibrationToggle != null)
                vibrationToggle.onValueChanged.AddListener(OnVibrationToggled);

            if (qualityDropdown != null)
                qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        private void LoadSettings()
        {
            float master = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            float sfx = PlayerPrefs.GetFloat("SfxVolume", 0.9f);
            bool mute = PlayerPrefs.GetInt("IsMuted", 0) == 1;
            bool vib = PlayerPrefs.GetInt("Vibration", 1) == 1;
            int quality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());

            if (masterVolumeSlider != null) masterVolumeSlider.value = master;
            if (musicVolumeSlider != null) musicVolumeSlider.value = music;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfx;
            if (muteToggle != null) muteToggle.isOn = mute;
            if (vibrationToggle != null) vibrationToggle.isOn = vib;
            if (qualityDropdown != null) qualityDropdown.value = quality;

            ApplyAudioSettings(master, music, sfx, mute, vib);
        }

        private void OnMasterVolumeChanged(float val)
        {
            PlayerPrefs.SetFloat("MasterVolume", val);
            if (AudioManager.Instance != null) AudioManager.Instance.SetMasterVolume(val);
        }

        private void OnMusicVolumeChanged(float val)
        {
            PlayerPrefs.SetFloat("MusicVolume", val);
            if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(val);
        }

        private void OnSfxVolumeChanged(float val)
        {
            PlayerPrefs.SetFloat("SfxVolume", val);
            if (AudioManager.Instance != null) AudioManager.Instance.SetSfxVolume(val);
        }

        private void OnMuteToggled(bool val)
        {
            PlayerPrefs.SetInt("IsMuted", val ? 1 : 0);
            if (AudioManager.Instance != null) AudioManager.Instance.SetMute(val);
        }

        private void OnVibrationToggled(bool val)
        {
            PlayerPrefs.SetInt("Vibration", val ? 1 : 0);
            if (AudioManager.Instance != null) AudioManager.Instance.SetVibration(val);
        }

        private void OnQualityChanged(int val)
        {
            QualitySettings.SetQualityLevel(val, true);
            PlayerPrefs.SetInt("QualityLevel", val);
        }

        private void ApplyAudioSettings(float master, float music, float sfx, bool mute, bool vib)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(master);
                AudioManager.Instance.SetMusicVolume(music);
                AudioManager.Instance.SetSfxVolume(sfx);
                AudioManager.Instance.SetMute(mute);
                AudioManager.Instance.SetVibration(vib);
            }
        }
    }
}
