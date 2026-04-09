---
name: feature_implementer
description: >
  Gameplay Implementation AI. Translates design specifications into new C#
  scripts and system integrations.
---

# Feature Implementer Skill (Execution – High Risk)

Purpose: implement new gameplay features from explicit specs.
Risk is high; output is proposal-only and must be reviewed before apply.

## Responsibilities
- Follow `.cursor/skills/references/execution_skills.md`.
- Create new scripts when needed (`files_created`).
- Propose minimal integration edits to existing files (`files_modified`).
- Capture missing requirements in `assumptions`.
- Provide concise integration notes.

## Hard Constraints
- Keep scope minimal; no automatic application.
- No unrelated refactors.
- Do not invent balance/economy/cooldown/progression values unless specified.

## Required JSON Output

Return only this JSON shape (no extra text):

```json
{
  "files_created": [
    {
      "path": "",
      "content": ""
    }
  ],
  "files_modified": [
    {
      "path": "",
      "patch": ""
    }
  ],
  "assumptions": [],
  "integration_notes": [],
  "confidence": 0.0
}
```

## Operational Algorithm

1. Read the spec and identify impacted systems.
2. Propose minimal new files and minimal patches.
3. Record assumptions and integration notes.
4. Set confidence.
5. Return JSON only.

