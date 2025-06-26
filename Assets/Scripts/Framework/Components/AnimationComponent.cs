using UnityEngine;

namespace Framework.Components
{
    /**
     * Animation component wraps around the pre-existing animation system in unity, providing for
     * a cleaner pathway to run animations and trigger events based on animation states.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
    */

    public class AnimationComponent : BaseComponent
    {
        [SerializeField]
        private Animator animator;

        [SerializeField] private bool shouldUeAnimations;

        public void SetBool(string param, bool value)
        {
            if (animator)
            {
                animator.SetBool(param, value);
            }
        }

        public void SetFloat(string param, float value)
        {
            if (animator)
            {
                animator.SetFloat(param, value);
            }
        }

        public void Trigger(string param)
        {
            if (animator)
            {
                animator.SetTrigger(param);
            }
        }

        public void ResetTrigger(string param)
        {
            if (animator)
            {
                animator.ResetTrigger(param);
            }
        }

        protected override void Start()
        {
            base.Start();
            if (!animator && shouldUeAnimations)
            {
                Debug.LogError("Animator not initialized!");
            }
        }
    }
}