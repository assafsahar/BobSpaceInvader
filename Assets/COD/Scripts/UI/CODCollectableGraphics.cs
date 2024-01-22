using COD.Core;
using COD.Shared;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace COD.UI
{
    /// <summary>
    /// the following data structure maps the types of collectables 
    /// to their visual representations
    /// </summary>
    [System.Serializable]
    public class CollectableSprite
    {
        public CollectableType type;
        public Sprite sprite;
    }
    /// <summary>
    /// Handles the visual representation of collectable items in the game. 
    /// It's responsible for associating the correct sprite to each collectable type,
    /// moving the collectables across the screen, and handling interactions when 
    /// the player collects an item.
    /// </summary>
    public class CODCollectableGraphics : CODPoolable
    {

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private List<CollectableSprite> collectableSpriteList;
        [SerializeField] float attractSpeed = 10f;
        [SerializeField] private float blinkDuration = 1f;
        [SerializeField] private float minAlpha = 0.2f;
        [SerializeField] private float maxAlpha = 1f;
        [SerializeField] private GameObject collectableParticlePrefab;

        private float speed;
        private ICollectable collectable;
        private Vector3 leftScreenBoundary;
        private float leftBoundaryX;
        private float offset = 1f;
        private Camera mainCamera;
        //private bool isAttractedByMagnet = false;
        private Vector3 shipPosition;        

        private Dictionary<CollectableType, Sprite> collectableSprites = new Dictionary<CollectableType, Sprite>();

        public static bool isAttractedByMagnet { get; set; }

        private void OnEnable()
        {
            AddListener(CODEventNames.OnSpeedChange, UpdateSpeed);
            AddListener(CODEventNames.OnMagnetActivated, HandleMagnetActivation);
            AddListener(CODEventNames.OnShipPositionUpdated, UpdateShipPosition);
        }
        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnSpeedChange, UpdateSpeed);
            RemoveListener(CODEventNames.OnMagnetActivated, HandleMagnetActivation);
            RemoveListener(CODEventNames.OnShipPositionUpdated, UpdateShipPosition);
        }

        private void Awake()
        {
            isAttractedByMagnet = false;
            mainCamera = Camera.main;
            foreach (var item in collectableSpriteList)
            {
                collectableSprites.Add(item.type, item.sprite);
            }
            leftScreenBoundary = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
            leftBoundaryX = leftScreenBoundary.x;
            leftBoundaryX -= offset;
        }
        private void Update()
        {

            if (isAttractedByMagnet && (collectable.Type == CollectableType.Coin || collectable.Type == CollectableType.SuperCoin))
            {
                MoveTowardsShip();
            }
            else
            {
                Vector3 leftMovement = Vector3.left * (speed * Time.deltaTime);
                transform.position += leftMovement;
            }
            if (transform.position.x < leftBoundaryX)
            {
                OutOfBounds();
            }
        }
        public void Initialize(ICollectable collectable)
        {
            this.collectable = collectable;
            spriteRenderer.sprite = collectableSprites[collectable.Type];
            if (ShouldBlink(collectable.Type)) 
            {
                StartBlinkEffect();
            }
        }
        public void PlayCollectionEffect()
        {
            if (collectableParticlePrefab)
            {
                GameObject effect = Instantiate(collectableParticlePrefab, transform.position, Quaternion.identity);
                effect.gameObject.GetComponentInParent<ParticleSystem>().Play();
            }
        }
        public CollectableType GetCollectableType()
        {
            return collectable.Type;
        }
        public float GetEnergyValue()
        {
            if (collectable != null)
            {
                return collectable.EnergyValue;
            }
            return 0; // default value if there's an issue
        }
        public int GetScoreValue()
        {
            if (collectable != null)
            {
                return collectable.ScoreValue;
            }
            return 0; // default value if there's an issue
        }
        
        public override void OnTakenFromPool()
        {
            base.OnTakenFromPool();
            ResetCollectableState();
        }
        private bool ShouldBlink(CollectableType type)
        {
            return type == CollectableType.Energy; 
        }
        private void StartBlinkEffect()
        {
            Sequence blinkSequence = DOTween.Sequence();
            blinkSequence.Append(spriteRenderer.DOFade(minAlpha, blinkDuration / 2).SetEase(Ease.InSine))
                         .Append(spriteRenderer.DOFade(maxAlpha, blinkDuration / 2).SetEase(Ease.OutSine))
                         .SetLoops(-1, LoopType.Yoyo); // Infinite loop with Yoyo effect (back and forth)
        }
        private void UpdateShipPosition(object positionObj)
        {
            if (positionObj is Vector3 position)
            {
                shipPosition = position;
            }
        }
        private void MoveTowardsShip()
        {
            transform.position = Vector3.MoveTowards(transform.position, shipPosition, attractSpeed * Time.deltaTime);
        }
        private void HandleMagnetActivation(object isActiveObj)
        {
            if (isActiveObj is bool isActive)
            {
                isAttractedByMagnet = isActive;
            }
        }
        private void ResetCollectableState()
        {
            gameObject.SetActive(true);
        }
        private void UpdateSpeed(object obj)
        {
            if(!isAttractedByMagnet)
            {
                speed = (float)obj + collectable.MovementSpeed;
            }
        }

        private void OutOfBounds()
        {
            CODManager.Instance.PoolManager.ReturnPoolable(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                InvokeEvent(CODEventNames.OnCollectableCollected, this);
                // Logic for the effect of the collectable on the player, if any
            }
        }
    }
}