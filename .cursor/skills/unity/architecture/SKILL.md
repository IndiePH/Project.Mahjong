---
name: unity_architecture_patterns
description: Unity Architecture Patterns. Implements Event Channels, Composition Root DI, State Machines, and Command patterns.
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

## Dependency Injection (Simple Injector / Composition Root)
Avoid global/static service locators. Prefer a scene-wired `GameContext` (composition root) that holds references to shared services and is passed explicitly (via serialized field or `Initialize(...)` calls). Keep injection reflection-free and easy to trace in the inspector and via call sites.

## Data-Driven Configuration
Prefer ScriptableObject/config-asset driven systems for rules, scoring, AI tuning, and balance values. Avoid scattering gameplay constants across MonoBehaviours; centralize tunables in dedicated data assets.

## State Machine
Use a generic `StateMachine<T>` with `IState` interfaces for characters or game flow management.

## Command Pattern
Use `ICommand` with a `CommandManager` for any system requiring undo/redo or input record/playback.
