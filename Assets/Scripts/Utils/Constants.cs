namespace LootOrLose.Utils
{
    /// <summary>
    /// Game-wide constants used throughout the application.
    /// These values are compile-time constants and cannot be changed at runtime.
    /// For runtime-configurable values, see <see cref="Config.GameConfig"/>.
    /// </summary>
    public static class Constants
    {
        // ─── Gameplay ────────────────────────────────────────────────

        /// <summary>Default time in seconds the player has to decide LOOT or LEAVE.</summary>
        public const float DEFAULT_TIMER_DURATION = 3.0f;

        /// <summary>Extended timer duration for accessibility mode.</summary>
        public const float EXTENDED_TIMER_DURATION = 5.0f;

        /// <summary>Default number of inventory slots for most characters.</summary>
        public const int DEFAULT_INVENTORY_SLOTS = 5;

        /// <summary>Maximum possible inventory slots (Merchant with upgrades).</summary>
        public const int MAX_INVENTORY_SLOTS = 8;

        /// <summary>Default starting HP for the Warrior character.</summary>
        public const int DEFAULT_PLAYER_HP = 100;

        /// <summary>Absolute maximum HP a player can reach.</summary>
        public const int MAX_PLAYER_HP = 200;

        /// <summary>A boss encounter occurs every N rounds.</summary>
        public const int BOSS_ROUND_INTERVAL = 15;

        // ─── Zones ──────────────────────────────────────────────────

        /// <summary>Last round of the tutorial zone (rounds 1-10).</summary>
        public const int TUTORIAL_ZONE_END = 10;

        /// <summary>Last round of the standard zone (rounds 11-25).</summary>
        public const int STANDARD_ZONE_END = 25;

        /// <summary>Last round of the danger zone (rounds 26-40). Beyond this is Chaos.</summary>
        public const int DANGER_ZONE_END = 40;

        // ─── Scoring ────────────────────────────────────────────────

        /// <summary>Base points awarded for each round completed.</summary>
        public const int POINTS_PER_ROUND = 10;

        /// <summary>Bonus points awarded for each boss defeated.</summary>
        public const int POINTS_PER_BOSS = 500;

        /// <summary>Bonus points awarded for each active synergy.</summary>
        public const int POINTS_PER_SYNERGY = 100;

        /// <summary>Score value for a Common rarity item in final inventory.</summary>
        public const int RARITY_SCORE_COMMON = 10;

        /// <summary>Score value for an Uncommon rarity item in final inventory.</summary>
        public const int RARITY_SCORE_UNCOMMON = 25;

        /// <summary>Score value for a Rare rarity item in final inventory.</summary>
        public const int RARITY_SCORE_RARE = 50;

        /// <summary>Score value for a Legendary rarity item in final inventory.</summary>
        public const int RARITY_SCORE_LEGENDARY = 100;

        /// <summary>Maximum score multiplier achievable through daily play streaks.</summary>
        public const float MAX_STREAK_MULTIPLIER = 2.0f;

        /// <summary>Additional streak multiplier gained per consecutive day played.</summary>
        public const float STREAK_MULTIPLIER_PER_DAY = 0.1f;

        // ─── Events ─────────────────────────────────────────────────

        /// <summary>Damage dealt when opening a trapped chest without a key.</summary>
        public const int CHEST_TRAP_DAMAGE = 20;

        /// <summary>HP restored by the Healer event.</summary>
        public const int HEALER_HEAL_AMOUNT = 30;

        /// <summary>Multiplier applied to weapon stats when upgraded by the Blacksmith.</summary>
        public const float BLACKSMITH_UPGRADE_MULTIPLIER = 1.5f;

        /// <summary>Minimum damage dealt by trap events.</summary>
        public const int TRAP_MIN_DAMAGE = 15;

        /// <summary>Maximum damage dealt by trap events.</summary>
        public const int TRAP_MAX_DAMAGE = 30;

        // ─── Combat ─────────────────────────────────────────────────

        /// <summary>Damage multiplier when hitting a boss's weakness category.</summary>
        public const float WEAKNESS_MULTIPLIER = 1.5f;

        /// <summary>Damage multiplier when hitting a boss's resistance category.</summary>
        public const float RESISTANCE_MULTIPLIER = 0.5f;

        // ─── Resource Paths ─────────────────────────────────────────

        /// <summary>File name for the local player progress save file.</summary>
        public const string SAVE_FILE_NAME = "player_progress.json";

        /// <summary>Path to the Firebase configuration file in StreamingAssets.</summary>
        public const string FIREBASE_CONFIG_PATH = "firebase-config";

        /// <summary>Resources path prefix for item data JSON files.</summary>
        public const string ITEMS_PATH_PREFIX = "Items/";

        /// <summary>Resources path for the bosses data JSON file.</summary>
        public const string BOSSES_PATH = "Bosses/bosses";

        /// <summary>Resources path for the events data JSON file.</summary>
        public const string EVENTS_PATH = "Events/events";

        /// <summary>Resources path for the characters data JSON file.</summary>
        public const string CHARACTERS_PATH = "Characters/characters";

        /// <summary>Resources path for the biomes data JSON file.</summary>
        public const string BIOMES_PATH = "Biomes/biomes";

        /// <summary>Resources path prefix for locale JSON files.</summary>
        public const string LOCALES_PATH_PREFIX = "Locales/";

        // ─── Firebase Collections ───────────────────────────────────

        /// <summary>Firestore collection name for leaderboard entries.</summary>
        public const string COLLECTION_LEADERBOARDS = "leaderboards";

        /// <summary>Firestore collection name for player profiles.</summary>
        public const string COLLECTION_PLAYERS = "players";

        /// <summary>Firestore collection name for daily run data.</summary>
        public const string COLLECTION_DAILY_RUNS = "dailyRuns";

        // ─── Unlock Conditions ──────────────────────────────────────

        /// <summary>Number of completed runs required to unlock the Rogue character.</summary>
        public const int UNLOCK_ROGUE_RUNS = 10;

        /// <summary>Number of bosses defeated required to unlock the Mage character.</summary>
        public const int UNLOCK_MAGE_BOSSES = 5;

        /// <summary>Number of items looted required to unlock the Merchant character.</summary>
        public const int UNLOCK_MERCHANT_ITEMS = 100;

        /// <summary>Round that must be reached to unlock the Volcano biome.</summary>
        public const int UNLOCK_VOLCANO_ROUND = 30;

        /// <summary>Score that must be achieved to unlock the Abyss biome.</summary>
        public const int UNLOCK_ABYSS_SCORE = 5000;
    }
}
