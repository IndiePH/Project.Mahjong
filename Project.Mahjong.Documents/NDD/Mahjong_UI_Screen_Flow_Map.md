# Mahjong UI Screen Flow Map (UI Toolkit)

This document defines the baseline screen flow for the Mahjong prototype and MVP path.

## Scope

- UI stack: UI Toolkit
- State model: data-driven view-state payloads
- Current prototype: bootstrap simulation + HUD
- Target flow: Main Menu -> Lobby/Setup -> Gameplay -> Results

---

## 1) High-Level Flow

```text
App Launch
  -> Main Menu
      -> (Start Local Match)
          -> Lobby / Match Setup
              -> Gameplay
                  -> Results
                      -> (Rematch) -> Lobby / Match Setup
                      -> (Back to Menu) -> Main Menu
```

---

## 2) Screen Definitions

## 2.1 Main Menu

### Purpose

Entry point for users to start a match or adjust baseline settings.

### Core UI Elements

- Title/logo
- Start Match button
- Settings button (audio, language, basic options)
- Exit button (platform-dependent)

### Data Inputs

- Last used ruleset ID
- Last used seat count
- Last used AI difficulty

### Data Outputs

- Navigation intent to Lobby/Setup

---

## 2.2 Lobby / Match Setup

### Purpose

Configure match parameters before entering gameplay.

### Core UI Elements

- Ruleset dropdown (`2P/3P/4P` config assets)
- Seat count selector
- AI difficulty selector
- Start Match button
- Back button

### Data Inputs

- `MatchConfigSet` collection / available presets
- Current selected preset

### Data Outputs

- `SelectedMatchSetup` payload:
  - config set ID
  - ruleset ID
  - seat count
  - AI difficulty

---

## 2.3 Gameplay

### Purpose

Display and drive the active round/match.

### Core UI Elements (MVP baseline)

- Player hand panel
- Opponent hand panels (hidden/public rules dependent)
- Discard piles
- Wall remaining indicator
- Call action panel (Pong/Chow/Kong/Win)
- Turn state indicators

### Prototype HUD (current)

- Seat count
- Wall remaining
- Round end reason
- Winner seat
- Calls summary
- Reaction window summary
- Last call
- Discard counts
- Winning states

### Data Inputs

- Runtime round state (`RoundState`/`MatchState`)
- Derived call availability
- Timing data for reaction windows

### Data Outputs

- Player action intents (discard, call, confirm/cancel)
- Pause/menu navigation intents

---

## 2.4 Results

### Purpose

Show round or match outcomes and next actions.

### Core UI Elements

- Winner/Draw banner
- Score summary
- Breakdown (optional MVP+)
- Rematch button
- Back to Menu button

### Data Inputs

- Finalized round/match result payload

### Data Outputs

- Navigation intent: rematch or main menu

---

## 3) Navigation State Machine (Concept)

```text
MainMenu
  -> LobbySetup
LobbySetup
  -> MainMenu
  -> Gameplay
Gameplay
  -> Results
Results
  -> LobbySetup
  -> MainMenu
```

---

## 4) Data-Driven Navigation Model

Use a data-driven screen graph asset in future iterations:

- `UIScreenFlowConfig` (ScriptableObject)
  - screen IDs
  - allowed transitions
  - default entry screen
  - optional guards/requirements

This avoids hardcoded navigation branching in MonoBehaviours.

---

## 5) MVP Implementation Notes

- Keep each screen as a separate UXML tree (or modular panels) with USS styles.
- Keep view state immutable snapshots for clean binding (`Apply(state)` pattern).
- Route navigation through one coordinator (`UIScreenCoordinator`) rather than cross-calling views.
- Keep gameplay UI derived from runtime state, not ad-hoc direct mutations.

---

## 6) Verification Checklist

- [ ] Main Menu -> Lobby/Setup works
- [ ] Lobby/Setup -> Gameplay passes selected data correctly
- [ ] Gameplay -> Results triggers on valid round end
- [ ] Results rematch returns to Lobby/Setup with expected defaults
- [ ] Results back returns to Main Menu
- [ ] No dead-end screen states

