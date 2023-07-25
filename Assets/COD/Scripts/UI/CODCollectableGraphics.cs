using COD.Core;
using COD.Shared;
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

    public class CODCollectableGraphics : CODMonoBehaviour
    {

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private List<CollectableSprite> collectableSpriteList;
        [SerializeField] private float speed = 5f;

        private Dictionary<CollectableType, Sprite> collectableSprites = new Dictionary<CollectableType, Sprite>();

        private ICollectable collectable;

        private void Awake()
        {
            foreach (var item in collectableSpriteList)
            {
                collectableSprites.Add(item.type, item.sprite);
            }
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
            transform.position += Vector3.left * speed * Time.deltaTime;
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