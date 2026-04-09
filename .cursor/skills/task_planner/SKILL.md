---
name: task_planner
description: >
  Meta task-planning assistant for Unity game development. Use when a clear
  goal has been identified and you need to break it into ordered steps.
---

# Task Planner Skill (Meta)

Purpose: turn a clear goal into ordered, skill-level steps.
Planner only; no execution.

## Responsibilities
- Build minimal ordered steps.
- Use exactly one skill per step.
- Encode dependencies via order and `purpose`.
- Consult `.cursor/skills/references/meta_consultation.md` before planning.
- Split gameplay implementation and scene setup into separate steps when both are needed.
- Support hybrid routing from orchestrator:
  - Simple tasks -> shortest valid step list (often 1-2 steps).
  - Complex tasks -> multi-step plan with explicit dependencies and optional parallel branches.

## Hard Constraints
- No code/asset edits or task execution.
- Never assign multiple skills in one step.
- Route new feature work to `feature_implementer`.
- Route scene/hierarchy/component wiring to `scene_component_builder`.
- Never assign `skill-creator` (manual-only).

## Required JSON Output

Return only this JSON shape (no extra text):

```json
{
  "steps": [
    {
      "step": 1,
      "skill": "",
      "purpose": ""
    }
  ]
}
```

## Operational Algorithm

1. Consult meta skills per `meta_consultation.md`.
2. Identify required skills and remove already-completed work.
3. If task is simple, emit the shortest valid plan.
4. If task is complex, emit full ordered steps with clear dependency purpose.
5. Return JSON only.

