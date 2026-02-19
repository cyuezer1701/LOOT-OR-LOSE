using System;
using System.Collections.Generic;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.State
{
    /// <summary>
    /// Immutable result data captured when a run ends (victory or death).
    /// Used for the results screen, statistics tracking, leaderboard submission,
    /// and daily run comparison.
    /// </summary>
    [Serializable]
    public class RunResult
    {
        // --- Score & Progress ---

        /// <summary>The final score achieved during the run.</summary>
        public int finalScore;

        /// <summary>Total number of rounds completed before the run ended.</summary>
        public int roundsCompleted;

        // --- Combat & Loot Stats ---

        /// <summary>Number of bosses defeated during the run.</summary>
        public int bossesDefeated;

        /// <summary>Number of items the player chose to loot.</summary>
        public int itemsLooted;

        /// <summary>Number of items the player chose to leave behind.</summary>
        public int itemsLeft;

        // --- Final State Snapshot ---

        /// <summary>
        /// The items in the player's inventory when the run ended.
        /// Useful for displaying the final loadout on the results screen.
        /// </summary>
        public List<ItemData> finalInventory;

        /// <summary>The character that was used for this run.</summary>
        public CharacterData character;

        /// <summary>The biome in which this run took place.</summary>
        public BiomeType biome;

        // --- Death Info ---

        /// <summary>
        /// Localization key describing the cause of death (e.g., "death_cause_boss", "death_cause_trap").
        /// Null or empty if the player completed the run successfully.
        /// </summary>
        public string deathCause;

        // --- Timing ---

        /// <summary>Total duration of the run in seconds.</summary>
        public float runDuration;

        /// <summary>UTC timestamp of when the run ended.</summary>
        public DateTime timestamp;

        // --- Daily Run ---

        /// <summary>Whether this run was a daily challenge run with a fixed seed.</summary>
        public bool isDailyRun;

        /// <summary>
        /// The seed used for the daily run's random number generator.
        /// Only meaningful when <see cref="isDailyRun"/> is true.
        /// </summary>
        public int dailySeed;
    }
}
