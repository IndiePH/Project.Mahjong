---
name: unity_ui_systems
description: Unity UI Systems. Guidelines for UGUI (Primary) and UI Toolkit with TextMeshPro standards.
---

# UI Implementation

## Primary: Unity UI Canvas (UGUI)
- **TextMeshPro**: User `TextMeshProUGUI` instead of `Text`.
- **Optimization**: Turn off **Raycast Target** on non-interactive elements.

## Secondary: UI Toolkit
- **Separation**: Split Business Logic (Controller) from UI Logic (View).
- **Styling**: Use USS for themes.

## Project Selection
**Project** defaults to **Unity UI Canvas / UGUI**.
