using System;
using System.Collections.Generic;
using System.Linq;
using LootOrLose.Core.Items;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.Core.Scoring
{
    // ─────────────────────────────────────────────
    //  Input / Output Structs
    // ─────────────────────────────────────────────

    /// <summary>
    /// All data needed to calculate the final score for a completed run.
    /// </summary>
    public struct RunScoreInput
    {
        /// <summary>How many rounds the player survived.</summary>
        public int roundsCompleted;

        /// <summary>How many bosses the player defeated.</summary>
        public int bossesDefeated;

        /// <summary>The items remaining in the player's inventory at run end.</summary>
        public List<ItemData> finalInventory;

        /// <summary>Active synergies at the time the run ended.</summary>
        public List<SynergyResult> synergies;

        /// <summary>The player's current daily play streak.</summary>
        public int currentStreak;

        /// <summary>Whether this was a daily challenge run.</summary>
        public bool isDailyRun;
    }

    /// <summary>
    /// Detailed breakdown of how the final score was calculated.
    /// </summary>
    public struct ScoreBreakdown
    {
        /// <summary>Points earned from rounds completed (10 points per round).</summary>
        public int roundScore;

        /// <summary>Points earned from bosses defeated (500 points per boss).</summary>
        public int bossScore;

        /// <summary>Points earned from active synergies (100 points per synergy).</summary>
        public int synergyScore;

        /// <summary>Points earned from item rarities in the final inventory.</summary>
        public int rarityScore;

        /// <summary>
        /// The streak multiplier applied to the total score.
        /// Calculated as 1.0 + (streak * 0.1), capped at 2.0.
        /// </summary>
        public float streakMultiplier;

        /// <summary>The final total score after all bonuses and multipliers.</summary>
        public int totalScore;
    }

    // ─────────────────────────────────────────────
    //  Calculator
    // ─────────────────────────────────────────────

    /// <summary>
    /// Calculates the final score for a completed game run.
    /// Scoring rewards exploration (rounds), combat (bosses), strategy (synergies),
    /// collection (rarity), and consistency (streak multiplier).
    /// Pure logic - no Unity dependencies.
    /// </summary>
    public static class ScoreCalculator
    {
        /// <summary>Points awarded per round completed.</summary>
        public const int PointsPerRound = 10;

        /// <summary>Points awarded per boss defeated.</summary>
        public const int PointsPerBoss = 500;

        /// <summary>Points awarded per active synergy.</summary>
        public const int PointsPerSynergy = 100;

        /// <summary>Maximum streak multiplier cap.</summary>
        public const float MaxStreakMultiplier = 2.0f;

        /// <summary>Streak multiplier increment per streak day.</summary>
        public const float StreakMultiplierStep = 0.1f;

        /// <summary>
        /// Calculate the full score breakdown for a completed run.
        /// </summary>
        /// <param name="input">All run data needed for scoring.</param>
        /// <returns>A detailed <see cref="ScoreBreakdown"/> with individual components and total.</returns>
        public static ScoreBreakdown CalculateRunScore(RunScoreInput input)
        {
            int roundScore = CalculateRoundScore(input.roundsCompleted);
            int bossScore = CalculateBossScore(input.bossesDefeated);
            int synergyScore = CalculateSynergyScore(input.synergies);
            int rarityScore = CalculateRarityScore(input.finalInventory);
            float streakMultiplier = CalculateStreakMultiplier(input.currentStreak);

            int subtotal = roundScore + bossScore + synergyScore + rarityScore;
            int totalScore = (int)(subtotal * streakMultiplier);

            return new ScoreBreakdown
            {
                roundScore = roundScore,
                bossScore = bossScore,
                synergyScore = synergyScore,
                rarityScore = rarityScore,
                streakMultiplier = streakMultiplier,
                totalScore = totalScore
            };
        }

        /// <summary>
        /// Calculate the round-based score component.
        /// </summary>
        /// <param name="roundsCompleted">Number of rounds the player survived.</param>
        /// <returns>Points from rounds (10 per round).</returns>
        public static int CalculateRoundScore(int roundsCompleted)
        {
            return Math.Max(roundsCompleted, 0) * PointsPerRound;
        }

        /// <summary>
        /// Calculate the boss-based score component.
        /// </summary>
        /// <param name="bossesDefeated">Number of bosses the player defeated.</param>
        /// <returns>Points from bosses (500 per boss).</returns>
        public static int CalculateBossScore(int bossesDefeated)
        {
            return Math.Max(bossesDefeated, 0) * PointsPerBoss;
        }

        /// <summary>
        /// Calculate the synergy-based score component.
        /// </summary>
        /// <param name="synergies">Active synergies at end of run.</param>
        /// <returns>Points from synergies (100 per synergy).</returns>
        public static int CalculateSynergyScore(List<SynergyResult> synergies)
        {
            if (synergies == null)
                return 0;

            return synergies.Count * PointsPerSynergy;
        }

        /// <summary>
        /// Calculate the rarity-based score component from the final inventory.
        /// Rarity values: Common = 10, Uncommon = 25, Rare = 50, Legendary = 100.
        /// </summary>
        /// <param name="inventory">The player's final inventory.</param>
        /// <returns>Sum of rarity point values for all items.</returns>
        public static int CalculateRarityScore(List<ItemData> inventory)
        {
            if (inventory == null || inventory.Count == 0)
                return 0;

            int total = 0;
            foreach (var item in inventory)
            {
                total += GetRarityValue(item.rarity);
            }
            return total;
        }

        /// <summary>
        /// Get the point value for a given item rarity.
        /// </summary>
        /// <param name="rarity">The item's rarity tier.</param>
        /// <returns>Point value: Common=10, Uncommon=25, Rare=50, Legendary=100.</returns>
        public static int GetRarityValue(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return 10;
                case ItemRarity.Uncommon: return 25;
                case ItemRarity.Rare: return 50;
                case ItemRarity.Legendary: return 100;
                default: return 0;
            }
        }

        /// <summary>
        /// Calculate the streak multiplier from the player's current daily streak.
        /// Formula: 1.0 + (streak * 0.1), capped at 2.0.
        /// </summary>
        /// <param name="currentStreak">The player's current consecutive-day streak.</param>
        /// <returns>The multiplier to apply to the total score (1.0 to 2.0).</returns>
        public static float CalculateStreakMultiplier(int currentStreak)
        {
            float multiplier = 1.0f + (Math.Max(currentStreak, 0) * StreakMultiplierStep);
            return Math.Min(multiplier, MaxStreakMultiplier);
        }
    }
}
