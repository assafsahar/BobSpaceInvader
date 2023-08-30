using COD.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static COD.Shared.GameEnums;

namespace COD.GameLogic
{
    public class CODUpgradeManager
    {
        public CODPlayerUpgradeInventoryData PlayerUpgradeInventoryData; //Player Saved Data
        public CODUpgradeManagerConfig UpgradeConfig = new CODUpgradeManagerConfig(); //From cloud

        //MockData
        //Load From Save Data On Device (Future)
        //Load Config From Load

        public CODUpgradeManager()
        {
            CODManager.Instance.ConfigManager.GetConfigAsync<CODUpgradeManagerConfig>("UpgradableConfig", delegate (CODUpgradeManagerConfig config)
            {
                UpgradeConfig = config;
            });

            CODManager.Instance.SaveManager.Load<CODPlayerUpgradeInventoryData>(data =>
            {
                PlayerUpgradeInventoryData = data ?? new CODPlayerUpgradeInventoryData
                {
                    Upgradeables = new List<CODUpgradeableData>()
                 {
                    new CODUpgradeableData
                    {
                        upgradableTypeID = UpgradeablesTypeID.GetMoreEnergy,
                        CurrentLevel = 1
                    }
                },
                    TotalCoins = 0,
                    TotalSuperCoins = 0,
                    CurrentScore = 0
                };

                CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnScoreSet, (ScoreTags.Coin, PlayerUpgradeInventoryData.TotalCoins));
                CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnScoreSet, (ScoreTags.SuperCoin, PlayerUpgradeInventoryData.TotalSuperCoins));
                CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnScoreSet, (ScoreTags.MainScore, PlayerUpgradeInventoryData.CurrentScore));
                //CODGameLogicManager.Instance.ScoreManager.SetScoreByTag(ScoreTags.Coin, PlayerUpgradeInventoryData.TotalCoins);
                //CODGameLogicManager.Instance.ScoreManager.SetScoreByTag(ScoreTags.MainScore, PlayerUpgradeInventoryData.CurrentScore);

            });
        }

        
        public bool CanMakeUpgrade(UpgradeablesTypeID typeID)
        {
            return UpgradeItemByID(typeID, false);
        }

        public bool UpgradeItemByID(UpgradeablesTypeID typeID, bool makeTheUpgrade = true)
        {
            var upgradeable = GetUpgradeableByID(typeID);

            if (upgradeable != null)
            {
                return TryTheUpgrade(typeID, makeTheUpgrade, upgradeable);
            }
            else
            {
                Debug.Log("failed because upgradable was null");
                //CODManager.Instance.CrashManager.LogExceptionHandling($"UpgradeItemByID {typeID.ToString()} failed because upgradable was null");
                return false;
            }
        }

        public CODUpgradeableConfig GetCodUpgradeableConfigByID(UpgradeablesTypeID typeID)
        {
            CODUpgradeManagerConfig codUpgradeManagerConfig = UpgradeConfig;
            CODUpgradeableConfig upgradeableConfig = UpgradeConfig.UpgradeableConfigs.FirstOrDefault(upgradable => upgradable.UpgradableTypeID == typeID);
            return upgradeableConfig;
        }

        public int GetScoreByIDAndLevel(UpgradeablesTypeID typeID, int level)
        {
            var upgradeableConfig = GetCodUpgradeableConfigByID(typeID);
            var score = upgradeableConfig.UpgradableLevelData[level].Score;
            return score;
        }

        public CODUpgradeableData GetUpgradeableByID(UpgradeablesTypeID typeID)
        {
            var upgradeable = PlayerUpgradeInventoryData.Upgradeables.FirstOrDefault(x => x.upgradableTypeID == typeID);
            return upgradeable;
        }

        public void SavePlayerData()
        {
            int coinScore = 0;
            int superCoinScore = 0;
            int mainScore = 0;

            if (CODGameLogicManager.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.Coin, ref coinScore))
            {
                PlayerUpgradeInventoryData.TotalCoins = coinScore;
            }
            if (CODGameLogicManager.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.SuperCoin, ref superCoinScore))
            {
                PlayerUpgradeInventoryData.TotalSuperCoins = superCoinScore;
            }
            if (CODGameLogicManager.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.MainScore, ref mainScore))
            {
                PlayerUpgradeInventoryData.CurrentScore = mainScore;
            }

            CODManager.Instance.SaveManager.Save(PlayerUpgradeInventoryData);
        }
        public void LoadPlayerData()
        {
            CODManager.Instance.SaveManager.Load<CODPlayerUpgradeInventoryData>(loadedData =>
            {
                // Check if the loaded data is not null. If it's null, it means no data was previously saved, and we can't proceed.
                if (loadedData != null)
                {
                    PlayerUpgradeInventoryData = loadedData;

                    // Update the CODScoreManager's data
                    CODGameLogicManager.Instance.ScoreManager.SetScoreByTag(ScoreTags.Coin, loadedData.TotalCoins);
                    CODGameLogicManager.Instance.ScoreManager.SetScoreByTag(ScoreTags.SuperCoin, loadedData.TotalSuperCoins);
                    CODGameLogicManager.Instance.ScoreManager.SetScoreByTag(ScoreTags.MainScore, loadedData.CurrentScore);
                }
            });
        }

        private bool TryTheUpgrade(UpgradeablesTypeID typeID, bool makeTheUpgrade, CODUpgradeableData upgradeable)
        {
            var upgradeableConfig = GetCodUpgradeableConfigByID(typeID);
            if (upgradeableConfig.UpgradableLevelData.Count <= upgradeable.CurrentLevel)
            {
                return false;
            }
            CODUpgradeableLevelData levelData = upgradeableConfig.UpgradableLevelData[upgradeable.CurrentLevel];
            int amountToReduce = levelData.CoinsNeeded;
            ScoreTags coinsType = levelData.CurrencyTag;
            int newLevel = levelData.Level;

            if (CODGameLogicManager.Instance.ScoreManager.TryUseScore(coinsType, amountToReduce, makeTheUpgrade))
            {
                if (makeTheUpgrade)
                {
                    upgradeable.CurrentLevel++;
                    CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnUpgraded, (coinsType, amountToReduce, newLevel, (int)typeID));
                    CODManager.Instance.SaveManager.Save(PlayerUpgradeInventoryData);
                }
                return true;
            }
            else
            {
                if (makeTheUpgrade)
                {
                    Debug.LogError($"UpgradeItemByID {typeID.ToString()} tried upgrade and there is no enough");
                }
                return false;
            }
        }
    }

    //Per Player Owned Item
    [Serializable]
    public class CODUpgradeableData
    {
        public UpgradeablesTypeID upgradableTypeID;
        public int CurrentLevel;
    }

    //Per Level in Item config
    [Serializable]
    public struct CODUpgradeableLevelData
    {
        public int Level;
        public int CoinsNeeded;
        public ScoreTags CurrencyTag;
        public int Score;
        public int Energy;
    }

    //Per Item Config
    [Serializable]
    public class CODUpgradeableConfig
    {
        public UpgradeablesTypeID UpgradableTypeID;
        public List<CODUpgradeableLevelData> UpgradableLevelData;
    }

    //All config for upgradeable
    [Serializable]  
    public class CODUpgradeManagerConfig
    {
        public List<CODUpgradeableConfig> UpgradeableConfigs;
    }

    //All player saved data
    [Serializable]
    public class CODPlayerUpgradeInventoryData : ICODSaveData
    {
        public List<CODUpgradeableData> Upgradeables;
        public int TotalCoins;
        public int TotalSuperCoins;
        public int CurrentScore;
    }

    [Serializable]
    public enum UpgradeablesTypeID
    {
        GetMoreEnergy = 0
    }
}
