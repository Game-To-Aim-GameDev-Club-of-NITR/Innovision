using UnityEngine;

namespace Framework
{
    [CreateAssetMenu(menuName = "Game/GameState/MainGameState")]
    public class MainGameState : GameState
    {
        public MainGameState() : base("Main") { }

        public override void LoadLevel()
        {
            // Load the scene that contains your LevelState
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/MainScene"); // Replace with actual scene name
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("MainGameState Entered");
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("MainGameState Exited");
        }
    }
}