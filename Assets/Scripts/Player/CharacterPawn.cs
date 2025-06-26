using Framework.Components;
using UnityEngine;

namespace Player
{
    /**
     * A passive pawn that exposes public methods to be controlled by PlayerController.
     * It handles health, movement, and weapon functionality.
     *
     * Date Created: 25-06-2025
     * Created By: Prayas Bharadwaj
     */

    public class PlayerPawn : Pawn
    {
        [SerializeField] private MovementComponent movement;
        [SerializeField] private HealthComponent health;
        [SerializeField] private WeaponComponent weapon;
        [SerializeField] private InputComponent input;

        private float _x = 0.0f;
        private float _z = 0.0f;

        public override void OnStart()
        {
            base.OnStart();

            if (!health) return;
            health.OnDeath += OnDeath;
            health.OnDamaged += OnDamaged;
            if (!input) return;
            input.ActionMappings.Add("Jump", Jump);
            input.ActionMappings.Add("Fire", Fire);
            input.ActionMappings.Add("Reload", Reload);
            input.Vector3Mappings.Add("Move", Move);
        }

        public override void Tick(float deltaTime)
        {

            base.Tick(deltaTime);
            if (!IsPossessed) return;
            movement?.Move(new Vector3(_x, 0, _z));
            movement?.Tick(deltaTime);
            weapon?.Tick(deltaTime);
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            base.FixedTick(fixedDeltaTime);
            if (!IsPossessed) return;
            movement?.FixedTick(fixedDeltaTime);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (health != null)
            {
                health.OnDeath -= OnDeath;
                health.OnDamaged -= OnDamaged;
            }
        }

        public override void OnPossess(PlayerController controller)
        {
            base.OnPossess(controller);
            Debug.Log($"[Pawn] OnPossess. Setting IsPossessed = true");
        }

        // These are called from PlayerController
        private void Move(Vector3 direction)
        {
            movement?.Move(direction);
        }

        public void MoveX(float x)
        {
            Debug.Log($"[Pawn] MoveX {x}");
            _x = x;
        }

        public void MoveZ(float z)
        {
            _z = z;
        }

        public void Jump()
        {
            movement?.Jump();
        }

        public void Fire()
        {
            weapon?.TryFire();
        }

        public void Reload()
        {
            weapon?.StartReload();
        }

        public void TakeDamage(float amount)
        {
            health?.TakeDamage(amount);
        }

        public void Heal(float amount)
        {
            health?.Heal(amount);
        }

        private void OnDeath()
        {
            Debug.Log($"{gameObject.name} died.");
            gameObject.SetActive(false);
        }

        private void OnDamaged()
        {
            Debug.Log($"{gameObject.name} took damage. Health: {health.CurrentHealth}");
        }
    }
}