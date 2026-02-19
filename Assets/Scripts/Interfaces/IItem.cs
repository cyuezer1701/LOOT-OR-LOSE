using LootOrLose.Enums;

namespace LootOrLose.Interfaces
{
    /// <summary>
    /// Defines the contract for all items in the game.
    /// Items are found each round and the player has 3 seconds to decide whether to loot or leave them.
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Unique identifier for this item definition.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Localization key for the item's display name.
        /// Resolved at runtime through the i18n system.
        /// </summary>
        string NameKey { get; }

        /// <summary>
        /// Localization key for the item's description text.
        /// Resolved at runtime through the i18n system.
        /// </summary>
        string DescriptionKey { get; }

        /// <summary>
        /// The functional category of the item (Weapon, Defense, Consumable, etc.).
        /// Determines how the item interacts with synergies and boss weaknesses.
        /// </summary>
        ItemCategory Category { get; }

        /// <summary>
        /// The rarity tier of the item, affecting its power level and drop frequency.
        /// </summary>
        ItemRarity Rarity { get; }

        /// <summary>
        /// The number of inventory slots this item occupies.
        /// Must be 1 or 2. The player has 5 inventory slots by default.
        /// </summary>
        int SlotSize { get; }

        /// <summary>
        /// Path to the item's icon sprite resource.
        /// Used by the UI layer to display the item visually.
        /// </summary>
        string IconPath { get; }
    }
}
