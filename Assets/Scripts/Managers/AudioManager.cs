using System.Collections.Generic;
using UnityEngine;

namespace LootOrLose.Managers
{
    /// <summary>
    /// Manages all game audio including background music and sound effects.
    /// Provides a centralized API for playing, stopping, and adjusting audio.
    /// Singleton pattern with DontDestroyOnLoad.
    ///
    /// Current implementation is a placeholder skeleton that logs audio events.
    /// Actual AudioClip loading and playback will be added when audio assets are available.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        // ─────────────────────────────────────────────
        //  Audio Sources
        // ─────────────────────────────────────────────

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        // ─────────────────────────────────────────────
        //  Volume Settings
        // ─────────────────────────────────────────────

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1.0f;

        // ─────────────────────────────────────────────
        //  Audio Clip Cache
        // ─────────────────────────────────────────────

        private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

        // ─────────────────────────────────────────────
        //  Known SFX Names
        //  Reference list of all SFX identifiers used throughout the game:
        //    "loot"           - Player loots an item
        //    "leave"          - Player leaves an item
        //    "timer_tick"     - Decision timer tick each second
        //    "timer_warning"  - Decision timer warning (low time)
        //    "boss_roar"      - Boss encounter begins
        //    "death"          - Player dies
        //    "level_up"       - Player levels up / zone change
        //    "synergy"        - Synergy bonus activated
        //    "trap"           - Trap or curse triggered
        //    "heal"           - Player healed
        //    "rare_item"      - Rare item revealed
        //    "legendary_item" - Legendary item revealed
        //    "event"          - Random event triggered
        // ─────────────────────────────────────────────

        // ─────────────────────────────────────────────
        //  Lifecycle
        // ─────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
        }

        /// <summary>
        /// Initialize audio sources. Creates them if not assigned in the Inspector.
        /// </summary>
        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = musicVolume;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = sfxVolume;
            }
        }

        // ─────────────────────────────────────────────
        //  Music
        // ─────────────────────────────────────────────

        /// <summary>
        /// Play a music track by name. Loads the clip from Resources/Audio/Music/{clipName}.
        /// If the same track is already playing, does nothing.
        /// </summary>
        /// <param name="clipName">The name of the music clip to play (without extension).</param>
        public void PlayMusic(string clipName)
        {
            if (string.IsNullOrEmpty(clipName)) return;

            AudioClip clip = LoadClip($"Audio/Music/{clipName}");
            if (clip == null)
            {
                Debug.Log($"[AudioManager] Music clip not found: {clipName} (placeholder)");
                return;
            }

            // Don't restart the same track
            if (musicSource.clip == clip && musicSource.isPlaying) return;

            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.Play();
            Debug.Log($"[AudioManager] Playing music: {clipName}");
        }

        /// <summary>
        /// Stop the currently playing music track.
        /// </summary>
        public void StopMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
                Debug.Log("[AudioManager] Music stopped.");
            }
        }

        /// <summary>
        /// Set the music volume.
        /// </summary>
        /// <param name="volume">Volume level between 0.0 (mute) and 1.0 (full).</param>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
            Debug.Log($"[AudioManager] Music volume set to: {musicVolume:F2}");
        }

        // ─────────────────────────────────────────────
        //  Sound Effects
        // ─────────────────────────────────────────────

        /// <summary>
        /// Play a sound effect by name. Loads the clip from Resources/Audio/SFX/{clipName}.
        /// Uses PlayOneShot to allow overlapping SFX.
        /// </summary>
        /// <param name="clipName">The name of the SFX clip to play (without extension).</param>
        public void PlaySFX(string clipName)
        {
            if (string.IsNullOrEmpty(clipName)) return;

            AudioClip clip = LoadClip($"Audio/SFX/{clipName}");
            if (clip == null)
            {
                Debug.Log($"[AudioManager] SFX: {clipName} (placeholder - clip not loaded)");
                return;
            }

            sfxSource.PlayOneShot(clip, sfxVolume);
            Debug.Log($"[AudioManager] Playing SFX: {clipName}");
        }

        /// <summary>
        /// Set the SFX volume.
        /// </summary>
        /// <param name="volume">Volume level between 0.0 (mute) and 1.0 (full).</param>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
            Debug.Log($"[AudioManager] SFX volume set to: {sfxVolume:F2}");
        }

        // ─────────────────────────────────────────────
        //  Clip Loading
        // ─────────────────────────────────────────────

        /// <summary>
        /// Load an AudioClip from the Resources folder with caching.
        /// Returns null if the clip does not exist (placeholder behavior).
        /// </summary>
        /// <param name="resourcePath">The resource path relative to Resources/.</param>
        /// <returns>The loaded AudioClip, or null if not found.</returns>
        private AudioClip LoadClip(string resourcePath)
        {
            if (clipCache.TryGetValue(resourcePath, out AudioClip cached))
            {
                return cached;
            }

            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            if (clip != null)
            {
                clipCache[resourcePath] = clip;
            }

            return clip;
        }

        // ─────────────────────────────────────────────
        //  Utility
        // ─────────────────────────────────────────────

        /// <summary>
        /// Clear the audio clip cache. Useful when switching scenes or freeing memory.
        /// </summary>
        public void ClearCache()
        {
            clipCache.Clear();
            Debug.Log("[AudioManager] Audio clip cache cleared.");
        }

        /// <summary>
        /// Get the current music volume level.
        /// </summary>
        /// <returns>The music volume (0.0 to 1.0).</returns>
        public float GetMusicVolume()
        {
            return musicVolume;
        }

        /// <summary>
        /// Get the current SFX volume level.
        /// </summary>
        /// <returns>The SFX volume (0.0 to 1.0).</returns>
        public float GetSFXVolume()
        {
            return sfxVolume;
        }
    }
}
