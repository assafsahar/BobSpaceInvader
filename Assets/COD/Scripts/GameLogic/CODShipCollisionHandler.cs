using COD.Core;
using COD.Shared;
using COD.UI;
using System.Collections;
using UnityEngine;
using static COD.GameLogic.CODGameFlowManager;

namespace COD.GameLogic
{
    /// <summary>
    /// responsible for handling collisions for the ship. 
    /// This includes detecting collisions with other objects, 
    /// determining the outcome of a collision (e.g., handle collectable), 
    /// and triggering any necessary responses in the game's logic
    /// </summary>
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
            else if (other.CompareTag("Collectable") && CODGameLogicManager.Instance.GameFlowManager.CurrentState != GameState.Falling)
            {
                // Notify CODCollectablesManager about the collectable collision
                CODGameLogicManager.Instance.CollectablesManager.HandleCollectableCollected(other.GetComponent<CODCollectableGraphics>());
            }
            
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (CODGameLogicManager.Instance.GameFlowManager.CurrentState == GameState.Falling && other.gameObject.CompareTag("GroundCollider"))
            {
                StartCoroutine(RestartAfterDelay(2f));  // Wait for 2 seconds before restarting
            }
        }
        private IEnumerator RestartAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            CODGameLogicManager.Instance.GameFlowManager.EndGame();
        }
    }
}
