using UnityEngine;
using System.Collections.Generic;
using COD.Core;
using COD.UI;
using COD.Shared;
using System;
using static COD.Shared.GameEnums;
using System.Collections;

namespace COD.GameLogic
{
    public class CODCollectablesManager : CODMonoBehaviour
    {
        [SerializeField] private CODCollectableGraphics prefab;
        [SerializeField] private Transform spawnPoint; // The point where collectables spawn.
        [SerializeField] private float minSpawnTime = 1.0f; 
        [SerializeField] private float maxSpawnTime = 5.0f;
        [SerializeField] private float minSpawnY = -2f; 
        [SerializeField] private float maxSpawnY = 2f;
        [SerializeField] private int initialPoolSize = 20;
        [SerializeField] private int maxPoolSize = 50;
        [SerializeField]
        private List<WeightedCollectable> weightedCollectables = new List<WeightedCollectable>();

        private float nextSpawnTime;
        private List<CODCollectableGraphics> activeCollectables = new List<CODCollectableGraphics>();
        

        private void OnEnable()
        {
            AddListener(CODEventNames.OnCollectableCollected, HandleCollectableCollected);
        }
        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnCollectableCollected, HandleCollectableCollected);
        }
        private void Start()
        {
            CODManager.Instance.PoolManager.InitPool(prefab, initialPoolSize, maxPoolSize);
            StartCoroutine(SpawnRoutine());
        }
        
        public CODCollectableGraphics SpawnCollectable(CollectableType type)
        {
            ICollectable collectable = new CODCollectable(type);
            CODCollectableGraphics instance = CODManager.Instance.PoolManager.GetPoolable(PoolNames.Collectable) as CODCollectableGraphics;
            if (instance == null)
            {
                Debug.LogError("No available collectables in pool.");
                return null;
            }
            instance.Initialize(collectable);
            activeCollectables.Add(instance);          
            return instance;
        }
        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                Debug.Log("SpawnRoutine");
                yield return new WaitForSeconds(UnityEngine.Random.Range(minSpawnTime, maxSpawnTime));
                SpawnRandomCollectable();
            }
        }
        private void SpawnRandomCollectable()
        {
            AdjustSpawnPointY();
            CollectableType randomType = GetRandomCollectableType();
            CODCollectableGraphics instance = SpawnCollectable(randomType);
            instance.transform.position = spawnPoint.position;
        }

        private void AdjustSpawnPointY()
        {
            float randomY = UnityEngine.Random.Range(minSpawnY, maxSpawnY);
            spawnPoint.position = new Vector3(spawnPoint.position.x, randomY, spawnPoint.position.z);
        }

        private CollectableType GetRandomCollectableType()
        {
            int totalWeight = 0;
            foreach (var item in weightedCollectables)
            {
                totalWeight += item.weight;
            }

            int randomWeightPoint = UnityEngine.Random.Range(0, totalWeight);
            foreach (var item in weightedCollectables)
            {
                if (randomWeightPoint < item.weight)
                    return item.collectableType;

                randomWeightPoint -= item.weight;
            }

            return CollectableType.Coin; // default type if all else fails
        }

        private void HandleCollectableCollected(object data)
        {
            if (data is CODCollectableGraphics collectableGraphics)
            {
                Collect(collectableGraphics);
                // More logic here for the effect of the collectable on the player
            }
        }

        private void Collect(CODCollectableGraphics collectableGraphics)
        {
            Debug.Log("Collected "+ collectableGraphics);
            
            HandleCollectableCollected(collectableGraphics);
            //UpdateScoreBasedOnCollectable(collectableGraphics);
        }

        private void HandleCollectableCollected(CODCollectableGraphics collectableGraphics)
        {
            // Logic for when a collectable is collected
            activeCollectables.Remove(collectableGraphics);
            UpdateScoreBasedOnCollectable(collectableGraphics);
            CODManager.Instance.PoolManager.ReturnPoolable(collectableGraphics);
        }

        private void UpdateScoreBasedOnCollectable(CODCollectableGraphics collectableGraphics)
        {
            ScoreTags tag;
            int scoreValue = collectableGraphics.GetScoreValue();
            switch (collectableGraphics.GetCollectableType())
            {
                case CollectableType.Coin:
                    tag = ScoreTags.Coin;
                    break;
                case CollectableType.SuperCoin:
                    tag = ScoreTags.SuperCoin;
                    break;
                case CollectableType.Bomb:
                    tag = ScoreTags.Bomb;
                    break;
                default:
                    Debug.LogError("Unrecognized CollectableType");
                    return;
            }

            CODGameLogicManager.Instance.ScoreManager.ChangeScoreByTagByAmount(tag, 1);
            CODGameLogicManager.Instance.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.MainScore, scoreValue);
        }
    }

}
