namespace LootOrLose.Enums
{
    /// <summary>
    /// Represents the current state of the game application.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        InRun,
        BossEncounter,
        Event,
        Death,
        RunSummary,
        Settings,
        Shop,
        DailyRun
    }

    /// <summary>
    /// Categories that define an item's function and behavior.
    /// </summary>
    public enum ItemCategory
    {
        Weapon,
        Defense,
        Consumable,
        Key,
        Trap,
        Artifact,
        Curse
    }

    /// <summary>
    /// Rarity tiers that determine an item's power level and drop frequency.
    /// </summary>
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    /// <summary>
    /// Biome types that define the dungeon environment and available loot pools.
    /// </summary>
    public enum BiomeType
    {
        Crypt,
        Volcano,
        IcePalace,
        Abyss
    }

    /// <summary>
    /// Types of random events the player can encounter between rounds.
    /// </summary>
    public enum EventType
    {
        Merchant,
        Altar,
        Chest,
        Curse,
        WheelOfFortune,
        Trap,
        Healer,
        Blacksmith
    }

    /// <summary>
    /// Boss types encountered every 15 rounds, each with unique mechanics.
    /// </summary>
    public enum BossType
    {
        SkeletonKing,
        FireDragon,
        IceQueen,
        AbyssLord,
        ShadowReaper
    }

    /// <summary>
    /// Dungeon zones that escalate in difficulty as the run progresses.
    /// </summary>
    public enum DungeonZone
    {
        Tutorial,
        Standard,
        Danger,
        Chaos
    }

    /// <summary>
    /// The result of the player's 3-second loot decision.
    /// </summary>
    public enum DecisionResult
    {
        Loot,
        Leave,
        Timeout
    }

    /// <summary>
    /// Temporary buff types that can be applied to the player during a run.
    /// </summary>
    public enum BuffType
    {
        DamageBoost,
        DefenseBoost,
        HealthBoost,
        LuckBoost,
        SpeedBoost
    }

    /// <summary>
    /// Synergy types triggered when specific item combinations are held in inventory.
    /// </summary>
    public enum SynergyType
    {
        ArmorSet,
        DualWield,
        ElementalCombo,
        PotionMaster,
        KeyCollector
    }

    /// <summary>
    /// Playable character archetypes, each with unique starting loadouts and passives.
    /// </summary>
    public enum CharacterType
    {
        Warrior,
        Rogue,
        Mage,
        Merchant
    }
}
