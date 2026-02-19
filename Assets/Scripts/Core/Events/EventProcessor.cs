using System;
using System.Collections.Generic;
using System.Linq;
using LootOrLose.Data;
using LootOrLose.Enums;
using LootOrLose.State;

namespace LootOrLose.Core.Events
{
    // ─────────────────────────────────────────────
    //  Result Struct
    // ─────────────────────────────────────────────

    /// <summary>
    /// The outcome of processing a game event, including items gained/lost,
    /// stat changes, and a localized message describing what happened.
    /// </summary>
    public struct EventOutcome
    {
        /// <summary>Whether the event resolved successfully for the player.</summary>
        public bool success;

        /// <summary>Localization key for the event outcome message.</summary>
        public string messageKey;

        /// <summary>Items gained by the player as a result of this event.</summary>
        public List<ItemData> itemsGained;

        /// <summary>IDs of items lost (removed from inventory) as a result of this event.</summary>
        public List<string> itemsLostIds;

        /// <summary>Change to the player's HP (positive = heal, negative = damage).</summary>
        public int hpChange;

        /// <summary>Change to the player's gold (positive = gain, negative = spend).</summary>
        public int goldChange;

        /// <summary>Temporary buff gained from this event, if any.</summary>
        public BuffType? buffGained;
    }

    // ─────────────────────────────────────────────
    //  Processor
    // ─────────────────────────────────────────────

    /// <summary>
    /// Processes random game events that occur between rounds.
    /// Each event type has unique logic affecting the player's inventory, HP, gold, or buffs.
    /// Pure logic - no Unity dependencies.
    /// </summary>
    public static class EventProcessor
    {
        /// <summary>
        /// Process a game event against the current run state.
        /// Dispatches to the appropriate handler based on event type.
        /// </summary>
        /// <param name="eventData">The event definition to process.</param>
        /// <param name="state">The current game run state (inventory, HP, gold, etc.).</param>
        /// <param name="random">Seeded random instance for deterministic outcomes.</param>
        /// <returns>An <see cref="EventOutcome"/> describing the result of the event.</returns>
        public static EventOutcome ProcessEvent(EventData eventData, GameRunState state, Random random)
        {
            return ProcessEvent(eventData, state, random, null);
        }

        /// <summary>
        /// Process a game event against the current run state, with an optional item pool
        /// for events that generate new items (Merchant, Chest, WheelOfFortune).
        /// </summary>
        /// <param name="eventData">The event definition to process.</param>
        /// <param name="state">The current game run state.</param>
        /// <param name="random">Seeded random instance.</param>
        /// <param name="itemPool">Optional pool of items for generation events.</param>
        /// <returns>An <see cref="EventOutcome"/> describing the result.</returns>
        public static EventOutcome ProcessEvent(
            EventData eventData,
            GameRunState state,
            Random random,
            List<ItemData> itemPool)
        {
            if (eventData == null)
                return CreateOutcome(false, "event_error_null");

            switch (eventData.type)
            {
                case EventType.Merchant:
                    return ProcessMerchant(state, random, itemPool);
                case EventType.Altar:
                    return ProcessAltar(state, random);
                case EventType.Chest:
                    return ProcessChest(state, random, itemPool);
                case EventType.Curse:
                    return ProcessCurse(state, random);
                case EventType.WheelOfFortune:
                    return ProcessWheelOfFortune(state, random, itemPool);
                case EventType.Healer:
                    return ProcessHealer(state);
                case EventType.Blacksmith:
                    return ProcessBlacksmith(state, random);
                case EventType.Trap:
                    return ProcessTrap(state, random);
                default:
                    return CreateOutcome(false, "event_error_unknown");
            }
        }

        // ─────────────────────────────────────────────
        //  Event Handlers
        // ─────────────────────────────────────────────

        /// <summary>
        /// Merchant: remove 1 random item from inventory, gain 1 better-rarity item from pool.
        /// Requires at least 1 item in inventory and items in the pool.
        /// </summary>
        private static EventOutcome ProcessMerchant(GameRunState state, Random random, List<ItemData> itemPool)
        {
            if (state.inventory == null || state.inventory.Count == 0)
                return CreateOutcome(false, "event_merchant_no_items");

            // Remove a random item from inventory
            int removeIndex = random.Next(state.inventory.Count);
            var removedItem = state.inventory[removeIndex];

            // Find a better-rarity item from the pool
            ItemData gainedItem = null;
            if (itemPool != null && itemPool.Count > 0)
            {
                var betterItems = itemPool.Where(i =>
                    i.rarity > removedItem.rarity &&
                    i.category != ItemCategory.Trap &&
                    i.category != ItemCategory.Curse).ToList();

                if (betterItems.Count > 0)
                {
                    gainedItem = betterItems[random.Next(betterItems.Count)];
                }
                else
                {
                    // If no better rarity available, pick any non-trap/curse item
                    var validItems = itemPool.Where(i =>
                        i.category != ItemCategory.Trap &&
                        i.category != ItemCategory.Curse).ToList();

                    if (validItems.Count > 0)
                        gainedItem = validItems[random.Next(validItems.Count)];
                }
            }

            var outcome = new EventOutcome
            {
                success = gainedItem != null,
                messageKey = gainedItem != null ? "event_merchant_trade_success" : "event_merchant_trade_fail",
                itemsGained = gainedItem != null ? new List<ItemData> { gainedItem } : new List<ItemData>(),
                itemsLostIds = new List<string> { removedItem.id },
                hpChange = 0,
                goldChange = 0,
                buffGained = null
            };

            return outcome;
        }

        /// <summary>
        /// Altar: sacrifice 1 item from inventory, gain a random buff.
        /// Requires at least 1 item in inventory.
        /// </summary>
        private static EventOutcome ProcessAltar(GameRunState state, Random random)
        {
            if (state.inventory == null || state.inventory.Count == 0)
                return CreateOutcome(false, "event_altar_no_items");

            // Sacrifice a random item
            int sacrificeIndex = random.Next(state.inventory.Count);
            var sacrificedItem = state.inventory[sacrificeIndex];

            // Grant a random buff
            var buffValues = (BuffType[])Enum.GetValues(typeof(BuffType));
            var buff = buffValues[random.Next(buffValues.Length)];

            return new EventOutcome
            {
                success = true,
                messageKey = "event_altar_sacrifice",
                itemsGained = new List<ItemData>(),
                itemsLostIds = new List<string> { sacrificedItem.id },
                hpChange = 0,
                goldChange = 0,
                buffGained = buff
            };
        }

        /// <summary>
        /// Chest: if the player has a matching Key-category item, grant great loot.
        /// Without a key, the chest is trapped and deals 20 HP damage.
        /// </summary>
        private static EventOutcome ProcessChest(GameRunState state, Random random, List<ItemData> itemPool)
        {
            bool hasKey = state.inventory != null &&
                          state.inventory.Any(i => i.category == ItemCategory.Key);

            if (hasKey)
            {
                // Consume the key
                var keyItem = state.inventory.First(i => i.category == ItemCategory.Key);

                // Generate great loot (Rare or Legendary)
                ItemData loot = null;
                if (itemPool != null && itemPool.Count > 0)
                {
                    var greatLoot = itemPool.Where(i =>
                        (i.rarity == ItemRarity.Rare || i.rarity == ItemRarity.Legendary) &&
                        i.category != ItemCategory.Trap &&
                        i.category != ItemCategory.Curse).ToList();

                    if (greatLoot.Count > 0)
                        loot = greatLoot[random.Next(greatLoot.Count)];
                }

                return new EventOutcome
                {
                    success = true,
                    messageKey = "event_chest_unlocked",
                    itemsGained = loot != null ? new List<ItemData> { loot } : new List<ItemData>(),
                    itemsLostIds = new List<string> { keyItem.id },
                    hpChange = 0,
                    goldChange = 0,
                    buffGained = null
                };
            }
            else
            {
                // No key: trap damage
                return new EventOutcome
                {
                    success = false,
                    messageKey = "event_chest_trapped",
                    itemsGained = new List<ItemData>(),
                    itemsLostIds = new List<string>(),
                    hpChange = -20,
                    goldChange = 0,
                    buffGained = null
                };
            }
        }

        /// <summary>
        /// Curse: a random item in inventory becomes cursed.
        /// The cursed item loses its positive stats (attackPower and defensePower halved).
        /// isCursed is set to true on the item.
        /// </summary>
        private static EventOutcome ProcessCurse(GameRunState state, Random random)
        {
            if (state.inventory == null || state.inventory.Count == 0)
                return CreateOutcome(false, "event_curse_no_items");

            // Find non-cursed items to curse
            var cursableItems = state.inventory.Where(i => !i.isCursed).ToList();
            if (cursableItems.Count == 0)
                return CreateOutcome(false, "event_curse_all_cursed");

            // Curse a random item
            var target = cursableItems[random.Next(cursableItems.Count)];
            target.isCursed = true;
            target.attackPower = target.attackPower / 2;
            target.defensePower = target.defensePower / 2;

            return new EventOutcome
            {
                success = false,
                messageKey = "event_curse_applied",
                itemsGained = new List<ItemData>(),
                itemsLostIds = new List<string>(),
                hpChange = 0,
                goldChange = 0,
                buffGained = null
            };
        }

        /// <summary>
        /// Wheel of Fortune: random outcome with weighted probabilities.
        /// 40% good item, 20% buff, 20% gold, 10% nothing, 10% damage.
        /// </summary>
        private static EventOutcome ProcessWheelOfFortune(GameRunState state, Random random, List<ItemData> itemPool)
        {
            int roll = random.Next(100);

            if (roll < 40)
            {
                // 40%: Good item
                ItemData item = null;
                if (itemPool != null && itemPool.Count > 0)
                {
                    var goodItems = itemPool.Where(i =>
                        i.category != ItemCategory.Trap &&
                        i.category != ItemCategory.Curse).ToList();

                    if (goodItems.Count > 0)
                        item = goodItems[random.Next(goodItems.Count)];
                }

                return new EventOutcome
                {
                    success = true,
                    messageKey = "event_wheel_item",
                    itemsGained = item != null ? new List<ItemData> { item } : new List<ItemData>(),
                    itemsLostIds = new List<string>(),
                    hpChange = 0,
                    goldChange = 0,
                    buffGained = null
                };
            }
            else if (roll < 60)
            {
                // 20%: Buff
                var buffValues = (BuffType[])Enum.GetValues(typeof(BuffType));
                var buff = buffValues[random.Next(buffValues.Length)];

                return new EventOutcome
                {
                    success = true,
                    messageKey = "event_wheel_buff",
                    itemsGained = new List<ItemData>(),
                    itemsLostIds = new List<string>(),
                    hpChange = 0,
                    goldChange = 0,
                    buffGained = buff
                };
            }
            else if (roll < 80)
            {
                // 20%: Gold
                int goldAmount = 50 + random.Next(101); // 50-150 gold

                return new EventOutcome
                {
                    success = true,
                    messageKey = "event_wheel_gold",
                    itemsGained = new List<ItemData>(),
                    itemsLostIds = new List<string>(),
                    hpChange = 0,
                    goldChange = goldAmount,
                    buffGained = null
                };
            }
            else if (roll < 90)
            {
                // 10%: Nothing
                return CreateOutcome(true, "event_wheel_nothing");
            }
            else
            {
                // 10%: Damage
                int damage = 10 + random.Next(21); // 10-30 damage

                return new EventOutcome
                {
                    success = false,
                    messageKey = "event_wheel_damage",
                    itemsGained = new List<ItemData>(),
                    itemsLostIds = new List<string>(),
                    hpChange = -damage,
                    goldChange = 0,
                    buffGained = null
                };
            }
        }

        /// <summary>
        /// Healer: restore 30 HP for free. Always succeeds.
        /// </summary>
        private static EventOutcome ProcessHealer(GameRunState state)
        {
            return new EventOutcome
            {
                success = true,
                messageKey = "event_healer_restore",
                itemsGained = new List<ItemData>(),
                itemsLostIds = new List<string>(),
                hpChange = 30,
                goldChange = 0,
                buffGained = null
            };
        }

        /// <summary>
        /// Blacksmith: upgrade 1 random item's attackPower or defensePower by 50%.
        /// Requires at least 1 non-cursed item with positive combat stats in inventory.
        /// </summary>
        private static EventOutcome ProcessBlacksmith(GameRunState state, Random random)
        {
            if (state.inventory == null || state.inventory.Count == 0)
                return CreateOutcome(false, "event_blacksmith_no_items");

            // Find items that can be upgraded (have attack or defense, not cursed)
            var upgradeable = state.inventory.Where(i =>
                !i.isCursed &&
                (i.attackPower > 0 || i.defensePower > 0)).ToList();

            if (upgradeable.Count == 0)
                return CreateOutcome(false, "event_blacksmith_no_upgradeable");

            // Pick a random item to upgrade
            var target = upgradeable[random.Next(upgradeable.Count)];

            // Decide whether to upgrade attack or defense
            bool upgradeAttack;
            if (target.attackPower > 0 && target.defensePower > 0)
                upgradeAttack = random.Next(2) == 0;
            else
                upgradeAttack = target.attackPower > 0;

            if (upgradeAttack)
            {
                int bonus = Math.Max(target.attackPower / 2, 1);
                target.attackPower += bonus;
            }
            else
            {
                int bonus = Math.Max(target.defensePower / 2, 1);
                target.defensePower += bonus;
            }

            return new EventOutcome
            {
                success = true,
                messageKey = "event_blacksmith_upgrade",
                itemsGained = new List<ItemData>(),
                itemsLostIds = new List<string>(),
                hpChange = 0,
                goldChange = 0,
                buffGained = null
            };
        }

        /// <summary>
        /// Trap: the player takes 15-30 damage (random). Always negative.
        /// </summary>
        private static EventOutcome ProcessTrap(GameRunState state, Random random)
        {
            int damage = 15 + random.Next(16); // 15-30 damage

            return new EventOutcome
            {
                success = false,
                messageKey = "event_trap_damage",
                itemsGained = new List<ItemData>(),
                itemsLostIds = new List<string>(),
                hpChange = -damage,
                goldChange = 0,
                buffGained = null
            };
        }

        // ─────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────

        /// <summary>
        /// Create a simple outcome with no item or stat changes.
        /// </summary>
        private static EventOutcome CreateOutcome(bool success, string messageKey)
        {
            return new EventOutcome
            {
                success = success,
                messageKey = messageKey,
                itemsGained = new List<ItemData>(),
                itemsLostIds = new List<string>(),
                hpChange = 0,
                goldChange = 0,
                buffGained = null
            };
        }
    }
}
