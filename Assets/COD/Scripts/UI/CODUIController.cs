using COD.Core;

namespace COD.UI
{
    /// <summary>
    /// Serves as an intermediary between the UI elements 
    /// and the game events system. It triggers game events in response 
    /// to user interactions with the UI, such as button presses.
    /// </summary>
    public class CODUIController : CODMonoBehaviour
    {
        public void UpgradeEnergyCapsuleButtonPressed()
        {
            InvokeEvent(CODEventNames.OnUpgraded, null);
        }
    }
}
