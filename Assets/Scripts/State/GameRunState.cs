using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.State
{
    /// <summary>
    /// Holds all mutable runtime state for an active game run.
    /// This class is NOT serialized to disk -- it exists only while a run is in progress.
    /// A new instance is created at the start of each run and discarded when the run ends.
    /// </summary>
    public class GameRunState
    {
        /// <summary>
        /// Creates a new run state initialized from the selected character and biome.
        /// </summary>
        /// <param name="character">The character chosen for this run.</param>
        /// <param name="biome">The biome (dungeon theme) for this run.</param>
        /// <param name="dailySeed">Optional seed for daily challenge runs.</param>
        public GameRunState(CharacterData character, BiomeData biome, int? dailySeed = null)
        {
            this.character = character;
            currentRound = 0;
            playerHP = character.baseHP;
            maxHP = character.baseHP;
            maxInventorySlots = character.inventorySlots;
            inventory = new List<ItemData>();
            gold = 0;
            score = 0;
            currentBiome = biome.type;
            currentZone = DungeonZone.Tutorial;
            activeBuffs = new List<BuffType>();
            isAlive = true;
            bossesDefeated = 0;
            itemsLooted = 0;
            itemsLeft = 0;
            runStartTime = 0f;
            seededRandom = dailySeed.HasValue ? new System.Random(dailySeed.Value) : new System.Random();
        }

        // --- Round Tracking ---

        /// <summary>The current round number (1-based). Boss fights occur every 15 rounds.</summary>
        public int currentRound;

        // --- Player Vitals ---

        /// <summary>The player's current hit points. The run ends when this reaches 0.</summary>
        public int playerHP;

        /// <summary>The player's maximum hit points, determined by character base HP and modifiers.</summary>
        public int maxHP;

        // --- Inventory ---

        /// <summary>
        /// The player's current inventory of looted items.
        /// Total slot usage must not exceed <see cref="maxInventorySlots"/>.
        /// </summary>
        public List<ItemData> inventory;

        /// <summary>
        /// Maximum number of inventory slots available (typically 5).
        /// Determined by the selected character's <see cref="CharacterData.inventorySlots"/>.
        /// </summary>
        public int maxInventorySlots;

        // --- Economy ---

        /// <summary>Gold collected during the current run.</summary>
        public int gold;

        /// <summary>Score accumulated during the current run.</summary>
        public int score;

        // --- Location ---

        /// <summary>The biome (dungeon theme) selected for this run.</summary>
        public BiomeType currentBiome;

        /// <summary>
        /// The current dungeon zone, computed from <see cref="currentRound"/>
        /// via <see cref="GetCurrentZone"/>.
        /// </summary>
        public DungeonZone currentZone;

        // --- Character ---

        /// <summary>The character the player selected for this run.</summary>
        public CharacterData character;

        // --- Buffs ---

        /// <summary>Active temporary buffs applied to the player during this run.</summary>
        public List<BuffType> activeBuffs;

        // --- Run Status ---

        /// <summary>Whether the player is still alive. False when HP reaches 0.</summary>
        public bool isAlive;

        // --- Statistics ---

        /// <summary>Number of bosses defeated during this run.</summary>
        public int bossesDefeated;

        /// <summary>Number of items the player chose to loot during this run.</summary>
        public int itemsLooted;

        /// <summary>Number of items the player chose to leave during this run.</summary>
        public int itemsLeft;

        // --- Timing ---

        /// <summary>
        /// Timestamp (Time.realtimeSinceStartup) when the run started,
        /// used to calculate total run duration.
        /// </summary>
        public float runStartTime;

        // --- Seeded Random ---

        /// <summary>
        /// Seeded random number generator for deterministic runs (e.g., daily challenges).
        /// Null for standard unseeded runs.
        /// </summary>
        public System.Random seededRandom;

        // --- Methods ---

        /// <summary>
        /// Determines the current dungeon zone based on the round number.
        /// Zones progress as the player advances deeper into the dungeon.
        /// </summary>
        /// <returns>The <see cref="DungeonZone"/> corresponding to the current round.</returns>
        public DungeonZone GetCurrentZone()
        {
            // Zone thresholds: rounds 1-15 = Zone 1, 16-30 = Zone 2, etc.
            int zoneIndex = (currentRound - 1) / 15;
            var zones = (DungeonZone[])Enum.GetValues(typeof(DungeonZone));

            if (zoneIndex >= zones.Length)
                zoneIndex = zones.Length - 1;

            currentZone = zones[zoneIndex];
            return currentZone;
        }

        /// <summary>
        /// Checks whether the player currently has a specific item in their inventory.
        /// </summary>
        /// <param name="id">The unique item ID to search for.</param>
        /// <returns>True if an item with the given ID is in the inventory.</returns>
        public bool HasItem(string id)
        {
            if (inventory == null)
                return false;

            return inventory.Any(item => item != null && item.id == id);
        }

        /// <summary>
        /// Calculates the total number of inventory slots currently in use.
        /// Items may occupy 1 or 2 slots each based on their <see cref="ItemData.slotSize"/>.
        /// </summary>
        /// <returns>The total number of slots currently occupied.</returns>
        public int GetInventorySlotUsage()
        {
            if (inventory == null)
                return 0;

            return inventory.Sum(item => item != null ? item.slotSize : 0);
        }

        /// <summary>
        /// Determines whether the player can loot a given item based on
        /// remaining inventory slot capacity.
        /// </summary>
        /// <param name="item">The item the player wants to loot.</param>
        /// <returns>True if there are enough free slots to accommodate the item.</returns>
        public bool CanLootItem(ItemData item)
        {
            if (item == null)
                return false;

            int usedSlots = GetInventorySlotUsage();
            return (usedSlots + item.slotSize) <= maxInventorySlots;
        }
    }
}
