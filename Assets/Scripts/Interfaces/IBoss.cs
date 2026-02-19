using System.Collections.Generic;
using LootOrLose.Enums;

namespace LootOrLose.Interfaces
{
    /// <summary>
    /// Defines the contract for boss encounters that occur every 15 rounds.
    /// Each boss has a weakness to a specific item category, rewarding strategic inventory management.
    /// </summary>
    public interface IBoss
    {
        /// <summary>
        /// The type of boss, determining its unique combat mechanics and visual theme.
        /// </summary>
        BossType Type { get; }

        /// <summary>
        /// Localization key for the boss's display name.
        /// </summary>
        string NameKey { get; }

        /// <summary>
        /// The boss's base hit points before any scaling is applied.
        /// </summary>
        int BaseHP { get; }

        /// <summary>
        /// The item category that deals bonus damage to this boss.
        /// Players who collected items of this category gain an advantage in combat.
        /// </summary>
        ItemCategory Weakness { get; }

        /// <summary>
        /// Calculates the outcome of combat between the player's inventory and this boss.
        /// Evaluates item synergies, weakness exploitation, and overall inventory power.
        /// </summary>
        /// <param name="inventory">The player's current inventory of items brought into the boss fight.</param>
        /// <returns>A <see cref="BossResult"/> describing the combat outcome, damage dealt, and rewards.</returns>
        BossResult CalculateCombat(List<ItemData> inventory);
    }
}
