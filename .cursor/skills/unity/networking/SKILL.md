---
name: unity_networking
description: Unity Multiplayer Utility. Standardizes Netcode for GameObjects (NGO), NetworkVariables, and RPCs.
---

# Netcode Rules

## NetworkVariables
```csharp
private readonly NetworkVariable<int> _health = new NetworkVariable<int>(
    100, 
    NetworkVariableReadPermission.Everyone, 
    NetworkVariableWritePermission.Server
);
```

## RPC Security
- **ServerRpc**: Use for client requests. **Always** validate on the server.
- **ClientRpc**: Use for server-to-client notifications.

## Lifecycle
Override `OnNetworkSpawn` for initialization and `OnNetworkDespawn` for cleanup.
