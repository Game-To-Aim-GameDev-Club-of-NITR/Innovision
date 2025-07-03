using Unity.Netcode;
using UnityEngine;

namespace Player
{
    /**
     * Abstract base class for all network-aware pawns.
     *
     * Extends the base Pawn class with Unity Netcode integration. Provides built-in access to
     * the NetworkObject component and encapsulates ownership checks. This class ensures that only
     * the owning client executes local updates and logic by gating core methods like Tick() and FixedTick().
     *
     * On network spawn, it automatically initializes the pawn and disables local-only components
     * if the current client is not the owner. Extend this class for any player-controlled or replicated
     * entity that requires network authority.
     *
     * Must be attached to a GameObject with a NetworkObject component.
     *
     * Date Created: 27-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
     */
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetworkPawn : Pawn, INetworkPawn
    {
        private NetworkObject _netObject;
        public NetworkObject NetObject => _netObject ??= GetComponent<NetworkObject>();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            OnStart();
        }

        public override void OnStart()
        {
            base.OnStart();
            if (!NetObject) return;
            if (!IsOwner) DisableLocalComponents();
        }

        public override void Tick(float deltaTime)
        {
            if (!IsOwner) return;
            base.Tick(deltaTime);
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            if (!IsOwner) return;
            base.FixedTick(fixedDeltaTime);
        }

        protected abstract void DisableLocalComponents();
    }
}