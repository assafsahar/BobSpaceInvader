using COD.Core;
using COD.Shared;
using COD.UI;
using UnityEngine;

namespace COD.GameLogic
{
    public class CODShipCollisionHandler : CODMonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("BackgroundFront"))
            {
                // Handle the game end condition here.
                Debug.Log("End");
                CODGameLogicManager.Instance.GameFlowManager.EndGame();
            }
            else if (other.CompareTag("Collectable"))
            {
                // Notify CODCollectablesManager about the collectable collision
                CODGameLogicManager.Instance.CollectablesManager.HandleCollectableCollected(other.GetComponent<CODCollectableGraphics>());
            }
        }
    }
}
