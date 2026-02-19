using System;
using System.Collections.Generic;
using System.Linq;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.Core.Items
{
    /// <summary>
    /// Generates random items based on current round, zone, biome, and rarity weights.
    /// Pure logic - no Unity dependencies.
    /// </summary>
    public static class ItemGenerator
    {
        /// <summary>
        /// Generate a random item appropriate for the current game state.
        /// </summary>
        /// <param name="itemPool">The full pool of available item definitions.</param>
        /// <param name="currentRound">The current round number (1-based).</param>
        /// <param name="biome">The active biome for the current run.</param>
        /// <param name="zone">The current dungeon zone derived from the round.</param>
        /// <param name="random">Seeded random instance for deterministic generation.</param>
        /// <returns>A randomly selected item, or null if no items are available.</returns>
        public static ItemData GenerateItem(
            List<ItemData> itemPool,
            int currentRound,
            BiomeType biome,
            DungeonZone zone,
            Random random)
        {
            if (itemPool == null || itemPool.Count == 0)
                return null;

            // Filter by biome availability
            var available = itemPool.Where(item =>
                item.availableBiomes == null ||
                item.availableBiomes.Length == 0 ||
                item.availableBiomes.Contains(biome)).ToList();

            // Filter by zone restrictions
            available = FilterByZone(available, zone);

            if (available.Count == 0)
                return null;

            // Calculate weighted probabilities based on rarity and round
            var weights = CalculateWeights(available, currentRound, zone);

            return SelectWeighted(available, weights, random);
        }

        /// <summary>
        /// Filter items based on dungeon zone.
        /// Tutorial: no traps, no curses, only common/uncommon.
        /// Standard: all categories, all rarities.
        /// Danger: higher curse/trap frequency, better rarity chances.
        /// Chaos: everything, highest rarity chances.
        /// </summary>
        /// <param name="items">The items to filter.</param>
        /// <param name="zone">The dungeon zone to filter for.</param>
        /// <returns>A filtered list of items appropriate for the zone.</returns>
        public static List<ItemData> FilterByZone(List<ItemData> items, DungeonZone zone)
        {
            if (items == null)
                return new List<ItemData>();

            switch (zone)
            {
                case DungeonZone.Tutorial:
                    return items.Where(i =>
                        i.category != ItemCategory.Trap &&
                        i.category != ItemCategory.Curse &&
                        (i.rarity == ItemRarity.Common || i.rarity == ItemRarity.Uncommon))
                        .ToList();
                case DungeonZone.Standard:
                    return new List<ItemData>(items);
                case DungeonZone.Danger:
                    return new List<ItemData>(items);
                case DungeonZone.Chaos:
                    return new List<ItemData>(items);
                default:
                    return new List<ItemData>(items);
            }
        }

        /// <summary>
        /// Calculate drop weights adjusted for round and zone.
        /// Higher rounds and more dangerous zones yield better rarity chances.
        /// </summary>
        /// <param name="items">The items to calculate weights for.</param>
        /// <param name="round">The current round number.</param>
        /// <param name="zone">The current dungeon zone.</param>
        /// <returns>An array of weights corresponding to each item in the list.</returns>
        public static float[] CalculateWeights(List<ItemData> items, int round, DungeonZone zone)
        {
            if (items == null || items.Count == 0)
                return new float[0];

            float[] weights = new float[items.Count];
            float rarityMultiplier = GetRarityMultiplier(zone);

            for (int i = 0; i < items.Count; i++)
            {
                float baseWeight = items[i].dropWeight;
                float rarityBonus = GetRarityWeight(items[i].rarity, rarityMultiplier);
                weights[i] = baseWeight * rarityBonus;
            }

            return weights;
        }

        /// <summary>
        /// Returns the rarity multiplier for a given dungeon zone.
        /// Higher zones grant higher multipliers, improving drop quality.
        /// </summary>
        private static float GetRarityMultiplier(DungeonZone zone)
        {
            switch (zone)
            {
                case DungeonZone.Tutorial: return 0.5f;
                case DungeonZone.Standard: return 1.0f;
                case DungeonZone.Danger: return 1.5f;
                case DungeonZone.Chaos: return 2.5f;
                default: return 1.0f;
            }
        }

        /// <summary>
        /// Returns the base weight for a given rarity tier, scaled by the zone multiplier.
        /// Common items are always frequent; higher rarities become more likely with higher multipliers.
        /// </summary>
        private static float GetRarityWeight(ItemRarity rarity, float multiplier)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return 60f;
                case ItemRarity.Uncommon: return 25f * multiplier;
                case ItemRarity.Rare: return 10f * multiplier;
                case ItemRarity.Legendary: return 5f * multiplier;
                default: return 1f;
            }
        }

        /// <summary>
        /// Select an item using weighted random selection.
        /// Each item's chance of being selected is proportional to its weight relative to the total.
        /// </summary>
        /// <typeparam name="T">The type of item to select.</typeparam>
        /// <param name="items">The list of candidates.</param>
        /// <param name="weights">Corresponding weights for each candidate.</param>
        /// <param name="random">Random instance for the roll.</param>
        /// <returns>The selected item.</returns>
        public static T SelectWeighted<T>(List<T> items, float[] weights, Random random)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("Items list cannot be null or empty.", nameof(items));
            if (weights == null || weights.Length != items.Count)
                throw new ArgumentException("Weights array must match items count.", nameof(weights));

            float totalWeight = 0f;
            for (int i = 0; i < weights.Length; i++)
                totalWeight += weights[i];

            if (totalWeight <= 0f)
                return items[0];

            float roll = (float)(random.NextDouble() * totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < items.Count; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative)
                    return items[i];
            }

            // Fallback: return last item (should not normally be reached)
            return items[items.Count - 1];
        }

        /// <summary>
        /// Get the dungeon zone for a given round number.
        /// Rounds 1-10 = Tutorial, 11-25 = Standard, 26-40 = Danger, 41+ = Chaos.
        /// </summary>
        /// <param name="round">The current round number (1-based).</param>
        /// <returns>The dungeon zone corresponding to the round.</returns>
        public static DungeonZone GetZoneForRound(int round)
        {
            if (round <= 10) return DungeonZone.Tutorial;
            if (round <= 25) return DungeonZone.Standard;
            if (round <= 40) return DungeonZone.Danger;
            return DungeonZone.Chaos;
        }

        /// <summary>
        /// Check if the current round is a boss round.
        /// Bosses appear every 15 rounds: 15, 30, 45, etc.
        /// </summary>
        /// <param name="round">The round number to check.</param>
        /// <returns>True if this round triggers a boss encounter.</returns>
        public static bool IsBossRound(int round)
        {
            return round > 0 && round % 15 == 0;
        }
    }
}
