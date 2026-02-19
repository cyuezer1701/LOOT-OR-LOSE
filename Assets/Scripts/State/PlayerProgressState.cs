using System;
using System.Collections.Generic;
using LootOrLose.Enums;

namespace LootOrLose.State
{
    /// <summary>
    /// Persistent player progression data that is saved to disk or cloud storage.
    /// Tracks lifetime statistics, unlocks, streaks, and meta-game currency.
    /// This state survives between runs and application restarts.
    /// </summary>
    [Serializable]
    public class PlayerProgressState
    {
        // --- Identity ---

        /// <summary>Unique player identifier (device-generated or cloud account ID).</summary>
        public string playerId;

        // --- Lifetime Statistics ---

        /// <summary>Total number of runs the player has started.</summary>
        public int totalRuns;

        /// <summary>The highest score achieved across all runs.</summary>
        public int bestScore;

        /// <summary>The highest round number reached across all runs.</summary>
        public int bestRound;

        /// <summary>Total number of items looted across all runs.</summary>
        public int totalItemsLooted;

        /// <summary>Total number of bosses defeated across all runs.</summary>
        public int totalBossesDefeated;

        // --- Unlocks ---

        /// <summary>IDs of characters the player has unlocked.</summary>
        public List<string> unlockedCharacterIds;

        /// <summary>IDs of biomes the player has unlocked.</summary>
        public List<string> unlockedBiomeIds;

        /// <summary>IDs of achievements the player has earned.</summary>
        public List<string> unlockedAchievementIds;

        // --- Streaks ---

        /// <summary>
        /// Number of consecutive days the player has completed at least one run.
        /// Resets if a day is missed.
        /// </summary>
        public int currentStreak;

        /// <summary>The longest daily play streak ever achieved.</summary>
        public int bestStreak;

        /// <summary>The date of the player's most recent completed run, used for streak calculation.</summary>
        public DateTime lastPlayDate;

        // --- Economy ---

        /// <summary>Premium (paid) currency balance used for cosmetic purchases.</summary>
        public int premiumCurrency;

        // --- Item Analytics ---

        /// <summary>
        /// Tracks how many times each item has been looted across all runs.
        /// Key = item ID, Value = total loot count.
        /// Used for unlocking achievements and displaying collection stats.
        /// </summary>
        public Dictionary<string, int> itemLootCount;

        // --- Selection ---

        /// <summary>ID of the character currently selected for the next run.</summary>
        public string selectedCharacterId;

        /// <summary>ID of the biome currently selected for the next run.</summary>
        public string selectedBiomeId;
    }
}
