---
name: unity_dots_ecs
description: Unity DOTS/ECS AI. Implements high-performance entities using ISystem, IJobEntity, and Native memory.
---

# DOTS Rules

## ISystem (Struct-based)
```csharp
[BurstCompile]
public partial struct MySystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) { }
}
```

## Data Iteration (IJobEntity)
Use `partial struct` for Jobs. Use `in` for read-only and `ref` for write access.

## Memory
Use `NativeArray<T>` with appropriate `Allocator` (Temp, TempJob, Persistent).
