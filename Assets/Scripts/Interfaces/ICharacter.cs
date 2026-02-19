using System.Collections.Generic;
using LootOrLose.Enums;

namespace LootOrLose.Interfaces
{
    /// <summary>
    /// Defines the contract for playable characters in the game.
    /// Each character has a unique archetype with different starting loadouts,
    /// base stats, and passive abilities that influence playstyle.
    /// </summary>
    public interface ICharacter
    {
        /// <summary>
        /// The archetype of this character (Warrior, Rogue, Mage, Merchant).
        /// </summary>
        CharacterType Type { get; }

        /// <summary>
        /// Localization key for the character's display name.
        /// </summary>
        string NameKey { get; }

        /// <summary>
        /// The character's base hit points at the start of a run.
        /// </summary>
        int BaseHP { get; }

        /// <summary>
        /// The number of inventory slots available to this character.
        /// Defaults to 5 for most characters.
        /// </summary>
        int InventorySlots { get; }

        /// <summary>
        /// List of item IDs that this character starts each run with.
        /// These items are placed in the inventory before round 1 begins.
        /// </summary>
        List<string> StartingItemIds { get; }

        /// <summary>
        /// Localization key for the character's passive ability description.
        /// Each character has a unique passive that provides a persistent gameplay bonus.
        /// </summary>
        string PassiveAbilityKey { get; }
    }
}
