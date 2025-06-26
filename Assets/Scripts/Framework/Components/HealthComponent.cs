using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework.Components
{
    /**
     * Health component is responsible for handling all the health related logic of the game object.
     * It provides action delegates that can be triggered when health changes and mapped to animation
     * or other game logic.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
    */
    public class HealthComponent : BaseComponent
    {
        public Action OnDamaged;
        public Action OnHeal;
        public Action OnDeath;
        public float MaxHealth { get; set; } = 100.0f;
        public float CurrentHealth { get; private set; } = 100.0f;
        public bool IsDead => CurrentHealth <= 0;

        public void TakeDamage(float damage)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
            OnDamaged?.Invoke();

            if (IsDead) Death();
        }

        public void Heal(float health)
        {
            OnHeal?.Invoke();
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + health);
        }

        private void Death()
        {
            OnDeath?.Invoke();
        }

        protected override void Start()
        {
            base.Start();
            CurrentHealth = MaxHealth;
        }
    }
}