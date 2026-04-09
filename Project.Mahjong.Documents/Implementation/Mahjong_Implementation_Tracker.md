# Mahjong Implementation Tracker

This document tracks implementation progress for `Project.Mahjong` based on the GDD.  
Update this file continuously during development.

## Development Principles

- Data-driven first: prefer ScriptableObject/config assets over hardcoded gameplay values.
- UI Toolkit + data binding first for UI state synchronization.
- Composition Root/Simple Injector for explicit dependency wiring.

## Status Legend

- `[ ]` Not started
- `[-]` In progress
- `[x]` Done
- `[!]` Blocked

## Milestone Overview

| Milestone | Goal | Target | Status |
|---|---|---|---|
| M0 | Project foundation and architecture setup | TBD | [ ] |
| M1 | Core 2-player playable prototype | TBD | [ ] |
| M2 | Full 4-player offline with AI | TBD | [ ] |
| M3 | UX polish, onboarding, and balancing | TBD | [ ] |
| M4 | Post-MVP features (online/full rules) | TBD | [ ] |

---

## M0 - Foundation Setup

### Project and Architecture

- [x] Establish initial `Assets` folder structure (`Core/`, `Systems/`, `Features/`, `Design/`)
- [x] Define DI direction: composition root/simple injector (`GameContext`)
- [x] Add ScriptableObject observer/event channel base
- [x] Define data-asset schema for rules, scoring, and AI tuning
- [x] Wire `MatchConfigSet` into runtime `MatchSetupLoader`
- [x] Introduce `RoundState` + `RoundController` runtime state orchestration layer
- [ ] Run config bootstrap menu to generate default config assets
- [ ] Create initial ScriptableObject configs for player-count variants (2P/3P/4P)
- [ ] Add assembly definition files for major modules (`Core`, `Systems`, `Features`)
- [ ] Create shared error/logging conventions for runtime diagnostics
- [ ] Define save data versioning strategy

### Rules and Documentation

- [x] GDD created and saved under `Project.Mahjong.Documents/GDD`
- [x] Rules updated for UI Toolkit + binding-first approach
- [ ] Create technical design note (NDD) for turn loop and call priority
- [ ] Create coding checklist for PR reviews (tests, performance, architecture)

### UI Platform Baseline

- [x] Create UI Toolkit base documents (`MainHUD.uxml`, `MainHUD.uss`)
- [x] Create UI binding model template (view model + binder)
- [x] Define UI screen flow map (main menu, lobby, gameplay, result)
- [x] Add data-driven `UIScreenCoordinator` skeleton wired to bootstrap (`Gameplay -> Results`)
- [x] Add `UIScreenHostBinder` panel toggling for visual screen transitions in UI Toolkit
- [x] Add basic navigation button wiring for MainMenu/Lobby/Results transitions
- [x] Add tiny Lobby setup panel to drive runtime `ruleset/seatCount/difficulty` into `MatchSetupLoader.TryLoadSetup(...)`
- [x] Add Gameplay controls (`Next Turn`, `End Round`) wired to `RoundRuntimeController`

---

## M1 - 2-Player Vertical Slice (Fast Prototype)

### Core Gameplay Slice

- [x] Implement tile data model and tile definitions
- [x] Implement wall generation and shuffle
- [x] Implement initial dealing logic
- [x] Implement draw/discard turn loop (2 players)
- [x] Implement hand validation (4 sets + 1 pair)
- [x] Implement round end (win or wall exhausted)

### Calls and Priority

- [x] Implement Pong
- [x] Implement Chow (if enabled for 2-player ruleset)
- [x] Implement Kong
- [x] Implement reaction window and arbitration
- [x] Enforce priority: Win > Kong > Pong > Chow

### Minimal UI Toolkit Integration

- [ ] Player hand rendering and tile selection
- [ ] Discard pile rendering
- [ ] Turn indicator and wall count display
- [ ] Call action panel (Pong/Chow/Kong/Win)
- [ ] Bind gameplay state to UI via data binding

### Test and Validation

- [x] Add edit mode tests for hand evaluation logic
- [x] Add deterministic tests for draw/discard flow
- [x] Add deterministic tests for reaction priority resolution
- [x] Playtest checklist for 10 complete prototype rounds

---

## M2 - 4-Player Offline + AI (MVP Target)

### Multiplayer Expansion (Offline)

- [ ] Expand turn manager from 2 to 4 seats
- [ ] Handle 1-4 player seat composition (human + AI fill)
- [ ] Implement 3-player and 2-player tile-set/ruleset variants
- [ ] Implement robust round reset and next-round transitions

### AI

- [ ] Easy AI (random valid actions)
- [ ] Medium AI (basic hand-building heuristics)
- [ ] Hard AI (tile tracking + probabilistic discard)
- [ ] Add AI behavior profiles (offensive/defensive/balanced)

### Scoring and Match Flow

- [ ] Implement simplified scoring core
- [ ] Add self-draw bonus
- [ ] Add match summary/result screen
- [ ] Track per-match stats (win rate, average turns, etc.)

### MVP Readiness

- [ ] Stable 4-player offline sessions without blocking bugs
- [ ] No critical UI clarity issues in beginner playtests
- [ ] Performance target validated on minimum supported hardware

---

## M3 - Polish and Onboarding

### Onboarding and UX

- [ ] Interactive tutorial (first-time flow)
- [ ] Context hints and valid-action highlights
- [ ] Improve tile readability and table clarity
- [ ] Add quality animations for draws/discards/calls

### Balance and Tuning

- [ ] Tune AI difficulty curve
- [ ] Tune round pacing and match length
- [ ] Collect telemetry for decision hotspots (optional)
- [ ] Iterate based on playtest feedback

### Content and Progression (Optional)

- [ ] Theme unlock framework
- [ ] Difficulty unlock progression
- [ ] Persistent stats and profile summary

---

## M4 - Future Expansion

- [ ] Online multiplayer architecture spike
- [ ] Private room + matchmaking concept
- [ ] Full Riichi extension planning (Yaku, Dora)
- [ ] Replay system design
- [ ] Spectator mode design

---

## Active Sprint Board

### This Sprint Focus

- Goal:
- Start date:
- End date:

### Sprint Tasks

- [ ] Task 1
- [ ] Task 2
- [ ] Task 3

### Blockers

- None

### Notes

- N/A

---

## Change Log

| Date | Author | Update |
|---|---|---|
| 2026-04-09 | AI Agent | Initial implementation tracker created |
| 2026-04-09 | AI Agent | Added first-pass ScriptableObject config classes (`RuleSetConfig`, `ScoringConfig`, `AiTuningConfig`, `MatchConfigSet`) |
| 2026-04-09 | AI Agent | Added editor bootstrap menu to auto-generate default config assets |
| 2026-04-09 | AI Agent | Added runtime `MatchSetupLoader` to resolve `MatchConfigSet` into match-ready setup |
| 2026-04-09 | AI Agent | Added data-driven tile definitions and runtime wall building integrated into bootstrap |
| 2026-04-09 | AI Agent | Added initial dealing logic and bootstrap logging for per-seat hand sizes + remaining wall |
| 2026-04-09 | AI Agent | Added simulated draw/discard turn loop with discard + remaining wall diagnostics |
| 2026-04-09 | AI Agent | Added standard hand validator (4 melds + 1 pair) with bootstrap diagnostics per seat |
| 2026-04-09 | AI Agent | Added round-end detection (win/wall exhausted/turn limit) with winner seat reporting |
| 2026-04-09 | AI Agent | Added call detection (Win/Kong/Pong/Chow) and priority-based reaction resolution in turn simulation |
| 2026-04-09 | AI Agent | Added deterministic reaction-window simulation metrics (opened/accepted/timed out/immediate) in turn loop |
| 2026-04-09 | AI Agent | Added EditMode NUnit tests for hand validation, wall/deal flow, and reaction priority/window behavior |
| 2026-04-09 | AI Agent | Added 10-round prototype playtest checklist under `Project.Mahjong.Documents/QA` |
| 2026-04-09 | AI Agent | Added UI Toolkit prototype HUD (`MainHUD.uxml/.uss`) and runtime HUD binder wiring from bootstrap state |
| 2026-04-09 | AI Agent | Added UI screen flow map doc (Main Menu -> Lobby/Setup -> Gameplay -> Results) |
| 2026-04-09 | AI Agent | Added `RoundState` + `RoundController` and refactored bootstrap to consume orchestrated state snapshots |
| 2026-04-09 | AI Agent | Added data-driven `UIScreenFlowConfig` + `UIScreenCoordinator` and bootstrap screen transitions |
| 2026-04-09 | AI Agent | Added `UIScreenHostBinder` and panelized `MainHUD` for visible MainMenu/Lobby/Gameplay/Results transitions |
| 2026-04-09 | AI Agent | Added `UIScreenButtonsBinder` and UI Toolkit button controls for manual navigation flow testing |
| 2026-04-09 | AI Agent | Added Lobby setup controls (`ruleset/seatCount/difficulty`) and wired Begin Gameplay to runtime loader inputs |
| 2026-04-09 | AI Agent | Added lobby preflight status indicator (ready/invalid) using runtime setup validation and color-coded summary |
| 2026-04-09 | AI Agent | Added `RoundRuntimeController` and interactive Gameplay controls for incremental turn stepping |
| 2026-04-09 | AI Agent | Added HUD turn telemetry (`turns played`, `active seat`) with live updates per step |
| 2026-04-09 | AI Agent | Added clickable player hand rendering and `S0` discard action wired to `RoundRuntimeController.Discard(tileIndex)` |
| 2026-04-09 | AI Agent | Added Results panel data binding (`end reason/winner/turns/calls/windows/discards`) via `MahjongResultsBinder` |

