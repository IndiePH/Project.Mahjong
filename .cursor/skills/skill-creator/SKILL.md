---
name: skill-creator
description: >
  Manual-only. Use only when the user explicitly asks to create or edit a skill.
  Do not select for normal requests. Skill Development Guide for creating,
  iterating, and packaging new AI skills.
license: Complete terms in LICENSE.txt
---

# Skill Creator

Use this only when the user explicitly asks to create/edit a skill.

## Core Principles
- Keep context lean: include only non-obvious, task-critical guidance.
- Put trigger criteria in frontmatter `description`; body is loaded after trigger.
- Keep SKILL body focused on workflow; move heavy details to `references/`.
- Prefer reusable `scripts/` for deterministic repeated logic.
- Keep references one level deep and linked directly from `SKILL.md`.

## Skill Anatomy
- Required: `SKILL.md` with YAML frontmatter (`name`, `description`) + body.
- Optional:
  - `scripts/` for executable utilities
  - `references/` for on-demand docs
  - `assets/` for output resources/templates
- Avoid extra docs (README/changelog/install notes) unless strictly needed.

## Build Workflow
1. Clarify usage examples and trigger phrases.
2. Plan reusable assets/scripts/references from those examples.
3. Initialize skill:
```bash
scripts/init_skill.py <skill-name> --path <output-directory>
```
4. Implement resources and write concise `SKILL.md`.
5. Validate/package:
```bash
scripts/package_skill.py <path/to/skill-folder>
```
Optional output dir:
```bash
scripts/package_skill.py <path/to/skill-folder> ./dist
```
6. Iterate from real usage feedback.

## Authoring Notes
- Write instructions in imperative form.
- Keep core procedure in `SKILL.md`; variant-specific details in `references/`.
- Test added scripts before packaging.
- Use `references/workflows.md` and `references/output-patterns.md` for proven patterns.
