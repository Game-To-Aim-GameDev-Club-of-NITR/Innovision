using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Framework
{
    /**
     * Represents the central in-scene logic controller for a level. Each level must contain
     * exactly one LevelState, which is responsible for initializing, updating, and shutting
     * down gameplay logic during that level.
     *
     * Responsibilities:
     * - Acts as the runtime representation of a level tied to a GameState.
     * - Manages all local and networked player controllers active in the scene.
     * - Handles per-frame (`Tick`) and fixed-frame (`FixedTick`) updates for active controllers.
     * - Provides extension points for multiplayer-aware logic (`OnClientJoined`, `OnClientLeft`).
     *
     * Usage:
     * - Attach one subclass of LevelState to a GameObject in the level scene.
     * - GameManager will discover and assign it to the active GameState on scene load.
     * - Register player controllers via index (offline) or clientId (networked).
     * - Override `OnStart` and `OnExit` to implement level-specific initialization/cleanup.
     *
     * Integration Flow:
     * GameState.LoadLevel() ->
     * Unity Scene loads ->
     * LevelState.Awake() ->
     * GameManager.OnLevelStateFound() ->
     * GameState.AssignLevelState() ->
     * LevelState.OnStart()
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: 26-06-2025
     * Modified By: Prayas Bharadwaj
     */


    public abstract class LevelState : MonoBehaviour
    {
        public string LevelName { get; private set; }

        public GameState OwnerState { get; private set; }

        public Dictionary<int, PlayerController> PlayerControllers { get; private set; } = new();
        public Dictionary<ulong, PlayerController> NetworkControllers { get; private set; } = new();

        public virtual void Initialize(GameState ownerState, string levelName)
        {
            OwnerState = ownerState;
            LevelName = levelName;
        }

        protected virtual void Awake()
        {
            GameManager.Instance?.OnLevelStateFound(this);
        }

        public abstract void OnStart();

        public abstract void OnExit();

        public virtual void Tick(float deltaTime)
        {
            foreach (var controller in GetAllActiveControllers())
            {
                controller.Tick(deltaTime);
            }
        }

        public virtual void FixedTick(float fixedDeltaTime)
        {
            foreach (var controller in GetAllActiveControllers())
            {
                controller.FixedTick(fixedDeltaTime);
            }
        }

        public virtual void RegisterController(int index, PlayerController controller)
        {
            if (PlayerControllers.ContainsKey(index))
            {
                Debug.LogWarning($"Controller with index '{index}' already exists and will be overwritten");
            }
            PlayerControllers[index] = controller;
        }

        public virtual void UnregisterController(int index)
        {
            PlayerControllers.Remove(index);
        }

        public virtual PlayerController GetController(int index)
        {
            return PlayerControllers.GetValueOrDefault(index);
        }

        public virtual IReadOnlyDictionary<int, PlayerController> GetAllControllers()
        {
            return PlayerControllers;
        }

        public virtual void RegisterNetworkController(ulong clientId, PlayerController controller)
        {
            if (NetworkControllers.ContainsKey(clientId))
            {
                Debug.LogWarning($"Network controller for client {clientId} already exists and will be overwritten.");
            }
            NetworkControllers[clientId] = controller;
        }

        public virtual void UnregisterNetworkController(ulong clientId)
        {
            NetworkControllers.Remove(clientId);
        }

        public virtual PlayerController GetNetworkController(ulong clientId)
        {
            return NetworkControllers.GetValueOrDefault(clientId);
        }

        public virtual void OnClientJoined(ulong clientId)
        {
            Debug.Log($"Client {clientId} joined level '{LevelName}'");
            // Subclasses can override to spawn controller or pawn
        }

        public virtual void OnClientLeft(ulong clientId)
        {
            Debug.Log($"Client {clientId} left level '{LevelName}'");
            UnregisterNetworkController(clientId);
        }

        public IEnumerable<PlayerController> GetAllActiveControllers()
        {
            foreach (var c in PlayerControllers.Values)
                if (c.IsActive) yield return c;

            foreach (var c in NetworkControllers.Values)
                if (c.IsActive) yield return c;
        }

    }
}