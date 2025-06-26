using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    /**
     * The core game instance of the entire game. This is the main entry point of the entire game.
     * It is recommended that for the entire game a single manager be created that is directly a subclass
     * of GameManager or directly uses this class.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
     */

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private Dictionary<string, GameState> RegisteredGameStates { get; set; }
        private List<string> Tags { get; set; }
        public GameState CurrentGameState { get; private set; }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(Instance);

            RegisteredGameStates = new Dictionary<string, GameState>();
            Tags = new List<string>();
        }

        private void Start()
        {
            LoadDefaultLevel();
        }

        // Update is called once per frame
        private void Update()
        {
            CurrentGameState?.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            CurrentGameState?.FixedTick(Time.fixedDeltaTime);
        }

        // Register a GameState into the global game manager
        public void RegisterGameState(GameState gameState)
        {
            if (RegisteredGameStates.TryAdd(gameState.Tag, gameState))
            {
                Tags.Add(gameState.Tag);
            }
            else
            {
                Debug.LogWarning($"Game State '{gameState.Tag}' already registered!");
            }
        }

        //Loads the default registered level
        protected virtual bool LoadDefaultLevel()
        {
            if (Tags.Count == 0)
            {
                Debug.LogWarning("No game states registered. Please register at least one game state.");
                return false;
            }

            if (RegisteredGameStates.TryGetValue(Tags[0], out var nextState))
            {
                CurrentGameState?.Exit();
                CurrentGameState = nextState;
                CurrentGameState.Enter();
                return true;
            }

            Debug.LogWarning($"GameState '{Tags[0]}' not found.");
            return false;
        }

        //Sets the current game state to the one specified
        public virtual bool SetGameState(string gsTag)
        {
            if (RegisteredGameStates.TryGetValue(gsTag, out var nextState))
            {
                CurrentGameState?.Exit();
                CurrentGameState = nextState;
                CurrentGameState.Enter();
                return true;
            }
            Debug.LogWarning($"GameState '{gsTag}' not found.");
            return false;
        }

        public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Scene Loaded '{scene.name}'");
            CurrentGameState?.OnSceneLoaded(scene.name);
        }

        public virtual void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"Scene Unloaded '{scene.name}'");
            CurrentGameState?.OnSceneUnloaded(scene.name);
        }

        public void OnLevelStateFound(LevelState levelState)
        {
            CurrentGameState?.AssignLevelState(levelState);
        }
    }
}