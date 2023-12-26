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
        [SerializeField] private CODToastMessage toastMessagePrefab;
        [SerializeField] private Vector2 toastPosition;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float minSpawnTime = 1.0f;
        [SerializeField] private float maxSpawnTime = 5.0f;
        [SerializeField] private float minSpawnY = -2f;
        [SerializeField] private float maxSpawnY = 2f;
        [SerializeField] private int initialPoolSize = 20;
        [SerializeField] private int initialToastPoolSize = 20;
        [SerializeField] private int maxPoolSize = 50;
        [SerializeField] private float magnetDuration = 5.0f;
        [SerializeField] private float shieldDuration = 5.0f;
        [SerializeField] private float capsuleGlowEffectDuration = 0.5f;
        [SerializeField] private float coinGlowEffectDuration = 0.1f;
        [SerializeField]
        private List<WeightedCollectable> weightedCollectables = new List<WeightedCollectable>();

        private ShipController shipController;
        private float originalMinSpawnTime;
        private float originalMaxSpawnTime;
        private float initialShipSpeed;
        private GameState currentGameState;
        private List<CODCollectableGraphics> activeCollectables = new List<CODCollectableGraphics>();
        private Dictionary<CollectableType, Action<CODCollectableGraphics>> collectableHandlers;
        private float explosionAnimationDuration = 1.5f;

        private void Awake()
        {
            shipController = FindObjectOfType<ShipController>();
        }
        private void Start()
        {
            
            if (shipController == null)
            {
                CODDebug.LogException("ShipController not found in scene!");
            }
            originalMinSpawnTime = minSpawnTime;
            originalMaxSpawnTime = maxSpawnTime;
            initialShipSpeed = 5f;

            CODManager.Instance.PoolManager.InitPool(prefab, initialPoolSize, maxPoolSize);
            // Initialize pool for toast messages
            CODManager.Instance.PoolManager.InitPool(toastMessagePrefab, initialToastPoolSize, maxPoolSize);
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
                CODDebug.LogException("No available collectables in pool.");
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
                { CollectableType.Shield, HandleShield },
                { CollectableType.Magnet, HandleMagnet }
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
                CODDebug.LogException($"Handler not found for collectable type: {type}");
            }

            CODManager.Instance.PoolManager.ReturnPoolable(collectableGraphics);
        }
        private void ShowToastMessage(string message, Vector3 startPosition, Vector2 targetPosition)
        {
            CODToastMessage toast = CODManager.Instance.PoolManager.GetPoolable(PoolNames.ScoreToast) as CODToastMessage;
            if (toast != null)
            {
                toast.Initialize(message, startPosition, targetPosition);
            }
        }
        private void HandleCoin(CODCollectableGraphics collectableGraphics)
        {
            int coinValue = collectableGraphics.GetScoreValue();
            UpdateScore(ScoreTags.Coin, 1, coinValue);
            CODGameLogicManager.Instance.UpgradeManager.PlayerUpgradeInventoryData.TotalCoins += coinValue;
            TriggerGlowEffect(coinGlowEffectDuration);
            ShowToastMessage($"+{coinValue} Coins", collectableGraphics.transform.position, toastPosition);
        }

        private void HandleSuperCoin(CODCollectableGraphics collectableGraphics)
        {
            int coinValue = collectableGraphics.GetScoreValue();
            UpdateScore(ScoreTags.SuperCoin, 1, coinValue);
            CODGameLogicManager.Instance.UpgradeManager.PlayerUpgradeInventoryData.TotalCoins += coinValue;
            TriggerGlowEffect(coinGlowEffectDuration);
        }

        private void HandleBomb(CODCollectableGraphics collectableGraphics)
        {
            if (!shipController.IsShieldActive)
            {
                shipController.ShipGraphics.TriggerExplosion();
                InvokeEvent(CODEventNames.OnShipCrash);
                shipController.EnableInput(false);
                StartCoroutine(EndGameAfterExplosion());
            }
        }
        private void HandleEnergy(CODCollectableGraphics collectableGraphics)
        {
            float energyAmount = collectableGraphics.GetEnergyValue();
            CODGameLogicManager.Instance.EnergyManager.AddEnergy(energyAmount);
            TriggerGlowEffect(capsuleGlowEffectDuration);
        }
        private void HandleShield(CODCollectableGraphics collectableGraphics)
        {
            StartCoroutine(ShieldEffectCoroutine());
            TriggerGlowEffect(capsuleGlowEffectDuration);
        }
        private void HandleMagnet(CODCollectableGraphics collectableGraphics)
        {
            StartCoroutine(MagnetEffectCoroutine());
            TriggerGlowEffect(capsuleGlowEffectDuration);
        }
        private IEnumerator MagnetEffectCoroutine()
        {
            ActivateMagnetEffect(true);
            CODCollectableGraphics.isAttractedByMagnet = true;

            yield return new WaitForSeconds(magnetDuration);

            ActivateMagnetEffect(false);
            CODCollectableGraphics.isAttractedByMagnet = false;
        }
        private IEnumerator ShieldEffectCoroutine()
        {
            ActivateShieldEffect(true);

            yield return new WaitForSeconds(shieldDuration);

            ActivateShieldEffect(false);
        }

        private void ActivateMagnetEffect(bool isActive)
        {
            InvokeEvent(CODEventNames.OnMagnetActivated, isActive);
        }
        private void ActivateShieldEffect(bool isActive)
        {
            InvokeEvent(CODEventNames.OnShieldActivated, isActive);
        }

        private void TriggerGlowEffect(float duration)
        {
            if(shipController.GetInputEnabled())
            {
                shipController.ShipGraphics.TriggerGlowEffectForDuration(duration);
            }
        }

        private IEnumerator EndGameAfterExplosion()
        {
            yield return new WaitForSeconds(explosionAnimationDuration);
            shipController.ShipGraphics.ResetGraphicsPostExplosion();
            CODGameLogicManager.Instance.GameFlowManager.EndGame();
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
