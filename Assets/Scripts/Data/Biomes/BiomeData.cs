using System;
using UnityEngine;
using LootOrLose.Enums;

namespace LootOrLose.Data
{
    /// <summary>
    /// Defines a biome (dungeon theme) that determines the visual style,
    /// music, exclusive loot pool, boss roster, and hazard frequencies for a run.
    /// </summary>
    [Serializable]
    public class BiomeData
    {
        /// <summary>Unique identifier for this biome definition.</summary>
        public string id;

        /// <summary>Localization key for the biome's display name.</summary>
        public string nameKey;

        /// <summary>Localization key for the biome's description or flavor text.</summary>
        public string descriptionKey;

        /// <summary>The categorical type of this biome (e.g., Crypt, Forest, Volcano).</summary>
        public BiomeType type;

        /// <summary>
        /// IDs of items that can only be found in this biome.
        /// These items will not appear in other biomes' loot tables.
        /// </summary>
        public string[] exclusiveItemIds;

        /// <summary>
        /// IDs of bosses that can appear as the boss encounter in this biome.
        /// </summary>
        public string[] bossIds;

        /// <summary>
        /// Modifier for trap encounter frequency in this biome.
        /// Values above 1.0 increase trap frequency; below 1.0 decrease it.
        /// </summary>
        public float trapFrequency;

        /// <summary>
        /// Modifier for cursed item frequency in this biome.
        /// Values above 1.0 increase curse frequency; below 1.0 decrease it.
        /// </summary>
        public float curseFrequency;

        /// <summary>Primary theme color used for UI elements in this biome.</summary>
        public Color primaryColor;

        /// <summary>Secondary theme color used for UI accents in this biome.</summary>
        public Color secondaryColor;

        /// <summary>Resource path to the biome's background artwork.</summary>
        public string backgroundPath;

        /// <summary>Resource path to the biome's background music track.</summary>
        public string musicPath;

        /// <summary>
        /// Whether the player has unlocked this biome.
        /// The first/default biome starts unlocked.
        /// </summary>
        public bool isUnlocked = false;
    }
}
