using UnityEngine;

namespace Player
{
    /**
     * Forms the base class for all player controllers. All player controllers in a given scene must be
     * registered on the level state instance. A player controller is responsible for handling input and
     * managing the player state and any other player related logic. It delegates input to the controlled pawn
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
    */

    public abstract class PlayerController : MonoBehaviour
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