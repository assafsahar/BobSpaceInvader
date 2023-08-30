using COD.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static COD.Shared.GameEnums;

namespace COD.GameLogic
{
    public class CODGameFlowManager : CODMonoBehaviour
    {
        private const string GameSceneName = "GameScene";
        private CODEnergyManager energyManager;
        private GameState _currentState;

        public enum GameState
        {
            Start,
            Playing,
            Paused,
            Ended
        }

        public GameState CurrentState {
            get
            {
                return _currentState;
            }
            private set
            {
                Debug.Log("GameState changed from " + _currentState + " to " + value);
                _currentState = value;
            } 
        }
        private void OnEnable()
        {
            AddListener(CODEventNames.OnUpgraded, HandleUpgradeEnergyCapsule);
        }
        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnUpgraded, HandleUpgradeEnergyCapsule);
        }
        private void Awake()
        {
            energyManager = CODGameLogicManager.Instance.EnergyManager;
        }
        private void Start()
        {
            CurrentState = GameState.Start;
            StartGame();
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
                CurrentState = GameState.Playing;
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
            if (CurrentState == GameState.Playing)
            {
                // Game ended, do cleanup or show relevant UI
                CODManager.Instance.PoolManager.Cleanup();

                // save player data
                CODGameLogicManager.Instance.UpgradeManager.SavePlayerData();

                // Reload the current scene
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
                CurrentState = GameState.Ended;

                CODGameLogicManager.Instance.UpgradeManager.LoadPlayerData();
            }
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
                energyManager?.UpdateEnergy(Time.deltaTime);
                yield return new WaitForSeconds(0.1f);   
            }
        }
    }
}
