using UnityEngine;
using UnityEngine.Events;
using LootOrLose.Enums;
using LootOrLose.State;
using LootOrLose.Data;

namespace LootOrLose.Managers
{
    /// <summary>
    /// Central game manager. Controls game state transitions and coordinates all other managers.
    /// Singleton pattern with DontDestroyOnLoad.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;

        // Runtime state
        private GameRunState currentRun;
        private PlayerProgressState playerProgress;

        // Events for UI to subscribe to
        public UnityEvent<GameState> OnStateChanged;
        public UnityEvent<GameRunState> OnRunStarted;
        public UnityEvent<RunResult> OnRunEnded;
        public UnityEvent<ItemData> OnItemPresented;
        public UnityEvent<DecisionResult, ItemData> OnDecisionMade;
        public UnityEvent<BossData> OnBossEncounter;
        public UnityEvent<EventData> OnEventTriggered;

        public GameState CurrentState => currentState;
        public GameRunState CurrentRun => currentRun;
        public PlayerProgressState PlayerProgress => playerProgress;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            OnStateChanged ??= new UnityEvent<GameState>();
            OnRunStarted ??= new UnityEvent<GameRunState>();
            OnRunEnded ??= new UnityEvent<RunResult>();
            OnItemPresented ??= new UnityEvent<ItemData>();
            OnDecisionMade ??= new UnityEvent<DecisionResult, ItemData>();
            OnBossEncounter ??= new UnityEvent<BossData>();
            OnEventTriggered ??= new UnityEvent<EventData>();
        }

        /// <summary>
        /// Transition the game to a new state and notify all listeners.
        /// </summary>
        /// <param name="newState">The target game state.</param>
        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;
            currentState = newState;
            OnStateChanged?.Invoke(newState);
            Debug.Log($"[GameManager] State changed to: {newState}");
        }

        /// <summary>
        /// Start a new run with the selected character and biome.
        /// </summary>
        /// <param name="character">The character chosen for this run.</param>
        /// <param name="biome">The biome (dungeon theme) for this run.</param>
        /// <param name="dailySeed">Optional seed for daily challenge runs.</param>
        public void StartNewRun(CharacterData character, BiomeData biome, int? dailySeed = null)
        {
            currentRun = new GameRunState(character, biome, dailySeed);
            ChangeState(GameState.InRun);
            OnRunStarted?.Invoke(currentRun);
            Debug.Log($"[GameManager] New run started: {character.nameKey} in {biome.nameKey}");
        }

        /// <summary>
        /// End the current run (death or completion).
        /// </summary>
        /// <param name="deathCause">Localization key for the cause of death, or null if completed.</param>
        public void EndRun(string deathCause)
        {
            if (currentRun == null) return;

            var result = new RunResult
            {
                finalScore = currentRun.score,
                roundsCompleted = currentRun.currentRound,
                bossesDefeated = currentRun.bossesDefeated,
                itemsLooted = currentRun.itemsLooted,
                itemsLeft = currentRun.itemsLeft,
                finalInventory = new System.Collections.Generic.List<ItemData>(currentRun.inventory),
                biome = currentRun.currentBiome,
                deathCause = deathCause,
                runDuration = Time.time - currentRun.runStartTime,
                timestamp = System.DateTime.UtcNow
            };

            ChangeState(GameState.RunSummary);
            OnRunEnded?.Invoke(result);
        }

        /// <summary>
        /// Present a new item to the player for decision.
        /// </summary>
        /// <param name="item">The item to present.</param>
        public void PresentItem(ItemData item)
        {
            OnItemPresented?.Invoke(item);
        }

        /// <summary>
        /// Process the player's decision on the current item.
        /// </summary>
        /// <param name="decision">Whether the player chose to loot, leave, or timed out.</param>
        /// <param name="item">The item the decision was made on.</param>
        public void MakeDecision(DecisionResult decision, ItemData item)
        {
            OnDecisionMade?.Invoke(decision, item);
        }

        /// <summary>
        /// Load player progress from save.
        /// </summary>
        /// <param name="progress">The deserialized player progress state.</param>
        public void LoadProgress(PlayerProgressState progress)
        {
            playerProgress = progress;
        }

        /// <summary>
        /// Save player progress.
        /// </summary>
        public void SaveProgress()
        {
            // Will be implemented by SaveManager
            Debug.Log("[GameManager] Save progress requested");
        }
    }
}
