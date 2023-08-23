using COD.Core;

namespace COD.UI
{
    public class CODUIController : CODMonoBehaviour
    {
        public void UpgradeEnergyCapsuleButtonPressed()
        {
            InvokeEvent(CODEventNames.OnUpgraded, null);
        }
    }
}
