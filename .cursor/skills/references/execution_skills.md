# Execution Skills (shared reference)

## When to use which skill

| Goal | Skill |
|------|--------|
| New gameplay features, systems, or behavior from specs | **feature_implementer** |
| Fix compile or runtime errors with minimal patches | **code_fixer** |
| Improve readability/performance without changing behavior or APIs | **code_refactorer** |
| Create/modify scenes, hierarchies, component wiring (no gameplay logic) | **scene_component_builder** |
| Generate JSON specs for prefabs/scene layout (no binary assets) | **prefab_scene_generator** |

- **feature_implementer**: Design specs → new C# and integrations. High risk; approval required before apply.
- **code_fixer**: Errors only; minimal patch; preserve behavior.
- **code_refactorer**: Refactor only; preserve behavior and public APIs.
- **scene_component_builder**: Scenes, GameObjects, components, wiring. No complex gameplay logic.
- **prefab_scene_generator**: Specs only (JSON). No .prefab/.unity output or edits.

## Project conventions (all execution skills)

- **Authority**: `Assets/Design/` (GDD/NDD) is source of truth for mechanics, economy, lore. Check before implementing.
- **Stack**: Unity 6, C# 12. UGUI (Unity UI Canvas). **TextMeshPro** for all text (`TextMeshProUGUI`, `TMP_Dropdown`, `TMP_InputField`). No legacy `Text`/`Dropdown`/`InputField`.
- **Loading**: **Addressables** for runtime loads. `Resources.Load` only for `Assets/Resources/Data/*` if allowed by project.
- **Architecture**: Event Channels (ScriptableObject), Service Locator. Folders: `Assets/Systems` (reusable), `Assets/Features` (project), `Assets/Core` (shared glue). Follow `.cursor/rules`.
- **Code style**: `[SerializeField] private _camelCase`; PascalCase for types/methods; Awaitable for async. Cache components in Awake; zero GetComponent in Update.

## Common constraints (all execution skills)

- **Scope**: Change only what is required for the task. Do not modify unrelated files, systems, or assets.
- **Output**: Return **only** valid JSON as defined per skill. No extra text or comments outside the JSON.
- **Apply**: Execution skills propose changes (patches, file contents, specs). A human or another agent reviews and applies; do not assume auto-apply unless the workflow explicitly does so.

## Code-only skills (code_fixer, code_refactorer)

- Do **not** modify scenes, prefabs, ScriptableObjects, project settings, or asset import settings.
- Do **not** change public APIs (methods, properties, events, fields) unless the task explicitly requires it.
- **code_fixer**: Minimal patch to fix the error; preserve behavior.
- **code_refactorer**: Behavior and public API must remain identical; set `behavior_changed: false` or flag if unsure.
