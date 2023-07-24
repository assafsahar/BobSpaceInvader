
using COD.Shared;

namespace COD.GameLogic
{
    public class CODCollectable : ICollectable
    {
        public CollectableType Type { get; private set; }

        public CODCollectable(CollectableType type)
        {
            Type = type;
        }

        // Any additional functionality/logic for the collectable
    }
}
