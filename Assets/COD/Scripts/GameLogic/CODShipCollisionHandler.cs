using COD.Core;
using COD.UI;
using System;
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
        private CODShipGraphics shipGraphics;

        private void Start()
        {
            shipController = GetComponent<ShipController>();
            shipGraphics = GetComponentInChildren<CODShipGraphics>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("BackgroundFront"))
            {
                HandleCollisionWithBackground();
            }
            else if (other.CompareTag("Collectable") && CODGameLogicManager.Instance.GameFlowManager.CurrentState != GameState.Falling)
            {
                HandleCollectableCollision(other);
            }
        }

        private void HandleCollectableCollision(Collider2D other)
        {
            if (!shipGraphics.ExplosionIsActive())
            {
                CODCollectableGraphics collectableGraphics = other.GetComponent<CODCollectableGraphics>();
                CODGameLogicManager.Instance.CollectablesManager.HandleCollectableCollected(collectableGraphics);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (CODGameLogicManager.Instance.GameFlowManager.CurrentState == GameState.Falling && other.gameObject.CompareTag("GroundCollider"))
            {
                HandleCollisionWithGround();
            }
        }
        private void HandleCollisionWithBackground()
        {
            if (shipController.IsShieldActive && CODGameLogicManager.Instance.EnergyManager.CurrentEnergy > 0)
            {
                return; // Ignore collision if shield is active and energy is not depleted
            }
            ShowExplosion();
        }

        private void HandleCollisionWithGround()
        {
            if (shipController.IsShieldActive)
            {
                return; // Ignore collision with ground if shield is active
            }
            ShowExplosion();
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
