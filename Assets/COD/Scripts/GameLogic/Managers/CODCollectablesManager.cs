using UnityEngine;
using System.Collections.Generic;
using COD.Core;
using COD.UI;
using COD.Shared;
using System;

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

        private float nextSpawnTime;
        private List<CODCollectableGraphics> activeCollectables = new List<CODCollectableGraphics>();
        private List<CollectableType> weightedCollectablesList = new List<CollectableType>
        {
            CollectableType.Coin,
            CollectableType.Coin,
            CollectableType.Coin,
            CollectableType.Coin,
            CollectableType.Coin,
            CollectableType.SuperCoin
        };

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
            SetNextSpawnTime();
        }
        private void Update()
        {
            if (Time.time >= nextSpawnTime)
            {
                SpawnRandomCollectable();
                SetNextSpawnTime();
            }
        }
        public CODCollectableGraphics SpawnCollectable(CollectableType type)
        {
            ICollectable collectable = new CODCollectable(type);
            CODCollectableGraphics instance = Instantiate(prefab);
            instance.Initialize(collectable);
            activeCollectables.Add(instance);
            return instance;
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
            int randomIndex = UnityEngine.Random.Range(0, weightedCollectablesList.Count);
            return weightedCollectablesList[randomIndex];
        }
        private void SetNextSpawnTime()
        {
            nextSpawnTime = Time.time + UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
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
        }

        private void HandleCollectableCollected(CODCollectableGraphics collectableGraphics)
        {
            // Logic for when a collectable is collected
            activeCollectables.Remove(collectableGraphics);
            Destroy(collectableGraphics.gameObject); // Replace with pooling later
        }
    }

}
