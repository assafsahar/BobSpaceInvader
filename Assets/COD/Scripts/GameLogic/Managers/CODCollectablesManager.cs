using UnityEngine;
using System.Collections.Generic;
using COD.Core;
using COD.UI;
using COD.Shared;
using System;
using static COD.Shared.GameEnums;
using System.Collections;
using static COD.GameLogic.CODGameFlowManager;

namespace COD.GameLogic
{
    /// <summary>
    /// This one is tasked with the overall management of collectable items 
    /// in the game, including spawning and handling their collection by the player
    /// </summary>
    public class CODCollectablesManager : CODMonoBehaviour
    {
        [SerializeField] private CODCollectableGraphics prefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float minSpawnTime = 1.0f;
        [SerializeField] private float maxSpawnTime = 5.0f;
        [SerializeField] private float minSpawnY = -2f;
        [SerializeField] private float maxSpawnY = 2f;
        [SerializeField] private int initialPoolSize = 20;
        [SerializeField] private int maxPoolSize = 50;
        [SerializeField] private int energyDecreaseRate = 1;
        [SerializeField]
        private List<WeightedCollectable> weightedCollectables = new List<WeightedCollectable>();

        private ShipController shipController;
        private float originalMinSpawnTime;
        private float originalMaxSpawnTime;
        private float initialShipSpeed;
        private GameState currentGameState;
        private float nextSpawnTime;
        private List<CODCollectableGraphics> activeCollectables = new List<CODCollectableGraphics>();
        private Dictionary<CollectableType, Action<CODCollectableGraphics>> collectableHandlers;

        private void Start()
        {
            shipController = FindObjectOfType<ShipController>();
            if (shipController == null)
            {
                Debug.LogError("ShipController not found in scene!");
            }
            originalMinSpawnTime = minSpawnTime;
            originalMaxSpawnTime = maxSpawnTime;
            initialShipSpeed = 5f;

            CODManager.Instance.PoolManager.InitPool(prefab, initialPoolSize, maxPoolSize);
            InitCollectableHandlers();
            StartCoroutine(SpawnRoutine());
        }

        private void OnEnable()
        {
            AddListener(CODEventNames.OnSpeedChange, AdjustSpawnRate);
            AddListener(CODEventNames.OnGameStateChange, UpdateGameState);
        }
        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnSpeedChange, AdjustSpawnRate);
            RemoveListener(CODEventNames.OnGameStateChange, UpdateGameState);
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
        private void UpdateGameState(object gameStateObj)
        {
            if (gameStateObj is GameState gameState)
            {
                currentGameState = gameState;
            }
        }
        private void AdjustSpawnRate(object speedObject)
        {
            if (speedObject is float speed)
            {
                float speedFactor = speed / initialShipSpeed;

                // The higher the speedFactor, the shorter the spawn time
                minSpawnTime = Mathf.Max(originalMinSpawnTime / speedFactor, 0.05f); 
                maxSpawnTime = Mathf.Max(originalMaxSpawnTime / speedFactor, 0.3f); 

                //Debug.Log($"Updated spawn times - Min: {minSpawnTime}, Max: {maxSpawnTime}");
            }
        }
        private void InitCollectableHandlers()
        {
            collectableHandlers = new Dictionary<CollectableType, Action<CODCollectableGraphics>>
            {
                { CollectableType.Coin, HandleCoin },
                { CollectableType.SuperCoin, HandleSuperCoin },
                { CollectableType.Bomb, HandleBomb },
                { CollectableType.Energy, HandleEnergy },
                { CollectableType.Shield, HandleShield }
            };
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(minSpawnTime, maxSpawnTime));
                if (currentGameState == GameState.Playing)
                {
                    SpawnRandomCollectable();
                }
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

        public void HandleCollectableCollected(CODCollectableGraphics collectableGraphics)
        {
            // This function can be called from CODShipCollisionHandler
            activeCollectables.Remove(collectableGraphics);

            CollectableType type = collectableGraphics.GetCollectableType();
            if (collectableHandlers.ContainsKey(type))
            {
                collectableHandlers[type].Invoke(collectableGraphics);
            }
            else
            {
                Debug.LogError($"Handler not found for collectable type: {type}");
            }

            CODManager.Instance.PoolManager.ReturnPoolable(collectableGraphics);
        }
        private void HandleCoin(CODCollectableGraphics collectableGraphics)
        {
            int coinValue = collectableGraphics.GetScoreValue();
            UpdateScore(ScoreTags.Coin, 1, coinValue);
            CODGameLogicManager.Instance.UpgradeManager.PlayerUpgradeInventoryData.TotalCoins += coinValue;
            shipController.ShipGraphics.TriggerGlowEffect();
        }

        private void HandleSuperCoin(CODCollectableGraphics collectableGraphics)
        {
            int coinValue = collectableGraphics.GetScoreValue();
            UpdateScore(ScoreTags.SuperCoin, 1, coinValue);
            CODGameLogicManager.Instance.UpgradeManager.PlayerUpgradeInventoryData.TotalCoins += coinValue;
            shipController.ShipGraphics.TriggerGlowEffect();
        }

        private void HandleBomb(CODCollectableGraphics collectableGraphics)
        {
            if (!shipController.IsShieldActive)
            {
                CODGameLogicManager.Instance.GameFlowManager.EndGame();
            }
        }
        private void HandleEnergy(CODCollectableGraphics collectableGraphics)
        {
            float energyAmount = collectableGraphics.GetEnergyValue();
            CODGameLogicManager.Instance.EnergyManager.AddEnergy(energyAmount);
            shipController.ShipGraphics.TriggerGlowEffect();
        }
        private void HandleShield(CODCollectableGraphics collectableGraphics)
        {
            // Logic to make the ship invincible
            InvokeEvent(CODEventNames.OnShieldActivated);
            shipController.ShipGraphics.TriggerGlowEffect();
        }

        private void UpdateScore(ScoreTags tag, int count, int scoreValue)
        {
            if (!CODGameLogicManager.Instance.ScoreManager.IsInitialized)
            {
                Debug.LogWarning("Trying to access ScoreManager before it's initialized!");
            }
            CODGameLogicManager.Instance.ScoreManager.ChangeScoreByTagByAmount(tag, count);
            CODGameLogicManager.Instance.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.MainScore, scoreValue);
        }
    }
}
