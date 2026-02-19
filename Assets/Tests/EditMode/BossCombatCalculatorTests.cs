using System;
using System.Collections.Generic;
using NUnit.Framework;
using LootOrLose.Core.Combat;
using LootOrLose.Core.Items;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.Tests.EditMode
{
    [TestFixture]
    public class BossCombatCalculatorTests
    {
        private BossData testBoss;

        [SetUp]
        public void SetUp()
        {
            testBoss = new BossData
            {
                id = "skeleton_king",
                type = BossType.SkeletonKing,
                baseHP = 100,
                scalingPerLevel = 5,
                weakness = ItemCategory.Weapon,
                resistance = ItemCategory.Consumable,
                preferredBiome = BiomeType.Crypt
            };
        }

        // ─────────────────────────────────────────────
        //  Basic Combat (player damage vs boss HP)
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateCombat_BasicDamage_DealsDamageBasedOnInventory()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, attackPower = 50 }
            };
            var synergies = new List<SynergyResult>();

            var result = BossCombatCalculator.CalculateCombat(testBoss, inventory, synergies, 15);

            Assert.Greater(result.damageDealt, 0);
        }

        [Test]
        public void CalculateBaseAttack_SumsAllItemAttackPower()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", attackPower = 20 },
                new ItemData { id = "axe", attackPower = 15 },
                new ItemData { id = "dagger", attackPower = 10 }
            };

            int attack = BossCombatCalculator.CalculateBaseAttack(inventory);

            Assert.AreEqual(45, attack);
        }

        [Test]
        public void CalculateBaseAttack_CursedItems_ContributeHalf()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "cursed_sword", attackPower = 20, isCursed = true },
                new ItemData { id = "normal_axe", attackPower = 10, isCursed = false }
            };

            int attack = BossCombatCalculator.CalculateBaseAttack(inventory);

            // Cursed: 20/2 = 10, Normal: 10 => Total: 20
            Assert.AreEqual(20, attack);
        }

        // ─────────────────────────────────────────────
        //  Weakness Multiplier (1.5x damage)
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateEffectiveDamage_WeaknessItems_Deal1Point5xDamage()
        {
            // Boss weakness = Weapon
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, attackPower = 20 }
            };

            int baseAttack = BossCombatCalculator.CalculateBaseAttack(inventory);
            int effective = BossCombatCalculator.CalculateEffectiveDamage(inventory, testBoss, baseAttack);

            // Weapon matches weakness: 20 * 1.5 = 30
            Assert.AreEqual(30, effective);
        }

        [Test]
        public void CalculateEffectiveDamage_MixedWeaknessAndNormal_CorrectDamage()
        {
            // Boss weakness = Weapon
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, attackPower = 20 },
                new ItemData { id = "shield", category = ItemCategory.Defense, attackPower = 5 }
            };

            int baseAttack = BossCombatCalculator.CalculateBaseAttack(inventory);
            int effective = BossCombatCalculator.CalculateEffectiveDamage(inventory, testBoss, baseAttack);

            // Weapon: 20 * 1.5 = 30, Defense: 5 * 1.0 = 5 => Total: 35
            Assert.AreEqual(35, effective);
        }

        // ─────────────────────────────────────────────
        //  Resistance Multiplier (0.5x damage)
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateEffectiveDamage_ResistanceItems_Deal0Point5xDamage()
        {
            // Boss resistance = Consumable
            var inventory = new List<ItemData>
            {
                new ItemData { id = "potion", category = ItemCategory.Consumable, attackPower = 10 }
            };

            int baseAttack = BossCombatCalculator.CalculateBaseAttack(inventory);
            int effective = BossCombatCalculator.CalculateEffectiveDamage(inventory, testBoss, baseAttack);

            // Consumable matches resistance: 10 * 0.5 = 5
            Assert.AreEqual(5, effective);
        }

        [Test]
        public void CalculateEffectiveDamage_MixedResistanceAndWeakness_CorrectDamage()
        {
            // Boss weakness = Weapon, resistance = Consumable
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, attackPower = 20 },
                new ItemData { id = "potion", category = ItemCategory.Consumable, attackPower = 10 }
            };

            int baseAttack = BossCombatCalculator.CalculateBaseAttack(inventory);
            int effective = BossCombatCalculator.CalculateEffectiveDamage(inventory, testBoss, baseAttack);

            // Weapon: 20 * 1.5 = 30, Consumable: 10 * 0.5 = 5 => Total: 35
            Assert.AreEqual(35, effective);
        }

        // ─────────────────────────────────────────────
        //  Boss HP Scaling by Round
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateBossHP_ScalesWithRound()
        {
            // baseHP=100, scalingPerLevel=5
            Assert.AreEqual(175, BossCombatCalculator.CalculateBossHP(testBoss, 15));
            Assert.AreEqual(250, BossCombatCalculator.CalculateBossHP(testBoss, 30));
            Assert.AreEqual(325, BossCombatCalculator.CalculateBossHP(testBoss, 45));
        }

        [Test]
        public void CalculateBossHP_Round1_ReturnsBaseHPPlusScaling()
        {
            Assert.AreEqual(105, BossCombatCalculator.CalculateBossHP(testBoss, 1));
        }

        [Test]
        public void CalculateBossHP_Round0_ReturnsBaseHP()
        {
            Assert.AreEqual(100, BossCombatCalculator.CalculateBossHP(testBoss, 0));
        }

        // ─────────────────────────────────────────────
        //  Boss Attack Calculation
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateBossAttack_IsTenthOfHP()
        {
            Assert.AreEqual(17, BossCombatCalculator.CalculateBossAttack(175));
            Assert.AreEqual(25, BossCombatCalculator.CalculateBossAttack(250));
        }

        [Test]
        public void CalculateBossAttack_MinimumIs1()
        {
            Assert.AreEqual(1, BossCombatCalculator.CalculateBossAttack(0));
            Assert.AreEqual(1, BossCombatCalculator.CalculateBossAttack(5));
        }

        // ─────────────────────────────────────────────
        //  Player Defense Reducing Damage Taken
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateBaseDefense_SumsAllItemDefensePower()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "shield", defensePower = 10 },
                new ItemData { id = "armor", defensePower = 15 },
                new ItemData { id = "helmet", defensePower = 5 }
            };

            int defense = BossCombatCalculator.CalculateBaseDefense(inventory);

            Assert.AreEqual(30, defense);
        }

        [Test]
        public void CalculateBaseDefense_CursedItems_ContributeHalf()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "cursed_shield", defensePower = 20, isCursed = true },
                new ItemData { id = "normal_armor", defensePower = 10, isCursed = false }
            };

            int defense = BossCombatCalculator.CalculateBaseDefense(inventory);

            // Cursed: 20/2 = 10, Normal: 10 => Total: 20
            Assert.AreEqual(20, defense);
        }

        [Test]
        public void CalculateCombat_DefenseReducesDamageTaken()
        {
            var inventoryNoDefense = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, attackPower = 200, defensePower = 0 }
            };
            var inventoryWithDefense = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, attackPower = 200, defensePower = 0 },
                new ItemData { id = "shield", category = ItemCategory.Defense, attackPower = 0, defensePower = 50 }
            };
            var synergies = new List<SynergyResult>();

            var resultNoDefense = BossCombatCalculator.CalculateCombat(testBoss, inventoryNoDefense, synergies, 15);
            var resultWithDefense = BossCombatCalculator.CalculateCombat(testBoss, inventoryWithDefense, synergies, 15);

            Assert.LessOrEqual(resultWithDefense.damageTaken, resultNoDefense.damageTaken);
        }

        // ─────────────────────────────────────────────
        //  Win Condition (damage > boss HP)
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateCombat_WinCondition_PlayerWinsWhenDamageExceedsBossHP()
        {
            // Boss at round 15: HP = 100 + (15*5) = 175
            var inventory = new List<ItemData>
            {
                new ItemData { id = "mega_sword", category = ItemCategory.Weapon, attackPower = 200 }
            };
            var synergies = new List<SynergyResult>();

            var result = BossCombatCalculator.CalculateCombat(testBoss, inventory, synergies, 15);

            Assert.IsTrue(result.playerWon);
            Assert.AreEqual(0, result.bossRemainingHP);
        }

        [Test]
        public void CalculateCombat_WinCondition_CombatLogContainsVictory()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "mega_sword", category = ItemCategory.Weapon, attackPower = 500 }
            };
            var synergies = new List<SynergyResult>();

            var result = BossCombatCalculator.CalculateCombat(testBoss, inventory, synergies, 15);

            Assert.IsTrue(result.playerWon);
            Assert.Contains("combat_boss_defeated", result.combatLog);
            Assert.Contains("combat_victory", result.combatLog);
        }

        // ─────────────────────────────────────────────
        //  Lose Condition (damage < boss HP)
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateCombat_LoseCondition_PlayerLosesWhenDamageBelowBossHP()
        {
            // Boss at round 15: HP = 175, player attack = 5 (way too low)
            var inventory = new List<ItemData>
            {
                new ItemData { id = "twig", category = ItemCategory.Weapon, attackPower = 5 }
            };
            var synergies = new List<SynergyResult>();

            var result = BossCombatCalculator.CalculateCombat(testBoss, inventory, synergies, 15);

            Assert.IsFalse(result.playerWon);
            Assert.Greater(result.bossRemainingHP, 0);
        }

        [Test]
        public void CalculateCombat_LoseCondition_CombatLogContainsDefeat()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "twig", category = ItemCategory.Weapon, attackPower = 1 }
            };
            var synergies = new List<SynergyResult>();

            var result = BossCombatCalculator.CalculateCombat(testBoss, inventory, synergies, 15);

            Assert.IsFalse(result.playerWon);
            Assert.Contains("combat_boss_survives", result.combatLog);
            Assert.Contains("combat_defeat", result.combatLog);
        }

        // ─────────────────────────────────────────────
        //  Empty Inventory (0 damage, auto-lose)
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateBaseAttack_EmptyInventory_ReturnsZero()
        {
            Assert.AreEqual(0, BossCombatCalculator.CalculateBaseAttack(new List<ItemData>()));
        }

        [Test]
        public void CalculateBaseAttack_NullInventory_ReturnsZero()
        {
            Assert.AreEqual(0, BossCombatCalculator.CalculateBaseAttack(null));
        }

        [Test]
        public void CalculateBaseDefense_EmptyInventory_ReturnsZero()
        {
            Assert.AreEqual(0, BossCombatCalculator.CalculateBaseDefense(new List<ItemData>()));
        }

        [Test]
        public void CalculateBaseDefense_NullInventory_ReturnsZero()
        {
            Assert.AreEqual(0, BossCombatCalculator.CalculateBaseDefense(null));
        }

        [Test]
        public void CalculateEffectiveDamage_EmptyInventory_ReturnsZero()
        {
            var inventory = new List<ItemData>();
            int effective = BossCombatCalculator.CalculateEffectiveDamage(inventory, testBoss, 0);
            Assert.AreEqual(0, effective);
        }

        [Test]
        public void CalculateCombat_EmptyInventory_AutoLose()
        {
            var inventory = new List<ItemData>();
            var synergies = new List<SynergyResult>();

            var result = BossCombatCalculator.CalculateCombat(testBoss, inventory, synergies, 15);

            Assert.IsFalse(result.playerWon);
            Assert.AreEqual(0, result.damageDealt);
            Assert.Greater(result.bossRemainingHP, 0);
        }

        // ─────────────────────────────────────────────
        //  Synergy Bonus in Combat
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateCombat_SynergyBonus_IncreasesTotalDamage()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, attackPower = 50 },
                new ItemData { id = "axe", category = ItemCategory.Weapon, attackPower = 50 }
            };
            var noSynergies = new List<SynergyResult>();
            var withSynergies = new List<SynergyResult>
            {
                new SynergyResult { type = SynergyType.DualWield, bonusAttack = 20, bonusDefense = 0, bonusHP = 0 }
            };

            var resultNo = BossCombatCalculator.CalculateCombat(testBoss, inventory, noSynergies, 15);
            var resultWith = BossCombatCalculator.CalculateCombat(testBoss, inventory, withSynergies, 15);

            Assert.Greater(resultWith.damageDealt, resultNo.damageDealt);
        }

        // ─────────────────────────────────────────────
        //  Combat Log Always Present
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateCombat_AlwaysReturnsCombatLog()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, attackPower = 10 }
            };
            var synergies = new List<SynergyResult>();

            var result = BossCombatCalculator.CalculateCombat(testBoss, inventory, synergies, 15);

            Assert.IsNotNull(result.combatLog);
            Assert.Greater(result.combatLog.Length, 0);
            Assert.Contains("combat_boss_appears", result.combatLog);
            Assert.Contains("combat_player_attack_base", result.combatLog);
        }
    }
}
