using UnityEngine;
using TootRaftRush.Gameplay;
using TootRaftRush.Pickups;

namespace TootRaftRush.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource tootSource;

        [Header("Music Clips")]
        [SerializeField] private AudioClip backgroundMusic;

        [Header("Fart Audio Clips")]
        [SerializeField] private AudioClip tootHippo;
        [SerializeField] private AudioClip tootRhino;
        [SerializeField] private AudioClip tootFrog;
        [SerializeField] private AudioClip tootCapybara;
        [SerializeField] private AudioClip fartCharge;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip splashSound;
        [SerializeField] private AudioClip pickupBean;
        [SerializeField] private AudioClip pickupChili;
        [SerializeField] private AudioClip pickupSoda;
        [SerializeField] private AudioClip wipeoutSound;
        [SerializeField] private AudioClip checkpointSound;
        [SerializeField] private AudioClip victorySound;

        [Header("Volume Settings")]
        [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.75f;
        [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.9f;
        [SerializeField] private bool isMuted = false;
        [SerializeField] private bool vibrationEnabled = true;

        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;
        public bool IsMuted => isMuted;
        public bool VibrationEnabled => vibrationEnabled;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SetupAudioSources();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            PlayMusic();
        }

        private void SetupAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            if (tootSource == null)
            {
                tootSource = gameObject.AddComponent<AudioSource>();
                tootSource.loop = false;
                tootSource.playOnAwake = false;
            }

            UpdateVolumes();
        }

        public void PlayMusic()
        {
            if (musicSource == null || isMuted) return;

            if (backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }

        public void PlayCharacterFart(AnimalType animal, float chargeRatio)
        {
            if (isMuted || tootSource == null) return;

            AudioClip clip = null;
            switch (animal)
            {
                case AnimalType.Hippo:
                    clip = tootHippo;
                    break;
                case AnimalType.Rhino:
                    clip = tootRhino;
                    break;
                case AnimalType.Frog:
                    clip = tootFrog;
                    break;
                case AnimalType.Capybara:
                    clip = tootCapybara;
                    break;
            }

            // Procedural fallback if clip is missing
            if (clip == null)
            {
                clip = ProceduralAudioSynthesizer.CreateTootClip(animal, chargeRatio);
            }

            // Subtle comical pitch variation +/- 10%
            float randomPitch = Random.Range(0.92f, 1.08f) + chargeRatio * 0.15f;
            tootSource.pitch = randomPitch;
            float vol = Mathf.Lerp(0.7f, 1.0f, chargeRatio) * sfxVolume * masterVolume;
            tootSource.PlayOneShot(clip, vol);

            // Optional haptic vibration on Android
            if (vibrationEnabled)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Handheld.Vibrate();
#endif
            }
        }

        public void PlayFartCharge()
        {
            if (isMuted || sfxSource == null) return;
            if (fartCharge != null)
            {
                sfxSource.PlayOneShot(fartCharge, 0.6f * sfxVolume * masterVolume);
            }
        }

        public void PlaySplash()
        {
            if (isMuted || sfxSource == null) return;
            AudioClip clip = splashSound != null ? splashSound : ProceduralAudioSynthesizer.CreateSplashClip();
            sfxSource.PlayOneShot(clip, 0.7f * sfxVolume * masterVolume);
        }

        public void PlayPickup(PickupType type)
        {
            if (isMuted || sfxSource == null) return;
            AudioClip clip = null;
            switch (type)
            {
                case PickupType.Beans: clip = pickupBean; break;
                case PickupType.Chili: clip = pickupChili; break;
                case PickupType.Soda: clip = pickupSoda; break;
            }
            if (clip == null) clip = ProceduralAudioSynthesizer.CreateChimeClip();
            sfxSource.PlayOneShot(clip, 0.8f * sfxVolume * masterVolume);
        }

        public void PlayWipeout()
        {
            if (isMuted || sfxSource == null) return;
            if (wipeoutSound != null)
            {
                sfxSource.PlayOneShot(wipeoutSound, 0.9f * sfxVolume * masterVolume);
            }
        }

        public void PlayCheckpoint()
        {
            if (isMuted || sfxSource == null) return;
            AudioClip clip = checkpointSound != null ? checkpointSound : ProceduralAudioSynthesizer.CreateChimeClip();
            sfxSource.PlayOneShot(clip, 0.85f * sfxVolume * masterVolume);
        }

        public void PlayVictory()
        {
            if (isMuted || sfxSource == null) return;
            if (victorySound != null)
            {
                sfxSource.PlayOneShot(victorySound, 1.0f * sfxVolume * masterVolume);
            }
        }

        public void SetMasterVolume(float vol)
        {
            masterVolume = Mathf.Clamp01(vol);
            UpdateVolumes();
        }

        public void SetMusicVolume(float vol)
        {
            musicVolume = Mathf.Clamp01(vol);
            UpdateVolumes();
        }

        public void SetSfxVolume(float vol)
        {
            sfxVolume = Mathf.Clamp01(vol);
            UpdateVolumes();
        }

        public void SetMute(bool mute)
        {
            isMuted = mute;
            UpdateVolumes();
        }

        public void SetVibration(bool enabled)
        {
            vibrationEnabled = enabled;
        }

        private void UpdateVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = isMuted ? 0f : (musicVolume * masterVolume);
            }
            if (sfxSource != null)
            {
                sfxSource.volume = isMuted ? 0f : (sfxVolume * masterVolume);
            }
            if (tootSource != null)
            {
                tootSource.volume = isMuted ? 0f : (sfxVolume * masterVolume);
            }
        }
    }
}
