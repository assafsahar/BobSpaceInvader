using COD.Core;
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
        [SerializeField] float travelDistancePerFrame = 0.5f;
        [SerializeField] private InputManager inputManager;

        private float currentSpeed;
        private float targetSpeed;
        private float lerpFactor = 0.01f;
        private CODEnergyManager energyManager;
        private GameState _currentState;
        private float distanceTravelledThisGame = 0;
        private CODStartScreenController startScreenController;
        private float distanceUpdateInterval = 0.5f;
        private float distanceUpdateTimer = 0f;
        private ShipController shipController;

        public enum GameState
        {
            WaitingToStart,
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
                //CODDebug.Log("GameState changed from " + _currentState + " to " + value);
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
            startScreenController = FindObjectOfType<CODStartScreenController>();
            shipController = FindObjectOfType<ShipController>();
            if (shipController != null)
            {
                shipController.EnableInput(true); 
            }
        }
        private void Start()
        {
            CurrentState = GameState.WaitingToStart;
            startScreenController.ShowStartScreen();
            StartCoroutine(StartWhenReady());
        }

        private void Update()
        {
            //CODDebug.Log($"[CODGameFlowManager] Update called. CurrentState: {CurrentState}");
            if (CurrentState == GameState.Playing)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, lerpFactor);
                InvokeEvent(CODEventNames.OnSpeedChange, currentSpeed);
                distanceUpdateTimer += Time.deltaTime;
                if (distanceUpdateTimer >= distanceUpdateInterval)
                {
                    float distanceIncrement = travelDistancePerFrame * (currentSpeed / initialShipSpeed);
                    distanceTravelledThisGame += distanceIncrement;
                    //InvokeEvent(CODEventNames.OnDistanceSet, distanceTravelledThisGame);
                    CODGameLogicManager.Instance.ScoreManager.AddDistance(distanceIncrement);
                    distanceUpdateTimer = 0f;
                    //CODDebug.Log("distanceIncrement=" + distanceIncrement);
                }
            }
        }

        public void StartGame()
        {
            // Reset the first load flag to true when starting a new game
            if (CODGameLogicManager.Instance.UpgradeManager != null)
            {
                CODGameLogicManager.Instance.UpgradeManager.isFirstLoad = true;
            }

            if (CurrentState != GameState.WaitingToStart)
                return;

            CODDebug.Log("[CODGameFlowManager] StartGame called. CurrentState: " + CurrentState);
            ChangeGameState(GameState.Playing);

            energyManager.ResetEnergy();
            distanceTravelledThisGame = 0;

            // Load player data and update UI
            CODGameLogicManager.Instance.UpgradeManager.LoadPlayerData();
            CODGameLogicManager.Instance.ScoreManager.ResetGameScores();
            InvokeEvent(CODEventNames.RequestScoreUpdate);
            StartCoroutine(EnergyUpdateRoutine());
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                Time.timeScale = 0;
                ChangeGameState(GameState.Paused); 
            }
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                Time.timeScale = 1;
                ChangeGameState(GameState.Playing);
            }
        }

        public void EndGame()
        {
            CODDebug.Log("EndGame CurrentState=" + CurrentState);
            if (inputManager != null)
            {
                inputManager.EnableInput(false);
            }
            //DisplayHighScoresAndDistance();
            if (CurrentState == GameState.Playing || CurrentState == GameState.Falling)
            {
                // Game ended, do cleanup or show relevant UI
                CODManager.Instance.PoolManager.Cleanup();

                // save player data
                CODGameLogicManager.Instance.UpgradeManager.SavePlayerData();

                //CODManager.Instance.EventsManager.StartListeningToSceneLoaded();

                /*string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);*/
                ChangeGameState(GameState.Ended);

                //CODGameLogicManager.Instance.ScoreManager.CalculateScore();

                StartCoroutine(EndGameRoutine());
            }
            if (startScreenController != null)
            {
                startScreenController.ShowStartScreen();
            }
            ChangeGameState(GameState.WaitingToStart);
        }
        private IEnumerator EndGameRoutine()
        {
            DisplayHighScoresAndDistance();

            // Wait for 1 second before restarting
            yield return new WaitForSeconds(1f);
            if (inputManager != null)
            {
                inputManager.EnableInput(true);
            }
            // Set the isFirstLoad to false to prevent the scores from being doubled on scene reload
            if (CODGameLogicManager.Instance.UpgradeManager != null)
            {
                CODGameLogicManager.Instance.UpgradeManager.isFirstLoad = false;
            }

            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);

            CODGameLogicManager.Instance.ScoreManager.CalculateScore();


        }
        public void DisplayHighScoresAndDistance()
        {
            var scoreManager = CODGameLogicManager.Instance.ScoreManager;

            int highestScore = scoreManager.GetHighestScore(ScoreTags.MainScore);
            int longestDistance = scoreManager.GetHighestScore(ScoreTags.Distance);

            CODDebug.Log($"Highest Score: {highestScore}");
            CODDebug.Log($"Longest Distance: {longestDistance}");
        }
        public void ChangeToFallState()
        {
            if (CurrentState == GameState.Playing)
            {
                ChangeGameState(GameState.Falling);
            }
        }
        public void ChangeGameState(GameState newState)
        {
            CODDebug.Log($"[CODGameFlowManager] ChangeGameState from {_currentState} to {newState}");
            if (_currentState != newState)
            {
                _currentState = newState;
                InvokeEvent(CODEventNames.OnGameStateChange, newState);
            }
        }
        private IEnumerator StartWhenReady()
        {
            while (!CODGameLogicManager.Instance.IsInitialized || !CODGameLogicManager.Instance.ScoreManager.IsInitialized)
            {
                yield return null;  
            }
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
                    CODDebug.Log("Upgrade Successful!");
                }
                else
                {
                    CODDebug.Log("Not enough coins!");
                }
            }
            else
            {
                CODDebug.LogException("Already at maximum upgrade level!");
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
