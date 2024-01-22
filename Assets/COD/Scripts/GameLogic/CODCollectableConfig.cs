using COD.Shared;
namespace COD.GameLogic
{
    public class CODCollectableConfig
    {
        public CollectableType Type { get; set; }
        public int ScoreValue { get; set; }
        public float EnergyValue { get; set; }
        public float MovementSpeed { get; set; }
    }
}