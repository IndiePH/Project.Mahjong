---
name: unity_input_system
description: Unity Input System. Configuration and implementation of Actions, PlayerInput, and Rebinding.
---

# New Input System Implementation

## Action Map Structure
- `Gameplay`: Move (Vector2), Jump (Button), Fire (Button).
- `UI`: Navigate, Submit, Cancel.

## Implementation Methods
### Generated C# Class (Preferred)
```csharp
private PlayerInputActions _inputActions;
private void Awake() => _inputActions = new PlayerInputActions();
private void OnEnable() {
    _inputActions.Gameplay.Jump.performed += OnJump;
    _inputActions.Gameplay.Enable();
}
```

## Runtime Rebinding
Use `InputActionRebindingExtensions.RebindingOperation` for interactive user rebinding.
