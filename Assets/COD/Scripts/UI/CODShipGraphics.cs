using UnityEngine;
using DG.Tweening;

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
        private float duration = 2f;  // Duration for one cycle (move to one side and back)
        private float amplitude = 0.5f;  // The maximum distance the ship will move to each side


        private void Start()
        {
            currentShipState = ShipState.Straight;
            UpdateShipSprite();
            StartSineWaveMotion();
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

        private void StartSineWaveMotion()
        {
            transform.DOKill();  // Kills any previous DOTween animations on this object to avoid overlapping tweens

            // Create the looping sine wave motion
            transform.DOMoveX(transform.position.x + amplitude, duration / 2)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    transform.DOMoveX(transform.position.x - amplitude, duration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);  // Infinite looping back and forth
                });
        }
    }
}
