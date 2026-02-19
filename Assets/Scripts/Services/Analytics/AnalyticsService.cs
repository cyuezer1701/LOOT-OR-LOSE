using UnityEngine;
using LootOrLose.Data;
using LootOrLose.Enums;
using LootOrLose.State;
using LootOrLose.Core.Events;

// Firebase Analytics — uncomment when Firebase Unity SDK is installed:
// #define FIREBASE_INSTALLED
#if FIREBASE_INSTALLED
using Firebase.Analytics;
#endif

namespace LootOrLose.Services.Analytics
{
    /// <summary>
    /// Analytics service for tracking game events.
    /// Uses Firebase Analytics when available, falls back to Debug.Log.
    /// Install Firebase Unity SDK and define FIREBASE_INSTALLED to enable.
    /// </summary>
    public class AnalyticsService : MonoBehaviour
    {
        public static AnalyticsService Instance { get; private set; }

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
        /// Log the start of a new run.
        /// </summary>
        public void LogRunStart(CharacterData character, BiomeData biome)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("run_start", new Parameter[] {
                new Parameter("character", character.id),
                new Parameter("biome", biome.id)
            });
#endif
            Debug.Log($"[Analytics] Run started: {character.id} in {biome.id}");
        }

        /// <summary>
        /// Log the end of a run with full results.
        /// </summary>
        public void LogRunEnd(RunResult result)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("run_end", new Parameter[] {
                new Parameter("score", result.finalScore),
                new Parameter("rounds", result.roundsCompleted),
                new Parameter("bosses_defeated", result.bossesDefeated),
                new Parameter("items_looted", result.itemsLooted),
                new Parameter("death_cause", result.deathCause),
                new Parameter("duration_seconds", (int)result.runDuration),
                new Parameter("biome", result.biome.ToString()),
                new Parameter("is_daily_run", result.isDailyRun ? 1 : 0)
            });
#endif
            Debug.Log($"[Analytics] Run ended: score={result.finalScore}, rounds={result.roundsCompleted}, cause={result.deathCause}");
        }

        /// <summary>
        /// Log an item decision (loot, leave, or timeout).
        /// </summary>
        public void LogItemDecision(ItemData item, DecisionResult decision)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("item_decision", new Parameter[] {
                new Parameter("item_id", item.id),
                new Parameter("category", item.category.ToString()),
                new Parameter("rarity", item.rarity.ToString()),
                new Parameter("decision", decision.ToString())
            });
#endif
            Debug.Log($"[Analytics] Item decision: {item.id} ({item.rarity}) -> {decision}");
        }

        /// <summary>
        /// Log a boss encounter and its outcome.
        /// </summary>
        public void LogBossEncounter(BossData boss, bool playerWon)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("boss_encounter", new Parameter[] {
                new Parameter("boss_id", boss.id),
                new Parameter("boss_type", boss.type.ToString()),
                new Parameter("player_won", playerWon ? 1 : 0)
            });
#endif
            Debug.Log($"[Analytics] Boss encounter: {boss.id} -> {(playerWon ? "WIN" : "LOSE")}");
        }

        /// <summary>
        /// Log an event trigger and outcome.
        /// </summary>
        public void LogEvent(EventData eventData, EventOutcome outcome)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("game_event", new Parameter[] {
                new Parameter("event_type", eventData.type.ToString()),
                new Parameter("success", outcome.success ? 1 : 0),
                new Parameter("hp_change", outcome.hpChange),
                new Parameter("gold_change", outcome.goldChange)
            });
#endif
            Debug.Log($"[Analytics] Event: {eventData.type} -> success={outcome.success}");
        }

        /// <summary>
        /// Log an in-app purchase.
        /// </summary>
        public void LogPurchase(string itemId, float priceUSD)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("purchase", new Parameter[] {
                new Parameter("item_id", itemId),
                new Parameter("price", priceUSD),
                new Parameter("currency", "USD")
            });
#endif
            Debug.Log($"[Analytics] Purchase: {itemId} for ${priceUSD}");
        }

        /// <summary>
        /// Log an achievement unlock.
        /// </summary>
        public void LogAchievementUnlocked(string achievementId)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("achievement_unlocked", new Parameter[] {
                new Parameter("achievement_id", achievementId)
            });
#endif
            Debug.Log($"[Analytics] Achievement unlocked: {achievementId}");
        }

        /// <summary>
        /// Log a character unlock.
        /// </summary>
        public void LogCharacterUnlocked(string characterId)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("character_unlocked", new Parameter[] {
                new Parameter("character_id", characterId)
            });
#endif
            Debug.Log($"[Analytics] Character unlocked: {characterId}");
        }

        /// <summary>
        /// Log a biome unlock.
        /// </summary>
        public void LogBiomeUnlocked(string biomeId)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("biome_unlocked", new Parameter[] {
                new Parameter("biome_id", biomeId)
            });
#endif
            Debug.Log($"[Analytics] Biome unlocked: {biomeId}");
        }
    }
}
