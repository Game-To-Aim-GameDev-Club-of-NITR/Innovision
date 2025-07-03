using Unity.Netcode;
using UnityEngine;

namespace Player
{
    /**
     * Extends PlayerController to support networked gameplay using Unity Netcode.
     *
     * A NetworkPlayerController handles client-side and server-side control logic, managing
     * ownership-based execution and player synchronization. This controller is typically spawned
     * by the server and owned by the connecting client.
     *
     * Automatically disables local-only logic for non-owner clients. Must be registered with
     * the LevelState using the client's NetworkClientId.
     *
     * This controller is expected to be attached to a GameObject with a NetworkObject component.
     *
     * Extend this class to implement multiplayer-aware input handling and control flow.
     *
     * Date Created: 27-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
    */
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetworkController : PlayerController
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            isLocalPlayer = IsOwner;

            if (!IsOwner)
                DisableLocalController();
            else
                OnStart(); // Initialize local player controller
        }

        public override void Tick(float deltaTime)
        {
            if (!IsOwner || !IsActive) return;
            base.Tick(deltaTime);
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            if (!IsOwner || !IsActive) return;
            base.FixedTick(fixedDeltaTime);
        }

        protected abstract void DisableLocalController();
    }
}