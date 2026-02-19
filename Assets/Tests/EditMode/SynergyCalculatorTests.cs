using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using LootOrLose.Core.Items;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.Tests.EditMode
{
    [TestFixture]
    public class SynergyCalculatorTests
    {
        // ─────────────────────────────────────────────
        //  ArmorSet Synergy (shield + armor + helmet)
        // ─────────────────────────────────────────────

        [Test]
        public void CheckArmorSet_WithShieldArmorHelmet_ReturnsSynergy()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "iron_shield", category = ItemCategory.Defense },
                new ItemData { id = "iron_armor", category = ItemCategory.Defense },
                new ItemData { id = "iron_helmet", category = ItemCategory.Defense }
            };

            var result = SynergyCalculator.CheckArmorSet(inventory);

            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(SynergyType.ArmorSet, result.Value.type);
            Assert.AreEqual(15, result.Value.bonusDefense);
            Assert.AreEqual(20, result.Value.bonusHP);
        }

        [Test]
        public void CheckArmorSet_MissingHelmet_ReturnsNull()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "iron_shield", category = ItemCategory.Defense },
                new ItemData { id = "iron_armor", category = ItemCategory.Defense }
            };

            var result = SynergyCalculator.CheckArmorSet(inventory);

            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void CheckArmorSet_NonDefenseItems_ReturnsNull()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "shield_sword", category = ItemCategory.Weapon },
                new ItemData { id = "armor_potion", category = ItemCategory.Consumable },
                new ItemData { id = "helmet_key", category = ItemCategory.Key }
            };

            var result = SynergyCalculator.CheckArmorSet(inventory);

            Assert.IsFalse(result.HasValue);
        }

        // ─────────────────────────────────────────────
        //  DualWield Synergy (2+ weapons)
        // ─────────────────────────────────────────────

        [Test]
        public void CheckDualWield_TwoWeapons_ReturnsSynergy()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon },
                new ItemData { id = "axe", category = ItemCategory.Weapon }
            };

            var result = SynergyCalculator.CheckDualWield(inventory);

            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(SynergyType.DualWield, result.Value.type);
            Assert.AreEqual(20, result.Value.bonusAttack); // 10 * 2 weapons
        }

        [Test]
        public void CheckDualWield_ThreeWeapons_ScalesBonusAttack()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon },
                new ItemData { id = "axe", category = ItemCategory.Weapon },
                new ItemData { id = "dagger", category = ItemCategory.Weapon }
            };

            var result = SynergyCalculator.CheckDualWield(inventory);

            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(30, result.Value.bonusAttack); // 10 * 3 weapons
        }

        [Test]
        public void CheckDualWield_OneWeapon_ReturnsNull()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon },
                new ItemData { id = "shield", category = ItemCategory.Defense }
            };

            var result = SynergyCalculator.CheckDualWield(inventory);

            Assert.IsFalse(result.HasValue);
        }

        // ─────────────────────────────────────────────
        //  PotionMaster Synergy (3+ consumables)
        // ─────────────────────────────────────────────

        [Test]
        public void CheckPotionMaster_ThreeConsumables_ReturnsSynergy()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "health_potion", category = ItemCategory.Consumable },
                new ItemData { id = "mana_potion", category = ItemCategory.Consumable },
                new ItemData { id = "speed_potion", category = ItemCategory.Consumable }
            };

            var result = SynergyCalculator.CheckPotionMaster(inventory);

            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(SynergyType.PotionMaster, result.Value.type);
            Assert.AreEqual(45, result.Value.bonusHP); // 15 * 3 consumables
            Assert.AreEqual(5, result.Value.bonusDefense);
        }

        [Test]
        public void CheckPotionMaster_TwoConsumables_ReturnsNull()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "health_potion", category = ItemCategory.Consumable },
                new ItemData { id = "mana_potion", category = ItemCategory.Consumable }
            };

            var result = SynergyCalculator.CheckPotionMaster(inventory);

            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void CheckPotionMaster_FourConsumables_ScalesBonusHP()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "potion_a", category = ItemCategory.Consumable },
                new ItemData { id = "potion_b", category = ItemCategory.Consumable },
                new ItemData { id = "potion_c", category = ItemCategory.Consumable },
                new ItemData { id = "potion_d", category = ItemCategory.Consumable }
            };

            var result = SynergyCalculator.CheckPotionMaster(inventory);

            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(60, result.Value.bonusHP); // 15 * 4 consumables
        }

        // ─────────────────────────────────────────────
        //  KeyCollector Synergy (3+ keys)
        // ─────────────────────────────────────────────

        [Test]
        public void CheckKeyCollector_ThreeKeys_ReturnsSynergy()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "key_bronze", category = ItemCategory.Key },
                new ItemData { id = "key_silver", category = ItemCategory.Key },
                new ItemData { id = "key_gold", category = ItemCategory.Key }
            };

            var result = SynergyCalculator.CheckKeyCollector(inventory);

            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(SynergyType.KeyCollector, result.Value.type);
            Assert.AreEqual(5, result.Value.bonusAttack);
            Assert.AreEqual(5, result.Value.bonusDefense);
            Assert.AreEqual(10, result.Value.bonusHP);
        }

        [Test]
        public void CheckKeyCollector_TwoKeys_ReturnsNull()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "key_bronze", category = ItemCategory.Key },
                new ItemData { id = "key_silver", category = ItemCategory.Key }
            };

            var result = SynergyCalculator.CheckKeyCollector(inventory);

            Assert.IsFalse(result.HasValue);
        }

        // ─────────────────────────────────────────────
        //  No Synergy with Unrelated Items
        // ─────────────────────────────────────────────

        [Test]
        public void CheckSynergies_UnrelatedItems_ReturnsEmptyList()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon },
                new ItemData { id = "shield", category = ItemCategory.Defense },
                new ItemData { id = "key_bronze", category = ItemCategory.Key }
            };

            var results = SynergyCalculator.CheckSynergies(inventory);

            // No synergy should trigger: 1 weapon (not 2), 1 defense (not full set), 1 key (not 3)
            Assert.AreEqual(0, results.Count);
        }

        // ─────────────────────────────────────────────
        //  Anti-Synergy Detection
        // ─────────────────────────────────────────────

        [Test]
        public void CheckAntiSynergies_ConflictingItems_DetectsConflict()
        {
            var inventory = new List<ItemData>
            {
                new ItemData
                {
                    id = "fire_oil",
                    category = ItemCategory.Consumable,
                    antiSynergyItemIds = new[] { "oil_flask" }
                },
                new ItemData
                {
                    id = "oil_flask",
                    category = ItemCategory.Consumable,
                    antiSynergyItemIds = new[] { "fire_oil" }
                }
            };

            var results = SynergyCalculator.CheckAntiSynergies(inventory);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(10, results[0].damage);
            Assert.IsFalse(results[0].isFatal);
        }

        [Test]
        public void CheckAntiSynergies_BothCursed_IsFatal()
        {
            var inventory = new List<ItemData>
            {
                new ItemData
                {
                    id = "cursed_blade",
                    category = ItemCategory.Weapon,
                    isCursed = true,
                    antiSynergyItemIds = new[] { "cursed_shield" }
                },
                new ItemData
                {
                    id = "cursed_shield",
                    category = ItemCategory.Defense,
                    isCursed = true,
                    antiSynergyItemIds = new[] { "cursed_blade" }
                }
            };

            var results = SynergyCalculator.CheckAntiSynergies(inventory);

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].isFatal);
            Assert.AreEqual(999, results[0].damage);
        }

        [Test]
        public void CheckAntiSynergies_NoDuplicatePairs()
        {
            // Both items reference each other - should only report one conflict
            var inventory = new List<ItemData>
            {
                new ItemData
                {
                    id = "item_a",
                    category = ItemCategory.Weapon,
                    antiSynergyItemIds = new[] { "item_b" }
                },
                new ItemData
                {
                    id = "item_b",
                    category = ItemCategory.Weapon,
                    antiSynergyItemIds = new[] { "item_a" }
                }
            };

            var results = SynergyCalculator.CheckAntiSynergies(inventory);

            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void CheckAntiSynergies_NoConflicts_ReturnsEmptyList()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, antiSynergyItemIds = null },
                new ItemData { id = "shield", category = ItemCategory.Defense, antiSynergyItemIds = new string[0] }
            };

            var results = SynergyCalculator.CheckAntiSynergies(inventory);

            Assert.AreEqual(0, results.Count);
        }

        // ─────────────────────────────────────────────
        //  Empty Inventory
        // ─────────────────────────────────────────────

        [Test]
        public void CheckSynergies_EmptyInventory_ReturnsEmptyList()
        {
            var results = SynergyCalculator.CheckSynergies(new List<ItemData>());
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void CheckSynergies_NullInventory_ReturnsEmptyList()
        {
            var results = SynergyCalculator.CheckSynergies(null);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void CheckAntiSynergies_EmptyInventory_ReturnsEmptyList()
        {
            var results = SynergyCalculator.CheckAntiSynergies(new List<ItemData>());
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void CheckAntiSynergies_NullInventory_ReturnsEmptyList()
        {
            var results = SynergyCalculator.CheckAntiSynergies(null);
            Assert.AreEqual(0, results.Count);
        }

        // ─────────────────────────────────────────────
        //  Single Item (no synergies possible)
        // ─────────────────────────────────────────────

        [Test]
        public void CheckSynergies_SingleWeapon_NoSynergies()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon }
            };

            var results = SynergyCalculator.CheckSynergies(inventory);

            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void CheckSynergies_SingleDefenseItem_NoSynergies()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "iron_shield", category = ItemCategory.Defense }
            };

            var results = SynergyCalculator.CheckSynergies(inventory);

            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void CheckSynergies_SingleConsumable_NoSynergies()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "health_potion", category = ItemCategory.Consumable }
            };

            var results = SynergyCalculator.CheckSynergies(inventory);

            Assert.AreEqual(0, results.Count);
        }

        // ─────────────────────────────────────────────
        //  Combined Synergy Totals
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateTotalSynergyBonus_MultipleSynergies_SumsCorrectly()
        {
            var inventory = new List<ItemData>
            {
                // DualWield: 2 weapons
                new ItemData { id = "sword", category = ItemCategory.Weapon },
                new ItemData { id = "axe", category = ItemCategory.Weapon },
                // PotionMaster: 3 consumables
                new ItemData { id = "potion_a", category = ItemCategory.Consumable },
                new ItemData { id = "potion_b", category = ItemCategory.Consumable },
                new ItemData { id = "potion_c", category = ItemCategory.Consumable }
            };

            var totals = SynergyCalculator.CalculateTotalSynergyBonus(inventory);

            // DualWield: bonusAttack = 10 * 2 = 20
            // PotionMaster: bonusDefense = 5, bonusHP = 15 * 3 = 45
            Assert.AreEqual(20, totals.totalBonusAttack);
            Assert.AreEqual(5, totals.totalBonusDefense);
            Assert.AreEqual(45, totals.totalBonusHP);
            Assert.AreEqual(0, totals.totalAntiSynergyDamage);
            Assert.IsFalse(totals.hasFatalAntiSynergy);
        }

        [Test]
        public void CalculateTotalSynergyBonus_WithAntiSynergy_IncludesDamage()
        {
            var inventory = new List<ItemData>
            {
                new ItemData
                {
                    id = "fire_oil",
                    category = ItemCategory.Consumable,
                    antiSynergyItemIds = new[] { "oil_flask" }
                },
                new ItemData
                {
                    id = "oil_flask",
                    category = ItemCategory.Consumable,
                    antiSynergyItemIds = new[] { "fire_oil" }
                }
            };

            var totals = SynergyCalculator.CalculateTotalSynergyBonus(inventory);

            Assert.AreEqual(10, totals.totalAntiSynergyDamage);
            Assert.IsFalse(totals.hasFatalAntiSynergy);
        }
    }
}
