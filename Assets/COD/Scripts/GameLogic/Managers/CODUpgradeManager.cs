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

            CODManager.Instance.SaveManager.Load(delegate (CODPlayerUpgradeInventoryData data)
            {
                PlayerUpgradeInventoryData = data ?? new CODPlayerUpgradeInventoryData
                {
                    Upgradeables = new List<CODUpgradeableData>(){new CODUpgradeableData
                            {
                                upgradableTypeID = UpgradeablesTypeID.GetMoreEnergy,
                                CurrentLevel = 1
                            }
                        }
                };
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
    }

    [Serializable]
    public enum UpgradeablesTypeID
    {
        GetMoreEnergy = 0
    }
}
