# Mahjong Prototype - 10 Round Playtest Checklist

Use this checklist to validate the current prototype loop across 10 complete rounds.

## Session Metadata

- Tester: Xent
- Date: 04/09/2026
- Build/Branch: main
- Ruleset (`2P/3P/4P`): Rules_4P_Default
- Seat setup (Human/AI): Human, AI, AI, AI
- Notes:

## Pass Criteria

- No blockers/crashes in all 10 rounds
- Core loop completes: setup -> deal -> turn flow -> round end
- Round end reason is always valid (`Win` or `WallExhausted` or expected test limit)
- No impossible call arbitration outcomes
- Logs and state counts remain mathematically consistent

## Per-Round Checklist (Repeat x10)

For each round, verify all items and mark pass/fail.

### Round Record Template

- Round #: ___
- Round initialized without error
- Wall built with expected tile count for current ruleset
- Starting hands dealt correctly (expected tiles per seat)
- Turn order rotated correctly
- Draw/discard counts remained consistent
- Calls resolved with correct priority (`Win > Kong > Pong > Chow`)
- Reaction window behavior matched config (`opened/accepted/timedOut/immediate`)
- Hand validation behaved correctly (no false positive wins observed)
- Round ended with valid reason and winner state
- No exceptions/errors in Console for this round
- Notes:

---

## 10-Round Summary Table


| Round | Init OK | Deal OK | Turn Flow OK | Calls/Priority OK | End State OK | Console Clean | Result |
| ----- | ------- | ------- | ------------ | ----------------- | ------------ | ------------- | ------ |
| 1     |         |         |              |                   |              |               |        |
| 2     |         |         |              |                   |              |               |        |
| 3     |         |         |              |                   |              |               |        |
| 4     |         |         |              |                   |              |               |        |
| 5     |         |         |              |                   |              |               |        |
| 6     |         |         |              |                   |              |               |        |
| 7     |         |         |              |                   |              |               |        |
| 8     |         |         |              |                   |              |               |        |
| 9     |         |         |              |                   |              |               |        |
| 10    |         |         |              |                   |              |               |        |


## Findings

### Critical

- None

### Major

- None

### Minor

- None

## Follow-up Actions

- Open tasks for all Critical findings
- Open tasks for all Major findings
- Schedule fix verification run
- Re-run failed rounds after fixes

