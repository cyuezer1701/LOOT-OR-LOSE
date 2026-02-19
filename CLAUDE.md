# Project: Loot or Lose — Unity Mobile Game

## Architecture Overview

Unity (C#) mobile roguelike decision game with Firebase backend. Pure logic separation ensures testability. All game content is data-driven via JSON. Targets iOS and Android via Unity 2022.3 LTS.

## Directory Structure

```
Assets/
  Scripts/
    Core/              - Pure game logic (NO Unity dependencies, NO MonoBehaviour)
                         ItemGenerator, InventoryLogic, SynergyResolver,
                         BossCombatLogic, ScoringLogic, EventResolver, RunManager
    Data/              - Serializable data models (ItemData, BossData, EventData,
                         CharacterData, BiomeData) — [System.Serializable] POCOs
    Enums/             - Game enumerations (GameState, ItemCategory, ItemRarity,
                         BossType, EventType, BiomeType, SynergyType)
    Interfaces/        - Contracts (IItemProvider, IScoringStrategy, ISaveService,
                         IAnalyticsService, IAudioService)
    Managers/          - Unity MonoBehaviour singletons (DontDestroyOnLoad)
                         GameManager, UIManager, AudioManager, InputManager,
                         InventoryManager, RunController, BossManager, EventManager
    Services/          - External integrations (FirebaseService, AnalyticsService,
                         IAPService, AudioService, CloudSaveService, LeaderboardService)
    State/             - Runtime state (GameSession, PlayerProgress, InventoryState)
                         and persistent state (SaveData, SettingsData)
    UI/                - UI components, screens, HUD elements, popups
                         MainMenuUI, GameplayHUD, BossUI, GameOverUI, InventoryUI,
                         LeaderboardUI, SettingsUI, DailyRunUI
    Utils/             - Helper classes, extension methods, constants
                         MathUtils, CollectionExtensions, JsonLoader, Timer
    Config/            - ScriptableObjects and runtime configuration
                         GameConfig, BalanceConfig, AdConfig
  Resources/           - JSON data files loaded at runtime
    items.json         - All item definitions (50+ items, 7 categories)
    bosses.json        - Boss definitions (5 types with weaknesses)
    events.json        - Random event definitions (8 types)
    characters.json    - Playable character definitions (4 characters)
    biomes.json        - Biome definitions (4 biomes)
    synergies.json     - Item synergy & anti-synergy rules
    achievements.json  - Achievement definitions (20+)
    localization/      - i18n strings (de.json, en.json)
  Art/                 - Sprites, fonts, materials, shaders, animations
  Audio/               - Music tracks and sound effects
    Music/             - Background music per biome
    SFX/               - UI sounds, loot, damage, boss, events
  Scenes/              - Unity scenes (BootScene, MainScene)
  Prefabs/             - Reusable prefabs
    UI/                - UI prefabs (buttons, popups, item cards)
    Effects/           - Visual effect prefabs (particles, flashes)
  Tests/
    EditMode/          - Pure logic tests (Core/) — no Unity lifecycle needed
    PlayMode/          - Integration tests — requires Unity Play mode
docs/                  - Game design document, API docs, architecture diagrams
.github/
  workflows/           - CI/CD pipelines (test, build-android, build-ios, deploy)
```

## Key Patterns

- **Pure Logic Separation**: `Assets/Scripts/Core/` has ZERO Unity dependencies. No `MonoBehaviour`, no `UnityEngine` namespace imports (except basic value types if absolutely necessary). All logic is testable with plain NUnit in EditMode tests.
- **Manager Singletons**: `Assets/Scripts/Managers/` use `DontDestroyOnLoad` pattern. Initialized in `BootScene`. Access via `GameManager.Instance`, `UIManager.Instance`, etc.
- **Data-Driven Design**: All game content lives in JSON files under `Assets/Resources/`. Adding items, bosses, events, characters, or biomes requires zero code changes.
- **Observer Pattern**: `UnityEvent` and custom C# events for decoupled communication between managers and UI. Never directly reference other managers — subscribe to events.
- **State Machine**: `GameState` enum in `Enums/GameEnums.cs` drives all screen transitions: `Loading -> MainMenu -> RunActive -> BossFight -> GameOver -> MainMenu`
- **Serializable Data Models**: `Assets/Scripts/Data/` classes are plain C# with `[System.Serializable]` attribute. Used for JSON deserialization and runtime data.
- **Interface-Driven Services**: All external services implement interfaces from `Assets/Scripts/Interfaces/`. Enables mocking in tests and swapping implementations.
- **i18n**: All user-facing strings use `LocalizationManager.t("key")`. String keys are defined in `Assets/Resources/localization/` JSON files.

## Commands

- **Run Tests (Unity)**: Window > General > Test Runner > EditMode / PlayMode > Run All
- **Build Android**: File > Build Settings > Android > Switch Platform > Build
- **Build iOS**: File > Build Settings > iOS > Switch Platform > Build
- **CI Tests**: Triggered automatically on push/PR via GitHub Actions
- **CI Builds**: Triggered on push to `main` or `develop` branches

## Testing Conventions

- **EditMode tests** in `Assets/Tests/EditMode/` test pure logic from `Core/` only
- **PlayMode tests** in `Assets/Tests/PlayMode/` test Unity lifecycle integration
- Test files mirror source file names: `ItemGenerator.cs` -> `ItemGeneratorTests.cs`
- Use NUnit framework: `[Test]`, `[TestCase]`, `[SetUp]`, `[TearDown]`
- Assertions: `Assert.AreEqual`, `Assert.IsTrue`, `Assert.Throws<T>`
- Mock external services using interfaces — never call Firebase in tests
- Test data: Create helper factory methods for test data (e.g., `TestItemFactory.CreateSword()`)

## Rules

- **Always** keep `Core/` free of Unity dependencies — no `MonoBehaviour`, no `using UnityEngine` (basic types excepted)
- **Always** run EditMode tests after modifying any file in `Core/`
- **All** user-facing strings must use `LocalizationManager.t("key")` — never hardcode display text
- **New items, bosses, events** go in JSON files under `Assets/Resources/` — not in code
- **Follow** existing Manager patterns: singleton + `DontDestroyOnLoad` + event-driven communication
- **Never** commit `firebase-config.json` — use `firebase-config.example.json` as template
- **Never** commit `google-services.json` or `GoogleService-Info.plist`
- **New UI elements** should be prefabs in `Assets/Prefabs/UI/`
- **New services** must implement an interface from `Assets/Scripts/Interfaces/`
- **Always** start with `/pm` for any new feature or task — never invoke agents directly
- **The PM** is the single point of coordination for all work

## Teamstruktur / Agent Hierarchy

This project uses a hierarchical agent model:

```
CEO (User) — gibt Aufgaben und Anweisungen
  |
  PM (/pm) — Zentraler Orchestrator, einziger Ansprechpartner des CEO
    |
    +-- Developer (/dev) — Implementierung, Debugging, Code Review
    +-- Designer (/designer) — UI/UX Design, Styling, Accessibility
    +-- Tester (/tester) — Tests, Coverage, Qualitaetssicherung
    +-- [Dynamische Agenten nach Bedarf]
```

### Arbeitsweise

1. **CEO** gibt dem PM eine Aufgabe (z.B. "Neues Feature: Boss Combat System")
2. **PM** analysiert, zerlegt in Teilaufgaben, erstellt einen Delegationsplan
3. **PM** gibt dem CEO die genaue Reihenfolge der `/command` Aufrufe vor
4. **CEO** fuehrt die Befehle der Reihe nach aus und gibt Ergebnisse an den PM zurueck
5. **PM** prueft Ergebnisse, koordiniert Nacharbeit, meldet Abschluss an den CEO

### Agent-Kommunikation

- Nur der PM kommuniziert mit dem CEO
- Alle Agenten liefern strukturierte Reports, die der PM auswerten kann
- Agenten referenzieren die Outputs anderer Agenten via Task-IDs (z.B. "DEV-001", "TEST-001")
- Neue Spezialisten-Agenten koennen vom PM vorgeschlagen werden (siehe `_agent-template.md`)

### Typische Aufgaben pro Rolle

| Rolle | Typische Aufgaben |
|-------|------------------|
| **Developer (/dev)** | Core logic, Managers, Services, JSON data, bug fixes |
| **Designer (/designer)** | UI prefabs, sprites, animations, UX flow, accessibility |
| **Tester (/tester)** | EditMode tests, PlayMode tests, coverage reports, edge cases |
