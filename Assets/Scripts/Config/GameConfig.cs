using UnityEngine;

namespace LootOrLose.Config
{
    /// <summary>
    /// ScriptableObject for runtime-configurable game settings.
    /// Values can be overridden by Firebase Remote Config at startup.
    /// Create an instance via Assets > Create > LootOrLose > Game Config.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "LootOrLose/Game Config")]
    public class GameConfig : ScriptableObject
    {
        // ─── Timer ──────────────────────────────────────────────────

        [Header("Timer")]
        [Tooltip("Time in seconds the player has to decide LOOT or LEAVE.")]
        public float timerDuration = 3.0f;

        [Tooltip("Extended timer duration when accessibility mode is enabled.")]
        public float accessibilityTimerDuration = 5.0f;

        // ─── Inventory ──────────────────────────────────────────────

        [Header("Inventory")]
        [Tooltip("Number of inventory slots available to most characters by default.")]
        public int defaultInventorySlots = 5;

        // ─── Player ─────────────────────────────────────────────────

        [Header("Player")]
        [Tooltip("Default starting HP for the base character (Warrior).")]
        public int defaultPlayerHP = 100;

        // ─── Progression ────────────────────────────────────────────

        [Header("Progression")]
        [Tooltip("A boss encounter occurs every N rounds.")]
        public int bossRoundInterval = 15;

        // ─── Events ─────────────────────────────────────────────────

        [Header("Events")]
        [Tooltip("Chance (0.0 to 1.0) that a random event occurs between rounds.")]
        [Range(0f, 1f)]
        public float eventChance = 0.3f;

        // ─── Difficulty ─────────────────────────────────────────────

        [Header("Difficulty")]
        [Tooltip("Global difficulty multiplier affecting enemy HP and trap damage. 1.0 = normal.")]
        [Range(0.5f, 2.0f)]
        public float difficultyMultiplier = 1.0f;

        // ─── Monetization ───────────────────────────────────────────

        [Header("Monetization")]
        [Tooltip("Whether rewarded video ads are enabled.")]
        public bool adsEnabled = true;

        [Tooltip("Maximum number of rewarded ads the player can watch per day.")]
        public int maxRewardedAdsPerDay = 3;

        [Tooltip("Whether this player has premium (ad-free) status.")]
        public bool premiumUser = false;
    }
}
