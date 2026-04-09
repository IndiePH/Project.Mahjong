# Mahjong Data-Driven Config Schema (First Pass)

This document defines the initial data contracts for a data-driven Mahjong implementation.

## Scope

Initial ScriptableObject-oriented config set:

1. `RuleSetConfig`
2. `ScoringConfig`
3. `AiTuningConfig`

These assets are intended to remove hardcoded gameplay values from runtime logic.

---

## 1) RuleSetConfig

### Purpose

Defines structural game rules per mode/profile (2P, 3P, 4P, beginner variants).

### Suggested Asset Name Pattern

- `Rules_2P_Default`
- `Rules_3P_Default`
- `Rules_4P_Default`

### Proposed Fields

| Field | Type | Description |
|---|---|---|
| `ruleSetId` | `string` | Unique stable ID (used for save/analytics). |
| `displayName` | `string` | Human-readable ruleset name. |
| `supportedPlayerCount` | `int` | 1-4. |
| `tileSetMode` | `enum` | `Full136`, `RemoveCharacters`, `Custom`. |
| `customIncludedSuits` | `flags enum` | Used only when `Custom`. |
| `wallTileCountOverride` | `int` | `0` means auto derive; otherwise explicit wall size. |
| `allowChow` | `bool` | Enables/disables Chow calls. |
| `allowPong` | `bool` | Enables/disables Pong calls. |
| `allowKong` | `bool` | Enables/disables Kong calls. |
| `callPriority` | `enum[]` | Ordered list; default `Win > Kong > Pong > Chow`. |
| `startingHandTileCount` | `int` | Usually 13. |
| `drawPerTurn` | `int` | Usually 1. |
| `enableReactionWindow` | `bool` | Whether post-discard reaction window is active. |
| `reactionWindowMs` | `int` | Reaction duration in milliseconds. |
| `winPatternProfile` | `enum` | `Standard4Sets1Pair`, `Custom`. |
| `allowSelfDrawBonus` | `bool` | Enables self-draw scoring bonus path. |
| `notes` | `string` | Designer-facing notes. |

### Validation Rules

- `supportedPlayerCount` must be in range `[1..4]`.
- If `tileSetMode != Custom`, `customIncludedSuits` must be ignored.
- `startingHandTileCount >= 1`.
- If `enableReactionWindow == true`, then `reactionWindowMs > 0`.
- `callPriority` must contain no duplicates and always include `Win`.

---

## 2) ScoringConfig

### Purpose

Defines point values and multipliers for simplified scoring.

### Suggested Asset Name Pattern

- `Scoring_Default_Simple`
- `Scoring_Beginner`

### Proposed Fields

| Field | Type | Description |
|---|---|---|
| `scoringProfileId` | `string` | Unique stable ID. |
| `displayName` | `string` | Human-readable profile name. |
| `baseWinPoints` | `int` | Base points on valid win. |
| `selfDrawBonusPoints` | `int` | Bonus points for self-draw wins. |
| `kongBonusPoints` | `int` | Optional bonus for declared Kong. |
| `bonusPatternEntries` | `BonusPatternEntry[]` | Optional pattern bonuses. |
| `clampMinScore` | `int` | Lower bound after score calculation. |
| `clampMaxScore` | `int` | Upper bound after score calculation. |
| `roundEndOnFirstWin` | `bool` | Ends round on first valid winner. |
| `notes` | `string` | Designer-facing notes. |

### `BonusPatternEntry` (Nested)

| Field | Type | Description |
|---|---|---|
| `patternId` | `string` | Stable ID for pattern bonus. |
| `displayName` | `string` | Name shown in result UI. |
| `points` | `int` | Bonus point value. |
| `enabled` | `bool` | Toggle for live balancing. |

### Validation Rules

- `baseWinPoints > 0`.
- `clampMaxScore >= clampMinScore`.
- `bonusPatternEntries.patternId` values must be unique.

---

## 3) AiTuningConfig

### Purpose

Defines difficulty and behavior tuning without changing AI code.

### Suggested Asset Name Pattern

- `AI_Easy_Default`
- `AI_Medium_Default`
- `AI_Hard_Default`

### Proposed Fields

| Field | Type | Description |
|---|---|---|
| `aiProfileId` | `string` | Unique stable ID. |
| `displayName` | `string` | Human-readable profile name. |
| `difficulty` | `enum` | `Easy`, `Medium`, `Hard`. |
| `behaviorStyle` | `enum` | `Offensive`, `Defensive`, `Balanced`. |
| `discardRandomness` | `float` | 0-1 where 1 is fully random. |
| `offenseWeight` | `float` | Weight for advancing own hand. |
| `defenseWeight` | `float` | Weight for safe discard behavior. |
| `callAggression` | `float` | 0-1 tendency to call Pong/Chow/Kong. |
| `riskTolerance` | `float` | 0-1 tolerance for risky discards. |
| `lookaheadDepth` | `int` | Heuristic planning depth. |
| `tileTrackingEnabled` | `bool` | Enables visible discard tracking heuristics. |
| `reactionDelayMinMs` | `int` | Minimum simulated think time. |
| `reactionDelayMaxMs` | `int` | Maximum simulated think time. |
| `notes` | `string` | Designer-facing notes. |

### Validation Rules

- `discardRandomness`, `callAggression`, `riskTolerance` in range `[0..1]`.
- `reactionDelayMaxMs >= reactionDelayMinMs`.
- `lookaheadDepth >= 0`.
- Recommend `offenseWeight + defenseWeight > 0`.

---

## 4) Runtime Wiring

### Recommended Context Bundle

Create a single runtime-selected bundle asset:

- `MatchConfigSet`
  - `RuleSetConfig rules`
  - `ScoringConfig scoring`
  - `AiTuningConfig[] aiProfilesBySeatOrDifficulty`

This keeps match setup deterministic and easy to inspect.

### Suggested Runtime Flow

1. Match setup selects `MatchConfigSet`
2. Turn/rule systems consume `rules`
3. Scoring system consumes `scoring`
4. AI controllers consume `aiProfiles`
5. UI binds to derived readonly state, not directly to config mutation

---

## 5) Versioning and Migration

- Add `schemaVersion` (int) to each config asset.
- Keep migration utilities for breaking schema changes.
- Never remove a field without a migration path for existing assets.

---

## 6) Implementation Checklist

- [ ] Create C# ScriptableObject classes for the three config assets
- [ ] Add custom editor validation or runtime validator
- [ ] Create default assets for `2P`, `3P`, and `4P`
- [ ] Create default scoring profile
- [ ] Create `Easy/Medium/Hard` AI profiles
- [ ] Add tests for config validation rules
- [ ] Add tests confirming runtime consumes config values

---

## 7) Out of Scope (First Pass)

- Full Riichi yaku table schema
- Online matchmaking/network config
- Economy/live-ops remote config pipeline

