
namespace COD.GameLogic
{
    public class CODCollectableSettingsManager
    {
        public int GetCurrentEnergyValue()
        {
            var upgradeConfig = CODGameLogicManager.Instance.UpgradeManager.GetCodUpgradeableConfigByID(UpgradeablesTypeID.GetMoreEnergy);
            var upgradeData = CODGameLogicManager.Instance.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.GetMoreEnergy);
            if (upgradeConfig != null && upgradeData != null && upgradeConfig.UpgradableLevelData.Count > upgradeData.CurrentLevel)
            {
                return upgradeConfig.UpgradableLevelData[upgradeData.CurrentLevel].Energy;
            }
            return 0;
        }
    }
}

