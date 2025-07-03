using Unity.Netcode;
using UnityEngine;

namespace Player
{
    /**
     * Forms the base class for all player controllers.
     *
     * A PlayerController acts as the interface between the player (or AI) and the Pawn they control.
     * It is responsible for receiving input, managing player-specific logic, and directing behavior
     * through the controlled Pawn instance.
     *
     * All PlayerControllers must be registered on the LevelState instance for the current level.
     * Supports lifecycle methods such as Possess(), UnPossess(), Tick(), and FixedTick() for
     * update-driven control logic.
     *
     * Extend this class to implement custom local or network-based player control.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: 27-06-2025
     * Modified By: Prayas Bharadwaj
     */

    public abstract class PlayerController : NetworkBehaviour
    {
        [SerializeField] public bool isLocalPlayer;
        public Pawn ControlledPawn { get; protected set; }
        public bool IsActive { get; set; }

        public virtual void Possess(Pawn pawn)
        {
            ControlledPawn = pawn;
            IsActive = true;
            ControlledPawn.OnPossess(this);
        }

        public virtual void OnStart()
        {

        }

        public virtual void OnExit()
        {
            UnPossess();
        }

        public virtual void UnPossess()
        {
            IsActive = false;
            ControlledPawn.OnUnPossessed();
            ControlledPawn = null;
        }
        public virtual void Tick(float deltaTime)
        {
            ControlledPawn?.Tick(deltaTime);
        }

        public virtual void FixedTick(float fixedDeltaTime)
        {
            ControlledPawn?.FixedTick(fixedDeltaTime);
        }


    }
}