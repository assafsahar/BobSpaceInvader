using UnityEngine;
using COD.Core;
using COD.GameLogic;
using COD.UI;

public class CODProjectileBehaviour : CODPoolable
{
    [SerializeField] private float speed = 10f;
    private float distanceForDestroy = 30f;

    void Update()
    {
        MoveProjectile();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectable"))
        {
            var collectableComponent = other.GetComponent<CODCollectableGraphics>();
            if (collectableComponent != null)
            {
                var collectable = collectableComponent.GetCollectable(); 
                int scoreForHit = collectable.ScoreValue; 

                CODGameLogicManager.Instance.ScoreManager.ChangeScoreByTagByAmount(COD.Shared.GameEnums.ScoreTags.MainScore, scoreForHit);
                var otherPoolable = other.GetComponent<CODPoolable>();
                if (otherPoolable != null)
                {
                    CODManager.Instance.PoolManager.ReturnPoolable(otherPoolable);
                    collectableComponent.PlayCollectionEffect();
                }
                CODGameLogicManager.Instance.ScoreManager.ChangeScoreByTagByAmount(COD.Shared.GameEnums.ScoreTags.MainScore, scoreForHit); // Example score increment
                ReturnToPool(this);
            }
        }
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        if (transform.position.magnitude > distanceForDestroy)
        {
            ReturnToPool(this);
        }
    }
    public void ReturnToPool(CODPoolable obj)
    {
        CODManager.Instance.PoolManager.ReturnPoolable(obj);
    }
}
