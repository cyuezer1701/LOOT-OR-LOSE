using System.IO;
using UnityEngine;
using LootOrLose.State;

namespace LootOrLose.Managers
{
    /// <summary>
    /// Manages local save and load operations for <see cref="PlayerProgressState"/>.
    /// Serializes progress to JSON and writes it to the persistent data path.
    /// Singleton pattern with DontDestroyOnLoad.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        // ─────────────────────────────────────────────
        //  Configuration
        // ─────────────────────────────────────────────

        private const string SaveFileName = "player_progress.json";

        /// <summary>
        /// Full file path for the save file.
        /// Located at Application.persistentDataPath/player_progress.json.
        /// </summary>
        public string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

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
        }

        // ─────────────────────────────────────────────
        //  Save
        // ─────────────────────────────────────────────

        /// <summary>
        /// Save the player's progress state to disk as a JSON file.
        /// Overwrites any existing save file at <see cref="SaveFilePath"/>.
        /// </summary>
        /// <param name="progress">The player progress state to serialize and save.</param>
        /// <returns>True if the save was successful, false if an error occurred.</returns>
        public bool SaveProgress(PlayerProgressState progress)
        {
            if (progress == null)
            {
                Debug.LogWarning("[SaveManager] Cannot save null progress.");
                return false;
            }

            try
            {
                string json = JsonUtility.ToJson(progress, prettyPrint: true);
                File.WriteAllText(SaveFilePath, json);
                Debug.Log($"[SaveManager] Progress saved to: {SaveFilePath}");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SaveManager] Failed to save progress: {ex.Message}");
                return false;
            }
        }

        // ─────────────────────────────────────────────
        //  Load
        // ─────────────────────────────────────────────

        /// <summary>
        /// Load the player's progress state from disk.
        /// Returns null if no save file exists or if deserialization fails.
        /// </summary>
        /// <returns>The deserialized <see cref="PlayerProgressState"/>, or null on failure.</returns>
        public PlayerProgressState LoadProgress()
        {
            if (!HasSaveFile())
            {
                Debug.Log("[SaveManager] No save file found. Returning null.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                PlayerProgressState progress = JsonUtility.FromJson<PlayerProgressState>(json);
                Debug.Log($"[SaveManager] Progress loaded from: {SaveFilePath}");
                return progress;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SaveManager] Failed to load progress: {ex.Message}");
                return null;
            }
        }

        // ─────────────────────────────────────────────
        //  Utility
        // ─────────────────────────────────────────────

        /// <summary>
        /// Check whether a save file exists at the expected path.
        /// </summary>
        /// <returns>True if a save file exists on disk.</returns>
        public bool HasSaveFile()
        {
            return File.Exists(SaveFilePath);
        }

        /// <summary>
        /// Delete the save file from disk. Useful for debug, testing, and "new game" flows.
        /// </summary>
        /// <returns>True if the file was deleted, false if it did not exist or an error occurred.</returns>
        public bool DeleteSave()
        {
            if (!HasSaveFile())
            {
                Debug.Log("[SaveManager] No save file to delete.");
                return false;
            }

            try
            {
                File.Delete(SaveFilePath);
                Debug.Log($"[SaveManager] Save file deleted: {SaveFilePath}");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SaveManager] Failed to delete save file: {ex.Message}");
                return false;
            }
        }
    }
}
