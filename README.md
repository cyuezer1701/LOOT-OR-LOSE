# LOOT OR LOSE

> A minimalist roguelike decision game. Find items. Make choices. Survive.

![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-000000?logo=unity&logoColor=white)
![License](https://img.shields.io/badge/License-TBD-blue)
![CI](https://img.shields.io/github/actions/workflow/status/cyuezer1701/LOOT-OR-LOSE/ci.yml?label=CI&logo=github)
![iOS](https://img.shields.io/badge/Platform-iOS-lightgrey?logo=apple)
![Android](https://img.shields.io/badge/Platform-Android-3DDC84?logo=android&logoColor=white)

---

## The Game

**Loot or Lose** is a fast-paced roguelike decision game for mobile. Each round presents you with a random item — you have **3 seconds** to decide: **LOOT** it or **LEAVE** it. Your inventory holds only **5 items**. Choose wisely — wrong decisions mean death.

**Runs last 3-5 minutes.** Perfect for quick sessions on the go.

---

### Core Loop

```
  +------------------+
  |  An item appears  |
  +--------+---------+
           |
           v
  +------------------+
  |  3 seconds to    |
  |  decide: LOOT    |
  |  or LEAVE        |
  +--------+---------+
           |
           v
  +------------------+
  |  Manage your     |
  |  5-slot inventory|
  +--------+---------+
           |
           v
  +------------------+
  |  Discover item   |
  |  synergies &     |
  |  avoid deadly    |
  |  combos          |
  +--------+---------+
           |
           v
  +------------------+
  |  Face bosses     |
  |  every 15 rounds |
  +--------+---------+
           |
           v
  +------------------+
  |  Survive as long |
  |  as possible     |
  +--------+---------+
           |
           v
  +------------------+
  |  Die. Learn.     |
  |  Try again.      |
  +------------------+
```

1. An item appears
2. You have 3 seconds to decide: **LOOT** or **LEAVE**
3. Manage your 5-slot inventory strategically
4. Discover item synergies and avoid deadly combos
5. Face bosses every 15 rounds
6. Survive as long as possible
7. Die. Learn. Try again.

---

### Features

| Feature | Details |
|---------|---------|
| Items | 50+ unique items across 7 categories |
| Synergies | Item synergies & anti-synergies (combos can save or kill you) |
| Bosses | 5 boss types with unique weaknesses |
| Events | 8 random event types (merchants, altars, traps...) |
| Characters | 4 playable characters with unique abilities |
| Biomes | 4 themed biomes (Crypt, Volcano, Ice Palace, Abyss) |
| Daily Runs | Global leaderboards with daily seeds |
| Achievements | 20+ achievements to unlock |
| Permadeath | Every run starts fresh — no shortcuts |

---

## Screenshots

_Coming soon_

---

## Tech Stack

| Component | Technology |
|-----------|-----------|
| **Engine** | Unity 2022.3 LTS (C#) |
| **Backend** | Firebase (Auth, Firestore, Analytics, Cloud Functions) |
| **Auth** | Firebase Anonymous Auth + optional upgrade |
| **Database** | Cloud Firestore (leaderboards, cloud save) |
| **Analytics** | Firebase Analytics + Custom Events |
| **A/B Testing** | Firebase Remote Config |
| **CI/CD** | GitHub Actions + GameCI |
| **Platforms** | iOS + Android |

---

## Project Structure

```
LOOT-OR-LOSE/
|
+-- Assets/
|   +-- Scripts/
|   |   +-- Core/               Pure game logic (no Unity deps, fully testable)
|   |   +-- Data/               Serializable data models (items, bosses, events)
|   |   +-- Enums/              Game enumerations (GameState, ItemCategory, etc.)
|   |   +-- Interfaces/         Contracts and abstractions
|   |   +-- Managers/           Unity MonoBehaviour managers (singletons)
|   |   +-- Services/           Firebase, Analytics, IAP, Audio services
|   |   +-- State/              Runtime & persistent game state
|   |   +-- UI/                 UI components, screens, and HUD elements
|   |   +-- Utils/              Helper classes and extensions
|   |   +-- Config/             Configuration and settings
|   |
|   +-- Resources/              JSON data (items, bosses, events, characters, biomes)
|   +-- Art/                    Sprites, fonts, materials, shaders
|   +-- Audio/                  Music and SFX
|   +-- Scenes/                 Unity scenes (Main, Loading, etc.)
|   +-- Prefabs/                Reusable prefabs (UI, items, effects)
|   +-- Tests/
|       +-- EditMode/           Pure logic tests (Core/)
|       +-- PlayMode/           Integration tests with Unity lifecycle
|
+-- docs/                       Game design document and other docs
+-- .github/
|   +-- workflows/              CI/CD pipelines (build, test, deploy)
|
+-- Packages/                   Unity package manifest
+-- ProjectSettings/            Unity project settings
```

---

## Architecture

Loot or Lose follows a **clean architecture** approach adapted for Unity:

### Pure Logic Separation

`Assets/Scripts/Core/` contains **zero Unity dependencies**. No `MonoBehaviour`, no `UnityEngine` (except basic value types like `Vector2` if needed). This layer is fully testable with NUnit in EditMode tests — no Play mode required.

```
Core/
  +-- ItemGenerator.cs          Item spawning logic & probability
  +-- InventoryLogic.cs         Inventory management (5-slot limit)
  +-- SynergyResolver.cs        Item combo detection & effects
  +-- BossCombatLogic.cs        Boss fight resolution
  +-- ScoringLogic.cs           Score calculation & multipliers
  +-- EventResolver.cs          Random event logic
  +-- RunManager.cs             Overall run progression
```

### Manager Pattern

Singleton managers in `Assets/Scripts/Managers/` coordinate Unity-specific systems. They use `DontDestroyOnLoad` and are initialized in a boot scene.

### Observer Pattern

`UnityEvent` and custom events provide decoupled communication between systems. Managers subscribe to events rather than directly referencing each other.

### State Machine

`GameState` enum in `Enums/GameEnums.cs` drives all screen transitions and game flow:

```
Loading -> MainMenu -> RunActive -> BossFight -> GameOver -> MainMenu
                          |                         ^
                          +--- Event ----------------+
```

### Data-Driven Design

All game content is defined in JSON files under `Assets/Resources/` and loaded at runtime. Adding new items, bosses, or events requires **zero code changes** — just edit JSON.

---

## Getting Started

### Prerequisites

| Requirement | Version |
|-------------|---------|
| Unity | 2022.3 LTS or newer |
| Firebase SDK | Firebase Unity SDK |
| Git LFS | Required for binary assets |
| Platform SDKs | Xcode (iOS) / Android SDK |

### Setup

```bash
# 1. Clone the repository
git clone https://github.com/cyuezer1701/LOOT-OR-LOSE.git
cd LOOT-OR-LOSE

# 2. Initialize Git LFS
git lfs install
git lfs pull

# 3. Open in Unity Hub
#    -> Add project from disk
#    -> Select the LOOT-OR-LOSE folder

# 4. Configure Firebase
cp firebase-config.example.json firebase-config.json
# Edit firebase-config.json with your credentials

# 5. Open the main scene
#    Assets/Scenes/MainScene.unity

# 6. Press Play!
```

### Firebase Setup

1. Create a new project at [Firebase Console](https://console.firebase.google.com/)
2. Enable **Anonymous Authentication** under Authentication > Sign-in method
3. Create a **Firestore Database** (start in test mode for development)
4. Add your apps:
   - **Android**: Download `google-services.json`
   - **iOS**: Download `GoogleService-Info.plist`
5. Place both files in `Assets/StreamingAssets/`
6. Install the Firebase Unity SDK via the Unity Package Manager

---

## Testing

| Test Type | Location | What It Tests | How to Run |
|-----------|----------|---------------|------------|
| **EditMode** | `Assets/Tests/EditMode/` | Pure logic (Core/) | Test Runner > EditMode |
| **PlayMode** | `Assets/Tests/PlayMode/` | Unity lifecycle integration | Test Runner > PlayMode |
| **CI** | `.github/workflows/` | Automated on every push | Automatic via GitHub Actions |

### Running Tests Locally

1. Open Unity
2. Go to **Window > General > Test Runner**
3. Select **EditMode** or **PlayMode** tab
4. Click **Run All**

### Test Naming Convention

```
Source:  Assets/Scripts/Core/ItemGenerator.cs
Test:    Assets/Tests/EditMode/ItemGeneratorTests.cs
```

---

## CI/CD Pipeline

```
+------------------+     +------------------+     +------------------+
|   Pull Request   | --> |   Validation +   | --> |   Merge Ready    |
|                  |     |   All Tests      |     |                  |
+------------------+     +------------------+     +------------------+

+------------------+     +------------------+     +------------------+
|   Push to main   | --> |   Tests +        | --> |   Android Build  |
|   or develop     |     |   Build          |     |   + iOS Build    |
+------------------+     +------------------+     +------------------+

+------------------+     +------------------+
|   Manual Deploy  | --> |   Play Store /   |
|   (release tag)  |     |   App Store      |
+------------------+     +------------------+
```

| Trigger | Actions |
|---------|---------|
| **Pull Request** | Validation + EditMode & PlayMode tests |
| **Push to main/develop** | Tests + Android build + iOS build |
| **Release tag** | Tests + builds + Play Store / App Store submission |

---

## Game Design

The full Game Design Document covers every system in detail:

**[docs/GAME_DESIGN_DOCUMENT.md](docs/GAME_DESIGN_DOCUMENT.md)**

Includes: item tables, boss specifications, event definitions, synergy matrices, scoring formulas, biome configurations, character abilities, and balancing parameters.

---

## Roadmap

### MVP (v1.0)

- [x] Project setup & architecture
- [x] Item system (50+ items, 7 categories)
- [x] Core game loop (item generation, timer, decisions)
- [ ] Boss combat system
- [ ] Event system
- [ ] Scoring & leaderboards
- [ ] Character & biome unlocks
- [ ] Daily runs
- [ ] UI/UX polish & animations
- [ ] Sound & haptics
- [ ] Monetization (IAP + rewarded ads)
- [ ] Localization (DE + EN)
- [ ] App Store submission

### Post-Launch (v1.x)

- [ ] New biomes & bosses
- [ ] Season pass system
- [ ] Social features (friends, sharing runs)
- [ ] Ghost mode (replay other players' runs)
- [ ] Additional languages (FR, ES, JA)
- [ ] Tablet-optimized UI
- [ ] Accessibility improvements

---

## Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow the architecture patterns (pure logic in `Core/`, managers in `Managers/`)
4. Write tests for new logic (EditMode tests for `Core/`)
5. Ensure all tests pass
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Code Style

- Follow C# naming conventions (PascalCase for public, _camelCase for private fields)
- Keep `Core/` free of Unity dependencies
- All user-facing strings go through `LocalizationManager.t()`
- New game content goes in JSON files, not in code

---

## License

**TBD** — License to be determined before public release.

---

## Contact

**Project by cyuezer1701**

---

<p align="center">
  <i>Die. Learn. Loot again.</i>
</p>
