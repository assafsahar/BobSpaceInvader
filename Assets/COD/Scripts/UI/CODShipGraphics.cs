using UnityEngine;

namespace COD.UI
{
    public class CODShipGraphics : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer shipSpriteRenderer;
        [SerializeField] private Sprite upSprite;
        [SerializeField] private Sprite straightSprite;
        [SerializeField] private Sprite downSprite;

        private ShipState currentShipState;
        private float upperLimit;
        private float lowerLimit;

        private void Start()
        {
            currentShipState = ShipState.Straight;
            UpdateShipSprite();
        }

        public void InitializeShip(float upperLimit, float lowerLimit)
        {
            this.upperLimit = upperLimit;
            this.lowerLimit = lowerLimit;
        }

        public void UpdateShipState(ShipState newState)
        {
            currentShipState = newState;
            UpdateShipSprite();
            if (currentShipState == ShipState.Straight)
            {
                transform.rotation = Quaternion.identity; // Set rotation to zero for straight state
            }
        }

        public void RotateShip(float rotationSpeed)
        {
            if (currentShipState == ShipState.Straight)
            {
                transform.rotation = Quaternion.identity;
                return;
            }
            transform.rotation = Quaternion.Euler(0, 0, rotationSpeed);
        }

        private void UpdateShipSprite()
        {
            Sprite newSprite = GetShipSprite(currentShipState);
            shipSpriteRenderer.sprite = newSprite;
        }

        private Sprite GetShipSprite(ShipState state)
        {
            switch (state)
            {
                case ShipState.Up:
                    return upSprite;
                case ShipState.Straight:
                    return straightSprite;
                case ShipState.Down:
                    return downSprite;
                default:
                    return null;
            }
        }

        private void ClampShipPosition()
        {
            float shipY = transform.position.y;
            float clampedY = Mathf.Clamp(shipY, lowerLimit, upperLimit);
            transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
        }
    }
}
