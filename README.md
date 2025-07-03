# Unity Multiplayer Framework

This repository contains the base framework for a multiplayer game built using **Unity** and **Unity.Netcode**. It includes core classes for managing game states, levels, player controllers, and networked pawns.

---

## Overview

### Core Concepts

- **GameManager**:  
  The singleton entry point managing global game state transitions and scenes. Handles registration and switching of `GameState`s, and listens to network events like client connect/disconnect.

- **GameState** (`ScriptableObject`):  
  Represents the global state of the game for a specific level or mode. Responsible for loading levels, managing the active `LevelState`, and updating gameplay logic.

- **LevelState** (`MonoBehaviour`):  
  Manages everything inside a particular level, including player controllers and networked entities. Registers controllers and updates them every frame.

- **PlayerController** (`MonoBehaviour`):  
  Handles player input and logic. Controls a `Pawn` instance and delegates input and updates to it.

- **Pawn** (`NetworkBehaviour`):  
  The controllable entity in the game world (e.g., character, vehicle). Supports possession by a `PlayerController`.

- **NetworkPawn** (`Pawn`):  
  Extends `Pawn` to include network ownership and spawn logic. Disables components if not owned locally.

---

## Setup Instructions

### 1. Add the GameManager

- Add the `GameManager` prefab or script to a persistent GameObject in your initial scene.
- The `GameManager` must be a singleton (will auto-destroy duplicates).
- It listens to scene load/unload events and network client connections.

### 2. Define Your GameStates

- Create subclasses of `GameState` for each game mode or level.
- Implement `LoadLevel()` to handle level loading logic.
- Override `Enter()` and `Exit()` to manage state transitions.
- Register your `GameState`s with the `GameManager` via `RegisterGameState()`.

### 3. Implement LevelState

- Create a subclass of `LevelState` for each level scene.
- Manage player controllers by registering/unregistering them.
- Handle client joins/leaves via `OnClientJoined()` and `OnClientLeft()`.
- Add your gameplay logic inside `Tick()` and `FixedTick()`.

### 4. Player Controllers

- Create subclasses of `PlayerController` that manage input and gameplay logic for a player.
- Controllers should possess a `Pawn` via the `Possess()` method.
- Override `Tick()` and `FixedTick()` to update input and actions.

### 5. Pawns and NetworkPawns

- `Pawn` is the base class for controllable game entities.
- `NetworkPawn` extends `Pawn` to handle network ownership and spawning.
- Override `DisableLocalComponents()` in `NetworkPawn` to disable local-only components for non-owned instances.
- Use `IsOwner` property to check if the pawn belongs to the local player.

---

## Networking Flow

- When a client connects, `GameManager` notifies the current `GameState`.
- `GameState` delegates to `LevelState` to handle spawning player controllers and pawns.
- Player controllers are registered in `LevelState` by client ID.
- Pawns are spawned and possessed by controllers, enabling input and gameplay logic.
- Network ownership is tracked via `NetworkPawn` and `NetworkObject`.

---

## Additional Notes

- Ensure each `NetworkPawn` prefab includes a `NetworkObject` component.
- Register all game states before calling `LoadDefaultLevel()` in `GameManager`.
- Use `IsLocal` and `IsOwner` flags to separate local player logic from remote instances.
- Extend the base classes as needed for your game-specific logic.

---

## Example Usage

```csharp
// Register your custom GameState in GameManager
GameManager.Instance.RegisterGameState(new MyCustomGameState());

// In LevelState, spawn player controller and network pawn on client join
public override void OnClientJoined(ulong clientId)
{
    var controller = Instantiate(playerControllerPrefab);
    RegisterNetworkController(clientId, controller);
    var pawn = Instantiate(networkPawnPrefab);
    controller.Possess(pawn);
}
```

For more detailed documentation, see the inline comments in each core script.







