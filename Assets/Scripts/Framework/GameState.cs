using System;
using UnityEngine;

namespace Framework
{
    /**
     * Forms the core game state of each level. Everything that happens inside a level must be
     * handled through a game state instance. As such, it is necessary that all levels must have
     * their own subclass of GameState
     * It is also the responsibility of the game state to manage all startup and shutdown operations
     * via LoadLevel() as well as the Enter() and Exit() methods.
     * All updates must be handled through the Tick() method
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
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
    }
}