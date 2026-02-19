# Loot or Lose -- Game Design Document

**Version:** 1.0
**Last Updated:** 2026-02-19
**Status:** Draft
**Engine:** Unity (C#)
**Platforms:** iOS, Android

---

## Table of Contents

1. [Game Overview](#1-game-overview)
2. [Core Mechanics](#2-core-mechanics)
3. [Item System](#3-item-system)
4. [Synergy & Anti-Synergy System](#4-synergy--anti-synergy-system)
5. [Dungeon Progression](#5-dungeon-progression)
6. [Boss System](#6-boss-system)
7. [Event System](#7-event-system)
8. [Characters](#8-characters)
9. [Biomes](#9-biomes)
10. [Scoring System](#10-scoring-system)
11. [Meta-Progression](#11-meta-progression)
12. [Monetization](#12-monetization)
13. [UI/UX Design](#13-uiux-design)
14. [Technical Architecture](#14-technical-architecture)
15. [No-Gos](#15-no-gos)

---

## 1. Game Overview

### Concept

**Loot or Lose** is a minimalist roguelike decision game designed for mobile. Every run is a rapid-fire sequence of binary choices: you see an item, and you have three seconds to decide -- LOOT it or LEAVE it. Build your inventory, survive traps, defeat bosses, and chase the high score. Die, and start over. There is no save point, no second chance, no undo.

### Key Details

| Field             | Value                                              |
| ----------------- | -------------------------------------------------- |
| **Title**         | Loot or Lose                                       |
| **Genre**         | Roguelike Decision Game / Hybrid-Casual             |
| **Platforms**     | iOS + Android (Unity)                              |
| **Target Audience** | Gen Z / Millennials, ages 16--30                 |
| **Session Length** | 3--5 minutes per run                              |
| **Play Mode**     | Single-player                                      |
| **Orientation**   | Portrait only, one-handed play                     |
| **Art Style**     | Dark Fantasy Pixel Art                             |
| **Monetization**  | Free-to-play, cosmetics-only, no pay-to-win        |

### Core Fantasy

The "one more run" addiction loop. Every death feels like it could have been avoided. Every new run feels like it could be *the* run. The game is trivially easy to pick up, impossible to master, and over before the player has time to put it down. That is the hook.

### Design Pillars

1. **Speed** -- No run should feel slow. Every second matters.
2. **Tension** -- The 3-second timer turns every item into a gut decision.
3. **Depth through Simplicity** -- Two choices (LOOT / LEAVE), but the consequences cascade through synergies, anti-synergies, bosses, and events.
4. **Fairness** -- No purchase gives a gameplay advantage. Skill and decision-making are the only progression that matters within a run.
5. **Replayability** -- Randomized items, events, bosses, and biomes ensure no two runs are identical.

---

## 2. Core Mechanics

### 2.1 Item Discovery

Each round presents exactly **one random item** to the player. The item appears in the center of the screen with its name, rarity color, stats, and category icon. The player must evaluate the item and make a decision before the timer expires.

The item pool is drawn from the current biome's item table, filtered by the active dungeon zone's rarity distribution (see [Section 5: Dungeon Progression](#5-dungeon-progression)).

### 2.2 Decision Timer

A **3-second countdown** is visualized as a shrinking ring around the item. The ring starts as a full circle and collapses inward. Color transitions reinforce urgency:

| Time Remaining | Ring Color | Ring Size |
| -------------- | ---------- | --------- |
| 3.0 -- 2.0s    | Green      | 100% -- 66% |
| 2.0 -- 1.0s    | Yellow     | 66% -- 33%  |
| 1.0 -- 0.0s    | Red        | 33% -- 0%   |

When the timer reaches zero, the item disappears. The player takes **no damage** from a timeout, but the item is lost forever. This creates a soft punishment: indecision costs opportunity, not health.

### 2.3 LOOT or LEAVE

The primary interaction is a horizontal swipe:

| Input               | Action   | Result                                      |
| -------------------- | -------- | ------------------------------------------- |
| **Swipe Right**      | LOOT     | Item is added to inventory (if space exists) |
| **Swipe Left**       | LEAVE    | Item is discarded, no effect                 |
| **No Input (Timeout)** | Lost   | Item disappears, no damage taken             |

If the player swipes LOOT but their inventory is full, a **drop prompt** appears. The player must select an existing item to discard before the new item is added. The timer pauses during the drop prompt.

If the looted item is a **Trap**, damage is applied immediately upon looting. There is no way to undo a LOOT action.

If the looted item is a **Consumable**, its effect is applied immediately (e.g., Health Potion restores HP instantly). The consumable does not occupy an inventory slot after use.

### 2.4 Inventory

The player has a limited number of **inventory slots** (default: 5). Inventory is displayed as a horizontal row at the bottom of the screen.

**Inventory Rules:**

- Each item occupies **1 slot** unless otherwise specified (some large items, such as heavy weapons or shields, occupy **2 slots**).
- When inventory is full, the player must **drop an existing item** to make room for a new one.
- Dropped items are gone permanently.
- Cursed items **cannot be dropped** unless the curse is removed.
- Inventory is visible at all times so the player can make informed decisions.
- Synergy indicators glow when active synergies are present (see [Section 4](#4-synergy--anti-synergy-system)).

### 2.5 Permadeath

Every run starts from scratch. There is no persistent inventory, no carry-over stats, and no mid-run saving. Death resets the player to the character select screen. The only things that persist between runs are:

- High scores and leaderboard entries
- Character unlocks
- Biome unlocks
- Achievement progress
- Cosmetic purchases
- Daily streak counter

---

## 3. Item System

The item system is the heart of the game. All items belong to one of **7 categories** and one of **4 rarities**. The MVP targets **50+ unique items** across all categories.

### 3.1 Rarities

| Rarity        | Color                          | Drop Weight (Base) | Visual Treatment                          |
| ------------- | ------------------------------ | ------------------ | ----------------------------------------- |
| **Common**    | White (`#FFFFFF`)              | 55%                | Plain border                              |
| **Uncommon**  | Green (`#0F9B0F`)              | 28%                | Subtle shimmer on border                  |
| **Rare**      | Blue (`#4361EE`)               | 13%                | Pulsing glow around item card             |
| **Legendary** | Gold (`#FFD700`)               | 4%                 | Full-card golden glow + particle effects  |

Drop weights are modified by dungeon zone (see [Section 5](#5-dungeon-progression)) and certain character abilities.

### 3.2 Category: Weapons

Weapons provide **attack power** used during boss encounters. The player's total attack is the sum of all equipped weapon values, modified by synergies.

| Item         | Rarity    | Attack | Slots | Special Effect                         |
| ------------ | --------- | ------ | ----- | -------------------------------------- |
| Rusty Sword  | Common    | 5      | 1     | None                                   |
| Wooden Bow   | Common    | 4      | 1     | None                                   |
| Dagger       | Common    | 3      | 1     | +2 attack if Rogue character           |
| Staff        | Common    | 3      | 1     | +50% potion effectiveness (Mage only)  |
| Iron Sword   | Uncommon  | 10     | 1     | None                                   |
| Longbow      | Uncommon  | 8      | 1     | None                                   |
| Twin Daggers | Uncommon  | 7      | 1     | Counts as 2 weapons for Dual Wield     |
| Battle Axe   | Uncommon  | 12     | 2     | None                                   |
| Fire Sword   | Rare      | 15     | 1     | +10 fire damage to bosses              |
| Ice Bow      | Rare      | 13     | 1     | +10 ice damage to bosses               |
| Shadow Blade | Rare      | 14     | 1     | +5 attack vs. Shadow-type bosses       |
| Holy Lance   | Legendary | 25     | 2     | +20 damage vs. Undead/Dark bosses      |
| Void Scythe  | Legendary | 22     | 1     | Drains 5 HP from boss per round        |

### 3.3 Category: Defense

Defense items reduce incoming damage from traps, events, and boss attacks.

| Item          | Rarity    | Defense | Slots | Special Effect                         |
| ------------- | --------- | ------- | ----- | -------------------------------------- |
| Wooden Shield | Common    | 3       | 1     | None                                   |
| Leather Armor | Common    | 4       | 1     | None                                   |
| Iron Helmet   | Uncommon  | 5       | 1     | None                                   |
| Iron Shield   | Uncommon  | 7       | 2     | None                                   |
| Steel Armor   | Rare      | 10      | 1     | None                                   |
| Magic Cloak   | Rare      | 8       | 1     | 20% chance to negate curse effects     |
| Dragon Shield | Legendary | 15      | 2     | Immune to fire damage                  |
| Aegis         | Legendary | 20      | 2     | Block one lethal hit per run           |

### 3.4 Category: Consumables

Consumables apply their effect **immediately upon looting** and do not occupy an inventory slot afterward.

| Item           | Rarity   | Effect                                 |
| -------------- | -------- | -------------------------------------- |
| Health Potion  | Common   | Restore 20 HP                          |
| Poison Potion  | Common   | Deal 15 damage to self (appears as loot -- risky grab) |
| Mana Potion    | Uncommon | Next item looted has +1 rarity tier    |
| Greater Health | Uncommon | Restore 40 HP                          |
| Elixir of Life | Rare     | Restore full HP                        |
| Antidote       | Uncommon | Remove all poison/curse effects        |
| Bomb           | Uncommon | Deal 30 damage to next boss            |

### 3.5 Category: Keys

Keys are used to open **Chest events** safely. Without a key, opening a chest triggers a trap. Keys occupy 1 slot each and are consumed upon use.

| Item        | Rarity   | Effect                                      |
| ----------- | -------- | ------------------------------------------- |
| Bronze Key  | Common   | Opens chest: guaranteed Uncommon+ item      |
| Silver Key  | Uncommon | Opens chest: guaranteed Rare+ item           |
| Gold Key    | Rare     | Opens chest: guaranteed Legendary item       |
| Skeleton Key| Legendary| Opens chest: choose 1 of 3 Legendary items  |

### 3.6 Category: Traps

Traps are disguised as regular loot. They deal damage or apply negative effects immediately when looted. Traps appear with slightly misleading names/icons until the player learns to recognize their visual cues.

| Item             | Rarity   | Effect                                      |
| ---------------- | -------- | ------------------------------------------- |
| Poison Needle    | Common   | Deal 10 damage + poison (5 damage/round for 3 rounds) |
| Exploding Box    | Uncommon | Deal 25 damage instantly                     |
| Teleport Trap    | Uncommon | Randomly remove 1 inventory item             |
| Mimic            | Rare     | Deal 20 damage + steal 1 random item         |
| Cursed Chest     | Rare     | Deal 15 damage + curse 1 inventory item      |

**Trap Identification:** The Rogue character can see hidden item properties, revealing traps before the LOOT/LEAVE decision. All other characters must rely on experience and visual cues (traps have a subtle red tint on their border that becomes more noticeable with play).

### 3.7 Category: Artifacts

Artifacts are **rare, powerful items** with unique passive effects. Only one artifact can be active at a time. Looting a second artifact forces the player to choose which to keep.

| Item               | Rarity    | Effect                                              |
| -------------------- | --------- | --------------------------------------------------- |
| Ring of Fortune      | Rare      | +10% chance for Rare+ items to appear               |
| Amulet of Vitality   | Rare      | +20 max HP                                          |
| Crown of Greed       | Rare      | +50% gold earned, -10 max HP                        |
| Eye of Truth         | Legendary | Reveal hidden item properties (traps, curses)        |
| Heart of the Phoenix | Legendary | Revive once with 30% HP upon death (consumed)        |
| Void Stone           | Legendary | +1 inventory slot                                   |
| Time Crystal         | Legendary | Decision timer extended to 5 seconds                 |

### 3.8 Category: Curses

Curses are negative items that can appear as loot or be applied through events. Cursed items **block inventory slots** and cannot be dropped without an Antidote or Altar event.

| Item               | Rarity   | Effect                                              |
| -------------------- | -------- | --------------------------------------------------- |
| Curse of Weakness    | Uncommon | -5 attack power (stacks)                            |
| Curse of Fragility   | Uncommon | -5 defense (stacks)                                 |
| Curse of Burden      | Rare     | Occupies 2 inventory slots, no benefit              |
| Curse of Blindness   | Rare     | Hides item stats for 5 rounds                       |
| Curse of Doom        | Legendary| Lose 5 HP per round until removed                   |

### 3.9 Item Count Summary

| Category    | Common | Uncommon | Rare | Legendary | Total |
| ----------- | ------ | -------- | ---- | --------- | ----- |
| Weapons     | 4      | 4        | 3    | 2         | 13    |
| Defense     | 2      | 2        | 2    | 2         | 8     |
| Consumables | 2      | 3        | 1    | 0         | 6     |
| Keys        | 1      | 1        | 1    | 1         | 4     |
| Traps       | 1      | 2        | 2    | 0         | 5     |
| Artifacts   | 0      | 0        | 3    | 4         | 7     |
| Curses      | 0      | 2        | 2    | 1         | 5     |
| **Total**   | **10** | **14**   | **14** | **10**  | **48+** |

Additional items will be added through biome-specific item pools and seasonal content to exceed the 50-item target.

---

## 4. Synergy & Anti-Synergy System

Synergies and anti-synergies add strategic depth to inventory management. Players must weigh the immediate value of an item against how it interacts with their existing inventory.

### 4.1 Synergies

Synergies activate automatically when the required items are present in inventory. Active synergies are indicated by a glowing border connecting the relevant items and a synergy icon in the HUD.

| Synergy Name       | Requirements                          | Bonus Effect                                |
| -------------------- | ------------------------------------- | ------------------------------------------- |
| **Armor Set**        | Shield + Armor + Helmet               | +30% total damage reduction                 |
| **Dual Wield**       | 2 or more Weapon items                | +25% total attack power                     |
| **Elemental Combo**  | 1 Fire item + 1 Ice item              | +20 elemental burst damage vs. bosses       |
| **Potion Master**    | 3+ Consumable items used in one run   | +50% effectiveness on all future potions     |
| **Key Collector**    | 3+ Key items in inventory             | Next chest event grants double loot          |
| **Shadow Arsenal**   | Shadow Blade + Magic Cloak            | +10% dodge chance (negate trap damage)       |
| **Holy Crusader**    | Holy Lance + any Shield               | +15 damage vs. Undead and Dark bosses        |
| **Scavenger**        | 4+ unique item categories in inventory| +5% chance for Legendary drops               |

### 4.2 Anti-Synergies

Anti-synergies trigger **negative effects** when incompatible items coexist in inventory. These are designed to punish greedy looting and reward careful decision-making.

| Anti-Synergy Name     | Trigger                              | Negative Effect                             |
| ----------------------- | ------------------------------------ | ------------------------------------------- |
| **Volatile Mix**        | Fire Sword + Oil Flask               | Explosion: 20 damage to player, both items destroyed |
| **Toxic Reaction**      | Poison Potion + Health Potion        | Poison amplified: 25 damage instead of 15    |
| **Curse Amplification** | 2+ Curse items in inventory          | All curse effects doubled                    |
| **Overloaded**          | 3+ items of the same category        | -10% effectiveness on all items in that category |
| **Dark Resonance**      | Void Scythe + Holy Lance             | Both weapons lose 50% attack power           |

### 4.3 Synergy Discovery

Synergies are **hidden by default**. Players discover them through gameplay. Once a synergy has been triggered at least once, it appears in a "Codex" accessible from the main menu. This encourages experimentation and repeat play.

---

## 5. Dungeon Progression

Each run progresses through increasingly difficult **zones**. The zone determines the item pool, rarity distribution, event frequency, and trap danger.

### 5.1 Zone Definitions

| Zone            | Rounds   | Item Pool                    | Rarity Distribution                   | Traps/Curses | Events     |
| --------------- | -------- | ---------------------------- | ------------------------------------- | ------------ | ---------- |
| **Tutorial**    | 1--10    | Weapons, Defense, Consumables, Keys | 70% Common, 25% Uncommon, 5% Rare, 0% Legendary | None         | None       |
| **Standard**    | 11--25   | All categories               | 50% Common, 30% Uncommon, 15% Rare, 5% Legendary | Introduced   | Introduced |
| **Danger**      | 26--40   | All categories (weighted toward Rare+) | 35% Common, 30% Uncommon, 25% Rare, 10% Legendary | Frequent     | Frequent   |
| **Chaos**       | 41+      | All categories (heavily weighted) | 20% Common, 25% Uncommon, 35% Rare, 20% Legendary | Constant     | Constant   |

### 5.2 Zone Transition

When the player crosses a zone boundary, a brief visual transition plays (screen wipe with the new zone's color palette). A text banner announces the zone name. The background music shifts to a higher-intensity track.

### 5.3 Round Structure

Each round follows this sequence:

```
1. Check for Boss encounter (every 15th round)
2. Check for Event trigger (15% chance in Standard, 25% in Danger, 35% in Chaos)
3. If no Boss or Event: present a random Item
4. Player makes LOOT / LEAVE / Timeout decision
5. Apply item effects (damage, healing, synergy checks)
6. Advance round counter
7. Apply per-round effects (poison ticks, curse damage, etc.)
8. Check for death (HP <= 0)
```

### 5.4 Difficulty Scaling

Beyond zone-based rarity shifts, the following values scale with round number:

| Parameter            | Scaling Formula                            |
| -------------------- | ------------------------------------------ |
| Trap Damage          | `baseDamage + (round * 0.5)`               |
| Boss HP              | `baseHP + (round * scalingFactor)`         |
| Event Trigger Chance | Increases per zone (see table above)       |
| Curse Frequency      | +2% per round after round 25              |

---

## 6. Boss System

Bosses are the primary skill check in Loot or Lose. They appear every **15 rounds** (rounds 15, 30, 45, 60, etc.) and are resolved through **auto-combat** based on the player's inventory.

### 6.1 Auto-Combat Resolution

When a boss encounter triggers, the game enters a cinematic combat sequence. The player does not control the fight directly. Instead, the outcome is determined by:

1. **Player Attack** = Sum of all weapon attack values + synergy bonuses + elemental modifiers
2. **Player Defense** = Sum of all defense values + synergy bonuses
3. **Boss HP** = `baseHP + (round * scalingFactor)`
4. **Boss Attack** = `baseAttack + (round * 0.8)`

**Combat Formula:**

```
playerDamagePerTurn = playerAttack - (bossDefense * 0.3)
bossDamagePerTurn   = bossAttack - (playerDefense * 0.5)

// Combat runs for up to 10 turns
// If boss HP reaches 0: player wins
// If player HP reaches 0: permadeath
// If 10 turns pass: boss wins (player takes remaining boss attack as damage)
```

Elemental weaknesses/resistances apply a **1.5x / 0.5x** multiplier to relevant damage types.

### 6.2 Boss Types

| Boss              | Biome       | Base HP | Base Attack | Weakness       | Resistance     | Special Ability                        |
| ----------------- | ----------- | ------- | ----------- | -------------- | -------------- | -------------------------------------- |
| **Skeleton King**  | Crypt       | 80      | 12          | Light items    | Dark items     | Summons skeletons (+5 HP per turn)     |
| **Fire Dragon**    | Volcano     | 120     | 18          | Ice items      | Fire items     | Fire breath (AoE, ignores 50% defense) |
| **Ice Queen**      | Ice Palace  | 100     | 15          | Fire items     | Ice items      | Freezes 1 random item (disabled for fight) |
| **Abyss Lord**     | Abyss       | 150     | 20          | Holy items     | Poison items   | Corrupts 1 item into a Curse           |
| **Shadow Reaper**  | Any biome   | 100     | 22          | Light items    | Physical items | Phases through 50% of physical attacks |

### 6.3 Boss HP Scaling

| Boss             | Scaling Factor | HP at Round 15 | HP at Round 30 | HP at Round 45 |
| ---------------- | -------------- | -------------- | -------------- | -------------- |
| Skeleton King    | 3.0            | 125            | 170            | 215            |
| Fire Dragon      | 3.5            | 172            | 225            | 277            |
| Ice Queen        | 3.0            | 145            | 190            | 235            |
| Abyss Lord       | 4.0            | 210            | 270            | 330            |
| Shadow Reaper    | 3.5            | 152            | 205            | 257            |

### 6.4 Boss Rewards

Defeating a boss grants:

- **Guaranteed Rare+ item drop** (50% Rare, 35% Legendary, 15% Artifact)
- **500 score points**
- **Full HP restore** (25% of max HP)
- **Gold reward** scaled to boss difficulty

If the player loses to a boss, the run ends (permadeath).

---

## 7. Event System

Events break up the item-decision loop with special encounters. They add variety, risk, and reward. Events are triggered randomly between item rounds based on the current zone's event chance.

### 7.1 Event Definitions

#### Event 1: Merchant

> *"A hooded figure offers you a trade..."*

- **Mechanic:** The player selects 1 item from their inventory to trade. The Merchant offers 1 random item of **equal or higher rarity** in return.
- **Risk:** The replacement item is random -- it may not synergize with the current build.
- **Merchant Character Bonus:** The Merchant character sees 3 offered items and chooses 1.

#### Event 2: Altar

> *"A glowing altar pulses with energy..."*

- **Mechanic:** The player sacrifices 1 inventory item. In return, they receive a **permanent buff** for the remainder of the run.
- **Buff Pool:**

| Sacrifice Rarity | Possible Buffs                                     |
| ----------------- | -------------------------------------------------- |
| Common            | +5 HP, +2 Attack, +2 Defense                      |
| Uncommon          | +10 HP, +5 Attack, +5 Defense, +1 Inventory Slot  |
| Rare              | +20 HP, +10 Attack, +10 Defense                   |
| Legendary         | +30 HP, +15 Attack, +15 Defense, Remove all Curses |

#### Event 3: Chest

> *"A locked chest sits in the corner..."*

- **Mechanic:** If the player has a Key, it is consumed and the chest opens with loot based on key type (see [Keys](#35-category-keys)). If the player has no key, they can choose to force-open the chest: **80% chance of 20 damage** (trap), 20% chance of a Common item.
- **Decision:** Open without key (risky) or walk away.

#### Event 4: Curse

> *"Dark energy surrounds your belongings..."*

- **Mechanic:** A random item in the player's inventory becomes **cursed**. Cursed items cannot be dropped and may have reduced stats or negative effects.
- **Prevention:** The Magic Cloak has a 20% chance to negate this event entirely.

#### Event 5: Wheel of Fortune

> *"A mystical wheel spins before you..."*

- **Mechanic:** The player taps to stop the wheel. Outcomes are weighted as follows:

| Outcome      | Chance | Effect                                |
| ------------ | ------ | ------------------------------------- |
| Item Drop    | 40%    | Random item (rarity based on zone)    |
| Buff         | 20%    | +10 HP or +5 Attack or +5 Defense     |
| Gold         | 20%    | 50--200 gold                          |
| Nothing      | 10%    | "Better luck next time"               |
| Damage       | 10%    | 15--25 damage to player               |

#### Event 6: Trap Room

> *"The ground crumbles beneath you..."*

- **Mechanic:** The player takes **15--30 damage** (random). Defense reduces the damage taken.
- **Damage Formula:** `actualDamage = max(5, randomDamage - (totalDefense * 0.4))`

#### Event 7: Healer

> *"A wandering healer offers aid..."*

- **Mechanic:** The player's HP is restored by **30 HP** at no cost.
- **Cap:** HP cannot exceed maximum HP.

#### Event 8: Blacksmith

> *"A blacksmith examines your gear..."*

- **Mechanic:** The player selects 1 item from their inventory. The Blacksmith **upgrades its stats by 50%** (rounded up). Attack, Defense, or effect values are all eligible for upgrade.
- **Restriction:** Each item can only be upgraded once. Consumables and Keys cannot be upgraded.

### 7.2 Event Frequency by Zone

| Zone       | Event Chance per Round | Available Events                                |
| ---------- | ---------------------- | ----------------------------------------------- |
| Tutorial   | 0%                     | None                                            |
| Standard   | 15%                    | Merchant, Chest, Healer, Wheel of Fortune       |
| Danger     | 25%                    | All 8 events                                    |
| Chaos      | 35%                    | All 8 events (weighted toward Curse, Trap Room) |

---

## 8. Characters

Four playable characters provide distinct playstyles and encourage replayability. Each character has unique base stats, a starting item, and a passive ability.

### 8.1 Character Roster

| Character    | HP  | Slots | Starting Item | Passive Ability                            | Unlock Condition         |
| ------------ | --- | ----- | ------------- | ------------------------------------------ | ------------------------ |
| **Warrior**  | 100 | 5     | Rusty Sword   | None (balanced baseline)                   | Default (unlocked)       |
| **Rogue**    | 80  | 6     | None          | Sees hidden item properties (reveals traps/curses before decision) | Complete 10 runs         |
| **Mage**     | 80  | 5     | Staff         | +50% potion/consumable effectiveness       | Defeat 5 bosses          |
| **Merchant** | 100 | 5     | Gold Key      | Merchant events offer 3 choices instead of 1; +20% gold from all sources | Loot 100 total items (across all runs) |

### 8.2 Character Design Notes

**Warrior** -- The default character. No gimmicks, no tricks. The Warrior is the measuring stick against which all other characters are balanced. New players learn the game with the Warrior's straightforward stats.

**Rogue** -- Trades 20 HP for 1 extra inventory slot and the ability to see through traps and curses. The Rogue rewards cautious, informed play. The extra slot enables more synergy combinations. The lower HP punishes reckless looting.

**Mage** -- Starts with a Staff and gains massive value from consumables. The +50% potion effectiveness turns a 20 HP Health Potion into a 30 HP heal. The Mage excels in sustain-heavy runs and synergizes strongly with the Potion Master synergy.

**Merchant** -- Starts with a Gold Key, guaranteeing an early Legendary if a Chest event appears. The improved Merchant event transforms a risky trade into a calculated decision. The Merchant rewards players who have learned the item pool well enough to evaluate trades.

### 8.3 Unlock Progression

Unlock conditions are tracked across all runs. Progress persists through permadeath.

```
Run 1:    Warrior available
Run 10:   Rogue unlocked    (if 10 runs completed)
Run ~15+: Mage unlocked     (if 5 bosses defeated across all runs)
Run ~20+: Merchant unlocked (if 100 items looted across all runs)
```

---

## 9. Biomes

Biomes determine the visual theme, background music, ambient effects, item pool weighting, and boss encounter for a run. The player selects a biome before starting a run.

### 9.1 Biome Details

| Biome          | Theme        | Item Pool Bias                | Boss           | Unlock Condition                         |
| -------------- | ------------ | ----------------------------- | -------------- | ---------------------------------------- |
| **Crypt**      | Undead/Gothic | Balanced (no bias)            | Skeleton King  | Default (unlocked)                       |
| **Volcano**    | Fire/Lava    | +20% Weapons, +15% Traps     | Fire Dragon    | Reach round 30 in any biome             |
| **Ice Palace** | Ice/Crystal  | +20% Defense, +15% Consumables | Ice Queen     | Defeat the Skeleton King                |
| **Abyss**      | Dark/Void    | +20% Artifacts, +15% Curses  | Abyss Lord     | Score 5000+ in all other biomes         |

### 9.2 Biome Visual Identity

| Biome       | Background Colors       | Accent Color | Ambient Effects              | Music Mood               |
| ----------- | ----------------------- | ------------ | ---------------------------- | ------------------------ |
| Crypt       | Dark gray, moss green   | Bone white   | Flickering torches, dust     | Eerie, slow strings      |
| Volcano     | Deep red, molten orange | Ember yellow | Rising heat waves, embers    | Aggressive, percussion   |
| Ice Palace  | Frost blue, silver      | Crystal cyan | Falling snowflakes, mist     | Ethereal, chimes         |
| Abyss       | Deep purple, void black | Neon violet  | Floating particles, distortion | Ominous, deep synths    |

### 9.3 Biome-Specific Items

Each biome introduces **2--3 exclusive items** that only appear in that biome's item pool:

- **Crypt:** Bone Shield (Defense, Common), Necromancer's Staff (Weapon, Rare), Soul Lantern (Artifact, Legendary)
- **Volcano:** Magma Blade (Weapon, Rare), Fireproof Vest (Defense, Uncommon), Oil Flask (Consumable, Common)
- **Ice Palace:** Frost Dagger (Weapon, Uncommon), Ice Armor (Defense, Rare), Permafrost Amulet (Artifact, Legendary)
- **Abyss:** Void Blade (Weapon, Rare), Abyssal Ward (Defense, Rare), Chaos Orb (Artifact, Legendary)

---

## 10. Scoring System

Scoring provides the competitive backbone of Loot or Lose. Scores are displayed on the death screen and recorded on leaderboards.

### 10.1 Score Components

| Component             | Points                                    | Description                                |
| --------------------- | ----------------------------------------- | ------------------------------------------ |
| **Rounds Survived**   | 10 per round                              | Base score, always awarded                 |
| **Bosses Defeated**   | 500 per boss                              | Major score milestone                      |
| **Active Synergies**  | 100 per active synergy at run end         | Rewards strategic inventory building       |
| **Inventory Rarity**  | Per item in final inventory (see below)   | Rewards holding high-rarity items          |
| **Streak Multiplier** | Applied to total score                    | Rewards daily engagement                   |

### 10.2 Rarity Score Bonus

| Rarity    | Points per Item |
| --------- | --------------- |
| Common    | 10              |
| Uncommon  | 25              |
| Rare      | 50              |
| Legendary | 100             |

The rarity bonus is calculated from the **items in inventory at the time of death** (or run completion). This incentivizes holding onto high-rarity items even when they may not be immediately useful.

### 10.3 Streak Multiplier

Playing on consecutive days builds a daily streak. The streak multiplier applies to the **final total score**.

```
multiplier = 1.0 + (daily_streak * 0.1)
maximum multiplier = 2.0x (at 10-day streak)
```

| Streak Days | Multiplier |
| ----------- | ---------- |
| 0 (no streak) | 1.0x    |
| 1           | 1.1x       |
| 2           | 1.2x       |
| 3           | 1.3x       |
| 5           | 1.5x       |
| 10+         | 2.0x (cap) |

Missing a day resets the streak to 0.

### 10.4 Score Example

A sample run:

```
Rounds survived:    35 rounds    = 350 points
Bosses defeated:    2 bosses     = 1000 points
Active synergies:   Dual Wield   = 100 points
Inventory:          1 Legendary (100) + 2 Rare (100) + 2 Uncommon (50) = 250 points
Subtotal:                        = 1700 points
Streak multiplier:  Day 3        = x1.3
FINAL SCORE:                     = 2210 points
```

---

## 11. Meta-Progression

Meta-progression is everything that persists between runs. It provides long-term goals without granting in-run power advantages (respecting the permadeath contract).

### 11.1 Character Unlocks

See [Section 8: Characters](#8-characters) for full details.

| Character  | Unlock Condition              |
| ---------- | ----------------------------- |
| Warrior    | Default                       |
| Rogue      | Complete 10 runs              |
| Mage       | Defeat 5 bosses (cumulative)  |
| Merchant   | Loot 100 items (cumulative)   |

### 11.2 Biome Unlocks

See [Section 9: Biomes](#9-biomes) for full details.

| Biome       | Unlock Condition                          |
| ----------- | ----------------------------------------- |
| Crypt       | Default                                   |
| Volcano     | Reach round 30 in any biome              |
| Ice Palace  | Defeat the Skeleton King                  |
| Abyss       | Score 5000+ in Crypt, Volcano, and Ice Palace |

### 11.3 Achievements

Achievements track lifetime accomplishments and are displayed in the player profile. Target: **20+ achievements** at launch.

| Achievement            | Condition                                | Reward              |
| ---------------------- | ---------------------------------------- | ------------------- |
| First Blood            | Defeat your first boss                   | Profile badge       |
| Survivor               | Reach round 30                           | Profile badge       |
| Chaos Walker           | Reach the Chaos Zone (round 41+)         | Profile badge       |
| Full Set               | Activate the Armor Set synergy           | Profile badge       |
| Dual Wielder           | Activate the Dual Wield synergy          | Profile badge       |
| Alchemist              | Use 10 consumables in a single run       | Profile badge       |
| Trap Dodger            | Complete a run without looting a trap    | Profile badge       |
| Curse Breaker          | Remove 5 curses in a single run          | Profile badge       |
| Boss Slayer            | Defeat all 5 boss types                  | Title: "Slayer"     |
| Legendary Hoarder      | Hold 3+ Legendary items at once          | Title: "Legendary"  |
| Speed Demon            | Complete 20 rounds in under 60 seconds   | Title: "Swift"      |
| Pacifist               | Reach round 20 without looting any weapon| Title: "Pacifist"   |
| Lucky                  | Win the Wheel of Fortune 5 times in one run | Profile badge    |
| Blacksmith's Friend    | Upgrade 3 items in a single run          | Profile badge       |
| Key Master             | Open 3 chests with keys in a single run  | Title: "Key Master" |
| Streak Lord            | Maintain a 10-day daily streak           | Title: "Devoted"    |
| Millionaire            | Accumulate 10,000 gold (lifetime)        | Gold border profile  |
| Undying                | Survive a boss fight with 1 HP remaining | Title: "Undying"    |
| Collector              | Discover all items in the Codex          | Title: "Collector"  |
| All Characters         | Unlock all 4 characters                  | Profile badge       |

### 11.4 Daily Run

Each day, a **fixed-seed run** is available. All players who attempt the Daily Run face the exact same sequence of items, events, and bosses. Results are posted to a **global leaderboard**.

- Resets at 00:00 UTC daily.
- One attempt per day.
- Uses the player's selected character (not fixed).
- Separate leaderboard from standard runs.

### 11.5 Streak System

The daily play streak is tracked and provides a score multiplier (see [Section 10.3](#103-streak-multiplier)). The streak counter is displayed on the main menu.

### 11.6 Unlock Track

As the player accumulates total runs, items looted, and bosses defeated, new content is unlocked through a linear **unlock track** displayed on the main menu:

```
Run 1:      Tutorial tips enabled
Run 5:      Synergy Codex unlocked
Run 10:     Rogue character unlocked
Run 15:     Daily Run mode unlocked
Run 20:     Volcano biome hint
Run 25:     Wheel of Fortune event added
Run 30+:    Continued item/event/biome unlocks
```

---

## 12. Monetization

Loot or Lose follows a **fair monetization model**. No purchase provides a gameplay advantage. All monetization is cosmetic or convenience-based.

### 12.1 Revenue Streams

| Revenue Stream         | Type                  | Description                                                    | Price Range     |
| ---------------------- | --------------------- | -------------------------------------------------------------- | --------------- |
| **Character Skins**    | Premium Currency      | Alternate visual appearances for characters (no stat changes)  | 200--500 gems   |
| **Biome Packs**        | Premium Currency      | Exclusive visual themes for biomes (same gameplay balance)      | 500--800 gems   |
| **Daily Run Modifiers**| Premium Currency      | Fun gameplay modifiers for Daily Runs (see below)               | 100 gems each   |
| **Rewarded Ads**       | Ad-supported          | Watch an ad for 1 extra life per run (optional, max 1 per run) | Free (ad view)  |
| **Ad-Free Premium**    | One-time Purchase     | Remove all ads permanently                                     | $4.99 USD       |
| **Season Pass**        | Seasonal Purchase     | Exclusive cosmetic rewards over a 30-day season                 | $2.99 USD       |

### 12.2 Premium Currency (Gems)

Gems are earned through gameplay and can be purchased with real money.

**Free Gem Sources:**

| Source               | Amount     | Frequency        |
| -------------------- | ---------- | ---------------- |
| Daily login          | 10 gems    | Daily            |
| Achievement unlock   | 25--100 gems | One-time       |
| Boss defeat          | 10 gems    | Per boss         |
| Daily Run completion | 20 gems    | Daily            |
| 7-day streak         | 50 gems    | Weekly (if streak held) |

**Gem Purchase Packs:**

| Pack       | Gems   | Price     | Bonus  |
| ---------- | ------ | --------- | ------ |
| Starter    | 100    | $0.99     | --     |
| Standard   | 550    | $4.99     | +10%   |
| Premium    | 1200   | $9.99     | +20%   |
| Ultimate   | 3000   | $19.99    | +50%   |

### 12.3 Daily Run Modifiers

Optional modifiers that change the Daily Run experience. These do not affect the standard leaderboard -- modified runs appear on a separate leaderboard.

| Modifier        | Effect                                  | Cost     |
| --------------- | --------------------------------------- | -------- |
| Double Loot     | 2 items appear per round (choose 1)     | 100 gems |
| No Timer        | Decision timer disabled                 | 100 gems |
| Cursed Run      | All items have a 30% curse chance, 2x score | 100 gems |
| Glass Cannon    | 1 HP, 3x attack                        | 100 gems |
| Treasure Hunt   | Keys appear 3x more often              | 100 gems |

### 12.4 Season Pass

Each season lasts **30 days** and includes a free track and a premium track.

**Free Track:** Basic cosmetic rewards (color swaps, simple badges).

**Premium Track ($2.99):** Exclusive character skins, biome themes, profile borders, animated item effects. All rewards are cosmetic.

### 12.5 Monetization Principles

1. **No pay-to-win.** No purchase affects gameplay stats, item drops, or run outcomes.
2. **No loot boxes with real money.** All purchases deliver exactly what is advertised.
3. **Rewarded ads are optional.** The extra life is a convenience, not a necessity. The game is fully playable and completable without ads.
4. **Fair gem economy.** Free gem income is sufficient to purchase 1--2 cosmetic items per month through regular play.

---

## 13. UI/UX Design

### 13.1 Art Style

**Dark Fantasy Pixel Art** -- clean, modern, and readable. Inspired by the visual language of *Dead Cells* and *Slay the Spire* but adapted for mobile's smaller screens and portrait orientation.

Key art principles:

- **Readability first.** Every item, stat, and UI element must be immediately legible on a 5-inch screen.
- **Color-coded rarity.** Rarity colors are the primary visual language for item evaluation.
- **Minimal clutter.** The screen shows only what is needed for the current decision.
- **Consistent iconography.** Each item category has a distinct silhouette and icon shape.

### 13.2 Color Palette

| Element            | Color Code  | Usage                                |
| ------------------- | ----------- | ------------------------------------ |
| Background Primary  | `#1A1A2E`   | Main game background                 |
| Background Secondary| `#16213E`   | Panel backgrounds, overlays          |
| UI Accent           | `#00F5D4`   | Buttons, highlights, active states   |
| Text Primary        | `#FFFFFF`   | Main text                            |
| Text Secondary      | `#A0A0B0`   | Descriptions, secondary info         |
| HP Bar              | `#E63946`   | Health bar fill                      |
| HP Bar Background   | `#3A0A0F`   | Health bar empty state               |
| Gold                | `#FFD700`   | Gold counter, Legendary items        |
| Danger              | `#FF4444`   | Damage numbers, warnings             |
| Success             | `#00FF88`   | Healing numbers, positive effects    |

### 13.3 Screen Layout

The game uses **portrait mode exclusively**, designed for comfortable one-handed play.

```
+------------------------------------------+
|  [HP BAR]          Round: 27    Gold: 150 |  <- Top Bar
|  ████████░░░░      Danger Zone            |
+------------------------------------------+
|                                           |
|                                           |
|            +------------------+           |
|            |                  |           |
|            |   [ITEM CARD]    |           |  <- Center Area
|            |   Fire Sword     |           |
|            |   ATK: 15        |           |
|            |   ★ Rare         |           |
|            +------------------+           |
|               (  Timer Ring  )            |
|                                           |
|    <-- LEAVE         LOOT -->             |  <- Swipe Hint
|                                           |
+------------------------------------------+
|  [slot1] [slot2] [slot3] [slot4] [slot5]  |  <- Inventory Bar
|  Sword   Shield  Potion  Key     ---      |
+------------------------------------------+
|         [Synergy: Dual Wield]             |  <- Synergy Indicator
+------------------------------------------+
```

### 13.4 Key Screens

| Screen            | Purpose                                              |
| ----------------- | ---------------------------------------------------- |
| **Main Menu**     | Play, Daily Run, Character Select, Biome Select, Settings, Codex |
| **Character Select** | Choose character, view stats and passive ability    |
| **Biome Select**  | Choose biome, view visual preview and item pool bias |
| **Game Screen**   | Main gameplay loop (layout above)                    |
| **Boss Screen**   | Auto-combat sequence with HP bars and attack animations |
| **Event Screen**  | Event-specific UI (Merchant trade, Wheel spin, etc.) |
| **Death Screen**  | Score breakdown, high score comparison, retry button |
| **Codex**         | Discovered items, synergies, boss bestiary            |
| **Leaderboard**   | Global and friends scores, Daily Run standings        |
| **Settings**      | Sound, language, account, notifications               |

### 13.5 Animations

| Animation            | Trigger                    | Duration | Description                                    |
| -------------------- | -------------------------- | -------- | ---------------------------------------------- |
| Item Reveal          | New round starts           | 0.3s     | Item card bounces in from top with slight rotation |
| Rarity Flash         | Item appears               | 0.2s     | Quick flash of rarity color across the card border |
| Swipe Feedback       | LOOT or LEAVE              | 0.2s     | Card slides off-screen in swipe direction with trail effect |
| Timer Tick           | Each second                | 0.1s     | Pulse on timer ring border                     |
| Timeout              | Timer expires              | 0.4s     | Item fades and crumbles into particles         |
| Damage Flash         | Player takes damage        | 0.15s    | Screen flashes red, HP bar decreases with shake |
| Heal Pulse           | Player heals               | 0.3s     | Green pulse radiates from HP bar               |
| Boss Entrance        | Boss round starts          | 1.5s     | Screen darkens, boss silhouette appears, name reveal |
| Boss Attack          | During auto-combat         | 0.3s     | Attack animation with hit spark                |
| Boss Defeat          | Boss HP reaches 0          | 1.0s     | Boss disintegrates, loot drops from above      |
| Death Sequence       | Player HP reaches 0        | 2.0s     | Screen cracks, fragments fall away, score reveals |
| Synergy Activation   | Synergy conditions met     | 0.5s     | Connected items glow, synergy name appears      |
| Zone Transition      | Crossing zone boundary     | 1.0s     | Color wipe, zone name banner                   |

### 13.6 Haptic Feedback

| Event           | Haptic Type      | Platform     |
| --------------- | ---------------- | ------------ |
| LOOT swipe      | Light tap        | iOS + Android |
| LEAVE swipe     | Light tap        | iOS + Android |
| Trap damage     | Heavy impact     | iOS + Android |
| Boss hit        | Medium impact    | iOS + Android |
| Legendary drop  | Success pattern  | iOS + Android |
| Death           | Heavy impact x3  | iOS + Android |

### 13.7 Sound Design

| Sound Category  | Examples                                    |
| --------------- | ------------------------------------------- |
| Item Reveal     | Soft chime (pitch varies by rarity)         |
| LOOT            | Satisfying "grab" sound                     |
| LEAVE           | Soft whoosh                                 |
| Timer Warning   | Heartbeat (accelerating)                    |
| Trap Trigger    | Sharp metallic snap + pain grunt            |
| Boss Music      | Unique boss theme per boss type             |
| Death           | Low reverberating tone, glass shatter       |
| Ambient         | Biome-specific ambient loops                |

---

## 14. Technical Architecture

### 14.1 Technology Stack

| Component        | Technology                                   |
| ---------------- | -------------------------------------------- |
| Game Engine      | Unity 2022 LTS (C#)                         |
| Backend          | Firebase (see below)                         |
| Analytics        | Firebase Analytics + custom events           |
| Crash Reporting  | Firebase Crashlytics                         |
| CI/CD            | Unity Cloud Build + Fastlane                 |
| Version Control  | Git (GitHub)                                 |
| Asset Pipeline   | Aseprite (pixel art) + FMOD (audio)          |

### 14.2 Firebase Services

| Service              | Usage                                              |
| -------------------- | -------------------------------------------------- |
| **Firebase Auth**    | Anonymous auth (default) + optional Google/Apple sign-in for cloud save |
| **Cloud Firestore**  | Player profiles, scores, leaderboards, daily run seeds, achievement tracking |
| **Cloud Functions**  | Daily run seed generation, leaderboard validation, anti-cheat score verification |
| **Remote Config**    | Item balance tuning, event probabilities, feature flags, A/B testing |
| **Firebase Analytics** | Session tracking, funnel analysis, retention metrics, monetization events |
| **Crashlytics**      | Crash reporting, ANR detection, performance monitoring |
| **Cloud Messaging**  | Push notifications (daily reminder, streak at risk, new season) |

### 14.3 Offline-First Architecture

Core gameplay runs entirely on the client with no server dependency. This ensures:

- **Zero-latency gameplay.** No waiting for server responses during a run.
- **Full offline play.** Players can complete runs without internet access.
- **Sync on reconnect.** Scores, achievements, and progression sync to Firestore when connectivity is restored.

**Data Flow:**

```
[Local Game State] --> [Local Storage (PlayerPrefs / SQLite)]
                           |
                           v (when online)
                   [Firebase Firestore]
                           |
                           v
                   [Global Leaderboard]
```

### 14.4 Cloud Save System

- **Anonymous Auth** is created on first launch -- no login required.
- Players can optionally link their account to Google or Apple for cross-device sync.
- Save data includes: character unlocks, biome unlocks, achievement progress, Codex discoveries, cosmetic purchases, high scores, daily streak.
- Run-in-progress is **never saved** (permadeath contract).

### 14.5 Performance Targets

| Metric            | Target                                      |
| ----------------- | ------------------------------------------- |
| Frame Rate        | 60 FPS stable                               |
| Minimum Device    | iPhone SE 2nd Gen / Android Snapdragon 600+ |
| App Size          | Under 100 MB (initial download)             |
| RAM Usage         | Under 200 MB                                |
| Battery Impact    | Low (no persistent GPS, camera, or heavy compute) |
| Load Time         | Under 3 seconds from launch to main menu    |
| Network Usage     | Minimal (sync only, no streaming)           |

### 14.6 Localization

| Priority | Languages      | Status      |
| -------- | -------------- | ----------- |
| MVP      | English (EN)   | Required    |
| MVP      | German (DE)    | Required    |
| Post-MVP | French (FR)    | Nice-to-have |
| Post-MVP | Spanish (ES)   | Nice-to-have |

All user-facing strings are externalized into localization files. Unity's localization package is used for runtime language switching. Right-to-left (RTL) languages are out of scope for MVP.

### 14.7 Anti-Cheat

Since game logic runs client-side, anti-cheat measures focus on **score validation**:

1. **Run Replay Hash:** Each run generates a deterministic hash of all decisions. Cloud Functions can replay the hash to verify the reported score.
2. **Statistical Outlier Detection:** Scores that deviate significantly from population norms are flagged for review.
3. **Device Fingerprinting:** Multiple accounts from the same device are flagged.
4. **Leaderboard Moderation:** Top 100 scores are manually reviewed for anomalies.

---

## 15. No-Gos

The following features and patterns are explicitly excluded from the game design. These are non-negotiable boundaries.

| No-Go                              | Rationale                                              |
| ---------------------------------- | ------------------------------------------------------ |
| **No deck-building**               | The game is about instant decisions, not deck construction. |
| **No crafting system**             | Crafting slows the session loop. Items are looted, not built. |
| **No PvP**                         | The game is a solo experience. Competition is via leaderboards only. |
| **No landscape mode**              | Portrait-only ensures consistent one-handed play design. |
| **No pay-to-win**                  | No purchase affects gameplay stats, item drops, or run outcomes. |
| **No loot boxes with real money**  | All purchases are transparent. No gambling mechanics.   |
| **No forced social login**         | Anonymous auth by default. Social login is optional.     |
| **No 20-screen tutorial**          | The Tutorial Zone (rounds 1--10) teaches through play, not text walls. |
| **No server-side game logic**      | Core gameplay is client-side only for offline play and zero latency. |
| **No energy system**               | Players can play as many runs as they want, whenever they want. |
| **No mandatory ads**               | Rewarded ads are optional. No interstitials, no forced views. |
| **No data harvesting**             | Minimal data collection. Analytics are for game improvement only. |

---

*End of Game Design Document.*
