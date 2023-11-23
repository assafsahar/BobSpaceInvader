using COD.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static COD.Shared.GameEnums;

namespace COD.GameLogic
{   /// <summary>
    /// This class manages the flow of the game. 
    /// It controls the transitions between different game states 
    /// like starting, pausing, resuming, and ending the game
    /// </summary>
    public class CODGameFlowManager : CODMonoBehaviour
    {
        [SerializeField] float initialShipSpeed = 5f;
        [SerializeField] float speedIncreaseRate = 0.1f;
        [SerializeField] float energyDecreaseInterval = 0.4f;
        [SerializeField] int travelDistancePerFrame = 1;

        private float currentSpeed;
        private float targetSpeed;
        private float lerpFactor = 0.01f;
        private const string GameSceneName = "GameScene";
        private CODEnergyManager energyManager;
        private GameState _currentState;
        private int distanceTravelledThisGame = 0;
        

        public enum GameState
        {
            Start,
            Playing,
            Paused,
            Ended,
            Falling
        }

        public GameState CurrentState {
            get
            {
                return _currentState;
            }
            private set
            {
                //Debug.Log("GameState changed from " + _currentState + " to " + value);
                _currentState = value;
            } 
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            AddListener(CODEventNames.OnUpgraded, HandleUpgradeEnergyCapsule);
        }
        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnUpgraded, HandleUpgradeEnergyCapsule);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        private void Awake()
        {
            energyManager = CODGameLogicManager.Instance.EnergyManager;
            targetSpeed = currentSpeed = initialShipSpeed;
        }
        private void Start()
        {
            CurrentState = GameState.Start;
            StartCoroutine(StartWhenReady());
        }

        private void Update()
        {
            if (CurrentState == GameState.Playing)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, lerpFactor);
                InvokeEvent(CODEventNames.OnSpeedChange, currentSpeed);
                distanceTravelledThisGame++;
                //InvokeEvent(CODEventNames.OnDistanceSet, distanceTravelledThisGame);
                CODGameLogicManager.Instance.ScoreManager.AddDistance(travelDistancePerFrame);
            }
        }

        public void StartGame()
        {
            Debug.Log("StartGame");
            if (CurrentState == GameState.Playing)
            {
                Debug.LogWarning("Attempting to start the game when it's already playing.");
                return;
            }
            if (CurrentState == GameState.Start || CurrentState == GameState.Ended)
            {
                energyManager.ResetEnergy();
                distanceTravelledThisGame = 0;
                CODGameLogicManager.Instance.ScoreManager.ResetGameScores();
                CurrentState = GameState.Playing;

                // Load player data and update UI
                CODGameLogicManager.Instance.UpgradeManager.LoadPlayerData();
                InvokeEvent(CODEventNames.RequestScoreUpdate);

                StartCoroutine(EnergyUpdateRoutine());
            }
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                Time.timeScale = 0;
                CurrentState = GameState.Paused;
            }
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                Time.timeScale = 1;
                CurrentState = GameState.Playing;
            }
        }

        public void EndGame()
        {
            Debug.Log("EndGame CurrentState=" + CurrentState);
            DisplayHighScoresAndDistance();
            if (CurrentState == GameState.Playing || CurrentState == GameState.Falling)
            {
                // Game ended, do cleanup or show relevant UI
                CODManager.Instance.PoolManager.Cleanup();

                // save player data
                CODGameLogicManager.Instance.UpgradeManager.SavePlayerData();

                //CODManager.Instance.EventsManager.StartListeningToSceneLoaded();

                // Reload the current scene
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
                CurrentState = GameState.Ended;

                CODGameLogicManager.Instance.ScoreManager.CalculateScore();
            }
        }
        public void DisplayHighScoresAndDistance()
        {
            var scoreManager = CODGameLogicManager.Instance.ScoreManager;

            int highestScore = scoreManager.GetHighestScore(ScoreTags.MainScore);
            int longestDistance = scoreManager.GetHighestScore(ScoreTags.Distance);

            Debug.Log($"Highest Score: {highestScore}");
            Debug.Log($"Longest Distance: {longestDistance}");
        }
        public void ChangeToFallState()
        {
            if (CurrentState == GameState.Playing)
            {
                CurrentState = GameState.Falling;
            }
        }
        private IEnumerator StartWhenReady()
        {
            while (!CODGameLogicManager.Instance.IsInitialized || !CODGameLogicManager.Instance.ScoreManager.IsInitialized)
            {
                yield return null;  
            }

            StartGame();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            CODGameLogicManager.Instance.UpgradeManager.LoadPlayerData();
        }

        private void HandleUpgradeEnergyCapsule(object unused)
        {
            if (CODGameLogicManager.Instance.UpgradeManager.CanMakeUpgrade(UpgradeablesTypeID.GetMoreEnergy))
            {
                if (CODGameLogicManager.Instance.UpgradeManager.UpgradeItemByID(UpgradeablesTypeID.GetMoreEnergy))
                {
                    Debug.Log("Upgrade Successful!");
                }
                else
                {
                    Debug.Log("Not enough coins!");
                }
            }
            else
            {
                Debug.Log("Already at maximum upgrade level!");
            }
        }

        private IEnumerator EnergyUpdateRoutine()
        {
            int loopCount = 0;

            while (CurrentState == GameState.Playing)
            {
                loopCount++;
                targetSpeed += speedIncreaseRate;
                energyManager?.UpdateEnergy(Time.deltaTime);
                yield return new WaitForSeconds(energyDecreaseInterval);   
            }
        }
    }
}
