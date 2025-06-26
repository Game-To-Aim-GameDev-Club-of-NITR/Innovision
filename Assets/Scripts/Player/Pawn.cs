using UnityEngine;

namespace Player
{
    /**
     * Abstract superclass for all controllable pawns. Pawns are the objects that are controlled by
     * a player controller or an AI controller. They can represent characters, vehicles, or any
     * other entity that can be controlled by a player or an AI.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
    */

    public abstract class Pawn : MonoBehaviour
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