using System;
using UnityEngine;

namespace TootRaftRush.Audio
{
    /// <summary>
    /// Procedural runtime audio synthesizer.
    /// Synthesizes dynamic WAV-free AudioClips on the fly using mathematical waveforms
    /// so the game never goes silent even if external audio files fail to load.
    /// </summary>
    public static class ProceduralAudioSynthesizer
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateTootClip(Gameplay.AnimalType animal, float chargeRatio)
        {
            float duration = Mathf.Lerp(0.5f, 0.9f, chargeRatio);
            int samples = (int)(SampleRate * duration);
            float[] data = new float[samples];

            float baseFreq = 90f;
            switch (animal)
            {
                case Gameplay.AnimalType.Hippo:
                    baseFreq = 85f;
                    break;
                case Gameplay.AnimalType.Rhino:
                    baseFreq = 55f;
                    break;
                case Gameplay.AnimalType.Frog:
                    baseFreq = 180f;
                    break;
                case Gameplay.AnimalType.Capybara:
                    baseFreq = 380f;
                    break;
            }

            System.Random rand = new System.Random();

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float progress = t / duration;
                float env = Mathf.Sin(Mathf.PI * progress);

                // Pitch drop envelope
                float freq = baseFreq * (1.0f - progress * 0.45f);
                float fm = Mathf.Sin(2f * Mathf.PI * 30f * t) * (baseFreq * 0.3f);
                float sine = Mathf.Sin(2f * Mathf.PI * (freq + fm) * t);

                // Flutter & noise pops
                float noise = (float)(rand.NextDouble() * 2.0 - 1.0) * 0.3f;
                data[i] = env * (sine * 0.75f + noise * 0.25f);
            }

            AudioClip clip = AudioClip.Create($"Procedural_Toot_{animal}", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public static AudioClip CreateChimeClip()
        {
            float duration = 0.6f;
            int samples = (int)(SampleRate * duration);
            float[] data = new float[samples];

            float[] notes = { 1046.5f, 1318.5f, 1567.98f }; // C6, E6, G6
            float noteDur = duration / notes.Length;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                int noteIdx = Mathf.Min(notes.Length - 1, (int)(t / noteDur));
                float tNote = t - noteIdx * noteDur;
                float env = Mathf.Exp(-7f * tNote);
                float freq = notes[noteIdx];
                data[i] = env * Mathf.Sin(2f * Mathf.PI * freq * t) * 0.7f;
            }

            AudioClip clip = AudioClip.Create("Procedural_Chime", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public static AudioClip CreateSplashClip()
        {
            float duration = 0.5f;
            int samples = (int)(SampleRate * duration);
            float[] data = new float[samples];
            System.Random rand = new System.Random();

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float env = Mathf.Exp(-6f * t);
                float noise = (float)(rand.NextDouble() * 2.0 - 1.0);
                float wave = Mathf.Sin(2f * Mathf.PI * (160f - 80f * t) * t);
                data[i] = env * (noise * 0.65f + wave * 0.35f) * 0.8f;
            }

            AudioClip clip = AudioClip.Create("Procedural_Splash", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
