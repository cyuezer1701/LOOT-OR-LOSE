using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootOrLose.Core.Combat;
using LootOrLose.Core.Items;
using LootOrLose.Data;
using LootOrLose.Enums;
using LootOrLose.State;

namespace LootOrLose.Managers
{
    /// <summary>
    /// Manages the active game run loop including round progression, item presentation,
    /// decision processing, boss encounters, random events, and the 3-second decision timer.
    /// Bridges Core/ pure logic with Unity's MonoBehaviour lifecycle.
    /// </summary>
    public class RunManager : MonoBehaviour
    {
        public static RunManager Instance { get; private set; }

        // ─────────────────────────────────────────────
        //  Configuration
        // ─────────────────────────────────────────────

        [Header("Timer Settings")]
        [SerializeField] private float decisionTimerDuration = 3f;
        [SerializeField] private float timerWarningThreshold = 1f;

        [Header("Round Settings")]
        [SerializeField] private int bossRoundInterval = 15;
        [SerializeField] private float eventChance = 0.15f;

        // ─────────────────────────────────────────────
        //  Runtime State
        // ─────────────────────────────────────────────

        private GameRunState currentRun;
        private ItemData currentItem;
        private Coroutine timerCoroutine;
        private float timerRemaining;
        private bool isTimerActive;

        // ─────────────────────────────────────────────
        //  Public Properties
        // ─────────────────────────────────────────────

        /// <summary>Seconds remaining on the current decision timer.</summary>
        public float TimerRemaining => timerRemaining;

        /// <summary>Whether the decision timer is currently counting down.</summary>
        public bool IsTimerActive => isTimerActive;

        /// <summary>The item currently being presented to the player.</summary>
        public ItemData CurrentItem => currentItem;

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
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRunStarted.AddListener(OnRunStarted);
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRunStarted.RemoveListener(OnRunStarted);
            }
        }

        // ─────────────────────────────────────────────
        //  Run Lifecycle
        // ─────────────────────────────────────────────

        /// <summary>
        /// Called when GameManager fires OnRunStarted. Initializes the run and begins the first round.
        /// </summary>
        /// <param name="runState">The freshly created run state.</param>
        private void OnRunStarted(GameRunState runState)
        {
            currentRun = runState;
            Debug.Log("[RunManager] Run started. Beginning first round.");
            NextRound();
        }

        /// <summary>
        /// Start a new run directly (alternative entry point).
        /// Delegates to GameManager.StartNewRun which triggers OnRunStarted.
        /// </summary>
        /// <param name="character">The character selected for the run.</param>
        /// <param name="biome">The biome selected for the run.</param>
        /// <param name="dailySeed">Optional seed for daily challenge runs.</param>
        public void StartRun(CharacterData character, BiomeData biome, int? dailySeed = null)
        {
            GameManager.Instance.StartNewRun(character, biome, dailySeed);
        }

        // ─────────────────────────────────────────────
        //  Round Progression
        // ─────────────────────────────────────────────

        /// <summary>
        /// Advance to the next round. Increments the round counter, updates the zone,
        /// and determines whether this is a boss round, event round, or normal item round.
        /// </summary>
        public void NextRound()
        {
            if (currentRun == null || !currentRun.isAlive) return;

            currentRun.currentRound++;
            currentRun.GetCurrentZone();

            Debug.Log($"[RunManager] Round {currentRun.currentRound} | Zone: {currentRun.currentZone}");

            // Check for boss round (every 15 rounds)
            if (IsBossRound(currentRun.currentRound))
            {
                HandleBossRound();
                return;
            }

            // Check for random event
            System.Random rng = currentRun.seededRandom ?? new System.Random();
            if (rng.NextDouble() < eventChance && currentRun.currentRound > 5)
            {
                HandleEvent();
                return;
            }

            // Normal round: generate and present an item
            GenerateAndPresentItem();
        }

        /// <summary>
        /// Generate a random item for the current round and present it to the player.
        /// Starts the decision timer.
        /// </summary>
        private void GenerateAndPresentItem()
        {
            if (DataManager.Instance == null)
            {
                Debug.LogError("[RunManager] DataManager not available. Cannot generate item.");
                return;
            }

            System.Random rng = currentRun.seededRandom ?? new System.Random();

            currentItem = ItemGenerator.GenerateItem(
                DataManager.Instance.AllItems,
                currentRun.currentRound,
                currentRun.currentBiome,
                currentRun.currentZone,
                rng
            );

            if (currentItem == null)
            {
                Debug.LogWarning("[RunManager] No item generated for this round. Skipping.");
                NextRound();
                return;
            }

            GameManager.Instance.PresentItem(currentItem);
            StartDecisionTimer();

            // Play rarity-specific SFX
            PlayItemRevealSFX(currentItem);
        }

        /// <summary>
        /// Play a sound effect based on the revealed item's rarity.
        /// </summary>
        /// <param name="item">The item being revealed.</param>
        private void PlayItemRevealSFX(ItemData item)
        {
            if (AudioManager.Instance == null) return;

            switch (item.rarity)
            {
                case ItemRarity.Legendary:
                    AudioManager.Instance.PlaySFX("legendary_item");
                    break;
                case ItemRarity.Rare:
                    AudioManager.Instance.PlaySFX("rare_item");
                    break;
            }
        }

        // ─────────────────────────────────────────────
        //  Decision Processing
        // ─────────────────────────────────────────────

        /// <summary>
        /// Process the player's decision on the currently presented item.
        /// Handles loot (with inventory capacity check), leave, and timeout outcomes.
        /// </summary>
        /// <param name="decision">The decision the player made.</param>
        public void ProcessDecision(DecisionResult decision)
        {
            if (currentItem == null || currentRun == null) return;

            StopDecisionTimer();

            switch (decision)
            {
                case DecisionResult.Loot:
                    HandleLoot();
                    break;
                case DecisionResult.Leave:
                    HandleLeave();
                    break;
                case DecisionResult.Timeout:
                    HandleTimeout();
                    break;
            }

            GameManager.Instance.MakeDecision(decision, currentItem);

            // Check death conditions after processing
            if (CheckDeathConditions()) return;

            // Proceed to next round
            currentItem = null;
            NextRound();
        }

        /// <summary>
        /// Handle the player choosing to loot the current item.
        /// If inventory is full, triggers the inventory swap UI instead of auto-adding.
        /// </summary>
        private void HandleLoot()
        {
            if (currentRun.CanLootItem(currentItem))
            {
                // Add item to inventory
                currentRun.inventory.Add(currentItem);
                currentRun.itemsLooted++;

                // Apply immediate effects (consumables, heals)
                ApplyItemEffects(currentItem);

                // Check synergies after adding item
                CheckAndApplySynergies();

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("loot");

                Debug.Log($"[RunManager] Looted: {currentItem.nameKey} ({currentItem.rarity})");
            }
            else
            {
                // Inventory full - trigger swap UI
                // The UI will call CompleteLootSwap() when the player picks an item to discard
                Debug.Log("[RunManager] Inventory full. Swap required.");
                // Note: The UI layer handles presenting the swap interface.
                // ProcessDecision flow pauses here until CompleteLootSwap is called.
            }
        }

        /// <summary>
        /// Complete an inventory swap when the player's inventory is full.
        /// Removes the discarded item and adds the new one.
        /// </summary>
        /// <param name="discardedItem">The item the player chose to discard.</param>
        public void CompleteLootSwap(ItemData discardedItem)
        {
            if (discardedItem == null || currentItem == null) return;

            currentRun.inventory.Remove(discardedItem);
            currentRun.inventory.Add(currentItem);
            currentRun.itemsLooted++;

            ApplyItemEffects(currentItem);
            CheckAndApplySynergies();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("loot");

            Debug.Log($"[RunManager] Swapped {discardedItem.nameKey} for {currentItem.nameKey}");

            currentItem = null;

            if (!CheckDeathConditions())
                NextRound();
        }

        /// <summary>
        /// Handle the player choosing to leave the current item.
        /// </summary>
        private void HandleLeave()
        {
            currentRun.itemsLeft++;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("leave");

            Debug.Log($"[RunManager] Left: {currentItem.nameKey}");
        }

        /// <summary>
        /// Handle decision timeout. The item is lost with no damage taken.
        /// </summary>
        private void HandleTimeout()
        {
            currentRun.itemsLeft++;

            Debug.Log($"[RunManager] Timeout: {currentItem.nameKey} lost.");
        }

        /// <summary>
        /// Apply immediate effects of a looted item such as healing and curse damage.
        /// Consumable items are removed from inventory after use.
        /// </summary>
        /// <param name="item">The item whose effects to apply.</param>
        private void ApplyItemEffects(ItemData item)
        {
            // Apply healing
            if (item.healAmount > 0)
            {
                currentRun.playerHP = Mathf.Min(
                    currentRun.playerHP + item.healAmount,
                    currentRun.maxHP
                );

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("heal");

                Debug.Log($"[RunManager] Healed {item.healAmount} HP. Current HP: {currentRun.playerHP}");
            }

            // Apply curse damage
            if (item.isCursed)
            {
                int curseDamage = 10;
                currentRun.playerHP -= curseDamage;

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("trap");

                Debug.Log($"[RunManager] Curse dealt {curseDamage} damage. Current HP: {currentRun.playerHP}");
            }

            // Consume single-use items
            if (item.isConsumable)
            {
                currentRun.inventory.Remove(item);
                Debug.Log($"[RunManager] Consumable {item.nameKey} used and removed.");
            }
        }

        /// <summary>
        /// Recalculate synergy bonuses after an inventory change and apply any anti-synergy damage.
        /// </summary>
        private void CheckAndApplySynergies()
        {
            var totals = SynergyCalculator.CalculateTotalSynergyBonus(currentRun.inventory);

            if (totals.totalAntiSynergyDamage > 0)
            {
                currentRun.playerHP -= totals.totalAntiSynergyDamage;

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("trap");

                Debug.Log($"[RunManager] Anti-synergy dealt {totals.totalAntiSynergyDamage} damage.");
            }

            if (totals.hasFatalAntiSynergy)
            {
                currentRun.playerHP = 0;
                Debug.Log("[RunManager] Fatal anti-synergy triggered!");
            }

            // Synergy bonuses are applied dynamically during boss combat via SynergyCalculator,
            // so we only log them here for feedback.
            if (totals.totalBonusAttack > 0 || totals.totalBonusDefense > 0 || totals.totalBonusHP > 0)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("synergy");

                Debug.Log($"[RunManager] Synergy bonuses: +{totals.totalBonusAttack} ATK, " +
                          $"+{totals.totalBonusDefense} DEF, +{totals.totalBonusHP} HP");
            }
        }

        // ─────────────────────────────────────────────
        //  Boss Encounters
        // ─────────────────────────────────────────────

        /// <summary>
        /// Handle a boss round. Finds the appropriate boss for the current round and biome,
        /// calculates combat using BossCombatCalculator, and applies the result.
        /// </summary>
        private void HandleBossRound()
        {
            GameManager.Instance.ChangeState(GameState.BossEncounter);

            BossData boss = DataManager.Instance != null
                ? DataManager.Instance.GetBossForRound(currentRun.currentRound, currentRun.currentBiome)
                : null;

            if (boss == null)
            {
                Debug.LogWarning("[RunManager] No boss found for this round. Skipping boss encounter.");
                GameManager.Instance.ChangeState(GameState.InRun);
                GenerateAndPresentItem();
                return;
            }

            GameManager.Instance.OnBossEncounter?.Invoke(boss);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("boss_roar");

            Debug.Log($"[RunManager] Boss encounter: {boss.nameKey} (Round {currentRun.currentRound})");

            // Calculate combat outcome
            var synergies = SynergyCalculator.CheckSynergies(currentRun.inventory);
            BossCombatResult result = BossCombatCalculator.CalculateCombat(
                boss,
                currentRun.inventory,
                synergies,
                currentRun.currentRound
            );

            // Apply combat results
            currentRun.playerHP -= result.damageTaken;

            if (result.playerWon)
            {
                currentRun.bossesDefeated++;
                currentRun.score += 500;

                // Grant boss loot drops
                GrantBossDrops(boss);

                Debug.Log($"[RunManager] Boss defeated! Damage dealt: {result.damageDealt}, " +
                          $"Damage taken: {result.damageTaken}");
            }
            else
            {
                Debug.Log($"[RunManager] Boss survived with {result.bossRemainingHP} HP. " +
                          $"Player took {result.damageTaken} damage.");
            }

            // Return to InRun state
            GameManager.Instance.ChangeState(GameState.InRun);

            if (!CheckDeathConditions())
            {
                currentItem = null;
                NextRound();
            }
        }

        /// <summary>
        /// Grant guaranteed loot drops from a defeated boss.
        /// Items are added to inventory if space permits.
        /// </summary>
        /// <param name="boss">The defeated boss whose drops to grant.</param>
        private void GrantBossDrops(BossData boss)
        {
            if (boss.guaranteedDropIds == null || DataManager.Instance == null) return;

            foreach (string dropId in boss.guaranteedDropIds)
            {
                ItemData drop = DataManager.Instance.GetItemById(dropId);
                if (drop != null && currentRun.CanLootItem(drop))
                {
                    currentRun.inventory.Add(drop);
                    currentRun.itemsLooted++;
                    Debug.Log($"[RunManager] Boss drop: {drop.nameKey}");
                }
            }
        }

        // ─────────────────────────────────────────────
        //  Random Events
        // ─────────────────────────────────────────────

        /// <summary>
        /// Handle a random event encounter. Selects a valid event for the current zone
        /// and fires the OnEventTriggered event for the UI to handle.
        /// </summary>
        private void HandleEvent()
        {
            GameManager.Instance.ChangeState(GameState.Event);

            System.Random rng = currentRun.seededRandom ?? new System.Random();
            EventData eventData = DataManager.Instance != null
                ? DataManager.Instance.GetRandomEvent(currentRun.currentBiome, rng)
                : null;

            if (eventData == null)
            {
                Debug.LogWarning("[RunManager] No valid event found. Falling back to normal round.");
                GameManager.Instance.ChangeState(GameState.InRun);
                GenerateAndPresentItem();
                return;
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("event");

            GameManager.Instance.OnEventTriggered?.Invoke(eventData);
            Debug.Log($"[RunManager] Event triggered: {eventData.titleKey} ({eventData.type})");

            // Event resolution is handled by the UI / event system.
            // The UI calls ResumeAfterEvent() when the event is resolved.
        }

        /// <summary>
        /// Resume the run after an event has been resolved by the UI.
        /// Returns to InRun state and advances to the next round.
        /// </summary>
        public void ResumeAfterEvent()
        {
            GameManager.Instance.ChangeState(GameState.InRun);

            if (!CheckDeathConditions())
            {
                currentItem = null;
                NextRound();
            }
        }

        // ─────────────────────────────────────────────
        //  Death Conditions
        // ─────────────────────────────────────────────

        /// <summary>
        /// Check all death conditions: HP <= 0, fatal curse effects, etc.
        /// If the player is dead, ends the run via GameManager.
        /// </summary>
        /// <returns>True if the player is dead and the run has ended.</returns>
        private bool CheckDeathConditions()
        {
            if (currentRun == null) return false;

            // HP check
            if (currentRun.playerHP <= 0)
            {
                currentRun.isAlive = false;
                currentRun.playerHP = 0;

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("death");

                Debug.Log("[RunManager] Player has died.");
                GameManager.Instance.EndRun("death_cause_hp");
                return true;
            }

            // Fatal anti-synergy check (double cursed items)
            var antiSynergies = SynergyCalculator.CheckAntiSynergies(currentRun.inventory);
            foreach (var anti in antiSynergies)
            {
                if (anti.isFatal)
                {
                    currentRun.isAlive = false;
                    currentRun.playerHP = 0;

                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlaySFX("death");

                    Debug.Log("[RunManager] Fatal curse combination detected.");
                    GameManager.Instance.EndRun("death_cause_curse");
                    return true;
                }
            }

            return false;
        }

        // ─────────────────────────────────────────────
        //  Timer
        // ─────────────────────────────────────────────

        /// <summary>
        /// Start the 3-second decision timer. When it expires, the current item is lost.
        /// </summary>
        private void StartDecisionTimer()
        {
            StopDecisionTimer();
            timerCoroutine = StartCoroutine(DecisionTimerCoroutine());
        }

        /// <summary>
        /// Stop the decision timer if it is currently running.
        /// </summary>
        private void StopDecisionTimer()
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
            isTimerActive = false;
            timerRemaining = 0f;
        }

        /// <summary>
        /// Coroutine that counts down the decision timer from <see cref="decisionTimerDuration"/>
        /// to zero. Plays a warning tick sound when the timer drops below the warning threshold.
        /// On expiry, processes the decision as a timeout.
        /// </summary>
        private IEnumerator DecisionTimerCoroutine()
        {
            isTimerActive = true;
            timerRemaining = decisionTimerDuration;
            bool warningPlayed = false;

            while (timerRemaining > 0f)
            {
                timerRemaining -= Time.deltaTime;

                // Play warning sound when timer is low
                if (!warningPlayed && timerRemaining <= timerWarningThreshold)
                {
                    warningPlayed = true;
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlaySFX("timer_warning");
                }

                // Play tick sound each second
                if (timerRemaining > 0f && Mathf.FloorToInt(timerRemaining) < Mathf.FloorToInt(timerRemaining + Time.deltaTime))
                {
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlaySFX("timer_tick");
                }

                yield return null;
            }

            timerRemaining = 0f;
            isTimerActive = false;

            // Time's up - process as timeout
            Debug.Log("[RunManager] Decision timer expired.");
            ProcessDecision(DecisionResult.Timeout);
        }

        // ─────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────

        /// <summary>
        /// Check if the given round number is a boss round.
        /// Bosses appear every 15 rounds: 15, 30, 45, etc.
        /// </summary>
        /// <param name="round">The round number to check.</param>
        /// <returns>True if this is a boss round.</returns>
        private bool IsBossRound(int round)
        {
            return round > 0 && round % bossRoundInterval == 0;
        }
    }
}
