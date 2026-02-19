using System;
using UnityEngine;
using LootOrLose.Enums;

namespace LootOrLose.Data
{
    /// <summary>
    /// Defines a playable character with unique stats, starting loadout,
    /// and a passive ability. Characters are unlocked through gameplay achievements.
    /// </summary>
    [Serializable]
    public class CharacterData
    {
        /// <summary>Unique identifier for this character definition.</summary>
        public string id;

        /// <summary>Localization key for the character's display name.</summary>
        public string nameKey;

        /// <summary>Localization key for the character's biography or description.</summary>
        public string descriptionKey;

        /// <summary>Localization key for the character's passive ability description.</summary>
        public string passiveAbilityKey;

        /// <summary>The archetype of this character (e.g., Warrior, Rogue, Mage).</summary>
        public CharacterType type;

        /// <summary>
        /// Base hit points this character starts each run with.
        /// May be modified by items and events during the run.
        /// </summary>
        public int baseHP = 100;

        /// <summary>
        /// Maximum number of inventory slots available to this character.
        /// Standard is 5; some characters trade slots for other advantages.
        /// </summary>
        public int inventorySlots = 5;

        /// <summary>
        /// IDs of items this character begins each run with.
        /// Starting items occupy inventory slots as normal.
        /// </summary>
        public string[] startingItemIds;

        /// <summary>
        /// Whether the player has unlocked this character.
        /// The first/default character starts unlocked.
        /// </summary>
        public bool isUnlocked = false;

        /// <summary>
        /// Localization key describing how to unlock this character.
        /// Displayed in the character select screen for locked characters.
        /// </summary>
        public string unlockConditionKey;

        /// <summary>Resource path to the character's portrait icon.</summary>
        public string iconPath;

        /// <summary>Resource path to the character's full sprite sheet.</summary>
        public string spritePath;
    }
}
