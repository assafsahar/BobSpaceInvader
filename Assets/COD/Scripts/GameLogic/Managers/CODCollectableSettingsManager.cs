
using COD.Shared;

namespace COD.GameLogic
{
    /// <summary>
    /// handles the settings for the collectable items
    /// </summary>
    public class CODCollectableSettingsManager
    {
        public float GetCurrentEnergyValue()
        {
            var upgradeConfig = CODGameLogicManager.Instance.UpgradeManager.GetCodUpgradeableConfigByID(UpgradeablesTypeID.GetMoreEnergy);
            var upgradeData = CODGameLogicManager.Instance.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.GetMoreEnergy);
            if (upgradeConfig != null && upgradeData != null && upgradeConfig.UpgradableLevelData.Count > upgradeData.CurrentLevel)
            {
                return upgradeConfig.UpgradableLevelData[upgradeData.CurrentLevel].Energy;
            }
            return 0;
        }
        /*public CODCollectableConfig GetCollectableConfig(CollectableType type)
        {
            return gameConfig.CollectableConfigs.FirstOrDefault(c => c.CollectableType == type.ToString());
        }*/

    }
}

