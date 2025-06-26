using UnityEngine;

namespace Framework.Components
{
    public class BaseComponent : MonoBehaviour, IGameComponent
    {
        public bool IsEnabled { get; private set; } = true;

        protected virtual void Awake()
        {
        }

        protected virtual void Start() => OnComponentStart();

        public virtual void OnComponentStart()
        {
            // Initialization logic can be added here
        }

        public virtual void OnComponentExit()
        {
            // Cleanup logic can be added here
        }

        public virtual void Tick(float deltaTime)
        {
            // Update logic can be added here
        }

        public virtual void FixedTick(float fixedDeltaTime)
        {
            // Fixed update logic can be added here
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
        }
    }
}