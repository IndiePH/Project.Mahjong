# Mahjong Game GDD (1-4 Players)

## 1. Overview

A digital Mahjong game supporting 1 to 4 players (human and/or AI), based primarily on simplified Riichi/Hong Kong hybrid rules.  
The design prioritizes accessibility, scalability, and clean architecture for future feature expansion.

## 2. Goals

- Support flexible player count (1-4 players)
- Maintain core Mahjong identity (hand building, discard strategy)
- Provide smooth onboarding for beginners
- Allow scalable AI difficulty
- Keep implementation modular and testable

## 3. Core Gameplay Loop

1. Setup wall and shuffle tiles
2. Deal starting hands
3. Player turn loop:
   - Draw tile
   - Evaluate hand
   - Discard tile
   - Other players may react (Pong/Chow/Kong/Win)
4. Repeat until:
   - A player wins
   - Draw condition (wall exhausted)

## 4. Game Modes

### 4.1 Single Player

- Player vs AI (1-3 bots)
- Adjustable difficulty
- Optional hints/tutorial

### 4.2 Local Multiplayer

- 2-4 players on one device (pass-and-play)

### 4.3 Online Multiplayer (Future)

- Matchmaking or private rooms

## 5. Player Count Rules

### 5.1 Four Players (Standard)

- Full tile set (136 tiles)
- Standard rules

### 5.2 Three Players

- Remove one suit (example: Characters)
- Faster rounds
- Increased draw/win rate

### 5.3 Two Players

- Remove one or two suits
- Reduced wall size
- Optional rule tweaks:
  - Limited calls (for example, no Chow)
  - Faster win condition

### 5.4 One Player

- AI opponents fill remaining seats

## 6. Tile System

### 6.1 Tile Types

- Suits: Dots, Bamboo, Characters
- Honors: Winds, Dragons

### 6.2 Tile Data Model (Example)

- TileId
- SuitType
- Rank
- IsHonor

## 7. Hand Rules

### 7.1 Winning Condition

- 4 sets (triplet or sequence)
- 1 pair

### 7.2 Calls

- Chow (sequence, restricted)
- Pong (triplet)
- Kong (quad)

### 7.3 Turn Priority

- Win > Kong > Pong > Chow

## 8. AI Design

### 8.1 Difficulty Levels

- Easy: Random valid play
- Medium: Basic hand-building logic
- Hard: Tile tracking + probability weighting

### 8.2 AI Behaviors

- Offensive (fast win)
- Defensive (safe discards)
- Balanced

## 9. UI/UX

### 9.1 Core Elements

- Player hand (bottom)
- Opponent hands (hidden, top/side)
- Discard pile (center)
- Wall indicator

### 9.2 Interaction

- Tap-to-select, tap-to-discard (drag support optional)
- Context call buttons (Pong/Chow/Kong/Win)

### 9.3 Visual Feedback

- Highlight valid actions
- Animate tile draws/discards
- Clear turn and reaction-window indicators

### 9.4 UI Technology Direction

- Primary UI stack: Unity UI Toolkit
- Prefer data binding for UI state synchronization whenever practical
- Keep UGUI/TextMeshPro as fallback-only for unsupported cases

## 10. Architecture (Unity-Focused)

### 10.1 Core Systems

- GameManager
- TurnManager
- TileManager
- PlayerController
- AIController
- RuleSetStrategy (config-driven rule variants)

### 10.2 Decoupling Strategy

- ScriptableObject event channels for cross-system messaging
- Composition Root / Simple Injector (`GameContext`) for explicit dependency wiring
- No global service locator usage

### 10.3 Example Events

- OnTileDrawn
- OnTileDiscarded
- OnCallDeclared
- OnWin
- OnRoundDraw

### 10.4 Data-Driven Development Model

- Data-driven first: core gameplay behavior should be configured from data assets where practical.
- Rule presets (2P/3P/4P), scoring values, and AI tuning should live in ScriptableObject/config assets.
- Avoid embedding balance/rule constants directly in gameplay scripts unless there is a strong technical reason.

## 11. Data Flow

1. TurnManager triggers turn start
2. PlayerController requests tile from TileManager
3. TileManager emits draw event
4. Active player updates hand
5. Active player discards tile
6. Discard event broadcast to other players
7. Reaction window opens by priority

## 12. Scoring System (Simplified)

- Base win: fixed points
- Bonuses:
  - Self draw
  - Optional special patterns (future extension)

## 13. Progression (Optional)

- Unlockable themes
- AI difficulty unlock progression
- Stats tracking and match history

## 14. MVP Scope

- 4-player offline (1 human + 3 AI minimum supported)
- Basic win condition (4 sets + 1 pair)
- Simplified scoring
- Clear, readable UI using UI Toolkit
- Core event-driven architecture

## 15. Future Features

- Online multiplayer
- Full Riichi rules (Yaku, Dora)
- Replay system
- Spectator mode
- Seasonal events/challenges

## 16. Risks

- Rule complexity may overwhelm beginners
- AI tuning can create frustration or low challenge
- UI clarity may degrade on small screens
- Scope creep from full-rules requests too early

## 17. Implementation Notes

- Start with a 2-player prototype for faster iteration
- Keep rules modular with strategy-based rule evaluators
- Keep gameplay rules completely separated from UI
- Use ScriptableObjects for config-driven rule presets and balance values
- Keep AI thresholds, scoring modifiers, and rule toggles editable via data assets
- Build deterministic test cases for draw/discard/call priority and win checks

## 18. Summary

This design provides a scalable Mahjong implementation that supports multiple player counts while maintaining core gameplay identity.  
The architecture focuses on modularity and explicit dependency flow, enabling gradual expansion from a clean MVP to a full-featured Mahjong experience.

