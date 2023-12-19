using UnityEngine;
using DG.Tweening;

namespace COD.UI
{
    /// <summary>
    /// Controls the visual state of the player's ship, 
    /// including changing the sprite to match the ship's current 
    /// movement state and handling any visual effects.
    /// </summary>
    public class CODShipGraphics : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer shipSpriteRenderer;
        [SerializeField] private Animator explosionAnimator;
        [SerializeField] private Sprite upSprite;
        [SerializeField] private Sprite straightSprite;
        [SerializeField] private Sprite downSprite;
        [SerializeField] private SpriteRenderer glowSpriteRenderer;
        [SerializeField] private Sprite glowSpriteNormal;
        [SerializeField] private Sprite glowSpriteUp;
        [SerializeField] private Sprite glowSpriteDown;
        [SerializeField] float glowEffectTime = 0.1f;

        private bool explosionIsActive;
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
        private void OnDestroy()
        {
            if (DOTween.IsTweening(transform))
            {
                transform.DOKill(); 
            }
        }

        private void OnDisable()
        {
            transform.DOKill(); 
        }

        public void InitializeShip(float upperLimit, float lowerLimit)
        {
            this.upperLimit = upperLimit;
            this.lowerLimit = lowerLimit;
        }
        public bool ExplosionIsActive()
        {
            return explosionIsActive;
        }
        public void TriggerExplosion()
        {
            explosionIsActive = true;
            shipSpriteRenderer.enabled = false;
            explosionAnimator.gameObject.SetActive(true);
            explosionAnimator.SetBool("IsExplode", true);
        }
        public void ResetGraphicsPostExplosion()
        {
            explosionAnimator.SetBool("IsExplode", false);
            explosionAnimator.gameObject.SetActive(false);
            explosionIsActive = false;
        }
        public void UpdateShipState(ShipState newState)
        {
            //CODDebug.Log($"Ship state changing from {currentShipState} to {newState}");
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
        public void TriggerGlowEffect()
        {
            // Choose the correct glow sprite based on the ship's state
            Sprite currentGlowSprite = currentShipState switch
            {
                ShipState.Up => glowSpriteUp,
                ShipState.Down => glowSpriteDown,
                _ => glowSpriteNormal
            };

            glowSpriteRenderer.sprite = currentGlowSprite;
            glowSpriteRenderer.enabled = true;
            DOVirtual.DelayedCall(glowEffectTime, () => glowSpriteRenderer.enabled = false); // Example: 0.5 seconds duration
        }

        private void UpdateShipSprite()
        {
            Sprite newSprite = GetShipSprite(currentShipState);
            //CODDebug.Log($"Updating ship sprite to state: {currentShipState}");
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
            if (transform)
            {
                transform.DOKill();  // Kills any previous DOTween animations on this object to avoid overlapping tweens
            }
            

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
