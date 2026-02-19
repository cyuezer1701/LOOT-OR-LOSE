using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.Managers
{
    // ─────────────────────────────────────────────
    //  JSON Wrapper Classes
    //  Unity's JsonUtility cannot deserialize top-level arrays,
    //  so each data type needs a wrapper with a root array field.
    // ─────────────────────────────────────────────

    /// <summary>Wrapper for deserializing a JSON array of <see cref="ItemData"/>.</summary>
    [Serializable]
    public class ItemDataList
    {
        public List<ItemData> items;
    }

    /// <summary>Wrapper for deserializing a JSON array of <see cref="BossData"/>.</summary>
    [Serializable]
    public class BossDataList
    {
        public List<BossData> bosses;
    }

    /// <summary>Wrapper for deserializing a JSON array of <see cref="EventData"/>.</summary>
    [Serializable]
    public class EventDataList
    {
        public List<EventData> events;
    }

    /// <summary>Wrapper for deserializing a JSON array of <see cref="CharacterData"/>.</summary>
    [Serializable]
    public class CharacterDataList
    {
        public List<CharacterData> characters;
    }

    /// <summary>Wrapper for deserializing a JSON array of <see cref="BiomeData"/>.</summary>
    [Serializable]
    public class BiomeDataList
    {
        public List<BiomeData> biomes;
    }

    // ─────────────────────────────────────────────
    //  DataManager
    // ─────────────────────────────────────────────

    /// <summary>
    /// Loads and provides access to all game data definitions (items, bosses, events,
    /// characters, biomes). Data is loaded from JSON files in the Resources folder on Awake.
    /// Singleton pattern with DontDestroyOnLoad.
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; private set; }

        // ─────────────────────────────────────────────
        //  Resource Paths
        // ─────────────────────────────────────────────

        [Header("Data Resource Paths")]
        [SerializeField] private string itemsPath = "Data/items";
        [SerializeField] private string bossesPath = "Data/bosses";
        [SerializeField] private string eventsPath = "Data/events";
        [SerializeField] private string charactersPath = "Data/characters";
        [SerializeField] private string biomesPath = "Data/biomes";

        // ─────────────────────────────────────────────
        //  Data Storage
        // ─────────────────────────────────────────────

        private List<ItemData> allItems = new List<ItemData>();
        private List<BossData> allBosses = new List<BossData>();
        private List<EventData> allEvents = new List<EventData>();
        private List<CharacterData> allCharacters = new List<CharacterData>();
        private List<BiomeData> allBiomes = new List<BiomeData>();

        // Lookup dictionaries for fast ID-based access
        private Dictionary<string, ItemData> itemLookup = new Dictionary<string, ItemData>();
        private Dictionary<string, BossData> bossLookup = new Dictionary<string, BossData>();

        // ─────────────────────────────────────────────
        //  Public Properties
        // ─────────────────────────────────────────────

        /// <summary>All loaded item definitions.</summary>
        public List<ItemData> AllItems => allItems;

        /// <summary>All loaded boss definitions.</summary>
        public List<BossData> AllBosses => allBosses;

        /// <summary>All loaded event definitions.</summary>
        public List<EventData> AllEvents => allEvents;

        /// <summary>All loaded character definitions.</summary>
        public List<CharacterData> AllCharacters => allCharacters;

        /// <summary>All loaded biome definitions.</summary>
        public List<BiomeData> AllBiomes => allBiomes;

        // ─────────────────────────────────────────────
        //  Lifecycle
        // ─────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAllData();
        }

        // ─────────────────────────────────────────────
        //  Data Loading
        // ─────────────────────────────────────────────

        /// <summary>
        /// Load all game data from JSON files in the Resources folder.
        /// Called once during Awake.
        /// </summary>
        private void LoadAllData()
        {
            LoadItems();
            LoadBosses();
            LoadEvents();
            LoadCharacters();
            LoadBiomes();

            Debug.Log($"[DataManager] Data loaded: {allItems.Count} items, {allBosses.Count} bosses, " +
                      $"{allEvents.Count} events, {allCharacters.Count} characters, {allBiomes.Count} biomes");
        }

        /// <summary>
        /// Load item definitions from Resources/<see cref="itemsPath"/>.json.
        /// </summary>
        private void LoadItems()
        {
            TextAsset json = Resources.Load<TextAsset>(itemsPath);
            if (json == null)
            {
                Debug.LogWarning($"[DataManager] Items data not found at Resources/{itemsPath}");
                return;
            }

            var wrapper = JsonUtility.FromJson<ItemDataList>(json.text);
            if (wrapper?.items != null)
            {
                allItems = wrapper.items;
                itemLookup.Clear();
                foreach (var item in allItems)
                {
                    if (!string.IsNullOrEmpty(item.id))
                        itemLookup[item.id] = item;
                }
            }
        }

        /// <summary>
        /// Load boss definitions from Resources/<see cref="bossesPath"/>.json.
        /// </summary>
        private void LoadBosses()
        {
            TextAsset json = Resources.Load<TextAsset>(bossesPath);
            if (json == null)
            {
                Debug.LogWarning($"[DataManager] Bosses data not found at Resources/{bossesPath}");
                return;
            }

            var wrapper = JsonUtility.FromJson<BossDataList>(json.text);
            if (wrapper?.bosses != null)
            {
                allBosses = wrapper.bosses;
                bossLookup.Clear();
                foreach (var boss in allBosses)
                {
                    if (!string.IsNullOrEmpty(boss.id))
                        bossLookup[boss.id] = boss;
                }
            }
        }

        /// <summary>
        /// Load event definitions from Resources/<see cref="eventsPath"/>.json.
        /// </summary>
        private void LoadEvents()
        {
            TextAsset json = Resources.Load<TextAsset>(eventsPath);
            if (json == null)
            {
                Debug.LogWarning($"[DataManager] Events data not found at Resources/{eventsPath}");
                return;
            }

            var wrapper = JsonUtility.FromJson<EventDataList>(json.text);
            if (wrapper?.events != null)
            {
                allEvents = wrapper.events;
            }
        }

        /// <summary>
        /// Load character definitions from Resources/<see cref="charactersPath"/>.json.
        /// </summary>
        private void LoadCharacters()
        {
            TextAsset json = Resources.Load<TextAsset>(charactersPath);
            if (json == null)
            {
                Debug.LogWarning($"[DataManager] Characters data not found at Resources/{charactersPath}");
                return;
            }

            var wrapper = JsonUtility.FromJson<CharacterDataList>(json.text);
            if (wrapper?.characters != null)
            {
                allCharacters = wrapper.characters;
            }
        }

        /// <summary>
        /// Load biome definitions from Resources/<see cref="biomesPath"/>.json.
        /// </summary>
        private void LoadBiomes()
        {
            TextAsset json = Resources.Load<TextAsset>(biomesPath);
            if (json == null)
            {
                Debug.LogWarning($"[DataManager] Biomes data not found at Resources/{biomesPath}");
                return;
            }

            var wrapper = JsonUtility.FromJson<BiomeDataList>(json.text);
            if (wrapper?.biomes != null)
            {
                allBiomes = wrapper.biomes;
            }
        }

        // ─────────────────────────────────────────────
        //  Lookup Methods
        // ─────────────────────────────────────────────

        /// <summary>
        /// Get an item definition by its unique ID.
        /// </summary>
        /// <param name="id">The item's unique identifier.</param>
        /// <returns>The matching <see cref="ItemData"/>, or null if not found.</returns>
        public ItemData GetItemById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            itemLookup.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Get a boss definition by its unique ID.
        /// </summary>
        /// <param name="id">The boss's unique identifier.</param>
        /// <returns>The matching <see cref="BossData"/>, or null if not found.</returns>
        public BossData GetBossById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            bossLookup.TryGetValue(id, out var boss);
            return boss;
        }

        /// <summary>
        /// Get the appropriate boss for a given round and biome.
        /// Prefers bosses whose preferred biome matches the current biome.
        /// Falls back to any boss eligible for the current round.
        /// </summary>
        /// <param name="round">The current round number.</param>
        /// <param name="biome">The current biome type.</param>
        /// <returns>A suitable <see cref="BossData"/>, or null if none available.</returns>
        public BossData GetBossForRound(int round, BiomeType biome)
        {
            if (allBosses == null || allBosses.Count == 0) return null;

            // Filter bosses eligible for this round
            var eligible = allBosses.Where(b => b.minRound <= round).ToList();
            if (eligible.Count == 0) return null;

            // Prefer bosses matching the current biome
            var biomeMatches = eligible.Where(b => b.preferredBiome == biome).ToList();
            if (biomeMatches.Count > 0)
            {
                return biomeMatches[UnityEngine.Random.Range(0, biomeMatches.Count)];
            }

            // Fallback: any eligible boss
            return eligible[UnityEngine.Random.Range(0, eligible.Count)];
        }

        /// <summary>
        /// Get a random event appropriate for the current dungeon zone.
        /// Filters events by zone availability and minimum round requirements.
        /// </summary>
        /// <param name="zone">The current dungeon zone.</param>
        /// <param name="random">Seeded random instance for deterministic selection.</param>
        /// <returns>A valid <see cref="EventData"/>, or null if none available.</returns>
        public EventData GetRandomEvent(DungeonZone zone, System.Random random)
        {
            if (allEvents == null || allEvents.Count == 0) return null;

            // Filter events available in this zone
            var available = allEvents.Where(e =>
                e.availableZones == null ||
                e.availableZones.Length == 0 ||
                e.availableZones.Contains(zone)
            ).ToList();

            if (available.Count == 0) return null;

            // Weighted selection by probability
            float totalProbability = available.Sum(e => e.probability);
            if (totalProbability <= 0f)
                return available[random.Next(available.Count)];

            float roll = (float)(random.NextDouble() * totalProbability);
            float cumulative = 0f;

            foreach (var eventData in available)
            {
                cumulative += eventData.probability;
                if (roll <= cumulative)
                    return eventData;
            }

            // Fallback
            return available[available.Count - 1];
        }

        /// <summary>
        /// Get all items belonging to a specific category.
        /// </summary>
        /// <param name="category">The item category to filter by.</param>
        /// <returns>A list of items matching the category.</returns>
        public List<ItemData> GetItemsByCategory(ItemCategory category)
        {
            return allItems.Where(i => i.category == category).ToList();
        }

        /// <summary>
        /// Get all items available in a specific biome.
        /// Items with no biome restriction are included in all biomes.
        /// </summary>
        /// <param name="biome">The biome type to filter by.</param>
        /// <returns>A list of items available in the specified biome.</returns>
        public List<ItemData> GetItemsByBiome(BiomeType biome)
        {
            return allItems.Where(i =>
                i.availableBiomes == null ||
                i.availableBiomes.Length == 0 ||
                i.availableBiomes.Contains(biome)
            ).ToList();
        }
    }
}
