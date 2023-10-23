using COD.Core;
using COD.Shared;
using COD.UI;
using System.Collections;
using UnityEngine;
using static COD.GameLogic.CODGameFlowManager;

namespace COD.GameLogic
{
    public class CODShipCollisionHandler : CODMonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (CODGameLogicManager.Instance.GameFlowManager.CurrentState == GameState.Falling && other.CompareTag("GroundCollider"))
            {
                StartCoroutine(RestartAfterDelay(2f));  // Wait for 2 seconds before restarting
            }
            else if (other.CompareTag("BackgroundFront") && CODGameLogicManager.Instance.GameFlowManager.CurrentState != GameState.Falling)
            {
                // Handle the game end condition here.
                Debug.Log("End");
                CODGameLogicManager.Instance.GameFlowManager.EndGame();
            }
            else if (other.CompareTag("Collectable") && CODGameLogicManager.Instance.GameFlowManager.CurrentState != GameState.Falling)
            {
                // Notify CODCollectablesManager about the collectable collision
                CODGameLogicManager.Instance.CollectablesManager.HandleCollectableCollected(other.GetComponent<CODCollectableGraphics>());
            }
            
        }
        private IEnumerator RestartAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            CODGameLogicManager.Instance.GameFlowManager.EndGame();
        }
    }
}
