using System;
using Player;
using UnityEngine;

namespace Framework
{
    public class MainLevelState : LevelState
    {
        public GameObject controllerPrefab;
        public GameObject playerPrefab;

        private void Start()
        {
            OnStart();
        }

        public override void OnStart()
        {
            Debug.Log("MainLevelState Started");

            var controllerGO = Instantiate(controllerPrefab);
            var controller = controllerGO.GetComponent<PlayerController>();
            RegisterController(0, controller);

            Debug.Log("Instantiated Controller: " + controller.name);
            var playerGO = Instantiate(playerPrefab, new Vector3(-257.44f, 9.3f, 10.72f), Quaternion.identity);
            var pawn = playerGO.GetComponent<PlayerPawn>();

            pawn.OnStart();
            controller.Possess(pawn);
            Debug.Log($"Possessed pawn: {controller.ControlledPawn.name}"); 
        }

        private void Update()
        {
            base.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            base.FixedTick(Time.fixedDeltaTime);
        }

        public override void OnExit()
        {
            Debug.Log("MainLevelState Exited");
        }
    }
}