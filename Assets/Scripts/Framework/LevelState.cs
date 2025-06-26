using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Framework
{
    /**
     * Forms the core state of each level. Everything that happens inside a level must be encapsulated
     * and controlled through a level state instance. Each level must only have a single level state instance.
     * All player controllers must be registered directly on the level state instance.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
    */

    public abstract class LevelState : MonoBehaviour
    {
        public string LevelName { get; private set; }

        public GameState OwnerState { get; private set; }

        public Dictionary<int, PlayerController> PlayerControllers { get; private set; } = new();

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
            foreach (var kv in PlayerControllers)
            {
                var controller = kv.Value;
                if (controller.IsActive)
                {
                    controller.Tick(deltaTime);
                }
            }
        }

        public virtual void FixedTick(float fixedDeltaTime)
        {
            foreach (var kv in PlayerControllers)
            {
                var controller = kv.Value;
                if (controller.IsActive)
                {
                    controller.FixedTick(fixedDeltaTime);
                }
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
    }
}