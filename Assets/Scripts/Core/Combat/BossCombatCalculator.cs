using System;
using System.Collections.Generic;
using System.Linq;
using LootOrLose.Core.Items;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.Core.Combat
{
    // ─────────────────────────────────────────────
    //  Result Struct
    // ─────────────────────────────────────────────

    /// <summary>
    /// The outcome of an auto-resolved boss combat encounter.
    /// Contains damage dealt and taken, final HP values, and a localized combat log.
    /// </summary>
    public struct BossCombatResult
    {
        /// <summary>Whether the player won the fight.</summary>
        public bool playerWon;

        /// <summary>Total damage dealt by the player to the boss.</summary>
        public int damageDealt;

        /// <summary>Total damage taken by the player from the boss.</summary>
        public int damageTaken;

        /// <summary>The boss's remaining HP after combat (0 if defeated).</summary>
        public int bossRemainingHP;

        /// <summary>
        /// Ordered sequence of i18n keys describing each phase of combat.
        /// The UI layer resolves these keys to display a combat narrative.
        /// </summary>
        public string[] combatLog;
    }

    // ─────────────────────────────────────────────
    //  Calculator
    // ─────────────────────────────────────────────

    /// <summary>
    /// Calculates the outcome of auto-resolved boss combat encounters.
    /// Evaluates player inventory power, synergy bonuses, boss weaknesses/resistances,
    /// and HP scaling to determine the fight result.
    /// Pure logic - no Unity dependencies.
    /// </summary>
    public static class BossCombatCalculator
    {
        /// <summary>
        /// Calculate the full combat outcome between the player's inventory and a boss.
        /// </summary>
        /// <param name="boss">The boss data definition being fought.</param>
        /// <param name="inventory">The player's current inventory of items.</param>
        /// <param name="synergies">Active synergy bonuses from the player's inventory.</param>
        /// <param name="round">The current round number, used for boss HP scaling.</param>
        /// <returns>A <see cref="BossCombatResult"/> describing the full combat outcome.</returns>
        public static BossCombatResult CalculateCombat(
            BossData boss,
            List<ItemData> inventory,
            List<SynergyResult> synergies,
            int round)
        {
            var log = new List<string>();

            // --- Boss HP Scaling ---
            int bossHP = CalculateBossHP(boss, round);
            log.Add("combat_boss_appears"); // "{bossName} appears with {bossHP} HP!"

            // --- Player Attack Calculation ---
            int baseAttack = CalculateBaseAttack(inventory);
            int synergyAttackBonus = synergies != null ? synergies.Sum(s => s.bonusAttack) : 0;
            int totalAttack = baseAttack + synergyAttackBonus;

            log.Add("combat_player_attack_base"); // "Your weapons deal {baseAttack} base damage."

            if (synergyAttackBonus > 0)
                log.Add("combat_synergy_attack_bonus"); // "Synergies grant +{synergyAttackBonus} attack!"

            // --- Weakness/Resistance Modifiers ---
            float weaknessMultiplier = CalculateWeaknessMultiplier(inventory, boss);
            float resistanceMultiplier = CalculateResistanceMultiplier(inventory, boss);

            int weaknessDamage = (int)(totalAttack * weaknessMultiplier);
            int resistedDamage = (int)(totalAttack * resistanceMultiplier);
            int effectivePlayerDamage = weaknessDamage - (totalAttack - resistedDamage);

            // Ensure effective damage is not negative
            if (effectivePlayerDamage < 0)
                effectivePlayerDamage = 0;

            // Simpler: apply weakness bonus to matching items, resistance penalty to matching items
            effectivePlayerDamage = CalculateEffectiveDamage(inventory, boss, totalAttack);

            if (weaknessMultiplier > 1.0f)
                log.Add("combat_weakness_exploited"); // "You exploit the boss's weakness! Bonus damage!"

            if (resistanceMultiplier < 1.0f)
                log.Add("combat_resistance_active"); // "The boss resists some of your attacks."

            // --- Boss Attack Calculation ---
            int bossAttack = CalculateBossAttack(bossHP);

            // --- Player Defense Calculation ---
            int baseDefense = CalculateBaseDefense(inventory);
            int synergyDefenseBonus = synergies != null ? synergies.Sum(s => s.bonusDefense) : 0;
            int totalDefense = baseDefense + synergyDefenseBonus;

            if (synergyDefenseBonus > 0)
                log.Add("combat_synergy_defense_bonus"); // "Synergies grant +{synergyDefenseBonus} defense!"

            // --- Damage Taken ---
            int damageTaken = Math.Max(bossAttack - totalDefense, 0);

            log.Add("combat_boss_attacks"); // "The boss attacks for {bossAttack} damage!"

            if (totalDefense > 0)
                log.Add("combat_defense_reduces"); // "Your armor absorbs {totalDefense} damage."

            // --- Resolve Combat ---
            int bossRemainingHP = Math.Max(bossHP - effectivePlayerDamage, 0);
            bool playerWon = bossRemainingHP <= 0;

            if (playerWon)
            {
                log.Add("combat_boss_defeated"); // "You defeated {bossName}!"
                log.Add("combat_victory"); // "Victory! The loot is yours."
            }
            else
            {
                log.Add("combat_boss_survives"); // "{bossName} survives with {bossRemainingHP} HP!"
                log.Add("combat_defeat"); // "You were overwhelmed..."
            }

            return new BossCombatResult
            {
                playerWon = playerWon,
                damageDealt = effectivePlayerDamage,
                damageTaken = damageTaken,
                bossRemainingHP = bossRemainingHP,
                combatLog = log.ToArray()
            };
        }

        /// <summary>
        /// Calculate the boss's total HP for the current round.
        /// HP = baseHP + (round * scalingPerLevel).
        /// </summary>
        /// <param name="boss">The boss data definition.</param>
        /// <param name="round">The current round number.</param>
        /// <returns>The boss's effective HP for this encounter.</returns>
        public static int CalculateBossHP(BossData boss, int round)
        {
            return boss.baseHP + (round * boss.scalingPerLevel);
        }

        /// <summary>
        /// Calculate the boss's attack power.
        /// Boss attack = total boss HP / 10.
        /// </summary>
        /// <param name="bossHP">The boss's total effective HP.</param>
        /// <returns>The boss's attack power.</returns>
        public static int CalculateBossAttack(int bossHP)
        {
            return Math.Max(bossHP / 10, 1);
        }

        /// <summary>
        /// Calculate the player's base attack power from inventory items.
        /// Sums the attackPower of all non-cursed items. Cursed items contribute half.
        /// </summary>
        /// <param name="inventory">The player's inventory.</param>
        /// <returns>Total base attack power.</returns>
        public static int CalculateBaseAttack(List<ItemData> inventory)
        {
            if (inventory == null || inventory.Count == 0)
                return 0;

            int total = 0;
            foreach (var item in inventory)
            {
                if (item.isCursed)
                    total += item.attackPower / 2;
                else
                    total += item.attackPower;
            }
            return total;
        }

        /// <summary>
        /// Calculate the player's base defense power from inventory items.
        /// Sums the defensePower of all non-cursed items. Cursed items contribute half.
        /// </summary>
        /// <param name="inventory">The player's inventory.</param>
        /// <returns>Total base defense power.</returns>
        public static int CalculateBaseDefense(List<ItemData> inventory)
        {
            if (inventory == null || inventory.Count == 0)
                return 0;

            int total = 0;
            foreach (var item in inventory)
            {
                if (item.isCursed)
                    total += item.defensePower / 2;
                else
                    total += item.defensePower;
            }
            return total;
        }

        /// <summary>
        /// Calculate the effective damage dealt by the player, accounting for
        /// boss weakness and resistance multipliers applied per-item.
        /// Items matching the boss weakness deal 1.5x damage.
        /// Items matching the boss resistance deal 0.5x damage.
        /// </summary>
        /// <param name="inventory">The player's inventory.</param>
        /// <param name="boss">The boss being fought.</param>
        /// <param name="totalAttackWithSynergies">Total attack including synergy bonuses.</param>
        /// <returns>Effective damage after weakness/resistance adjustments.</returns>
        public static int CalculateEffectiveDamage(
            List<ItemData> inventory,
            BossData boss,
            int totalAttackWithSynergies)
        {
            if (inventory == null || inventory.Count == 0)
                return 0;

            // Calculate per-item contributions with weakness/resistance applied
            float itemDamage = 0f;
            foreach (var item in inventory)
            {
                float attack = item.isCursed ? item.attackPower / 2f : item.attackPower;

                if (item.category == boss.weakness)
                    attack *= 1.5f;
                else if (item.category == boss.resistance)
                    attack *= 0.5f;

                itemDamage += attack;
            }

            // Synergy bonus is added at full value (not affected by weakness/resistance)
            float synergyBonus = totalAttackWithSynergies - CalculateBaseAttack(inventory);

            return (int)(itemDamage + synergyBonus);
        }

        /// <summary>
        /// Calculate the weakness multiplier. Returns > 1.0 if any inventory items
        /// match the boss's weakness category.
        /// </summary>
        private static float CalculateWeaknessMultiplier(List<ItemData> inventory, BossData boss)
        {
            if (inventory == null) return 1.0f;
            bool hasWeaknessItem = inventory.Any(i => i.category == boss.weakness);
            return hasWeaknessItem ? 1.5f : 1.0f;
        }

        /// <summary>
        /// Calculate the resistance multiplier. Returns < 1.0 if any inventory items
        /// match the boss's resistance category.
        /// </summary>
        private static float CalculateResistanceMultiplier(List<ItemData> inventory, BossData boss)
        {
            if (inventory == null) return 1.0f;
            bool hasResistanceItem = inventory.Any(i => i.category == boss.resistance);
            return hasResistanceItem ? 0.5f : 1.0f;
        }
    }
}
