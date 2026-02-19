using System;
using UnityEngine;
using LootOrLose.Enums;

namespace LootOrLose.Data
{
    /// <summary>
    /// Defines a boss encounter that appears every 15 rounds.
    /// Bosses have scaling HP, category-based weaknesses and resistances,
    /// and guaranteed rare+ item drops on defeat.
    /// </summary>
    [Serializable]
    public class BossData
    {
        /// <summary>Unique identifier for this boss definition.</summary>
        public string id;

        /// <summary>Localization key for the boss's display name.</summary>
        public string nameKey;

        /// <summary>Localization key for the boss's description or flavor text.</summary>
        public string descriptionKey;

        /// <summary>The archetype of this boss (e.g., Brute, Sorcerer, Guardian).</summary>
        public BossType type;

        /// <summary>Base hit points before level scaling is applied.</summary>
        public int baseHP;

        /// <summary>
        /// Additional HP added per level (round number / 15).
        /// Effective HP = baseHP + (scalingPerLevel * bossLevel).
        /// </summary>
        public int scalingPerLevel;

        /// <summary>
        /// Item category this boss is weak against.
        /// Items of this category deal bonus damage to the boss.
        /// </summary>
        public ItemCategory weakness;

        /// <summary>
        /// Item category this boss resists.
        /// Items of this category deal reduced damage to the boss.
        /// </summary>
        public ItemCategory resistance;

        /// <summary>
        /// IDs of items guaranteed to drop when this boss is defeated.
        /// These should be Rare or higher rarity items.
        /// </summary>
        public string[] guaranteedDropIds;

        /// <summary>
        /// The earliest round number at which this boss can appear.
        /// Prevents overly difficult bosses from spawning in early rounds.
        /// </summary>
        public int minRound;

        /// <summary>Resource path to the boss's icon sprite.</summary>
        public string iconPath;

        /// <summary>
        /// The biome where this boss is most likely to appear.
        /// The boss can still appear in other biomes at reduced probability.
        /// </summary>
        public BiomeType preferredBiome;
    }
}
