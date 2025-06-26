using System.Runtime.ConstrainedExecution;
using Framework.Components;
using UnityEngine;

namespace Player
{
    public class LocalPlayerController : PlayerController
    {
        private InputComponent input;
        private PlayerPawn pawn;
        private Transform Camera;

        private float forward = 0.0f;
        private float right = 0.0f;
        private const float damping = 0.1f;

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (!pawn)
            {
                pawn = ControlledPawn as PlayerPawn;
                if (!pawn)
                {
                    Debug.LogWarning("[Controller] Failed to cast ControlledPawn to PlayerPawn");
                    return;
                }

                input = pawn.GetComponent<InputComponent>();
                Camera = pawn.GetComponentInChildren<Camera>().transform;
            }

            forward = Input.GetAxis("Vertical");
            right = Input.GetAxis("Horizontal");

            Vector3 inputDirection = Camera.forward * forward + Camera.right * right;
            inputDirection.Normalize();

            if (pawn)
            {
                pawn.MoveX(inputDirection.x);
                pawn.MoveZ(inputDirection.z);
            }
            else Debug.Log("No Pawn");

            if (!IsActive || !isLocalPlayer || input == null) return;
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            base.FixedTick(fixedDeltaTime);
            pawn?.FixedTick(fixedDeltaTime);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}