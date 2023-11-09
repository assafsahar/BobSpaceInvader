using COD.Core;
using COD.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace COD.UI
{
    [System.Serializable]
    public class CollectableSprite
    {
        public CollectableType type;
        public Sprite sprite;
    }

    public class CODCollectableGraphics : CODPoolable
    {

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private List<CollectableSprite> collectableSpriteList;
        private float speed = 5f;

        private Dictionary<CollectableType, Sprite> collectableSprites = new Dictionary<CollectableType, Sprite>();

        private ICollectable collectable;
        private Vector3 leftScreenBoundary;
        private float leftBoundaryX;
        private float offset = 1f;
        private Camera mainCamera;

        private void OnEnable()
        {
            AddListener(CODEventNames.OnSpeedChange, UpdateSpeed);
        }
        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnSpeedChange, UpdateSpeed);
        }

        private void Awake()
        {
            mainCamera = Camera.main;
            foreach (var item in collectableSpriteList)
            {
                collectableSprites.Add(item.type, item.sprite);
            }
            leftScreenBoundary = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
            leftBoundaryX = leftScreenBoundary.x;
            leftBoundaryX -= offset;
        }
        public void Initialize(ICollectable collectable)
        {
            this.collectable = collectable;
            spriteRenderer.sprite = collectableSprites[collectable.Type];
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

        private void Update()
        {
            Vector3 leftMovement = Vector3.left * (speed * Time.deltaTime);
            transform.position += leftMovement;
            if (transform.position.x < leftBoundaryX)
            {
                OutOfBounds();
            }
        }

        private void UpdateSpeed(object obj)
        {
            speed = (float)obj;
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