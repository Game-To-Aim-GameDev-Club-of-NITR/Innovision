using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    /**
     * The core singleton manager responsible for high-level game flow, including managing GameStates,
     * scene transitions, and multiplayer client lifecycle events. It acts as the central entry point
     * for orchestrating gameplay logic across scenes and multiplayer sessions.
     *
     * Responsibilities:
     * - Holds and manages all registered GameStates.
     * - Handles scene load/unload callbacks and delegates to the current GameState.
     * - Tracks network client connections and disconnections (server-side only).
     * - Delegates global Update and FixedUpdate calls to the active GameState.
     *
     * Usage:
     * - This class is intended to be subclassed by the user.
     * - In your subclass (e.g., MyGameManager), manually register all required GameStates in Awake().
     * - GameManager must be present in the persistent bootstrap scene and marked DontDestroyOnLoad.
     * - Call `SetGameState(string tag)` to switch between GameStates as needed.
     *
     * Multiplayer Notes:
     * - Hooks into Unity Netcode’s NetworkManager to track client connections.
     * - Calls `OnPlayerConnected(ulong)` and `OnPlayerDisconnected(ulong)` on the current GameState.
     * - Only the server receives these callbacks.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: 07-06-2025
     * Modified By: Prayas Bharadwaj
     */

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private Dictionary<string, GameState> RegisteredGameStates { get; set; }
        private List<string> Tags { get; set; }
        public GameState CurrentGameState { get; private set; }

        private void OnEnable()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
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

        private void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            if (CurrentGameState != null)
                CurrentGameState.OnPlayerConnected(clientId);
            else
                Debug.LogWarning($"Client {clientId} connected, but no active GameState yet.");
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            if (CurrentGameState != null)
                CurrentGameState.OnPlayerDisconnected(clientId);
            else
                Debug.LogWarning($"Client {clientId} disconnected, but no active GameState yet.");
        }
    }
}