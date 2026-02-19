using System;
using System.Collections.Generic;
using System.Linq;
using LootOrLose.Data;
using LootOrLose.Enums;
using LootOrLose.State;

namespace LootOrLose.Core.Progression
{
    // ─────────────────────────────────────────────
    //  Result Structs
    // ─────────────────────────────────────────────

    /// <summary>
    /// Describes a single unlock that the player has earned through progression.
    /// </summary>
    public struct UnlockResult
    {
        /// <summary>
        /// The category of unlock: "character", "biome", or "item_type".
        /// </summary>
        public string unlockType;

        /// <summary>
        /// The specific ID of the unlocked content (e.g., "rogue", "volcano").
        /// </summary>
        public string unlockId;

        /// <summary>
        /// Localization key describing the condition that was met to earn this unlock.
        /// </summary>
        public string conditionKey;
    }

    /// <summary>
    /// Summary of a completed run, used to check for newly earned achievements.
    /// </summary>
    public struct RunResult
    {
        /// <summary>The highest round reached in this run.</summary>
        public int roundsCompleted;

        /// <summary>Number of bosses defeated in this run.</summary>
        public int bossesDefeated;

        /// <summary>The final score achieved in this run.</summary>
        public int finalScore;

        /// <summary>The final inventory at run end.</summary>
        public List<ItemData> finalInventory;

        /// <summary>Whether the player survived (was not killed).</summary>
        public bool survived;

        /// <summary>Total number of cursed items held during the run.</summary>
        public int cursedItemsHeld;

        /// <summary>The biome this run was played in.</summary>
        public BiomeType biome;

        /// <summary>IDs of bosses defeated in this run.</summary>
        public List<string> defeatedBossIds;
    }

    // ─────────────────────────────────────────────
    //  Manager
    // ─────────────────────────────────────────────

    /// <summary>
    /// Calculates meta-progression unlocks and achievement completions.
    /// Evaluates player lifetime statistics against unlock conditions and
    /// run results against achievement criteria.
    /// Pure logic - no Unity dependencies.
    /// </summary>
    public static class ProgressionManager
    {
        // ─────────────────────────────────────────────
        //  Unlock Calculation
        // ─────────────────────────────────────────────

        /// <summary>
        /// Evaluate all unlock conditions against the player's current progression state.
        /// Returns only NEW unlocks that the player has not yet earned.
        /// </summary>
        /// <param name="progress">The player's persistent progression data.</param>
        /// <returns>A list of newly earned unlocks.</returns>
        public static List<UnlockResult> CalculateUnlocks(PlayerProgressState progress)
        {
            var unlocks = new List<UnlockResult>();

            if (progress == null)
                return unlocks;

            // --- Character Unlocks ---

            // Rogue: 10 total runs
            if (!IsUnlocked(progress.unlockedCharacterIds, "rogue") && progress.totalRuns >= 10)
            {
                unlocks.Add(new UnlockResult
                {
                    unlockType = "character",
                    unlockId = "rogue",
                    conditionKey = "unlock_rogue_condition"
                });
            }

            // Mage: defeat 5 bosses total
            if (!IsUnlocked(progress.unlockedCharacterIds, "mage") && progress.totalBossesDefeated >= 5)
            {
                unlocks.Add(new UnlockResult
                {
                    unlockType = "character",
                    unlockId = "mage",
                    conditionKey = "unlock_mage_condition"
                });
            }

            // Merchant: loot 100 items total
            if (!IsUnlocked(progress.unlockedCharacterIds, "merchant") && progress.totalItemsLooted >= 100)
            {
                unlocks.Add(new UnlockResult
                {
                    unlockType = "character",
                    unlockId = "merchant",
                    conditionKey = "unlock_merchant_condition"
                });
            }

            // --- Biome Unlocks ---

            // Volcano: reach round 30
            if (!IsUnlocked(progress.unlockedBiomeIds, "volcano") && progress.bestRound >= 30)
            {
                unlocks.Add(new UnlockResult
                {
                    unlockType = "biome",
                    unlockId = "volcano",
                    conditionKey = "unlock_volcano_condition"
                });
            }

            // IcePalace: defeat SkeletonKing boss
            if (!IsUnlocked(progress.unlockedBiomeIds, "ice_palace") &&
                HasDefeatedBoss(progress, "skeleton_king"))
            {
                unlocks.Add(new UnlockResult
                {
                    unlockType = "biome",
                    unlockId = "ice_palace",
                    conditionKey = "unlock_ice_palace_condition"
                });
            }

            // Abyss: complete a run in all other biomes with score > 5000
            if (!IsUnlocked(progress.unlockedBiomeIds, "abyss") &&
                HasCompletedAllBiomesWithScore(progress, 5000))
            {
                unlocks.Add(new UnlockResult
                {
                    unlockType = "biome",
                    unlockId = "abyss",
                    conditionKey = "unlock_abyss_condition"
                });
            }

            return unlocks;
        }

        // ─────────────────────────────────────────────
        //  Achievement Checks
        // ─────────────────────────────────────────────

        /// <summary>
        /// Check which achievements were newly earned based on the latest run result
        /// and the player's overall progression state.
        /// Returns only achievement IDs that the player has not yet earned.
        /// </summary>
        /// <param name="result">The result of the just-completed run.</param>
        /// <param name="progress">The player's persistent progression data.</param>
        /// <returns>A list of newly earned achievement IDs.</returns>
        public static List<string> CheckAchievements(RunResult result, PlayerProgressState progress)
        {
            var newAchievements = new List<string>();

            if (progress == null)
                return newAchievements;

            var earned = progress.unlockedAchievementIds ?? new List<string>();

            // survivor_50: survived 50 rounds in a single run
            if (!earned.Contains("survivor_50") && result.roundsCompleted >= 50)
                newAchievements.Add("survivor_50");

            // cursed_collector: held 3+ cursed items and survived
            if (!earned.Contains("cursed_collector") && result.cursedItemsHeld >= 3 && result.survived)
                newAchievements.Add("cursed_collector");

            // pacifist: completed a run with no weapons in final inventory
            if (!earned.Contains("pacifist") && result.survived && HasNoWeapons(result.finalInventory))
                newAchievements.Add("pacifist");

            // dragon_slayer: defeated the FireDragon boss
            if (!earned.Contains("dragon_slayer") && HasDefeatedBossInRun(result, "fire_dragon"))
                newAchievements.Add("dragon_slayer");

            // first_blood: defeat your first boss ever
            if (!earned.Contains("first_blood") && result.bossesDefeated >= 1)
                newAchievements.Add("first_blood");

            // hoarder: finish a run with a full inventory (5 items)
            if (!earned.Contains("hoarder") && result.survived &&
                result.finalInventory != null && result.finalInventory.Count >= 5)
                newAchievements.Add("hoarder");

            // legendary_collector: finish a run with a Legendary item
            if (!earned.Contains("legendary_collector") && result.survived &&
                result.finalInventory != null &&
                result.finalInventory.Any(i => i.rarity == ItemRarity.Legendary))
                newAchievements.Add("legendary_collector");

            // boss_slayer_3: defeat 3 bosses in a single run
            if (!earned.Contains("boss_slayer_3") && result.bossesDefeated >= 3)
                newAchievements.Add("boss_slayer_3");

            // high_scorer: achieve a score of 5000+
            if (!earned.Contains("high_scorer") && result.finalScore >= 5000)
                newAchievements.Add("high_scorer");

            // skeleton_king_slayer: defeated the SkeletonKing
            if (!earned.Contains("skeleton_king_slayer") && HasDefeatedBossInRun(result, "skeleton_king"))
                newAchievements.Add("skeleton_king_slayer");

            // ice_queen_slayer: defeated the IceQueen
            if (!earned.Contains("ice_queen_slayer") && HasDefeatedBossInRun(result, "ice_queen"))
                newAchievements.Add("ice_queen_slayer");

            // abyss_lord_slayer: defeated the AbyssLord
            if (!earned.Contains("abyss_lord_slayer") && HasDefeatedBossInRun(result, "abyss_lord"))
                newAchievements.Add("abyss_lord_slayer");

            // veteran_100: completed 100 total runs
            if (!earned.Contains("veteran_100") && progress.totalRuns >= 100)
                newAchievements.Add("veteran_100");

            return newAchievements;
        }

        // ─────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────

        /// <summary>
        /// Check if a given ID is already present in an unlock list.
        /// </summary>
        private static bool IsUnlocked(List<string> unlockedIds, string id)
        {
            if (unlockedIds == null)
                return false;
            return unlockedIds.Contains(id);
        }

        /// <summary>
        /// Check if the player has ever defeated a specific boss based on achievement history.
        /// Uses the convention that defeating a boss earns an achievement with the boss ID.
        /// </summary>
        private static bool HasDefeatedBoss(PlayerProgressState progress, string bossId)
        {
            if (progress.unlockedAchievementIds == null)
                return false;

            // Check if the boss-specific achievement exists
            return progress.unlockedAchievementIds.Contains(bossId + "_slayer") ||
                   progress.unlockedAchievementIds.Contains(bossId + "_defeated");
        }

        /// <summary>
        /// Check if a specific boss was defeated in the current run.
        /// </summary>
        private static bool HasDefeatedBossInRun(RunResult result, string bossId)
        {
            if (result.defeatedBossIds == null)
                return false;
            return result.defeatedBossIds.Contains(bossId);
        }

        /// <summary>
        /// Check if the player has completed runs in all non-Abyss biomes with a score above the threshold.
        /// This is tracked via biome-specific achievements in the progress state.
        /// </summary>
        private static bool HasCompletedAllBiomesWithScore(PlayerProgressState progress, int minScore)
        {
            if (progress.unlockedAchievementIds == null)
                return false;

            // Required biomes: Crypt, Volcano, IcePalace (all except Abyss)
            var requiredBiomes = new[] { "crypt", "volcano", "ice_palace" };

            foreach (var biome in requiredBiomes)
            {
                // Convention: biome completion with score tracked as "biome_{id}_score_{threshold}"
                string achievementKey = "biome_" + biome + "_score_" + minScore;
                if (!progress.unlockedAchievementIds.Contains(achievementKey))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if the player's final inventory contains no Weapon-category items.
        /// </summary>
        private static bool HasNoWeapons(List<ItemData> inventory)
        {
            if (inventory == null || inventory.Count == 0)
                return true;

            return !inventory.Any(i => i.category == ItemCategory.Weapon);
        }
    }
}
