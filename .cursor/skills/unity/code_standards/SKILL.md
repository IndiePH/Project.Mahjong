---
name: unity_code_standards
description: C# Best Practices. Enforces naming, lifecycle ordering, serialization attributes, and code documentation standards.
---

# C# & MonoBehaviour Standards

## Lifecycle Ordering
1. Serialized Fields -> 2. Private Fields -> 3. Properties -> 4. Awake/OnEnable/Start -> 5. Update/FixedUpdate/LateUpdate -> 6. OnDisable/OnDestroy.

## Serialization Attributes
- Use `[Header("Label")]` for grouping inspector fields.
- Use `[Tooltip("Info")]` for internal documentation.
- Use `[Range(min, max)]` to constrain numeric values.

## Clean Code Patterns
- Use **Pattern Matching** and **String Interpolation**.
- Use **Expression-bodied members** for simple properties.
- **XML Docs**: Provide `<summary>` for public classes and `<param>`/`<returns>` for public methods.

## Example Standard Mono
```csharp
public class LifecycleExample : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _speed = 5f;
    
    private Rigidbody _rigidbody;
    
    public float CurrentSpeed => _speed;
    
    private void Awake()
    {
        if (!TryGetComponent(out _rigidbody))
        {
             Debug.LogError("Missing RB");
        }
    }
}
```
