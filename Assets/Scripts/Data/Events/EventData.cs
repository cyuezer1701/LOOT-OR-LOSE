using System;
using UnityEngine;
using LootOrLose.Enums;

namespace LootOrLose.Data
{
    /// <summary>
    /// Defines a random event that can occur during a run.
    /// Events may require specific items (e.g., a key for a chest),
    /// have round-based availability, and are zone-restricted.
    /// </summary>
    [Serializable]
    public class EventData
    {
        /// <summary>Unique identifier for this event definition.</summary>
        public string id;

        /// <summary>Localization key for the event's title text.</summary>
        public string titleKey;

        /// <summary>Localization key for the event's description or narrative text.</summary>
        public string descriptionKey;

        /// <summary>The type of event (e.g., Treasure, Trap, Merchant, Shrine).</summary>
        public LootOrLose.Enums.EventType type;

        /// <summary>
        /// Whether the player needs a specific item category in their inventory
        /// to interact with this event (e.g., a key to open a chest).
        /// </summary>
        public bool requiresItem;

        /// <summary>
        /// The item category required to interact with this event.
        /// Only relevant when <see cref="requiresItem"/> is true.
        /// </summary>
        public ItemCategory requiredItemCategory;

        /// <summary>
        /// Base probability of this event occurring when an event is triggered (0.0 to 1.0).
        /// Actual probability may be modified by biome and zone modifiers.
        /// </summary>
        [Range(0f, 1f)]
        public float probability;

        /// <summary>
        /// The earliest round number at which this event can appear.
        /// Prevents dangerous events from spawning too early in a run.
        /// </summary>
        public int minRound;

        /// <summary>
        /// Biomes where this event is allowed to occur.
        /// An empty array means the event can appear in any biome.
        /// JSON field name is "availableZones" but contains BiomeType values.
        /// </summary>
        public BiomeType[] availableZones;

        /// <summary>Resource path to the event's icon sprite.</summary>
        public string iconPath;
    }
}
