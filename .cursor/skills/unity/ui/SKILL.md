---
name: unity_ui_systems
description: Unity UI Systems. Guidelines for UI Toolkit (Primary), UGUI fallback, and binding-first workflows.
---

# UI Implementation

## Primary: UI Toolkit
- **Binding first**: Prefer data binding for UI state whenever possible.
- **Separation**: Keep business logic in presenters/controllers and UI state in bindable models.
- **Data-driven UI**: Drive labels, thresholds, and display rules from UI/view data assets where practical.
- **Styling**: Use USS for themes and visual states.
- **Markup**: Build views with UXML and query by name in controller code only when binding is not enough.

## Secondary: UGUI (Fallback)
- Use only when a specific feature cannot be delivered cleanly with UI Toolkit.
- If UGUI is used, keep TextMeshPro as text component standard.

## Project Selection
**Project** defaults to **UI Toolkit + data binding**.
