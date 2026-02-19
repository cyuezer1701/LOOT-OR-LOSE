using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using LootOrLose.State;
using LootOrLose.Utils;

// Firebase SDK imports — uncomment when Firebase Unity SDK is installed:
// #define FIREBASE_INSTALLED
#if FIREBASE_INSTALLED
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
#endif

namespace LootOrLose.Services.Firebase
{
    /// <summary>
    /// Firebase service for authentication, Firestore operations, and cloud features.
    /// Uses preprocessor directives to compile without Firebase SDK installed.
    /// Install Firebase Unity SDK and define FIREBASE_INSTALLED to enable.
    /// </summary>
    public class FirebaseService : MonoBehaviour
    {
        public static FirebaseService Instance { get; private set; }

        public bool IsInitialized { get; private set; }
        public string UserId { get; private set; }

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
        /// Initialize Firebase and check dependencies.
        /// </summary>
        public async Task InitializeAsync()
        {
#if FIREBASE_INSTALLED
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                IsInitialized = true;
                Debug.Log("[FirebaseService] Firebase initialized successfully");
            }
            else
            {
                Debug.LogError($"[FirebaseService] Could not resolve dependencies: {dependencyStatus}");
            }
#else
            await Task.Delay(100); // Simulate async
            IsInitialized = true;
            Debug.Log("[FirebaseService] Firebase not installed — running in offline mode");
#endif
        }

        /// <summary>
        /// Sign in anonymously. Creates a persistent anonymous account.
        /// </summary>
        public async Task<string> SignInAnonymouslyAsync()
        {
#if FIREBASE_INSTALLED
            var auth = FirebaseAuth.DefaultInstance;
            var result = await auth.SignInAnonymouslyAsync();
            UserId = result.User.UserId;
            Debug.Log($"[FirebaseService] Signed in anonymously: {UserId}");
            return UserId;
#else
            await Task.Delay(50);
            UserId = "offline_user_" + Guid.NewGuid().ToString().Substring(0, 8);
            Debug.Log($"[FirebaseService] Offline mode — generated user ID: {UserId}");
            return UserId;
#endif
        }

        /// <summary>
        /// Submit a run score to the global leaderboard.
        /// </summary>
        public async Task SubmitScoreAsync(RunResult result)
        {
#if FIREBASE_INSTALLED
            var db = FirebaseFirestore.DefaultInstance;
            var data = new Dictionary<string, object>
            {
                { "playerId", UserId },
                { "score", result.finalScore },
                { "roundsCompleted", result.roundsCompleted },
                { "bossesDefeated", result.bossesDefeated },
                { "biome", result.biome.ToString() },
                { "isDailyRun", result.isDailyRun },
                { "timestamp", FieldValue.ServerTimestamp }
            };
            await db.Collection(Constants.COLLECTION_LEADERBOARDS).AddAsync(data);
            Debug.Log($"[FirebaseService] Score submitted: {result.finalScore}");
#else
            await Task.Delay(50);
            Debug.Log($"[FirebaseService] Offline — score not submitted: {result.finalScore}");
#endif
        }

        /// <summary>
        /// Load top scores from the leaderboard.
        /// </summary>
        public async Task<List<LeaderboardEntry>> LoadLeaderboardAsync(int limit = 50)
        {
            var entries = new List<LeaderboardEntry>();
#if FIREBASE_INSTALLED
            var db = FirebaseFirestore.DefaultInstance;
            var query = db.Collection(Constants.COLLECTION_LEADERBOARDS)
                .OrderByDescending("score")
                .Limit(limit);
            var snapshot = await query.GetSnapshotAsync();
            foreach (var doc in snapshot.Documents)
            {
                entries.Add(new LeaderboardEntry
                {
                    playerId = doc.GetValue<string>("playerId"),
                    score = doc.GetValue<int>("score"),
                    roundsCompleted = doc.GetValue<int>("roundsCompleted"),
                    bossesDefeated = doc.GetValue<int>("bossesDefeated")
                });
            }
#else
            await Task.Delay(50);
            Debug.Log("[FirebaseService] Offline — returning empty leaderboard");
#endif
            return entries;
        }

        /// <summary>
        /// Save player progress to Firestore cloud save.
        /// </summary>
        public async Task SaveProgressAsync(PlayerProgressState progress)
        {
#if FIREBASE_INSTALLED
            var db = FirebaseFirestore.DefaultInstance;
            var json = JsonUtility.ToJson(progress);
            var data = new Dictionary<string, object> { { "data", json }, { "updatedAt", FieldValue.ServerTimestamp } };
            await db.Collection(Constants.COLLECTION_PLAYERS).Document(UserId).SetAsync(data);
            Debug.Log("[FirebaseService] Progress saved to cloud");
#else
            await Task.Delay(50);
            Debug.Log("[FirebaseService] Offline — cloud save skipped");
#endif
        }

        /// <summary>
        /// Load player progress from Firestore cloud save.
        /// </summary>
        public async Task<PlayerProgressState> LoadProgressAsync()
        {
#if FIREBASE_INSTALLED
            var db = FirebaseFirestore.DefaultInstance;
            var doc = await db.Collection(Constants.COLLECTION_PLAYERS).Document(UserId).GetSnapshotAsync();
            if (doc.Exists)
            {
                var json = doc.GetValue<string>("data");
                return JsonUtility.FromJson<PlayerProgressState>(json);
            }
#else
            await Task.Delay(50);
            Debug.Log("[FirebaseService] Offline — no cloud save available");
#endif
            return null;
        }

        /// <summary>
        /// Get the daily run seed from Cloud Functions or generate locally.
        /// </summary>
        public async Task<int> GetDailySeedAsync()
        {
#if FIREBASE_INSTALLED
            // TODO: Call Cloud Function to get server-side daily seed
            // For now, generate from date
            await Task.Delay(50);
#else
            await Task.Delay(50);
#endif
            // Deterministic seed from current date (same for all players)
            var today = DateTime.UtcNow;
            return today.Year * 10000 + today.Month * 100 + today.Day;
        }
    }

    /// <summary>
    /// Represents a single leaderboard entry.
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerId;
        public int score;
        public int roundsCompleted;
        public int bossesDefeated;
    }
}
