using COD.Core;
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
        private ShipController shipController;
        //private Animator shipAnimator;
        /*private Animator explosionAnimator;
        private SpriteRenderer shipSpriteRenderer;
        private SpriteRenderer explosionSpriteRenderer;*/
        private CODShipGraphics shipGraphics;

        private void Start()
        {
            shipController = GetComponent<ShipController>();
            /*explosionAnimator = shipController.transform.GetComponentInChildren<Animator>();
            explosionAnimator.gameObject.SetActive(false);
            explosionAnimator.SetBool("IsExplode", false);
            explosionSpriteRenderer = explosionAnimator.GetComponent<SpriteRenderer>();
            shipSpriteRenderer = GetComponent<SpriteRenderer>();
            shipSpriteRenderer.gameObject.SetActive(true);*/
            shipGraphics = GetComponentInChildren<CODShipGraphics>();
            //shipAnimator = GetComponent<Animator>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("BackgroundFront"))
            {
                if (shipController.IsShieldActive)
                {
                    return;
                }
                // Handle the game end condition here.
                CODDebug.Log("End");
                ShowExplosion();
                //shipAnimator.SetBool("IsExplode", true);
            }
            else if (other.CompareTag("Collectable") && CODGameLogicManager.Instance.GameFlowManager.CurrentState != GameState.Falling)
            {
                if (!shipGraphics.ExplosionIsActive())
                {
                    // Notify CODCollectablesManager about the collectable collision
                    CODCollectableGraphics collectableGraphics = other.GetComponent<CODCollectableGraphics>();
                    CODGameLogicManager.Instance.CollectablesManager.HandleCollectableCollected(collectableGraphics);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (CODGameLogicManager.Instance.GameFlowManager.CurrentState == GameState.Falling && other.gameObject.CompareTag("GroundCollider"))
            {
                if (shipController.IsShieldActive)
                {
                    return;
                }
                ShowExplosion();
            }
        }
        private void ShowExplosion()
        {
            shipController.EnableInput(false);
            shipGraphics.TriggerExplosion();
            InvokeEvent(CODEventNames.OnShipCrash);
            StartCoroutine(RestartAfterDelay(2f));
        }
        private IEnumerator RestartAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            shipGraphics.ResetGraphicsPostExplosion();
            CODGameLogicManager.Instance.GameFlowManager.EndGame();
        }
    }
}
