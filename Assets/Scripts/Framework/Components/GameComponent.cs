using UnityEngine;

namespace Framework.Components
{
    public interface IGameComponent
    {
        void OnComponentStart();

        void OnComponentExit();

        void Tick(float deltaTime);

        void FixedTick(float deltaTime);

        bool IsEnabled { get; }
    }
}