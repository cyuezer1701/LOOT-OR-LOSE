using System;
using UnityEngine;
using LootOrLose.Data;
using LootOrLose.Enums;
using LootOrLose.State;

#if FIREBASE_INSTALLED
using Firebase.Analytics;
#endif

namespace LootOrLose.Services.Analytics
{
    /// <summary>
    /// Analytics service for tracking player behavior and game events.
    /// All methods are no-ops with Debug.Log output until Firebase Analytics
    /// is integrated. Define FIREBASE_INSTALLED in Scripting Define Symbols
    /// to enable actual analytics logging.
    /// </summary>
    public class AnalyticsService : MonoBehaviour
    {
        public static AnalyticsService Instance { get; private set; }

        /// <summary>Whether analytics collection is enabled (respects user privacy settings).</summary>
        public bool IsEnabled { get; set; } = true;

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
        /// Logs the start of a new run, including the selected character and biome.
        /// </summary>
        /// <param name="character">The character selected for this run.</param>
        /// <param name="biome">The biome selected for this run.</param>
        public void LogRunStart(CharacterData character, BiomeData biome)
        {
            if (!IsEnabled) return;

#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("run_start",
                new Parameter("character_id", character?.id ?? "unknown"),
                new Parameter("character_type", character?.type.ToString() ?? "unknown"),
                new Parameter("biome_id", biome?.id ?? "unknown"),
                new Parameter("biome_type", biome?.type.ToString() ?? "unknown")
            );
#endif
            Debug.Log($"[Analytics] Run started - Character: {character?.id ?? "null"}, Biome: {biome?.id ?? "null"}");
        }

        /// <summary>
        /// Logs the end of a run with the complete run result data.
        /// </summary>
        /// <param name="result">The completed run result containing score, rounds, etc.</param>
        public void LogRunEnd(RunResult result)
        {
            if (!IsEnabled) return;

#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("run_end",
                new Parameter("final_score", result.finalScore),
                new Parameter("rounds_completed", result.roundsCompleted),
                new Parameter("bosses_defeated", result.bossesDefeated),
                new Parameter("items_looted", result.itemsLooted),
                new Parameter("items_left", result.itemsLeft),
                new Parameter("death_cause", result.deathCause ?? "victory"),
                new Parameter("run_duration", (double)result.runDuration),
                new Parameter("is_daily_run", result.isDailyRun ? 1 : 0),
                new Parameter("character_id", result.character?.id ?? "unknown"),
                new Parameter("biome", result.biome.ToString())
            );
#endif
            Debug.Log($"[Analytics] Run ended - Score: {result.finalScore}, Rounds: {result.roundsCompleted}, " +
                       $"Death: {result.deathCause ?? "victory"}, Duration: {result.runDuration:F1}s");
        }

        /// <summary>
        /// Logs a player's loot/leave decision for an individual item.
        /// Tracks which items players choose to take or leave behind.
        /// </summary>
        /// <param name="item">The item that was presented to the player.</param>
        /// <param name="decision">Whether the player looted, left, or timed out.</param>
        public void LogItemDecision(ItemData item, DecisionResult decision)
        {
            if (!IsEnabled) return;

#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("item_decision",
                new Parameter("item_id", item?.id ?? "unknown"),
                new Parameter("item_category", item?.category.ToString() ?? "unknown"),
                new Parameter("item_rarity", item?.rarity.ToString() ?? "unknown"),
                new Parameter("is_cursed", item?.isCursed == true ? 1 : 0),
                new Parameter("decision", decision.ToString())
            );
#endif
            Debug.Log($"[Analytics] Item decision - {item?.id ?? "null"} ({item?.rarity}): {decision}");
        }

        /// <summary>
        /// Logs a boss encounter and its outcome.
        /// </summary>
        /// <param name="boss">The boss that was encountered.</param>
        /// <param name="won">Whether the player won the boss fight.</param>
        public void LogBossEncounter(BossData boss, bool won)
        {
            if (!IsEnabled) return;

#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("boss_encounter",
                new Parameter("boss_id", boss?.id ?? "unknown"),
                new Parameter("boss_type", boss?.type.ToString() ?? "unknown"),
                new Parameter("result", won ? "victory" : "defeat")
            );
#endif
            Debug.Log($"[Analytics] Boss encounter - {boss?.id ?? "null"}: {(won ? "VICTORY" : "DEFEAT")}");
        }

        /// <summary>
        /// Logs a random event encounter and its outcome.
        /// </summary>
        /// <param name="eventData">The event that occurred.</param>
        /// <param name="outcome">The outcome of the event interaction.</param>
        public void LogEvent(EventData eventData, EventOutcome outcome)
        {
            if (!IsEnabled) return;

#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("game_event",
                new Parameter("event_id", eventData?.id ?? "unknown"),
                new Parameter("event_type", eventData?.type.ToString() ?? "unknown"),
                new Parameter("outcome", outcome.ToString())
            );
#endif
            Debug.Log($"[Analytics] Event - {eventData?.id ?? "null"} ({eventData?.type}): {outcome}");
        }

        /// <summary>
        /// Logs an in-app purchase or premium currency transaction.
        /// </summary>
        /// <param name="itemId">The store item ID that was purchased.</param>
        /// <param name="price">The price paid in real currency or premium currency.</param>
        public void LogPurchase(string itemId, float price)
        {
            if (!IsEnabled) return;

#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent("purchase",
                new Parameter("item_id", itemId ?? "unknown"),
                new Parameter("price", (double)price),
                new Parameter(FirebaseAnalytics.ParameterCurrency, "USD")
            );
#endif
            Debug.Log($"[Analytics] Purchase - Item: {itemId ?? "null"}, Price: {price:F2}");
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }

    /// <summary>
    /// Possible outcomes when a player interacts with a random event.
    /// </summary>
    public enum EventOutcome
    {
        /// <summary>The event had a positive result for the player.</summary>
        Success,

        /// <summary>The event had a negative result for the player.</summary>
        Failure,

        /// <summary>The player chose not to interact with the event.</summary>
        Skipped,

        /// <summary>The event had no significant effect.</summary>
        Neutral
    }
}
