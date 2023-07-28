
using COD.Shared;

namespace COD.GameLogic
{
    public class CODCollectable : ICollectable
    {
        public CollectableType Type { get; private set; }
        public int ScoreValue { get; private set; }

        public CODCollectable(CollectableType type, int? weight = null)
        {
            Type = type;
            switch (type)
            {
                case CollectableType.Coin:
                    ScoreValue = 10;
                    break;
                case CollectableType.SuperCoin:
                    ScoreValue = 50;
                    break;
                default:
                    ScoreValue = 0;
                    break;
            }
        }
    }
}
