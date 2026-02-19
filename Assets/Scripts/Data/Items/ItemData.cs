using System;
using UnityEngine;
using LootOrLose.Enums;

namespace LootOrLose.Data
{
    /// <summary>
    /// Defines a single item that can appear as loot during a run.
    /// Items have combat stats, synergy relationships, and biome-specific availability.
    /// Players must decide to LOOT or LEAVE each item within 3 seconds.
    /// </summary>
    [Serializable]
    public class ItemData
    {
        /// <summary>Unique identifier for this item definition.</summary>
        public string id;

        /// <summary>Localization key for the item's display name.</summary>
        public string nameKey;

        /// <summary>Localization key for the item's description text.</summary>
        public string descriptionKey;

        /// <summary>The functional category of this item (Weapon, Armor, Consumable, etc.).</summary>
        public ItemCategory category;

        /// <summary>The rarity tier affecting drop rates and power level.</summary>
        public ItemRarity rarity;

        /// <summary>
        /// Number of inventory slots this item occupies (1 or 2).
        /// Powerful items may cost 2 of the player's 5 available slots.
        /// </summary>
        public int slotSize = 1;

        /// <summary>Resource path to the item's icon sprite.</summary>
        public string iconPath;

        // --- Combat Stats ---

        /// <summary>Bonus attack power granted while this item is in the inventory.</summary>
        public int attackPower;

        /// <summary>Bonus defense power granted while this item is in the inventory.</summary>
        public int defensePower;

        /// <summary>Amount of HP restored when this item is used (consumables) or passively.</summary>
        public int healAmount;

        // --- Special Properties ---

        /// <summary>
        /// Whether this item carries a curse. Cursed items have powerful stats
        /// but inflict a negative effect on the player.
        /// </summary>
        public bool isCursed;

        /// <summary>
        /// Whether this item is consumed on use and removed from the inventory.
        /// Consumables trigger their effect once and are gone.
        /// </summary>
        public bool isConsumable;

        /// <summary>
        /// IDs of items that synergize with this one, granting bonus effects
        /// when both are held in the inventory simultaneously.
        /// </summary>
        public string[] synergyItemIds;

        /// <summary>
        /// IDs of items that conflict with this one, causing negative effects
        /// or canceling bonuses when both are held.
        /// </summary>
        public string[] antiSynergyItemIds;

        // --- Biome Availability ---

        /// <summary>
        /// Biomes where this item can drop. An empty array means the item
        /// is available in all biomes.
        /// </summary>
        public BiomeType[] availableBiomes;

        // --- Drop Calculation ---

        /// <summary>
        /// Relative weight used in the loot drop probability calculation.
        /// Higher values make this item more likely to appear.
        /// </summary>
        public float dropWeight = 1f;
    }
}
