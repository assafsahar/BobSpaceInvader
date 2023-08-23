
using COD.Shared;

namespace COD.GameLogic
{
    public class CODCollectable : ICollectable
    {
        public CollectableType Type { get; private set; }
        public int ScoreValue { get; private set; }
        public int EnergyValue { get; set; }

        public CODCollectable(CollectableType type, int? weight = null)
        {
            Type = type;
            int currentEnergyValue = CODGameLogicManager.Instance.CollectableSettingsManager.GetCurrentEnergyValue();
            //Todo - make this from configuration
            switch (type)
            {
                case CollectableType.Coin:
                    ScoreValue = 10;
                    EnergyValue = 0;
                    break;
                case CollectableType.SuperCoin:
                    ScoreValue = 50;
                    EnergyValue = 0;
                    break;
                case CollectableType.Bomb:
                    ScoreValue = 0;
                    EnergyValue = 0;
                    break;
                case CollectableType.Energy:
                    ScoreValue = 0;
                    EnergyValue = currentEnergyValue;
                    break;
                default:
                    ScoreValue = 0;
                    break;
            }
        }
    }
}
