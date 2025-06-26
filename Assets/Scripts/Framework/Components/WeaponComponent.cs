using System;
using UnityEngine;

namespace Framework.Components
{
    /**
     * Weapon component is responsible for handling all weapon related operations. It delegates
     * actions that can be performed on a weapon to the appropriate component.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
    */
    public class WeaponComponent : BaseComponent
    {
        [SerializeField] private GameObject muzzleGameObject;

        public Action<Vector3> OnFired;
        public Action OnMuzzleFlash;
        public Action OnReloadStarted;
        public Action OnReloadFinished;

        public float FireRate { get; set; } = 5.0f; // Shots per second
        public float Damage { get; set; } = 10.0f;
        public float Range { get; set; } = 100.0f;
        public int AmmoCapacity { get; set; } = 30;
        public float ReloadTime { get; set; } = 2.0f;

        private int _ammo = 0;
        private float _lastFireTime = -Mathf.Infinity;
        private bool _isReloading = false;
        private bool _isFiring = false;
        private float _reloadTimer = 0.0f;

        public bool IsFiring() => _isFiring;
        public bool IsReloading() => _isReloading;
        public int AmmoLeft() => _ammo;

        protected override void Start()
        {
            base.Start();
            _ammo = AmmoCapacity;
        }

        public void TryFire()
        {
            if (_isReloading || Time.time - _lastFireTime < 1f / FireRate || _ammo <= 0)
                return;

            _lastFireTime = Time.time;
            _ammo--;
            _isFiring = true;

            Vector3 fireDirection = muzzleGameObject 
                ? muzzleGameObject.transform.forward
                : transform.forward;

            OnFired?.Invoke(fireDirection);
            OnMuzzleFlash?.Invoke();

            if (_ammo <= 0)
                StartReload();
        }

        public void StartReload()
        {
            if (_isReloading || _ammo == AmmoCapacity)
                return;

            _isReloading = true;
            _reloadTimer = ReloadTime;
            OnReloadStarted?.Invoke();
        }

        public override void Tick(float deltaTime)
        {
            if (_isReloading)
            {
                _reloadTimer -= deltaTime;
                if (_reloadTimer <= 0.0f)
                    FinishReload();
            }

            _isFiring = false;
        }

        private void FinishReload()
        {
            _ammo = AmmoCapacity;
            _isReloading = false;
            OnReloadFinished?.Invoke();
        }
    }
}
