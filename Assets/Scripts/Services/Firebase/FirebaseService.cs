using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LootOrLose.State;
using LootOrLose.Utils;

#if FIREBASE_INSTALLED
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
#endif

namespace LootOrLose.Services.Firebase
{
    /// <summary>
    /// Central Firebase service handling authentication, Firestore operations,
    /// and remote configuration. All methods are async and return Tasks.
    /// 
    /// This service compiles without the Firebase SDK installed by using the
    /// FIREBASE_INSTALLED preprocessor directive. When the SDK is added to the
    /// project, define FIREBASE_INSTALLED in Player Settings > Scripting Define Symbols.
    /// </summary>
    public class FirebaseService : MonoBehaviour
    {
        public static FirebaseService Instance { get; private set; }

        /// <summary>Whether Firebase has been successfully initialized.</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>Whether the user is currently signed in (anonymously or otherwise).</summary>
        public bool IsSignedIn { get; private set; }

        /// <summary>The current user's unique ID, or null if not signed in.</summary>
        public string UserId { get; private set; }

#if FIREBASE_INSTALLED
        private FirebaseApp _app;
        private FirebaseAuth _auth;
        private FirebaseFirestore _firestore;
#endif

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

        /// <summary>
        /// Initializes Firebase services. Must be called before any other Firebase operations.
        /// Checks dependencies and resolves them if necessary.
        /// </summary>
        /// <returns>True if initialization succeeded, false otherwise.</returns>
        public async Task<bool> InitializeAsync()
        {
#if FIREBASE_INSTALLED
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _app = FirebaseApp.DefaultInstance;
                    _auth = FirebaseAuth.DefaultInstance;
                    _firestore = FirebaseFirestore.DefaultInstance;
                    IsInitialized = true;
                    Debug.Log("[FirebaseService] Firebase initialized successfully.");
                    return true;
                }
                else
                {
                    Debug.LogError($"[FirebaseService] Could not resolve Firebase dependencies: {dependencyStatus}");
                    IsInitialized = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseService] Initialization failed: {ex.Message}");
                IsInitialized = false;
                return false;
            }
#else
            Debug.LogWarning("[FirebaseService] Firebase SDK not installed. Running in offline mode.");
            IsInitialized = false;
            await Task.CompletedTask;
            return false;
#endif
        }

        /// <summary>
        /// Signs in the user anonymously. Creates a new anonymous account if none exists,
        /// or restores the previous anonymous session.
        /// </summary>
        /// <returns>The anonymous user's ID, or null if sign-in failed.</returns>
        public async Task<string> SignInAnonymouslyAsync()
        {
#if FIREBASE_INSTALLED
            if (!IsInitialized)
            {
                Debug.LogWarning("[FirebaseService] Cannot sign in: Firebase not initialized.");
                return null;
            }

            try
            {
                var authResult = await _auth.SignInAnonymouslyAsync();
                UserId = authResult.User.UserId;
                IsSignedIn = true;
                Debug.Log($"[FirebaseService] Signed in anonymously as: {UserId}");
                return UserId;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseService] Anonymous sign-in failed: {ex.Message}");
                IsSignedIn = false;
                return null;
            }
#else
            Debug.LogWarning("[FirebaseService] Firebase SDK not installed. Using local-only mode.");
            UserId = SystemInfo.deviceUniqueIdentifier;
            IsSignedIn = false;
            await Task.CompletedTask;
            return UserId;
#endif
        }

        /// <summary>
        /// Submits a run result to the global leaderboard.
        /// Creates a document in the leaderboards collection with score, round, and metadata.
        /// </summary>
        /// <param name="result">The completed run result to submit.</param>
        /// <returns>True if submission succeeded, false otherwise.</returns>
        public async Task<bool> SubmitScoreAsync(RunResult result)
        {
#if FIREBASE_INSTALLED
            if (!IsInitialized || !IsSignedIn)
            {
                Debug.LogWarning("[FirebaseService] Cannot submit score: not initialized or not signed in.");
                return false;
            }

            try
            {
                var data = new Dictionary<string, object>
                {
                    { "playerId", UserId },
                    { "score", result.finalScore },
                    { "roundsCompleted", result.roundsCompleted },
                    { "bossesDefeated", result.bossesDefeated },
                    { "characterId", result.character?.id ?? "unknown" },
                    { "biome", result.biome.ToString() },
                    { "isDailyRun", result.isDailyRun },
                    { "dailySeed", result.dailySeed },
                    { "timestamp", FieldValue.ServerTimestamp }
                };

                var collectionRef = _firestore.Collection(Constants.COLLECTION_LEADERBOARDS);
                await collectionRef.AddAsync(data);

                Debug.Log($"[FirebaseService] Score submitted: {result.finalScore}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseService] Score submission failed: {ex.Message}");
                return false;
            }
#else
            Debug.LogWarning($"[FirebaseService] Score submission skipped (offline): {result.finalScore} points");
            await Task.CompletedTask;
            return false;
#endif
        }

        /// <summary>
        /// Loads the top scores from the global leaderboard.
        /// </summary>
        /// <param name="limit">Maximum number of entries to retrieve.</param>
        /// <returns>
        /// A list of dictionaries representing leaderboard entries,
        /// ordered by score descending. Returns an empty list on failure.
        /// </returns>
        public async Task<List<Dictionary<string, object>>> LoadLeaderboardAsync(int limit = 50)
        {
#if FIREBASE_INSTALLED
            if (!IsInitialized)
            {
                Debug.LogWarning("[FirebaseService] Cannot load leaderboard: not initialized.");
                return new List<Dictionary<string, object>>();
            }

            try
            {
                var query = _firestore
                    .Collection(Constants.COLLECTION_LEADERBOARDS)
                    .OrderByDescending("score")
                    .Limit(limit);

                var snapshot = await query.GetSnapshotAsync();
                var results = new List<Dictionary<string, object>>();

                foreach (var document in snapshot.Documents)
                {
                    results.Add(document.ToDictionary());
                }

                Debug.Log($"[FirebaseService] Loaded {results.Count} leaderboard entries.");
                return results;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseService] Leaderboard load failed: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
#else
            Debug.LogWarning("[FirebaseService] Leaderboard load skipped (offline).");
            await Task.CompletedTask;
            return new List<Dictionary<string, object>>();
#endif
        }

        /// <summary>
        /// Saves the player's persistent progress to Firestore.
        /// Overwrites the existing document for this player.
        /// </summary>
        /// <param name="progress">The player progress state to save.</param>
        /// <returns>True if save succeeded, false otherwise.</returns>
        public async Task<bool> SaveProgressAsync(PlayerProgressState progress)
        {
#if FIREBASE_INSTALLED
            if (!IsInitialized || !IsSignedIn)
            {
                Debug.LogWarning("[FirebaseService] Cannot save progress: not initialized or not signed in.");
                return false;
            }

            try
            {
                // TODO: Serialize PlayerProgressState to a Firestore-compatible dictionary
                // For now, use JsonUtility as an intermediary
                var json = JsonUtility.ToJson(progress);
                var data = new Dictionary<string, object>
                {
                    { "playerId", progress.playerId },
                    { "data", json },
                    { "lastUpdated", FieldValue.ServerTimestamp }
                };

                var docRef = _firestore
                    .Collection(Constants.COLLECTION_PLAYERS)
                    .Document(progress.playerId);

                await docRef.SetAsync(data);

                Debug.Log($"[FirebaseService] Progress saved for player: {progress.playerId}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseService] Progress save failed: {ex.Message}");
                return false;
            }
#else
            Debug.LogWarning($"[FirebaseService] Progress save skipped (offline) for: {progress.playerId}");
            await Task.CompletedTask;
            return false;
#endif
        }

        /// <summary>
        /// Loads a player's persistent progress from Firestore.
        /// </summary>
        /// <param name="playerId">The ID of the player whose progress to load.</param>
        /// <returns>The player's progress state, or null if not found or on failure.</returns>
        public async Task<PlayerProgressState> LoadProgressAsync(string playerId)
        {
#if FIREBASE_INSTALLED
            if (!IsInitialized)
            {
                Debug.LogWarning("[FirebaseService] Cannot load progress: not initialized.");
                return null;
            }

            try
            {
                var docRef = _firestore
                    .Collection(Constants.COLLECTION_PLAYERS)
                    .Document(playerId);

                var snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    // TODO: Deserialize from Firestore document to PlayerProgressState
                    var data = snapshot.ToDictionary();
                    if (data.ContainsKey("data"))
                    {
                        var json = data["data"].ToString();
                        var progress = JsonUtility.FromJson<PlayerProgressState>(json);
                        Debug.Log($"[FirebaseService] Progress loaded for player: {playerId}");
                        return progress;
                    }
                }

                Debug.Log($"[FirebaseService] No progress found for player: {playerId}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseService] Progress load failed: {ex.Message}");
                return null;
            }
#else
            Debug.LogWarning($"[FirebaseService] Progress load skipped (offline) for: {playerId}");
            await Task.CompletedTask;
            return null;
#endif
        }

        /// <summary>
        /// Retrieves the daily run seed from Firestore. The seed is generated once per day
        /// and shared across all players to ensure fair daily run competition.
        /// </summary>
        /// <returns>
        /// The daily seed integer. Falls back to a date-based seed if Firestore is unavailable.
        /// </returns>
        public async Task<int> GetDailySeedAsync()
        {
#if FIREBASE_INSTALLED
            if (!IsInitialized)
            {
                Debug.LogWarning("[FirebaseService] Cannot get daily seed: not initialized. Using fallback.");
                return GetFallbackDailySeed();
            }

            try
            {
                var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
                var docRef = _firestore
                    .Collection(Constants.COLLECTION_DAILY_RUNS)
                    .Document(today);

                var snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists && snapshot.ContainsField("seed"))
                {
                    var seed = snapshot.GetValue<int>("seed");
                    Debug.Log($"[FirebaseService] Daily seed retrieved: {seed}");
                    return seed;
                }

                // No seed exists for today yet; generate and store one
                var newSeed = today.GetHashCode();
                var data = new Dictionary<string, object>
                {
                    { "seed", newSeed },
                    { "date", today },
                    { "createdAt", FieldValue.ServerTimestamp }
                };

                await docRef.SetAsync(data);
                Debug.Log($"[FirebaseService] New daily seed created: {newSeed}");
                return newSeed;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseService] Daily seed fetch failed: {ex.Message}");
                return GetFallbackDailySeed();
            }
#else
            Debug.LogWarning("[FirebaseService] Daily seed fetch skipped (offline). Using fallback.");
            await Task.CompletedTask;
            return GetFallbackDailySeed();
#endif
        }

        /// <summary>
        /// Generates a deterministic daily seed based on the current UTC date.
        /// Used as a fallback when Firestore is unavailable.
        /// </summary>
        private int GetFallbackDailySeed()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd").GetHashCode();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
