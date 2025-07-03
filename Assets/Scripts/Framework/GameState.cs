using System;
using UnityEngine;

namespace Framework
{
    /**
     * Represents the core abstract state of a level. All level-specific logic, including
     * startup, shutdown, time tracking, and per-frame updates, must be managed through
     * a subclass of this GameState.
     *
     * Responsibilities:
     * - Manages the lifecycle of a level via `Enter()`, `Exit()`, and `LoadLevel()`.
     * - Owns a reference to a `LevelState` MonoBehaviour, which handles in-scene logic.
     * - Delegates per-frame and fixed-frame updates to the associated LevelState.
     * - Handles scene load/unload and multiplayer client events (if applicable).
     *
     * Usage:
     * - Subclass this ScriptableObject to define a specific game mode or level.
     * - Set a unique `Tag` in the subclass constructor to register it in GameManager.
     * - Implement `LoadLevel()` to load the associated Unity scene.
     * - Override `OnPlayerConnected()` / `OnPlayerDisconnected()` in multiplayer contexts.
     *
     * Lifecycle Flow:
     * GameManager -> SetGameState(tag) ->
     * GameState.Enter() ->
     * LoadLevel() ->
     * OnSceneLoaded() ->
     * AssignLevelState() ->
     * LevelState.OnStart()
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: 25-06-2025
     * Modified By: Prayas Bharadwaj
     */

    public abstract class GameState : ScriptableObject
    {
        public string Tag { get; protected set; }
        protected LevelState LevelState { get; set; }
        protected bool IsActive { get; set; }
        protected float ElapsedTime { get; set; }
        protected string LevelName { get; set; }

        public bool IsGameStateActive() => IsActive;

        protected GameState(string gsTag)
        { 
            Tag = gsTag;
        }

        public abstract void LoadLevel();

        public void SetLevelName(string levelName)
        {
            LevelName = levelName;
        }


        public virtual void Enter()
        {
            if (!IsActive) IsActive = !IsActive;
        }

        public virtual void Exit()
        {
            if (IsActive) IsActive = !IsActive;
            Debug.Log("Not implemented. Define a subclass to override");
        }

        public virtual void Tick(float deltaTime)
        {
            LevelState?.Tick(deltaTime);
            if (IsActive) ElapsedTime += deltaTime;
        }

        public virtual void FixedTick(float fixedDeltaTime)
        {
            LevelState?.FixedTick(fixedDeltaTime);
        }

        public virtual void OnSceneLoaded(string sceneName)
        {
            Debug.Log($"Scene '{sceneName}' loaded in GameState '{Tag}'.");
        }

        public virtual void OnSceneUnloaded(string sceneName)
        {
            Debug.Log($"Scene '{sceneName}' unloaded in GameState '{Tag}'.");
        }

        public void AssignLevelState(LevelState levelState)
        {
            LevelState = levelState;
            LevelState.Initialize(this, LevelName);
            LevelState.OnStart();
        }

        public virtual void OnPlayerConnected(ulong clientId)
        {
            Debug.Log($"Client {clientId} connected to GameState '{Tag}'");
            // Subclasses should override this to spawn players, assign controllers, etc.
        }

        public virtual void OnPlayerDisconnected(ulong clientId)
        {
            Debug.Log($"Client {clientId} disconnected from GameState '{Tag}'");
            // Subclasses should override this to clean up players, controllers, etc.
        }
    }
}