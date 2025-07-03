using Framework.Components;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    /**
     * Abstract superclass for all controllable pawns.
     *
     * Pawns are entities that can be possessed and controlled by a PlayerController or AIController.
     * They serve as the primary gameplay actors in the world, such as characters, vehicles, drones,
     * or any entity with input-driven behavior. The pawn handles activation, possession logic,
     * and simulation ticks for both game and physics updates.
     *
     * This base class can be extended for both offline and network-aware pawns. For networked gameplay,
     * consider extending NetworkPawn instead.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: 27-06-2025
     * Modified By: Prayas Bharadwaj
     */
    [RequireComponent(typeof(AnimationComponent))]
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(InputComponent))]
    [RequireComponent(typeof(MovementComponent))]
    public abstract class Pawn : NetworkBehaviour
    {
        public bool IsPossessed { get; private set; }

        public bool IsLocal()
        {
            return Owner ? Owner.isLocalPlayer : false;
        }

        protected PlayerController Owner { get; private set; }

        public PlayerController GetController() => Owner;

        public virtual void OnStart()
        {
        }

        public virtual void OnExit()
        {
            if (IsPossessed)
            {
                OnUnPossessed();
            }
        }

        public virtual void OnPossess(PlayerController owner)
        {
            IsPossessed = true;
            Owner = owner;
            gameObject.SetActive(true);
            OnBecamePossessed();
        }

        public virtual void OnUnPossessed()
        {
            IsPossessed = false;
            Owner = null;
            gameObject.SetActive(false);
            OnBecameUnPossessed();
        }

        public virtual void Tick(float deltaTime)
        {
        }

        public virtual void FixedTick(float fixedDeltaTime)
        {
        }

        public virtual void OnBecamePossessed()
        {
        }

        public virtual void OnBecameUnPossessed()
        {
        }
    }
}