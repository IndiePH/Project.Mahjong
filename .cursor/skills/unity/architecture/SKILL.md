---
name: unity_architecture_patterns
description: Unity Architecture Patterns. Implements Event Channels, Service Locators, State Machines, and Command patterns.
---

# Unity Architecture Patterns

## Event Channels (ScriptableObjects)
Standard for decoupled communication. 
```csharp
[CreateAssetMenu(menuName = "Events/Int Event")]
public class IntEventChannel : ScriptableObject {
    private event Action<int> _onEvent;
    public void Raise(int val) => _onEvent?.Invoke(val);
    public void Subscribe(Action<int> action) => _onEvent += action;
    public void Unsubscribe(Action<int> action) => _onEvent -= action;
}
```

## Dependency Injection (Service Locator)
Use a persistent `ServiceLocator` to register and retrieve cross-system managers (e.g., `AudioManager`).

## State Machine
Use a generic `StateMachine<T>` with `IState` interfaces for characters or game flow management.

## Command Pattern
Use `ICommand` with a `CommandManager` for any system requiring undo/redo or input record/playback.
