using COD.Core;
using System;
using System.Linq;
using UnityEngine;

namespace COD.GameLogic
{
    /// <summary>
    /// This class coordinate between various other managers 
    /// and ensure that the game logic is executed correctly.
    /// </summary>
    public class CODGameLogicManager : ICODBaseManager
    {
        public static CODGameLogicManager Instance;
        public CODScoreManager ScoreManager;
        private CODGameFlowManager gameFlowManager;
        private CODCollectablesManager collectablesManager;
        private float startEnergyDefault = 20f;

        public CODEnergyManager EnergyManager { get; private set; }
        public CODCollectableSettingsManager CollectableSettingsManager { get; private set; }
        public bool IsInitialized { get; private set; } = false;
        public CODUpgradeManagerConfig UpgradeManagerConfigData { get; private set; }


        public CODCollectablesManager CollectablesManager
        {
            get
            {
                if (collectablesManager == null)
                {
                    collectablesManager = GameObject.FindObjectOfType<CODCollectablesManager>();
                    if (collectablesManager == null)
                    {
                        CODDebug.LogException("Could not find CODCollectablesManager in the scene!");
                    }
                }
                return collectablesManager;
            }
        }
        public CODGameFlowManager GameFlowManager
        {
            get
            {
                if(gameFlowManager == null)
                {
                    gameFlowManager = GameObject.FindObjectOfType<CODGameFlowManager>();
                    if(gameFlowManager == null)
                    {
                        CODDebug.LogException("Could not find CODGameFlowManager in the scene!");
                    }
                }
                return gameFlowManager;
            }
        }
        public CODUpgradeManager UpgradeManager;

        public CODGameLogicManager()
        {
            if (Instance != null)
            {
                CODDebug.LogException("Multiple instances of CODGameLogicManager being created!");
                return;
            }

            Instance = this;
            
        }

        public void LoadManager(Action onComplete)
        {
            ScoreManager = new CODScoreManager();
            ScoreManager.Initialize();

            while (!ScoreManager.IsInitialized)
            {
                // Wait until the ScoreManager is fully initialized
            }

            CODManager.Instance.ConfigManager.GetConfigAsync<CODUpgradeManagerConfig>("UpgradableConfig", (config) =>
            {
                UpgradeManagerConfigData = config;

                var energyDataMain = UpgradeManagerConfigData.UpgradeableConfigs.FirstOrDefault(x => x.UpgradableTypeID == UpgradeablesTypeID.GetMoreEnergy);
                var energyData = energyDataMain.UpgradableLevelData[0];

                float? maxEnergy = energyData.MaxEnergy;
                float initialEnergy = maxEnergy.HasValue ? maxEnergy.Value : startEnergyDefault;
                float energyDecreaseRate = energyData.EnergyDecreaseRate; 

                EnergyManager = new CODEnergyManager(maxEnergy, initialEnergy, energyDecreaseRate);
                UpgradeManager = new CODUpgradeManager();
                CollectableSettingsManager = new CODCollectableSettingsManager();

                IsInitialized = true;
                onComplete.Invoke();
            });
        }
    }
}
