using System;
using System.Collections.Generic;
using System.Linq;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.Core.Items
{
    // ─────────────────────────────────────────────
    //  Result Structs
    // ─────────────────────────────────────────────

    /// <summary>
    /// Describes a positive synergy bonus triggered by a specific item combination.
    /// </summary>
    public struct SynergyResult
    {
        /// <summary>The type of synergy that was triggered.</summary>
        public SynergyType type;

        /// <summary>Localization key describing the synergy effect.</summary>
        public string descriptionKey;

        /// <summary>Bonus attack power granted by this synergy.</summary>
        public int bonusAttack;

        /// <summary>Bonus defense power granted by this synergy.</summary>
        public int bonusDefense;

        /// <summary>Bonus hit points granted by this synergy.</summary>
        public int bonusHP;
    }

    /// <summary>
    /// Describes a negative anti-synergy penalty when conflicting items are held together.
    /// </summary>
    public struct AntiSynergyResult
    {
        /// <summary>ID of the first conflicting item.</summary>
        public string item1Id;

        /// <summary>ID of the second conflicting item.</summary>
        public string item2Id;

        /// <summary>Localization key describing the anti-synergy effect.</summary>
        public string descriptionKey;

        /// <summary>Damage inflicted on the player due to the conflict.</summary>
        public int damage;

        /// <summary>Whether this anti-synergy is instantly fatal (permadeath trigger).</summary>
        public bool isFatal;
    }

    /// <summary>
    /// Aggregated totals from all active synergy and anti-synergy effects.
    /// </summary>
    public struct SynergyTotals
    {
        /// <summary>Total bonus attack from all synergies.</summary>
        public int totalBonusAttack;

        /// <summary>Total bonus defense from all synergies.</summary>
        public int totalBonusDefense;

        /// <summary>Total bonus HP from all synergies.</summary>
        public int totalBonusHP;

        /// <summary>Total damage from all anti-synergies.</summary>
        public int totalAntiSynergyDamage;

        /// <summary>Whether any anti-synergy is fatal.</summary>
        public bool hasFatalAntiSynergy;
    }

    // ─────────────────────────────────────────────
    //  Calculator
    // ─────────────────────────────────────────────

    /// <summary>
    /// Calculates item synergies and anti-synergies for a player's inventory.
    /// Synergies reward strategic item collection; anti-synergies punish conflicting combinations.
    /// Pure logic - no Unity dependencies.
    /// </summary>
    public static class SynergyCalculator
    {
        /// <summary>
        /// Evaluate all positive synergies present in the given inventory.
        /// </summary>
        /// <param name="inventory">The player's current inventory.</param>
        /// <returns>A list of all active synergy bonuses.</returns>
        public static List<SynergyResult> CheckSynergies(List<ItemData> inventory)
        {
            var results = new List<SynergyResult>();

            if (inventory == null || inventory.Count == 0)
                return results;

            // Check each synergy type
            var armorSet = CheckArmorSet(inventory);
            if (armorSet.HasValue)
                results.Add(armorSet.Value);

            var dualWield = CheckDualWield(inventory);
            if (dualWield.HasValue)
                results.Add(dualWield.Value);

            var potionMaster = CheckPotionMaster(inventory);
            if (potionMaster.HasValue)
                results.Add(potionMaster.Value);

            var keyCollector = CheckKeyCollector(inventory);
            if (keyCollector.HasValue)
                results.Add(keyCollector.Value);

            var elementalCombo = CheckElementalCombo(inventory);
            if (elementalCombo.HasValue)
                results.Add(elementalCombo.Value);

            // Check item-defined synergies (from synergyItemIds)
            results.AddRange(CheckItemDefinedSynergies(inventory));

            return results;
        }

        /// <summary>
        /// Evaluate all anti-synergies (negative conflicts) in the given inventory.
        /// Anti-synergies occur when items with conflicting IDs are held simultaneously.
        /// </summary>
        /// <param name="inventory">The player's current inventory.</param>
        /// <returns>A list of all active anti-synergy penalties.</returns>
        public static List<AntiSynergyResult> CheckAntiSynergies(List<ItemData> inventory)
        {
            var results = new List<AntiSynergyResult>();

            if (inventory == null || inventory.Count == 0)
                return results;

            // Track already-reported pairs to avoid duplicates
            var reportedPairs = new HashSet<string>();

            for (int i = 0; i < inventory.Count; i++)
            {
                var item = inventory[i];
                if (item.antiSynergyItemIds == null || item.antiSynergyItemIds.Length == 0)
                    continue;

                for (int j = 0; j < inventory.Count; j++)
                {
                    if (i == j) continue;

                    var other = inventory[j];
                    if (!item.antiSynergyItemIds.Contains(other.id))
                        continue;

                    // Create a canonical pair key to avoid duplicate reports
                    string pairKey = string.Compare(item.id, other.id, StringComparison.Ordinal) < 0
                        ? item.id + "|" + other.id
                        : other.id + "|" + item.id;

                    if (reportedPairs.Contains(pairKey))
                        continue;

                    reportedPairs.Add(pairKey);

                    // Cursed + cursed combination is fatal
                    bool fatal = item.isCursed && other.isCursed;

                    results.Add(new AntiSynergyResult
                    {
                        item1Id = item.id,
                        item2Id = other.id,
                        descriptionKey = "antisynergy_conflict",
                        damage = fatal ? 999 : 10,
                        isFatal = fatal
                    });
                }
            }

            return results;
        }

        /// <summary>
        /// Calculate the combined totals from all synergies and anti-synergies in the inventory.
        /// </summary>
        /// <param name="inventory">The player's current inventory.</param>
        /// <returns>Aggregated synergy and anti-synergy totals.</returns>
        public static SynergyTotals CalculateTotalSynergyBonus(List<ItemData> inventory)
        {
            var synergies = CheckSynergies(inventory);
            var antiSynergies = CheckAntiSynergies(inventory);

            var totals = new SynergyTotals();

            foreach (var syn in synergies)
            {
                totals.totalBonusAttack += syn.bonusAttack;
                totals.totalBonusDefense += syn.bonusDefense;
                totals.totalBonusHP += syn.bonusHP;
            }

            foreach (var anti in antiSynergies)
            {
                totals.totalAntiSynergyDamage += anti.damage;
                if (anti.isFatal)
                    totals.hasFatalAntiSynergy = true;
            }

            return totals;
        }

        // ─────────────────────────────────────────────
        //  Individual Synergy Checks
        // ─────────────────────────────────────────────

        /// <summary>
        /// Armor Set synergy: if inventory contains Defense-category items whose IDs
        /// include "shield", "armor", and "helmet", grant bonus defense and HP.
        /// </summary>
        /// <param name="inventory">The player's current inventory.</param>
        /// <returns>The synergy result if triggered, or null.</returns>
        public static SynergyResult? CheckArmorSet(List<ItemData> inventory)
        {
            var defenseItems = inventory.Where(i => i.category == ItemCategory.Defense).ToList();

            bool hasShield = defenseItems.Any(i => i.id.Contains("shield"));
            bool hasArmor = defenseItems.Any(i => i.id.Contains("armor"));
            bool hasHelmet = defenseItems.Any(i => i.id.Contains("helmet"));

            if (hasShield && hasArmor && hasHelmet)
            {
                return new SynergyResult
                {
                    type = SynergyType.ArmorSet,
                    descriptionKey = "synergy_armor_set",
                    bonusAttack = 0,
                    bonusDefense = 15,
                    bonusHP = 20
                };
            }

            return null;
        }

        /// <summary>
        /// Dual Wield synergy: if inventory contains two or more Weapon-category items,
        /// grant bonus attack power.
        /// </summary>
        /// <param name="inventory">The player's current inventory.</param>
        /// <returns>The synergy result if triggered, or null.</returns>
        public static SynergyResult? CheckDualWield(List<ItemData> inventory)
        {
            int weaponCount = inventory.Count(i => i.category == ItemCategory.Weapon);

            if (weaponCount >= 2)
            {
                return new SynergyResult
                {
                    type = SynergyType.DualWield,
                    descriptionKey = "synergy_dual_wield",
                    bonusAttack = 10 * weaponCount,
                    bonusDefense = 0,
                    bonusHP = 0
                };
            }

            return null;
        }

        /// <summary>
        /// Potion Master synergy: if inventory contains 3 or more Consumable-category items,
        /// grant bonus HP representing increased effectiveness.
        /// </summary>
        /// <param name="inventory">The player's current inventory.</param>
        /// <returns>The synergy result if triggered, or null.</returns>
        public static SynergyResult? CheckPotionMaster(List<ItemData> inventory)
        {
            int consumableCount = inventory.Count(i => i.category == ItemCategory.Consumable);

            if (consumableCount >= 3)
            {
                return new SynergyResult
                {
                    type = SynergyType.PotionMaster,
                    descriptionKey = "synergy_potion_master",
                    bonusAttack = 0,
                    bonusDefense = 5,
                    bonusHP = 15 * consumableCount
                };
            }

            return null;
        }

        /// <summary>
        /// Key Collector synergy: if inventory contains 3 or more Key-category items,
        /// grant bonus stats representing the unlock bonus.
        /// </summary>
        /// <param name="inventory">The player's current inventory.</param>
        /// <returns>The synergy result if triggered, or null.</returns>
        public static SynergyResult? CheckKeyCollector(List<ItemData> inventory)
        {
            int keyCount = inventory.Count(i => i.category == ItemCategory.Key);

            if (keyCount >= 3)
            {
                return new SynergyResult
                {
                    type = SynergyType.KeyCollector,
                    descriptionKey = "synergy_key_collector",
                    bonusAttack = 5,
                    bonusDefense = 5,
                    bonusHP = 10
                };
            }

            return null;
        }

        /// <summary>
        /// Elemental Combo synergy: if inventory contains items whose IDs reference
        /// both "fire" and "ice" elements, trigger a special elemental effect.
        /// </summary>
        /// <param name="inventory">The player's current inventory.</param>
        /// <returns>The synergy result if triggered, or null.</returns>
        public static SynergyResult? CheckElementalCombo(List<ItemData> inventory)
        {
            bool hasFire = inventory.Any(i => i.id.Contains("fire"));
            bool hasIce = inventory.Any(i => i.id.Contains("ice"));

            if (hasFire && hasIce)
            {
                return new SynergyResult
                {
                    type = SynergyType.ElementalCombo,
                    descriptionKey = "synergy_elemental_combo",
                    bonusAttack = 20,
                    bonusDefense = 10,
                    bonusHP = 0
                };
            }

            return null;
        }

        /// <summary>
        /// Check for synergies defined directly on items via their synergyItemIds arrays.
        /// These are custom item-to-item synergies defined in the data.
        /// </summary>
        /// <param name="inventory">The player's current inventory.</param>
        /// <returns>A list of item-defined synergy results.</returns>
        private static List<SynergyResult> CheckItemDefinedSynergies(List<ItemData> inventory)
        {
            var results = new List<SynergyResult>();
            var reportedPairs = new HashSet<string>();
            var inventoryIds = new HashSet<string>(inventory.Select(i => i.id));

            foreach (var item in inventory)
            {
                if (item.synergyItemIds == null || item.synergyItemIds.Length == 0)
                    continue;

                foreach (var partnerId in item.synergyItemIds)
                {
                    if (!inventoryIds.Contains(partnerId))
                        continue;

                    // Canonical pair key to avoid duplicates
                    string pairKey = string.Compare(item.id, partnerId, StringComparison.Ordinal) < 0
                        ? item.id + "|" + partnerId
                        : partnerId + "|" + item.id;

                    if (reportedPairs.Contains(pairKey))
                        continue;

                    reportedPairs.Add(pairKey);

                    results.Add(new SynergyResult
                    {
                        type = SynergyType.ArmorSet, // Generic item-defined synergy uses ArmorSet as placeholder
                        descriptionKey = "synergy_item_combo_" + pairKey.Replace("|", "_"),
                        bonusAttack = 5,
                        bonusDefense = 5,
                        bonusHP = 5
                    });
                }
            }

            return results;
        }
    }
}
