
using COD.Shared;

namespace COD.GameLogic
{
    /// <summary>
    /// This class represents a collectable item within the game. 
    /// It likely contains properties such as the type of collectable, 
    /// its value (be it score, energy, or other resources), 
    /// and any effects it has when collected by the player
    /// </summary>
    public class CODCollectable : ICollectable
    {
        public CollectableType Type { get; private set; }
        public int ScoreValue { get; private set; }
        public float EnergyValue { get; set; }

        public CODCollectable(CollectableType type)
        {
            Type = type;
            float currentEnergyValue = CODGameLogicManager.Instance.CollectableSettingsManager.GetCurrentEnergyValue();
            //Todo - make this from configuration
            switch (type)
            {
                case CollectableType.Coin:
                    ScoreValue = 1;
                    EnergyValue = 0;
                    break;
                case CollectableType.SuperCoin:
                    ScoreValue = 5;
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
                case CollectableType.Shield:
                    ScoreValue = 0;
                    EnergyValue = 0;
                    break;
                case CollectableType.Magnet:
                    ScoreValue = 0;
                    EnergyValue = 0;
                    break;
                default:
                    ScoreValue = 0;
                    break;
            }
        }
    }
}
