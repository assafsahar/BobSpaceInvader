using UnityEngine;
using COD.Core;
using COD.UI;
using static COD.GameLogic.CODGameFlowManager;

namespace COD.GameLogic
{
    public class ShipController : CODMonoBehaviour
    {
        [SerializeField] private CODShipGraphics shipGraphics;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float verticalSpeed;
        [SerializeField] private float upperLimit;
        [SerializeField] private float lowerLimit;
        [SerializeField] private float stateChangeThreshold = 0.05f;

        private float stillnessDuration = 0.2f;  // Adjust based on your preference. Represents the time of stillness before changing to straight state.
        private float currentStillnessTime = 0f;
        private float fallingRotationSpeed = 50f;

        private void Start()
        {
            shipGraphics.InitializeShip(upperLimit, lowerLimit);
        }
        private void OnEnable()
        {
            AddListener(CODEventNames.OnTouchStarted, OnTouchStarted);
            AddListener(CODEventNames.OnTouchStayed, OnTouchStayed);
            AddListener(CODEventNames.OnTouchEnded, OnTouchEnded);
        }

        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnTouchStarted, OnTouchStarted);
            RemoveListener(CODEventNames.OnTouchStayed, OnTouchStayed);
            RemoveListener(CODEventNames.OnTouchEnded, OnTouchEnded);
        }

        private void Update()
        {
            if (CODGameLogicManager.Instance.GameFlowManager.CurrentState == GameState.Falling)
            {
                FallDown();
                return; 
            }
        }

        void FallDown()
        {
            Vector3 newShipPosition = transform.position + new Vector3(0, -Time.deltaTime * verticalSpeed, 0);
            transform.position = newShipPosition;
            float rotationAmount = fallingRotationSpeed * Time.deltaTime; 
            transform.Rotate(0f, 0f, rotationAmount);
        }

        private void OnTouchStarted(object obj)
        {
            // No need to handle touch position here
        }

        private void OnTouchStayed(object obj)
        {
            if(CODGameLogicManager.Instance.GameFlowManager.CurrentState  == GameState.Falling) {
                return;
            }
            float targetY = Camera.main.ScreenToWorldPoint(new Vector3(0, (float)obj, 0)).y;

            // Calculate distance and direction
            float distanceToTarget = Mathf.Abs(shipGraphics.transform.position.y - targetY);
            float direction = targetY > shipGraphics.transform.position.y ? 1 : -1;

            // Update position
            Vector3 newShipPosition = shipGraphics.transform.position + new Vector3(0, direction * Time.deltaTime * verticalSpeed, 0);

            // If the ship overshot its target, set it directly to the target
            if (direction > 0 && newShipPosition.y > targetY || direction < 0 && newShipPosition.y < targetY)
            {
                newShipPosition.y = targetY;
            }

            // Clamp position
            newShipPosition.y = Mathf.Clamp(newShipPosition.y, lowerLimit, upperLimit);

            shipGraphics.transform.position = newShipPosition;

            // If distance is minor, accumulate time and consider switching to Straight state.
            if (distanceToTarget < stateChangeThreshold)
            {
                currentStillnessTime += Time.deltaTime;

                if (currentStillnessTime >= stillnessDuration)
                {
                    shipGraphics.UpdateShipState(ShipState.Straight);
                    return;
                }
            }
            else
            {
                currentStillnessTime = 0;  // Reset stillness time if there's notable movement

                // Handle visuals based on movement direction
                if (direction > 0)
                {
                    MoveShipUp();
                }
                else
                {
                    MoveShipDown();
                }
            }
        }

        private void OnTouchEnded(object obj)
        {
            StopShipMovement();
        }

        private void StopShipMovement()
        {
            shipGraphics.UpdateShipState(ShipState.Straight);
        }

        private void MoveShipDown()
        {
            shipGraphics.UpdateShipState(ShipState.Down);
            shipGraphics.RotateShip(-rotationSpeed);
        }

        private void MoveShipUp()
        {
            shipGraphics.UpdateShipState(ShipState.Up);
            shipGraphics.RotateShip(rotationSpeed);
        }

        
    }
}

