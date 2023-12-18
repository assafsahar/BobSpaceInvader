using UnityEngine;
using COD.Core;
using COD.UI;
using static COD.GameLogic.CODGameFlowManager;
using System.Collections;

namespace COD.GameLogic
{
    /// <summary>
    /// the main controller for the player's ship. It handles movement, 
    /// possibly firing weapons, and any other direct control mechanics 
    /// related to the ship's in-game behavior.
    /// </summary>
    public class ShipController : CODMonoBehaviour
    {
        public CODShipGraphics ShipGraphics;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float verticalSpeed;
        [SerializeField] private float upperLimit;
        [SerializeField] private float lowerLimit;
        [SerializeField] private float stateChangeThreshold = 0.05f;
        [SerializeField] private float shieldDuration = 5.0f;
        [SerializeField] private float blinkStart = 1f;
        [SerializeField] private float blinkInterval = 0.2f;

        public bool IsShieldActive => isShieldActive;

        private float stillnessDuration = 0.2f;  // Adjust based on your preference. Represents the time of stillness before changing to straight state.
        private float currentStillnessTime = 0f;
        private float fallingRotationSpeed = 50f;
        public bool isShieldActive = false;
        private SpriteRenderer shipRenderer;
        public bool inputEnabled = true;

        private void Awake()
        {
            shipRenderer = GetComponent<SpriteRenderer>();
            ShipGraphics = GetComponent<CODShipGraphics>();
        }
        private void Start()
        {
            ShipGraphics.InitializeShip(upperLimit, lowerLimit);
        }
        private void OnEnable()
        {
            AddListener(CODEventNames.OnTouchStarted, OnTouchStarted);
            AddListener(CODEventNames.OnTouchStayed, OnTouchStayed);
            AddListener(CODEventNames.OnTouchEnded, OnTouchEnded);
            AddListener(CODEventNames.OnShieldActivated, ActivateShield);
        }

        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnTouchStarted, OnTouchStarted);
            RemoveListener(CODEventNames.OnTouchStayed, OnTouchStayed);
            RemoveListener(CODEventNames.OnTouchEnded, OnTouchEnded);
            RemoveListener(CODEventNames.OnShieldActivated, ActivateShield);
        }

        private void Update()
        {
            if (CODGameLogicManager.Instance.GameFlowManager.CurrentState == GameState.Falling)
            {
                FallDown();
                return; 
            }
        }
        public void EnableInput(bool enable)
        {
            inputEnabled = enable;
        }
        public bool GetInputEnabled()
        {
            return inputEnabled;    
        }
        private void ActivateShield(object unused)
        {
            if (isShieldActive) return;

            isShieldActive = true;
            StartCoroutine(ShieldRoutine());
            UpdateShipAppearanceForShield(true); // Implement this method to change the ship's appearance
        }
        private IEnumerator ShieldRoutine()
        {
            // Invincibility period before blinking starts
            yield return new WaitForSeconds(shieldDuration - blinkStart);

            // Blinking effect
            float endTime = Time.time + blinkStart;
            while (Time.time < endTime)
            {
                shipRenderer.enabled = !shipRenderer.enabled;
                yield return new WaitForSeconds(blinkInterval);
            }

            // End of shield
            shipRenderer.enabled = true;
            isShieldActive = false;
            UpdateShipAppearanceForShield(false); // Revert ship's appearance
        }
        
        private void UpdateShipAppearanceForShield(bool isShieldActive)
        {
            // Example: Change the color of the ship to indicate the shield is active
            if (isShieldActive)
            {
                ShipGraphics.GetComponent<SpriteRenderer>().color = Color.blue; // Example: Blue for shield active
            }
            else
            {
                ShipGraphics.GetComponent<SpriteRenderer>().color = Color.white; // Default color
            }

            // If you have a specific visual effect for the shield, you can enable/disable it here
            // Example:
            // shieldEffect.SetActive(isShieldActive);
        }

        private void FallDown()
        {
            float fallDistance = -Time.deltaTime * verticalSpeed;
            Vector3 newShipPosition = transform.position + new Vector3(0, fallDistance, 0);
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
            Debug.Log("OnTouchStayed called");
            if (!inputEnabled || CODGameLogicManager.Instance.GameFlowManager.CurrentState  == GameState.Falling) {
                return;
            }
            float targetY = Camera.main.ScreenToWorldPoint(new Vector3(0, (float)obj, 0)).y;

            // Calculate distance and direction
            float distanceToTarget = Mathf.Abs(ShipGraphics.transform.position.y - targetY);
            float direction = targetY > ShipGraphics.transform.position.y ? 1 : -1;

            // Update position
            Vector3 newShipPosition = ShipGraphics.transform.position + new Vector3(0, direction * Time.deltaTime * verticalSpeed, 0);

            // If the ship overshot its target, set it directly to the target
            if (direction > 0 && newShipPosition.y > targetY || direction < 0 && newShipPosition.y < targetY)
            {
                newShipPosition.y = targetY;
            }

            // Clamp position
            newShipPosition.y = Mathf.Clamp(newShipPosition.y, lowerLimit, upperLimit);

            ShipGraphics.transform.position = newShipPosition;

            // If distance is minor, accumulate time and consider switching to Straight state.
            if (distanceToTarget < stateChangeThreshold)
            {
                currentStillnessTime += Time.deltaTime;

                if (currentStillnessTime >= stillnessDuration)
                {
                    ShipGraphics.UpdateShipState(ShipState.Straight);
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
            Debug.Log("OnTouchEnded called");
            StopShipMovement();
        }

        private void StopShipMovement()
        {
            ShipGraphics.UpdateShipState(ShipState.Straight);
        }

        private void MoveShipDown()
        {
            ShipGraphics.UpdateShipState(ShipState.Down);
            ShipGraphics.RotateShip(-rotationSpeed);
        }

        private void MoveShipUp()
        {
            ShipGraphics.UpdateShipState(ShipState.Up);
            ShipGraphics.RotateShip(rotationSpeed);
        }

        
    }
}

